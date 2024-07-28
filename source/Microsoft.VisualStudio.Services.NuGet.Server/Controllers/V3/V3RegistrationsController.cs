// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3.V3RegistrationsController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "registrations2")]
  public class V3RegistrationsController : NuGetApiController
  {
    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722000)]
    public async 
    #nullable disable
    Task<HttpResponseMessage> GetPackageIndexAsync(
      string feedId,
      string packageId,
      int? pageSize = null,
      int? spillToPagesThreshold = null)
    {
      V3RegistrationsController registrationsController = this;
      IFeedRequest feedRequest = registrationsController.GetFeedRequest(feedId);
      bool includeSemVer2Versions = (bool) registrationsController.Request.GetRouteData().Route.Defaults["/withSemVer2Support"];
      IFactory<IFeedRequest, Task<IAsyncHandler<NuGetGetPackageIndexRequest<RawPackageNameRequest>, HttpResponseMessage>>> factory = NuGetAggregationResolver.Bootstrap(registrationsController.TfsRequestContext).FactoryFor<IAsyncHandler<NuGetGetPackageIndexRequest<RawPackageNameRequest>, HttpResponseMessage>>((IRequireAggBootstrapper<IAsyncHandler<NuGetGetPackageIndexRequest<RawPackageNameRequest>, HttpResponseMessage>>) new RegistrationPackageIndexBlobHandlerBootstrapper(registrationsController.TfsRequestContext));
      NuGetGetPackageIndexRequest<RawPackageNameRequest> request = new NuGetGetPackageIndexRequest<RawPackageNameRequest>(new RawPackageNameRequest(feedRequest, packageId), pageSize, spillToPagesThreshold, includeSemVer2Versions);
      NuGetGetPackageIndexRequest<RawPackageNameRequest> input = request;
      HttpResponseMessage httpResponseMessage = await (await factory.Get((IFeedRequest) input)).Handle(request);
      registrationsController.TfsRequestContext.UpdateTimeToFirstPage();
      HttpResponseMessage packageIndexAsync = httpResponseMessage;
      request = (NuGetGetPackageIndexRequest<RawPackageNameRequest>) null;
      return packageIndexAsync;
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722010)]
    public async Task<HttpResponseMessage> GetPackagePageAsync(
      string feedId,
      string packageId,
      string lower,
      string upper)
    {
      V3RegistrationsController registrationsController = this;
      IFeedRequest feedRequest = registrationsController.GetFeedRequest(feedId);
      bool data = (bool) registrationsController.Request.GetRouteData().Route.Defaults["/withSemVer2Support"];
      IFactory<IFeedRequest, Task<IAsyncHandler<RawPackageRangeRequest<IncludeSemVer2VersionsFlag>, HttpResponseMessage>>> factory = NuGetAggregationResolver.Bootstrap(registrationsController.TfsRequestContext).FactoryFor<IAsyncHandler<RawPackageRangeRequest<IncludeSemVer2VersionsFlag>, HttpResponseMessage>>((IRequireAggBootstrapper<IAsyncHandler<RawPackageRangeRequest<IncludeSemVer2VersionsFlag>, HttpResponseMessage>>) new RegistrationPackagePageBlobHandlerBootstrapper(registrationsController.TfsRequestContext));
      RawPackageRangeRequest<IncludeSemVer2VersionsFlag> request = new RawPackageRangeRequest<IncludeSemVer2VersionsFlag>(feedRequest, packageId, lower, upper, registrationsController.Request, (IncludeSemVer2VersionsFlag) data);
      RawPackageRangeRequest<IncludeSemVer2VersionsFlag> input = request;
      HttpResponseMessage packagePageAsync = await (await factory.Get((IFeedRequest) input)).Handle(request);
      request = (RawPackageRangeRequest<IncludeSemVer2VersionsFlag>) null;
      return packagePageAsync;
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722020)]
    public async Task<HttpResponseMessage> GetPackageVersionAsync(
      string feedId,
      string packageId,
      string packageVersion)
    {
      V3RegistrationsController registrationsController = this;
      IFeedRequest feedRequest = registrationsController.GetFeedRequest(feedId);
      IFactory<IFeedRequest, Task<IAsyncHandler<RawPackageRequest, HttpResponseMessage>>> factory = NuGetAggregationResolver.Bootstrap(registrationsController.TfsRequestContext).FactoryFor<IAsyncHandler<RawPackageRequest, HttpResponseMessage>>((IRequireAggBootstrapper<IAsyncHandler<RawPackageRequest, HttpResponseMessage>>) new RegistrationPackageVersionBlobHandlerBootstrapper(registrationsController.TfsRequestContext));
      RawPackageRequest request = new RawPackageRequest(feedRequest, packageId, packageVersion);
      RawPackageRequest input = request;
      HttpResponseMessage packageVersionAsync = await (await factory.Get((IFeedRequest) input)).Handle(request);
      request = (RawPackageRequest) null;
      return packageVersionAsync;
    }

    public static class ContextObjects
    {
      public static readonly JObject PackageVersion = JObject.Parse("\r\n            {\r\n                \"@vocab\": \"http://schema.nuget.org/schema#\",\r\n                \"xsd\": \"http://www.w3.org/2001/XMLSchema#\",\r\n                \"catalogEntry\": {\r\n                    \"@type\": \"@id\"\r\n                },\r\n                \"registration\": {\r\n                    \"@type\": \"@id\"\r\n                },\r\n                \"packageContent\": {\r\n                    type: \"@id\"\r\n                },\r\n                \"published\": {\r\n                    \"@type\": \"xsd:dateTime\"\r\n                }\r\n            }");
      public static readonly JObject PackageIndex = JObject.Parse("\r\n            {\r\n                \"@vocab\": \"http://schema.nuget.org/schema#\",\r\n                \"catalog\": \"http://schema.nuget.org/catalog#\",\r\n                \"xsd\": \"http://www.w3.org/2001/XMLSchema#\",\r\n                \"items\": {\r\n                    \"@id\": \"catalog:item\",\r\n                    \"@container\": \"@set\"\r\n                },\r\n                \"commitTimeStamp\": {\r\n                    \"@id\": \"catalog:commitTimeStamp\",\r\n                    \"@type\": \"xsd:dateTime\"\r\n                },\r\n                \"commitId\": {\r\n                    \"@id\": \"catalog:commitId\"\r\n                },\r\n                \"count\": {\r\n                    \"@id\": \"catalog:count\"\r\n                },\r\n                \"parent\": {\r\n                    \"@id\": \"catalog:parent\",\r\n                    \"@type\": \"@id\"\r\n                },\r\n                \"tags\": {\r\n                    \"@container\": \"@set\",\r\n                    \"@id\": \"tag\"\r\n                },\r\n                \"packageTargetFrameworks\": {\r\n                    \"@container\": \"@set\",\r\n                    \"@id\": \"packageTargetFramework\"\r\n                },\r\n                \"dependencyGroups\": {\r\n                    \"@container\": \"@set\",\r\n                    \"@id\": \"dependencyGroup\"\r\n                },\r\n                \"dependencies\": {\r\n                    \"@container\": \"@set\",\r\n                    \"@id\": \"dependency\"\r\n                },\r\n                \"packageContent\": {\r\n                    \"@type\": \"@id\"\r\n                },\r\n                \"published\": {\r\n                    \"@type\": \"xsd:dateTime\"\r\n                },\r\n                \"registration\": {\r\n                    \"@type\": \"@id\"\r\n                }\r\n            }");
    }
  }
}
