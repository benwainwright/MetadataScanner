namespace MetadataScanner.Utils
{
    using System;

    public static class DebugGuard
    {
        public static class Against
        {
            public static void Null(object value, string name = null)
            {
#if DEBUG
                if (value == null) {
                    var message = name == null ? $"Cannot be null" : $"{name} cannot be null";
                    throw new ArgumentNullException(message);
                }
#endif
            }
        }
    }
}
