// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RuntimeServicePrincipalTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Rest;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class RuntimeServicePrincipalTokenProvider : ITokenProvider
  {
    private readonly string m_resource;
    private readonly string m_authority;
    private readonly AzureServicePrincipalProvider m_tokenProvider;

    internal RuntimeServicePrincipalTokenProvider(
      string resource,
      string authority = null,
      ITFLogger logger = null)
    {
      this.m_tokenProvider = new AzureServicePrincipalProvider(logger);
      this.m_authority = authority ?? "https://login.microsoftonline.com/" + this.m_tokenProvider.RuntimeServicePrincipalApplicationTenantId;
      this.m_resource = resource;
    }

    public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(
      CancellationToken cancellationToken)
    {
      return AuthenticationHeaderValueExtensions.ToBearerToken(await this.GetTokenAsync());
    }

    public Task<string> GetTokenAsync() => Task.FromResult<string>(this.m_tokenProvider.GetAuthResult(this.m_authority, this.m_resource).AccessToken);
  }
}
