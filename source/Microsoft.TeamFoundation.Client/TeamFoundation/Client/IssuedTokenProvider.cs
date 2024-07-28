// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IssuedTokenProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.IssuedTokenProvider instead.", false)]
  public abstract class IssuedTokenProvider
  {
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

    protected internal IssuedTokenCredential Credential { get; private set; }

    public IssuedToken CurrentToken { get; internal set; }

    public abstract bool GetTokenIsInteractive { get; }

    private bool InvokeRequired => this.GetTokenIsInteractive && this.Credential.SyncContext != null;

    public Uri SignInUrl { get; private set; }

    protected Uri ServerUrl { get; private set; }

    private IssuedTokenProvider.GetTokenOperation CreateOperation(
      out IssuedTokenProvider.GetTokenOperation operationInProgress,
      IssuedToken failedToken,
      Microsoft.TeamFoundation.Framework.Common.TimeoutHelper timeout,
      bool canRefresh,
      AsyncCallback callback = null,
      object state = null)
    {
      operationInProgress = (IssuedTokenProvider.GetTokenOperation) null;
      bool flag = false;
      try
      {
        flag = Monitor.TryEnter(this.m_thisLock, timeout.RemainingTime());
        if (!flag)
          throw new TimeoutException();
        if (this.m_operations == null)
          this.m_operations = new List<IssuedTokenProvider.GetTokenOperation>();
        IssuedTokenProvider.GetTokenOperation operation = new IssuedTokenProvider.GetTokenOperation(this, failedToken, timeout, callback, state);
        this.m_operations.Add(operation);
        if (this.m_operations.Count > 1)
          operationInProgress = this.m_operations[0];
        return operation;
      }
      finally
      {
        if (flag)
          Monitor.Exit(this.m_thisLock);
      }
    }

    internal virtual bool IsAuthenticationChallenge(HttpWebResponse webResponse) => this.Credential.IsAuthenticationChallenge(webResponse);

    internal void ValidateToken(ref IssuedToken token, HttpWebResponse webResponse)
    {
      if (token == null)
        return;
      lock (this.m_thisLock)
      {
        TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "{0}: Validating the current token {1}", (object) this.GetType().FullName, (object) token);
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
            this.CurrentToken = token = token1.IsAuthenticated ? token1 : (IssuedToken) null;
          else
            token = token1.IsAuthenticated ? token1 : (IssuedToken) null;
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
            TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "{0}: Invalidating the current token {1}", (object) this.GetType().FullName, (object) token);
            this.CurrentToken = (IssuedToken) null;
            flag = true;
          }
        }
      }
      if (!flag)
        return;
      this.OnTokenInvalidated(token);
    }

    public IssuedToken GetToken(TimeSpan timeout) => this.GetToken((IssuedToken) null, timeout);

    public IssuedToken GetToken(IssuedToken failedToken, TimeSpan timeout) => this.GetToken(failedToken, timeout, true);

    public IssuedToken GetToken(IssuedToken failedToken, TimeSpan timeout, bool canRefresh) => this.GetToken(failedToken, new Microsoft.TeamFoundation.Framework.Common.TimeoutHelper(timeout), canRefresh);

    internal IssuedToken GetToken(IssuedToken failedToken, Microsoft.TeamFoundation.Framework.Common.TimeoutHelper timeout, bool canRefresh)
    {
      IssuedToken currentToken = this.CurrentToken;
      if (!canRefresh || currentToken != null)
        return currentToken;
      IssuedTokenProvider.GetTokenOperation operationInProgress;
      IssuedTokenProvider.GetTokenOperation operation = this.CreateOperation(out operationInProgress, failedToken, timeout, canRefresh);
      bool isOnGuiThread = false;
      bool flag = operationInProgress == null;
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.GUITHREADINFO lpgui = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.GUITHREADINFO();
      lpgui.cbSize = Marshal.SizeOf<Microsoft.TeamFoundation.Common.Internal.NativeMethods.GUITHREADINFO>(lpgui);
      if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetGUIThreadInfo((uint) Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetCurrentThreadId(), ref lpgui) && lpgui.hwndActive != IntPtr.Zero)
      {
        isOnGuiThread = true;
        flag = true;
        operationInProgress?.Join(operation);
      }
      if (!flag)
        return operationInProgress.WaitForToken(operation);
      try
      {
        return operation.GetToken(isOnGuiThread);
      }
      finally
      {
        if (operation.ProviderInvoked)
        {
          lock (this.m_thisLock)
            this.m_operations.Clear();
        }
      }
    }

    protected abstract IssuedToken OnGetToken(IssuedToken failedToken, TimeSpan timeout);

    protected virtual IssuedToken OnValidatingToken(IssuedToken token, HttpWebResponse webResponse) => token;

    protected virtual void OnTokenValidated(IssuedToken token)
    {
    }

    protected virtual void OnTokenInvalidated(IssuedToken token)
    {
    }

    public IAsyncResult BeginGetToken(TimeSpan timeout, AsyncCallback callback, object state) => this.BeginGetToken((IssuedToken) null, timeout, callback, state);

    public IAsyncResult BeginGetToken(
      IssuedToken failedToken,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetToken(failedToken, timeout, true, callback, state);
    }

    public IAsyncResult BeginGetToken(
      IssuedToken failedToken,
      TimeSpan timeout,
      bool canRefresh,
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetToken(failedToken, new Microsoft.TeamFoundation.Framework.Common.TimeoutHelper(timeout), canRefresh, callback, state);
    }

    internal IAsyncResult BeginGetToken(
      IssuedToken failedToken,
      Microsoft.TeamFoundation.Framework.Common.TimeoutHelper timeout,
      bool canRefresh,
      AsyncCallback callback,
      object state)
    {
      IssuedToken currentToken = this.CurrentToken;
      if (currentToken != null || !canRefresh)
        return (IAsyncResult) new CompletedOperation<IssuedToken>(currentToken, callback, state);
      IssuedTokenProvider.GetTokenOperation operationInProgress;
      IssuedTokenProvider.GetTokenOperation operation = this.CreateOperation(out operationInProgress, failedToken, timeout, canRefresh, callback, state);
      if (operationInProgress == null)
        operation.BeginGetToken();
      else
        operationInProgress.BeginWaitForToken(operation);
      return (IAsyncResult) operation;
    }

    protected abstract IAsyncResult OnBeginGetToken(
      IssuedToken failedToken,
      TimeSpan timeout,
      AsyncCallback callback,
      object state);

    public IssuedToken EndGetToken(IAsyncResult result)
    {
      if (result is CompletedOperation<IssuedToken>)
        return CompletedOperation<IssuedToken>.End(result);
      IssuedTokenProvider.GetTokenOperation operation;
      Exception error;
      bool flag = IssuedTokenProvider.GetTokenOperation.TryEnd(result, out operation, out error);
      if (operation.ProviderInvoked)
      {
        lock (this.m_thisLock)
          this.m_operations.Clear();
      }
      if (!flag)
        throw error;
      return operation.NewToken;
    }

    protected abstract IssuedToken OnEndGetToken(IAsyncResult result);

    private sealed class GetTokenOperation : Microsoft.TeamFoundation.Framework.Common.AsyncOperation
    {
      private object m_thisLock;
      private IssuedToken m_token;
      private Exception m_exception;
      private ManualResetEvent m_waitEvent;
      private ManualResetEvent m_sendEvent;
      private static int s_operationId;

      public GetTokenOperation(
        IssuedTokenProvider provider,
        IssuedToken failedToken,
        Microsoft.TeamFoundation.Framework.Common.TimeoutHelper timeout,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.m_thisLock = new object();
        this.FailedToken = failedToken;
        this.Provider = provider;
        this.Timeout = timeout;
        this.Id = Interlocked.Increment(ref IssuedTokenProvider.GetTokenOperation.s_operationId);
      }

      public bool Cancelled { get; private set; }

      public int Id { get; private set; }

      private IssuedToken FailedToken { get; set; }

      private IssuedTokenProvider Provider { get; set; }

      public bool ProviderInvoked { get; private set; }

      private Microsoft.TeamFoundation.Framework.Common.TimeoutHelper Timeout { get; set; }

      public IssuedToken NewToken => this.m_token;

      private IssuedTokenProvider.GetTokenOperation PreemptedBy { get; set; }

      private bool WasPreempted => this.PreemptedBy != null;

      internal void Join(IssuedTokenProvider.GetTokenOperation operation) => this.PreemptedBy = operation;

      public IssuedToken GetToken(bool isOnGuiThread)
      {
        Exception exception = (Exception) null;
        try
        {
          this.m_token = this.Provider.CurrentToken;
          if (this.m_token != null)
          {
            TeamFoundationTrace.Info(FrameworkTraceKeywordSets.TokenProvider, "Returning cached token");
          }
          else
          {
            this.ProviderInvoked = true;
            if (this.Provider.InvokeRequired)
            {
              if (!isOnGuiThread)
              {
                try
                {
                  this.m_sendEvent = new ManualResetEvent(false);
                  this.m_waitEvent = new ManualResetEvent(false);
                  this.Provider.Credential.SyncContext.Post(new SendOrPostCallback(IssuedTokenProvider.GetTokenOperation.SendCallback), (object) this);
                  if (this.m_sendEvent.WaitOne(TimeSpan.FromSeconds(3.0), true))
                  {
                    if (this.WasPreempted)
                    {
                      this.ProviderInvoked = false;
                      this.m_token = this.PreemptedBy.WaitForToken(this);
                      goto label_24;
                    }
                    else
                    {
                      this.Timeout.Suspend();
                      this.m_waitEvent.WaitOne(-1, true);
                      if (this.m_exception != null)
                        throw this.m_exception;
                      goto label_24;
                    }
                  }
                  else
                  {
                    this.Cancelled = true;
                    goto label_24;
                  }
                }
                finally
                {
                  this.Timeout.Resume();
                  lock (this.m_thisLock)
                  {
                    if (this.m_sendEvent != null)
                    {
                      this.m_sendEvent.Close();
                      this.m_sendEvent = (ManualResetEvent) null;
                    }
                    if (this.m_waitEvent != null)
                    {
                      this.m_waitEvent.Close();
                      this.m_waitEvent = (ManualResetEvent) null;
                    }
                  }
                }
              }
            }
            if (this.Provider.InvokeRequired)
            {
              try
              {
                this.Timeout.Suspend();
                this.m_token = this.Provider.OnGetToken(this.FailedToken, this.Timeout.RemainingTime());
              }
              finally
              {
                this.Timeout.Resume();
              }
            }
            else
              this.m_token = this.Provider.OnGetToken(this.FailedToken, this.Timeout.RemainingTime());
          }
label_24:
          return this.m_token;
        }
        catch (Exception ex)
        {
          exception = ex;
          throw;
        }
        finally
        {
          this.Provider.CurrentToken = this.m_token ?? this.FailedToken;
          this.Complete(true, exception);
        }
      }

      public void BeginGetToken()
      {
        try
        {
          TeamFoundationTrace.Enter(TraceKeywordSets.Authentication, TraceLevel.Verbose, "GetTokenOperation.BeginGetToken");
          this.ProviderInvoked = true;
          if (this.Provider.InvokeRequired)
          {
            TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Posting a message to the UI thread.");
            this.Provider.Credential.SyncContext.Post(new SendOrPostCallback(IssuedTokenProvider.GetTokenOperation.PostCallback), (object) this);
          }
          else
          {
            IAsyncResult token = this.Provider.OnBeginGetToken(this.FailedToken, this.Timeout.RemainingTime(), new AsyncCallback(IssuedTokenProvider.GetTokenOperation.EndGetToken), (object) this);
            if (!token.CompletedSynchronously || !this.CompleteGetToken(token))
              return;
            this.Complete(true);
          }
        }
        finally
        {
          TeamFoundationTrace.Exit(TraceKeywordSets.Authentication, TraceLevel.Verbose, "GetTokenOperation.BeginGetToken");
        }
      }

      private static void EndGetToken(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        IssuedTokenProvider.GetTokenOperation asyncState = (IssuedTokenProvider.GetTokenOperation) result.AsyncState;
        bool flag = true;
        Exception exception = (Exception) null;
        try
        {
          flag = asyncState.CompleteGetToken(result);
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        if (!flag)
          return;
        asyncState.Complete(false, exception);
      }

      private bool CompleteGetToken(IAsyncResult result)
      {
        try
        {
          this.m_token = this.Provider.OnEndGetToken(result);
          return true;
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
          throw;
        }
        finally
        {
          this.Provider.CurrentToken = this.m_token ?? this.FailedToken;
        }
      }

      public IssuedToken WaitForToken(IssuedTokenProvider.GetTokenOperation waiter)
      {
        if (!this.AsyncWaitHandle.WaitOne(waiter.Timeout.RemainingTime(), true))
          throw new TimeoutException();
        return this.m_token;
      }

      public void BeginWaitForToken(IssuedTokenProvider.GetTokenOperation waiter)
      {
        if (this.AsyncWaitHandle.WaitOne(0, true))
          waiter.Complete(true);
        else
          ThreadPool.RegisterWaitForSingleObject((WaitHandle) this.AsyncWaitHandle, new WaitOrTimerCallback(this.EndWaitForToken), (object) waiter, waiter.Timeout.RemainingTime(), true);
      }

      private void EndWaitForToken(object state, bool timedOut)
      {
        IssuedTokenProvider.GetTokenOperation getTokenOperation = (IssuedTokenProvider.GetTokenOperation) state;
        getTokenOperation.m_token = this.m_token;
        getTokenOperation.Complete(false, timedOut ? (Exception) new TimeoutException() : this.Exception);
      }

      private static void SendCallback(object state)
      {
        IssuedTokenProvider.GetTokenOperation getTokenOperation = (IssuedTokenProvider.GetTokenOperation) state;
        if (getTokenOperation.Cancelled)
          return;
        lock (getTokenOperation.m_thisLock)
        {
          if (getTokenOperation.m_sendEvent != null)
            getTokenOperation.m_sendEvent.Set();
        }
        if (getTokenOperation.WasPreempted)
          return;
        try
        {
          getTokenOperation.m_token = getTokenOperation.Provider.OnGetToken(getTokenOperation.FailedToken, getTokenOperation.Timeout.RemainingTime());
        }
        catch (Exception ex)
        {
          getTokenOperation.m_exception = ex;
        }
        finally
        {
          lock (getTokenOperation.m_thisLock)
          {
            if (getTokenOperation.m_waitEvent != null)
              getTokenOperation.m_waitEvent.Set();
          }
        }
      }

      private static void PostCallback(object state)
      {
        IssuedTokenProvider.GetTokenOperation state1 = (IssuedTokenProvider.GetTokenOperation) state;
        if (!state1.WasPreempted)
        {
          try
          {
            state1.Timeout.Suspend();
            state1.m_token = state1.Provider.OnGetToken(state1.FailedToken, state1.Timeout.RemainingTime());
          }
          catch (Exception ex)
          {
            state1.m_exception = ex;
          }
          finally
          {
            state1.Timeout.Resume();
          }
        }
        if (ThreadPool.QueueUserWorkItem(new WaitCallback(IssuedTokenProvider.GetTokenOperation.CompleteCallback), (object) state1))
          return;
        IssuedTokenProvider.GetTokenOperation.CompleteCallback((object) state1);
      }

      private static void CompleteCallback(object state)
      {
        IssuedTokenProvider.GetTokenOperation waiter = (IssuedTokenProvider.GetTokenOperation) state;
        if (waiter.WasPreempted)
        {
          waiter.ProviderInvoked = false;
          waiter.PreemptedBy.BeginWaitForToken(waiter);
        }
        else
        {
          waiter.Provider.CurrentToken = waiter.m_token ?? waiter.FailedToken;
          waiter.Complete(false, waiter.m_exception);
        }
      }

      internal static bool TryEnd(
        IAsyncResult result,
        out IssuedTokenProvider.GetTokenOperation operation,
        out Exception error)
      {
        return Microsoft.TeamFoundation.Framework.Common.AsyncOperation.TryEnd<IssuedTokenProvider.GetTokenOperation>(result, out operation, out error);
      }
    }
  }
}
