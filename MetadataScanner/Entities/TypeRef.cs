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
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Reflection.Metadata.Ecma335;
    using MetadataScanner.Entities.Base;
    using MetadataScanner.Enums;

    internal class TypeRef : LocalTypeEntity, ITypeRef
    {
        public TypeRef(MetadataReader reader, TypeReferenceHandle handle, Dictionary<int, AssemblyRef> assemblies)
            : base(
                reader.GetString(reader.GetTypeReference(handle).Name),
                reader.GetString(reader.GetTypeReference(handle).Namespace),
                reader.GetToken(handle))
        {
            var reference = reader.GetTypeReference(handle);
            var assemblyToken = reader.GetToken(reference.ResolutionScope);
            if (assemblies.ContainsKey(assemblyToken)) {
                Assembly = assemblies[assemblyToken];
            }

            Definition = new TypeDef(Name, DeclaredNamespace);
        }

        public IAssemblyRef Assembly { get; }

        public ITypeDef Definition { get; private set; }

        public static List<TypeRef> LoadReferences(MetadataReader reader, Dictionary<int, AssemblyRef> assemblies)
        {
            var query = from type
                        in reader.TypeReferences
                        select new TypeRef(reader, type, assemblies);

            return query.ToList();
        }

        public override bool ImplementsInterface(ILocalTypeEntity entity)
        {
            if (Definition.ResolutionStatus != ResolutionStatus.UnResolved) {
                return Definition.ImplementsInterface(entity);
            }

            return false;
        }

        public void ResolveTypesFromLinkedAssembly()
        {
            if (Definition.ResolutionStatus == ResolutionStatus.Resolved || Assembly == null) {
                return;
            }

            var metaData = Assembly?.MetaData;
            if (metaData == null) {
                return;
            }

            var defined = metaData.TypeDefinitions;

            var query = from type
                        in defined
                        where type.Name.Equals(Name, StringComparison.InvariantCulture) &&
                              type.DeclaredNamespace.Equals(DeclaredNamespace, StringComparison.InvariantCulture)
                        select type;

            Definition = query.FirstOrDefault();
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
