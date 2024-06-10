﻿/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using NUnit.Framework;
using SanteDB.Core.Model.Roles;
using SanteDB.Matcher.Definition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SanteDB.Matcher.Test
{
    [ExcludeFromCodeCoverage]
    [TestFixture(Category = "Matching")]
    public class MatchConfigurationTest
    {
        /// <summary>
        /// Should load configuration
        /// </summary>
        [Test]
        public void ShouldLoadConfiguration()
        {
            var loaded = MatchConfiguration.Load(typeof(MatchConfigurationTest).Assembly.GetManifestResourceStream("SanteDB.Matcher.Test.Resources.DateOfBirthGenderIdClassified.xml"));
            Assert.IsNotNull(loaded);
            Assert.AreEqual(2, loaded.Scoring.Count);
            Assert.IsNotNull(loaded.Scoring.First().Assertion);
            Assert.AreEqual(1, loaded.Blocking.Count);
            Assert.AreEqual(3, loaded.Blocking.First().Filter.Count);
            Assert.AreEqual(typeof(Patient), loaded.Target.First().ResourceType);
            Assert.AreNotEqual(0.0d, loaded.Scoring.First().MatchWeight);
        }
    }
}