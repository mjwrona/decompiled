// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.DiffLine
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public class DiffLine
  {
    private int m_hash;
    private string m_content;
    private EndOfLineTerminator m_eolType;

    internal DiffLine(string content, EndOfLineTerminator eol, DiffOptions options)
    {
      this.m_content = content;
      this.m_eolType = eol;
      if ((options.Flags & (DiffOptionFlags.IgnoreCase | DiffOptionFlags.IgnoreWhiteSpace)) != DiffOptionFlags.None)
      {
        bool ignoreWhiteSpace = (options.Flags & DiffOptionFlags.IgnoreWhiteSpace) != 0;
        bool ignoreCase = (options.Flags & DiffOptionFlags.IgnoreCase) != 0;
        this.m_hash = this.ComputeHashCode(options.CultureInfo != null ? options.CultureInfo : CultureInfo.CurrentCulture, ignoreWhiteSpace, ignoreCase);
      }
      else if ((options.Flags & DiffOptionFlags.IgnoreLeadingAndTrailingWhiteSpace) != DiffOptionFlags.None)
        this.m_hash = this.m_content.Trim().GetHashCode();
      else
        this.m_hash = this.m_content.GetHashCode();
    }

    private int ComputeHashCode(CultureInfo cultureInfo, bool ignoreWhiteSpace, bool ignoreCase)
    {
      int num1 = 5381;
      int num2 = num1;
      for (int index = 0; index < this.m_content.Length; ++index)
      {
        if (!ignoreWhiteSpace || !char.IsWhiteSpace(this.m_content[index]))
        {
          char ch = ignoreCase ? char.ToLower(this.m_content[index], cultureInfo) : this.m_content[index];
          num1 = (num1 << 5) + num1 ^ (int) ch & (int) byte.MaxValue;
          num2 = (num2 << 5) + num2 ^ (int) ch >> 8;
        }
      }
      return num1 + num2 * 1566083941;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.Content);
      switch (this.EndOfLineTerminator)
      {
        case EndOfLineTerminator.LineFeed:
          stringBuilder.Append('\n');
          break;
        case EndOfLineTerminator.CarriageReturn:
          stringBuilder.Append('\r');
          break;
        case EndOfLineTerminator.CarriageReturnLineFeed:
          stringBuilder.Append("\r\n");
          break;
        case EndOfLineTerminator.LineSeparator:
          stringBuilder.Append('\u2028');
          break;
        case EndOfLineTerminator.ParagraphSeparator:
          stringBuilder.Append('\u2029');
          break;
        case EndOfLineTerminator.NextLine:
          stringBuilder.Append('\u0085');
          break;
      }
      return stringBuilder.ToString();
    }

    public override int GetHashCode() => this.m_hash;

    public string Content => this.m_content;

    public EndOfLineTerminator EndOfLineTerminator => this.m_eolType;
  }
}
