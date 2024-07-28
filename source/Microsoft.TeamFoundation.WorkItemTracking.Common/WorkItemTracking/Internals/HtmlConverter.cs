// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.HtmlConverter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class HtmlConverter
  {
    public static string Strip(string s)
    {
      HtmlSimpleWriter writer = new HtmlSimpleWriter();
      HtmlConverter.Parse(s, (IHtmlFilterWriter) writer);
      return writer.ToString();
    }

    public static StringBuilder StripToBuilder(string s)
    {
      HtmlSimpleWriter writer = new HtmlSimpleWriter();
      HtmlConverter.Parse(s, (IHtmlFilterWriter) writer);
      return writer.Content;
    }

    public static string ConvertToPlainText(string s)
    {
      HtmlPlainTextWriter writer = new HtmlPlainTextWriter();
      HtmlConverter.Parse(s, (IHtmlFilterWriter) writer);
      return writer.ToString();
    }

    public static string ConvertToHtml(object o, bool editableStyle)
    {
      string html = UriUtility.HtmlEncode(string.Concat(o));
      StringBuilder stringBuilder = new StringBuilder();
      int index = 0;
      do
      {
        int startIndex = index;
        while (index < html.Length && html[index] != '\r' && html[index] != '\n')
          ++index;
        int num = index;
        if (index < html.Length)
        {
          ++index;
          if (index < html.Length && (html[index] == '\r' || html[index] == '\n') && (int) html[index] != (int) html[index - 1])
            ++index;
        }
        if (editableStyle)
        {
          if (num != startIndex)
          {
            stringBuilder.Append("<p>");
            stringBuilder.Append(html, startIndex, num - startIndex);
            stringBuilder.Append("</p>\r\n");
          }
          else if (index < html.Length)
            stringBuilder.Append("<p>&nbsp;</p>\r\n");
        }
        else
        {
          if (startIndex == 0 && index == html.Length)
            return html;
          stringBuilder.Append(html, startIndex, num - startIndex);
          if (index != html.Length)
            stringBuilder.Append("<br>\r\n");
        }
      }
      while (index < html.Length);
      return stringBuilder.ToString();
    }

    private static int SkipWhiteSpaces(string s, int i)
    {
      int length = s.Length;
      while (i < length && (char.IsWhiteSpace(s, i) || char.IsControl(s, i)))
        ++i;
      return i;
    }

    private static int SkipUntil(string s, int i, char c1)
    {
      int length = s.Length;
      do
        ;
      while (i < length && (int) s[i++] != (int) c1);
      return i;
    }

    private static int SkipUntil(string s, int i, char c1, char c2, char c3)
    {
      int length = s.Length;
      while (i + 2 < length && ((int) s[i] != (int) c1 || (int) s[i + 1] != (int) c2 || (int) s[i + 2] != (int) c3))
        ++i;
      if (i + 2 < length)
        i += 3;
      else
        i = length;
      return i;
    }

    private static int ScanName(string s, int i, out string name)
    {
      int length = s.Length;
      int startIndex = i;
      while (i < length && (char.IsLetterOrDigit(s, i) || s[i] == ':' || s[i] == '_'))
        ++i;
      name = s.Substring(startIndex, i - startIndex);
      return i;
    }

    internal static void Parse(string s, IHtmlFilterWriter writer)
    {
      int length = s.Length;
      int num1 = 0;
      while (num1 < length)
      {
        int offs1 = num1;
        while (num1 < length && s[num1] != '<')
          ++num1;
        if (num1 > offs1)
        {
          writer.WriteText(s, offs1, num1 - offs1);
        }
        else
        {
          int num2 = num1;
          int num3 = num2 + 1;
          int offs2 = num2;
          if (num3 < length && s[num3] == '!')
          {
            int num4 = num3 + 1;
            if (num4 + 1 < length && s[num4] == '-' && s[num4 + 1] == '-')
            {
              int i = num4 + 2;
              num1 = HtmlConverter.SkipUntil(s, i, '-', '-', '>');
            }
            else if (num4 + 6 < length && s[num4] == '[' && s[num4 + 1] == 'C' && s[num4 + 2] == 'D' && s[num4 + 3] == 'A' && s[num4 + 4] == 'T' && s[num4 + 5] == 'A' && s[num4 + 6] == '[')
            {
              int i = num4 + 7;
              num1 = HtmlConverter.SkipUntil(s, i, ']', ']', '>');
            }
            else
              num1 = HtmlConverter.SkipUntil(s, num4, '>');
          }
          else if (num3 < length && s[num3] == '?')
          {
            int i = num3 + 1;
            num1 = HtmlConverter.SkipUntil(s, i, '>');
          }
          else
          {
            bool endTag = false;
            if (num3 < length && s[num3] == '/')
            {
              endTag = true;
              ++num3;
            }
            int i1 = HtmlConverter.SkipWhiteSpaces(s, num3);
            string name1;
            num1 = HtmlConverter.ScanName(s, i1, out name1);
            if (AllowedHtmlTags.IsSpecialTag(name1))
            {
              num1 = HtmlConverter.SkipUntil(s, num1, '>');
              while (num1 < length)
              {
                num1 = HtmlConverter.SkipUntil(s, num1, '<');
                if (num1 + 2 < length && s[num1] == '!' && s[num1 + 1] == '-' && s[num1 + 2] == '-')
                  num1 = HtmlConverter.SkipUntil(s, num1, '-', '-', '>');
                else if (num1 < length && s[num1] == '/')
                {
                  int i2 = num1 + 1;
                  int i3 = HtmlConverter.SkipWhiteSpaces(s, i2);
                  string name2;
                  num1 = HtmlConverter.ScanName(s, i3, out name2);
                  if (AllowedHtmlTags.AreTagsEqual(name1, name2))
                  {
                    num1 = HtmlConverter.SkipUntil(s, num1, '>');
                    break;
                  }
                }
                else if (num1 < length)
                  ++num1;
              }
            }
            else if (!AllowedHtmlTags.IsAllowedTag(name1))
            {
              num1 = HtmlConverter.SkipUntil(s, num1, '>');
            }
            else
            {
              writer.WriteTag(s, offs2, num1 - offs2, name1, endTag);
              while (num1 < length)
              {
                int offs3 = num1;
                num1 = HtmlConverter.SkipWhiteSpaces(s, num1);
                if (num1 < length && s[num1] == '/')
                  ++num1;
                if (num1 < length && s[num1] == '>')
                {
                  ++num1;
                  writer.WriteEndOfTag(s, offs3, num1 - offs3, name1);
                  break;
                }
                if (num1 < length && char.IsLetterOrDigit(s, num1))
                {
                  string name3;
                  int i4 = HtmlConverter.ScanName(s, num1, out name3);
                  num1 = HtmlConverter.SkipWhiteSpaces(s, i4);
                  int num5 = 0;
                  int i2 = 0;
                  if (num1 < length && s[num1] == '=')
                  {
                    int i5 = num1 + 1;
                    num1 = HtmlConverter.SkipWhiteSpaces(s, i5);
                    num5 = num1;
                    if (num1 < length && (s[num1] == '\'' || s[num1] == '"'))
                    {
                      char ch = s[num1++];
                      num5 = num1;
                      while (num1 < length && (int) s[num1] != (int) ch)
                        ++num1;
                      i2 = num1;
                      if (num1 < length && (int) s[num1] == (int) ch)
                        ++num1;
                    }
                    else
                    {
                      while (num1 < length && s[num1] != '>' && !char.IsWhiteSpace(s, num1) && !char.IsControl(s, num1))
                        ++num1;
                      i2 = num1;
                    }
                    if (num1 >= num5 + 11 && AllowedHtmlTags.AreTagsEqual(s.Substring(num5, 11), "javascript:"))
                      name3 = string.Empty;
                  }
                  if (AllowedHtmlTags.IsAllowedAttribute(name1, name3))
                    writer.WriteAttribute(s, offs3, num1 - offs3, name1, name3, num5, i2);
                }
                else if (num1 < length && s[num1] != '>')
                  ++num1;
              }
            }
          }
        }
      }
    }
  }
}
