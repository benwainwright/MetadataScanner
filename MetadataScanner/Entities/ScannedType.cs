/*
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */
namespace MetadataScanner.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MetadataScanner.Enums;
    using MetadataScanner.Interfaces;
    using MetadataScanner.Utils;

    public class ScannedType : MetadataEntity, IType, IEquatable<IType>
    {
        private readonly IAssembly assembly;

        private readonly TypeAttributes attributes;

        private readonly IType baseType;

        private readonly string definedNamespace;

        private readonly List<IType> interfaces = new List<IType>();

        private readonly bool isLocal;

        private readonly string name;

        public ScannedType(
            int token,
            bool isLocal,
            string name,
            string theNamespace,
            IAssembly assembly = null,
            IType baseType = null,
            TypeAttributes attributes = default(TypeAttributes),
            List<IType> interfaces = null,
            ResolutionStatus resolutionStatus = default(ResolutionStatus))
        {
            Token = token;
            this.isLocal = isLocal;
            this.definedNamespace = theNamespace;
            this.name = name;
            this.baseType = baseType;
            this.attributes = attributes;
            this.assembly = assembly;
            this.interfaces = interfaces;
            ResolutionStatus = resolutionStatus;
        }

        public ScannedType(int token)
        {
            isLocal = true;
            Token = token;
            ResolutionStatus = ResolutionStatus.UnResolved;
        }

        public IAssembly Assembly => Target?.Assembly ?? assembly;

        public TypeAttributes Attributes => Target?.Attributes ?? attributes;

        public IType BaseType => Target?.BaseType ?? baseType;

        public IEnumerable<IType> InterfaceImplementations => Target?.InterfaceImplementations ?? interfaces;

        public bool IsInterface => Target?.IsInterface ?? (attributes & TypeAttributes.Interface) == TypeAttributes.Interface;

        public bool IsLocal => Target?.IsLocal ?? isLocal;

        public string Name => Target?.Name ?? name;

        public string Namespace => Target?.Namespace ?? definedNamespace;

        public IType AsInterface()
        {
            return this;
        }

        public override bool Equals(object obj) => Equals(obj as IType);

        public bool Equals(IType other)
        {
            DebugGuard.Against.Null(other, nameof(other));

            if (other == null) {
                return false;
            }

            if (other == this) {
                return true;
            }

            return other.Name.Equals(Name, StringComparison.InvariantCulture) &&
                   other.Namespace.Equals(Namespace, StringComparison.InvariantCulture) &&
                   other.Assembly.Equals(Assembly);
        }

        public bool ImplementsInterface(IType entity)
        {
            DebugGuard.Against.Null(entity, nameof(entity));
            if (InterfaceImplementations?.Contains(entity) == true) {
                return true;
            }

            if (BaseType?.ImplementsInterface(entity) == true) {
                return true;
            }

            return false;
        }

        public void ResolveExternal(Dictionary<int, IType> entities)
        {
            DebugGuard.Against.Null(entities, nameof(entities));

            if (!IsLocal && ResolutionStatus == ResolutionStatus.UnResolved) {
                var query = from type
                            in entities.Values
                            where type.Name.Equals(name, StringComparison.InvariantCulture) &&
                                   type.Namespace.Equals(definedNamespace, StringComparison.InvariantCulture) &&
                                   type.IsLocal
                            select type;
                if (query.Any()) {
                    Target = query.First();
                }
            }
        }

        public void ResolveLocal(Dictionary<int, IType> entities)
        {
            DebugGuard.Against.Null(entities, nameof(entities));

            if (ResolutionStatus == ResolutionStatus.UnResolved) {
                if (entities.ContainsKey(Token)) {
                    Target = entities[Token];
                }
            }

            ResolveChildren(entities);
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        private void ResolveChildren(Dictionary<int, IType> entities)
        {
            Guard.Against.Null(entities, nameof(entities));
            baseType?.ResolveLocal(entities);

            foreach (var implementation in interfaces) {
                implementation.ResolveLocal(entities);
            }
        }
    }
}
