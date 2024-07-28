// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Executor.ExecutionState`1
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Microsoft.Azure.Storage.Core.Executor
{
  internal class ExecutionState<T> : StorageCommandAsyncResult
  {
    private Stream reqStream;
    private volatile Exception exceptionRef;
    private object timeoutLockerObj = new object();
    private bool reqTimedOut;
    private HttpResponseMessage resp;

    public ExecutionState(
      StorageCommandBase<T> cmd,
      IRetryPolicy policy,
      OperationContext operationContext)
    {
      this.Cmd = cmd;
      this.RetryPolicy = policy != null ? policy.CreateInstance() : (IRetryPolicy) new NoRetry();
      this.OperationContext = operationContext ?? new OperationContext();
      this.CancellationTokenSource = new CancellationTokenSource();
      this.InitializeLocation();
      if (!(this.OperationContext.StartTime == DateTime.MinValue))
        return;
      this.OperationContext.StartTime = DateTime.Now;
    }

    public ExecutionState(
      StorageCommandBase<T> cmd,
      IRetryPolicy policy,
      OperationContext operationContext,
      AsyncCallback callback,
      object asyncState)
      : base(callback, asyncState)
    {
      this.Cmd = cmd;
      this.RetryPolicy = policy != null ? policy.CreateInstance() : (IRetryPolicy) new NoRetry();
      this.OperationContext = operationContext ?? new OperationContext();
      this.InitializeLocation();
      if (!(this.OperationContext.StartTime == DateTime.MinValue))
        return;
      this.OperationContext.StartTime = DateTime.Now;
    }

    internal void Init()
    {
      this.Req = (HttpRequestMessage) null;
      this.resp = (HttpResponseMessage) null;
      this.ReqTimedOut = false;
      this.CancelDelegate = (Action) null;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        Timer backoffTimer = this.BackoffTimer;
        if (backoffTimer != null)
        {
          this.BackoffTimer = (Timer) null;
          backoffTimer.Dispose();
        }
        this.CheckDisposeSendStream();
        this.CheckDisposeAction();
      }
      base.Dispose(disposing);
    }

    internal Timer BackoffTimer { get; set; }

    internal OperationContext OperationContext { get; private set; }

    internal DateTime? OperationExpiryTime => this.Cmd.OperationExpiryTime;

    internal IRetryPolicy RetryPolicy { get; private set; }

    internal StorageCommandBase<T> Cmd { get; private set; }

    internal StorageLocation CurrentLocation { get; set; }

    internal RESTCommand<T> RestCMD => this.Cmd as RESTCommand<T>;

    internal ExecutorOperation CurrentOperation { get; set; }

    internal TimeSpan RemainingTimeout
    {
      get
      {
        if (!this.OperationExpiryTime.HasValue || this.OperationExpiryTime.Value.Equals(DateTime.MaxValue))
          return Constants.DefaultClientSideTimeout;
        TimeSpan timeSpan = this.OperationExpiryTime.Value - DateTime.Now;
        return !(timeSpan <= TimeSpan.Zero) ? timeSpan : throw Exceptions.GenerateTimeoutException(this.Cmd.CurrentResult, (Exception) null);
      }
    }

    internal int RetryCount { get; set; }

    internal Stream ReqStream
    {
      get => this.reqStream;
      set => this.reqStream = value == null ? (Stream) null : value.WrapWithByteCountingStream(this.Cmd.CurrentResult);
    }

    internal Exception ExceptionRef
    {
      get => this.exceptionRef;
      set
      {
        this.exceptionRef = value;
        if (this.Cmd == null || this.Cmd.CurrentResult == null)
          return;
        this.Cmd.CurrentResult.Exception = value;
      }
    }

    internal T Result { get; set; }

    internal bool ReqTimedOut
    {
      get
      {
        lock (this.timeoutLockerObj)
          return this.reqTimedOut;
      }
      set
      {
        lock (this.timeoutLockerObj)
          this.reqTimedOut = value;
      }
    }

    private void CheckDisposeSendStream()
    {
      RESTCommand<T> restCmd = this.RestCMD;
      if (restCmd == null || restCmd.StreamToDispose == null)
        return;
      restCmd.StreamToDispose.Dispose();
      restCmd.StreamToDispose = (Stream) null;
    }

    private void CheckDisposeAction()
    {
      RESTCommand<T> restCmd = this.RestCMD;
      if (restCmd == null || restCmd.DisposeAction == null)
        return;
      Logger.LogInformational(this.OperationContext, "Disposing action invoked.");
      try
      {
        restCmd.DisposeAction(restCmd);
      }
      catch (Exception ex)
      {
        Logger.LogWarning(this.OperationContext, "Disposing action threw an exception : {0}.", (object) ex.Message);
      }
    }

    internal CancellationTokenSource CancellationTokenSource { get; set; }

    internal HttpRequestMessage Req { get; set; }

    internal HttpResponseMessage Resp
    {
      get => this.resp;
      set
      {
        this.resp = value;
        if (this.resp == null)
          return;
        if (value.Headers != null)
        {
          this.Cmd.CurrentResult.ServiceRequestID = this.resp.Headers.GetHeaderSingleValueOrDefault("x-ms-request-id");
          RequestResult currentResult = this.Cmd.CurrentResult;
          DateTimeOffset? date = this.resp.Headers.Date;
          string str;
          if (!date.HasValue)
          {
            str = (string) null;
          }
          else
          {
            date = this.resp.Headers.Date;
            str = date.Value.UtcDateTime.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture);
          }
          currentResult.RequestDate = str;
          this.Cmd.CurrentResult.Etag = this.resp.Headers.ETag != null ? this.resp.Headers.ETag.ToString() : (string) null;
        }
        if (this.resp.Content != null && this.resp.Content.Headers != null)
          this.Cmd.CurrentResult.ContentMd5 = this.resp.Content.Headers.ContentMD5 != null ? Convert.ToBase64String(this.resp.Content.Headers.ContentMD5) : (string) null;
        if (this.resp.Content != null && this.resp.Content.Headers != null)
          this.Cmd.CurrentResult.ContentCrc64 = HttpResponseParsers.GetContentCRC64(this.resp);
        this.Cmd.CurrentResult.HttpStatusMessage = this.resp.ReasonPhrase;
        this.Cmd.CurrentResult.HttpStatusCode = (int) this.resp.StatusCode;
      }
    }

    private void InitializeLocation()
    {
      RESTCommand<T> restCmd = this.RestCMD;
      if (restCmd != null)
      {
        switch (restCmd.LocationMode)
        {
          case LocationMode.PrimaryOnly:
          case LocationMode.PrimaryThenSecondary:
            this.CurrentLocation = StorageLocation.Primary;
            break;
          case LocationMode.SecondaryOnly:
          case LocationMode.SecondaryThenPrimary:
            this.CurrentLocation = StorageLocation.Secondary;
            break;
          default:
            CommonUtility.ArgumentOutOfRange("LocationMode", (object) restCmd.LocationMode);
            break;
        }
        Logger.LogInformational(this.OperationContext, "Starting operation with location {0} per location mode {1}.", (object) this.CurrentLocation, (object) restCmd.LocationMode);
      }
      else
        this.CurrentLocation = StorageLocation.Primary;
    }
  }
}
