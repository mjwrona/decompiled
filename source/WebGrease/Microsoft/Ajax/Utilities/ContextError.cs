// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ContextError
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public class ContextError
  {
    public int ErrorNumber { get; set; }

    public string File { get; set; }

    public virtual int Severity { get; set; }

    public virtual string Subcategory { get; set; }

    [Localizable(false)]
    public virtual string ErrorCode { get; set; }

    public virtual int StartLine { get; set; }

    public virtual int StartColumn { get; set; }

    public virtual int EndLine { get; set; }

    public virtual int EndColumn { get; set; }

    public virtual string Message { get; set; }

    public virtual bool IsError { get; set; }

    public string HelpKeyword { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(this.File))
        stringBuilder.Append(this.File);
      if (this.StartLine > 0)
      {
        stringBuilder.AppendFormat("({0}", (object) this.StartLine);
        if (this.EndLine > this.StartLine)
        {
          if (this.StartColumn > 0 && this.EndColumn > 0)
            stringBuilder.AppendFormat(",{0},{1},{2}", (object) this.StartColumn, (object) this.EndLine, (object) this.EndColumn);
          else
            stringBuilder.AppendFormat("-{0}", (object) this.EndLine);
        }
        else if (this.StartColumn > 0)
        {
          stringBuilder.AppendFormat(",{0}", (object) this.StartColumn);
          if (this.EndColumn > this.StartColumn)
            stringBuilder.AppendFormat("-{0}", (object) this.EndColumn);
        }
        stringBuilder.Append(')');
      }
      stringBuilder.Append(':');
      if (!string.IsNullOrEmpty(this.Subcategory))
      {
        stringBuilder.Append(' ');
        stringBuilder.Append(this.Subcategory);
      }
      stringBuilder.Append(this.IsError ? " error " : " warning ");
      if (!string.IsNullOrEmpty(this.ErrorCode))
        stringBuilder.Append(this.ErrorCode);
      stringBuilder.Append(": ");
      if (!string.IsNullOrEmpty(this.Message))
        stringBuilder.Append(this.Message);
      return stringBuilder.ToString();
    }

    internal static string GetSubcategory(int severity)
    {
      switch (severity)
      {
        case 0:
          return CommonStrings.Severity0;
        case 1:
          return CommonStrings.Severity1;
        case 2:
          return CommonStrings.Severity2;
        case 3:
          return CommonStrings.Severity3;
        case 4:
          return CommonStrings.Severity4;
        default:
          return CommonStrings.SeverityUnknown.FormatInvariant((object) severity);
      }
    }
  }
}
