// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.RegistrationPackageVersionHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class RegistrationPackageVersionHandler : 
    IAsyncHandler<
    #nullable disable
    PackageRequest<VssNuGetPackageIdentity>, HttpResponseMessage>,
    IHaveInputType<PackageRequest<VssNuGetPackageIdentity>>,
    IHaveOutputType<HttpResponseMessage>
  {
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;
    private readonly ILocationFacade locationFacade;
    private readonly IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>> sourceUriCalculator;

    public RegistrationPackageVersionHandler(
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      ILocationFacade locationFacade,
      IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>> sourceUriCalculator)
    {
      this.metadataService = metadataService;
      this.locationFacade = locationFacade;
      this.sourceUriCalculator = sourceUriCalculator;
    }

    public async Task<HttpResponseMessage> Handle(PackageRequest<VssNuGetPackageIdentity> request)
    {
      IDictionary<IPackageIdentity, Uri> idToUriMap = this.sourceUriCalculator.Handle(new PackageIdentityBatchRequest((IFeedRequest) request, (ICollection<IPackageIdentity>) new VssNuGetPackageIdentity[1]
      {
        request.PackageId
      }));
      INuGetMetadataEntry versionStateAsync = await this.metadataService.GetPackageVersionStateAsync((IPackageRequest<VssNuGetPackageIdentity>) request);
      SecureJObject secureJobject = new JObject(new object[8]
      {
        (object) new JProperty("@id", (object) this.GetPackageVersionNewRegistrationUri((IPackageRequest<VssNuGetPackageIdentity>) request).AbsoluteUri),
        (object) new JProperty("@type", (object) new JArray(new object[2]
        {
          (object) "Package",
          (object) "http://schema.nuget.org/catalog#Permalink"
        })),
        (object) new JProperty("catalogEntry", (object) this.GetCommitLogEntryUri((IFeedRequest) request, versionStateAsync.CommitId).AbsoluteUri),
        (object) new JProperty("listed", (object) versionStateAsync.Metadata.Listed),
        (object) new JProperty("packageContent", (object) idToUriMap[(IPackageIdentity) request.PackageId]),
        (object) new JProperty("published", (object) versionStateAsync.CreatedDate),
        (object) new JProperty("registration", (object) this.GetPackageNewRegistrationUri(((IPackageRequest<IPackageIdentity<VssNuGetPackageName, VssNuGetPackageVersion>>) request).ToPackageNameRequest<VssNuGetPackageName, VssNuGetPackageVersion>()).AbsoluteUri),
        (object) new JProperty("@context", (object) V3RegistrationsController.ContextObjects.PackageVersion)
      }).ToSecureJObject((IFeedRequest) request);
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) new ObjectContent<SecureJObject>(secureJobject, (MediaTypeFormatter) new ServerVssJsonMediaTypeFormatter(true))
      };
      idToUriMap = (IDictionary<IPackageIdentity, Uri>) null;
      return httpResponseMessage;
    }

    private Uri GetPackageNewRegistrationUri(
      IPackageNameRequest<VssNuGetPackageName> packageNameRequest)
    {
      return this.locationFacade.GetResourceUri("nuget", ResourceIds.Registrations2PackageResourceId, (IFeedRequest) packageNameRequest, PackagingUriNamePreference.PreferCanonicalId, (object) new
      {
        packageId = packageNameRequest.PackageName.NormalizedName
      });
    }

    private Uri GetCommitLogEntryUri(IFeedRequest feedRequest, PackagingCommitId commitId) => this.locationFacade.GetResourceUri("nuget", ResourceIds.CommitLogResourceId, feedRequest, PackagingUriNamePreference.PreferCanonicalId, (object) new
    {
      commitId = commitId
    });

    private Uri GetPackageVersionNewRegistrationUri(
      IPackageRequest<VssNuGetPackageIdentity> packageRequest)
    {
      return this.locationFacade.GetResourceUri("nuget", ResourceIds.Registrations2PackageVersionResourceId, (IFeedRequest) packageRequest, PackagingUriNamePreference.PreferCanonicalId, (object) new
      {
        packageId = packageRequest.PackageId.Name.NormalizedName,
        packageVersion = packageRequest.PackageId.Version.NormalizedVersion
      });
    }
  }
}
