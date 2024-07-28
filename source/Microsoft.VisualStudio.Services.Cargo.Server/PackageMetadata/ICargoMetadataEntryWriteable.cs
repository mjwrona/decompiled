// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.ICargoMetadataEntryWriteable
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public interface ICargoMetadataEntryWriteable : 
    IMetadataEntryWriteable<CargoPackageIdentity>,
    IMetadataEntry<CargoPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    IMetadataEntryWritable,
    ICargoMetadataEntry,
    ICreateWriteable<ICargoMetadataEntryWriteable>
  {
    new LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata>? Metadata { get; set; }

    new IReadOnlyList<HashAndType> Hashes { get; set; }

    new bool Yanked { get; set; }

    new ImmutableArray<InnerFileReference> CrateInnerFiles { get; set; }
  }
}
