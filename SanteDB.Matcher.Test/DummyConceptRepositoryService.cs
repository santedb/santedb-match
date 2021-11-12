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
using SanteDB.Core.Event;
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.DataTypes;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Security.Principal;

namespace SanteDB.Matcher.Test
{
    /// <summary>
    /// Repository service for concepts
    /// </summary>
    [ExcludeFromCodeCoverage]
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
            { EntityRelationshipTypeKeys.Birthplace, new Concept() { Mnemonic = "Birthplace" } },
            { EntityRelationshipTypeKeys.Mother, new Concept() { Mnemonic = "Mother" } }
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

        public Concept Get(Guid id, Guid? versionId, bool loadFast, IPrincipal principal)
        {
            return this.Get(id);
        }

        public Concept Get(Guid key, Guid? versionKey, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Concept Insert(Concept data, TransactionMode mode, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Concept Obsolete(Concept data, TransactionMode mode, IPrincipal principal)
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
