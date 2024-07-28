// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher.ContentStitcherService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9521FAE3-5DB1-49D0-98DB-6A544E3AB730
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher
{
  public class ContentStitcherService : IContentStitcherService, IVssFrameworkService
  {
    private HttpClient _contentStitcherHttpClient;
    private string _contentStitcherHostName;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this._contentStitcherHostName = this.GetTargetContentStitcherHostName(requestContext);
      this._contentStitcherHttpClient = ArtifactHttpRetryMessageHandler.CreateHttpClientWithRetryHandler((IAppTraceSource) new CallbackAppTraceSource((Action<string>) (message => requestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", "BlobStore", message)), SourceLevels.All));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public async Task<HttpResponseMessage> GetStitchedFileAsync(
      IDomainId domainId,
      DedupIdentifier nodeId,
      IVssRequestContext requestContext,
      string fileName = null,
      bool useGzipCompression = false)
    {
      HttpResponseMessage signedUriAsync = await this.GetSignedUriAsync(domainId, nodeId, requestContext, fileName);
      if (signedUriAsync == null || !signedUriAsync.IsSuccessStatusCode || signedUriAsync.Content == null)
        return signedUriAsync;
      requestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", nameof (GetStitchedFileAsync), "Response from ContentStitcher was successful. Request will be redirected.");
      Uri result;
      if (Uri.TryCreate((await signedUriAsync.Content.ReadAsStringAsync()).Replace("\"", ""), UriKind.Absolute, out result))
        return await this.CallContentStitcherAsync(HttpMethod.Get, result, (HttpContent) null, requestContext, useGzipCompression);
      requestContext.TraceAlways(5708101, TraceLevel.Error, "ContentStitcher", nameof (GetStitchedFileAsync), "Response from ContentStitcher was an invalid Uri.");
      return new HttpResponseMessage(HttpStatusCode.InternalServerError);
    }

    public async Task<HttpResponseMessage> GetSignedUriAsync(
      IDomainId domainId,
      DedupIdentifier nodeId,
      IVssRequestContext requestContext,
      string fileName = null)
    {
      return await this.CallContentStitcherAsync(HttpMethod.Get, this.GetSigningUri(domainId, nodeId, requestContext, fileName), (HttpContent) null, requestContext);
    }

    private Uri GetSigningUri(
      IDomainId domainId,
      DedupIdentifier dedupId,
      IVssRequestContext requestContext,
      string fileName = null)
    {
      UriBuilder uriBuilder = new UriBuilder(Uri.UriSchemeHttps, this._contentStitcherHostName, -1, UrlHelper.GetContentStitcherSigningPath(domainId, requestContext.ServiceHost.InstanceId.ToString(), dedupId)).AppendQuery("e2eId", requestContext.E2EId.ToString()).AppendQuery("sessionId", requestContext.UniqueIdentifier.ToString());
      if (fileName != null)
        uriBuilder = uriBuilder.AppendQuery(nameof (fileName), fileName);
      requestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", nameof (GetSigningUri), string.Format("The target uri was resolved to: {0} for {1}.", (object) uriBuilder.Uri, (object) this._contentStitcherHostName));
      return uriBuilder.Uri;
    }

    public async Task<HttpResponseMessage> GetZipAsync(
      IDomainId domainId,
      ZippedContentRequest zippedContentRequest,
      IVssRequestContext requestContext,
      string fileName)
    {
      return await this.CallContentStitcherAsync(HttpMethod.Post, this.GetZippedContentUri(domainId, requestContext, fileName), JsonSerializer.SerializeToContent<ZippedContentRequest>(zippedContentRequest), requestContext);
    }

    private Uri GetZippedContentUri(
      IDomainId domainId,
      IVssRequestContext requestContext,
      string fileName)
    {
      UriBuilder uriBuilder = new UriBuilder(Uri.UriSchemeHttps, this._contentStitcherHostName, -1, UrlHelper.GetContentStitcherZippingPath(domainId, requestContext.ServiceHost.InstanceId.ToString())).AppendQuery("e2eId", requestContext.E2EId.ToString()).AppendQuery("sessionId", requestContext.UniqueIdentifier.ToString()).AppendQuery(nameof (fileName), fileName ?? "StitchedContent.zip");
      requestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", "GetSigningUri", string.Format("The target uri was resolved to: {0} for {1}.", (object) uriBuilder.Uri, (object) this._contentStitcherHostName));
      return uriBuilder.Uri;
    }

    private async Task<HttpResponseMessage> CallContentStitcherAsync(
      HttpMethod httpMethod,
      Uri requestUri,
      HttpContent content,
      IVssRequestContext requestContext,
      bool acceptGzip = false)
    {
      try
      {
        AuthenticationResult accessTokenAsync = await this.GetAccessTokenAsync(requestContext);
        if (accessTokenAsync == null)
          return (HttpResponseMessage) null;
        HttpRequestMessage request = new HttpRequestMessage(httpMethod, requestUri)
        {
          Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenAsync.AccessToken);
        if (acceptGzip)
          request.Headers.AcceptEncoding.ParseAdd("gzip");
        return await this._contentStitcherHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, requestContext.CancellationToken).EnforceCancellation<HttpResponseMessage>(requestContext.CancellationToken, (Func<string>) (() => "Timed out waiting for response from ContentStitcher."), "D:\\a\\_work\\1\\s\\BlobStore\\ServiceShared\\DedupStorage\\ContentStitcher\\ContentStitcherService.cs", nameof (CallContentStitcherAsync), 216).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5708101, TraceLevel.Error, "ContentStitcher", "BlobStore", ex);
        bool flag;
        switch (ex)
        {
          case TimeoutException _:
          case RequestCanceledException _:
          case TaskCanceledException _:
            flag = true;
            break;
          default:
            flag = false;
            break;
        }
        return new HttpResponseMessage(flag ? HttpStatusCode.RequestTimeout : HttpStatusCode.InternalServerError);
      }
    }

    private Task<AuthenticationResult> GetAccessTokenAsync(IVssRequestContext requestContext) => requestContext.ToDeploymentHostContext().GetService<IContentStitcherTokenService>().GetAccessTokenAsync(requestContext);

    private string GetTargetContentStitcherHostName(IVssRequestContext requestContext)
    {
      requestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", nameof (GetTargetContentStitcherHostName), string.Format("Execution environment flag: {0} ", (object) requestContext.ExecutionEnvironment.Flags) + string.Format("IsHostedDeployment:{0} ", (object) requestContext.ExecutionEnvironment.IsHostedDeployment) + string.Format("IsCloudDeployment: {0} ", (object) requestContext.ExecutionEnvironment.IsCloudDeployment) + string.Format("IsProduction: {0}.", (object) requestContext.ServiceHost.IsProduction) + string.Format("IsDevFabric: {0}", (object) requestContext.ExecutionEnvironment.IsDevFabricDeployment));
      TeamFoundationExecutionEnvironment executionEnvironment;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        executionEnvironment = requestContext.ExecutionEnvironment;
        if (!executionEnvironment.IsCloudDeployment)
          throw new InvalidOperationException(string.Format("Unexpected execution environment detected. Environment flag: {0}.", (object) requestContext.ExecutionEnvironment.Flags));
      }
      executionEnvironment = requestContext.ExecutionEnvironment;
      int num;
      if (executionEnvironment.IsCloudDeployment)
      {
        executionEnvironment = requestContext.ExecutionEnvironment;
        num = !executionEnvironment.IsDevFabricDeployment ? 1 : 0;
      }
      else
        num = 0;
      bool isProduction = num != 0;
      string hostedServiceName = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Settings/HostedServiceName", string.Empty);
      string tenantName;
      if (!string.IsNullOrWhiteSpace(hostedServiceName) && hostedServiceName.StartsWith("vsblob", StringComparison.OrdinalIgnoreCase))
      {
        tenantName = hostedServiceName.Remove(hostedServiceName.IndexOf("vsblob"), "vsblob".Length);
      }
      else
      {
        string blobStoreTenantName = ((IEnumerable<string>) requestContext.GetClient<ContentStitcherBootstrapHttpClient>(ServiceInstanceTypes.BlobStore).BaseAddress.Host.Split('.')).FirstOrDefault<string>();
        tenantName = blobStoreTenantName != null && blobStoreTenantName.StartsWith("vsblob", StringComparison.OrdinalIgnoreCase) ? blobStoreTenantName.Remove(blobStoreTenantName.IndexOf("vsblob"), "vsblob".Length) : throw new ContentStitcherHostnameNotFoundException(hostedServiceName, requestContext.ServiceName, blobStoreTenantName);
      }
      return ContentStitcherServiceConstants.GetHostNameByTenantName(isProduction, tenantName);
    }
  }
}
