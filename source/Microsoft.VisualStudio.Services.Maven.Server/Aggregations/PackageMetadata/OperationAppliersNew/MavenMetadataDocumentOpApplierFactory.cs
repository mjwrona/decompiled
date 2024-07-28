// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew.MavenMetadataDocumentOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew
{
  public class MavenMetadataDocumentOpApplierFactory : 
    IMetadataDocumentOpApplierFactory<MetadataDocument<IMavenMetadataEntry>>
  {
    private static readonly ReverseVersionComparer<MavenPackageVersion> VersionComparer = new ReverseVersionComparer<MavenPackageVersion>();
    private static readonly IReadOnlyDictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<IMavenMetadataEntry>>> applierDictionary = (IReadOnlyDictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<IMavenMetadataEntry>>>) new Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<IMavenMetadataEntry>>>()
    {
      {
        typeof (UpdateUpstreamMetadataOperationData<MavenPackageIdentity, MavenPackageName, MavenPackageVersion, IMavenMetadataEntry>),
        (IMetadataDocumentOpApplier<MetadataDocument<IMavenMetadataEntry>>) new MavenUpdateUpstreamMetadataWithFilesOpApplier((IComparer<IPackageVersion>) MavenMetadataDocumentOpApplierFactory.VersionComparer, (IEqualityComparer<IPackageVersion>) MavenMetadataDocumentOpApplierFactory.VersionComparer)
      },
      {
        typeof (MavenCommitOperationData),
        (IMetadataDocumentOpApplier<MetadataDocument<IMavenMetadataEntry>>) new AddSourceChainOpApplier<MavenPackageIdentity, IMavenMetadataEntry>()
      }
    };

    public IMetadataDocumentOpApplier<MetadataDocument<IMavenMetadataEntry>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataDocumentOpApplier<MetadataDocument<IMavenMetadataEntry>> documentOpApplier;
      if (MavenMetadataDocumentOpApplierFactory.applierDictionary.TryGetValue(operationData.GetType(), out documentOpApplier))
        return documentOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataDocumentOpApplier<MetadataDocument<IMavenMetadataEntry>>) null;
    }
  }
}
