// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PermanentDelete.PackageBlobRefsEnumeratingConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PermanentDelete
{
  public class PackageBlobRefsEnumeratingConverter<TPackageIdentity, TMetadataEntry> : 
    IConverter<TMetadataEntry, IEnumerable<BlobReferenceIdentifier>>,
    IHaveInputType<TMetadataEntry>,
    IHaveOutputType<IEnumerable<BlobReferenceIdentifier>>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>, IPackageFiles
  {
    private readonly IFeedRequest feedRequest;

    public PackageBlobRefsEnumeratingConverter(IFeedRequest feedRequest) => this.feedRequest = feedRequest;

    public IEnumerable<BlobReferenceIdentifier> Convert(TMetadataEntry metadataEntry)
    {
      FromFilePathRefCalculatingConverter<TPackageIdentity> calculatingConverter = new FromFilePathRefCalculatingConverter<TPackageIdentity>();
      List<BlobReferenceIdentifier> referenceIdentifierList = new List<BlobReferenceIdentifier>();
      foreach (IPackageFile packageFile in metadataEntry.PackageFiles.Where<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId is BlobStorageId)))
      {
        BlobReferenceIdentifier referenceIdentifier = new BlobReferenceIdentifier(BlobIdentifier.Deserialize(packageFile.StorageId.ValueString), calculatingConverter.Convert((IPackageFileRequest<TPackageIdentity>) new PackageFileRequest<TPackageIdentity>(this.feedRequest, metadataEntry.PackageIdentity, packageFile.Path)), metadataEntry.PackageIdentity.Name.Protocol.LowercasedName);
        referenceIdentifierList.Add(referenceIdentifier);
      }
      return (IEnumerable<BlobReferenceIdentifier>) referenceIdentifierList;
    }
  }
}
