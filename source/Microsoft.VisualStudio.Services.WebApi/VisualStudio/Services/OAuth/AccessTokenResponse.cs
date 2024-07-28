// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.AccessTokenResponse
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.OAuth
{
  [DataContract]
  public sealed class AccessTokenResponse
  {
    public AccessTokenResponse()
    {
    }

    public AccessTokenResponse(string accessToken, string expiresIn, string refreshToken)
    {
      this.AccessToken = accessToken;
      this.TokenType = "bearer";
      this.ExpiresIn = expiresIn;
      this.RefreshToken = refreshToken;
    }

    [DataMember(Name = "access_token")]
    public string AccessToken { get; set; }

    [DataMember(Name = "token_type")]
    public string TokenType { get; set; }

    [DataMember(Name = "expires_in")]
    public string ExpiresIn { get; set; }

    [DataMember(Name = "refresh_token")]
    public string RefreshToken { get; set; }
  }
}
