using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SanteDB.Core;
using SanteDB.Core.Data;
using SanteDB.Core.Interfaces;
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.DataTypes;
using SanteDB.Core.Model.EntityLoader;
using SanteDB.Core.Model.Roles;
using SanteDB.Core.Services;
using SanteDB.Matcher.Matchers;
using SanteDB.Persistence.ADO.Test.Core;

namespace SanteDB.Matcher.Test
{
    [TestClass]
    public class MatcherTest
    {

        /// <summary>
        /// Initialize the test class
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
               "DataDirectory",
               Path.Combine(context.TestDeploymentDir, string.Empty));

            EntitySource.Current = new EntitySource(new RepositoryEntitySource());

            // Register the AuditAdoPersistenceService
            TestApplicationContext.TestAssembly = typeof(MatcherTest).Assembly;
            TestApplicationContext.Initialize(context.DeploymentDirectory);

            (ApplicationServiceContext.Current as IServiceManager).AddServiceProvider(typeof(DummyMatchConfigurationProvider)); // Sec repo service is for get user name implementation
            (ApplicationServiceContext.Current as IServiceManager).AddServiceProvider(typeof(DummyConceptRepositoryService));
            (ApplicationServiceContext.Current as IServiceManager).AddServiceProvider(typeof(DummyPatientDataPersistenceService));
            (ApplicationServiceContext.Current as IServiceManager).AddServiceProvider(typeof(ProbabalisticRecordMatchingService));

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
        [TestMethod]
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
            var blocks = matchService.Block<Patient>(patient, "test.dob_and_gender_no_class");
            Assert.AreEqual(2, blocks.Count());
        }


        /// <summary>
        /// Test that matching patients are blocked
        /// </summary>
        [TestMethod]
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
            var blocks = matchService.Block(patient, "test.dob_and_gender_with_class");
            Assert.AreEqual(5, blocks.Count());
            var output = matchService.Classify(patient, blocks, "test.dob_and_gender_with_class");
            Assert.AreEqual(5, output.Count());
        }
    }
}
