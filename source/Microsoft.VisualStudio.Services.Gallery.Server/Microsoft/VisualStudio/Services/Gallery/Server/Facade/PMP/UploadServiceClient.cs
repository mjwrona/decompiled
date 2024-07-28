// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.UploadServiceClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP
{
  public class UploadServiceClient : IUploadServiceClient
  {
    private readonly HttpClient httpClient;
    private IMsiAccessTokenProvider _managedIdentityAccessTokenProvider;

    public UploadServiceClient(HttpClient httpClient) => this.httpClient = httpClient ?? throw new ArgumentNullException(nameof (httpClient));

    internal UploadServiceClient(
      HttpClient httpClient,
      IMsiAccessTokenProvider msiAccessTokenProvider)
    {
      this.httpClient = httpClient;
      this._managedIdentityAccessTokenProvider = msiAccessTokenProvider;
    }

    public async Task<HttpResponseMessage> UploadArtifactFileToPMPAsync(
      IVssRequestContext requestContext,
      string requestUri,
      MultipartFormDataContent content)
    {
      UploadServiceClient var = this;
      requestContext.TraceEnter(12062087, "gallery", nameof (UploadArtifactFileToPMPAsync), nameof (UploadArtifactFileToPMPAsync));
      ArgumentUtility.CheckForNull<UploadServiceClient>(var, nameof (requestUri));
      ArgumentUtility.CheckForNull<UploadServiceClient>(var, nameof (content));
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
      {
        Method = HttpMethod.Post,
        RequestUri = new Uri(requestUri),
        Content = (HttpContent) content
      };
      content.Headers.Add("X-PackageManagement-CorrelationId", string.Format("vscode{0}", (object) requestContext.ActivityId));
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableAuthTokenFetchForPMPWebApi"))
        var.AddManagedIdentityAccessTokenToHttpRequest(requestContext, httpRequestMessage);
      HttpResponseMessage pmpAsync = await var.httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
      requestContext.Trace(12062087, TraceLevel.Info, "gallery", nameof (UploadArtifactFileToPMPAsync), "Request sent to Upload service with response code {0}", (object) pmpAsync.StatusCode.ToString());
      return pmpAsync;
    }

    private void AddManagedIdentityAccessTokenToHttpRequest(
      IVssRequestContext requestContext,
      HttpRequestMessage httpRequestMessage)
    {
      if (this._managedIdentityAccessTokenProvider == null)
        this._managedIdentityAccessTokenProvider = (IMsiAccessTokenProvider) new MsiAccessTokenProvider(MsiTokenCache.SharedCache, (IAzureInstanceMetadataProvider) new AzureInstanceMetadataProvider(this.httpClient));
      string accessToken = this._managedIdentityAccessTokenProvider.GetAccessToken(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PMP/WebApiResourceUri", false, (string) null));
      httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
  }
}
