// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssOAuthToken
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Client
{
  public sealed class VssOAuthToken : IssuedToken
  {
    private string m_token;
    private DateTime m_expiration;
    private VssOAuthTokenType m_tokenType;

    public VssOAuthToken(string token)
      : this(token, VssOAuthTokenType.AccessToken, DateTime.MaxValue)
    {
    }

    internal VssOAuthToken(string token, VssOAuthTokenType tokenType)
      : this(token, tokenType, DateTime.MaxValue)
    {
    }

    internal VssOAuthToken(string token, VssOAuthTokenType tokenType, DateTime expiration)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
      this.m_token = token;
      this.m_tokenType = tokenType;
      this.m_expiration = expiration;
    }

    public string Token => this.m_token;

    public VssOAuthTokenType TokenType => this.m_tokenType;

    public DateTime Expiration => this.m_expiration;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

    internal override void ApplyTo(IHttpRequest request)
    {
      if (this.m_tokenType != VssOAuthTokenType.AccessToken)
        return;
      request.Headers.SetValue("Authorization", "Bearer " + this.m_token);
    }
  }
}
