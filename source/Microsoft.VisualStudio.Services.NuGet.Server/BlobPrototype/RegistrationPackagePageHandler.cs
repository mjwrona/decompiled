// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.RegistrationPackagePageHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class RegistrationPackagePageHandler : 
    TracingHandler<
    #nullable disable
    PackageRangeWithDataRequest<VssNuGetPackageName, VssNuGetPackageVersion, IncludeSemVer2VersionsFlag>, HttpResponseMessage>
  {
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;
    private readonly IAsyncHandler<PackageNameRequest<VssNuGetPackageName, List<INuGetMetadataEntry>>, JObject> pageWithDetailsHandler;

    public RegistrationPackagePageHandler(
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      IAsyncHandler<PackageNameRequest<VssNuGetPackageName, List<INuGetMetadataEntry>>, JObject> pageWithDetailsHandler,
      ITracerService tracerService)
      : base(tracerService)
    {
      this.metadataService = metadataService;
      this.pageWithDetailsHandler = pageWithDetailsHandler;
    }

    public override async Task<HttpResponseMessage> Handle(
      PackageRangeWithDataRequest<VssNuGetPackageName, VssNuGetPackageVersion, IncludeSemVer2VersionsFlag> request,
      ITracerBlock tracer)
    {
      PackageNameRequest<VssNuGetPackageName> packageNameRequest = new PackageNameRequest<VssNuGetPackageName>((IFeedRequest) request, request.PackageRequest.PackageName);
      NuGetPackageNameQuery packageNameQueryRequest = new NuGetPackageNameQuery((IPackageNameRequest<IPackageName>) packageNameRequest);
      packageNameQueryRequest.Options = new QueryOptions<INuGetMetadataEntry>()
      {
        VersionLower = ((IPackageVersion) request.PackageRequest.PackageVersionLower),
        VersionUpper = ((IPackageVersion) request.PackageRequest.PackageVersionUpper)
      }.WithFilter((Func<INuGetMetadataEntry, bool>) (x => !x.DeletedDate.HasValue));
      List<INuGetMetadataEntry> source = await this.metadataService.GetPackageVersionStatesAsync((PackageNameQuery<INuGetMetadataEntry>) packageNameQueryRequest);
      if (!request.AdditionalData.IncludeSemVer2Versions)
        source = source.Where<INuGetMetadataEntry>((Func<INuGetMetadataEntry, bool>) (x => !x.PackageIdentity.Version.NuGetVersion.HasMetadata)).ToList<INuGetMetadataEntry>();
      VersionRange pageRange = new VersionRange(NuGetVersion.Parse(request.PackageRequest.PackageVersionLower.DisplayVersion), maxVersion: NuGetVersion.Parse(request.PackageRequest.PackageVersionUpper.DisplayVersion), includeMaxVersion: true);
      List<INuGetMetadataEntry> list = source.Where<INuGetMetadataEntry>((Func<INuGetMetadataEntry, bool>) (x => pageRange.Satisfies(x.PackageIdentity.Version.NuGetVersion, VersionComparison.VersionRelease))).ToList<INuGetMetadataEntry>();
      if (!list.Any<INuGetMetadataEntry>())
        throw ControllerExceptionHelper.PackageNotFound_LegacyNuGetSpecificType((IPackageName) request.PackageRequest.PackageName, request.Feed);
      JObject jObject = await this.pageWithDetailsHandler.Handle(new PackageNameRequest<VssNuGetPackageName, List<INuGetMetadataEntry>>((IPackageNameRequest<VssNuGetPackageName>) packageNameRequest, list));
      jObject.Add("@context", (JToken) V3RegistrationsController.ContextObjects.PackageIndex);
      SecureJObject secureJobject = jObject.ToSecureJObject((IFeedRequest) request);
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) new ObjectContent<SecureJObject>(secureJobject, (MediaTypeFormatter) new ServerVssJsonMediaTypeFormatter(true))
      };
      packageNameRequest = (PackageNameRequest<VssNuGetPackageName>) null;
      return httpResponseMessage;
    }
  }
}
