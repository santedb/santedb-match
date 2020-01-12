/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 *
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
 * User: JustinFyfe
 * Date: 2019-1-22
 */
using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SanteDB.Core.Attributes;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SanteMatch")]
[assembly: AssemblyDescription("Deterministic and probabalistic matchers for SanteDB")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Fyfe Software Inc.")]
[assembly: AssemblyProduct("SanteMatch")]
[assembly: AssemblyCopyright("Copyright (C) 2018 - 2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.28.*")]
[assembly: AssemblyVersion("1.118.0.0")]
[assembly: AssemblyFileVersion("1.118.0.0")]
[assembly: PluginTraceSource("SanteDB.Matcher.Engine")]
[assembly: Plugin(EnableByDefault = false, Environment = PluginEnvironment.Server)]
