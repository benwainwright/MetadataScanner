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

    public interface IAssembly : IResolveableEntity<string, IAssembly>, IEquatable<IAssembly>
    {
        string FilePath { get; }

        Version Version { get; }

        List<byte> PublicKey { get; }

        IEnumerable<IAssembly> AssemblyReferences { get; }

        IEnumerable<IType> TypeDefinitions { get; }

        IEnumerable<IType> TypeReferences { get; }
    }
}
