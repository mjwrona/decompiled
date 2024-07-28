// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SharedSecretTokenProvider
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Microsoft.Azure.NotificationHubs
{
  public class SharedSecretTokenProvider : TokenProvider
  {
    private readonly string issuerName;
    private readonly byte[] issuerSecret;
    private readonly Uri stsUri;

    internal SharedSecretTokenProvider(string issuerName, string issuerSecret)
      : this(issuerName, SharedSecretTokenProvider.DecodeSecret(issuerSecret), TokenScope.Entity)
    {
    }

    internal SharedSecretTokenProvider(
      string issuerName,
      string issuerSecret,
      TokenScope tokenScope)
      : this(issuerName, SharedSecretTokenProvider.DecodeSecret(issuerSecret), tokenScope)
    {
    }

    internal SharedSecretTokenProvider(string issuerName, byte[] issuerSecret)
      : this(issuerName, issuerSecret, TokenScope.Entity)
    {
    }

    internal SharedSecretTokenProvider(
      string issuerName,
      byte[] issuerSecret,
      TokenScope tokenScope)
      : base(true, true, tokenScope)
    {
      if (string.IsNullOrEmpty(issuerName))
        throw new ArgumentException(SRClient.NullIssuerName, nameof (issuerName));
      if (issuerSecret == null || issuerSecret.Length == 0)
        throw new ArgumentException(SRClient.NullIssuerSecret, nameof (issuerSecret));
      this.issuerName = issuerName;
      this.issuerSecret = issuerSecret;
      this.stsUri = (Uri) null;
    }

    internal SharedSecretTokenProvider(string issuerName, string issuerSecret, Uri stsUri)
      : this(issuerName, issuerSecret, stsUri, TokenScope.Entity)
    {
    }

    internal SharedSecretTokenProvider(
      string issuerName,
      string issuerSecret,
      Uri stsUri,
      TokenScope tokenScope)
      : this(issuerName, SharedSecretTokenProvider.DecodeSecret(issuerSecret), stsUri, tokenScope)
    {
    }

    internal SharedSecretTokenProvider(string issuerName, byte[] issuerSecret, Uri stsUri)
      : this(issuerName, issuerSecret, stsUri, TokenScope.Entity)
    {
    }

    internal SharedSecretTokenProvider(
      string issuerName,
      byte[] issuerSecret,
      Uri stsUri,
      TokenScope tokenScope)
      : base(true, true, tokenScope)
    {
      if (string.IsNullOrEmpty(issuerName))
        throw new ArgumentException(SRClient.NullIssuerName, nameof (issuerName));
      if (issuerSecret == null || issuerSecret.Length == 0)
        throw new ArgumentException(SRClient.NullIssuerSecret, nameof (issuerSecret));
      if (stsUri == (Uri) null)
        throw new ArgumentNullException(nameof (stsUri));
      if (!stsUri.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
        throw new ArgumentException(SRClient.STSURIFormat, nameof (stsUri));
      this.issuerName = issuerName;
      this.issuerSecret = issuerSecret;
      this.stsUri = stsUri;
    }

    internal string IssuerName => this.issuerName;

    internal byte[] IssuerSecret => this.issuerSecret;

    protected override TokenProvider.Key BuildKey(string appliesTo, string action) => new TokenProvider.Key(appliesTo, string.Empty);

    public static string ComputeSimpleWebTokenString(string issuerName, string issuerSecret)
    {
      if (string.IsNullOrEmpty(issuerName))
        throw new ArgumentException(SRClient.NullIssuerName, nameof (issuerName));
      if (string.IsNullOrEmpty(issuerSecret))
        throw new ArgumentException(SRClient.NullIssuerSecret, nameof (issuerSecret));
      byte[] issuerSecret1;
      try
      {
        issuerSecret1 = Convert.FromBase64String(issuerSecret);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(SRClient.InvalidIssuerSecret, nameof (issuerSecret));
      }
      return SharedSecretTokenProvider.ComputeSimpleWebTokenString(issuerName, issuerSecret1);
    }

    public static string ComputeSimpleWebTokenString(string issuerName, byte[] issuerSecret)
    {
      if (string.IsNullOrEmpty(issuerName))
        throw new ArgumentException(SRClient.NullIssuerName, nameof (issuerName));
      if (issuerSecret == null || issuerSecret.Length < 1)
        throw new ArgumentException(SRClient.NullIssuerSecret, nameof (issuerSecret));
      string s = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
      {
        (object) "Issuer",
        (object) HttpUtility.UrlEncode(issuerName)
      });
      string base64String;
      using (HMACSHA256 hmacshA256 = new HMACSHA256(issuerSecret))
        base64String = Convert.ToBase64String(hmacshA256.ComputeHash(Encoding.UTF8.GetBytes(s)));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(s);
      stringBuilder.Append('&');
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
      {
        (object) "HMACSHA256",
        (object) HttpUtility.UrlEncode(base64String)
      }));
      return stringBuilder.ToString();
    }

    protected override IAsyncResult OnBeginGetToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      this.ValidateAction(action);
      return TokenProviderHelper.BeginGetAccessTokenByAssertion(TokenProviderHelper.GetStsUri(this.stsUri, appliesTo), appliesTo, SharedSecretTokenProvider.ComputeSimpleWebTokenString(this.issuerName, this.issuerSecret), "SWT", timeout, callback, state);
    }

    protected override IAsyncResult OnBeginGetWebToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      this.ValidateAction(action);
      return (IAsyncResult) new CompletedAsyncResult<TokenProviderHelper.TokenResult<string>>(TokenProviderHelper.GetHttpAuthAccessTokenByAssertion(TokenProviderHelper.GetStsUri(this.stsUri, appliesTo), appliesTo, SharedSecretTokenProvider.ComputeSimpleWebTokenString(this.issuerName, this.issuerSecret), "SWT", timeout), callback, state);
    }

    protected override SecurityToken OnEndGetToken(IAsyncResult result, out DateTime cacheUntil)
    {
      TokenProviderHelper.TokenResult<SecurityToken> tokenByAssertion = TokenProviderHelper.EndGetAccessTokenByAssertion(result);
      cacheUntil = tokenByAssertion.CacheUntil;
      return tokenByAssertion.Token;
    }

    protected override string OnEndGetWebToken(IAsyncResult result, out DateTime cacheUntil)
    {
      TokenProviderHelper.TokenResult<string> tokenResult = CompletedAsyncResult<TokenProviderHelper.TokenResult<string>>.End(result);
      cacheUntil = tokenResult.CacheUntil;
      return tokenResult.Token;
    }

    internal static byte[] DecodeSecret(string issuerSecret)
    {
      try
      {
        return Convert.FromBase64String(issuerSecret);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(SRClient.InvalidIssuerSecret, nameof (issuerSecret));
      }
    }
  }
}
