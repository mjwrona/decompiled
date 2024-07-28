// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SimpleWebTokenProvider
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.IdentityModel.Tokens;

namespace Microsoft.Azure.NotificationHubs
{
  public class SimpleWebTokenProvider : TokenProvider
  {
    private readonly string simpleWebToken;
    private readonly Uri stsUri;

    internal SimpleWebTokenProvider(string simpleWebToken)
      : this(simpleWebToken, TokenScope.Entity)
    {
    }

    internal SimpleWebTokenProvider(string simpleWebToken, TokenScope tokenScope)
      : base(true, true, tokenScope)
    {
      this.simpleWebToken = !string.IsNullOrEmpty(simpleWebToken) ? simpleWebToken : throw new ArgumentException(SRClient.NullSimpleWebToken, nameof (simpleWebToken));
      this.stsUri = (Uri) null;
    }

    internal SimpleWebTokenProvider(string simpleWebToken, Uri stsUri)
      : this(simpleWebToken, stsUri, TokenScope.Entity)
    {
    }

    internal SimpleWebTokenProvider(string simpleWebToken, Uri stsUri, TokenScope tokenScope)
      : base(true, true, tokenScope)
    {
      if (string.IsNullOrEmpty(simpleWebToken))
        throw new ArgumentException(SRClient.NullSimpleWebToken, nameof (simpleWebToken));
      if (stsUri == (Uri) null)
        throw new ArgumentNullException(nameof (stsUri));
      if (!stsUri.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
        throw new ArgumentException(SRClient.STSURIFormat);
      this.simpleWebToken = simpleWebToken;
      this.stsUri = stsUri;
    }

    internal string SimpleWebToken => this.simpleWebToken;

    protected override TokenProvider.Key BuildKey(string appliesTo, string action) => new TokenProvider.Key(appliesTo, string.Empty);

    protected override IAsyncResult OnBeginGetToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      this.ValidateAction(action);
      return (IAsyncResult) new CompletedAsyncResult<TokenProviderHelper.TokenResult<SecurityToken>>(TokenProviderHelper.GetAccessTokenByAssertion(TokenProviderHelper.GetStsUri(this.stsUri, appliesTo), appliesTo, this.SimpleWebToken, "SWT", timeout), callback, state);
    }

    protected override IAsyncResult OnBeginGetWebToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      this.ValidateAction(action);
      return (IAsyncResult) new CompletedAsyncResult<TokenProviderHelper.TokenResult<string>>(TokenProviderHelper.GetHttpAuthAccessTokenByAssertion(TokenProviderHelper.GetStsUri(this.stsUri, appliesTo), appliesTo, this.SimpleWebToken, "SWT", timeout), callback, state);
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
  }
}
