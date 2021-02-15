/*
 *
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms
{
    /// <summary>
    /// Represents a transformer factory
    /// </summary>
    public class TransformerFactory
    {
        // Singleton instance
        private static TransformerFactory s_current;
        // Lock
        private static Object s_lock = new object();

        // Data transformers
        private Dictionary<String, Type> m_dataTransformers;

        /// <summary>
        /// Gets the current factory
        /// </summary>
        public static TransformerFactory Current
        {
            get
            {
                if (s_current == null)
                    lock(s_lock)
                        s_current = s_current ?? new TransformerFactory();
                return s_current;
            }
        }

        /// <summary>
        /// Private CTOR
        /// </summary>
        private TransformerFactory()
        {
            this.m_dataTransformers = typeof(TransformerFactory).Assembly.ExportedTypes
                .Where(o => typeof(IDataTransformer).IsAssignableFrom(o) && !o.IsAbstract && !o.IsInterface)
                .ToDictionary(o=>(Activator.CreateInstance(o) as IDataTransformer).Name, o=>o);
        }

        /// <summary>
        /// Get the specified data transformer
        /// </summary>
        /// <param name="name">The name of the transformer</param>
        /// <returns>The transformer instance</returns>
        public IDataTransformer CreateTransformer(String name)
        {
            Type ttype = null;
            this.m_dataTransformers.TryGetValue(name, out ttype);
            return Activator.CreateInstance(ttype) as IDataTransformer;
        }

        /// <summary>
        /// Add the specified data transformer to the factory
        /// </summary>
        /// <param name="transformerType">The transformer type to register</param>
        public void Add(String name, Type transformerType)
        {
            lock (s_lock)
                if (!this.m_dataTransformers.ContainsKey(transformerType.Name))
                    this.m_dataTransformers.Add(transformerType.Name, transformerType);
        }
    }
}
