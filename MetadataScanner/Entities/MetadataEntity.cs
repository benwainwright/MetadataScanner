namespace MetadataScanner.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text;
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
