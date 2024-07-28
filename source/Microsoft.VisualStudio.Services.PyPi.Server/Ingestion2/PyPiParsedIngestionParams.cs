// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion2.PyPiParsedIngestionParams
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion2
{
  public class PyPiParsedIngestionParams : 
    IParsedPackageIngestionParams<PyPiPackageIdentity, SimplePackageFileName>
  {
    public PyPiParsedIngestionParams(
      PyPiPackageIdentity packageIdentity,
      IReadOnlyDictionary<string, string[]> metadataFields,
      PyPiResolvedMetadata resolvedMetadata,
      DeflateCompressibleBytes gpgSignature,
      PackageFileStream packageFileStream,
      IEnumerable<HashAndType> hashes,
      IStorageId storageId,
      IngestionDirection ingestionDirection)
    {
      this.PackageIdentity = packageIdentity;
      this.MetadataFields = metadataFields;
      this.ResolvedMetadata = resolvedMetadata;
      this.GpgSignature = gpgSignature;
      this.PackageFileStream = packageFileStream;
      this.StorageId = storageId;
      this.IngestionDirection = ingestionDirection;
      this.Hashes = hashes.ToImmutableList<HashAndType>();
    }

    public PyPiPackageIdentity PackageIdentity { get; }

    public IReadOnlyDictionary<string, string[]> MetadataFields { get; }

    public PyPiResolvedMetadata ResolvedMetadata { get; }

    public DeflateCompressibleBytes GpgSignature { get; }

    public PackageFileStream PackageFileStream { get; }

    public IStorageId StorageId { get; }

    public IngestionDirection IngestionDirection { get; }

    public ImmutableList<HashAndType> Hashes { get; }

    public SimplePackageFileName FileName => new SimplePackageFileName(this.PackageFileStream.FilePath);
  }
}
