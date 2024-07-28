// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.OAuthToken
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.WebApi.VssOAuthAccessToken instead.", false)]
  [Serializable]
  public sealed class OAuthToken : IssuedToken
  {
    private string m_token;
    private OAuthTokenType m_tokenType;
    private DateTime m_expiration;

    internal OAuthToken(string token, OAuthTokenType tokenType)
      : this(token, tokenType, DateTime.MaxValue)
    {
    }

    internal OAuthToken(string token, OAuthTokenType tokenType, DateTime expiration)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
      this.m_token = token;
      this.m_tokenType = tokenType;
      this.m_expiration = expiration;
    }

    public string Token => this.m_token;

    public OAuthTokenType TokenType => this.m_tokenType;

    public DateTime Expiration => this.m_expiration;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

    internal override void ApplyTo(HttpWebRequest webRequest)
    {
      if (this.m_tokenType != OAuthTokenType.AccessToken)
        return;
      webRequest.Headers.Remove(HttpRequestHeader.Authorization);
      webRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + this.m_token);
      webRequest.ServicePoint.Expect100Continue = false;
    }
  }
}
