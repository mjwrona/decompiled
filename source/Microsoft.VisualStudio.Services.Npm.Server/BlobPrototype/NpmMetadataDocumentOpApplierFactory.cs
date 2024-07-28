// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype.NpmMetadataDocumentOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype
{
  public class NpmMetadataDocumentOpApplierFactory : 
    IMetadataDocumentOpApplierFactory<MetadataDocument<INpmMetadataEntry>>
  {
    private readonly Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>> applierDictionary;

    public NpmMetadataDocumentOpApplierFactory(ITracerService tracerService)
    {
      AddSourceChainOpApplier<NpmPackageIdentity, INpmMetadataEntry> sourceChainOpApplier = new AddSourceChainOpApplier<NpmPackageIdentity, INpmMetadataEntry>();
      ReverseVersionComparer<SemanticVersion> reverseVersionComparer = new ReverseVersionComparer<SemanticVersion>();
      this.applierDictionary = new Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>>()
      {
        {
          typeof (UpdateUpstreamMetadataOperationData<NpmPackageIdentity, NpmPackageName, SemanticVersion, INpmMetadataEntry>),
          (IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>) new UpdateUpstreamMetadataOpApplier<NpmPackageIdentity, NpmPackageName, SemanticVersion, INpmMetadataEntry, INpmMetadataEntryWriteable>((IComparer<IPackageVersion>) reverseVersionComparer, (IEqualityComparer<IPackageVersion>) reverseVersionComparer, tracerService)
        },
        {
          typeof (NpmAddOperationData),
          (IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>) sourceChainOpApplier
        },
        {
          typeof (NpmDistTagSetOperationData),
          (IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>) new NpmSetDistTagOpApplier()
        },
        {
          typeof (NpmDistTagRemoveOperationData),
          (IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>) new NpmDistTagRemoveOpApplier()
        },
        {
          typeof (NpmMetadataDiffOperationData),
          (IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>) new NpmMetadataDiffOpApplier()
        }
      };
    }

    public IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>> documentOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out documentOpApplier))
        return documentOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>) null;
    }
  }
}
