// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.OAuthTokenContainer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.WebApi.VssOAuthAccessToken instead.", false)]
  public sealed class OAuthTokenContainer : IssuedToken
  {
    private OAuthToken m_accessToken;
    private OAuthToken m_refreshToken;

    internal OAuthTokenContainer(OAuthToken accessToken, OAuthToken refreshToken)
    {
      this.m_accessToken = accessToken;
      this.m_refreshToken = refreshToken;
    }

    public OAuthToken AccessToken => this.m_accessToken;

    public OAuthToken RefreshToken => this.m_refreshToken;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

    internal override void ApplyTo(HttpWebRequest webRequest)
    {
      if (this.m_accessToken == null)
        return;
      this.m_accessToken.ApplyTo(webRequest);
    }

    internal static OAuthTokenContainer ExtractTokens(byte[] responseData, Encoding encoding)
    {
      OAuthTokenContainer.OAuthTokenData tokenData = OAuthTokenContainer.GetTokenData(responseData, encoding);
      return new OAuthTokenContainer(new OAuthToken(tokenData.access_token, OAuthTokenType.AccessToken, DateTime.UtcNow.AddSeconds(double.Parse(tokenData.expires_in))), new OAuthToken(tokenData.refresh_token, OAuthTokenType.RefreshToken));
    }

    internal static OAuthTokenContainer FromAccessToken(string accessToken) => new OAuthTokenContainer(new OAuthToken(accessToken, OAuthTokenType.AccessToken), (OAuthToken) null);

    internal static OAuthTokenContainer FromAuthCodeOrRefreshToken(string authCodeOrRefreshToken)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(authCodeOrRefreshToken, nameof (authCodeOrRefreshToken));
      if (authCodeOrRefreshToken[0] == 'Z')
        return new OAuthTokenContainer((OAuthToken) null, new OAuthToken(authCodeOrRefreshToken, OAuthTokenType.AuthenticationCode));
      return authCodeOrRefreshToken[0] == 'R' ? new OAuthTokenContainer((OAuthToken) null, new OAuthToken(authCodeOrRefreshToken, OAuthTokenType.RefreshToken)) : throw new ArgumentException(nameof (authCodeOrRefreshToken));
    }

    private static OAuthTokenContainer.OAuthTokenData GetTokenData(
      byte[] responseData,
      Encoding encoding)
    {
      IDictionary<string, object> dictionary = (IDictionary<string, object>) JavaScriptObjectDeserializer.BasicDeserialize(encoding.GetString(responseData));
      object obj = (object) null;
      OAuthTokenContainer.OAuthTokenData tokenData = new OAuthTokenContainer.OAuthTokenData();
      if (dictionary.TryGetValue("access_token", out obj))
        tokenData.access_token = obj.ToString();
      if (dictionary.TryGetValue("token_type", out obj))
        tokenData.token_type = obj.ToString();
      if (dictionary.TryGetValue("expires_in", out obj))
        tokenData.expires_in = obj.ToString();
      if (dictionary.TryGetValue("refresh_token", out obj))
        tokenData.refresh_token = obj.ToString();
      return tokenData;
    }

    private class OAuthTokenData
    {
      public string access_token { get; set; }

      public string token_type { get; set; }

      public string expires_in { get; set; }

      public string refresh_token { get; set; }
    }
  }
}
