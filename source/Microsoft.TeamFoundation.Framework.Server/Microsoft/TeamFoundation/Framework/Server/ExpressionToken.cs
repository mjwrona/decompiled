// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ExpressionToken
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ExpressionToken
  {
    private string m_tokenValue;
    private TokenType m_tokenType;

    public ExpressionToken(TokenType tokenType)
      : this(tokenType, (string) null)
    {
    }

    public ExpressionToken(TokenType tokenType, string tokenValue)
    {
      this.m_tokenType = tokenType;
      this.m_tokenValue = tokenValue;
    }

    public TokenType TokenType
    {
      get => this.m_tokenType;
      set => this.m_tokenType = value;
    }

    public string Value
    {
      get => this.m_tokenValue;
      set => this.m_tokenValue = value;
    }
  }
}
