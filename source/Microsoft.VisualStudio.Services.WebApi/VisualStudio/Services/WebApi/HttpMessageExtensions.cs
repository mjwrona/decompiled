// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.HttpMessageExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Net.Http;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class HttpMessageExtensions
  {
    private const string tracerKey = "VSS_HTTP_TIMER_TRACE";

    internal static void Trace(this HttpRequestMessage request)
    {
      object obj = (object) null;
      VssRequestTimerTrace requestTimerTrace;
      if (request.Properties.TryGetValue("VSS_HTTP_TIMER_TRACE", out obj))
      {
        requestTimerTrace = obj as VssRequestTimerTrace;
      }
      else
      {
        requestTimerTrace = new VssRequestTimerTrace();
        request.Properties["VSS_HTTP_TIMER_TRACE"] = (object) requestTimerTrace;
      }
      requestTimerTrace?.TraceRequest(request);
    }

    internal static void Trace(this HttpResponseMessage response)
    {
      object obj = (object) null;
      VssRequestTimerTrace requestTimerTrace = (VssRequestTimerTrace) null;
      if (response.RequestMessage.Properties.TryGetValue("VSS_HTTP_TIMER_TRACE", out obj))
        requestTimerTrace = obj as VssRequestTimerTrace;
      requestTimerTrace?.TraceResponse(response);
    }
  }
}
