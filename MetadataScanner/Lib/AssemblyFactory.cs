namespace MetadataScanner.Lib
{
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;
    using MetadataScanner.Interfaces;

    internal static class AssemblyFactory
    {
        public static string MapNamePrefix { get; private set; }

        public static unsafe ScannedAssembly LoadAssembly(
            string filename,
            MetadataReaderOptions options = default(MetadataReaderOptions),
            MetadataStringDecoder decoder = null)
        {
            using (var file = LoadAssembly(filename, out var length, out var access)) {
                using (var stream = file.CreateViewStream(0x0, length, access)) {
                    var headers = new PEHeaders(stream);
                    var start = (byte*)0;
                    stream.SafeMemoryMappedViewHandle.AcquirePointer(ref start);
                    var size = headers.MetadataSize;
                    var reader = new MetadataReader(start + headers.MetadataStartOffset, size, options, decoder);
                    return new ScannedAssembly(filename, reader);
                }
            }
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
    }
}
