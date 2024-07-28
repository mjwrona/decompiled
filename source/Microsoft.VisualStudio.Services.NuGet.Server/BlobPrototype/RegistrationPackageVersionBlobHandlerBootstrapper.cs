// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.RegistrationPackageVersionBlobHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class RegistrationPackageVersionBlobHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawPackageRequest, HttpResponseMessage, INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public RegistrationPackageVersionBlobHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawPackageRequest, HttpResponseMessage> Bootstrap(
      INuGetMetadataService metadataAccessor,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      ILocationFacade locationFacade = this.requestContext.GetLocationFacade();
      ThrowIfNotFoundOrDeletedDelegatingMetadataStore metadataService = new ThrowIfNotFoundOrDeletedDelegatingMetadataStore((IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataAccessor, upstreamVersionListService, true).Bootstrap());
      return (IAsyncHandler<RawPackageRequest, HttpResponseMessage>) new NuGetRawPackageRequestConverter(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<IRawPackageRequest, PackageRequest<VssNuGetPackageIdentity>, HttpResponseMessage>((IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, HttpResponseMessage>) new RegistrationPackageVersionHandler((IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataService, locationFacade, (IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>) new NuGetDownloadV3UriCalculator(locationFacade)));
    }
  }
}
