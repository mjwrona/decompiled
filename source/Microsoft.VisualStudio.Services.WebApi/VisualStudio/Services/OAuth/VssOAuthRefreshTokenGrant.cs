// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthRefreshTokenGrant
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public sealed class VssOAuthRefreshTokenGrant : VssOAuthGrant
  {
    private readonly string m_refreshToken;

    public VssOAuthRefreshTokenGrant(string refreshToken)
      : base(VssOAuthGrantType.RefreshToken)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(refreshToken, nameof (refreshToken));
      this.m_refreshToken = refreshToken;
    }

    public string RefreshToken => this.m_refreshToken;

    protected override void SetParameters(IDictionary<string, string> parameters)
    {
      parameters["grant_type"] = "refresh_token";
      parameters["refresh_token"] = this.m_refreshToken;
      parameters["assertion"] = this.m_refreshToken;
    }
  }
}
