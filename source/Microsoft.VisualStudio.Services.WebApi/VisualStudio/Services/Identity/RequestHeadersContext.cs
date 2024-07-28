// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.RequestHeadersContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class RequestHeadersContext
  {
    internal SequenceContext SequenceContext { get; set; }

    internal bool IgnoreCache { get; set; }

    public RequestHeadersContext(SequenceContext sequenceContext)
      : this(sequenceContext, false)
    {
    }

    public RequestHeadersContext(SequenceContext sequenceContext, bool ignoreCache)
    {
      this.SequenceContext = sequenceContext;
      this.IgnoreCache = ignoreCache;
    }

    private static bool ParseOrGetDefault(string s)
    {
      bool result;
      return !string.IsNullOrWhiteSpace(s) && bool.TryParse(s, out result) && result;
    }

    internal class HeadersUtils
    {
      private const string c_ignoreCacheHeader = "X-VSSF-IMS-IgnoreCache";

      public static KeyValuePair<string, string>[] PopulateRequestHeaders(
        RequestHeadersContext requestHeaderContext)
      {
        if (requestHeaderContext == null)
          return new KeyValuePair<string, string>[0];
        KeyValuePair<string, string>[] keyValuePairArray1 = SequenceContext.HeadersUtils.PopulateRequestHeaders(requestHeaderContext.SequenceContext);
        KeyValuePair<string, string>[] keyValuePairArray2 = new KeyValuePair<string, string>[keyValuePairArray1.Length + 1];
        keyValuePairArray1.CopyTo((Array) keyValuePairArray2, 0);
        keyValuePairArray2[keyValuePairArray1.Length] = new KeyValuePair<string, string>("X-VSSF-IMS-IgnoreCache", requestHeaderContext.IgnoreCache.ToString());
        return keyValuePairArray2;
      }

      public static bool TryExtractRequestHeaderContext(
        HttpRequestHeaders httpRequestHeaders,
        out RequestHeadersContext requestHeadersContext)
      {
        requestHeadersContext = (RequestHeadersContext) null;
        IEnumerable<string> values;
        int num1 = !httpRequestHeaders.TryGetValues("X-VSSF-IMS-IgnoreCache", out values) ? 0 : (values != null ? 1 : 0);
        SequenceContext sequenceContext1;
        bool sequenceContext2 = SequenceContext.HeadersUtils.TryExtractSequenceContext(httpRequestHeaders, out sequenceContext1);
        bool orGetDefault = RequestHeadersContext.ParseOrGetDefault(values != null ? values.FirstOrDefault<string>() : (string) null);
        requestHeadersContext = new RequestHeadersContext(sequenceContext1, orGetDefault);
        int num2 = sequenceContext2 ? 1 : 0;
        return (num1 | num2) != 0;
      }
    }
  }
}
