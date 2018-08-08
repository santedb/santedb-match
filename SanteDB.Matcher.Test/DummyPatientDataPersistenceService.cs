using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.DataTypes;
using SanteDB.Core.Model.Entities;
using SanteDB.Core.Model.Roles;
using System;
using System.Collections.Generic;
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
    public class DummyPatientDataPersistenceService : IDataPersistenceService<Patient>
    {

        // Sample Patients
        private IEnumerable<Patient> m_patients;

        /// <summary>
        /// Loads patient data from resource
        /// </summary>
        public DummyPatientDataPersistenceService()
        {
            this.LoadPatientData();
        }

        public event EventHandler<PrePersistenceEventArgs<Patient>> Inserting;
        public event EventHandler<PostPersistenceEventArgs<Patient>> Inserted;
        public event EventHandler<PrePersistenceEventArgs<Patient>> Updating;
        public event EventHandler<PostPersistenceEventArgs<Patient>> Updated;
        public event EventHandler<PrePersistenceEventArgs<Patient>> Obsoleting;
        public event EventHandler<PostPersistenceEventArgs<Patient>> Obsoleted;
        public event EventHandler<PreRetrievalEventArgs> Retrieving;
        public event EventHandler<PostRetrievalEventArgs<Patient>> Retrieved;
        public event EventHandler<PreQueryEventArgs<Patient>> Querying;
        public event EventHandler<PostQueryEventArgs<Patient>> Queried;

        public int Count(Expression<Func<Patient, bool>> query, IPrincipal authContext)
        {
            throw new NotImplementedException();
        }

        public Patient Get<TIdentifier>(MARC.HI.EHRS.SVC.Core.Data.Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
        {
            var c = containerId as Identifier<Guid>;
            return this.m_patients.FirstOrDefault(o => o.Key.Value == c.Id);
        }

        public Patient Insert(Patient storageData, IPrincipal principal, TransactionMode mode)
        {
            throw new NotImplementedException();
        }

        public Patient Obsolete(Patient storageData, IPrincipal principal, TransactionMode mode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Patient> Query(Expression<Func<Patient, bool>> query, IPrincipal authContext)
        {
            return this.m_patients.Where(query.Compile());
        }

        /// <summary>
        /// Query with limits
        /// </summary>
        public IEnumerable<Patient> Query(Expression<Func<Patient, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
        {
            totalCount = this.m_patients.Count(query.Compile());
            return this.m_patients.Where(query.Compile()).Skip(offset).Take(count ?? 100);
        }

        public Patient Update(Patient storageData, IPrincipal principal, TransactionMode mode)
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
                while(!sr.EndOfStream)
                {
                    var data = sr.ReadLine().Split(',');
                    patients.Add(new Patient()
                    {
                        Key = Guid.NewGuid(),
                        Identifiers = new List<EntityIdentifier>()
                        {
                            new EntityIdentifier(Guid.Parse("8ba75e94-e1ac-4aea-aede-b2f0c9aa8b77"), data[0]),
                            new EntityIdentifier(Guid.Parse("22ae27be-8b5f-46e0-9429-80d66acf6f7c"), data[6])
                        },
                        GenderConceptKey = Guid.Parse(data[2].ToLower() == "m" ? "f4e3a6bb-612e-46b2-9f77-ff844d971198" :
                            data[2].ToLower() == "f" ? "094941e9-a3db-48b5-862c-bc289bd7f86c" :
                            "091f27e8-e592-4e81-b5d7-338b7749d8b8"),
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
                    });
                }
            }


        }
    }
}
