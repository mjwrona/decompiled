// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsHttpRetryChannel
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.TeamFoundation.Client.Channels
{
  internal sealed class TfsHttpRetryChannel : ITfsRequestChannel
  {
    private ManualResetEvent m_aborted;
    private ITfsRequestChannel m_innerChannel;
    private TfsHttpRetryChannelFactory m_factory;
    private static TimeSpan m_maxSleepDuration = TimeSpan.FromSeconds(30.0);

    public TfsHttpRetryChannel(TfsHttpRetryChannelFactory factory, ITfsRequestChannel innerChannel)
    {
      this.m_factory = factory;
      this.m_innerChannel = innerChannel;
      this.m_aborted = new ManualResetEvent(false);
    }

    private ITfsRequestChannel InnerChannel => this.m_innerChannel;

    public Uri Uri => this.m_innerChannel.Uri;

    public VssCredentials Credentials => this.m_innerChannel.Credentials;

    public CultureInfo Culture => this.m_innerChannel.Culture;

    public Guid SessionId => this.m_innerChannel.SessionId;

    public TfsRequestSettings Settings => this.m_innerChannel.Settings;

    public TfsHttpClientState State => this.m_innerChannel.State;

    public void Abort()
    {
      if (this.m_aborted != null)
        this.m_aborted.Set();
      this.m_innerChannel.Abort();
    }

    public IAsyncResult BeginRequest(TfsMessage message, AsyncCallback callback, object state) => this.BeginRequest(message, this.Settings.SendTimeout, callback, state);

    public IAsyncResult BeginRequest(
      TfsMessage message,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new TfsHttpRetryChannel.RequestOperation(this, message, timeout, callback, state);
    }

    public TfsMessage EndRequest(IAsyncResult result) => TfsHttpRetryChannel.RequestOperation.End(result);

    public TfsMessage Request(TfsMessage message) => this.Request(message, this.Settings.SendTimeout);

    public TfsMessage Request(TfsMessage message, TimeSpan timeout)
    {
      TimeSpan timeout1 = TimeSpan.FromSeconds(0.0);
      Microsoft.TeamFoundation.Framework.Common.TimeoutHelper timeoutHelper = new Microsoft.TeamFoundation.Framework.Common.TimeoutHelper(this.m_factory.MaxTimeout);
      Exception exception1;
      TfsMessage tfsMessage;
      do
      {
        Exception exception2;
        TimeSpan timeSpan;
        do
        {
          exception1 = (Exception) null;
          if (timeoutHelper.RemainingTime().TotalMilliseconds == 0.0)
            throw new TimeoutException(ClientResources.HttpRequestTimeout((object) timeoutHelper.Value));
          try
          {
            tfsMessage = this.m_innerChannel.Request(message, timeout);
            goto label_16;
          }
          catch (Exception ex)
          {
            if (!this.CanRetryException(ex))
              throw;
            else
              exception2 = ex;
          }
          timeSpan = timeoutHelper.RemainingTime();
        }
        while (exception2 == null || timeSpan.TotalMilliseconds <= 0.0);
        if (exception2 is AzureServiceBusyException)
        {
          timeout1 = TfsHttpRetryChannel.m_maxSleepDuration;
        }
        else
        {
          timeout1 = timeout1.Add(TimeSpan.FromSeconds(1.0));
          if (timeout1 > TfsHttpRetryChannel.m_maxSleepDuration)
            timeout1 = TfsHttpRetryChannel.m_maxSleepDuration;
        }
        if (timeout1 > timeSpan)
          timeout1 = timeSpan;
      }
      while (!this.m_aborted.WaitOne(timeout1, false));
      throw new System.OperationCanceledException(ClientResources.CommandCanceled());
label_16:
      if (exception1 != null)
        throw exception1;
      return tfsMessage;
    }

    private bool CanRetryException(Exception ex)
    {
      if (!this.m_factory.Enabled)
        return false;
      switch (ex)
      {
        case TeamFoundationServerInvalidResponseException _:
        case TeamFoundationServiceUnavailableException _:
          return VssNetworkHelper.IsTransientNetworkException(ex);
        case AzureServerUnavailableException _:
        case AzureServiceBusyException _:
        case AzureSessionTerminatedException _:
        case DatabaseConnectionException _:
          return true;
        default:
          return ex is DatabaseOperationTimeoutException;
      }
    }

    private sealed class RequestOperation : AsyncOperation
    {
      private TimeSpan m_timeout;
      private TfsMessage m_request;
      private TfsMessage m_response;
      private TfsHttpRetryChannel m_channel;
      private Microsoft.TeamFoundation.Framework.Common.TimeoutHelper m_timeoutHelper;
      private TimeSpan m_currentSleepDuration = TimeSpan.Zero;

      public RequestOperation(
        TfsHttpRetryChannel channel,
        TfsMessage request,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.m_channel = channel;
        this.m_request = request;
        this.m_timeout = timeout;
        this.m_timeoutHelper = new Microsoft.TeamFoundation.Framework.Common.TimeoutHelper(channel.m_factory.MaxTimeout);
        this.Begin(true);
      }

      public void Begin(bool synchronous)
      {
        if (this.m_timeoutHelper.RemainingTime().TotalMilliseconds == 0.0)
          throw new TimeoutException(ClientResources.HttpRequestTimeout((object) this.m_timeoutHelper.Value));
        IAsyncResult result = this.m_channel.InnerChannel.BeginRequest(this.m_request, this.m_timeout, new AsyncCallback(TfsHttpRetryChannel.RequestOperation.EndSendRequest), (object) this);
        if (!result.CompletedSynchronously || !this.CompleteSendRequest(result))
          return;
        this.Complete(true);
      }

      private static void EndSendRequest(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        TfsHttpRetryChannel.RequestOperation asyncState = (TfsHttpRetryChannel.RequestOperation) result.AsyncState;
        bool flag = true;
        Exception exception = (Exception) null;
        try
        {
          flag = asyncState.CompleteSendRequest(result);
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        if (!flag)
          return;
        asyncState.Complete(false, exception);
      }

      private bool CompleteSendRequest(IAsyncResult result)
      {
        Exception exception = (Exception) null;
        TfsMessage tfsMessage = (TfsMessage) null;
        try
        {
          tfsMessage = this.m_channel.InnerChannel.EndRequest(result);
        }
        catch (Exception ex)
        {
          if (!this.m_channel.CanRetryException(ex))
            throw;
          else
            exception = ex;
        }
        TimeSpan timeSpan = this.m_timeoutHelper.RemainingTime();
        if (exception != null && timeSpan.TotalMilliseconds > 0.0)
        {
          if (exception is AzureServiceBusyException)
            this.m_currentSleepDuration = TfsHttpRetryChannel.m_maxSleepDuration;
          else if (this.m_currentSleepDuration < TfsHttpRetryChannel.m_maxSleepDuration)
            this.m_currentSleepDuration = this.m_currentSleepDuration.Add(TimeSpan.FromSeconds(1.0));
          if (this.m_currentSleepDuration > timeSpan)
            this.m_currentSleepDuration = timeSpan;
          ThreadPool.RegisterWaitForSingleObject((WaitHandle) this.m_channel.m_aborted, new WaitOrTimerCallback(TfsHttpRetryChannel.RequestOperation.OnWaitSignaled), (object) this, (int) this.m_currentSleepDuration.TotalMilliseconds, true);
          return false;
        }
        if (exception != null)
          throw exception;
        this.m_response = tfsMessage;
        return true;
      }

      private static void OnWaitSignaled(object state, bool timedOut)
      {
        TfsHttpRetryChannel.RequestOperation requestOperation = (TfsHttpRetryChannel.RequestOperation) state;
        if (timedOut)
        {
          try
          {
            requestOperation.Begin(false);
          }
          catch (Exception ex)
          {
            requestOperation.Complete(false, ex);
          }
        }
        else
          requestOperation.Complete(false, (Exception) new System.OperationCanceledException(ClientResources.CommandCanceled()));
      }

      public static TfsMessage End(IAsyncResult result) => AsyncOperation.End<TfsHttpRetryChannel.RequestOperation>(result).m_response;
    }
  }
}
