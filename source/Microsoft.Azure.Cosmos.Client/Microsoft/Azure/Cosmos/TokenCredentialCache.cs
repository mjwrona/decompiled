// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.TokenCredentialCache
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure;
using Azure.Core;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.Azure.Cosmos
{
  internal sealed class TokenCredentialCache : IDisposable
  {
    public static readonly double DefaultBackgroundTokenCredentialRefreshIntervalPercentage = 0.5;
    public static readonly TimeSpan MaxBackgroundRefreshInterval = TimeSpan.FromMilliseconds((double) int.MaxValue);
    public static readonly TimeSpan MinimumTimeBetweenBackgroundRefreshInterval = TimeSpan.FromMinutes(1.0);
    private const string ScopeFormat = "https://{0}/.default";
    private readonly TokenRequestContext tokenRequestContext;
    private readonly TokenCredential tokenCredential;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly CancellationToken cancellationToken;
    private readonly TimeSpan? userDefinedBackgroundTokenCredentialRefreshInterval;
    private readonly SemaphoreSlim isTokenRefreshingLock = new SemaphoreSlim(1);
    private readonly object backgroundRefreshLock = new object();
    private TimeSpan? systemBackgroundTokenCredentialRefreshInterval;
    private Task<AccessToken>? currentRefreshOperation;
    private AccessToken? cachedAccessToken;
    private bool isBackgroundTaskRunning;
    private bool isDisposed;

    internal TokenCredentialCache(
      TokenCredential tokenCredential,
      Uri accountEndpoint,
      TimeSpan? backgroundTokenCredentialRefreshInterval)
    {
      this.tokenCredential = tokenCredential ?? throw new ArgumentNullException(nameof (tokenCredential));
      this.tokenRequestContext = !(accountEndpoint == (Uri) null) ? new TokenRequestContext(new string[1]
      {
        string.Format("https://{0}/.default", (object) accountEndpoint.Host)
      }, (string) null, (string) null, (string) null) : throw new ArgumentNullException(nameof (accountEndpoint));
      if (backgroundTokenCredentialRefreshInterval.HasValue)
      {
        if (backgroundTokenCredentialRefreshInterval.Value <= TimeSpan.Zero)
          throw new ArgumentException(string.Format("{0} must be a positive value greater than 0. Value '{1}'.", (object) nameof (backgroundTokenCredentialRefreshInterval), (object) backgroundTokenCredentialRefreshInterval.Value.TotalMilliseconds));
        if (backgroundTokenCredentialRefreshInterval.Value > TokenCredentialCache.MaxBackgroundRefreshInterval && backgroundTokenCredentialRefreshInterval.Value != TimeSpan.MaxValue)
          throw new ArgumentException(string.Format("{0} must be less than or equal to {1}. Value '{2}'.", (object) nameof (backgroundTokenCredentialRefreshInterval), (object) TokenCredentialCache.MaxBackgroundRefreshInterval, (object) backgroundTokenCredentialRefreshInterval.Value));
      }
      this.userDefinedBackgroundTokenCredentialRefreshInterval = backgroundTokenCredentialRefreshInterval;
      this.cancellationTokenSource = new CancellationTokenSource();
      this.cancellationToken = this.cancellationTokenSource.Token;
    }

    public TimeSpan? BackgroundTokenCredentialRefreshInterval => this.userDefinedBackgroundTokenCredentialRefreshInterval ?? this.systemBackgroundTokenCredentialRefreshInterval;

    internal async ValueTask<string> GetTokenAsync(ITrace trace)
    {
      TokenCredentialCache tokenCredentialCache = this;
      if (tokenCredentialCache.isDisposed)
        throw new ObjectDisposedException(nameof (TokenCredentialCache));
      if (tokenCredentialCache.cachedAccessToken.HasValue)
      {
        DateTimeOffset utcNow = (DateTimeOffset) DateTime.UtcNow;
        AccessToken accessToken1 = tokenCredentialCache.cachedAccessToken.Value;
        DateTimeOffset expiresOn = ((AccessToken) ref accessToken1).ExpiresOn;
        if (utcNow < expiresOn)
        {
          AccessToken accessToken2 = tokenCredentialCache.cachedAccessToken.Value;
          return ((AccessToken) ref accessToken2).Token;
        }
      }
      AccessToken newTokenAsync = await tokenCredentialCache.GetNewTokenAsync(trace);
      if (!tokenCredentialCache.isBackgroundTaskRunning)
        Task.Run(new Action(tokenCredentialCache.StartBackgroundTokenRefreshLoop));
      return ((AccessToken) ref newTokenAsync).Token;
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      this.cancellationTokenSource.Cancel();
      this.cancellationTokenSource.Dispose();
      this.isDisposed = true;
    }

    private async Task<AccessToken> GetNewTokenAsync(ITrace trace)
    {
      Task<AccessToken> task = this.currentRefreshOperation;
      if (task != null)
        return await task;
      try
      {
        await this.isTokenRefreshingLock.WaitAsync();
        if (this.currentRefreshOperation == null)
        {
          task = this.RefreshCachedTokenWithRetryHelperAsync(trace).AsTask();
          this.currentRefreshOperation = task;
        }
        else
          task = this.currentRefreshOperation;
      }
      finally
      {
        this.isTokenRefreshingLock.Release();
      }
      return await task;
    }

    private async ValueTask<AccessToken> RefreshCachedTokenWithRetryHelperAsync(ITrace trace)
    {
      object obj = (object) null;
      int num = 0;
      AccessToken accessToken;
      try
      {
        Exception exception1 = (Exception) null;
        for (int retry = 0; retry < 2; ++retry)
        {
          if (this.cancellationToken.IsCancellationRequested)
          {
            DefaultTrace.TraceInformation("Stop RefreshTokenWithIndefiniteRetries because cancellation is requested");
            break;
          }
          ITrace getTokenTrace = trace.StartChild(nameof (RefreshCachedTokenWithRetryHelperAsync), TraceComponent.Authorization, TraceLevel.Info);
          try
          {
            this.cachedAccessToken = new AccessToken?(await this.tokenCredential.GetTokenAsync(this.tokenRequestContext, this.cancellationToken));
            AccessToken accessToken1 = this.cachedAccessToken.HasValue ? this.cachedAccessToken.Value : throw new ArgumentNullException("TokenCredential.GetTokenAsync returned a null token.");
            if (((AccessToken) ref accessToken1).ExpiresOn < DateTimeOffset.UtcNow)
            {
              // ISSUE: variable of a boxed type
              __Boxed<DateTime> utcNow = (ValueType) DateTime.UtcNow;
              AccessToken accessToken2 = this.cachedAccessToken.Value;
              // ISSUE: variable of a boxed type
              __Boxed<DateTimeOffset> expiresOn = (ValueType) ((AccessToken) ref accessToken2).ExpiresOn;
              throw new ArgumentOutOfRangeException(string.Format("TokenCredential.GetTokenAsync returned a token that is already expired. Current Time:{0:O}; Token expire time:{1:O}", (object) utcNow, (object) expiresOn));
            }
            if (!this.userDefinedBackgroundTokenCredentialRefreshInterval.HasValue)
            {
              AccessToken accessToken3 = this.cachedAccessToken.Value;
              double val1_1 = (((AccessToken) ref accessToken3).ExpiresOn - DateTimeOffset.UtcNow).TotalSeconds * TokenCredentialCache.DefaultBackgroundTokenCredentialRefreshIntervalPercentage;
              TimeSpan backgroundRefreshInterval = TokenCredentialCache.MinimumTimeBetweenBackgroundRefreshInterval;
              double totalSeconds1 = backgroundRefreshInterval.TotalSeconds;
              double val1_2 = Math.Max(val1_1, totalSeconds1);
              backgroundRefreshInterval = TokenCredentialCache.MaxBackgroundRefreshInterval;
              double totalSeconds2 = backgroundRefreshInterval.TotalSeconds;
              this.systemBackgroundTokenCredentialRefreshInterval = new TimeSpan?(TimeSpan.FromSeconds(Math.Min(val1_2, totalSeconds2)));
            }
            accessToken = this.cachedAccessToken.Value;
            goto label_27;
          }
          catch (RequestFailedException ex)
          {
            exception1 = (Exception) ex;
            getTokenTrace.AddDatum("RequestFailedException at " + DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) ex);
            DefaultTrace.TraceError(string.Format("TokenCredential.GetToken() failed with RequestFailedException. scope = {0}, retry = {1}, Exception = {2}", (object) string.Join(";", ((TokenRequestContext) ref this.tokenRequestContext).Scopes), (object) retry, (object) exception1));
            if (ex.Status != 401)
            {
              if (ex.Status != 403)
                goto label_22;
            }
            this.cachedAccessToken = new AccessToken?();
            throw;
          }
          catch (OperationCanceledException ex)
          {
            exception1 = (Exception) ex;
            getTokenTrace.AddDatum("OperationCanceledException at " + DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) ex);
            DefaultTrace.TraceError(string.Format("TokenCredential.GetTokenAsync() failed. scope = {0}, retry = {1}, Exception = {2}", (object) string.Join(";", ((TokenRequestContext) ref this.tokenRequestContext).Scopes), (object) retry, (object) exception1));
            string failedToGetAadToken = ClientResources.FailedToGetAadToken;
            Headers headers = new Headers();
            headers.SubStatusCode = SubStatusCodes.FailedToGetAadToken;
            Exception exception2 = exception1;
            ITrace trace1 = getTokenTrace;
            Exception innerException = exception2;
            throw CosmosExceptionFactory.CreateRequestTimeoutException(failedToGetAadToken, headers, trace: trace1, innerException: innerException);
          }
          catch (Exception ex)
          {
            exception1 = ex;
            getTokenTrace.AddDatum("Exception at " + DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) ex);
            DefaultTrace.TraceError(string.Format("TokenCredential.GetTokenAsync() failed. scope = {0}, retry = {1}, Exception = {2}", (object) string.Join(";", ((TokenRequestContext) ref this.tokenRequestContext).Scopes), (object) retry, (object) exception1));
          }
          finally
          {
            getTokenTrace?.Dispose();
          }
label_22:
          getTokenTrace = (ITrace) null;
          continue;
label_27:
          num = 1;
          goto label_29;
        }
        if (exception1 == null)
          throw new ArgumentException("Last exception is null.");
        throw exception1;
      }
      catch (object ex)
      {
        obj = ex;
      }
