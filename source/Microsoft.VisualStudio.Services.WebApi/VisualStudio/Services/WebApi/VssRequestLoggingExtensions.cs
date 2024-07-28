// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssRequestLoggingExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class VssRequestLoggingExtensions
  {
    private const string _requestFormat = "Web method running: [{0}] ({1}){2}[{3}] E2EId: {4}";
    private const string _responseFormat = "Web method response: [{0}] ({1}){2}[{3}] E2EId: {4}, ActivityId: {5} {6} ms";
    private const string _unknown = "<unknown>";
    private const string _apis = "_apis/";

    internal static string GetRequestString(this HttpRequestMessage message)
    {
      Guid guid;
      VssRequestLoggingExtensions.TryGetHeaderGuid((HttpHeaders) message.Headers, "X-VSS-E2EID", out guid);
      string verb;
      string resource;
      string area;
      VssRequestLoggingExtensions.ExtractRequestStrings(message, out verb, out resource, out area);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Web method running: [{0}] ({1}){2}[{3}] E2EId: {4}", (object) message.RequestUri.AbsoluteUri, (object) verb, (object) resource, (object) area, (object) guid);
    }

    internal static string GetResponseString(this HttpResponseMessage response, long milliseconds)
    {
      string verb;
      string resource;
      string area;
      VssRequestLoggingExtensions.ExtractRequestStrings(response.RequestMessage, out verb, out resource, out area);
      Guid guid1;
      VssRequestLoggingExtensions.TryGetHeaderGuid((HttpHeaders) response.Headers, "X-VSS-E2EID", out guid1);
      Guid guid2;
      VssRequestLoggingExtensions.TryGetHeaderGuid((HttpHeaders) response.Headers, "ActivityId", out guid2);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Web method response: [{0}] ({1}){2}[{3}] E2EId: {4}, ActivityId: {5} {6} ms", (object) response.RequestMessage.RequestUri.AbsoluteUri, (object) verb, (object) resource, (object) area, (object) guid1, (object) guid2, (object) milliseconds);
    }

    private static void ExtractRequestStrings(
      HttpRequestMessage message,
      out string verb,
      out string resource,
      out string area)
    {
      verb = message.Method.ToString().ToUpper();
      resource = "<unknown>";
      area = "<unknown>";
      int length = message.RequestUri.Segments.Length;
      if (length > 0)
        resource = message.RequestUri.Segments[length - 1].TrimEnd('/');
      for (int index = 0; index < length; ++index)
      {
        if (string.Equals(message.RequestUri.Segments[index], "_apis/", StringComparison.OrdinalIgnoreCase))
        {
          if (length <= index + 1)
            break;
          area = message.RequestUri.Segments[index + 1].TrimEnd('/');
          break;
        }
      }
    }

    private static bool TryGetHeaderGuid(HttpHeaders headers, string key, out Guid value)
    {
      value = Guid.Empty;
      IEnumerable<string> values;
      return headers.TryGetValues(key, out values) && Guid.TryParse(values.FirstOrDefault<string>(), out value);
    }
  }
}
