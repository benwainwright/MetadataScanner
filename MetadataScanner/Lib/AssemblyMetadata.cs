/*
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */
namespace MetadataScanner.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;
    using MetadataScanner.Entities.Base;
    using MetadataScanner.Interfaces;

    internal class AssemblyMetadata : IAssembly
    {
        private const string MapNamePrefix = "ASSEMBLY_SCANNER_MAP_";

        private List<AssemblyRef> assemblyReferences;

        private List<TypeDef> typeDefinitions;

        private List<TypeRef> typeReferences;

        public AssemblyMetadata(
            string path,
            MetadataReaderOptions options = MetadataReaderOptions.Default,
            MetadataStringDecoder decoder = null)
        {
            FilePath = path;
            LoadReaderAndParseMetadataFromAssembly(path, options, decoder);
        }

        public string FilePath { get; }

        public string Name { get; private set; }

        public Version Version { get; private set; }

        public List<byte> PublicKey { get; private set; }

        public IEnumerable<IAssemblyRef> AssemblyReferences => assemblyReferences;

        public IEnumerable<ITypeDef> TypeDefinitions => typeDefinitions;

        public IEnumerable<ITypeRef> TypeReferences => typeReferences;

        public void LinkAssemblies(List<AssemblyMetadata> assemblies)
        {
            foreach (var assembly in AssemblyReferences) {
                assembly.LinkAssemblyMetaData(assemblies);
            }

            foreach (var reference in TypeReferences) {
                reference.ResolveTypesFromLinkedAssembly();
            }
        }

        public unsafe void LoadReaderAndParseMetadataFromAssembly(
            string filename,
            MetadataReaderOptions options,
            MetadataStringDecoder decoder)
        {
            using (var file = LoadAssembly(filename, out var length, out var access)) {
                using (var stream = file.CreateViewStream(0, length, access)) {
                    var headers = new PEHeaders(stream);
                    var start = (byte*)0;
                    stream.SafeMemoryMappedViewHandle.AcquirePointer(ref start);
                    var size = headers.MetadataSize;
                    var reader = new MetadataReader(start + headers.MetadataStartOffset, size, options, decoder);
                    PopulateMetaDataFromReader(reader);
                }
            }
        }

        public void PopulateMetaDataFromReader(MetadataReader reader)
        {
            ParseAssemblyDefinition(reader);
            assemblyReferences = AssemblyRef.LoadReferences(reader);
            typeDefinitions = TypeDef.LoadDefinitions(reader);
            typeReferences = TypeRef.LoadReferences(reader, assemblyReferences);
            LinkTypes(typeDefinitions, typeReferences);
        }

        public override string ToString()
        {
            return $"{Name} {Version}";
        }

        private static MemoryMappedFile LoadAssembly(string filename, out long length, out MemoryMappedFileAccess access)
        {
            var fileInfo = new FileInfo(filename);
            length = fileInfo.Length;
            var mapName = MapNamePrefix + fileInfo.Name;
            var mode = FileMode.Open;
            access = MemoryMappedFileAccess.Read;
            return MemoryMappedFile.CreateFromFile(filename, mode, mapName, length, access);
        }

        private static void LinkTypes(IEnumerable<TypeDef> definitions, IEnumerable<TypeRef> references)
        {
            var allTypes = new List<LocalTypeEntity>();
            allTypes.AddRange(definitions);
            allTypes.AddRange(references);

            foreach (var type in definitions) {
                type.LinkBaseType(allTypes);
                type.LinkInterfaceImplementations(allTypes);
            }
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
