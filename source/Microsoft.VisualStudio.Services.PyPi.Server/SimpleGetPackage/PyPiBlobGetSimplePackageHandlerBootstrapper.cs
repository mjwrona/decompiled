// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage.PyPiBlobGetSimplePackageHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Name;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage
{
  public class PyPiBlobGetSimplePackageHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawPackageNameRequest, HttpResponseMessage, IPyPiMetadataAggregationAccessor, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiBlobGetSimplePackageHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawPackageNameRequest, HttpResponseMessage> Bootstrap(
      IPyPiMetadataAggregationAccessor metadataAccessor,
      IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion> upstreamVersionListService)
    {
      IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> metadataService = new PyPiUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>) metadataAccessor, upstreamVersionListService).Bootstrap();
      ILocationFacade locationFacade = this.requestContext.GetLocationFacade();
      IAsyncHandler<BatchPackageFileRequest<PyPiPackageIdentity>, IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri>> uriCalculatingHandler = (IAsyncHandler<BatchPackageFileRequest<PyPiPackageIdentity>, IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri>>) new PyPiDownloadUriCalculator(locationFacade);
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IConverter<RawPackageNameRequest, (RawPackageNameRequest, PackageNameRequest<PyPiPackageName>)> converter = new PyPiRawPackageNameRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap().KeepInput<RawPackageNameRequest, PackageNameRequest<PyPiPackageName>>();
      return converter.ThenDelegateTo<RawPackageNameRequest, (RawPackageNameRequest, PackageNameRequest<PyPiPackageName>), HttpResponseMessage>(UntilNonNullHandler.Create<(RawPackageNameRequest, PackageNameRequest<PyPiPackageName>), HttpResponseMessage>((IAsyncHandler<(RawPackageNameRequest, PackageNameRequest<PyPiPackageName>), HttpResponseMessage>) new RedirectNonNormalizedSimpleRequestsHandler(locationFacade), ConvertFrom.OutputTypeOf<(RawPackageNameRequest, PackageNameRequest<PyPiPackageName>)>((IHaveOutputType<(RawPackageNameRequest, PackageNameRequest<PyPiPackageName>)>) converter).By<(RawPackageNameRequest, PackageNameRequest<PyPiPackageName>), PackageNameRequest<PyPiPackageName>>((Func<(RawPackageNameRequest, PackageNameRequest<PyPiPackageName>), PackageNameRequest<PyPiPackageName>>) (x => x.Out)).ThenDelegateTo<(RawPackageNameRequest, PackageNameRequest<PyPiPackageName>), PackageNameRequest<PyPiPackageName>, HttpResponseMessage>((IAsyncHandler<PackageNameRequest<PyPiPackageName>, HttpResponseMessage>) new PyPiBlobGetSimplePackageHandler((IReadMetadataService<PyPiPackageIdentity, IPyPiMetadataEntry>) metadataService, uriCalculatingHandler, tracerFacade))));
    }
  }
}
