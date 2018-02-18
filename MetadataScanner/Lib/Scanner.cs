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
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Reflection.Metadata.Ecma335;
    using System.Reflection.PortableExecutable;
    using MetadataScanner.Entities;
    using MetadataScanner.Interfaces;

    internal class Scanner : IAssemblyScanner
    {
        private List<string> paths = new List<string>();

        private Dictionary<string, ScannedAssembly> assemblies = new Dictionary<string, ScannedAssembly>();

        public Scanner(params string[] paths)
        {
            this.paths.AddRange(paths.ToList());
        }

        public IEnumerable<string> Paths => paths;

        public IEnumerable<IAssembly> Assemblies => assemblies.Values.ToList();

        public void AddPath(string path)
        {
            paths.Add(path);
        }

        public void AddPaths(List<string> paths)
        {
            this.paths.AddRange(paths);
        }

        public void Scan()
        {
            assemblies.Clear();
            foreach (var path in paths) {
                var fileAttributes = File.GetAttributes(path);
                if (fileAttributes.HasFlag(FileAttributes.Directory)) {
                    ScanDirectory(path);
                } else if (File.Exists(path)) {
                    ScanAssembly(path);
                }
            }

            foreach (var assembly in assemblies) {
                assembly.Value.ResolveReferences(Assemblies.ToList());
            }
        }

        public void ScanDirectory(string directory)
        {
            foreach (var file in Directory.EnumerateFiles(directory)) {
                ScanAssembly(file);
            }
        }

        public void ScanAssembly(string file)
        {
            var parts = file.Split('.');
            if (parts.Length > 0 && parts[parts.Length - 1].Equals("dll", StringComparison.InvariantCulture)) {
                assemblies[file] = AssemblyFactory.LoadAssembly(file);
            }
        }
    }
}
