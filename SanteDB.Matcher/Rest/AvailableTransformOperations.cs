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
using Newtonsoft.Json;
using SanteDB.Core.Interop;
using SanteDB.Core.Matching;
using SanteDB.Core.Model.Parameters;
using SanteDB.Matcher.Transforms;
using SanteDB.Rest.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Rest
{
    /// <summary>
    /// The type of transformer
    /// </summary>
    [XmlType(nameof(TransformerType), Namespace = "http://santedb.org/matching")]
    internal enum TransformerType
    {
        /// <summary>
        /// The transform is binary (works with two operands)
        /// </summary>
        [XmlEnum("binary")]
        Binary,

        /// <summary>
        /// The transform is unary
        /// </summary>
        [XmlEnum("unary")]
        Unary
    }

    /// <summary>
    /// Transformer information
    /// </summary>
    [XmlType(nameof(TransformerInfo), Namespace = "http://santedb.org/matching")]
    [ExcludeFromCodeCoverage]
    internal class TransformerInfo
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        public TransformerInfo()
        {
        }

        /// <summary>
        /// Create transformer information from an argument type
        /// </summary>
        public TransformerInfo(String name, Type transformerType)
        {
            this.Name = name;
            this.Type = typeof(IUnaryDataTransformer).IsAssignableFrom(transformerType) ? TransformerType.Unary : TransformerType.Binary;
            this.DisplayName = transformerType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
            this.Description = transformerType.GetCustomAttribute<DescriptionAttribute>()?.Description;
            this.Arguments = transformerType.GetCustomAttributes<TransformArgumentAttribute>().Select(o => new TransformerArgumentInfo(o)).ToList();
        }

        /// <summary>
        /// Gets the name of the transformer
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets the type of transformer
        /// </summary>
        [XmlElement("type"), JsonProperty("type")]
        public TransformerType Type { get; set; }

        /// <summary>
        /// Gets the display name of the transformer
        /// </summary>
        [XmlElement("display"), JsonProperty("display")]
        public String DisplayName { get; set; }

        /// <summary>
        /// Gets the description of the transformer
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public String Description { get; set; }

        /// <summary>
        /// Gets the parameters for the
        /// </summary>
        [XmlArray("arguments"), XmlArrayItem("arg"), JsonProperty("arguments")]
        public List<TransformerArgumentInfo> Arguments { get; set; }
    }

    /// <summary>
    /// Transformer information
    /// </summary>
    [XmlType(nameof(TransformerArgumentInfo), Namespace = "http://santedb.org/matching")]
    [ExcludeFromCodeCoverage]
    internal class TransformerArgumentInfo
    {
        /// <summary>
        /// CTOR for argument serialization
        /// </summary>
        public TransformerArgumentInfo()
        {
        }

        /// <summary>
        /// Create from argument info
        /// </summary>
        public TransformerArgumentInfo(TransformArgumentAttribute arg)
        {
            this.Name = arg.Name;
            this.Required = arg.Required;
            this.Description = arg.Description;
            this.TypeName = arg.ArgumentType.Name;
        }

        /// <summary>
        /// Gets or sets the name of the transform argument
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets the desciption
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public String Description { get; set; }

        /// <summary>
        /// Gets the type of argument
        /// </summary>
        [XmlElement("type"), JsonProperty("type")]
        public String TypeName { get; set; }

        /// <summary>
        /// Gets if the argument is required
        /// </summary>
        [XmlElement("required"), JsonProperty("required")]
        public bool Required { get; set; }
    }

    /// <summary>
    /// A rest operation that returns the available transforms
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AvailableTransformOperations : IApiChildOperation
    {
        /// <summary>
        /// Get the scope binding
        /// </summary>
        public ChildObjectScopeBinding ScopeBinding => ChildObjectScopeBinding.Class;

        /// <summary>
        /// Gets the parent types
        /// </summary>
        public Type[] ParentTypes => new Type[] { typeof(IRecordMatchingConfiguration) };

        /// <summary>
        /// Get the name
        /// </summary>
        public string Name => "transforms";

        /// <summary>
        /// Invoke the method
        /// </summary>
        public object Invoke(Type scopingType, object scopingKey, ParameterCollection parameters)
        {
            return TransformerFactory.Current.GetTransformers().Select(o => new TransformerInfo(o.Key, o.Value)).ToList();
        }
    }
}