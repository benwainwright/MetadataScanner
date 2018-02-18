namespace MetadataScanner.Utils
{
    using System;

    public static class Guard
    {
        public static class Against
        {
            public static void Null(object value, string name = null)
            {
                if (value == null) {
                    var message = name == null ? $"Cannot be null" : $"{name} cannot be null";
                    throw new ArgumentNullException(message);
                }
            }
        }
    }
}