// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Internal.NpmRegistryController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Npm.Server.Controllers;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Internal
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "registry", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  [ValidateModel]
  public class NpmRegistryController : NpmApiController
  {
    [HttpGet]
    [ClientIgnore]
    [PackagingPublicProjectRequestRestrictions]
    public HttpResponseMessage GetRegistry(string feedId)
    {
      feedId.ThrowIfNull<string>((Func<Exception>) (() => (Exception) new ArgumentFormatException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_ParameterCannotBeNull((object) nameof (feedId)))));
      this.GetFeedRequest(feedId);
      HttpResponseMessage response = new HttpResponseMessage(!IsRequestFromAuthLib() || !IsUnauthenticatedRequest() ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
      PublicAuthUtils.AddAuthHeadersToResponse(this.TfsRequestContext, response);
      return response;

      bool IsRequestFromAuthLib()
      {
        HttpHeaderValueCollection<ProductInfoHeaderValue> userAgent = this.Request.Headers.UserAgent;
        return userAgent != null && userAgent.Select<ProductInfoHeaderValue, string>((Func<ProductInfoHeaderValue, string>) (x => x.Product?.Name ?? string.Empty)).Any<string>((Func<string, bool>) (name => name.Equals("VssPkgsAuthLib", StringComparison.OrdinalIgnoreCase)));
      }

      bool IsUnauthenticatedRequest()
      {
        int num1 = this.TfsRequestContext.UserContext.Equals(UserWellKnownIdentityDescriptors.AnonymousPrincipal) ? 1 : 0;
        string authenticationMechanism = this.TfsRequestContext.GetAuthenticationMechanism();
        int num2 = string.IsNullOrWhiteSpace(authenticationMechanism) || authenticationMechanism.Equals("None", StringComparison.OrdinalIgnoreCase) ? (true ? 1 : 0) : (authenticationMechanism.StartsWith("None!!", StringComparison.OrdinalIgnoreCase) ? 1 : 0);
        return (num1 | num2) != 0;
      }
    }

    [HttpPut]
    [ControllerMethodTraceFilter(12000000)]
    [ClientIgnore]
    public Task<HttpResponseMessage> PutPackageAsync(
      string feedId,
      string packageName,
      [FromBody] PackageMetadata package)
    {
      throw new InvalidOperationException("This controller method was deprecated.  See NpmPublishHttpHandler.");
    }

    [HttpPut]
    [ControllerMethodTraceFilter(12000000)]
    [ClientIgnore]
    public Task<HttpResponseMessage> PutPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      [FromBody] PackageMetadata package)
    {
      throw new InvalidOperationException("This controller method was deprecated.  See NpmPublishHttpHandler.");
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12000010)]
    [ClientLocationId("D9B75B07-F1D9-4A67-AAA6-A4D9E66B3352")]
    [ClientResponseType(typeof (JObject), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> GetUnscopedPackageRegistrationAsync(
      string feedId,
      string packageName)
    {
      return await this.GetScopedPackageRegistrationAsync(feedId, (string) null, packageName);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12000010)]
    [ClientLocationId("CE899084-B217-4B5B-80CE-8CB8FE4DDDDE")]
    [ClientResponseType(typeof (JObject), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> GetScopedPackageRegistrationAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName)
    {
      NpmRegistryController registryController = this;
      IFeedRequest feed = registryController.GetFeedRequest(feedId);
      string content = await NpmAggregationResolver.Bootstrap(registryController.TfsRequestContext).HandlerFor<RawPackageNameRequest, string>((IRequireAggBootstrapper<IAsyncHandler<RawPackageNameRequest, string>>) new GetPackageRegistrationHandlerBootstrapper(registryController.TfsRequestContext)).TaskYieldOnException<RawPackageNameRequest, string>().Handle(new RawPackageNameRequest(feed, RawNpmPackageName.Create(packageScope, unscopedPackageName)));
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feed.Feed);
      Encoding utF8 = Encoding.UTF8;
      ISecuredObject securedObject = securedObjectReadOnly;
      VssServerStringContent serverStringContent = new VssServerStringContent(content, utF8, "application/json", (object) securedObject);
      registryController.TfsRequestContext.UpdateTimeToFirstPage();
      HttpResponseMessage registrationAsync = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) serverStringContent
      };
      feed = (IFeedRequest) null;
      return registrationAsync;
    }

    [HttpPost]
    [ControllerMethodTraceFilter(12000040)]
    [ClientLocationId("8DC6EF3A-8A03-4D76-A505-387C432A7625")]
    [ClientResponseType(typeof (JObject), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    [DecodeGzippedRequestBodies]
    public async Task<HttpResponseMessage> GetAuditBulk(
      string feedId,
      bool isBulkApi,
      [FromBody] JObject auditJsonPayload)
    {
      return await this.GetAuditHelper(feedId, auditJsonPayload, new Uri(WellKnownSources.Npmjs.Location, "-/npm/v1/security/advisories/bulk"));
    }

    [HttpPost]
    [ControllerMethodTraceFilter(12000050)]
    [ClientLocationId("B7A9B30B-3535-4B2C-B51A-4CD156A31F2E")]
    [ClientResponseType(typeof (JObject), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    [DecodeGzippedRequestBodies]
    public async Task<HttpResponseMessage> GetAuditQuick(
      string feedId,
      bool isQuickApi,
      [FromBody] JObject auditJsonPayload)
    {
      return await this.GetAuditHelper(feedId, auditJsonPayload, new Uri(WellKnownSources.Npmjs.Location, "-/npm/v1/security/audits/quick"));
    }

    [HttpPost]
    [ControllerMethodTraceFilter(12000060)]
    [ClientLocationId("E9B047A3-74C9-4D2F-895A-62C8145795D6")]
    [ClientResponseType(typeof (JObject), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    [DecodeGzippedRequestBodies]
    public async Task<HttpResponseMessage> GetAuditFull(
      string feedId,
      bool isFullApi,
      [FromBody] JObject auditJsonPayload)
    {
      return await this.GetAuditHelper(feedId, auditJsonPayload, new Uri(WellKnownSources.Npmjs.Location, "-/npm/v1/security/audits"));
    }

    private async Task<HttpResponseMessage> GetAuditHelper(
      string feedId,
      JObject auditJsonPayload,
      Uri requestUri)
    {
      NpmRegistryController registryController = this;
      if (!FeatureFlagConstants.NpmEnableAudit.Bootstrap(registryController.TfsRequestContext).Get())
        throw new FeatureDisabledException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_AuditNotEnabled());
      ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(registryController.GetFeedRequest(feedId).Feed);
      HttpClientFacade httpClientFacade = new HttpClientFacade(registryController.TfsRequestContext, UpstreamHttpClient.ForProtocol((IProtocol) Protocol.npm));
      byte[] bytes = Encoding.UTF8.GetBytes(auditJsonPayload.Serialize<JObject>());
      MemoryStream outputStream = new MemoryStream();
      GZipStream gzipStream;
      HttpResponseMessage auditHelper;
      try
      {
        gzipStream = new GZipStream((Stream) outputStream, CompressionMode.Compress);
        try
        {
          gzipStream.Write(bytes, 0, bytes.Length);
          gzipStream.Close();
          HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
          {
            Content = (HttpContent) new ByteArrayContent(outputStream.ToArray())
          };
          request.Content.Headers.ContentEncoding.Add("gzip");
          request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
          request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
          HttpResponseMessage httpResponseMessage = await httpClientFacade.SendAsync(request, HttpCompletionOption.ResponseContentRead);
          HttpResponseMessage httpResponseMessage1 = new HttpResponseMessage(httpResponseMessage.StatusCode);
          HttpResponseMessage httpResponseMessage2 = httpResponseMessage1;
          httpResponseMessage2.Content = (HttpContent) new VssServerStreamContent(await httpResponseMessage.Content.ReadAsStreamAsync(), (object) securedObject);
          auditHelper = httpResponseMessage1;
          httpResponseMessage2 = (HttpResponseMessage) null;
          httpResponseMessage1 = (HttpResponseMessage) null;
        }
        finally
        {
          gzipStream?.Dispose();
        }
      }
      finally
      {
        outputStream?.Dispose();
      }
      securedObject = (ISecuredObject) null;
      outputStream = (MemoryStream) null;
      gzipStream = (GZipStream) null;
      return auditHelper;
    }
  }
}
