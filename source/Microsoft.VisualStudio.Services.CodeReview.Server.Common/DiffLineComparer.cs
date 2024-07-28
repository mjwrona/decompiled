// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.DiffLineComparer
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public class DiffLineComparer : IEqualityComparer<DiffLine>
  {
    private DiffOptions m_options;
    private bool m_ignoreCase;
    private bool m_ignoreWhiteSpace;
    private bool m_ignoreEndOfLine;
    private bool m_ignoreLeadingAndTrailingWhiteSpace;
    private bool m_ignoreEndOfFileEndOfLineDifference;

    public DiffLineComparer(DiffOptions options)
    {
      this.m_options = options;
      this.m_ignoreCase = (options.Flags & DiffOptionFlags.IgnoreCase) != 0;
      this.m_ignoreWhiteSpace = (options.Flags & DiffOptionFlags.IgnoreWhiteSpace) != 0;
      this.m_ignoreEndOfLine = (options.Flags & DiffOptionFlags.IgnoreEndOfLineDifference) != 0;
      this.m_ignoreEndOfFileEndOfLineDifference = (options.Flags & DiffOptionFlags.IgnoreEndOfFileEndOfLineDifference) != 0;
      this.m_ignoreLeadingAndTrailingWhiteSpace = (options.Flags & DiffOptionFlags.IgnoreLeadingAndTrailingWhiteSpace) != 0;
    }

    public bool Equals(DiffLine x, DiffLine y)
    {
      if (x.GetHashCode() != y.GetHashCode())
        return false;
      if (this.m_ignoreEndOfLine)
      {
        if (!this.m_ignoreEndOfFileEndOfLineDifference && x.EndOfLineTerminator == EndOfLineTerminator.None != (y.EndOfLineTerminator == EndOfLineTerminator.None))
          return false;
      }
      else if (x.EndOfLineTerminator != y.EndOfLineTerminator)
        return false;
      CompareOptions options = this.m_ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None;
      CultureInfo cultureInfo = this.m_options.CultureInfo != null ? this.m_options.CultureInfo : CultureInfo.CurrentCulture;
      if (this.m_ignoreWhiteSpace)
      {
        string string1 = this.RemoveWhitespace(x.Content);
        string string2 = this.RemoveWhitespace(y.Content);
        return cultureInfo.CompareInfo.Compare(string1, string2, options) == 0;
      }
      if (!this.m_ignoreLeadingAndTrailingWhiteSpace)
        return cultureInfo.CompareInfo.Compare(x.Content, y.Content, options) == 0;
      string string1_1 = x.Content.Trim();
      string string2_1 = y.Content.Trim();
      return cultureInfo.CompareInfo.Compare(string1_1, string2_1, options) == 0;
    }

    public bool Equals(object left, object right) => this.Equals(left as DiffLine, right as DiffLine);

    public int GetHashCode(DiffLine element) => element.GetHashCode();

    public int GetHashCode(object element) => this.GetHashCode(element as DiffLine);

    private string RemoveWhitespace(string input)
    {
      StringBuilder stringBuilder = new StringBuilder(input.Length);
      int startIndex = 0;
      for (int index = 0; index < input.Length; ++index)
      {
        if (char.IsWhiteSpace(input[index]))
        {
          int count = index - startIndex;
          if (count > 0)
            stringBuilder.Append(input, startIndex, count);
          startIndex = index + 1;
        }
      }
      int count1 = input.Length - startIndex;
      if (count1 > 0)
        stringBuilder.Append(input, startIndex, count1);
      return stringBuilder.ToString();
    }
  }
}
