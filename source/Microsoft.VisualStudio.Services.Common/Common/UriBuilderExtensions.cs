// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.UriBuilderExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class UriBuilderExtensions
  {
    public static UriBuilder AppendPathSegments(
      this UriBuilder uriBuilder,
      params string[] segments)
    {
      ArgumentUtility.CheckForNull<UriBuilder>(uriBuilder, nameof (uriBuilder));
      ArgumentUtility.CheckForNull<string[]>(segments, nameof (segments));
      if (segments.Length == 0)
        return uriBuilder;
      List<string> source = new List<string>(segments.Length + 1);
      source.Add(uriBuilder.Path);
      source.AddRange((IEnumerable<string>) segments);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      uriBuilder.Path = string.Join("/", source.Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))).SelectMany<string, string>((Func<string, IEnumerable<string>>) (s => (IEnumerable<string>) s.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries))).Select<string, string>((Func<string, string>) (s => s.Trim())).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))).Select<string, string>(UriBuilderExtensions.\u003C\u003EO.\u003C0\u003E__EscapeDataString ?? (UriBuilderExtensions.\u003C\u003EO.\u003C0\u003E__EscapeDataString = new Func<string, string>(Uri.EscapeDataString))));
      return uriBuilder;
    }

    public static UriBuilder AppendQuery(this UriBuilder uriBuilder, string name, string value)
    {
      ArgumentUtility.CheckForNull<UriBuilder>(uriBuilder, nameof (uriBuilder));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      string query = uriBuilder.Query;
      string str;
      if (query == null)
        str = (string) null;
      else
        str = query.TrimStart('?');
      StringBuilder stringBuilder = new StringBuilder(str);
      if (stringBuilder.Length > 0)
        stringBuilder.Append("&");
      uriBuilder.Query = stringBuilder.Append(Uri.EscapeDataString(name)).Append("=").Append(Uri.EscapeDataString(value)).ToString();
      return uriBuilder;
    }

    public static UriBuilder AppendQueryEscapeUriString(
      this UriBuilder uriBuilder,
      string name,
      string value)
    {
      ArgumentUtility.CheckForNull<UriBuilder>(uriBuilder, nameof (uriBuilder));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      string query = uriBuilder.Query;
      string str;
      if (query == null)
        str = (string) null;
      else
        str = query.TrimStart('?');
      StringBuilder stringBuilder = new StringBuilder(str);
      if (stringBuilder.Length > 0)
        stringBuilder.Append("&");
      uriBuilder.Query = stringBuilder.Append(Uri.EscapeUriString(name)).Append("=").Append(Uri.EscapeUriString(value)).ToString();
      return uriBuilder;
    }

    public static UriBuilder AppendQueryValueOnly(this UriBuilder uriBuilder, string value)
    {
      ArgumentUtility.CheckForNull<UriBuilder>(uriBuilder, nameof (uriBuilder));
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      string query = uriBuilder.Query;
      string str;
      if (query == null)
        str = (string) null;
      else
        str = query.TrimStart('?');
      StringBuilder stringBuilder = new StringBuilder(str);
      if (stringBuilder.Length > 0)
        stringBuilder.Append("&");
      uriBuilder.Query = stringBuilder.Append(Uri.EscapeDataString(value)).ToString();
      return uriBuilder;
    }

    public static string AbsoluteUri(this UriBuilder uriBuilder)
    {
      ArgumentUtility.CheckForNull<UriBuilder>(uriBuilder, nameof (uriBuilder));
      return uriBuilder.Uri.AbsoluteUri();
    }
  }
}
