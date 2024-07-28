// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.CodeElementFilterExpression
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions
{
  public class CodeElementFilterExpression : RelevanceExpression
  {
    public CodeElementFilterExpression(string value, IEnumerable<int> tokenIds)
    {
      this.FieldValue = value;
      this.CodeTokenIds = tokenIds;
    }

    public string FieldValue { get; set; }

    public IEnumerable<int> CodeTokenIds { get; set; }

    public override string ToString() => this.FieldValue + this.WrapTokenIds(this.CodeTokenIds);

    private string WrapTokenIds(IEnumerable<int> tokenIds)
    {
      string str = string.Empty;
      string separator = ",";
      if (tokenIds.Any<int>())
        str = "CodeTokenIds(" + string.Join<int>(separator, tokenIds) + ")";
      return str;
    }
  }
}
