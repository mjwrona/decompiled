// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.TermsExpression
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions
{
  public class TermsExpression : RelevanceExpression
  {
    public TermsExpression(string type, Operator op, IEnumerable<string> terms)
    {
      if (string.IsNullOrWhiteSpace(type))
        throw new ArgumentException("Invalid type");
      if (op.ToSyntaxString() != Operator.In.ToSyntaxString())
        throw new NotSupportedException(op.ToSyntaxString());
      this.Type = type;
      this.Operator = op;
      this.Terms = terms;
    }

    public string Type { get; set; }

    public Operator Operator { get; set; }

    public IEnumerable<string> Terms { get; set; }

    public override string ToString()
    {
      if (this.Terms == null)
        return string.Empty;
      return this.Type + this.Operator.ToSyntaxString() + "[" + string.Join(", ", this.Terms.Select<string, string>((Func<string, string>) (v => "\"" + v + "\""))) + "]";
    }
  }
}
