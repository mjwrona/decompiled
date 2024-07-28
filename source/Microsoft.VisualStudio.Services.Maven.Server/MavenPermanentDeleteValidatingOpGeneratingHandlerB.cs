// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenPermanentDeleteValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Converters;
using Microsoft.VisualStudio.Services.Maven.Server.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenPermanentDeleteValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageRequest<MavenPackageIdentity>, IPermanentDeleteOperationData, IMavenMetadataAggregationAccessor>
  {
    protected override IAsyncHandler<PackageRequest<MavenPackageIdentity>, IPermanentDeleteOperationData> Bootstrap(
      IMavenMetadataAggregationAccessor metadataAccessor)
    {
      return (IAsyncHandler<PackageRequest<MavenPackageIdentity>, IPermanentDeleteOperationData>) new MavenPermanentDeleteValidatingOpGeneratingHandler((IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>) metadataAccessor.ToPointQueryHandler<MavenPackageIdentity, IMavenMetadataEntry>(), (IConverter<MavenPackageRequestWithMetadata, IEnumerable<BlobReferenceIdentifier>>) new MavenGetExtraAssetsBlobRefIdsHandler((IConverter<IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>, IEnumerable<BlobReferenceIdentifier>>) new MavenPackageFilesToBlobReferenceIdentifiersConverter()));
    }
  }
}
