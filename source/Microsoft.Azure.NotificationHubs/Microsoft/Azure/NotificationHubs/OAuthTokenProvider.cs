// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.OAuthTokenProvider
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Web;

namespace Microsoft.Azure.NotificationHubs
{
  public class OAuthTokenProvider : TokenProvider
  {
    private const int DefaultCacheSize = 100;
    private const string OAuthTokenServicePath = "$STS/OAuth/";
    private const string ClientPasswordFormat = "grant_type=authorization_code&client_id={0}&client_secret={1}&scope={2}";
    private readonly Func<Uri, Uri> onBuildUri = new Func<Uri, Uri>(OAuthTokenProvider.BuildStsUri);
    private readonly List<Uri> stsUris;
    private readonly NetworkCredential credential;

    internal OAuthTokenProvider(IEnumerable<Uri> stsUris, NetworkCredential credential)
      : base(true, true, 100, TokenScope.Namespace)
    {
      if (credential == null)
        throw FxTrace.Exception.ArgumentNull(nameof (credential));
      this.stsUris = stsUris != null ? stsUris.ToList<Uri>() : throw FxTrace.Exception.ArgumentNull(nameof (stsUris));
      if (this.stsUris.Count == 0)
        throw FxTrace.Exception.ArgumentNull(nameof (stsUris));
      this.credential = credential;
    }

    protected override IAsyncResult OnBeginGetToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      DateTime expiresIn;
      string audience;
      SimpleWebSecurityToken webSecurityToken = new SimpleWebSecurityToken(TokenProviderHelper.GetOAuthAccessTokenCore((IEnumerator<Uri>) this.stsUris.GetEnumerator(), this.onBuildUri, this.BuildRequestToken(appliesTo), timeout, out expiresIn, out audience), expiresIn, audience);
      return (IAsyncResult) new CompletedAsyncResult<TokenProviderHelper.TokenResult<SecurityToken>>(new TokenProviderHelper.TokenResult<SecurityToken>()
      {
        CacheUntil = expiresIn,
        Token = (SecurityToken) webSecurityToken
      }, callback, state);
    }

    protected override IAsyncResult OnBeginGetWebToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      DateTime expiresIn;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}=\"{2}\"", new object[3]
      {
        (object) "WRAP",
        (object) "access_token",
        (object) TokenProviderHelper.GetOAuthAccessTokenCore((IEnumerator<Uri>) this.stsUris.GetEnumerator(), this.onBuildUri, this.BuildRequestToken(appliesTo), timeout, out expiresIn, out string _)
      });
      return (IAsyncResult) new CompletedAsyncResult<TokenProviderHelper.TokenResult<string>>(new TokenProviderHelper.TokenResult<string>()
      {
        CacheUntil = expiresIn,
        Token = str
      }, callback, state);
    }

    protected override SecurityToken OnEndGetToken(IAsyncResult result, out DateTime cacheUntil)
    {
      TokenProviderHelper.TokenResult<SecurityToken> tokenResult = CompletedAsyncResult<TokenProviderHelper.TokenResult<SecurityToken>>.End(result);
      cacheUntil = tokenResult.CacheUntil;
      return tokenResult.Token;
    }

    protected override string OnEndGetWebToken(IAsyncResult result, out DateTime cacheUntil)
    {
      TokenProviderHelper.TokenResult<string> tokenResult = CompletedAsyncResult<TokenProviderHelper.TokenResult<string>>.End(result);
      cacheUntil = tokenResult.CacheUntil;
      return tokenResult.Token;
    }

    protected override TokenProvider.Key BuildKey(string appliesTo, string action) => new TokenProvider.Key(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", new object[2]
    {
      (object) this.credential.Domain,
      (object) this.credential.UserName
    }), string.Empty);

    protected override string NormalizeAppliesTo(string appliesTo) => ServiceBusUriHelper.NormalizeUri(appliesTo, "http", this.StripQueryParameters);

    private string BuildRequestToken(string scope)
    {
      string str;
      if (!string.IsNullOrWhiteSpace(this.credential.Domain))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}", new object[2]
        {
          (object) this.credential.UserName,
          (object) this.credential.Domain
        });
      else
        str = this.credential.UserName;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "grant_type=authorization_code&client_id={0}&client_secret={1}&scope={2}", new object[3]
      {
        (object) HttpUtility.UrlEncode(str),
        (object) HttpUtility.UrlEncode(this.credential.Password),
        (object) HttpUtility.UrlEncode(scope)
      });
    }

    private static Uri BuildStsUri(Uri baseAddress)
    {
      UriBuilder httpsSchemeAndPort = MessagingUtilities.CreateUriBuilderWithHttpsSchemeAndPort(baseAddress);
      httpsSchemeAndPort.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
      {
        (object) httpsSchemeAndPort.Path,
        (object) "$STS/OAuth/"
      });
      return httpsSchemeAndPort.Uri;
    }
  }
}
