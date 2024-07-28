// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthAccessTokenCredential
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public class VssOAuthAccessTokenCredential : FederatedCredential
  {
    public VssOAuthAccessTokenCredential(string accessToken)
      : this(new VssOAuthAccessToken(accessToken))
    {
    }

    public VssOAuthAccessTokenCredential(JsonWebToken accessToken)
      : this(new VssOAuthAccessToken(accessToken))
    {
    }

    public VssOAuthAccessTokenCredential(VssOAuthAccessToken accessToken)
      : base((IssuedToken) accessToken)
    {
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

    protected override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response)
    {
      return (IssuedTokenProvider) new VssOAuthAccessTokenCredential.VssOAuthAccessTokenProvider((IssuedTokenCredential) this, serverUrl, (Uri) null);
    }

    private class VssOAuthAccessTokenProvider : IssuedTokenProvider
    {
      public VssOAuthAccessTokenProvider(
        IssuedTokenCredential credential,
        Uri serverUrl,
        Uri signInUrl)
        : base(credential, serverUrl, signInUrl)
      {
      }

      public override bool GetTokenIsInteractive => false;
    }
  }
}
