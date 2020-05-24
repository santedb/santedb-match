/*
 * Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
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
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Util
{
    /// <summary>
    /// Represents a utility for string differences
    /// </summary>
    public static class StringDifference
    {

        /// <summary>
        /// Calculate the levenshtein distance between strings
        /// </summary>
        public static double Levenshtein(this String source, String target)
        {

            if (string.IsNullOrEmpty(source))
                return string.IsNullOrEmpty(target) ? 0 : target.Length;

            if (string.IsNullOrEmpty(target))
                return string.IsNullOrEmpty(source) ? 0 : source.Length;

            source = source.ToLowerInvariant().Trim();
            target = target.ToLowerInvariant().Trim();

            var sourceLength = source.Length;
            var targetLength = target.Length;

            var distance = new int[sourceLength + 1, targetLength + 1];

            for (var i = 0; i <= sourceLength; distance[i, 0] = i++) ;
            for (var j = 0; j <= targetLength; distance[0, j] = j++) ;

            for (var i = 1; i <= sourceLength; i++)
            {
                for (var j = 1; j <= targetLength; j++)
                {
                    var cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            //return distance[sourceLength, targetLength];

            double stepsToSame = distance[sourceLength, targetLength];
            return stepsToSame;

        }


        /// <summary>
        /// Returns the similarty to 
        /// </summary>
        public static double SimilarityTo(this String source, String target)
        {

            return (1.0 - (source.Levenshtein(target) / (double)Math.Max(source.Length, target.Length) + 0.000001));

        }
    }
}
