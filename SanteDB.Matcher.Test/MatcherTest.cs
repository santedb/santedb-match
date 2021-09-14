/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2021-8-5
 */
using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SanteDB.Core;
using SanteDB.Core.Data;
using SanteDB.Core.Interfaces;
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.DataTypes;
using SanteDB.Core.Model.EntityLoader;
using SanteDB.Core.Model.Roles;
using SanteDB.Core.Matching;
using SanteDB.Core.TestFramework;
using SanteDB.Matcher.Matchers;

namespace SanteDB.Matcher.Test
{
    [TestFixture(Category = "Matching")]
    public class MatcherTest
    {

        /// <summary>
        /// Initialize the test class
        /// </summary>
        [SetUp]
        public void ClassInitialize()
        {
            AppDomain.CurrentDomain.SetData(
               "DataDirectory",
               Path.Combine(TestContext.CurrentContext.TestDirectory, string.Empty));

            EntitySource.Current = new EntitySource(new PersistenceEntitySource());

            // Register the AuditAdoPersistenceService
            TestApplicationContext.TestAssembly = typeof(MatcherTest).Assembly;
            TestApplicationContext.Initialize(TestContext.CurrentContext.TestDirectory);

            ApplicationServiceContext.Current.GetService<IServiceManager>().AddServiceProvider(typeof(DummyMatchConfigurationProvider)); // Sec repo service is for get user name implementation
            ApplicationServiceContext.Current.GetService<IServiceManager>().AddServiceProvider(typeof(DummyConceptRepositoryService));
            ApplicationServiceContext.Current.GetService<IServiceManager>().AddServiceProvider(typeof(DummyPatientDataPersistenceService));
            ApplicationServiceContext.Current.GetService<IServiceManager>().AddServiceProvider(typeof(WeightedRecordMatchingService));

            // Start the daemon services
            if (!ApplicationServiceContext.Current.IsRunning)
            {
                //adoPersistenceService.Start();
                TestApplicationContext.Current.Start();
                ApplicationServiceContext.Current = ApplicationServiceContext.Current;
            }
        }

        /// <summary>
        /// Test that matching patients are blocked
        /// </summary>
        [Test]
        public void ShouldBlockMatchingPatients()
        {
            var matchService = ApplicationServiceContext.Current.GetService<IRecordMatchingService>();

            // We will block patients, there are no patients that match this record (last name doesn't sound like)
            var patient = new Patient()
            {
                DateOfBirth = DateTime.Parse("1986-10-01"),
                GenderConceptKey = Guid.Parse("094941e9-a3db-48b5-862c-bc289bd7f86c"),
                Identifiers = new System.Collections.Generic.List<Core.Model.DataTypes.EntityIdentifier>()
                {
                    new Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("HIN", "Health Insurance", "1.2.3.4.56"), "993642-49382938-1986S")
                },
                Names = new System.Collections.Generic.List<Core.Model.Entities.EntityName>()
                {
                    new Core.Model.Entities.EntityName(NameUseKeys.OfficialRecord, "Smit", "Chloey")
                }
            };
            patient = patient.LoadConcepts();
            var blocks = matchService.Block<Patient>(patient, "test.dob_and_gender_no_class", null);
            Assert.AreEqual(2, blocks.Count());
        }


