/*
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */
namespace MetadataScanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using MetadataScanner.Interfaces;

    public class Scanner
    {
        private List<string> paths = new List<string>();

        private Dictionary<string, AssemblyMetadata> assemblies = new Dictionary<string, AssemblyMetadata>();

        public Scanner(List<string> paths)
        {
            paths.AddRange(paths);
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
                assembly.Value.LinkAssemblies(assemblies.Values.ToList());
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
                assemblies[file] = new AssemblyMetadata(file);
            }
        }
    }
}
