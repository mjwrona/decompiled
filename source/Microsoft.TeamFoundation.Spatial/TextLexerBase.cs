// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.TextLexerBase
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.IO;
using System.Text;

namespace Microsoft.Spatial
{
  internal abstract class TextLexerBase
  {
    private TextReader reader;
    private LexerToken currentToken;
    private LexerToken peekToken;

    protected TextLexerBase(TextReader text) => this.reader = text;

    public LexerToken CurrentToken => this.currentToken;

    public bool Peek(out LexerToken token)
    {
      if (this.peekToken != null)
      {
        token = this.peekToken;
        return true;
      }
      LexerToken currentToken = this.currentToken;
      if (this.Next())
      {
        this.peekToken = this.currentToken;
        token = this.currentToken;
        this.currentToken = currentToken;
        return true;
      }
      this.peekToken = (LexerToken) null;
      token = (LexerToken) null;
      this.currentToken = currentToken;
      return false;
    }

    public bool Next()
    {
      if (this.peekToken != null)
      {
        this.currentToken = this.peekToken;
        this.peekToken = (LexerToken) null;
        return true;
      }
      LexerToken currentToken = this.CurrentToken;
      int? currentType = new int?();
      StringBuilder stringBuilder = (StringBuilder) null;
      bool flag = false;
      int num1;
      while (!flag && (num1 = this.reader.Peek()) >= 0)
      {
        char nextChar = (char) num1;
        int type;
        flag = this.MatchTokenType(nextChar, currentType, out type);
        if (!currentType.HasValue)
        {
          currentType = new int?(type);
          stringBuilder = new StringBuilder();
          stringBuilder.Append(nextChar);
          this.reader.Read();
        }
        else
        {
          int? nullable = currentType;
          int num2 = type;
          if (nullable.GetValueOrDefault() == num2 & nullable.HasValue)
          {
            stringBuilder.Append(nextChar);
            this.reader.Read();
          }
          else
            flag = true;
        }
      }
      if (currentType.HasValue)
        this.currentToken = new LexerToken()
        {
          Text = stringBuilder.ToString(),
          Type = currentType.Value
        };
      return currentToken != this.currentToken;
    }

    protected abstract bool MatchTokenType(char nextChar, int? currentType, out int type);
  }
}
