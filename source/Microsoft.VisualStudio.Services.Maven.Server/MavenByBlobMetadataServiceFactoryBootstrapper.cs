// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenByBlobMetadataServiceFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenByBlobMetadataServiceFactoryBootstrapper : 
    IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenByBlobMetadataServiceFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>> Bootstrap()
    {
      RequestContextItemsAsCacheFacade telemetryCache = new RequestContextItemsAsCacheFacade(this.requestContext);
      ITracerService tracerService = this.requestContext.GetTracerFacade();
      MavenMetadataDocumentSerializerFactory serializerFactory = new MavenMetadataDocumentSerializerFactory((ICache<string, object>) telemetryCache, tracerService);
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IFactory<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>>) new ByFuncInputFactory<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>>((Func<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>>) (containerAddress => (IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>) new UpstreamFileFilteringMetadataService<MavenPackageIdentity, IMavenMetadataEntry>((IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>) new MetadataByBlobService<MavenPackageIdentity, IMavenMetadataEntry>(blobServiceFactory, containerAddress, (IFactory<PackageNameQuery<IMavenMetadataEntry>, ISerializer<MetadataDocument<IMavenMetadataEntry>>>) serializerFactory, (IMetadataEntryOpApplierFactory<IMavenMetadataEntry, MetadataDocument<IMavenMetadataEntry>>) new MavenMetadataOpApplierFactory(), (IMetadataDocumentOpApplierFactory<MetadataDocument<IMavenMetadataEntry>>) new MavenMetadataDocumentOpApplierFactory(), (IConverter<IPackageNameRequest, Locator>) new ByFuncConverter<IPackageNameRequest, Locator>(MavenByBlobMetadataServiceFactoryBootstrapper.\u003C\u003EO.\u003C0\u003E__RequestToLocator ?? (MavenByBlobMetadataServiceFactoryBootstrapper.\u003C\u003EO.\u003C0\u003E__RequestToLocator = new Func<IPackageNameRequest, Locator>(MavenByBlobMetadataServiceFactoryBootstrapper.RequestToLocator))), (IComparer<IPackageVersion>) new ReverseVersionComparer<MavenPackageVersion>(), tracerService, (ICache<string, object>) telemetryCache, (IMetadataChangeValidator<IMavenMetadataEntry>) new LocalFilesCannotDecreaseMetadataChangeValidator<IMavenMetadataEntry>()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext))));
    }

    private static Locator RequestToLocator(IPackageNameRequest req)
    {
      MavenPackageName packageName = (MavenPackageName) req.PackageName;
      return new Locator(new string[3]
      {
        req.Feed.Id.ToString(),
        packageName.NormalizedGroupId,
        packageName.NormalizedArtifactId + ".txt"
      });
    }
  }
}
