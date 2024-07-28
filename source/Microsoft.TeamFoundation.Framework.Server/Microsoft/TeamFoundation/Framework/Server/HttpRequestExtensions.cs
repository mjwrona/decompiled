// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpRequestExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HttpRequestExtensions
  {
    public static IDictionary<string, string> GetSimpleNonEmptyHeaders(this HttpRequest request)
    {
      Dictionary<string, string> simpleNonEmptyHeaders = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      foreach (string allKey in request.Headers.AllKeys)
      {
        if (!simpleNonEmptyHeaders.ContainsKey(allKey))
        {
          string str = ((IEnumerable<string>) request.Headers.GetValues(allKey)).FirstOrDefault<string>();
          if (!string.IsNullOrEmpty(str))
            simpleNonEmptyHeaders.Add(allKey, str);
        }
      }
      return (IDictionary<string, string>) simpleNonEmptyHeaders;
    }

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

    public static string ReadBodyAsString(this HttpRequest httpRequest, int maxPostRequestSize) => HttpRequestExtensions.ReadBodyAsString(httpRequest.InputStream, maxPostRequestSize);

    internal static string ReadBodyAsString(Stream requestBodyStream, int maxPostRequestSize)
    {
      if (!requestBodyStream.CanSeek)
        throw new NotSupportedException();
      try
      {
        using (RestrictedStream restrictedStream = new RestrictedStream(requestBodyStream, 0L, (long) (maxPostRequestSize + 1), true))
          return new StreamReader((Stream) restrictedStream).ReadToEnd();
      }
      finally
      {
        requestBodyStream.Seek(0L, SeekOrigin.Begin);
      }
    }
  }
}
