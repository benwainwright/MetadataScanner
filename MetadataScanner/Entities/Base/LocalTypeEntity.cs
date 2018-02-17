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
    using System;
    using MetadataScanner.Enums;
    using MetadataScanner.Interfaces;

    internal abstract class LocalTypeEntity : ResolveableEntity, IEquatable<ILocalTypeEntity>, ILocalTypeEntity
    {
        private const int HashingPrimeSeventeen = 17;

        private const int HashingPrimeTwentyThree = 23;

        protected LocalTypeEntity(string name, string theNamespace, int token)
            : base(name, token, ResolutionStatus.Resolved)
        {
            DeclaredNamespace = theNamespace;
        }

        protected LocalTypeEntity(string name, string theNamespace)
            : base(name)
        {
            DeclaredNamespace = theNamespace;
        }

        protected LocalTypeEntity(int token)
            : base(token)
        {
        }

        public string DeclaredNamespace { get; }

        public abstract bool ImplementsInterface(ILocalTypeEntity entity);

        public override bool Equals(object obj)
        {
            return Equals(obj as ILocalTypeEntity);
        }

        public bool Equals(ILocalTypeEntity other)
        {
            if (other == null) {
                return false;
            }

            if (other == this) {
                return true;
            }

            if (other.Name == null || Name == null) {
                return false;
            }

            return other.Name.Equals(Name, StringComparison.InvariantCulture) &&
                   other.DeclaredNamespace?.Equals(DeclaredNamespace, StringComparison.InvariantCulture) == true;
        }

        public override int GetHashCode()
        {
            unchecked {
                var hash = HashingPrimeSeventeen;
                hash = (HashingPrimeTwentyThree * 23) + Name.GetHashCode();
                hash = (HashingPrimeTwentyThree * 23) + Name.GetHashCode();
                return hash;
            }
        }
    }
}
