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

    public interface IAssembly
    {
        string FilePath { get; }

        string Name { get; }

        Version Version { get; }

        List<byte> PublicKey { get; }

        IEnumerable<IAssemblyRef> AssemblyReferences { get; }

        IEnumerable<ITypeDef> TypeDefinitions { get; }

        IEnumerable<ITypeRef> TypeReferences { get; }
    }
}
