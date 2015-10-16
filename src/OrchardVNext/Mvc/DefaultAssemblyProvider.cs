﻿using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Runtime;
using OrchardVNext.Environment;
using OrchardVNext.Environment.Extensions.Loaders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace OrchardVNext.Mvc {
    public class DefaultAssemblyProviderTest : IAssemblyProvider {
        /// <summary>
        /// Gets the set of assembly names that are used as root for discovery of
        /// MVC controllers, view components and views.
        /// </summary>
        protected virtual HashSet<string> ReferenceAssemblies { get; }
        = new HashSet<string>(StringComparer.Ordinal)
        {
            "Microsoft.AspNet.Mvc",
            "Microsoft.AspNet.Mvc.Core",
            "Microsoft.AspNet.Mvc.ModelBinding",
            "Microsoft.AspNet.Mvc.Razor",
            "Microsoft.AspNet.Mvc.Razor.Host",
            "Microsoft.AspNet.Mvc.Rendering",
        };

        private readonly IOrchardLibraryManager _libraryManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAssemblyLoaderContainer _loaderContainer;

        public DefaultAssemblyProviderTest(IOrchardLibraryManager libraryManager,
            IServiceProvider serviceProvider,
            IAssemblyLoaderContainer container) {
            _libraryManager = libraryManager;
            _serviceProvider = serviceProvider;
            _loaderContainer = container;
        }

        /// <inheritdoc />
        public IEnumerable<Assembly> CandidateAssemblies {
            get {
                return GetCandidateLibraries().SelectMany(l => l.LoadableAssemblies).Select(Load);
            }
        }

        /// <summary>
        /// Returns a list of libraries that references the assemblies in <see cref="ReferenceAssemblies"/>.
        /// By default it returns all assemblies that reference any of the primary MVC assemblies
        /// while ignoring MVC assemblies.
        /// </summary>
        /// <returns>A set of <see cref="ILibraryInformation"/>.</returns>
        protected virtual IEnumerable<ILibraryInformation> GetCandidateLibraries() {
            if (ReferenceAssemblies == null) {
                return Enumerable.Empty<ILibraryInformation>();
            }

            // GetReferencingLibraries returns the transitive closure of referencing assemblies
            // for a given assembly.
            return ReferenceAssemblies.SelectMany(_libraryManager.GetReferencingLibraries)
                                      .Distinct()
                                      .Where(IsCandidateLibrary);
        }

        private Assembly Load(AssemblyName assemblyName) {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
                return assembly;

            using (_loaderContainer.AddLoader(new ExtensionAssemblyLoader(@"", _serviceProvider))) {
                return Assembly.Load(assemblyName);
            }
        }

        private bool IsCandidateLibrary(ILibraryInformation library) {
            Debug.Assert(ReferenceAssemblies != null);
            return !ReferenceAssemblies.Contains(library.Name);
        }
    }
}