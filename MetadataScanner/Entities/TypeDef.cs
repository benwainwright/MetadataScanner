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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata;
    using System.Reflection.Metadata.Ecma335;
    using MetadataScanner.Entities;
    using MetadataScanner.Entities.Base;
    using MetadataScanner.Enums;

    internal class TypeDef : LocalTypeEntity, ITypeDef
    {
        private readonly TypeDefinition definition;

        private List<LocalTypeEntity> interfaceImplementations = new List<LocalTypeEntity>();

        public TypeDef(MetadataReader reader, TypeDefinitionHandle handle)
            : base(
                reader.GetString(reader.GetTypeDefinition(handle).Name),
                reader.GetString(reader.GetTypeDefinition(handle).Namespace),
                reader.GetToken(handle))
        {
            definition = reader.GetTypeDefinition(handle);
            BaseType = new AmbiguousLocalType(reader.GetToken(definition.BaseType));
            Attributes = definition.Attributes;
            foreach (var implementation in definition.GetInterfaceImplementations()) {
                var newInterface = new AmbiguousLocalType(reader.GetToken(reader.GetInterfaceImplementation(implementation).Interface));
                interfaceImplementations.Add(newInterface);
            }
        }

        public TypeDef(string name, string theNamespace)
            : base(name, theNamespace)
        {
        }

        public TypeDef(int token)
            : base(token)
        {
        }

        public ILocalTypeEntity BaseType { get; private set; }

        public TypeAttributes Attributes { get; }

        public bool IsInterface => (Attributes & TypeAttributes.Interface) != 0;

        public IEnumerable<ILocalTypeEntity> InterfaceImplementations => interfaceImplementations;

        public static List<TypeDef> LoadDefinitions(MetadataReader reader)
        {
            var query = from definition
                        in reader.TypeDefinitions
                        select new TypeDef(reader, definition);

            return query.ToList();
        }

        public override bool ImplementsInterface(ILocalTypeEntity entity)
        {
            if (entity is TypeDef otherInterface && !otherInterface.IsInterface) {
                return false;
            }

            if (InterfaceImplementations.Contains(entity)) {
                return true;
            }

            foreach (var implementation in InterfaceImplementations) {
                if (implementation.ImplementsInterface(entity)) {
                    return true;
                }
            }

            if (BaseType?.ResolutionStatus == ResolutionStatus.Resolved) {
                if (BaseType.ImplementsInterface(entity)) {
                    return true;
                }
            }

            return false;
        }

        public void LinkBaseType(List<LocalTypeEntity> types)
        {
            if (BaseType.ResolutionStatus == ResolutionStatus.Resolved) {
                return;
            }

            var query = from type
                        in types
                        where type.Token == BaseType.Token
                        select type;

            BaseType = query.FirstOrDefault();
        }

        public void LinkInterfaceImplementations(List<LocalTypeEntity> types)
        {
            var unresolved = from implementation
                             in interfaceImplementations
                             where implementation.ResolutionStatus == ResolutionStatus.UnResolved
                             select implementation;

            var remove = new List<LocalTypeEntity>();
            var resolved = new List<LocalTypeEntity>();

            foreach (var implementation in unresolved.ToList()) {
                var query = from type
                            in types
                            where type.Token == implementation.Token
                            select type;

                if (query.Any()) {
                    remove.Add(implementation);
                    resolved.Add(query.First());
                }
            }

            foreach (var unresolvedItem in remove) {
                interfaceImplementations.Remove(unresolvedItem);
            }

            interfaceImplementations.AddRange(resolved);
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
