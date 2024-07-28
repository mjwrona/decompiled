// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Ingestion.CargoParsedIngestionParams
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Ingestion
{
  public record CargoParsedIngestionParams : 
    IParsedPackageIngestionParams<CargoPackageIdentity, SimplePackageFileName>
  {
    public CargoParsedIngestionParams(
      CargoPackageIdentity PackageIdentity,
      LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> Metadata,
      Stream CrateStream,
      BlobStorageId StorageId,
      IngestionDirection IngestionDirection,
      ImmutableArray<HashAndType> Hashes,
      ImmutableArray<(CapturedFile CapturedFile, InnerFileReference InnerFileReference)> InnerFiles,
      bool AddAsYanked)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CPackageIdentity\u003Ek__BackingField = PackageIdentity;
      // ISSUE: reference to a compiler-generated field
      this.\u003CMetadata\u003Ek__BackingField = Metadata;
      // ISSUE: reference to a compiler-generated field
      this.\u003CCrateStream\u003Ek__BackingField = CrateStream;
      // ISSUE: reference to a compiler-generated field
      this.\u003CStorageId\u003Ek__BackingField = StorageId;
      // ISSUE: reference to a compiler-generated field
      this.\u003CIngestionDirection\u003Ek__BackingField = IngestionDirection;
      // ISSUE: reference to a compiler-generated field
      this.\u003CHashes\u003Ek__BackingField = Hashes;
      // ISSUE: reference to a compiler-generated field
      this.\u003CInnerFiles\u003Ek__BackingField = InnerFiles;
      // ISSUE: reference to a compiler-generated field
      this.\u003CAddAsYanked\u003Ek__BackingField = AddAsYanked;
      this.Size = CrateStream.Length;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public CargoPackageIdentity PackageIdentity { get; init; }

    public LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> Metadata { get; init; }

    public Stream CrateStream { get; init; }

    public BlobStorageId StorageId { get; init; }

    public IngestionDirection IngestionDirection { get; init; }

    public ImmutableArray<HashAndType> Hashes { get; init; }

    public ImmutableArray<(CapturedFile CapturedFile, InnerFileReference InnerFileReference)> InnerFiles { get; init; }

    public bool AddAsYanked { get; init; }

    public SimplePackageFileName FileName => new SimplePackageFileName(this.PackageIdentity.GetCanonicalCrateFileName());

    public long Size { get; }

    [CompilerGenerated]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("PackageIdentity = ");
      builder.Append((object) this.PackageIdentity);
      builder.Append(", Metadata = ");
      builder.Append((object) this.Metadata);
      builder.Append(", CrateStream = ");
      builder.Append((object) this.CrateStream);
      builder.Append(", StorageId = ");
      builder.Append((object) this.StorageId);
      builder.Append(", IngestionDirection = ");
      builder.Append(this.IngestionDirection.ToString());
      builder.Append(", Hashes = ");
      builder.Append(this.Hashes.ToString());
      builder.Append(", InnerFiles = ");
      builder.Append(this.InnerFiles.ToString());
      builder.Append(", AddAsYanked = ");
      builder.Append(this.AddAsYanked.ToString());
      builder.Append(", FileName = ");
      builder.Append((object) this.FileName);
      builder.Append(", Size = ");
      builder.Append(this.Size.ToString());
      return true;
    }

    [CompilerGenerated]
    public void Deconstruct(
      out CargoPackageIdentity PackageIdentity,
      out LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> Metadata,
      out Stream CrateStream,
      out BlobStorageId StorageId,
      out IngestionDirection IngestionDirection,
      out ImmutableArray<HashAndType> Hashes,
      out ImmutableArray<(CapturedFile CapturedFile, InnerFileReference InnerFileReference)> InnerFiles,
      out bool AddAsYanked)
    {
      PackageIdentity = this.PackageIdentity;
      Metadata = this.Metadata;
      CrateStream = this.CrateStream;
      StorageId = this.StorageId;
      IngestionDirection = this.IngestionDirection;
      Hashes = this.Hashes;
      InnerFiles = this.InnerFiles;
      AddAsYanked = this.AddAsYanked;
    }
  }
}
