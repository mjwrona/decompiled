// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Secrets.AzureAadAppIdentityTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.Secrets
{
  public class AzureAadAppIdentityTokenProvider
  {
    private AuthenticationResult m_authenticationResult;
    private readonly string m_resource;
    private readonly string m_authority;

    public AzureAadAppIdentityTokenProvider(string resource, string authority)
    {
      this.m_resource = resource;
      this.m_authority = authority;
    }

    public Task<AuthenticationResult> GetAuthResultAsync(IVssRequestContext requestContext) => requestContext.GetService<IAadServiceAppIdentityService>().GetAuthResultAsync(this.m_resource, this.m_authority);

    public async Task<string> GetTokenAsync(IVssRequestContext requestContext)
    {
      if (this.m_authenticationResult == null || this.m_authenticationResult.ExpiresOn >= (DateTimeOffset) DateTime.UtcNow.AddMinutes(-5.0))
        this.m_authenticationResult = await this.GetAuthResultAsync(requestContext);
      return this.m_authenticationResult.AccessToken;
    }
  }
}
