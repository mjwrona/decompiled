// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.RegistrationPackageIndexBlobHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class RegistrationPackageIndexBlobHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<NuGetGetPackageIndexRequest<RawPackageNameRequest>, HttpResponseMessage, INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public RegistrationPackageIndexBlobHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<NuGetGetPackageIndexRequest<RawPackageNameRequest>, HttpResponseMessage> Bootstrap(
      INuGetMetadataService metadataAccessor,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      IRegistryService registryService = (IRegistryService) new RegistryServiceFacade(this.requestContext);
      this.requestContext.GetFeatureFlagFacade();
      ILocationFacade locationFacade = this.requestContext.GetLocationFacade();
      NuGetExtractInnerFileFromNupkgUriCalculator innerFileUriCalculator = new NuGetExtractInnerFileFromNupkgUriCalculator(locationFacade);
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService = new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataAccessor, upstreamVersionListService, true).Bootstrap();
      RegistrationPackageIndexHandler handler = new RegistrationPackageIndexHandler(locationFacade, registryService, (IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataService, (IAsyncHandler<PackageNameRequest<VssNuGetPackageName, List<INuGetMetadataEntry>>, JObject>) new RegistrationsPageDetailsHandler(locationFacade, (IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>) new NuGetDownloadV3UriCalculator(locationFacade), (IRegistrationsPackageFrameCreator) new RegistrationsPackageFrameCreator(locationFacade, tracerFacade), tracerFacade, (INuGetLicenseUriCalculator) new NuGetLicenseUriCalculator((INuGetExtractInnerFileFromNupkgUriCalculator) innerFileUriCalculator), (INuGetIconUriCalculator) new NuGetIconUriCalculator((INuGetExtractInnerFileFromNupkgUriCalculator) innerFileUriCalculator)), (IRegistrationsPackageFrameCreator) new RegistrationsPackageFrameCreator(locationFacade, tracerFacade), tracerFacade);
      return new NuGetGetPackageIndexRawToResolvedRequestConverter(new NuGetPackageNameParsingConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<NuGetGetPackageIndexRequest<RawPackageNameRequest>, NuGetGetPackageIndexRequest<PackageNameRequest<VssNuGetPackageName>>, HttpResponseMessage>((IAsyncHandler<NuGetGetPackageIndexRequest<PackageNameRequest<VssNuGetPackageName>>, HttpResponseMessage>) handler);
    }
  }
}
