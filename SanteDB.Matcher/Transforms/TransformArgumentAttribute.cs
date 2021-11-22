using System;
using System.Collections.Generic;
using System.Text;

namespace SanteDB.Matcher.Transforms
{
    /// <summary>
    /// Allows a transform to expose it expects an argument
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
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