using System;
using Mono.Cecil;

namespace CSharpDecompiler
{
    /// <summary>
    ///  Ignores missing assemblies.
    /// </summary>
    public class IgnoringExceptionsAssemblyResolver : DefaultAssemblyResolver
    {
        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            try
            {
                return base.Resolve(name);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

