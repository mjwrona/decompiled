// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueryExpression
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class QueryExpression
  {
    private string m_expression;
    private HashSet<string> m_extendedComparers;
    private ExpressionToken m_currentToken = new ExpressionToken(TokenType.UnknownToken, string.Empty);
    private int m_parseIndex;
    private StringBuilder m_tokenValue = new StringBuilder();
    private List<ExpressionToken> m_tokenList = new List<ExpressionToken>();
    private static TokenType[] s_comparerTypes = new TokenType[5]
    {
      TokenType.LessThan,
      TokenType.LessThanEqual,
      TokenType.Equal,
      TokenType.GreaterThan,
      TokenType.GreaterThanEqual
    };
    private static TokenType[] s_valueTypes = new TokenType[4]
    {
      TokenType.DecNumericToken,
      TokenType.HexNumericToken,
      TokenType.StringToken,
      TokenType.QuotedStringToken
    };

    public QueryExpression(string expression)
      : this(expression, (IEnumerable<string>) null)
    {
    }

    public QueryExpression(string expression, IEnumerable<string> extendedComparers)
    {
      this.m_expression = expression;
      if (extendedComparers != null)
        this.m_extendedComparers = new HashSet<string>(extendedComparers, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.NextToken();
      this.Rule_Expression();
      this.Match(TokenType.EndOfExpression);
    }

    public IEnumerable<ExpressionToken> Tokens => (IEnumerable<ExpressionToken>) this.m_tokenList;

    private void Rule_Expression()
    {
      do
      {
        if (this.m_currentToken.TokenType == TokenType.OpenParen)
        {
          this.m_tokenList.Add(this.Match(TokenType.OpenParen));
          this.Rule_Expression();
          this.m_tokenList.Add(this.Match(TokenType.CloseParen));
        }
        else
          this.Rule_Comparison();
        TokenType tokenType;
        if ((tokenType = this.Junction()) != TokenType.UnknownToken)
        {
          this.Match(TokenType.StringToken);
          this.m_tokenList.Add(new ExpressionToken(tokenType));
        }
      }
      while (this.m_currentToken.TokenType != TokenType.CloseParen && this.m_currentToken.TokenType != TokenType.EndOfExpression);
    }

    private void Rule_Comparison()
    {
      this.m_tokenList.Add(this.Match(TokenType.StringToken));
      if (this.m_extendedComparers != null && this.m_extendedComparers.Contains(this.m_currentToken.Value))
      {
        this.m_currentToken.TokenType = TokenType.ExtendedComparer;
        this.m_tokenList.Add(this.Match(TokenType.ExtendedComparer));
      }
      else
        this.m_tokenList.Add(this.Match((IEnumerable<TokenType>) QueryExpression.s_comparerTypes));
      this.m_tokenList.Add(this.Match((IEnumerable<TokenType>) QueryExpression.s_valueTypes));
    }

    private ExpressionToken Match(TokenType tokenType)
    {
      if (this.m_currentToken.TokenType != tokenType)
        throw new IncompatibleTokenException(tokenType.ToString(), this.m_currentToken.TokenType.ToString(), this.m_parseIndex);
      ExpressionToken currentToken = this.m_currentToken;
      this.NextToken();
      return currentToken;
    }

    private ExpressionToken Match(IEnumerable<TokenType> tokenTypes)
    {
      foreach (TokenType tokenType in tokenTypes)
      {
        if (this.m_currentToken.TokenType == tokenType)
        {
          ExpressionToken currentToken = this.m_currentToken;
          this.NextToken();
          return currentToken;
        }
      }
      StringBuilder stringBuilder = new StringBuilder();
      foreach (TokenType tokenType in tokenTypes)
      {
        stringBuilder.Append(tokenType.ToString());
        stringBuilder.Append(',');
      }
      throw new IncompatibleTokenException(stringBuilder.ToString(), this.m_currentToken.TokenType.ToString(), this.m_parseIndex);
    }

    private void NextToken()
    {
      this.m_currentToken = new ExpressionToken(TokenType.UnknownToken, string.Empty);
      this.m_tokenValue.Length = 0;
      while (this.m_parseIndex < this.m_expression.Length && char.IsWhiteSpace(this.m_expression[this.m_parseIndex]))
        ++this.m_parseIndex;
      if (this.m_parseIndex == this.m_expression.Length)
        this.m_currentToken.TokenType = TokenType.EndOfExpression;
      else if (this.m_expression[this.m_parseIndex] == '(')
      {
        ++this.m_parseIndex;
        this.m_currentToken.TokenType = TokenType.OpenParen;
      }
      else if (this.m_expression[this.m_parseIndex] == ')')
      {
        ++this.m_parseIndex;
        this.m_currentToken.TokenType = TokenType.CloseParen;
      }
      else if (this.m_expression[this.m_parseIndex] == '<')
      {
        ++this.m_parseIndex;
        if (this.m_parseIndex < this.m_expression.Length && this.m_expression[this.m_parseIndex] == '=')
        {
          ++this.m_parseIndex;
          this.m_currentToken.TokenType = TokenType.LessThanEqual;
          this.m_currentToken.Value = "<=";
        }
        else
        {
          this.m_currentToken.TokenType = TokenType.LessThan;
          this.m_currentToken.Value = "<";
        }
      }
      else if (this.m_expression[this.m_parseIndex] == '>')
      {
        ++this.m_parseIndex;
        if (this.m_parseIndex < this.m_expression.Length && this.m_expression[this.m_parseIndex] == '=')
        {
          ++this.m_parseIndex;
          this.m_currentToken.TokenType = TokenType.GreaterThanEqual;
          this.m_currentToken.Value = ">=";
        }
        else
        {
          this.m_currentToken.TokenType = TokenType.GreaterThan;
          this.m_currentToken.Value = ">";
        }
      }
      else if (this.m_expression[this.m_parseIndex] == '=')
      {
        ++this.m_parseIndex;
        if (this.m_parseIndex >= this.m_expression.Length || this.m_expression[this.m_parseIndex] != '=')
          throw new InvalidTokenFormatExpcetion("=" + this.m_expression[this.m_parseIndex].ToString(), this.m_parseIndex);
        ++this.m_parseIndex;
        this.m_currentToken.TokenType = TokenType.Equal;
        this.m_currentToken.Value = "=";
      }
      else
      {
        bool flag = false;
        while (this.m_parseIndex < this.m_expression.Length)
        {
          char c = this.m_expression[this.m_parseIndex++];
          if (flag || !char.IsWhiteSpace(c))
          {
            if (this.m_currentToken.TokenType == TokenType.QuotedStringToken && !flag)
              throw new InvalidTokenFormatExpcetion(this.m_tokenValue.ToString() + c.ToString(), this.m_parseIndex);
            switch (c)
            {
              case '\'':
                if (flag)
                {
                  goto label_54;
                }
                else
                {
                  flag = true;
                  this.m_currentToken.TokenType = TokenType.QuotedStringToken;
                  continue;
                }
              case '\\':
                if (!flag || this.m_parseIndex == this.m_expression.Length)
                  throw new InvalidEscapeSequenceException(this.m_parseIndex);
                this.m_tokenValue.Append(this.m_expression[this.m_parseIndex++]);
                continue;
              default:
                if (flag)
                {
                  this.m_tokenValue.Append(c);
                  continue;
                }
                if (c == '(' || c == ')')
                {
                  --this.m_parseIndex;
                  goto label_54;
                }
                else
                {
                  if (char.IsNumber(c))
                  {
                    if (this.m_tokenValue.Length == 0 && this.m_currentToken.TokenType == TokenType.UnknownToken)
                    {
                      if (this.m_parseIndex < this.m_expression.Length && (this.m_expression[this.m_parseIndex] == 'x' || this.m_expression[this.m_parseIndex] == 'X'))
                      {
                        ++this.m_parseIndex;
                        this.m_currentToken.TokenType = TokenType.HexNumericToken;
                        continue;
                      }
                      this.m_tokenValue.Append(c);
                      this.m_currentToken.TokenType = TokenType.DecNumericToken;
                      continue;
                    }
                    this.m_tokenValue.Append(c);
                    continue;
                  }
                  if (!char.IsLetter(c) && c != '.' && c != '_' && c != '-')
                    throw new InvalidTokenFormatExpcetion(this.m_tokenValue.ToString() + c.ToString(), this.m_parseIndex);
                  if (this.m_currentToken.TokenType == TokenType.DecNumericToken)
                    throw new InvalidTokenFormatExpcetion(this.m_tokenValue.ToString() + c.ToString(), this.m_parseIndex);
                  if (this.m_currentToken.TokenType == TokenType.HexNumericToken)
                  {
                    if ((c < 'a' || c > 'f') && (c < 'A' || c > 'F'))
                      throw new InvalidTokenFormatExpcetion(this.m_tokenValue.ToString() + c.ToString(), this.m_parseIndex);
                    this.m_tokenValue.Append(c);
                    continue;
                  }
                  if (this.m_currentToken.TokenType == TokenType.UnknownToken)
                    this.m_currentToken.TokenType = TokenType.StringToken;
                  this.m_tokenValue.Append(c);
                  continue;
                }
            }
          }
          else
            break;
        }
      }
label_54:
      if (this.m_tokenValue.Length <= 0)
        return;
      this.m_currentToken.Value = this.m_tokenValue.ToString();
    }

    private TokenType Junction()
    {
      if (this.m_currentToken.Value.Equals("and", StringComparison.OrdinalIgnoreCase))
        return TokenType.And;
      return this.m_currentToken.Value.Equals("or", StringComparison.OrdinalIgnoreCase) ? TokenType.Or : TokenType.UnknownToken;
    }
  }
}
