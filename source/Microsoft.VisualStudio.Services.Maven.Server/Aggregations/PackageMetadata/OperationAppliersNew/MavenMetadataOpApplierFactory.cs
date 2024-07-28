// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew.MavenMetadataOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew
{
  public class MavenMetadataOpApplierFactory : 
    IMetadataEntryOpApplierFactory<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>
  {
    private static readonly IReadOnlyDictionary<Type, IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>> applierDictionary = (IReadOnlyDictionary<Type, IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>>) new Dictionary<Type, IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>>()
    {
      {
        typeof (MavenCommitOperationData),
        (IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>) new MavenAddMetadataOpApplier()
      },
      {
        typeof (ViewOperationData),
        (IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>) new ViewMetadataOpApplier<IMavenMetadataEntry, IMavenMetadataEntryWritable>()
      },
      {
        typeof (DeleteOperationData),
        (IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>) new DeleteMetadataOpApplier<IMavenMetadataEntry, IMavenMetadataEntryWritable>()
      },
      {
        typeof (MavenRestoreToFeedOperationData),
        (IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>) new RestoreToFeedMetadataOpApplier<IMavenMetadataEntry, IMavenMetadataEntryWritable, IRestoreToFeedOperationData>()
      },
      {
        typeof (MavenPermanentDeleteOperationData),
        (IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>) new PermanentDeleteMetadataOpApplier<IMavenMetadataEntry, IMavenMetadataEntryWritable>()
      },
      {
        typeof (MavenSnapshotCleanupOperationData),
        (IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>) new MavenSnapshotCleanupOpApplier()
      },
      {
        typeof (AddProblemPackageOperationData),
        (IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>) new NoChangeMetadataOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>()
      }
    };

    public IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>> metadataEntryOpApplier;
      if (MavenMetadataOpApplierFactory.applierDictionary.TryGetValue(operationData.GetType(), out metadataEntryOpApplier))
        return metadataEntryOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataEntryOpApplier<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>) null;
    }
  }
}
