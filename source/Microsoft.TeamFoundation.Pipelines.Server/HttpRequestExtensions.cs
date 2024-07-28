// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.HttpRequestExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class HttpRequestExtensions
  {
    private const int c_maxPostRequestSize = 26214400;

    public static string GetSimpleHeaderValue(this HttpRequest request, string name)
    {
      if (!string.IsNullOrEmpty(name))
      {
        string[] values = request.Headers.GetValues(name);
        if (values != null && values.Length != 0)
          return values[0];
      }
      return (string) null;
    }

    public static string GetSimpleHeaderValue(this HttpRequestMessage request, string name)
    {
      IEnumerable<string> values = (IEnumerable<string>) null;
      return !string.IsNullOrEmpty(name) && request.Headers.TryGetValues(name, out values) ? values.FirstOrDefault<string>() : (string) null;
    }

    public static IDictionary<string, string> GetSimpleHeaders(this HttpRequestMessage request)
    {
      Dictionary<string, string> simpleHeaders = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Headers)
      {
        if (!simpleHeaders.ContainsKey(header.Key))
        {
          string str = header.Value.FirstOrDefault<string>();
          if (!string.IsNullOrEmpty(str))
            simpleHeaders.Add(header.Key, str);
        }
      }
      return (IDictionary<string, string>) simpleHeaders;
    }

    public static IDictionary<string, string> GetSimpleHeaders(this HttpRequest request)
    {
      Dictionary<string, string> simpleHeaders = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      foreach (string allKey in request.Headers.AllKeys)
      {
        if (!simpleHeaders.ContainsKey(allKey))
        {
          string str = ((IEnumerable<string>) request.Headers.GetValues(allKey)).FirstOrDefault<string>();
          if (!string.IsNullOrEmpty(str))
            simpleHeaders.Add(allKey, str);
        }
      }
      return (IDictionary<string, string>) simpleHeaders;
    }

    public static string ReadBodyAsString(this HttpRequest httpRequest) => HttpRequestExtensions.ReadBodyAsString(httpRequest.InputStream);

    public static string ReadBodyAsString(this HttpRequestMessage httpRequest) => HttpRequestExtensions.ReadBodyAsString(httpRequest.Content.ReadAsStreamAsync().Result);

    private static string ReadBodyAsString(Stream requestBodyStream)
    {
      if (!requestBodyStream.CanSeek)
        throw new NotSupportedException();
      try
      {
        using (RestrictedStream restrictedStream = new RestrictedStream(requestBodyStream, 0L, 26214401L, true))
          return new StreamReader((Stream) restrictedStream).ReadToEnd();
      }
      finally
      {
        requestBodyStream.Seek(0L, SeekOrigin.Begin);
      }
    }
  }
}
