namespace MetadataScanner.Interfaces
{
    using System.Collections.Generic;
    using System.Reflection;

    public interface IType : IResolveableEntity<int, IType>
    {
        IAssembly Assembly { get; }

        string Namespace { get; }

        bool IsLocal { get; }

        bool IsAbstract { get; }

        TypeAttributes Attributes { get; }

        IType BaseType { get; }

        IEnumerable<IType> InterfaceImplementations { get; }

        bool IsInterface { get; }

        string ToString();

        bool ImplementsInterface(IType entity);

        bool IsSubclassOf(IType entity);
    }
}
