// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.OperationContext
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage
{
  public sealed class OperationContext
  {
    private IList<RequestResult> requestResults = (IList<RequestResult>) new List<RequestResult>();

    static OperationContext() => OperationContext.DefaultLogLevel = LogLevel.Verbose;

    public OperationContext()
    {
      this.ClientRequestID = Guid.NewGuid().ToString();
      this.LogLevel = OperationContext.DefaultLogLevel;
    }

    public IDictionary<string, string> UserHeaders { get; set; }

    public string ClientRequestID { get; set; }

    public string CustomUserAgent { get; set; }

    public static LogLevel DefaultLogLevel { get; set; }

    public LogLevel LogLevel { get; set; }

    public static event EventHandler<RequestEventArgs> GlobalSendingRequest;

    public static event EventHandler<RequestEventArgs> GlobalResponseReceived;

    public static event EventHandler<RequestEventArgs> GlobalRequestCompleted;

    public static event EventHandler<RequestEventArgs> GlobalRetrying;

    public event EventHandler<RequestEventArgs> SendingRequest;

    public event EventHandler<RequestEventArgs> ResponseReceived;

    public event EventHandler<RequestEventArgs> RequestCompleted;

    public event EventHandler<RequestEventArgs> Retrying;

    internal void FireSendingRequest(RequestEventArgs args)
    {
      EventHandler<RequestEventArgs> sendingRequest = this.SendingRequest;
      if (sendingRequest != null)
        sendingRequest((object) this, args);
      EventHandler<RequestEventArgs> globalSendingRequest = OperationContext.GlobalSendingRequest;
      if (globalSendingRequest == null)
        return;
      globalSendingRequest((object) this, args);
    }

    internal void FireResponseReceived(RequestEventArgs args)
    {
      EventHandler<RequestEventArgs> responseReceived1 = this.ResponseReceived;
      if (responseReceived1 != null)
        responseReceived1((object) this, args);
      EventHandler<RequestEventArgs> responseReceived2 = OperationContext.GlobalResponseReceived;
      if (responseReceived2 == null)
        return;
      responseReceived2((object) this, args);
    }

    internal void FireRequestCompleted(RequestEventArgs args)
    {
      EventHandler<RequestEventArgs> requestCompleted1 = this.RequestCompleted;
      if (requestCompleted1 != null)
        requestCompleted1((object) this, args);
      EventHandler<RequestEventArgs> requestCompleted2 = OperationContext.GlobalRequestCompleted;
      if (requestCompleted2 == null)
        return;
      requestCompleted2((object) this, args);
    }

    internal void FireRetrying(RequestEventArgs args)
    {
      EventHandler<RequestEventArgs> retrying = this.Retrying;
      if (retrying != null)
        retrying((object) this, args);
      EventHandler<RequestEventArgs> globalRetrying = OperationContext.GlobalRetrying;
      if (globalRetrying == null)
        return;
      globalRetrying((object) this, args);
    }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public IList<RequestResult> RequestResults => this.requestResults;

    public RequestResult LastResult => this.requestResults == null || this.requestResults.Count == 0 ? (RequestResult) null : this.requestResults[this.requestResults.Count - 1];
  }
}
