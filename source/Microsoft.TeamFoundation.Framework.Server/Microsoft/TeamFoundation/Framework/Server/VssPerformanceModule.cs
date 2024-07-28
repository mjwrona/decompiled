// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssPerformanceModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssPerformanceModule : IHttpModule
  {
    private static long m_counter;
    private static readonly Guid c_activityIdSource = Guid.NewGuid();
    internal const string RequestManagedStartTime = "X-VSSF-ManagedStartTime";
    internal const string RequestStartedTime = "X-VSSF-RequestStartedTime";
    internal const string RequestTimer = "X-VSSF-RequestTimer";

    public void Dispose()
    {
    }

    public void Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.Module_BeginRequest);

    private bool TryParseDateTime(string str, out DateTime val) => DateTime.TryParseExact(str, "o", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out val);

    private bool TryParseHexLong(string str, out long val) => long.TryParse(str, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out val);

    private void Module_BeginRequest(object sender, EventArgs e)
    {
      long timestamp = Stopwatch.GetTimestamp();
      HttpContext context = ((HttpApplication) sender).Context;
      DateTime val1;
      if (!this.GetHeaderValue<DateTime>(context, "X-VSSF-RequestStartedTime", new VssPerformanceModule.TryDeserialize<DateTime>(this.TryParseDateTime), out val1))
        val1 = DateTime.UtcNow;
      long val2;
      if (!this.GetHeaderValue<long>(context, "X-VSSF-RequestTimer", new VssPerformanceModule.TryDeserialize<long>(this.TryParseHexLong), out val2))
        val2 = timestamp;
      context.Items[(object) "X-VSSF-ManagedStartTime"] = (object) timestamp;
      context.Items[(object) "X-VSSF-RequestStartedTime"] = (object) val1;
      context.Items[(object) "X-VSSF-RequestTimer"] = (object) val2;
      Trace.CorrelationManager.ActivityId = this.UnsafeCreateNewActivityId();
      this.LogOriginalPath(context.Request);
    }

    private void LogOriginalPath(HttpRequest request)
    {
      Uri url;
      try
      {
        url = request.Url;
      }
      catch (UriFormatException ex)
      {
        TeamFoundationTracingService.TraceRaw(1050002, TraceLevel.Info, nameof (VssPerformanceModule), nameof (LogOriginalPath), "Could not parse URI {0}: {1}", (object) request.RawUrl, (object) ex);
        return;
      }
      request.ServerVariables["VSS_URI_STEM_ORIGINAL"] = url.AbsolutePath;
    }

    private bool GetHeaderValue<T>(
      HttpContext context,
      string headerName,
      VssPerformanceModule.TryDeserialize<T> tryDeserialize,
      out T val)
    {
      val = default (T);
      string str = context.Request.Headers.Get(headerName);
      return !string.IsNullOrEmpty(str) && tryDeserialize(str, out val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe Guid UnsafeCreateNewActivityId()
    {
      long num = Interlocked.Increment(ref VssPerformanceModule.m_counter);
      Guid activityIdSource = VssPerformanceModule.c_activityIdSource;
      Guid* guidPtr = &activityIdSource;
      *(long*) guidPtr = *(long*) guidPtr ^ num;
      return activityIdSource;
    }

    private delegate bool TryDeserialize<T>(string str, out T val);
  }
}
