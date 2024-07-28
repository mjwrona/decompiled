// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssRequestTimerTrace
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal class VssRequestTimerTrace
  {
    private Stopwatch _requestTimer;
    private Guid _activityId;

    internal VssRequestTimerTrace() => this._requestTimer = new Stopwatch();

    internal void TraceRequest(HttpRequestMessage message)
    {
      string requestString = message.GetRequestString();
      VssPerformanceEventSource.Log.RESTStart(Guid.Empty, requestString);
      this._requestTimer.Start();
      int num = (int) VssRequestTimerTrace.EventActivityIdControl(1, ref this._activityId);
    }

    internal void TraceResponse(HttpResponseMessage response)
    {
      this._requestTimer.Stop();
      string responseString = response.GetResponseString(this._requestTimer.ElapsedMilliseconds);
      VssPerformanceEventSource.Log.RESTStop(Guid.Empty, this._activityId, responseString, this._requestTimer.ElapsedMilliseconds);
    }

    [DllImport("ADVAPI32.DLL")]
    internal static extern uint EventActivityIdControl([In] int ControlCode, [In, Out] ref Guid ActivityId);
  }
}
