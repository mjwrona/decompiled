// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SharedAccessSignatureTokenProvider
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Microsoft.Azure.NotificationHubs
{
  public class SharedAccessSignatureTokenProvider : TokenProvider
  {
    public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    internal readonly byte[] encodedSharedAccessKey;
    internal readonly string keyName;
    internal readonly TimeSpan tokenTimeToLive;
    private readonly string sharedAccessSignature;

    internal SharedAccessSignatureTokenProvider(string sharedAccessSignature)
      : base(false, true, TokenScope.Entity)
    {
      SharedAccessSignatureToken.Validate(sharedAccessSignature);
      this.sharedAccessSignature = sharedAccessSignature;
    }

    internal SharedAccessSignatureTokenProvider(
      string keyName,
      string sharedAccessKey,
      TimeSpan tokenTimeToLive)
      : this(keyName, sharedAccessKey, tokenTimeToLive, TokenScope.Entity)
    {
    }

    internal SharedAccessSignatureTokenProvider(
      string keyName,
      string sharedAccessKey,
      TimeSpan tokenTimeToLive,
      TokenScope tokenScope)
      : base(true, true, tokenScope)
    {
      if (string.IsNullOrEmpty(keyName))
        throw new ArgumentNullException(nameof (keyName));
      if (keyName.Length > 256)
        throw new ArgumentOutOfRangeException(nameof (keyName), SRCore.ArgumentStringTooBig((object) nameof (keyName), (object) 256));
      if (string.IsNullOrEmpty(sharedAccessKey))
        throw new ArgumentNullException(nameof (sharedAccessKey));
      this.encodedSharedAccessKey = sharedAccessKey.Length <= 256 ? Encoding.UTF8.GetBytes(sharedAccessKey) : throw new ArgumentOutOfRangeException(nameof (sharedAccessKey), SRCore.ArgumentStringTooBig((object) nameof (sharedAccessKey), (object) 256));
      this.keyName = keyName;
      this.tokenTimeToLive = tokenTimeToLive;
    }

    protected override bool StripQueryParameters => false;

    protected override IAsyncResult OnBeginGetToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      SharedAccessSignatureToken accessSignatureToken = new SharedAccessSignatureToken(this.BuildSignature(appliesTo));
      return (IAsyncResult) new CompletedAsyncResult<TokenProviderHelper.TokenResult<SecurityToken>>(new TokenProviderHelper.TokenResult<SecurityToken>()
      {
        CacheUntil = accessSignatureToken.ExpiresOn,
        Token = (SecurityToken) accessSignatureToken
      }, callback, state);
    }

    protected override IAsyncResult OnBeginGetWebToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      string str = this.BuildSignature(appliesTo);
      DateTime dateTime = DateTime.UtcNow + this.tokenTimeToLive;
      return (IAsyncResult) new CompletedAsyncResult<TokenProviderHelper.TokenResult<string>>(new TokenProviderHelper.TokenResult<string>()
      {
        CacheUntil = dateTime,
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

    protected override TokenProvider.Key BuildKey(string appliesTo, string action) => new TokenProvider.Key(appliesTo, string.Empty);

    private string BuildSignature(string targetUri) => !string.IsNullOrWhiteSpace(this.sharedAccessSignature) ? this.sharedAccessSignature : SharedAccessSignatureTokenProvider.SharedAccessSignatureBuilder.BuildSignature(this.keyName, this.encodedSharedAccessKey, targetUri, this.tokenTimeToLive);

    public static string GetSharedAccessSignature(
      string keyName,
      string sharedAccessKey,
      string resource,
      TimeSpan tokenTimeToLive)
    {
      if (string.IsNullOrWhiteSpace(keyName))
        throw new ArgumentException(nameof (keyName));
      if (string.IsNullOrWhiteSpace(sharedAccessKey))
        throw new ArgumentException(nameof (sharedAccessKey));
      if (string.IsNullOrWhiteSpace(resource))
        throw new ArgumentException(nameof (resource));
      if (tokenTimeToLive < TimeSpan.Zero)
        throw new ArgumentException(nameof (tokenTimeToLive));
      byte[] bytes = Encoding.UTF8.GetBytes(sharedAccessKey);
      return SharedAccessSignatureTokenProvider.SharedAccessSignatureBuilder.BuildSignature(keyName, bytes, resource, tokenTimeToLive);
    }

    internal static class SharedAccessSignatureBuilder
    {
      public static string BuildSignature(
        string keyName,
        byte[] encodedSharedAccessKey,
        string targetUri,
        TimeSpan timeToLive)
      {
        string str1 = SharedAccessSignatureTokenProvider.SharedAccessSignatureBuilder.BuildExpiresOn(timeToLive);
        string str2 = HttpUtility.UrlEncode(targetUri.ToLowerInvariant());
        string str3 = SharedAccessSignatureTokenProvider.SharedAccessSignatureBuilder.Sign(string.Join("\n", (IEnumerable<string>) new List<string>()
        {
          str2,
          str1
        }), encodedSharedAccessKey);
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}={2}&{3}={4}&{5}={6}&{7}={8}", (object) "SharedAccessSignature", (object) "sr", (object) str2, (object) "sig", (object) HttpUtility.UrlEncode(str3), (object) "se", (object) HttpUtility.UrlEncode(str1), (object) "skn", (object) HttpUtility.UrlEncode(keyName));
      }

      private static string BuildExpiresOn(TimeSpan timeToLive) => Convert.ToString(Convert.ToInt64((object) DateTime.UtcNow.Add(timeToLive).Subtract(SharedAccessSignatureTokenProvider.EpochTime).TotalSeconds, (IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture);

      private static string Sign(string requestString, byte[] encodedSharedAccessKey)
      {
        using (HMACSHA256 hmacshA256 = new HMACSHA256(encodedSharedAccessKey))
          return Convert.ToBase64String(hmacshA256.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
      }
    }
  }
}
