/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2022-5-30
 */
using SanteDB.Core.Interfaces;
using SanteDB.Core.Model;
using SanteDB.Core.Model.Collection;
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.DataTypes;
using SanteDB.Core.Model.Entities;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Model.Roles;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Test
{
    /// <summary>
    /// Dummy data persistence service
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DummyPatientDataPersistenceService : IRepositoryService<Patient>, IAliasProvider
    {
        public String ServiceName => "Fake Patient Repository";

        // Sample Patients
        private IEnumerable<Patient> m_patients;

        /// <summary>
        /// Loads patient data from resource
        /// </summary>
        public DummyPatientDataPersistenceService()
        {
            this.LoadPatientData();
        }

        public IQueryResultSet<Patient> Find(Expression<Func<Patient, bool>> query)
        {
            return new MemoryQueryResultSet<Patient>(this.m_patients.Where(query.Compile()));
        }

        public Patient Get(Guid key)
        {
            return this.m_patients.FirstOrDefault(o => o.Key.Value == key);
        }

        public Patient Get(Guid key, Guid versionKey)
        {
            return this.m_patients.FirstOrDefault(o => o.Key.Value == key);
        }

        public IEnumerable<ComponentAlias> GetAlias(string name)
        {
            switch (name.ToLower())
            {
                case "sam":
                    return new ComponentAlias[] {
                        new ComponentAlias("samantha", 1.0)
                    };

                case "samantha":
                    return new ComponentAlias[] {
                        new ComponentAlias("sam", 1.0)
                    };

                case "samira":
                    return new ComponentAlias[] {
                        new ComponentAlias("sam", 0.5)
                    };

                case "william":
                    return new ComponentAlias[] {
                        new ComponentAlias("bill", 1.0),
                        new ComponentAlias("will", 1.0)
                    };

                case "robert":
                    return new ComponentAlias[]
                    {
                        new ComponentAlias("rob", 1.0),
                        new ComponentAlias("bob", 1.0)
                    };
            }
            return null;
        }

        public Patient Insert(Patient data)
        {
            throw new NotImplementedException();
        }

        public Patient Delete(Guid key)
        {
            throw new NotImplementedException();
        }

        public Patient Save(Patient data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load patient
        /// </summary>
        private void LoadPatientData()
        {
            var placeRefs = new Dictionary<String, Guid>()
            {
                { "Clinic1", Guid.Parse("049df673-384f-4dcf-91b2-53b232a5e277") },
                { "Hospital1", Guid.Parse("9e721f9e-489c-41aa-8502-ffc6668984b2") },
                { "Hospital2", Guid.Parse("86a51e4f-5fa7-415d-940e-18b25eec68a9") },
                { "Hospital3", Guid.Parse("8f4c9122-7661-4c7c-9069-4a572a965b3f") },
                { "Home", Guid.Parse("07a4595c-8076-4dc3-bdfb-9fc3345757a1") },
                { "Clinic2", Guid.Parse("339ac8da-48ed-4ee7-998f-8f29d9edfa76") },
            };

            var patients = new List<Patient>();
            using (var sr = new StreamReader(typeof(DummyPatientDataPersistenceService).Assembly.GetManifestResourceStream("SanteDB.Matcher.Test.Resources.Patients.csv")))
            {
                sr.ReadLine(); // header
                while (!sr.EndOfStream)
                {
                    var data = sr.ReadLine().Split(',');
                    var pat = new Patient()
                    {
                        Key = Guid.NewGuid(),
                        Identifiers = new List<EntityIdentifier>()
                        {
                            new EntityIdentifier(new IdentityDomain("MRN", "MRN", "1.2.3.4"), data[0]),
                            new EntityIdentifier(new IdentityDomain("HIN", "Health Insurance", "1.2.3.4.56"), data[6])
                        },
                        GenderConceptKey = Guid.Parse(data[2].ToLower() == "m" ? "f4e3a6bb-612e-46b2-9f77-ff844d971198" :
                            data[2].ToLower() == "f" ? "094941e9-a3db-48b5-862c-bc289bd7f86c" :
                            "091f27e8-e592-4e81-b5d7-338b7749d8b8"),
                        StatusConcept = new Concept()
                        {
                            Key = StatusKeys.Active,
                            Mnemonic = "ACTIVE"
                        },
                        StatusConceptKey = StatusKeys.Active,
                        DateOfBirth = DateTime.Parse(data[1]),
                        Names = new List<Core.Model.Entities.EntityName>()
                        {
                            new EntityName(NameUseKeys.OfficialRecord, data[3], data[4], data[5]),
                            new EntityName(NameUseKeys.Pseudonym, data[13])
                        },
                        Addresses = new List<EntityAddress>()
                        {
                            new EntityAddress(AddressUseKeys.HomeAddress, data[7], data[8], data[9], data[10], data[11])
                        },
                        Relationships = new List<EntityRelationship>()
                        {
                            !String.IsNullOrEmpty(data[12]) ? new EntityRelationship(EntityRelationshipTypeKeys.Birthplace, placeRefs[data[12]]) : null,
                            new EntityRelationship(EntityRelationshipTypeKeys.Mother, new Person()
                            {
                                Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, data[14], data[15]) }
                            })
                        },
                        MultipleBirthOrder = !String.IsNullOrEmpty(data[16]) ? (Int32?)Int32.Parse(data[16]) : null
                    };

                    patients.Add(pat);
                }
            }

            this.m_patients = patients.AsParallel().Select(p => p.LoadConcepts()).ToList();
        }

        public void AddAlias(string name, string alias, double weight)
        {
            throw new NotImplementedException();
        }

        public void RemoveAlias(string name, string alias)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IEnumerable<ComponentAlias>> GetAllAliases(String filter, int offset, int? count, out int totalResults)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> query, int offset, int? count, out int totalResults, params ModelSort<Patient>[] orderBy)
        {
            throw new NotImplementedException();
        }
    }
}