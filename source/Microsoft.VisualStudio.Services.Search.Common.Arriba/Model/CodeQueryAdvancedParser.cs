// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Model.CodeQueryAdvancedParser
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Model
{
  public class CodeQueryAdvancedParser : QueryParser
  {
    public CodeQueryAdvancedParser()
    {
    }

    public CodeQueryAdvancedParser(ISet<string> supportedOperators)
      : base(supportedOperators)
    {
    }

    internal override string ParseUntilEndToken(TokenType endToken)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.m_scanner.Current.Content);
      this.m_scanner.Next();
      while (this.m_scanner.Current.Type != TokenType.End)
      {
        if (this.m_scanner.Current.Type == endToken)
        {
          stringBuilder.Append(this.m_scanner.Current.Prefix);
          stringBuilder.Append(this.m_scanner.Current.Content);
          this.m_scanner.Next();
          if (this.m_scanner.Current.Type == endToken && this.m_scanner.Current.Prefix.Length == 0)
            stringBuilder.Append(this.m_scanner.Current.Content);
          else
            break;
        }
        else
        {
          stringBuilder.Append(this.m_scanner.Current.Prefix);
          stringBuilder.Append(this.m_scanner.Current.Content);
        }
        this.m_scanner.Next();
      }
      return stringBuilder.ToString();
    }
  }
}
