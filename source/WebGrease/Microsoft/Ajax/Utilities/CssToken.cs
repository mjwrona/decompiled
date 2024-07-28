// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CssToken
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.ComponentModel;

namespace Microsoft.Ajax.Utilities
{
  internal class CssToken
  {
    private TokenType m_tokenType;
    private string m_text;
    private CssContext m_context;

    public TokenType TokenType => this.m_tokenType;

    public string Text => this.m_text;

    public CssContext Context => this.m_context;

    public CssToken(TokenType tokenType, [Localizable(false)] string text, CssContext context)
    {
      this.m_tokenType = tokenType;
      this.m_text = text;
      this.m_context = context.Clone();
    }

    public CssToken(TokenType tokenType, char ch, CssContext context)
      : this(tokenType, new string(ch, 1), context)
    {
    }
  }
}
