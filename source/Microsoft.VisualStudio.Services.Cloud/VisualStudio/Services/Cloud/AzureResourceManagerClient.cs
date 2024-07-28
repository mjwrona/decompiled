// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureResourceManagerClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AzureResourceManagerClient : IAzureResourceManagerClient, IDisposable
  {
    private Uri _baseUri;
    private HttpClient _client;
    private const string CloudServiceManagementLocationRegistryPath = "/Configuration/Azure/CloudServiceModel/ResourceManagementUrl";
    private static readonly int TracePoint = 10011300;
    private static readonly string s_Area = nameof (AzureResourceManagerClient);
    private static readonly string s_Layer = nameof (AzureResourceManagerClient);

    public AzureResourceManagerClient(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      string uriString = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Azure/CloudServiceModel/ResourceManagementUrl", string.Empty);
      if (string.IsNullOrEmpty(uriString))
      {
        requestContext.Trace(AzureResourceManagerClient.TracePoint + 1, TraceLevel.Error, AzureResourceManagerClient.s_Area, AzureResourceManagerClient.s_Layer, "ARM location not present.");
        throw new InvalidPathException("Azure resource location not found");
      }
      this._baseUri = new Uri(uriString);
      this.PrepareClient(requestContext, token);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => this._client.SendAsync(request);

    public Uri BaseAddress => this._baseUri;

    public void Dispose()
    {
      if (this._client == null)
        return;
      this._client.Dispose();
    }

    private void PrepareClient(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      this._client = new HttpClient();
      this._client.BaseAddress = this._baseUri;
      this._client.DefaultRequestHeaders.Accept.Clear();
      this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      this._client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", (object) token.RawData));
    }
  }
}
