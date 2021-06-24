using SanteDB.Core.Event;
using SanteDB.Core.Interfaces;
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.DataTypes;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Test
{
    /// <summary>
    /// Repository service for concepts
    /// </summary>
    public class DummyConceptRepositoryService : IDataPersistenceService<Concept>
    {

        private Dictionary<Guid, Concept> m_concepts = new Dictionary<Guid, Concept>()
        {
            { NameUseKeys.OfficialRecord, new Concept() { Mnemonic = "OfficialRecord" } },
            { NameUseKeys.Pseudonym, new Concept() { Mnemonic = "Pseudonym" } },
            { AddressUseKeys.HomeAddress, new Concept() { Mnemonic = "HomeAddress" } },
            { NameComponentKeys.Given, new Concept() { Mnemonic = "Given" } },
            { NameComponentKeys.Family, new Concept() { Mnemonic = "Family" } },
            { AddressComponentKeys.StreetAddressLine, new Concept() { Mnemonic = "StreetAddressLine" } },
            { AddressComponentKeys.City, new Concept() { Mnemonic = "City" } },
            { AddressComponentKeys.Country, new Concept() { Mnemonic = "Country" } },
            { AddressComponentKeys.State, new Concept() { Mnemonic = "State" } },
            { AddressComponentKeys.PostalCode, new Concept() { Mnemonic = "PostalCode" } },
            { Guid.Parse("094941e9-a3db-48b5-862c-bc289bd7f86c"), new Concept(){ Mnemonic = "Female"} },
            { Guid.Parse("f4e3a6bb-612e-46b2-9f77-ff844d971198"), new Concept() { Mnemonic = "Male"} },
            { EntityClassKeys.Patient, new Concept() { Mnemonic = "Patient"} },
            { EntityClassKeys.Person, new Concept() { Mnemonic = "Person"} },
            { DeterminerKeys.Specific, new Concept() { Mnemonic = "Specific" } },
            {  EntityRelationshipTypeKeys.Birthplace, new Concept() { Mnemonic = "Birthplace" } },
            {  EntityRelationshipTypeKeys.Mother, new Concept() { Mnemonic = "Mother" } },
        };

        public string ServiceName => throw new NotImplementedException();

       
        public event EventHandler<DataPersistingEventArgs<Concept>> Inserting;
        public event EventHandler<DataPersistedEventArgs<Concept>> Inserted;
        public event EventHandler<DataPersistingEventArgs<Concept>> Updating;
        public event EventHandler<DataPersistedEventArgs<Concept>> Updated;
        public event EventHandler<DataPersistingEventArgs<Concept>> Obsoleting;
        public event EventHandler<DataPersistedEventArgs<Concept>> Obsoleted;
        public event EventHandler<DataRetrievingEventArgs<Concept>> Retrieving;
        public event EventHandler<DataRetrievedEventArgs<Concept>> Retrieved;
        public event EventHandler<QueryRequestEventArgs<Concept>> Querying;
        public event EventHandler<QueryResultEventArgs<Concept>> Queried;

        public long Count(Expression<Func<Concept, bool>> p, IPrincipal authContext = null)
        {
            throw new NotImplementedException();
        }

        public Concept Get(Guid key)
        {
            Concept tr = null;
            if (this.m_concepts.TryGetValue(key, out tr))
            {
                tr.Key = key;
                return tr;
            }
            else
                return null;
        }

        public Concept Get(Guid key, Guid versionKey)
        {
            return this.Get(key);
        }

        public Concept Get(Guid id, Guid? versionId, IPrincipal principal)
        {
            return this.Get(id);
        }
        

        public Concept Insert(Concept data, TransactionMode mode, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Concept Obsolete(Guid key, TransactionMode mode, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Concept> Query(Expression<Func<Concept, bool>> query, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Concept> Query(Expression<Func<Concept, bool>> query, int offset, int? count, out int totalResults, IPrincipal principal, params ModelSort<Concept>[] order)
        {
            throw new NotImplementedException();
        }

        public Concept Update(Concept data, TransactionMode mode, IPrincipal principal)
        {
            throw new NotImplementedException();
        }
    }
}
