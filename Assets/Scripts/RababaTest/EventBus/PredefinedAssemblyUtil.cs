using System;
using System.Collections.Generic;
using System.Reflection;

namespace RababaTest.EventBus
{
    /// <summary>
    /// Utility class to work with and retrieve data from predefined Unity assemblies.
    /// </summary>
    public static class PredefinedAssemblyUtil
    {
        enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharpFirstPass
        }

        // <summary>
        /// Determines the type of a predefined assembly based on its name.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to check.</param>
        /// <returns>The corresponding <see cref="AssemblyType"/> value or null if the name does not match any predefined type.</returns>

        static AssemblyType? GetAssemblyType(string assemblyName)
        {
            return assemblyName switch
            {
                "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
                "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
                _ => null
            };
        }

        /// <summary>
        /// Adds all types from a given assembly that implement a specific interface to the provided collection.
        /// </summary>
        /// <param name="assembly">An array of types from the assembly to check.</param>
        /// <param name="types">The collection to store matching types.</param>
        /// <param name="interfaceType">The interface type to filter by.</param>
        static void AddTypesFromAssembly(Type[] assembly, ICollection<Type> types, Type interfaceType)
        {
            if (assembly == null) return;
            for (int i = 0; i < assembly.Length; i++)
            {
                Type type = assembly[i];
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                {
                    types.Add(type);
                }
            }
        }

        /// <summary>
        /// Retrieves a list of types from predefined Unity assemblies that implement the specified interface.
        /// </summary>
        /// <param name="interfaceType">The interface type to filter by.</param>
        /// <returns>A list of types implementing the specified interface.</returns>
        public static List<Type> GetTypes(Type interfaceType)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Dictionary<AssemblyType, Type[]> assemblyTypes = new Dictionary<AssemblyType, Type[]>();
            List<Type> types = new List<Type>();
            for (int i = 0; i < assemblies.Length; i++)
            {
                AssemblyType? assemblyType = GetAssemblyType(assemblies[i].GetName().Name);
                if (assemblyType != null)
                {
                    assemblyTypes.Add((AssemblyType) assemblyType, assemblies[i].GetTypes());
                }
            }
            
            AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharp], types, interfaceType);
            AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharpEditor], types, interfaceType);

            return types;
        }
    }
}