label_29:
      try
      {
        await this.isTokenRefreshingLock.WaitAsync();
        this.currentRefreshOperation = (Task<AccessToken>) null;
      }
      finally
      {
        this.isTokenRefreshingLock.Release();
      }
      object obj1 = obj;
      if (obj1 != null)
      {
        if (!(obj1 is Exception source))
          throw obj1;
        ExceptionDispatchInfo.Capture(source).Throw();
      }
      if (num == 1)
        return accessToken;
      obj = (object) null;
      accessToken = new AccessToken();
      AccessToken accessToken4;
      return accessToken4;
    }

    private async void StartBackgroundTokenRefreshLoop()
    {
      if (this.isBackgroundTaskRunning)
        return;
      lock (this.backgroundRefreshLock)
      {
        if (this.isBackgroundTaskRunning)
          return;
        this.isBackgroundTaskRunning = true;
      }
      TimeSpan? nullable;
      while (!this.cancellationTokenSource.IsCancellationRequested)
      {
        try
        {
          nullable = this.BackgroundTokenCredentialRefreshInterval;
          nullable = nullable.HasValue ? this.BackgroundTokenCredentialRefreshInterval : throw new ArgumentException("BackgroundTokenCredentialRefreshInterval");
          if (nullable.Value > TokenCredentialCache.MaxBackgroundRefreshInterval)
          {
            object[] objArray = new object[1];
            nullable = this.BackgroundTokenCredentialRefreshInterval;
            objArray[0] = (object) nullable.Value;
            DefaultTrace.TraceWarning("BackgroundTokenRefreshLoop() Stopped - The BackgroundTokenCredentialRefreshInterval is {0} which is greater than the maximum allow.", objArray);
            break;
          }
          nullable = this.BackgroundTokenCredentialRefreshInterval;
          await Task.Delay(nullable.Value, this.cancellationToken);
          DefaultTrace.TraceInformation("BackgroundTokenRefreshLoop() - Invoking refresh");
          AccessToken newTokenAsync = await this.GetNewTokenAsync((ITrace) Microsoft.Azure.Cosmos.Tracing.Trace.GetRootTrace("TokenCredentialCacheBackground refresh"));
        }
        catch (Exception ex)
        {
          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            switch (ex)
            {
              case OperationCanceledException _:
                return;
              case ObjectDisposedException _:
                return;
            }
          }
          DefaultTrace.TraceWarning("BackgroundTokenRefreshLoop() - Unable to refresh token credential cache. Exception: {0}", (object) ex.ToString());
          if (!this.userDefinedBackgroundTokenCredentialRefreshInterval.HasValue)
          {
            if (this.cachedAccessToken.HasValue)
            {
              AccessToken accessToken = this.cachedAccessToken.Value;
              this.systemBackgroundTokenCredentialRefreshInterval = new TimeSpan?(TimeSpan.FromSeconds((((AccessToken) ref accessToken).ExpiresOn - DateTimeOffset.UtcNow).TotalSeconds * TokenCredentialCache.DefaultBackgroundTokenCredentialRefreshIntervalPercentage));
              nullable = this.systemBackgroundTokenCredentialRefreshInterval;
              TimeSpan backgroundRefreshInterval = TokenCredentialCache.MinimumTimeBetweenBackgroundRefreshInterval;
              if ((nullable.HasValue ? (nullable.GetValueOrDefault() < backgroundRefreshInterval ? 1 : 0) : 0) != 0)
              {
                lock (this.backgroundRefreshLock)
                {
                  this.isBackgroundTaskRunning = false;
                  break;
                }
              }
            }
          }
        }
      }
    }
  }
}
