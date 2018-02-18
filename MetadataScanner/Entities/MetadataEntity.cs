namespace MetadataScanner.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MetadataScanner.Enums;

    public abstract class MetadataEntity
    {
        public int Token { get; protected set; }

        public ResolutionStatus ResolutionStatus { get; protected set; }
    }
}
