/*
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */
namespace MetadataScanner.Entities.Base
{
    using MetadataScanner.Enums;
    using MetadataScanner.Interfaces;

    internal class TypeEntity : EntityWithToken, ITypeEntity
    {
        public TypeEntity(string name, int token)
            : base(token, ResolutionStatus.Resolved)
        {
            Name = name;
        }

        public TypeEntity(string name)
            : base(ResolutionStatus.UnResolved)
        {
            Name = name;
        }

        public TypeEntity(int token)
            : base(token, ResolutionStatus.UnResolved)
        {
        }

        public string Name { get; }
    }
}
