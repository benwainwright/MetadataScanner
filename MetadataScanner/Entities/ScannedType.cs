namespace MetadataScanner.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MetadataScanner.Enums;
    using MetadataScanner.Interfaces;

    public class ScannedType : MetadataEntity, IType, IEquatable<IType>
    {
        private readonly List<IType> interfaces = new List<IType>();

        private readonly IType baseType;

        private readonly TypeAttributes attributes;

        private readonly IAssembly assembly;

        private readonly string definedNamespace;

        private readonly bool isLocal;

        private readonly string name;

        private IType target;

        public ScannedType(
            int token,
            bool isLocal,
            string name,
            string theNamespace,
            IAssembly assembly = null,
            IType baseType = null,
            TypeAttributes attributes = default(TypeAttributes),
            List<IType> interfaces = null)
        {
            Token = token;
            this.isLocal = isLocal;
            this.definedNamespace = theNamespace;
            this.name = name;
            this.baseType = baseType;
            this.attributes = attributes;
            this.assembly = assembly;
            this.interfaces = interfaces;
        }

        public ScannedType(int token)
        {
            isLocal = true;
            Token = token;
            ResolutionStatus = ResolutionStatus.UnResolved;
        }

        public string Name => target?.Name ?? name;

        public IType BaseType => target?.BaseType ?? baseType;

        public TypeAttributes Attributes => target?.Attributes ?? attributes;

        public IEnumerable<IType> InterfaceImplementations => target?.InterfaceImplementations ?? interfaces;

        public IAssembly Assembly => target?.Assembly ?? assembly;

        public string Namespace => target?.Namespace ?? definedNamespace;

        public bool IsLocal => target?.IsLocal ?? isLocal;

        public bool IsInterface => target?.IsInterface ?? (attributes & TypeAttributes.Interface) == TypeAttributes.Interface;

        public void ResolveLocal(Dictionary<int, IType> entities)
        {
            if (ResolutionStatus == ResolutionStatus.UnResolved) {
                if (entities.ContainsKey(Token)) {
                    target = entities[Token];
                    ResolutionStatus = ResolutionStatus.Resolved;
                }
            }

            ResolveChildren(entities);
        }

        public void ResolveExternal(Dictionary<int, IType> entities)
        {
            if (ResolutionStatus == ResolutionStatus.UnResolved && !IsLocal) {
                var query = from type
                            in entities.Values
                            where type.Name.Equals(name, StringComparison.InvariantCulture) &&
                                  type.Namespace.Equals(definedNamespace, StringComparison.InvariantCulture)
                            select type;
                target = query.FirstOrDefault();
                if (query.Any()) {
                    target = query.First();
                    ResolutionStatus = ResolutionStatus.Resolved;
                }
            }
        }

        public bool ImplementsInterface(IType entity)
        {
            if (InterfaceImplementations?.Contains(entity) == true) {
                return true;
            }

            if (BaseType?.ImplementsInterface(entity) == true) {
                return true;
            }

            return false;
        }

        public override bool Equals(object obj) => Equals(obj as IType);

        public bool Equals(IType other)
        {
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

        public override string ToString()
        {
            return $"{Name}";
        }

        public IType AsInterface()
        {
            return this;
        }

        private static IType ResolveExternalEntity(Dictionary<int, IType> entities, string name, string theNamespace)
        {
            var query = from type
                        in entities.Values
                        where type.Name.Equals(name, StringComparison.InvariantCulture) &&
                                type.Namespace.Equals(theNamespace, StringComparison.InvariantCulture)
                        select type;
            return query.FirstOrDefault();
        }

        private void ResolveChildren(Dictionary<int, IType> entities)
        {
            baseType?.ResolveLocal(entities);

            foreach (var implementation in interfaces) {
                implementation.ResolveLocal(entities);
            }
        }
    }
}
