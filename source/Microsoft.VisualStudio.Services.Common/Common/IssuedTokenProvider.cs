// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.IssuedTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class IssuedTokenProvider
  {
    private const double c_slowTokenAcquisitionTimeInSeconds = 2.0;
    private object m_thisLock;
    private List<IssuedTokenProvider.GetTokenOperation> m_operations;

    protected IssuedTokenProvider(IssuedTokenCredential credential, Uri serverUrl, Uri signInUrl)
    {
      ArgumentUtility.CheckForNull<IssuedTokenCredential>(credential, nameof (credential));
      this.SignInUrl = signInUrl;
      this.Credential = credential;
      this.ServerUrl = serverUrl;
      this.m_thisLock = new object();
    }

    protected virtual string AuthenticationScheme => string.Empty;

    protected virtual string AuthenticationParameter => string.Empty;

    protected internal IssuedTokenCredential Credential { get; }

    internal VssCredentialsType CredentialType => this.Credential.CredentialType;

    public IssuedToken CurrentToken { get; internal set; }

    public abstract bool GetTokenIsInteractive { get; }

    private bool InvokeRequired => this.GetTokenIsInteractive && this.Credential.Scheduler != null;

    public Uri SignInUrl { get; private set; }

    protected Uri ServerUrl { get; }

    internal Uri TokenStorageUrl { get; set; }

    protected internal virtual bool IsAuthenticationChallenge(IHttpResponse webResponse) => this.Credential.IsAuthenticationChallenge(webResponse);

    internal string GetAuthenticationParameters() => string.IsNullOrEmpty(this.AuthenticationParameter) ? this.AuthenticationScheme : string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.AuthenticationScheme, (object) this.AuthenticationParameter);

    internal void ValidateToken(IssuedToken token, IHttpResponse webResponse)
    {
      if (token == null)
        return;
      lock (this.m_thisLock)
      {
        IssuedToken token1 = this.OnValidatingToken(token, webResponse);
        if (token1.IsAuthenticated)
          return;
        try
        {
          token1.GetUserData(webResponse);
          this.OnTokenValidated(token1);
          token1.Authenticated();
        }
        finally
        {
          if (this.CurrentToken == token)
            this.CurrentToken = token1.IsAuthenticated ? token1 : (IssuedToken) null;
        }
      }
    }

    internal void InvalidateToken(IssuedToken token)
    {
      bool flag = false;
      lock (this.m_thisLock)
      {
        if (token != null)
        {
          if (this.CurrentToken == token)
          {
            this.CurrentToken = (IssuedToken) null;
            flag = true;
          }
        }
      }
      if (!flag)
        return;
      this.OnTokenInvalidated(token);
    }

    public async Task<IssuedToken> GetTokenAsync(
      IssuedToken failedToken,
      CancellationToken cancellationToken)
    {
      IssuedTokenProvider provider = this;
      IssuedToken currentToken = provider.CurrentToken;
      VssTraceActivity traceActivity = VssTraceActivity.Current;
      Stopwatch aadAuthTokenTimer = Stopwatch.StartNew();
      try
      {
        VssHttpEventSource.Log.AuthenticationStart(traceActivity);
        if (currentToken != null)
        {
          VssHttpEventSource.Log.IssuedTokenRetrievedFromCache(traceActivity, provider, currentToken);
          return currentToken;
        }
        IssuedTokenProvider.GetTokenOperation operation = (IssuedTokenProvider.GetTokenOperation) null;
        try
        {
          IssuedTokenProvider.GetTokenOperation operationInProgress;
          operation = provider.CreateOperation(traceActivity, failedToken, cancellationToken, out operationInProgress);
          return operationInProgress == null ? await operation.GetTokenAsync(traceActivity).ConfigureAwait(false) : await operationInProgress.WaitForTokenAsync(traceActivity, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
          lock (provider.m_thisLock)
            provider.m_operations.Remove(operation);
          operation?.Dispose();
        }
      }
      finally
      {
        VssHttpEventSource.Log.AuthenticationStop(traceActivity);
        aadAuthTokenTimer.Stop();
        TimeSpan elapsed = aadAuthTokenTimer.Elapsed;
        if (elapsed.TotalSeconds >= 2.0)
          VssHttpEventSource.Log.AuthorizationDelayed(elapsed.TotalSeconds.ToString());
      }
    }

    protected virtual Task<IssuedToken> OnGetTokenAsync(
      IssuedToken failedToken,
      CancellationToken cancellationToken)
    {
      return this.Credential.Prompt != null ? this.Credential.Prompt.GetTokenAsync(this, failedToken) : Task.FromResult<IssuedToken>((IssuedToken) null);
    }

    protected virtual IssuedToken OnValidatingToken(IssuedToken token, IHttpResponse webResponse) => token;

    protected virtual void OnTokenValidated(IssuedToken token)
    {
      if (!token.FromStorage && this.TokenStorageUrl != (Uri) null)
        this.Credential.Storage?.StoreToken(this.TokenStorageUrl, token);
      VssHttpEventSource.Log.IssuedTokenValidated(VssTraceActivity.Current, this, token);
    }

    protected virtual void OnTokenInvalidated(IssuedToken token)
    {
      if (this.Credential.Storage != null && this.TokenStorageUrl != (Uri) null)
        this.Credential.Storage.RemoveTokenValue(this.TokenStorageUrl, token);
      VssHttpEventSource.Log.IssuedTokenInvalidated(VssTraceActivity.Current, this, token);
    }

    private IssuedTokenProvider.GetTokenOperation CreateOperation(
      VssTraceActivity traceActivity,
      IssuedToken failedToken,
      CancellationToken cancellationToken,
      out IssuedTokenProvider.GetTokenOperation operationInProgress)
    {
      operationInProgress = (IssuedTokenProvider.GetTokenOperation) null;
      IssuedTokenProvider.GetTokenOperation operation = (IssuedTokenProvider.GetTokenOperation) null;
      lock (this.m_thisLock)
      {
        if (this.m_operations == null)
          this.m_operations = new List<IssuedTokenProvider.GetTokenOperation>();
        if (this.m_operations.Count > 0)
        {
          operationInProgress = this.m_operations[0];
          operation = new IssuedTokenProvider.GetTokenOperation(traceActivity, this, failedToken, cancellationToken, operationInProgress.CompletionSource);
        }
        else
          operation = new IssuedTokenProvider.GetTokenOperation(traceActivity, this, failedToken, cancellationToken);
        this.m_operations.Add(operation);
      }
      return operation;
    }

    private class DisposableTaskCompletionSource<T> : TaskCompletionSource<T>, IDisposable
    {
      private bool m_disposed;
      private bool m_completed;

      public DisposableTaskCompletionSource() => this.Task.ConfigureAwait(false).GetAwaiter().OnCompleted((Action) (() => this.m_completed = true));

      ~DisposableTaskCompletionSource() => this.TraceErrorIfNotCompleted();

      public void Dispose()
      {
        if (this.m_disposed)
          return;
        this.TraceErrorIfNotCompleted();
        this.m_disposed = true;
        GC.SuppressFinalize((object) this);
      }

      private void TraceErrorIfNotCompleted()
      {
        if (this.m_completed)
          return;
        VssHttpEventSource.Log.TokenSourceNotCompleted();
      }
    }

    private sealed class GetTokenOperation : IDisposable
    {
      public GetTokenOperation(
        VssTraceActivity activity,
        IssuedTokenProvider provider,
        IssuedToken failedToken,
        CancellationToken cancellationToken)
        : this(activity, provider, failedToken, cancellationToken, new IssuedTokenProvider.DisposableTaskCompletionSource<IssuedToken>(), true)
      {
      }

      public GetTokenOperation(
        VssTraceActivity activity,
        IssuedTokenProvider provider,
        IssuedToken failedToken,
        CancellationToken cancellationToken,
        IssuedTokenProvider.DisposableTaskCompletionSource<IssuedToken> completionSource,
        bool ownsCompletionSource = false)
      {
        this.Provider = provider;
        this.ActivityId = activity != null ? activity.Id : Guid.Empty;
        this.FailedToken = failedToken;
        this.CancellationToken = cancellationToken;
        this.CompletionSource = completionSource;
        this.OwnsCompletionSource = ownsCompletionSource;
      }

      public Guid ActivityId { get; }

      public CancellationToken CancellationToken { get; }

      public IssuedTokenProvider.DisposableTaskCompletionSource<IssuedToken> CompletionSource { get; }

      public bool OwnsCompletionSource { get; }

      private IssuedToken FailedToken { get; }

      private IssuedTokenProvider Provider { get; }

      public void Dispose()
      {
        if (!this.OwnsCompletionSource)
          return;
        this.CompletionSource?.Dispose();
      }

      public async Task<IssuedToken> GetTokenAsync(VssTraceActivity traceActivity)
      {
        IssuedTokenProvider.GetTokenOperation state1 = this;
        IssuedToken token = (IssuedToken) null;
        IssuedToken tokenAsync;
        try
        {
          VssHttpEventSource.Log.IssuedTokenAcquiring(traceActivity, state1.Provider);
          if (state1.Provider.InvokeRequired)
          {
            TaskCompletionSource<object> timerTask = new TaskCompletionSource<object>();
            CancellationTokenSource timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3.0));
            timeoutTokenSource.Token.Register((Action) (() => timerTask.SetResult((object) null)), false);
            Task<IssuedToken> uiTask = Task.Factory.StartNew<Task<IssuedToken>>((Func<object, Task<IssuedToken>>) (state => IssuedTokenProvider.GetTokenOperation.PostCallback(state, timeoutTokenSource)), (object) state1, state1.CancellationToken, TaskCreationOptions.None, state1.Provider.Credential.Scheduler).Unwrap<IssuedToken>();
            if (await Task.WhenAny((Task) timerTask.Task, (Task) uiTask).ConfigureAwait(false) == uiTask)
              token = uiTask.Result;
            uiTask = (Task<IssuedToken>) null;
          }
          else
            token = await state1.Provider.OnGetTokenAsync(state1.FailedToken, state1.CancellationToken).ConfigureAwait(false);
          state1.CompletionSource.TrySetResult(token);
          tokenAsync = token;
        }
        catch (Exception ex)
        {
          state1.CompletionSource.TrySetException(ex);
          throw;
        }
        finally
        {
          state1.Provider.CurrentToken = token ?? state1.FailedToken;
          VssHttpEventSource.Log.IssuedTokenAcquired(traceActivity, state1.Provider, token);
        }
        token = (IssuedToken) null;
        return tokenAsync;
      }

      public async Task<IssuedToken> WaitForTokenAsync(
        VssTraceActivity traceActivity,
        CancellationToken cancellationToken)
      {
        IssuedTokenProvider.GetTokenOperation getTokenOperation = this;
        IssuedToken token = (IssuedToken) null;
        try
        {
          VssHttpEventSource.Log.IssuedTokenWaitStart(traceActivity, getTokenOperation.Provider, getTokenOperation.ActivityId);
          // ISSUE: reference to a compiler-generated method
          token = await Task.Factory.ContinueWhenAll<IssuedToken>(new Task[1]
          {
            (Task) getTokenOperation.CompletionSource.Task
          }, new Func<Task[], IssuedToken>(getTokenOperation.\u003CWaitForTokenAsync\u003Eb__22_0), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
          VssHttpEventSource.Log.IssuedTokenWaitStop(traceActivity, getTokenOperation.Provider, token);
        }
        IssuedToken issuedToken = token;
        token = (IssuedToken) null;
        return issuedToken;
      }

      private static Task<IssuedToken> PostCallback(
        object state,
        CancellationTokenSource timeoutTokenSource)
      {
        using (timeoutTokenSource)
        {
          timeoutTokenSource.CancelAfter(-1);
          if (timeoutTokenSource.IsCancellationRequested)
            return Task.FromResult<IssuedToken>((IssuedToken) null);
        }
        IssuedTokenProvider.GetTokenOperation getTokenOperation = (IssuedTokenProvider.GetTokenOperation) state;
        return getTokenOperation.Provider.OnGetTokenAsync(getTokenOperation.FailedToken, getTokenOperation.CancellationToken);
      }
    }
  }
}
