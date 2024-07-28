// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiMetadataHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiMetadataHandlerBootstrapper : 
    IBootstrapper<
    #nullable disable
    IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiMetadataHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry> Bootstrap() => PyPiAggregationResolver.Bootstrap(this.requestContext).HandlerFor<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry, IPyPiMetadataAggregationAccessor, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>((Func<IPyPiMetadataAggregationAccessor, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>, IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry>>) ((metadataAccessor, upstreamVersionListService) =>
    {
      IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> upstreamFetchingMetadata = new PyPiUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>) metadataAccessor, upstreamVersionListService).Bootstrap();
      return ByAsyncFuncAsyncHandler.For<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry>((Func<PackageRequest<PyPiPackageIdentity>, Task<IPyPiMetadataEntry>>) (async request => await upstreamFetchingMetadata.GetPackageVersionStateAsync((IPackageRequest<PyPiPackageIdentity>) request)));
    }));
  }
}
