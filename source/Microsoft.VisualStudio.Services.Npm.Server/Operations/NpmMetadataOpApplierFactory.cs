// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Operations.NpmMetadataOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Operations
{
  public class NpmMetadataOpApplierFactory : 
    IMetadataEntryOpApplierFactory<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>
  {
    private readonly Dictionary<Type, IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>> applierDictionary;

    public NpmMetadataOpApplierFactory(IExecutionEnvironment executionEnv) => this.applierDictionary = new Dictionary<Type, IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>>()
    {
      {
        typeof (NpmAddOperationData),
        (IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new UpdateSharedMetadataMetadataOpApplier(executionEnv, (IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new NpmAddMetadataOpApplier())
      },
      {
        typeof (NpmDeprecateOperationData),
        (IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new UpdateSharedMetadataMetadataOpApplier(executionEnv, (IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new NpmDeprecateMetadataOpApplier())
      },
      {
        typeof (ViewOperationData),
        (IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new UpdateSharedMetadataMetadataOpApplier(executionEnv, (IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new ViewMetadataOpApplier<INpmMetadataEntry, INpmMetadataEntryWriteable>())
      },
      {
        typeof (DeleteOperationData),
        (IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new UpdateSharedMetadataMetadataOpApplier(executionEnv, (IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new DeleteMetadataOpApplier<INpmMetadataEntry, INpmMetadataEntryWriteable>())
      },
      {
        typeof (RestoreToFeedOperationData),
        (IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new UpdateSharedMetadataMetadataOpApplier(executionEnv, (IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new RestoreToFeedMetadataOpApplier<INpmMetadataEntry, INpmMetadataEntryWriteable, IRestoreToFeedOperationData>())
      },
      {
        typeof (NpmPermanentDeleteOperationData),
        (IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new UpdateSharedMetadataMetadataOpApplier(executionEnv, (IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new PermanentDeleteMetadataOpApplier<INpmMetadataEntry, INpmMetadataEntryWriteable>())
      },
      {
        typeof (UpdateUpstreamMetadataVersionOperationData<NpmPackageIdentity, INpmMetadataEntry>),
        (IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new UpdateUpstreamMetadataVersionOpApplier<NpmPackageIdentity, INpmMetadataEntry>()
      },
      {
        typeof (AddProblemPackageOperationData),
        (IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new NoChangeMetadataOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>()
      }
    };

    public IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>> metadataEntryOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out metadataEntryOpApplier))
        return metadataEntryOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) null;
    }
  }
}
