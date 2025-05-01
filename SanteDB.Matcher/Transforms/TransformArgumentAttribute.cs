/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using System;
using System.Diagnostics.CodeAnalysis;

namespace SanteDB.Matcher.Transforms
{
    /// <summary>
    /// Allows a transform to expose it expects an argument
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [ExcludeFromCodeCoverage]
    public class TransformArgumentAttribute : Attribute
    {
        /// <summary>
        /// Creates a new transform argument attribute
        /// </summary>
        public TransformArgumentAttribute(Type argumentType, String argumentName, String argumentDescription, bool isRequired = false)
        {
            this.ArgumentType = argumentType;
            this.Name = argumentName;
            this.Description = argumentDescription;
            this.Required = isRequired;
        }

        /// <summary>
        /// The type of argument
        /// </summary>
        public Type ArgumentType { get; }

        /// <summary>
        /// The name of the argument
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// The description of the argument
        /// </summary>
        public String Description { get; }

        /// <summary>
        /// True if the argument is required
        /// </summary>
        public bool Required { get; }
    }
}