// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenGetPackageFileMetadataHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenGetPackageFileMetadataHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<MavenFileRequest, MavenPackageFileResponse, IMavenMetadataAggregationAccessor, IMavenPluginMetadataStoreAggregationAccessor, IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenGetPackageFileMetadataHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<MavenFileRequest, MavenPackageFileResponse> Bootstrap(
      IMavenMetadataAggregationAccessor metadataAggregation,
      IMavenPluginMetadataStoreAggregationAccessor pluginAggregation,
      IUpstreamVersionListService<MavenPackageName, MavenPackageVersion> upstreamVersionListService)
    {
      return new MavenGetPackageFileMetadataHandler((IReadMetadataService<MavenPackageIdentity, IMavenMetadataEntry>) new MavenUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>) metadataAggregation, upstreamVersionListService).Bootstrap(), (IMavenPluginMetadataStore) pluginAggregation, this.requestContext.GetFeatureFlagFacade()).ThenForwardResultTo<MavenFileRequest, MavenPackageFileResponse>(new MavenDownloadCiDataFacadeHandler(this.requestContext).ThenDelegateTo<MavenPackageFileResponse, ICiData>((IAsyncHandler<ICiData>) new TelemetryPublisherUsingTracerFacadeBootstrapper(this.requestContext).Bootstrap()));
    }
  }
}
