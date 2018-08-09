using SanteDB.Core.Model;
using SanteDB.Core.Model.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Test
{
    public static class ExtensionMethodHelpers
    {

        /// <summary>
        /// Load all concepts for T
        /// </summary>
        public static T LoadConcepts<T>(this T me) where T  : IdentifiedData
        {

            foreach(var p in me.GetType().GetRuntimeProperties())
            {
                var val = p.GetValue(me);
                if (val is IEnumerable)
                    foreach (var v in val as IEnumerable)
                        (v as IdentifiedData)?.LoadConcepts();
                else if (val == null && p.PropertyType == typeof(Concept))
                    me.LoadProperty<Concept>(p.Name);
            }
            return me;
        }
    }
}
