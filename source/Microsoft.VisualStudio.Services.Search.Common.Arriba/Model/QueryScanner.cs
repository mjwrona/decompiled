// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Model.QueryScanner
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Model
{
  internal class QueryScanner
  {
    [StaticSafe]
    internal static IReadOnlyDictionary<string, TokenType> AllLiteralTokens = (IReadOnlyDictionary<string, TokenType>) new Dictionary<string, TokenType>()
    {
      ["STARTSWITH"] = TokenType.CompareOperatorStartsWith,
      ["MATCHEXACT"] = TokenType.CompareOperatorMatchesExact,
      ["FREETEXT"] = TokenType.CompareOperatorMatches,
      ["CONTAINS"] = TokenType.CompareOperatorMatches,
      ["UNDER"] = TokenType.CompareOperatorStartsWith,
      ["MATCH"] = TokenType.CompareOperatorMatches,
      ["LIKE"] = TokenType.CompareOperatorMatches,
      ["NOT"] = TokenType.UnaryOperatorNot,
      ["AND"] = TokenType.BooleanOperatorAnd,
      ["OR"] = TokenType.BooleanOperatorOr,
      ["NEAR"] = TokenType.ProximityOperatorNear,
      ["BEFORE"] = TokenType.ProximityOperatorBefore,
      ["AFTER"] = TokenType.ProximityOperatorAfter,
      ["&&"] = TokenType.BooleanOperatorAnd,
      ["||"] = TokenType.BooleanOperatorOr,
      ["::"] = TokenType.CompareOperatorMatchesExact,
      ["|>"] = TokenType.CompareOperatorStartsWith,
      ["<="] = TokenType.CompareOperatorLessThanOrEqual,
      [">="] = TokenType.CompareOperatorGreaterThanOrEqual,
      ["=="] = TokenType.CompareOperatorEquals,
      ["!="] = TokenType.CompareOperatorNotEquals,
      ["<>"] = TokenType.CompareOperatorNotEquals,
      ["("] = TokenType.LeftParenthesis,
      [")"] = TokenType.RightParenthesis,
      ["["] = TokenType.LeftBracket,
      ["]"] = TokenType.RightBracket,
      ["\""] = TokenType.DoubleQuote,
      ["\\\""] = TokenType.EscapedDoubleQuote,
      ["-"] = TokenType.UnaryOperatorNot,
      ["!"] = TokenType.UnaryOperatorNot,
      ["&"] = TokenType.BooleanOperatorAnd,
      ["|"] = TokenType.BooleanOperatorOr,
      [":"] = TokenType.CompareOperatorMatches,
      ["<"] = TokenType.CompareOperatorLessThan,
      [">"] = TokenType.CompareOperatorGreaterThan,
      ["="] = TokenType.CompareOperatorEquals
    };
    private List<QueryScanner.TokenNameMap> m_supportedLiterals;
    private HashSet<char> m_supportedNonIdentifiers;
    private string m_text;
    private int m_currentIndex;
    private int m_lineNumber;
    private int m_charInLine;

    public QueryScanner()
      : this((ISet<string>) new HashSet<string>(QueryScanner.AllLiteralTokens.Keys))
    {
    }

    public QueryScanner(ISet<string> supportedTokens)
    {
      this.m_supportedLiterals = new List<QueryScanner.TokenNameMap>(supportedTokens.Count);
      this.m_supportedNonIdentifiers = new HashSet<char>();
      foreach (string supportedToken in (IEnumerable<string>) supportedTokens)
      {
        this.m_supportedLiterals.Add(new QueryScanner.TokenNameMap(supportedToken, QueryScanner.AllLiteralTokens[supportedToken]));
        char c = supportedToken[0];
        if (!char.IsLetter(c))
          this.m_supportedNonIdentifiers.Add(c);
      }
      this.m_supportedLiterals.Sort((Comparison<QueryScanner.TokenNameMap>) ((m1, m2) => -m1.Name.Length.CompareTo(m2.Name.Length)));
    }

    public void Reset(TextReader reader)
    {
      this.m_text = reader.ReadToEnd().Trim();
      this.m_currentIndex = 0;
      this.m_lineNumber = 1;
      this.m_charInLine = 0;
    }

    public Token Current { get; private set; }

    public bool Next()
    {
      this.UpdatePosition();
      this.Current = new Token();
      this.ReadWhitespace();
      if (this.m_currentIndex >= this.m_text.Length)
      {
        this.Current.Content = string.Empty;
        this.Current.Type = TokenType.End;
        return false;
      }
      bool flag = false;
      if (!flag)
        flag = this.ReadLiteral();
      if (!flag)
        flag = this.ReadIdentifier();
      if (!flag)
      {
        this.Current.Content = this.m_text.Substring(this.m_currentIndex, 1);
        this.Current.Type = TokenType.Other;
        ++this.m_currentIndex;
      }
      return true;
    }

    private bool ReadWhitespace()
    {
      int length = 0;
      while (this.m_currentIndex + length < this.m_text.Length && char.IsWhiteSpace(this.m_text[this.m_currentIndex + length]))
        ++length;
      if (length > 0)
      {
        this.Current.Prefix = this.m_text.Substring(this.m_currentIndex, length);
        this.m_currentIndex += length;
        return true;
      }
      this.Current.Prefix = string.Empty;
      return false;
    }

    private bool ReadLiteral()
    {
      int num = this.m_text.Length - this.m_currentIndex;
      for (int index1 = 0; index1 < this.m_supportedLiterals.Count; ++index1)
      {
        string name = this.m_supportedLiterals[index1].Name;
        if (num >= name.Length)
        {
          bool flag = true;
          int index2;
          for (index2 = 0; index2 < name.Length && (int) this.m_text[this.m_currentIndex + index2] == (int) name[index2]; ++index2)
          {
            if (!char.IsLetter(name[index2]))
              flag = false;
          }
          if (index2 == name.Length)
          {
            if (flag)
            {
              if (num != name.Length)
              {
                char c = this.m_text[this.m_currentIndex + name.Length];
                if (!char.IsWhiteSpace(c) && c != '(')
                  continue;
              }
              else
                continue;
            }
            this.Current.Type = this.m_supportedLiterals[index1].Type;
            this.Current.Content = this.m_text.Substring(this.m_currentIndex, name.Length);
            this.m_currentIndex += name.Length;
            return true;
          }
        }
      }
      return false;
    }

    private bool ReadIdentifier()
    {
      int length;
      for (length = 0; this.m_currentIndex + length < this.m_text.Length; ++length)
      {
        char c = this.m_text[this.m_currentIndex + length];
        if (char.IsWhiteSpace(c) || this.m_supportedNonIdentifiers.Contains(c))
          break;
      }
      if (length <= 0)
        return false;
      this.Current.Type = TokenType.Identifier;
      this.Current.Content = this.m_text.Substring(this.m_currentIndex, length);
      this.m_currentIndex += length;
      return true;
    }

    private void UpdatePosition()
    {
      if (this.Current == null)
        return;
      this.UpdatePosition(this.Current.Prefix);
      this.UpdatePosition(this.Current.Content);
    }

    private void UpdatePosition(string value)
    {
      if (string.IsNullOrEmpty(value))
        return;
      foreach (int num in value)
      {
        ++this.m_charInLine;
        if (num == 10)
        {
          ++this.m_lineNumber;
          this.m_charInLine = 1;
        }
      }
    }

    private class TokenNameMap
    {
      public TokenNameMap(string name, TokenType tokenType)
      {
        this.Name = name;
        this.Type = tokenType;
      }

      public string Name { get; private set; }

      public TokenType Type { get; private set; }
    }
  }
}
