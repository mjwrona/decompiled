// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.HttpRequestTraceDetails
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public static class HttpRequestTraceDetails
  {
    private static readonly HashSet<string> ResponseHeaderNamesToCapture = new HashSet<string>((IEnumerable<string>) new string[13]
    {
      "X-CID",
      "X-CCC",
      "X-MSEdge-Ref",
      "X-MS-Ref-OriginShield",
      "x-ms-request-id",
      "x-ms-version",
      "x-ms-lease-state",
      "x-ms-lease-status",
      "x-ms-meta-keepUntil",
      "x-ms-access-tier",
      "x-ms-access-tier-inferred",
      "x-ms-archive-status",
      "x-ms-access-tier-change-time"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string ExceptionDataResponseKey = "LastRequestResponse";

    public static bool SetHttpMessagesForTracing(
      this Exception exception,
      HttpRequestMessage request,
      HttpResponseMessage response)
    {
      try
      {
        string str = HttpRequestTraceDetails.SerializeRequestResponse(request, response);
        exception.Data[(object) "LastRequestResponse"] = (object) str;
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public static string GetHttpMessageDetailsForTracing(this Exception exception)
    {
      string detailsForTracing = (string) null;
      if (exception.Data.Contains((object) "LastRequestResponse"))
      {
        object obj = exception.Data[(object) "LastRequestResponse"];
        if (obj != null && obj is string str)
          detailsForTracing = str;
      }
      if (detailsForTracing == null)
        detailsForTracing = "No LastRequestResponse on exception " + exception.GetType().Name + ": " + exception.Message;
      return detailsForTracing;
    }

    private static string SerializeRequestResponse(
      HttpRequestMessage request,
      HttpResponseMessage response)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (request == null && response != null)
        request = response.RequestMessage;
      if (request != null)
      {
        string leftPart = request.RequestUri.GetLeftPart(UriPartial.Path);
        stringBuilder.AppendLine("HttpRequestMessage.Method: " + request.Method.ToString());
        stringBuilder.AppendLine("HttpRequestMessage.RequestUri: " + leftPart);
      }
      if (response != null)
      {
        stringBuilder.AppendLine(string.Format("{0}.{1}: {2} {3}", (object) "HttpResponseMessage", (object) "StatusCode", (object) (int) response.StatusCode, (object) response.StatusCode.ToString()));
        if (response?.Content?.Headers != null)
        {
          try
          {
            HttpContentHeaders headers = response.Content.Headers;
            if (headers.ContentLength.HasValue)
              stringBuilder.AppendLine(string.Format("{0}.{1}: {2}", (object) "HttpResponseMessage", (object) "ContentLength", (object) response.Content.Headers.ContentLength));
            if (headers.LastModified.HasValue)
            {
              string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) headers.LastModified);
              stringBuilder.AppendLine("HttpResponseMessage.LastModified: " + str);
            }
          }
          catch (ObjectDisposedException ex)
          {
          }
        }
        if (response.Headers.ETag != null)
          stringBuilder.AppendLine("HttpResponseMessage.ETag: " + response.Headers.ETag.Tag);
        if (response.Headers.Server != null)
        {
          string str = string.Join(" ", response.Headers.Server.Select<ProductInfoHeaderValue, string>((Func<ProductInfoHeaderValue, string>) (s => s.Product.ToString() + (string.IsNullOrEmpty(s.Comment) ? string.Empty : " " + s.Comment))));
          stringBuilder.AppendLine("HttpResponseMessage.Server: " + str);
        }
        foreach (string name in HttpRequestTraceDetails.ResponseHeaderNamesToCapture)
        {
          try
          {
            if (response.Headers.Contains(name))
            {
              string str = string.Join(",", response.Headers.GetValues(name).ToArray<string>());
              stringBuilder.AppendLine("HttpResponseHeaders." + name + ": " + str);
            }
          }
          catch (Exception ex) when (ex.StackTrace.Contains("CheckHeaderName"))
          {
          }
        }
      }
      return stringBuilder.ToString();
    }
  }
}
