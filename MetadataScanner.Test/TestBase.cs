namespace MetadataScanner.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MetadataScanner.Interfaces;

    public class TestBase : IComparer<IAssembly>
    {
        public int Compare(IAssembly x, IAssembly y)
        {
            if (x.Name == null || y.Name == null) {
                return -1;
            }

            if (x.Version == null || y.Version == null) {
                return -1;
            }

            if (x.PublicKey == null || y.PublicKey == null) {
                return -1;
            }

            return x.Name.Equals(y.Name, StringComparison.InvariantCulture) &&
                   x.Version.Equals(y.Version) &&
                   x.PublicKey.SequenceEqual(y.PublicKey)
                   ? 0
                   : -1;
        }
    }
}
