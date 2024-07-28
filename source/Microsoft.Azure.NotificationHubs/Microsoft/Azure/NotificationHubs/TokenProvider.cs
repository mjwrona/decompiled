// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.TokenProvider
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Channels;
using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Common.Parallel;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.PerformanceCounters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs
{
  public abstract class TokenProvider
  {
    internal static readonly TimeSpan DefaultTokenTimeout = TimeSpan.FromMinutes(20.0);
    private const int DefaultCacheSize = 1000;
    private const TokenScope DefaultTokenScope = TokenScope.Entity;
    private static readonly TimeSpan InitialRetrySleepTime = TimeSpan.FromMilliseconds(50.0);
    private static readonly TimeSpan MaxRetrySleepTime = TimeSpan.FromSeconds(3.0);
    private readonly bool isWebTokenSupported;
    private readonly object mutex = new object();
    private readonly TimeSpan retrySleepTime;
    private int cacheSize;
    private MruCache<TokenProvider.Key, TokenProvider.TokenInfo> tokenCache;

    protected TokenProvider(bool cacheTokens, bool supportHttpAuthToken)
      : this(cacheTokens, supportHttpAuthToken, 1000, TokenScope.Entity)
    {
    }

    protected TokenProvider(bool cacheTokens, bool supportHttpAuthToken, TokenScope tokenScope)
      : this(cacheTokens, supportHttpAuthToken, 1000, tokenScope)
    {
    }

    protected TokenProvider(
      bool cacheTokens,
      bool supportHttpAuthToken,
      int cacheSize,
      TokenScope tokenScope)
    {
      if (cacheSize < 1)
        throw new ArgumentOutOfRangeException(nameof (cacheSize), SRClient.ArgumentOutOfRangeLessThanOne);
      this.TokenScope = tokenScope;
      this.cacheSize = cacheSize;
      this.CacheTokens = cacheTokens;
      this.isWebTokenSupported = supportHttpAuthToken;
      this.retrySleepTime = TokenProvider.InitialRetrySleepTime;
    }

    public TokenScope TokenScope { get; private set; }

    public bool CacheTokens
    {
      get
      {
        lock (this.mutex)
          return this.tokenCache != null;
      }
      set
      {
        lock (this.mutex)
        {
          if (value)
            this.tokenCache = new MruCache<TokenProvider.Key, TokenProvider.TokenInfo>(this.cacheSize);
          else
            this.tokenCache = (MruCache<TokenProvider.Key, TokenProvider.TokenInfo>) null;
        }
      }
    }

    public int CacheSize
    {
      get
      {
        lock (this.mutex)
          return this.cacheSize;
      }
      set
      {
        lock (this.mutex)
        {
          this.cacheSize = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof (value), SRClient.ArgumentOutOfRangeLessThanOne);
          this.Clear();
        }
      }
    }

    public bool IsWebTokenSupported => this.isWebTokenSupported;

    internal MruCache<TokenProvider.Key, TokenProvider.TokenInfo> TokenCache => this.tokenCache;

    protected virtual bool StripQueryParameters => true;

    public void Clear()
    {
      lock (this.mutex)
      {
        if (this.tokenCache == null)
          return;
        this.tokenCache = new MruCache<TokenProvider.Key, TokenProvider.TokenInfo>(this.cacheSize);
      }
    }

    public static TokenProvider CreateSharedAccessSignatureTokenProvider(
      string sharedAccessSignature)
    {
      return (TokenProvider) new SharedAccessSignatureTokenProvider(sharedAccessSignature);
    }

    public static TokenProvider CreateSharedAccessSignatureTokenProvider(
      string keyName,
      string sharedAccessKey)
    {
      return (TokenProvider) new SharedAccessSignatureTokenProvider(keyName, sharedAccessKey, TokenProvider.DefaultTokenTimeout);
    }

    public static TokenProvider CreateSharedAccessSignatureTokenProvider(
      string keyName,
      string sharedAccessKey,
      TimeSpan tokenTimeToLive)
    {
      return (TokenProvider) new SharedAccessSignatureTokenProvider(keyName, sharedAccessKey, tokenTimeToLive);
    }

    public static TokenProvider CreateSharedAccessSignatureTokenProvider(
      string keyName,
      string sharedAccessKey,
      TokenScope tokenScope)
    {
      return (TokenProvider) new SharedAccessSignatureTokenProvider(keyName, sharedAccessKey, TokenProvider.DefaultTokenTimeout, tokenScope);
    }

    public static TokenProvider CreateSharedAccessSignatureTokenProvider(
      string keyName,
      string sharedAccessKey,
      TimeSpan tokenTimeToLive,
      TokenScope tokenScope)
    {
      return (TokenProvider) new SharedAccessSignatureTokenProvider(keyName, sharedAccessKey, tokenTimeToLive, tokenScope);
    }

    public static TokenProvider CreateSharedSecretTokenProvider(
      string issuerName,
      string issuerSecret)
    {
      return (TokenProvider) new SharedSecretTokenProvider(issuerName, issuerSecret);
    }

    public static TokenProvider CreateSharedSecretTokenProvider(
      string issuerName,
      string issuerSecret,
      Uri stsUri)
    {
      return (TokenProvider) new SharedSecretTokenProvider(issuerName, issuerSecret, stsUri);
    }

    public static TokenProvider CreateSharedSecretTokenProvider(
      string issuerName,
      byte[] issuerSecret)
    {
      return (TokenProvider) new SharedSecretTokenProvider(issuerName, issuerSecret);
    }

    public static TokenProvider CreateSharedSecretTokenProvider(
      string issuerName,
      byte[] issuerSecret,
      Uri stsUri)
    {
      return (TokenProvider) new SharedSecretTokenProvider(issuerName, issuerSecret, stsUri);
    }

    public static TokenProvider CreateSharedSecretTokenProvider(
      string issuerName,
      string issuerSecret,
      TokenScope tokenScope)
    {
      return (TokenProvider) new SharedSecretTokenProvider(issuerName, issuerSecret, tokenScope);
    }

    public static TokenProvider CreateSharedSecretTokenProvider(
      string issuerName,
      string issuerSecret,
      Uri stsUri,
      TokenScope tokenScope)
    {
      return (TokenProvider) new SharedSecretTokenProvider(issuerName, issuerSecret, stsUri, tokenScope);
    }

    public static TokenProvider CreateSharedSecretTokenProvider(
      string issuerName,
      byte[] issuerSecret,
      TokenScope tokenScope)
    {
      return (TokenProvider) new SharedSecretTokenProvider(issuerName, issuerSecret, tokenScope);
    }

    public static TokenProvider CreateSharedSecretTokenProvider(
      string issuerName,
      byte[] issuerSecret,
      Uri stsUri,
      TokenScope tokenScope)
    {
      return (TokenProvider) new SharedSecretTokenProvider(issuerName, issuerSecret, stsUri, tokenScope);
    }

    public static TokenProvider CreateSimpleWebTokenProvider(string token) => (TokenProvider) new SimpleWebTokenProvider(token);

    public static TokenProvider CreateSimpleWebTokenProvider(string token, Uri stsUri) => (TokenProvider) new SimpleWebTokenProvider(token, stsUri);

    public static TokenProvider CreateSimpleWebTokenProvider(string token, TokenScope tokenScope) => (TokenProvider) new SimpleWebTokenProvider(token, tokenScope);

    public static TokenProvider CreateSimpleWebTokenProvider(
      string token,
      Uri stsUri,
      TokenScope tokenScope)
    {
      return (TokenProvider) new SimpleWebTokenProvider(token, stsUri, tokenScope);
    }

    public static TokenProvider CreateWindowsTokenProvider(IEnumerable<Uri> stsUris) => (TokenProvider) new WindowsTokenProvider(stsUris, (NetworkCredential) null);

    public static TokenProvider CreateWindowsTokenProvider(
      IEnumerable<Uri> stsUris,
      NetworkCredential credential)
    {
      return (TokenProvider) new WindowsTokenProvider(stsUris, credential);
    }

    public static TokenProvider CreateOAuthTokenProvider(
      IEnumerable<Uri> stsUris,
      NetworkCredential credential)
    {
      return (TokenProvider) new OAuthTokenProvider(stsUris, credential);
    }

    public IAsyncResult BeginGetToken(
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetToken((Uri) null, appliesTo, action, bypassCache, timeout, callback, state);
    }

    internal IAsyncResult BeginGetToken(
      Uri namespaceAddress,
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      if (string.IsNullOrEmpty(appliesTo))
        throw new ArgumentException(SRClient.NullAppliesTo);
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      if (timeout > TimeoutHelper.MaxWait)
        timeout = TimeoutHelper.MaxWait;
      TokenProvider.ValidateTimeout(timeout);
      return (IAsyncResult) new TokenProvider.GetTokenAsyncResult(this, this.NormalizeAppliesTo(namespaceAddress, appliesTo), action, bypassCache, timeout, callback, state);
    }

    public IAsyncResult BeginGetWebToken(
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetWebToken((Uri) null, appliesTo, action, bypassCache, timeout, callback, state);
    }

    internal IAsyncResult BeginGetWebToken(
      Uri namespaceAddress,
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      if (string.IsNullOrEmpty(appliesTo))
        throw new ArgumentException(SRClient.NullAppliesTo);
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      if (timeout > TimeoutHelper.MaxWait)
        timeout = TimeoutHelper.MaxWait;
      TokenProvider.ValidateTimeout(timeout);
      if (!this.IsWebTokenSupported)
        throw new InvalidOperationException(SRClient.BeginGetWebTokenNotSupported);
      return (IAsyncResult) new TokenProvider.GetWebTokenAsyncResult(this, this.NormalizeAppliesTo(namespaceAddress, appliesTo), action, bypassCache, timeout, callback, state);
    }

    public SecurityToken EndGetToken(IAsyncResult result) => AsyncResult<TokenProvider.GetTokenAsyncResult>.End(result).SecurityToken;

    public Task<SecurityToken> GetTokenAsync(
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout)
    {
      return TaskHelpers.CreateTask<SecurityToken>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginGetToken(appliesTo, action, bypassCache, timeout, c, s)), new Func<IAsyncResult, SecurityToken>(this.EndGetToken));
    }

    public Task<string> GetWebTokenAsync(
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout)
    {
      return TaskHelpers.CreateTask<string>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginGetWebToken(appliesTo, action, bypassCache, timeout, c, s)), new Func<IAsyncResult, string>(this.EndGetWebToken));
    }

    public string EndGetWebToken(IAsyncResult result) => AsyncResult<TokenProvider.GetWebTokenAsyncResult>.End(result).WebToken;

    internal void ValidateAction(string action)
    {
      switch (action)
      {
        case "Send":
          break;
        case "Listen":
          break;
        case "Manage":
          break;
        default:
          throw new ArgumentException(SRClient.UnsupportedAction((object) action), nameof (action));
      }
    }

    protected abstract IAsyncResult OnBeginGetToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state);

    protected abstract IAsyncResult OnBeginGetWebToken(
      string appliesTo,
      string action,
      TimeSpan timeout,
      AsyncCallback callback,
      object state);

    protected abstract SecurityToken OnEndGetToken(IAsyncResult result, out DateTime cacheUntil);

    protected abstract string OnEndGetWebToken(IAsyncResult result, out DateTime cacheUntil);

    protected virtual TokenProvider.Key BuildKey(string appliesTo, string action) => new TokenProvider.Key(appliesTo, action);

    protected virtual string NormalizeAppliesTo(string appliesTo) => ServiceBusUriHelper.NormalizeUri(appliesTo, "http", this.StripQueryParameters, this.TokenScope == TokenScope.Namespace, true);

    private string NormalizeAppliesTo(Uri namespaceAddress, string appliesTo) => namespaceAddress != (Uri) null ? ServiceBusUriHelper.NormalizeUri(this.TokenScope != TokenScope.Namespace || !(namespaceAddress != (Uri) null) ? appliesTo : namespaceAddress.AbsoluteUri, "http", this.StripQueryParameters, ensureTrailingSlash: true) : this.NormalizeAppliesTo(appliesTo);

    private static void ValidateTimeout(TimeSpan timeout)
    {
      if (timeout < TimeSpan.Zero)
        throw new ArgumentOutOfRangeException(nameof (timeout));
    }

    private TokenProvider.TokenInfo GetTokenInfoFromCache(TokenProvider.Key key)
    {
      lock (this.mutex)
      {
        if (!this.CacheTokens)
          return new TokenProvider.TokenInfo();
        TokenProvider.TokenInfo tokenInfoFromCache1;
        if (!this.tokenCache.TryGetValue(key, out tokenInfoFromCache1))
        {
          TokenProvider.TokenInfo tokenInfoFromCache2 = new TokenProvider.TokenInfo();
          this.tokenCache.Add(key, tokenInfoFromCache2);
          return tokenInfoFromCache2;
        }
        DateTime utcNow = DateTime.UtcNow;
        if (tokenInfoFromCache1.WebTokenCacheUntil < utcNow)
          tokenInfoFromCache1.ResetWebToken();
        if (tokenInfoFromCache1.SecurityTokenCacheUntil < utcNow)
          tokenInfoFromCache1.ResetSecurityToken();
        return tokenInfoFromCache1;
      }
    }

    private abstract class GetTokenAsyncResultBase<T> : IteratorAsyncResult<T> where T : TokenProvider.GetTokenAsyncResultBase<T>
    {
      private readonly TokenProvider.Key cacheKey;
      private readonly bool bypassCache;
      private readonly Uri appliesToUri;
      private TimeSpan retrySleepTime;

      protected GetTokenAsyncResultBase(
        TokenProvider tokenProvider,
        string appliesTo,
        string action,
        bool bypassCache,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        this.TokenProvider = tokenProvider;
        this.cacheKey = this.TokenProvider.BuildKey(appliesTo, action);
        this.retrySleepTime = this.TokenProvider.retrySleepTime;
        this.bypassCache = bypassCache;
        this.AppliesTo = appliesTo;
        this.Action = action;
        if (Uri.TryCreate(appliesTo, UriKind.RelativeOrAbsolute, out this.appliesToUri))
          return;
        this.appliesToUri = new UriBuilder()
        {
          Scheme = "sb",
          Host = appliesTo
        }.Uri;
      }

      protected TokenProvider TokenProvider { get; private set; }

      protected string AppliesTo { get; private set; }

      protected string Action { get; private set; }

      protected abstract IteratorAsyncResult<T>.BeginCall GetTokenBeginCall { get; }

      protected override IEnumerator<IteratorAsyncResult<T>.AsyncStep> GetAsyncSteps()
      {
        bool retry;
        do
        {
          retry = false;
          if (!this.bypassCache)
          {
            bool flag;
            lock (this.TokenProvider.mutex)
              flag = this.OnProcessCachedEntryFromTokenProvider(this.TokenProvider.GetTokenInfoFromCache(this.cacheKey));
            if (flag)
              yield break;
          }
          Stopwatch perfWatch = Stopwatch.StartNew();
          try
          {
            yield return this.CallAsync(this.GetTokenBeginCall, (IteratorAsyncResult<T>.EndCall) ((thisPtr, r) => thisPtr.OnCompletion(r)), IteratorAsyncResult<T>.ExceptionPolicy.Continue);
            if (this.LastAsyncStepException == null)
            {
              perfWatch.Stop();
              MessagingPerformanceCounters.IncrementTokenAcquisitionLatency(this.appliesToUri, perfWatch.ElapsedTicks);
              MessagingPerformanceCounters.IncrementTokensAcquiredPerSec(this.appliesToUri, 1);
              yield break;
            }
          }
          finally
          {
            perfWatch.Stop();
          }
          MessagingPerformanceCounters.IncrementTokenAcquisitionFailuresPerSec(this.appliesToUri, 1);
          SecurityTokenException asyncStepException1 = this.LastAsyncStepException as SecurityTokenException;
          TokenProviderException asyncStepException2 = this.LastAsyncStepException as TokenProviderException;
          if (this.LastAsyncStepException is TimeoutException asyncStepException3 && asyncStepException3.InnerException != null && asyncStepException3.InnerException is WebException)
            retry = true;
          else if (asyncStepException2 != null && asyncStepException2.InnerException != null && asyncStepException2.InnerException is WebException)
            retry = true;
          else if (asyncStepException1 != null)
          {
            TokenProviderHelper.InternalSecurityTokenException securityTokenException = asyncStepException1 as TokenProviderHelper.InternalSecurityTokenException;
            retry = true;
            if (securityTokenException != null)
            {
              this.LastAsyncStepException = (Exception) new SecurityTokenException(securityTokenException.Message, securityTokenException.InnerException);
              switch (securityTokenException.StatusCode)
              {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                  retry = false;
                  break;
              }
            }
          }
          if (retry)
          {
            TimeSpan val2 = this.RemainingTime();
            if (val2 > TimeSpan.Zero)
            {
              yield return this.CallAsyncSleep(TimeoutHelper.Min(this.retrySleepTime, val2));
              TimeSpan timeSpan = this.retrySleepTime.Add(this.retrySleepTime);
              this.retrySleepTime = timeSpan < TokenProvider.MaxRetrySleepTime ? timeSpan : TokenProvider.MaxRetrySleepTime;
            }
            else
              retry = false;
          }
          perfWatch = (Stopwatch) null;
        }
        while (retry);
        if (this.LastAsyncStepException != null)
          this.Complete(this.LastAsyncStepException);
      }

      private void OnCompletion(IAsyncResult result)
      {
        DateTime cacheUntil;
        this.OnEndTokenProviderCallback(result, out cacheUntil);
        if (!(cacheUntil >= DateTime.UtcNow))
          return;
        lock (this.TokenProvider.mutex)
        {
          TokenProvider.TokenInfo tokenInfoFromCache = this.TokenProvider.GetTokenInfoFromCache(this.cacheKey);
          this.OnUpdateTokenProviderCacheEntry(cacheUntil, ref tokenInfoFromCache);
        }
      }

      protected abstract bool OnProcessCachedEntryFromTokenProvider(
        TokenProvider.TokenInfo tokenInfo);

      protected abstract void OnEndTokenProviderCallback(
        IAsyncResult result,
        out DateTime cacheUntil);

      protected abstract void OnUpdateTokenProviderCacheEntry(
        DateTime cacheUntil,
        ref TokenProvider.TokenInfo tokenInfo);
    }

    private sealed class GetTokenAsyncResult : 
      TokenProvider.GetTokenAsyncResultBase<TokenProvider.GetTokenAsyncResult>
    {
      internal GetTokenAsyncResult(
        TokenProvider tokenProvider,
        string appliesTo,
        string action,
        bool bypassCache,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(tokenProvider, appliesTo, action, bypassCache, timeout, callback, state)
      {
        this.Start();
      }

      public SecurityToken SecurityToken { get; private set; }

      protected override IteratorAsyncResult<TokenProvider.GetTokenAsyncResult>.BeginCall GetTokenBeginCall => (IteratorAsyncResult<TokenProvider.GetTokenAsyncResult>.BeginCall) ((thisPtr, t, c, s) => thisPtr.TokenProvider.OnBeginGetToken(thisPtr.AppliesTo, thisPtr.Action, t, c, s));

      protected override bool OnProcessCachedEntryFromTokenProvider(
        TokenProvider.TokenInfo tokenInfo)
      {
        this.SecurityToken = tokenInfo.SecurityToken;
        return this.SecurityToken != null;
      }

      protected override void OnEndTokenProviderCallback(
        IAsyncResult result,
        out DateTime cacheUntil)
      {
        this.SecurityToken = this.TokenProvider.OnEndGetToken(result, out cacheUntil);
      }

      protected override void OnUpdateTokenProviderCacheEntry(
        DateTime cacheUntil,
        ref TokenProvider.TokenInfo tokenInfo)
      {
        if (tokenInfo.SecurityToken != null && !(cacheUntil > tokenInfo.SecurityTokenCacheUntil))
          return;
        tokenInfo.SecurityToken = this.SecurityToken;
        tokenInfo.SecurityTokenCacheUntil = cacheUntil;
      }
    }

    private sealed class GetWebTokenAsyncResult : 
      TokenProvider.GetTokenAsyncResultBase<TokenProvider.GetWebTokenAsyncResult>
    {
      internal GetWebTokenAsyncResult(
        TokenProvider tokenProvider,
        string appliesTo,
        string action,
        bool bypassCache,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(tokenProvider, appliesTo, action, bypassCache, timeout, callback, state)
      {
        this.Start();
      }

      public string WebToken { get; private set; }

      protected override IteratorAsyncResult<TokenProvider.GetWebTokenAsyncResult>.BeginCall GetTokenBeginCall => (IteratorAsyncResult<TokenProvider.GetWebTokenAsyncResult>.BeginCall) ((thisPtr, t, c, s) => thisPtr.TokenProvider.OnBeginGetWebToken(thisPtr.AppliesTo, thisPtr.Action, t, c, s));

      protected override bool OnProcessCachedEntryFromTokenProvider(
        TokenProvider.TokenInfo tokenInfo)
      {
        this.WebToken = tokenInfo.WebToken;
        return this.WebToken != null;
      }

      protected override void OnEndTokenProviderCallback(
        IAsyncResult result,
        out DateTime cacheUntil)
      {
        this.WebToken = this.TokenProvider.OnEndGetWebToken(result, out cacheUntil);
      }

      protected override void OnUpdateTokenProviderCacheEntry(
        DateTime cacheUntil,
        ref TokenProvider.TokenInfo tokenInfo)
      {
        if (tokenInfo.WebToken != null && !(cacheUntil > tokenInfo.WebTokenCacheUntil))
          return;
        tokenInfo.WebToken = this.WebToken;
        tokenInfo.WebTokenCacheUntil = cacheUntil;
      }
    }

    protected internal class Key : IEquatable<TokenProvider.Key>
    {
      private readonly string appliesTo;
      private readonly string claim;

      public Key(string appliesTo, string claim)
      {
        if (appliesTo == null)
          throw new ArgumentNullException(nameof (appliesTo));
        if (claim == null)
          throw new ArgumentNullException(nameof (claim));
        this.appliesTo = appliesTo;
        this.claim = claim;
      }

      public override bool Equals(object obj) => this.Equals(obj as TokenProvider.Key);

      public override int GetHashCode() => this.appliesTo.GetHashCode() * 397 ^ this.claim.GetHashCode();

      public bool Equals(TokenProvider.Key other)
      {
        if (other == null)
          return false;
        if (this == other)
          return true;
        return object.Equals((object) other.appliesTo, (object) this.appliesTo) && object.Equals((object) other.claim, (object) this.claim);
      }
    }

    internal class TokenInfo
    {
      internal TokenInfo()
      {
        this.ResetSecurityToken();
        this.ResetWebToken();
      }

      internal SecurityToken SecurityToken { get; set; }

      internal DateTime SecurityTokenCacheUntil { get; set; }

      internal string WebToken { get; set; }

      internal DateTime WebTokenCacheUntil { get; set; }

      internal void ResetSecurityToken()
      {
        this.SecurityToken = (SecurityToken) null;
        this.SecurityTokenCacheUntil = DateTime.MaxValue;
      }

      internal void ResetWebToken()
      {
        this.WebToken = (string) null;
        this.WebTokenCacheUntil = DateTime.MaxValue;
      }
    }
  }
}
