namespace MetadataScanner.Interfaces
{
    using System.Collections.Generic;

    public interface IAssemblyScanner
    {
        IEnumerable<IAssembly> Assemblies { get; }

        IEnumerable<string> Paths { get; }

        void AddPath(string path);

        void AddPaths(List<string> paths);

        void Scan();

        void ScanAssembly(string file);

        void ScanDirectory(string directory);
    }
}