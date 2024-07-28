// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Metadata.NpmByBlobMetadataServiceFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Operations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Metadata
{
  public class NpmByBlobMetadataServiceFactoryBootstrapper : 
    IBootstrapper<IFactory<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmByBlobMetadataServiceFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>> Bootstrap()
    {
      RequestContextItemsAsCacheFacade telemetryCache = new RequestContextItemsAsCacheFacade(this.requestContext);
      ITracerService tracerService = this.requestContext.GetTracerFacade();
      NpmMetadataDocumentSerializerFactory serializerFactory = new NpmMetadataDocumentSerializerFactory((ICache<string, object>) telemetryCache, tracerService);
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap();
      return (IFactory<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>) new ByFuncInputFactory<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>((Func<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>) (containerAddress => (IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) new UpstreamFilteringMetadataService<NpmPackageIdentity, INpmMetadataEntry>((IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) new MetadataByBlobService<NpmPackageIdentity, INpmMetadataEntry>(blobServiceFactory, containerAddress, (IFactory<PackageNameQuery<INpmMetadataEntry>, ISerializer<MetadataDocument<INpmMetadataEntry>>>) serializerFactory, (IMetadataEntryOpApplierFactory<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>) new NpmMetadataOpApplierFactory(this.requestContext.GetExecutionEnvironmentFacade()), (IMetadataDocumentOpApplierFactory<MetadataDocument<INpmMetadataEntry>>) new NpmMetadataDocumentOpApplierFactory(tracerService), (IConverter<IPackageNameRequest, Locator>) new ByFuncConverter<IPackageNameRequest, Locator>((Func<IPackageNameRequest, Locator>) (req => new Locator(new string[2]
      {
        req.Feed.Id.ToString(),
        req.PackageName.NormalizedName + ".txt"
      }))), (IComparer<IPackageVersion>) new ReverseVersionComparer<SemanticVersion>(), tracerService, (ICache<string, object>) telemetryCache, (IMetadataChangeValidator<INpmMetadataEntry>) new LocalEntriesCannotDecreaseMetadataChangeValidator<INpmMetadataEntry>()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext))));
    }
  }
}
