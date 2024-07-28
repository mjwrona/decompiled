// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetMetadataDocumentOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetMetadataDocumentOpApplierFactory : 
    IMetadataDocumentOpApplierFactory<MetadataDocument<INuGetMetadataEntry>>
  {
    private readonly Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<INuGetMetadataEntry>>> applierDictionary;

    public NuGetMetadataDocumentOpApplierFactory(ITracerService tracerService)
    {
      AddSourceChainOpApplier<VssNuGetPackageIdentity, INuGetMetadataEntry> sourceChainOpApplier = new AddSourceChainOpApplier<VssNuGetPackageIdentity, INuGetMetadataEntry>();
      ReverseVersionComparer<VssNuGetPackageVersion> reverseVersionComparer = new ReverseVersionComparer<VssNuGetPackageVersion>();
      this.applierDictionary = new Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<INuGetMetadataEntry>>>()
      {
        {
          typeof (UpdateUpstreamMetadataOperationData<VssNuGetPackageIdentity, VssNuGetPackageName, VssNuGetPackageVersion, INuGetMetadataEntry>),
          (IMetadataDocumentOpApplier<MetadataDocument<INuGetMetadataEntry>>) new UpdateUpstreamMetadataOpApplier<VssNuGetPackageIdentity, VssNuGetPackageName, VssNuGetPackageVersion, INuGetMetadataEntry, INuGetMetadataEntryWriteable>((IComparer<IPackageVersion>) reverseVersionComparer, (IEqualityComparer<IPackageVersion>) reverseVersionComparer, tracerService)
        },
        {
          typeof (NuGetAddOperationData),
          (IMetadataDocumentOpApplier<MetadataDocument<INuGetMetadataEntry>>) sourceChainOpApplier
        }
      };
    }

    public IMetadataDocumentOpApplier<MetadataDocument<INuGetMetadataEntry>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataDocumentOpApplier<MetadataDocument<INuGetMetadataEntry>> documentOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out documentOpApplier))
        return documentOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataDocumentOpApplier<MetadataDocument<INuGetMetadataEntry>>) null;
    }
  }
}
