// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.HtmlPlainTextWriter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  internal class HtmlPlainTextWriter : IHtmlFilterWriter
  {
    private StringBuilder m_str = new StringBuilder();
    private int m_eol = 9;
    private bool m_pre;

    public override string ToString()
    {
      this.m_str.Replace(" \r\n", "\r\n");
      return this.m_str.ToString();
    }

    private static bool IsHexChar(char ch, out int val)
    {
      if (ch >= '0' && ch <= '9')
      {
        val = (int) ch - 48;
        return true;
      }
      if (ch >= 'a' && ch <= 'f')
      {
        val = (int) ch - 97 + 10;
        return true;
      }
      if (ch >= 'A' && ch <= 'F')
      {
        val = (int) ch - 65 + 10;
        return true;
      }
      val = 0;
      return false;
    }

    private void AppendEol(int eol)
    {
      for (; eol > this.m_eol; ++this.m_eol)
        this.m_str.Append("\r\n");
    }

    private void AppendString(string s, int offs) => this.AppendString(s, offs, s.Length);

    private void AppendString(string s, int offs, int len)
    {
      int index = offs;
      int num = offs + len;
      while (index < num)
      {
        int startIndex1 = index;
        while (index < num && s[index] != '&' && s[index] != '\r' && s[index] != '\n')
          ++index;
        if (index > startIndex1)
        {
          this.m_str.Append(s, startIndex1, index - startIndex1);
          this.m_eol = 0;
        }
        if (index < num && (s[index] == '\r' || s[index] == '\n'))
        {
          ++index;
          if (index < num && (s[index] == '\r' || s[index] == '\n') && (int) s[index] != (int) s[index - 1])
            ++index;
          if (this.m_pre)
          {
            this.m_str.Append("\r\n");
            ++this.m_eol;
          }
        }
        else if (index < num && s[index] == '&')
        {
          int utf32 = 0;
          int startIndex2 = index++;
          if (index < num && s[index] == '#')
          {
            ++index;
            if (index < num && (s[index] == 'x' || s[index] == 'X'))
            {
              int val;
              for (++index; index < num && HtmlPlainTextWriter.IsHexChar(s[index], out val); ++index)
                utf32 = (utf32 << 4) + val;
            }
            else
            {
              for (; index < num && s[index] >= '0' && s[index] <= '9'; ++index)
                utf32 = utf32 * 10 + ((int) s[index] - 48);
            }
          }
          else
          {
            int startIndex3 = index;
            while (index < num && char.IsLetterOrDigit(s, index))
              ++index;
            if (!HtmlEntities.TryGetValue(s.Substring(startIndex3, index - startIndex3), out utf32))
              index = startIndex3;
          }
          if (index < num && s[index] == ';')
          {
            ++index;
            if (utf32 <= (int) ushort.MaxValue)
              this.m_str.Append((char) utf32);
            else
              this.m_str.Append(char.ConvertFromUtf32(utf32));
            if (utf32 == 13)
              ++this.m_eol;
            else
              this.m_eol = 0;
          }
          else if (index > startIndex2)
          {
            this.m_str.Append(s, startIndex2, index - startIndex2);
            this.m_eol = 0;
          }
        }
      }
    }

    void IHtmlFilterWriter.WriteText(string s, int offs, int len) => this.AppendString(s, offs, len);

    void IHtmlFilterWriter.WriteTag(string s, int offs, int len, string tag, bool endTag)
    {
      this.AppendEol(endTag ? AllowedHtmlTags.GetEolAfter(tag) : AllowedHtmlTags.GetEolBefore(tag));
      if (string.Equals(tag, "pre", StringComparison.OrdinalIgnoreCase))
      {
        this.m_pre = !endTag;
      }
      else
      {
        if (endTag)
          return;
        if (string.Equals(tag, "li", StringComparison.OrdinalIgnoreCase))
        {
          this.AppendString(InternalsResourceStrings.Get("BuletPlainText"), 0);
        }
        else
        {
          if (!string.Equals(tag, "br", StringComparison.OrdinalIgnoreCase))
            return;
          this.AppendEol(this.m_eol + 1);
        }
      }
    }

    void IHtmlFilterWriter.WriteEndOfTag(string s, int offs, int len, string tag)
    {
    }

    void IHtmlFilterWriter.WriteAttribute(
      string s,
      int offs,
      int len,
      string tag,
      string attr,
      int i1,
      int i2)
    {
      if (i2 <= i1 || !string.Equals(tag, "img", StringComparison.OrdinalIgnoreCase) || !string.Equals(attr, "alt", StringComparison.OrdinalIgnoreCase))
        return;
      this.AppendString(InternalsResourceStrings.Get("ImageAltTextStart"), 0);
      this.AppendString(s, i1, i2 - i1);
      this.AppendString(InternalsResourceStrings.Get("ImageAltTextEnd"), 0);
    }
  }
}
