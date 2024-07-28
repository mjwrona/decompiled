// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.RegistrationPackageIndexHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Newtonsoft.Json.Linq;
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
  public class RegistrationPackageIndexHandler : 
    TracingHandler<
    #nullable disable
    NuGetGetPackageIndexRequest<PackageNameRequest<VssNuGetPackageName>>, HttpResponseMessage>
  {
    private const int DefaultPageSize = 5010;
    private readonly ILocationFacade locationFacade;
    private readonly IRegistryService registryService;
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;
    private readonly IAsyncHandler<PackageNameRequest<VssNuGetPackageName, List<INuGetMetadataEntry>>, JObject> pageWithDetailsHandler;
    private readonly IRegistrationsPackageFrameCreator registrationsPackageFrameCreator;

    public RegistrationPackageIndexHandler(
      ILocationFacade locationFacade,
      IRegistryService registryService,
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      IAsyncHandler<PackageNameRequest<VssNuGetPackageName, List<INuGetMetadataEntry>>, JObject> pageWithDetailsHandler,
      IRegistrationsPackageFrameCreator registrationsPackageFrameCreator,
      ITracerService tracerService)
      : base(tracerService)
    {
      this.locationFacade = locationFacade;
      this.registryService = registryService;
      this.metadataService = metadataService;
      this.pageWithDetailsHandler = pageWithDetailsHandler;
      this.registrationsPackageFrameCreator = registrationsPackageFrameCreator;
    }

    public override async Task<HttpResponseMessage> Handle(
      NuGetGetPackageIndexRequest<PackageNameRequest<VssNuGetPackageName>> request,
      ITracerBlock tracerBlock)
    {
      NuGetPackageNameQuery packageNameQueryRequest = new NuGetPackageNameQuery((IPackageNameRequest<IPackageName>) request.PackageRequest);
      packageNameQueryRequest.Options = new QueryOptions<INuGetMetadataEntry>().WithFilter((Func<INuGetMetadataEntry, bool>) (x => !x.IsDeleted()));
      List<INuGetMetadataEntry> getMetadataEntryList = await this.metadataService.GetPackageVersionStatesAsync((PackageNameQuery<INuGetMetadataEntry>) packageNameQueryRequest);
      if (!request.IncludeSemVer2Versions)
        getMetadataEntryList = getMetadataEntryList.Where<INuGetMetadataEntry>((Func<INuGetMetadataEntry, bool>) (x => !x.PackageIdentity.Version.NuGetVersion.HasMetadata)).ToList<INuGetMetadataEntry>();
      Uri registrationUri = this.locationFacade.GetResourceUri("nuget", ResourceIds.Registrations2PackageResourceId, (IFeedRequest) request, PackagingUriNamePreference.PreferCanonicalId, (object) new
      {
        packageId = request.PackageRequest.PackageName.NormalizedName
      });
      if (!getMetadataEntryList.Any<INuGetMetadataEntry>())
        throw ControllerExceptionHelper.PackageNotFound_LegacyNuGetSpecificType((IPackageName) request.PackageRequest.PackageName, request.Feed);
      IList<JObject> content;
      if (getMetadataEntryList.Count < 5010)
      {
        content = (IList<JObject>) new JObject[1]
        {
          await this.pageWithDetailsHandler.Handle(new PackageNameRequest<VssNuGetPackageName, List<INuGetMetadataEntry>>((IPackageNameRequest<VssNuGetPackageName>) request.PackageRequest, getMetadataEntryList))
        };
      }
      else
      {
        getMetadataEntryList.Reverse();
        content = (IList<JObject>) getMetadataEntryList.GetPages<INuGetMetadataEntry>(5010).Select<List<INuGetMetadataEntry>, JObject>((Func<List<INuGetMetadataEntry>, JObject>) (page => this.registrationsPackageFrameCreator.GetPackagePageFrame((IPackageNameRequest<VssNuGetPackageName>) request.PackageRequest, page.First<INuGetMetadataEntry>().PackageIdentity.Version, page.Last<INuGetMetadataEntry>().PackageIdentity.Version, registrationUri))).ToList<JObject>();
      }
      SecureJObject secureJobject = new JObject(new object[5]
      {
        (object) new JProperty("@id", (object) registrationUri.AbsoluteUri),
        (object) new JProperty("@type", (object) new JArray(new object[3]
        {
          (object) "catalog:CatalogRoot",
          (object) "PackageRegistration",
          (object) "catalog:Permalink"
        })),
        (object) new JProperty("count", (object) content.Count),
        (object) new JProperty("items", (object) content),
        (object) new JProperty("@context", (object) V3RegistrationsController.ContextObjects.PackageIndex)
      }).ToSecureJObject((IFeedRequest) request);
      return new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) new ObjectContent<SecureJObject>(secureJobject, (MediaTypeFormatter) new ServerVssJsonMediaTypeFormatter(true))
      };
    }
  }
}
