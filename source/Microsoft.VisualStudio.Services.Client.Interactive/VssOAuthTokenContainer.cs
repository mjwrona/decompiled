// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssOAuthTokenContainer
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Microsoft.VisualStudio.Services.Client
{
  public sealed class VssOAuthTokenContainer : IssuedToken
  {
    private VssOAuthToken m_accessToken;
    private VssOAuthToken m_refreshToken;
    private static Lazy<DataContractJsonSerializer> s_serializer = new Lazy<DataContractJsonSerializer>((Func<DataContractJsonSerializer>) (() => new DataContractJsonSerializer(typeof (VssOAuthTokenContainer.OAuthTokenData))));

    internal VssOAuthTokenContainer(VssOAuthToken accessToken, VssOAuthToken refreshToken)
    {
      this.m_accessToken = accessToken;
      this.m_refreshToken = refreshToken;
    }

    public VssOAuthToken AccessToken => this.m_accessToken;

    public VssOAuthToken RefreshToken => this.m_refreshToken;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

    internal override void ApplyTo(IHttpRequest message)
    {
      if (this.m_accessToken == null)
        return;
      this.m_accessToken.ApplyTo(message);
    }

    internal static VssOAuthTokenContainer ExtractTokens(Stream stream)
    {
      VssOAuthTokenContainer.OAuthTokenData oauthTokenData = (VssOAuthTokenContainer.OAuthTokenData) VssOAuthTokenContainer.s_serializer.Value.ReadObject(stream);
      return new VssOAuthTokenContainer(new VssOAuthToken(oauthTokenData.access_token, VssOAuthTokenType.AccessToken, DateTime.UtcNow.AddSeconds(double.Parse(oauthTokenData.expires_in))), new VssOAuthToken(oauthTokenData.refresh_token, VssOAuthTokenType.RefreshToken));
    }

    internal static VssOAuthTokenContainer FromAccessToken(string accessToken) => new VssOAuthTokenContainer(new VssOAuthToken(accessToken, VssOAuthTokenType.AccessToken), (VssOAuthToken) null);

    internal static VssOAuthTokenContainer FromAuthCodeOrRefreshToken(string authCodeOrRefreshToken)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(authCodeOrRefreshToken, nameof (authCodeOrRefreshToken));
      if (authCodeOrRefreshToken[0] == 'Z')
        return new VssOAuthTokenContainer((VssOAuthToken) null, new VssOAuthToken(authCodeOrRefreshToken, VssOAuthTokenType.AuthenticationCode));
      return authCodeOrRefreshToken[0] == 'R' ? new VssOAuthTokenContainer((VssOAuthToken) null, new VssOAuthToken(authCodeOrRefreshToken, VssOAuthTokenType.RefreshToken)) : throw new ArgumentException(nameof (authCodeOrRefreshToken));
    }

    [DataContract]
    private class OAuthTokenData
    {
      [DataMember]
      public string access_token { get; set; }

      [DataMember]
      public string token_type { get; set; }

      [DataMember]
      public string expires_in { get; set; }

      [DataMember]
      public string refresh_token { get; set; }
    }
  }
}
