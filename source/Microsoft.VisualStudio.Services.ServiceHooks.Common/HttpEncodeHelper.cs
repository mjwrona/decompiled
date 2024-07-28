// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.HttpEncodeHelper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using System.Web.Util;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class HttpEncodeHelper
  {
    public static string EncodeHeaderValue(string headerValue)
    {
      string encodedHeaderValue;
      HttpEncodeHelper.AdoHttpEncoder.Encoder.HeaderEncode(string.Empty, headerValue, out string _, out encodedHeaderValue);
      return encodedHeaderValue;
    }

    private class AdoHttpEncoder : HttpEncoder
    {
      private static HttpEncodeHelper.AdoHttpEncoder _AdoHttpEncoder;

      internal static HttpEncodeHelper.AdoHttpEncoder Encoder => HttpEncodeHelper.AdoHttpEncoder._AdoHttpEncoder ?? (HttpEncodeHelper.AdoHttpEncoder._AdoHttpEncoder = new HttpEncodeHelper.AdoHttpEncoder());

      internal void HeaderEncode(
        string headerName,
        string headerValue,
        out string encodedHeaderName,
        out string encodedHeaderValue)
      {
        this.HeaderNameValueEncode(headerName, headerValue, out encodedHeaderName, out encodedHeaderValue);
      }
    }
  }
}
