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
using SanteDB.Core.Diagnostics;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Definition;
using SanteDB.Matcher.Exceptions;
using SanteDB.Matcher.Transforms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.Matcher.Util
{

    /// <summary>
    /// Assertion result class
    /// </summary>
    internal class AssertionResult
    {

        /// <summary>
        /// Creates a new assertion result
        /// </summary>
        public AssertionResult(string propertyName, bool evaluated, bool result, double? calculatedScore, object aValue, object bValue)
        {
            this.PropertyName = propertyName;
            this.Evaluated = evaluated;
            this.CalculatedScore = calculatedScore;
            this.A = aValue;
            this.B = bValue;
            this.Result = result;
        }

        /// <summary>
        /// Gets the property name
        /// </summary>
        public String PropertyName { get; }

        /// <summary>
        /// Gets whether the assertion was evaluated
        /// </summary>
        public bool Evaluated { get; }

        /// <summary>
        /// The calculated score
        /// </summary>
        public double? CalculatedScore { get; }

        /// <summary>
        /// The A value
        /// </summary>
        public object A { get; }

        /// <summary>
        /// The B value
        /// </summary>
        public object B { get; }

        /// <summary>
        /// Gets the result (TRUE|FALSE) of the assertion if it was execute
        /// </summary>
        public bool Result { get; }

    }

    /// <summary>
    /// Assertion utility
    /// </summary>
    internal static class AssertionUtil
    {

        /// <summary>
        /// Static ctor
        /// </summary>
        static AssertionUtil()
        {
            foreach (var t in typeof(AssertionUtil).Assembly.ExportedTypes.Where(t => typeof(IDataTransformer).IsAssignableFrom(t) && !t.IsAbstract))
            {
                var idt = Activator.CreateInstance(t) as IDataTransformer;
                s_transforms.Add(idt.Name, idt);
            }
        }

        // Transforms loaded by this matcher
        private static Dictionary<String, IDataTransformer> s_transforms = new Dictionary<string, IDataTransformer>();

        // Tracer
        private static Tracer m_tracer = Tracer.GetTracer(typeof(AssertionUtil));

        /// <summary>
        /// Get the null score
        /// </summary>
        private static double? GetNullScore(MatchAttribute attribute)
        {
            switch (attribute.WhenNull)
            {
                case MatchAttributeNullBehavior.Disqualify:
                    return -100;
                case MatchAttributeNullBehavior.Zero:
                    return 0.0;
                case MatchAttributeNullBehavior.Match:
                    return attribute.MatchWeight;
                case MatchAttributeNullBehavior.NonMatch:
                    return attribute.NonMatchWeight;
                case MatchAttributeNullBehavior.Ignore:
                    return null;
                default:
                    throw new InvalidOperationException("Should not be here - Can't determine null behavior");
            }
        }


        /// <summary>
        /// Executes a transform
        /// </summary>
        /// <param name="transform">The transform to run</param>
        /// <param name="aValue">The A value to pass to the transform</param>
        /// <param name="bValue">The B value to pass to the transform</param>
        /// <returns>The result of the transform to be assigned back to the scope (or to the values in the case of unary transforms)</returns>
        internal static object ExecuteTransform(String transformName, object[] transformParameters, ref object aValue, ref object bValue)
        {
            try
            {
                IDataTransformer transformer = null;
                if (s_transforms.TryGetValue(transformName, out transformer))
                {
                    if (transformer is IUnaryDataTransformer)
                    {
#if DEBUG
                        m_tracer.TraceVerbose("Will apply unary transform {0} on {1}", transformName, aValue);
#endif
                        aValue = (transformer as IUnaryDataTransformer).Apply(aValue, transformParameters);
#if DEBUG
                        m_tracer.TraceVerbose("Will apply unary transform {0} on {1}", transformName, bValue);
#endif
                        bValue = (transformer as IUnaryDataTransformer).Apply(bValue, transformParameters);
                        return null; // No result for unary transforms
                    }
                    else if (transformer is IBinaryDataTransformer)
                    {
#if DEBUG
                        m_tracer.TraceVerbose("Will apply binary transform {0} on {1} and {2}", transformName, aValue, bValue);
#endif
                        return (transformer as IBinaryDataTransformer).Apply(aValue, bValue, transformParameters);
                    }
                    else
                        throw new InvalidOperationException("Invalid type of transform");
                }
                else
                    throw new KeyNotFoundException($"Transform {transformName} is not registered");
            }
            catch (Exception e)
            {
                m_tracer.TraceError("Error executing {0} on {1} and {2} - {3}", transformName, aValue, bValue, e.Message);
                throw new MatchingException($"Error executing {transformName} on {aValue} and {bValue}", e);
            }
        }


        /// <summary>
        /// Execute an assertion
        /// </summary>
        /// <param name="assertion">The assertion to execute</param>
        /// <param name="aValue">The value of the A value</param>
        /// <param name="bValue">The value of the B value</param>
        /// <returns>True if the assertion passes, false if not, null if the execution was skipped</returns>
        internal static AssertionResult ExecuteAssertion(string propertyName, MatchAttributeAssertion assertion, MatchAttribute attribute, object aValue, object bValue)
        {
            try
            {

                // Apply all transforms first
                object a = aValue, b = bValue;
                object scope = null;
                foreach (var xform in assertion.Transforms)
                {
                    if (a == null || b == null)
                        break;
                    scope = ExecuteTransform(xform.Name, xform.Parameters.ToArray(), ref a, ref b) ?? scope;
                }
                if (a == null || b == null)
                    return new AssertionResult(propertyName, false, false, GetNullScore(attribute), aValue, bValue);
                else if(a is IEnumerable enumA && !enumA.OfType<Object>().Any() || b is IEnumerable enumB && !enumB.OfType<Object>().Any())
                    return new AssertionResult(propertyName, false, false, GetNullScore(attribute), aValue, bValue);

                // Scope as enum
                if (scope is IEnumerable enumScope)
                {
                    if (!enumScope.OfType<Object>().Any()) // No results - cannot evaluate
                        return new AssertionResult(propertyName, false, false, GetNullScore(attribute), aValue, bValue);
                    else
                        scope = enumScope.OfType<Object>().Max();
                }

                var retVal = true;
                switch (assertion.Operator)
                {
                    case BinaryOperatorType.AndAlso: // There are sub-assertions 
                        {
                            foreach (var asrt in assertion.Assertions)
                            {
                                var subVal = ExecuteAssertion(propertyName, asrt, attribute, a, b);
                                if (subVal.Evaluated)
                                    retVal &= subVal.Result;
                                else
                                    return new AssertionResult(propertyName, false, false, GetNullScore(attribute), aValue, bValue);
                            }
                            break;
                        }
                    case BinaryOperatorType.OrElse:
                        {
                            foreach (var asrt in assertion.Assertions)
                            {
                                var subVal = ExecuteAssertion(propertyName, asrt, attribute, a, b);
                                if (subVal.Evaluated)
                                    retVal |= subVal.Result;
                                else
                                    return new AssertionResult(propertyName, false, false, GetNullScore(attribute), aValue, bValue);
                            }
                            break;
                        }
                    case BinaryOperatorType.Equal:
                        if (assertion.ValueSpecified)
                            retVal = scope.Equals(assertion.Value);
                        else
                        {
                            if (a == b) // reference equality
                                retVal = true;
                            else if (a is IEnumerable aEnum && b is IEnumerable bEnum) // Run against sequence
                                retVal = aEnum.OfType<Object>().SequenceEqual(bEnum.OfType<Object>());
                            else
                                retVal = a.Equals(b);
                        }
                        break;
                    case BinaryOperatorType.NotEqual:
                        if (assertion.ValueSpecified)
                            retVal = !scope.Equals(assertion.Value);
                        else
                        {
                            if (a == null)
                                retVal = b != null;
                            else if (a is IEnumerable aEnum && b is IEnumerable bEnum) // Run against sequence
                                retVal = !aEnum.OfType<Object>().SequenceEqual(bEnum.OfType<Object>());
                            else
                                retVal = !a.Equals(b);
                        }
                        break;
                    case BinaryOperatorType.GreaterThan:
                        retVal = (double)scope > assertion.Value;
                        break;
                    case BinaryOperatorType.GreaterThanOrEqual:
                        retVal = (double)scope >= assertion.Value;
                        break;
                    case BinaryOperatorType.LessThan:
                        retVal = (double)scope < assertion.Value;
                        break;
                    case BinaryOperatorType.LessThanOrEqual:
                        retVal = (double)scope <= assertion.Value;
                        break;
                    default:
                        throw new InvalidOperationException($"Operator type {assertion.Operator} is not supported in this context");
                }

#if DEBUG
                m_tracer.TraceVerbose("ASSERT - {0} {1} {2} => {3}", aValue, assertion.Operator, bValue, retVal);
#endif
                var weightedScore = retVal ? attribute.MatchWeight : attribute.NonMatchWeight;
                // Is there a partial measure applied? only apply if the match assertion was correct
                if (attribute.Measure != null && retVal)
                {
                    a = aValue; b = bValue;
                    foreach (var xform in attribute.Measure.Transforms)
                        ExecuteTransform(xform.Name, xform.Parameters.ToArray(), ref a, ref b);
                    weightedScore *= (double)ExecuteTransform(attribute.Measure.Name, attribute.Measure.Parameters.ToArray(), ref a, ref b);
                }

                m_tracer.TraceVerbose("Match attribute ({0}) was scored against input as {1}", attribute, weightedScore);

                return new AssertionResult(propertyName, true, retVal, weightedScore, aValue, bValue);
            }
            catch (Exception e)
            {
                m_tracer.TraceError("Error executing assertion {0} - {1}", e);
                throw new MatchingException($"Error executing assertion {assertion}", e);
            }
        }
    }
}
