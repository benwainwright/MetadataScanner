﻿namespace CleanIoc.Metadata.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata;
    using System.Reflection.Metadata.Ecma335;
    using CleanIoc.Metadata.Enums;

    public class AssemblyRef : EntityWithToken
    {
        public AssemblyRef(MetadataReader reader, AssemblyReferenceHandle handle)
            : base(reader.GetToken(handle), ResolutionStatus.Resolved)
        {
            var reference = reader.GetAssemblyReference(handle);
            Name = reader.GetString(reference.Name);
            Culture = reader.GetString(reference.Culture);
            PublicKeyOrToken = reader.GetBlobBytes(reference.PublicKeyOrToken).ToList();
            Version = reference.Version;
            Flags = reference.Flags;
        }

        public AssemblyMetadata MetaData { get; private set; }

        public string Culture { get; }

        public string Name { get; }

        public List<byte> PublicKeyOrToken { get; }

        public Version Version { get; }

        public AssemblyFlags Flags { get; }

        public static List<AssemblyRef> LoadReferences(MetadataReader reader)
        {
            var query = from reference
                        in reader.AssemblyReferences
                        select new AssemblyRef(reader, reference);

            return query.ToList();
        }

        public void LinkAssemblyMetaData(IEnumerable<AssemblyMetadata> assemblies)
        {
            if (MetaData != null) {
                return;
            }

            var query = from assembly
                        in assemblies
                        where assembly.Name.Equals(Name, StringComparison.InvariantCulture) &&
                              assembly.PublicKey.SequenceEqual(PublicKeyOrToken) &&
                              assembly.Version.Equals(Version)
                        select assembly;

            MetaData = query.FirstOrDefault();
        }

        public override string ToString()
        {
            return $"{Name} {Version}";
        }
    }
}