        /// <summary>
        /// Test that matching patients are blocked
        /// </summary>
        [Test]
        public void ShouldBlockFuzzyPatients()
        {
            var matchService = ApplicationServiceContext.Current.GetService<IRecordMatchingService>();

            // We will block patients, there are no patients that match this record (last name doesn't sound like)
            var patient = new Patient()
            {
                DateOfBirth = DateTime.Parse("1986-10-01"),
                GenderConceptKey = Guid.Parse("094941e9-a3db-48b5-862c-bc289bd7f86c"),
                Identifiers = new System.Collections.Generic.List<Core.Model.DataTypes.EntityIdentifier>()
                {
                    new Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("HIN", "Health Insurance", "1.2.3.4.56"), "993644-49382738-1986S")
                },
                Names = new System.Collections.Generic.List<Core.Model.Entities.EntityName>()
                {
                    new Core.Model.Entities.EntityName(NameUseKeys.OfficialRecord, "Ross", "Sam")
                }
            };
            patient = patient.LoadConcepts();
            var blocks = matchService.Block(patient, "test.dob_and_gender_with_class", null);
            Assert.AreEqual(5, blocks.Count());
            var output = matchService.Classify(patient, blocks, "test.dob_and_gender_with_class");
            Assert.AreEqual(5, output.Count());
        }


        /// <summary>
        /// Test that when a HIN matches, and name matches, but the DOB does not match the patient is identified as a match
        /// </summary>
        [Test]
        public void ComplexByHinShouldResultInMatch()
        {
            var matchService = ApplicationServiceContext.Current.GetService<IRecordMatchingService>();

            // We will block patients, there are no patients that match this record (last name doesn't sound like)
            var patient = new Patient()
            {
                DateOfBirth = DateTime.Parse("1985-02-06"),
                GenderConceptKey = Guid.Parse("f4e3a6bb-612e-46b2-9f77-ff844d971198"),
                Identifiers = new System.Collections.Generic.List<Core.Model.DataTypes.EntityIdentifier>()
                {
                    new Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("HIN", "Health Insurance", "1.2.3.4.56"), "496447-080506-1985S")
                },
                Names = new System.Collections.Generic.List<Core.Model.Entities.EntityName>()
                {
                    new Core.Model.Entities.EntityName(NameUseKeys.OfficialRecord, "Smith", "Lucas")
                },
                Addresses = new System.Collections.Generic.List<Core.Model.Entities.EntityAddress>()
                {
                    new Core.Model.Entities.EntityAddress(AddressUseKeys.Direct, "483 Some Different Street", "Hamilton", "ON", "CA", "L8K5NN")
                }
            };
            patient = patient.LoadConcepts();
            var blocks = matchService.Block(patient, "test.complex", null);
            Assert.AreEqual(113, blocks.Count());
            var output = matchService.Classify(patient, blocks, "test.complex");
            Assert.AreEqual(1, output.Where(o => o.Classification == RecordMatchClassification.Match).Count());

        }

        /// <summary>
        /// Test that when a HIN matches, and name matches, but the DOB does not match the patient is identified as a match
        /// </summary>
        [Test]
        public void ComplexByNameOnlyShouldMatch()
        {
            var matchService = ApplicationServiceContext.Current.GetService<IRecordMatchingService>();

            // We will block patients, there are no patients that match this record (last name doesn't sound like)
            var patient = new Patient()
            {
                DateOfBirth = DateTime.Parse("1985-02-06"),
                GenderConceptKey = Guid.Parse("094941e9-a3db-48b5-862c-bc289bd7f86c"),
                Identifiers = new System.Collections.Generic.List<Core.Model.DataTypes.EntityIdentifier>()
                {
                    new Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("HIN", "Health Insurance", "1.2.3.4.56"), "496447-080506-1985D")
                },
                Names = new System.Collections.Generic.List<Core.Model.Entities.EntityName>()
                {
                    new Core.Model.Entities.EntityName(NameUseKeys.OfficialRecord, "Murphy", "Savannah")
                },
                Addresses = new System.Collections.Generic.List<Core.Model.Entities.EntityAddress>()
                {
                    new Core.Model.Entities.EntityAddress(AddressUseKeys.Direct, "483 Some Different Street", "Hamilton", "ON", "CA", "L8K5NN")
                },
                Relationships = new System.Collections.Generic.List<Core.Model.Entities.EntityRelationship>()
                {
                    new Core.Model.Entities.EntityRelationship(EntityRelationshipTypeKeys.Mother, new Core.Model.Entities.Entity()
                    {
                        Names = new System.Collections.Generic.List<Core.Model.Entities.EntityName>()
                        {
                            new Core.Model.Entities.EntityName(NameUseKeys.OfficialRecord, "Morphy", "Monique")
                        }
                    }),
                    new Core.Model.Entities.EntityRelationship(EntityRelationshipTypeKeys.Birthplace, Guid.Parse("8f4c9122-7661-4c7c-9069-4a572a965b3f"))
                }
            };
            patient = patient.LoadConcepts();
            var blocks = matchService.Block(patient, "test.complex", null);
            Assert.AreEqual(140, blocks.Count());
            var output = matchService.Classify(patient, blocks, "test.complex");
            Assert.AreEqual(3, output.Where(o => o.Classification == RecordMatchClassification.Match).Count());
            Assert.AreEqual(9, output.Where(o => o.Classification == RecordMatchClassification.Probable).Count());
        }
    }
}
