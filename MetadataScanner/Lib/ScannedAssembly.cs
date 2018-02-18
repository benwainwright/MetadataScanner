/*
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */
namespace MetadataScanner.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata;
    using System.Reflection.Metadata.Ecma335;
    using MetadataScanner.Enums;
    using MetadataScanner.Interfaces;

    internal class ScannedAssembly : IAssembly
    {
        private Dictionary<int, IType> allTypes = new Dictionary<int, IType>();

        private Dictionary<int, IAssembly> assemblyReferences;

        private List<IType> typeDefinitions;

        private List<IType> typeReferences;

        public ScannedAssembly(int token)
        {
            Token = token;
        }

        public ScannedAssembly(AssemblyReferenceHandle handle, MetadataReader reader)
        {
            var reference = reader.GetAssemblyReference(handle);
            Name = reader.GetString(reference.Name);
            Culture = reader.GetString(reference.Culture);
            PublicKey = reader.GetBlobBytes(reference.PublicKeyOrToken).ToList();
            Token = reader.GetToken(handle);
            Version = reference.Version;
            Flags = reference.Flags;
            ResolutionStatus = ResolutionStatus.UnResolved;
        }

        public ScannedAssembly(string path, MetadataReader reader)
        {
            FilePath = path;
            PopulateMetaDataFromReader(reader);
        }

        public string FilePath { get; }

        public string Name { get; private set; }

        public Version Version { get; private set; }

        public List<byte> PublicKey { get; private set; }

        public AssemblyFlags Flags { get; }

        public string Culture { get; }

        public int Token { get;  }

        public ResolutionStatus ResolutionStatus { get; }

        public IEnumerable<IAssembly> AssemblyReferences => assemblyReferences.Values;

        public IEnumerable<IType> TypeDefinitions => typeDefinitions;

        public IEnumerable<IType> TypeReferences => typeReferences;

        public void PopulateMetaDataFromReader(MetadataReader reader)
        {
            ParseAssemblyDefinition(reader);
            assemblyReferences = LoadReferencedAssemblies(reader);
            typeDefinitions = LoadDefinedTypes(reader, this);
            typeDefinitions.ForEach(item => allTypes.Add(item.Token, item));

            typeReferences = LoadReferencedTypes(reader);
            typeReferences.ForEach(item => allTypes.Add(item.Token, item));

            foreach (var definition in typeDefinitions) {
                definition.ResolveLocal(allTypes);
            }
        }

        public void ResolveReferences(List<IAssembly> assemblies)
        {
            foreach (var assembly in assemblies) {
                if (!assembly.Equals(this)) {
                    foreach (var reference in assembly.TypeReferences) {
                        reference.ResolveExternal(allTypes);
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{Name} {Version}";
        }

        public bool Equals(IAssembly other)
        {
            return other.Name.Equals(Name, StringComparison.InvariantCulture) &&
                   other.PublicKey.SequenceEqual(PublicKey) &&
                   other.Version.Equals(Version);
        }

        private static List<IType> LoadDefinedTypes(MetadataReader reader, IAssembly assembly)
        {
            var definitions = from handle
                              in reader.TypeDefinitions
                              let definition = reader.GetTypeDefinition(handle)
                              let interfaces = from theInterface
                                               in definition.GetInterfaceImplementations()
                                               let theImplementation = reader.GetInterfaceImplementation(theInterface).Interface
                                               select new ScannedType(reader.GetToken(theImplementation)).AsInterface()
                              select new ScannedType(
                                  token: reader.GetToken(handle),
                                  isLocal: true,
                                  name: reader.GetString(definition.Name),
                                  theNamespace: reader.GetString(definition.Namespace),
                                  assembly: assembly,
                                  baseType: new ScannedType(reader.GetToken(definition.BaseType)),
                                  attributes: definition.Attributes,
                                  interfaces: interfaces.ToList()).AsInterface();
            return definitions.ToList();
        }

        private static Dictionary<int, IAssembly> LoadReferencedAssemblies(MetadataReader reader)
        {
            IEnumerable<IAssembly> assemblies = from assembly
                                                in reader.AssemblyReferences
                                                select new ScannedAssembly(assembly, reader);

            return assemblies.ToDictionary(item => item.Token, item => item);
        }

        private static List<IType> LoadReferencedTypes(MetadataReader reader)
        {
            var references = from handle
                             in reader.TypeReferences
                             let reference = reader.GetTypeReference(handle)
                             select new ScannedType(
                                 token: reader.GetToken(handle),
                                 isLocal: false,
                                 name: reader.GetString(reference.Name),
                                 theNamespace: reader.GetString(reference.Namespace),
                                 resolutionStatus: ResolutionStatus.UnResolved,
                                 assembly: new ScannedAssembly(reader.GetToken(reference.ResolutionScope))).AsInterface();
            return references.ToList();
        }

        private void ParseAssemblyDefinition(MetadataReader reader)
        {
            var assembly = reader.GetAssemblyDefinition();
            Name = reader.GetString(assembly.Name);
            Version = assembly.Version;
            PublicKey = reader.GetBlobBytes(assembly.PublicKey).ToList();
        }
    }
}
