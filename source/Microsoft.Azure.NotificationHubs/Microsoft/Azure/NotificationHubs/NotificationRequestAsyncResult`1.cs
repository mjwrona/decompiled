// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NotificationRequestAsyncResult`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Microsoft.Azure.NotificationHubs
{
  internal abstract class NotificationRequestAsyncResult<TAsyncResult> : 
    IteratorAsyncResult<TAsyncResult>
    where TAsyncResult : NotificationRequestAsyncResult<TAsyncResult>
  {
    private readonly bool needRequestStream;
    private IOThreadTimer requestCancelTimer;
    private bool isRequestAborted;

    public NotificationRequestAsyncResult(
      NotificationHubManager manager,
      bool needRequestStream,
      AsyncCallback callback,
      object state)
      : base(manager.namespaceManager.Settings.InternalOperationTimeout, callback, state)
    {
      this.TrackingContext = TrackingContext.GetInstance(Guid.NewGuid(), manager.notificationHubPath);
      this.Manager = manager;
      this.needRequestStream = needRequestStream;
    }

    public NotificationHubManager Manager { get; private set; }

    public TrackingContext TrackingContext { get; private set; }

    public HttpWebRequest Request { get; set; }

    public Stream RequestStream { get; set; }

    public HttpWebResponse Response { get; set; }

    private void CancelRequest(object state)
    {
      this.isRequestAborted = true;
      try
      {
        ((WebRequest) state).Abort();
      }
      catch (Exception ex)
      {
      }
    }

    protected string GetNotificationIdFromResponse()
    {
      string responseHeader = this.Response.GetResponseHeader("Location");
      if (!string.IsNullOrEmpty(responseHeader))
      {
        Uri uri = new Uri(responseHeader.Trim('/'));
        if (uri.Segments.Length != 0)
          return uri.Segments[uri.Segments.Length - 1];
      }
      return string.Empty;
    }

    protected override IEnumerator<IteratorAsyncResult<TAsyncResult>.AsyncStep> GetAsyncSteps()
    {
      this.PrepareRequest();
      HttpWebRequest request = this.Request;
      TimeSpan originalTimeout = this.OriginalTimeout;
      int num;
      if (originalTimeout.TotalMilliseconds <= (double) int.MaxValue)
      {
        originalTimeout = this.OriginalTimeout;
        num = (int) originalTimeout.TotalMilliseconds;
      }
      else
        num = int.MaxValue;
      request.Timeout = num;
      this.Request.AddXProcessAtHeader();
      this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.Request, true);
      if (this.needRequestStream)
      {
        this.requestCancelTimer.SetIfValid(this.RemainingTime());
        yield return this.CallAsync((IteratorAsyncResult<TAsyncResult>.BeginCall) ((thisPtr, t, c, s) => thisPtr.Request.BeginGetRequestStream(c, s)), (IteratorAsyncResult<TAsyncResult>.EndCall) ((thisPtr, r) => thisPtr.RequestStream = thisPtr.Request.EndGetRequestStream(r)), IteratorAsyncResult<TAsyncResult>.ExceptionPolicy.Continue);
        if (this.LastAsyncStepException != null)
        {
          this.requestCancelTimer.Cancel();
          Exception operationException = this.LastAsyncStepException;
          if (operationException is WebException webException)
            operationException = ServiceBusResourceOperations.ConvertWebException(this.TrackingContext, webException, this.Request.Timeout);
          this.Complete(operationException);
          yield break;
        }
        else
        {
          try
          {
            this.WriteToStream();
          }
          catch (WebException ex)
          {
            this.Complete(ServiceBusResourceOperations.ConvertWebException(this.TrackingContext, ex, this.Request.Timeout));
            yield break;
          }
          catch (IOException ex)
          {
            this.Complete(ServiceBusResourceOperations.ConvertIOException(this.TrackingContext, ex, this.Request.Timeout, this.isRequestAborted));
            yield break;
          }
          finally
          {
            this.requestCancelTimer.Cancel();
          }
        }
      }
      this.requestCancelTimer.SetIfValid(this.RemainingTime());
      yield return this.CallAsync((IteratorAsyncResult<TAsyncResult>.BeginCall) ((thisPtr, t, c, s) => thisPtr.Request.BeginGetResponse(c, s)), (IteratorAsyncResult<TAsyncResult>.EndCall) ((thisPtr, r) => thisPtr.Response = (HttpWebResponse) thisPtr.Request.EndGetResponse(r)), IteratorAsyncResult<TAsyncResult>.ExceptionPolicy.Continue);
      if (this.LastAsyncStepException != null)
      {
        this.requestCancelTimer.Cancel();
        Exception operationException = this.LastAsyncStepException;
        if (this.LastAsyncStepException is WebException)
          operationException = ServiceBusResourceOperations.ConvertWebException(this.TrackingContext, (WebException) this.LastAsyncStepException, this.Request.Timeout);
        else if (this.LastAsyncStepException is IOException)
          operationException = ServiceBusResourceOperations.ConvertIOException(this.TrackingContext, (IOException) this.LastAsyncStepException, this.Request.Timeout, this.isRequestAborted);
        this.Complete(operationException);
      }
      else
      {
        try
        {
          using (this.Response)
          {
            if (this.Response.StatusCode != HttpStatusCode.Created && this.Response.StatusCode != HttpStatusCode.OK)
              this.Complete(ServiceBusResourceOperations.ConvertWebException(this.TrackingContext, new WebException(this.Response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.Response), this.Request.Timeout));
            else
              this.ProcessResponse();
          }
        }
        catch (IOException ex)
        {
          this.Complete(ServiceBusResourceOperations.ConvertIOException(this.TrackingContext, ex, this.Request.Timeout, this.isRequestAborted));
        }
        catch (Exception ex)
        {
          this.Complete(ex);
        }
        finally
        {
          this.requestCancelTimer.Cancel();
        }
      }
    }

    protected abstract void PrepareRequest();

    protected abstract void WriteToStream();

    protected abstract void ProcessResponse();
  }
}
