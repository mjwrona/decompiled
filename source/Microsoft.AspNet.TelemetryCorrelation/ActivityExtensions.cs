// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.TelemetryCorrelation.ActivityExtensions
// Assembly: Microsoft.AspNet.TelemetryCorrelation, Version=1.0.8.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7ACB3991-3C84-47CC-A6F7-137F032D1A74
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.TelemetryCorrelation.dll

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.AspNet.TelemetryCorrelation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ActivityExtensions
  {
    internal const string RequestIdHeaderName = "Request-Id";
    internal const string TraceparentHeaderName = "traceparent";
    internal const string TracestateHeaderName = "tracestate";
    internal const string CorrelationContextHeaderName = "Correlation-Context";
    internal const int MaxCorrelationContextLength = 1024;

    public static bool Extract(this Activity activity, NameValueCollection requestHeaders)
    {
      if (activity == null)
      {
        AspNetTelemetryCorrelationEventSource.Log.ActvityExtractionError("activity is null");
        return false;
      }
      if (activity.ParentId != null)
      {
        AspNetTelemetryCorrelationEventSource.Log.ActvityExtractionError("ParentId is already set on activity");
        return false;
      }
      if (activity.Id != null)
      {
        AspNetTelemetryCorrelationEventSource.Log.ActvityExtractionError("Activity is already started");
        return false;
      }
      string[] values1 = requestHeaders.GetValues("traceparent");
      if (values1 == null || values1.Length == 0)
        values1 = requestHeaders.GetValues("Request-Id");
      if (values1 == null || values1.Length == 0 || string.IsNullOrEmpty(values1[0]))
        return false;
      activity.SetParentId(values1[0]);
      string[] values2 = requestHeaders.GetValues("tracestate");
      if (values2 != null && values2.Length != 0)
        activity.TraceStateString = values2.Length != 1 || string.IsNullOrEmpty(values2[0]) ? string.Join(",", values2) : values2[0];
      string[] values3 = requestHeaders.GetValues("Correlation-Context");
      if (values3 != null)
      {
        int num = -1;
        foreach (string str1 in values3)
        {
          if (num < 1024)
          {
            string str2 = str1;
            char[] chArray = new char[1]{ ',' };
            foreach (string str3 in str2.Split(chArray))
            {
              num += str3.Length + 1;
              if (num < 1024)
              {
                NameValueHeaderValue parsedValue;
                if (NameValueHeaderValue.TryParse(str3, out parsedValue))
                  activity.AddBaggage(parsedValue.Name, parsedValue.Value);
                else
                  AspNetTelemetryCorrelationEventSource.Log.HeaderParsingError("Correlation-Context", str3);
              }
              else
                break;
            }
          }
          else
            break;
        }
      }
      return true;
    }

    [Obsolete("Method is obsolete, use Extract method instead", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool TryParse(this Activity activity, NameValueCollection requestHeaders) => activity.Extract(requestHeaders);
  }
}
