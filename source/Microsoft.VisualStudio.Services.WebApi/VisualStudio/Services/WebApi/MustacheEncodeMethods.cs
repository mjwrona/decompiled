// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MustacheEncodeMethods
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class MustacheEncodeMethods
  {
    public static string HtmlEncode(string value) => UriUtility.HtmlEncode(value);

    public static string JsonEncode(string value)
    {
      if (string.IsNullOrEmpty(value))
        return string.Empty;
      string str = JsonConvert.ToString(value);
      return str.Substring(1, str.Length - 2);
    }

    public static string NoEncode(string value) => value;
  }
}
