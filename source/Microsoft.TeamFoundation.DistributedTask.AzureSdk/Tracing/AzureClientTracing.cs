// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Tracing.AzureClientTracing
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Tracing
{
  public static class AzureClientTracing
  {
    private static readonly List<IAzureClientTracingInterceptor> s_interceptors = new List<IAzureClientTracingInterceptor>();
    private static List<IAzureClientTracingInterceptor> s_threadSafeInterceptors = new List<IAzureClientTracingInterceptor>();
    private static readonly object s_lock = new object();
    private static long s_nextInvocationId = 0;

    public static bool IsEnabled { get; set; } = false;

    internal static IEnumerable<IAzureClientTracingInterceptor> TracingInterceptors => (IEnumerable<IAzureClientTracingInterceptor>) AzureClientTracing.s_threadSafeInterceptors;

    public static string NextInvocationId => Interlocked.Increment(ref AzureClientTracing.s_nextInvocationId).ToString();

    public static void AddTracingInterceptor(IAzureClientTracingInterceptor interceptor)
    {
      if (interceptor == null)
        throw new ArgumentNullException(nameof (interceptor));
      lock (AzureClientTracing.s_lock)
      {
        if (AzureClientTracing.s_interceptors.Contains(interceptor))
          return;
        AzureClientTracing.s_interceptors.Add(interceptor);
        AzureClientTracing.s_threadSafeInterceptors = new List<IAzureClientTracingInterceptor>((IEnumerable<IAzureClientTracingInterceptor>) AzureClientTracing.s_interceptors);
      }
    }

    public static bool RemoveTracingInterceptor(IAzureClientTracingInterceptor interceptor)
    {
      if (interceptor == null)
        throw new ArgumentNullException(nameof (interceptor));
      lock (AzureClientTracing.s_lock)
      {
        int num = AzureClientTracing.s_interceptors.Remove(interceptor) ? 1 : 0;
        if (num != 0)
          AzureClientTracing.s_threadSafeInterceptors = new List<IAzureClientTracingInterceptor>((IEnumerable<IAzureClientTracingInterceptor>) AzureClientTracing.s_interceptors);
        return num != 0;
      }
    }

    public static bool AnyActiveInterceptors()
    {
      lock (AzureClientTracing.s_lock)
        return AzureClientTracing.s_threadSafeInterceptors.Count > 0;
    }

    public static void Configuration(Guid activityId, string source, string name, string value)
    {
      if (!AzureClientTracing.IsEnabled)
        return;
      using (IEnumerator<IAzureClientTracingInterceptor> enumerator = AzureClientTracing.TracingInterceptors.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        enumerator.Current.Configuration(activityId, source, name, value);
      }
    }

    public static void Information(
      Guid activityId,
      string invocationId,
      string message,
      params object[] parameters)
    {
      if (!AzureClientTracing.IsEnabled)
        return;
      AzureClientTracing.Information(activityId, invocationId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, parameters));
    }

    public static void Information(Guid activityId, string invocationId, string message)
    {
      if (!AzureClientTracing.IsEnabled)
        return;
      using (IEnumerator<IAzureClientTracingInterceptor> enumerator = AzureClientTracing.TracingInterceptors.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        enumerator.Current.Information(activityId, invocationId, message);
      }
    }

    public static void Enter(
      Guid activityId,
      string invocationId,
      object instance,
      string method,
      IDictionary<string, object> parameters)
    {
      if (!AzureClientTracing.IsEnabled)
        return;
      using (IEnumerator<IAzureClientTracingInterceptor> enumerator = AzureClientTracing.TracingInterceptors.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        enumerator.Current.EnterMethod(activityId, invocationId, instance, method, parameters);
      }
    }

    public static void SendRequest(
      Guid activityId,
      string invocationId,
      HttpRequestMessage request)
    {
      if (!AzureClientTracing.IsEnabled)
        return;
      using (IEnumerator<IAzureClientTracingInterceptor> enumerator = AzureClientTracing.TracingInterceptors.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        enumerator.Current.SendRequest(activityId, invocationId, request);
      }
    }

    public static void ReceiveResponse(
      Guid activityId,
      string invocationId,
      HttpResponseMessage response)
    {
      if (!AzureClientTracing.IsEnabled)
        return;
      using (IEnumerator<IAzureClientTracingInterceptor> enumerator = AzureClientTracing.TracingInterceptors.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        enumerator.Current.ReceiveResponse(activityId, invocationId, response);
      }
    }

    public static void Error(Guid activityId, string invocationId, Exception ex)
    {
      if (!AzureClientTracing.IsEnabled)
        return;
      using (IEnumerator<IAzureClientTracingInterceptor> enumerator = AzureClientTracing.TracingInterceptors.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        enumerator.Current.TraceError(activityId, invocationId, ex);
      }
    }

    public static void Exit(
      Guid activityId,
      string invocationId,
      object instance,
      string method,
      object result)
    {
      if (!AzureClientTracing.IsEnabled)
        return;
      using (IEnumerator<IAzureClientTracingInterceptor> enumerator = AzureClientTracing.TracingInterceptors.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        enumerator.Current.ExitMethod(activityId, invocationId, instance, method, result);
      }
    }
  }
}
