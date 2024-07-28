// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.TermExpression
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions
{
  public class TermExpression : RelevanceExpression
  {
    public TermExpression(string type, Operator op, string value)
    {
      this.Type = !string.IsNullOrWhiteSpace(type) ? type : throw new ArgumentException("Invalid type");
      this.Operator = op;
      this.Value = value;
    }

    public string Type { get; set; }

    public Operator Operator { get; set; }

    public string Value { get; set; }

    public override string ToString() => TermExpression.WrapColumnName(this.Type) + this.Operator.ToSyntaxString() + TermExpression.WrapValue(this.Value);

    public override bool Equals(object obj) => obj is TermExpression termExpression && this.Type == termExpression.Type && this.Operator == termExpression.Operator && this.Value == termExpression.Value;

    public override int GetHashCode() => this.Type.GetHashCode() ^ this.Operator.GetHashCode() ^ this.Value.GetHashCode();

    protected internal static string WrapColumnName(string type)
    {
      bool flag = false;
      for (int index = 0; index < type.Length; ++index)
      {
        char c = type[index];
        if (char.IsWhiteSpace(c) || c == ']')
        {
          flag = true;
          break;
        }
      }
      return !flag ? type : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) type.Replace("]", "]]"));
    }

    protected internal static string WrapValue(string value)
    {
      if (string.IsNullOrEmpty(value))
        return "\"\"";
      bool flag = false;
      for (int index = 0; index < value.Length; ++index)
      {
        char c = value[index];
        if (char.IsWhiteSpace(c) || c == '"')
        {
          flag = true;
          break;
        }
      }
      return !flag ? value : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) value.Replace("\"", "\"\""));
    }
  }
}
