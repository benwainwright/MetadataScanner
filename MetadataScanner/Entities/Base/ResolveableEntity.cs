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

    internal abstract class ResolveableEntity : IResolveableEntity
    {
        protected ResolveableEntity(string name, int token, ResolutionStatus resolutionStatus)
        {
            Name = name;
            Token = token;
            ResolutionStatus = resolutionStatus;
        }

        protected ResolveableEntity(string name, int token)
            : this(name, token, ResolutionStatus.Resolved)
        {
        }

        protected ResolveableEntity(int token)
            : this(null, token, ResolutionStatus.UnResolved)
        {
        }

        protected ResolveableEntity(string name)
            : this(name, 0, ResolutionStatus.UnResolved)
        {
        }

        protected ResolveableEntity(ResolutionStatus resolutionStatus)
            : this(null, 0, resolutionStatus)
        {
        }

        public int Token { get; }

        public string Name { get; }

        public ResolutionStatus ResolutionStatus { get; }
    }
}
