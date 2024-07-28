// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetMetadataOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetMetadataOpApplierFactory : 
    IMetadataEntryOpApplierFactory<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>
  {
    private readonly Dictionary<Type, IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>> applierDictionary;

    public NuGetMetadataOpApplierFactory() => this.applierDictionary = new Dictionary<Type, IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>>()
    {
      {
        typeof (NuGetAddOperationData),
        (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new NuGetAddMetadataOpApplier()
      },
      {
        typeof (DelistOperationData),
        (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new NuGetListMetadataOpApplier(false)
      },
      {
        typeof (DeleteOperationData),
        (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new DeleteMetadataOpApplier<INuGetMetadataEntry, INuGetMetadataEntryWriteable>()
      },
      {
        typeof (RelistOperationData),
        (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new NuGetListMetadataOpApplier(true)
      },
      {
        typeof (ViewOperationData),
        (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new ViewMetadataOpApplier<INuGetMetadataEntry, INuGetMetadataEntryWriteable>()
      },
      {
        typeof (NuGetRestoreToFeedOperationData),
        (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new RestoreToFeedMetadataOpApplier<INuGetMetadataEntry, INuGetMetadataEntryWriteable, INuGetRestoreToFeedOperationData>((IHandler<(INuGetMetadataEntryWriteable, INuGetRestoreToFeedOperationData), INuGetMetadataEntryWriteable>) new NuGetRestoreToFeedRepopulateNuspecHandler())
      },
      {
        typeof (PermanentDeleteOperationData),
        (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new PermanentDeleteMetadataOpApplier<INuGetMetadataEntry, INuGetMetadataEntryWriteable>()
      },
      {
        typeof (UpdateUpstreamMetadataVersionOperationData<VssNuGetPackageIdentity, INuGetMetadataEntry>),
        (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new UpdateUpstreamMetadataVersionOpApplier<VssNuGetPackageIdentity, INuGetMetadataEntry>()
      },
      {
        typeof (AddProblemPackageOperationData),
        (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) new NoChangeMetadataOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>()
      }
    };

    public IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>> metadataEntryOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out metadataEntryOpApplier))
        return metadataEntryOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>) null;
    }
  }
}
