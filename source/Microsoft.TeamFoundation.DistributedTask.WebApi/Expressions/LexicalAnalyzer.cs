// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.LexicalAnalyzer
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class LexicalAnalyzer
  {
    private readonly string m_expression;
    private readonly HashSet<string> m_extensionFunctions;
    private readonly HashSet<string> m_extensionNamedValues;
    private int m_index;
    private Token m_lastToken;
    private readonly bool m_allowKeyHyphens;

    public LexicalAnalyzer(
      string expression,
      IEnumerable<string> namedValues,
      IEnumerable<string> functions,
      bool allowKeywordHyphens)
    {
      this.m_expression = expression;
      this.m_extensionNamedValues = new HashSet<string>((IEnumerable<string>) ((object) namedValues ?? (object) Array.Empty<string>()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_extensionFunctions = new HashSet<string>((IEnumerable<string>) ((object) functions ?? (object) Array.Empty<string>()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_allowKeyHyphens = allowKeywordHyphens;
    }

    public bool TryGetNextToken(ref Token token)
    {
      while (this.m_index < this.m_expression.Length && char.IsWhiteSpace(this.m_expression[this.m_index]))
        ++this.m_index;
      if (this.m_index >= this.m_expression.Length)
      {
        token = (Token) null;
        return false;
      }
      char rawValue = this.m_expression[this.m_index];
      switch (rawValue)
      {
        case '\'':
          token = this.ReadStringToken();
          break;
        case '(':
          token = new Token(TokenKind.StartParameter, rawValue, this.m_index++);
          break;
        case ')':
          token = new Token(TokenKind.EndParameter, rawValue, this.m_index++);
          break;
        case '*':
          token = new Token(TokenKind.Wildcard, rawValue, this.m_index++);
          break;
        case ',':
          token = new Token(TokenKind.Separator, rawValue, this.m_index++);
          break;
        case '[':
          token = new Token(TokenKind.StartIndex, rawValue, this.m_index++);
          break;
        case ']':
          token = new Token(TokenKind.EndIndex, rawValue, this.m_index++);
          break;
        default:
          token = rawValue != '.' ? (rawValue == '-' || rawValue >= '0' && rawValue <= '9' ? this.ReadNumberOrVersionToken() : this.ReadKeywordToken(this.m_allowKeyHyphens)) : (this.m_lastToken == null || this.m_lastToken.Kind == TokenKind.Separator || this.m_lastToken.Kind == TokenKind.StartIndex || this.m_lastToken.Kind == TokenKind.StartParameter ? this.ReadNumberOrVersionToken() : new Token(TokenKind.Dereference, rawValue, this.m_index++));
          break;
      }
      this.m_lastToken = token;
      return true;
    }

    public bool TryPeekNextToken(ref Token token)
    {
      int index = this.m_index;
      Token lastToken = this.m_lastToken;
      int num = this.TryGetNextToken(ref token) ? 1 : 0;
      this.m_index = index;
      this.m_lastToken = lastToken;
      return num != 0;
    }

    private Token ReadNumberOrVersionToken()
    {
      int index = this.m_index;
      int num = 0;
      do
      {
        if (this.m_expression[this.m_index] == '.')
          ++num;
        ++this.m_index;
      }
      while (this.m_index < this.m_expression.Length && (!LexicalAnalyzer.TestWhitespaceOrPunctuation(this.m_expression[this.m_index]) || this.m_expression[this.m_index] == '.'));
      int length = this.m_index - index;
      string str = this.m_expression.Substring(index, length);
      if (num >= 2)
      {
        Version result;
        if (Version.TryParse(str, out result))
          return new Token(TokenKind.Version, str, index, (object) result);
      }
      else
      {
        Decimal result;
        if (Decimal.TryParse(str, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          return new Token(TokenKind.Number, str, index, (object) result);
      }
      return new Token(TokenKind.Unrecognized, str, index);
    }

    private Token ReadKeywordToken(bool allowHyphen)
    {
      int index = this.m_index;
      ++this.m_index;
      while (this.m_index < this.m_expression.Length && !LexicalAnalyzer.TestWhitespaceOrPunctuation(this.m_expression[this.m_index]))
        ++this.m_index;
      int length = this.m_index - index;
      string str = this.m_expression.Substring(index, length);
      if (LexicalAnalyzer.TestKeyword(str, allowHyphen))
      {
        if (this.m_lastToken != null && this.m_lastToken.Kind == TokenKind.Dereference)
          return new Token(TokenKind.PropertyName, str, index);
        if (str.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
          return new Token(TokenKind.Boolean, str, index, (object) true);
        if (str.Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase))
          return new Token(TokenKind.Boolean, str, index, (object) false);
        if (ExpressionConstants.WellKnownFunctions.ContainsKey(str))
          return new Token(TokenKind.WellKnownFunction, str, index);
        if (this.m_extensionNamedValues.Contains(str))
          return new Token(TokenKind.ExtensionNamedValue, str, index);
        if (this.m_extensionFunctions.Contains(str))
          return new Token(TokenKind.ExtensionFunction, str, index);
      }
      return new Token(TokenKind.UnknownKeyword, str, index);
    }

    private Token ReadStringToken()
    {
      int index = this.m_index;
      bool flag = false;
      StringBuilder stringBuilder = new StringBuilder();
      ++this.m_index;
      while (this.m_index < this.m_expression.Length)
      {
        char ch = this.m_expression[this.m_index++];
        if (ch == '\'')
        {
          if (this.m_index >= this.m_expression.Length || this.m_expression[this.m_index] != '\'')
          {
            flag = true;
            break;
          }
          ++this.m_index;
        }
        stringBuilder.Append(ch);
      }
      int length = this.m_index - index;
      string rawValue = this.m_expression.Substring(index, length);
      return flag ? new Token(TokenKind.String, rawValue, index, (object) stringBuilder.ToString()) : new Token(TokenKind.Unrecognized, rawValue, index);
    }

    private static bool TestKeyword(string str, bool allowHyphen)
    {
      if (string.IsNullOrEmpty(str))
        return false;
      char ch1 = str[0];
      if ((ch1 < 'a' || ch1 > 'z') && (ch1 < 'A' || ch1 > 'Z') && ch1 != '_')
        return false;
      for (int index = 1; index < str.Length; ++index)
      {
        char ch2 = str[index];
        if ((ch2 < 'a' || ch2 > 'z') && (ch2 < 'A' || ch2 > 'Z') && (ch2 < '0' || ch2 > '9') && ch2 != '_' && (!allowHyphen || ch2 != '-'))
          return false;
      }
      return true;
    }

    private static bool TestWhitespaceOrPunctuation(char c)
    {
      switch (c)
      {
        case '(':
        case ')':
        case ',':
        case '.':
        case '[':
        case ']':
          return true;
        default:
          return char.IsWhiteSpace(c);
      }
    }
  }
}
