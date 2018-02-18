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
    using MetadataScanner.Enums;
    using MetadataScanner.Interfaces;
    using MetadataScanner.Utils;

    public abstract class MetadataEntity
    {
        private IType target;

        public int Token { get; protected set; }

        public ResolutionStatus ResolutionStatus { get; protected set; }

        protected IType Target
        {
            get => target;

            set
            {
                Guard.Against.Null(value);

                if (target == value) {
                    throw new InvalidOperationException("Tried to set object target to the same instance");
                }

                ResolutionStatus = ResolutionStatus.Resolved;
                target = value;
            }
        }
    }
}
