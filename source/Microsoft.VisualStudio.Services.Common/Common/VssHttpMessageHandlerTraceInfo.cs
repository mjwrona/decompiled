// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssHttpMessageHandlerTraceInfo
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Common
{
  public class VssHttpMessageHandlerTraceInfo
  {
    private DateTime _lastTime;
    private static readonly string TfsTraceInfoKey = "TFS_TraceInfo";

    public int TokenRetries { get; internal set; }

    public TimeSpan HandlerStartTime { get; private set; }

    public TimeSpan BufferedRequestTime { get; private set; }

    public TimeSpan RequestSendTime { get; private set; }

    public TimeSpan ResponseContentTime { get; private set; }

    public TimeSpan GetTokenTime { get; private set; }

    public TimeSpan TrailingTime { get; private set; }

    public VssHttpMessageHandlerTraceInfo() => this._lastTime = DateTime.UtcNow;

    internal void TraceHandlerStartTime()
    {
      DateTime lastTime = this._lastTime;
      this._lastTime = DateTime.UtcNow;
      this.HandlerStartTime += this._lastTime - lastTime;
    }

    internal void TraceBufferedRequestTime()
    {
      DateTime lastTime = this._lastTime;
      this._lastTime = DateTime.UtcNow;
      this.BufferedRequestTime += this._lastTime - lastTime;
    }

    internal void TraceRequestSendTime()
    {
      DateTime lastTime = this._lastTime;
      this._lastTime = DateTime.UtcNow;
      this.RequestSendTime += this._lastTime - lastTime;
    }

    internal void TraceResponseContentTime()
    {
      DateTime lastTime = this._lastTime;
      this._lastTime = DateTime.UtcNow;
      this.ResponseContentTime += this._lastTime - lastTime;
    }

    internal void TraceGetTokenTime()
    {
      DateTime lastTime = this._lastTime;
      this._lastTime = DateTime.UtcNow;
      this.GetTokenTime += this._lastTime - lastTime;
    }

    internal void TraceTrailingTime()
    {
      DateTime lastTime = this._lastTime;
      this._lastTime = DateTime.UtcNow;
      this.TrailingTime += this._lastTime - lastTime;
    }

    public static void SetTraceInfo(
      HttpRequestMessage message,
      VssHttpMessageHandlerTraceInfo traceInfo)
    {
      if (message.Properties.TryGetValue(VssHttpMessageHandlerTraceInfo.TfsTraceInfoKey, out object _))
        return;
      message.Properties.Add(VssHttpMessageHandlerTraceInfo.TfsTraceInfoKey, (object) traceInfo);
    }

    public static VssHttpMessageHandlerTraceInfo GetTraceInfo(HttpRequestMessage message)
    {
      VssHttpMessageHandlerTraceInfo traceInfo = (VssHttpMessageHandlerTraceInfo) null;
      object obj;
      if (message.Properties.TryGetValue(VssHttpMessageHandlerTraceInfo.TfsTraceInfoKey, out obj))
        traceInfo = obj as VssHttpMessageHandlerTraceInfo;
      return traceInfo;
    }

    public override string ToString() => string.Format("R:{0}, HS:{1}, BR:{2}, RS:{3}, RC:{4}, GT:{5}, TT={6}", (object) this.TokenRetries, (object) this.HandlerStartTime.Ticks, (object) this.BufferedRequestTime.Ticks, (object) this.RequestSendTime.Ticks, (object) this.ResponseContentTime.Ticks, (object) this.GetTokenTime.Ticks, (object) this.TrailingTime.Ticks);
  }
}
