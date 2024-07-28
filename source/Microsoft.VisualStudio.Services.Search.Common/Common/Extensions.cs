// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Extensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class Extensions
  {
    public static string Indent(this string s, int size = 4)
    {
      if (string.IsNullOrEmpty(s))
        return s;
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = " ".PadLeft(size);
      foreach (string str2 in Regex.Split(s, "\r\n"))
      {
        if (str2.Length > 0)
          stringBuilder.Append(str1);
        stringBuilder.AppendLine(str2);
      }
      return stringBuilder.ToString();
    }

    public static string PrettyJson(this string json)
    {
      try
      {
        return JObject.Parse(json).ToString(Formatting.Indented);
      }
      catch
      {
        return json;
      }
    }

    public static string CompactJson(this string json)
    {
      try
      {
        return JObject.Parse(json).ToString(Formatting.None);
      }
      catch
      {
        return json;
      }
    }

    public static string NormalizeString(this string text)
    {
      if (!string.IsNullOrEmpty(text))
      {
        text = text.Trim();
        text = text.ToLowerInvariant();
      }
      return text;
    }

    public static string NormalizeStringAndReplaceTurkishDottedI(this string text)
    {
      if (!string.IsNullOrEmpty(text))
      {
        text = text.Trim();
        text = text.ToLowerInvariant();
        text = text.Replace('İ', 'i');
      }
      return text;
    }

    public static string NormalizeStringForTurkishLocale(this string text)
    {
      if (!string.IsNullOrEmpty(text))
      {
        text = text.Trim();
        if (text.Contains("I"))
          text = text.Replace('I', 'ı');
        text = text.ToLowerInvariant();
        text = text.Replace('İ', 'i');
      }
      return text;
    }

    public static string NormalizeStringWithCurrentCulture(this string text)
    {
      if (!string.IsNullOrEmpty(text))
      {
        text = text.Trim();
        text = text.ToLower(CultureInfo.CurrentCulture);
      }
      return text;
    }

    public static string NormalizePath(this string text)
    {
      if (!string.IsNullOrWhiteSpace(text))
      {
        text = text.NormalizePathWithoutTrimming();
        text = text.Trim('\\');
      }
      return text;
    }

    public static string NormalizePathWithoutTrimming(this string text)
    {
      if (!string.IsNullOrWhiteSpace(text))
      {
        text = text.NormalizeString();
        text = text.Replace('/', '\\');
        text = text.BackSlashTrim();
      }
      return text;
    }

    public static string NormalizePathWithoutTrimmingSlashesWithoutChangingCase(this string text)
    {
      if (!string.IsNullOrWhiteSpace(text))
      {
        text = text.Trim();
        text = text.BackSlashTrim();
      }
      return text;
    }

    private static string BackSlashTrim(this string text)
    {
      if (!string.IsNullOrWhiteSpace(text))
        text = Microsoft.VisualStudio.Services.Search.WebApi.Legacy.RegularExpressions.SlashTrimRegex.Replace(text, "\\");
      return text;
    }

    public static string NormalizeFileExtension(this string text)
    {
      if (!string.IsNullOrWhiteSpace(text))
      {
        text = text.NormalizeString();
        int num = text.LastIndexOf('.');
        if (num != -1)
          text = text.Substring(num + 1);
      }
      return text;
    }

    public static string CorrectEscapedDoubleQuotes(this string text)
    {
      if (!string.IsNullOrWhiteSpace(text) && text.Length > 1)
      {
        text = text.Substring(1, text.Length - 2);
        text = text.Replace("\\\"", "\"");
        text = "\"" + text + "\"";
      }
      return text;
    }

    public static bool ContainsWhitespace(this string input) => input != null ? Microsoft.VisualStudio.Services.Search.WebApi.Legacy.RegularExpressions.WhitespaceRegex.IsMatch(input) : throw new ArgumentNullException(nameof (input));

    public static bool ContainsWhitespaceOrSpecialCharacters(this string input) => input != null ? Microsoft.VisualStudio.Services.Search.WebApi.Legacy.RegularExpressions.SpecialCharsRegex.IsMatch(input) : throw new ArgumentNullException(nameof (input));

    public static bool ContainsSubstring(this string input) => input != null ? Microsoft.VisualStudio.Services.Search.WebApi.Legacy.RegularExpressions.SupportedSubStringRegex.IsMatch(input) : throw new ArgumentNullException(nameof (input));
  }
}
