// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Mba.Host
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using WixToolset.Mba.Core;

    /// <summary>
    /// Entry point for the managed host to create and return the BA to the engine.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class BootstrapperApplicationFactory : MarshalByRefObject, IBootstrapperApplicationFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BootstrapperApplicationFactory"/> class.
        /// Entry point for the MBA host.
        /// </summary>
        public BootstrapperApplicationFactory()
        {
        }

        /// <summary>
        /// Loads the bootstrapper application assembly and calls its IBootstrapperApplicationFactory.Create method.
        /// </summary>
        /// <param name="pArgs">Pointer to BOOTSTRAPPER_CREATE_ARGS struct.</param>
        /// <param name="pResults">Pointer to BOOTSTRAPPER_CREATE_RESULTS struct.</param>
        /// <exception cref="MissingAttributeException">The bootstrapper application assembly
        /// does not define the <see cref="BootstrapperApplicationFactoryAttribute"/>.</exception>
        public void Create(IntPtr pArgs, IntPtr pResults)
        {
            // Get the wix.boostrapper section group to get the name of the bootstrapper application assembly to host.
            var section = ConfigurationManager.GetSection("wix.bootstrapper/host") as HostSection;
            if (null == section)
            {
                throw new MissingAttributeException(); // TODO: throw a more specific exception than this.
            }

            // Load the BA's IBootstrapperApplicationFactory.
            var baFactoryType = BootstrapperApplicationFactory.GetBAFactoryTypeFromAssembly(section.AssemblyName);
            var baFactory = (IBootstrapperApplicationFactory)Activator.CreateInstance(baFactoryType);
            if (null == baFactory)
            {
                throw new InvalidBootstrapperApplicationFactoryException();
            }

            baFactory.Create(pArgs, pResults);
        }

        /// <summary>
        /// Locates the <see cref="BootstrapperApplicationFactoryAttribute"/> and returns the specified type.
        /// </summary>
        /// <param name="assemblyName">The assembly that defines the IBootstrapperApplicationFactory implementation.</param>
        /// <returns>The bootstrapper application factory <see cref="Type"/>.</returns>
        private static Type GetBAFactoryTypeFromAssembly(string assemblyName)
        {
            Type baFactoryType = null;

            // Load the requested assembly.
            Assembly asm = AppDomain.CurrentDomain.Load(assemblyName);

            // If an assembly was loaded and is not the current assembly, check for the required attribute.
            // This is done to avoid using the BootstrapperApplicationFactoryAttribute which we use at build time
            // to specify the BootstrapperApplicationFactory assembly in the manifest.
            if (!Assembly.GetExecutingAssembly().Equals(asm))
            {
                // There must be one and only one BootstrapperApplicationFactoryAttribute.
                // The attribute prevents multiple declarations already.
                var attrs = (BootstrapperApplicationFactoryAttribute[])asm.GetCustomAttributes(typeof(BootstrapperApplicationFactoryAttribute), false);
                if (null != attrs)
                {
                    baFactoryType = attrs[0].BootstrapperApplicationFactoryType;
                }
            }

            if (null == baFactoryType)
            {
                throw new MissingAttributeException();
            }

            return baFactoryType;
        }
    }
}
