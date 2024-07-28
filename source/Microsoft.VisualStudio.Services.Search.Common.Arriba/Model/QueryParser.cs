// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Model.QueryParser
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Model
{
  public class QueryParser
  {
    [StaticSafe]
    private static readonly ISet<TokenType> s_allComparisonTokens = (ISet<TokenType>) new HashSet<TokenType>()
    {
      TokenType.CompareOperatorEquals,
      TokenType.CompareOperatorGreaterThan,
      TokenType.CompareOperatorGreaterThanOrEqual,
      TokenType.CompareOperatorLessThan,
      TokenType.CompareOperatorLessThanOrEqual,
      TokenType.CompareOperatorMatches,
      TokenType.CompareOperatorMatchesExact,
      TokenType.CompareOperatorNotEquals,
      TokenType.CompareOperatorStartsWith,
      TokenType.ProximityOperatorNear,
      TokenType.ProximityOperatorBefore,
      TokenType.ProximityOperatorAfter
    };
    [StaticSafe]
    private static IReadOnlyDictionary<TokenType, Operator> s_comparisonTokenToOperatorMap = (IReadOnlyDictionary<TokenType, Operator>) new Dictionary<TokenType, Operator>()
    {
      [TokenType.CompareOperatorEquals] = Operator.Equals,
      [TokenType.CompareOperatorStartsWith] = Operator.StartsWith,
      [TokenType.CompareOperatorGreaterThan] = Operator.GreaterThan,
      [TokenType.CompareOperatorGreaterThanOrEqual] = Operator.GreaterThanOrEqual,
      [TokenType.CompareOperatorLessThan] = Operator.LessThan,
      [TokenType.CompareOperatorLessThanOrEqual] = Operator.LessThanOrEqual,
      [TokenType.CompareOperatorMatches] = Operator.Matches,
      [TokenType.CompareOperatorMatchesExact] = Operator.MatchesExact,
      [TokenType.CompareOperatorNotEquals] = Operator.NotEquals,
      [TokenType.ProximityOperatorNear] = Operator.Near,
      [TokenType.ProximityOperatorBefore] = Operator.Before,
      [TokenType.ProximityOperatorAfter] = Operator.After
    };
    [StaticSafe]
    private static readonly ISet<TokenType> s_endTokensForBracketEscapedType = (ISet<TokenType>) new HashSet<TokenType>()
    {
      TokenType.DoubleQuote,
      TokenType.RightBracket
    };
    internal QueryScanner m_scanner;
    private ISet<TokenType> m_supportedComparisonOperators;

    public QueryParser()
    {
      this.m_scanner = new QueryScanner();
      this.m_supportedComparisonOperators = QueryParser.s_allComparisonTokens;
    }

    public QueryParser(ISet<string> supportedOperators)
    {
      this.m_scanner = new QueryScanner(supportedOperators);
      this.m_supportedComparisonOperators = (ISet<TokenType>) new HashSet<TokenType>();
      foreach (string supportedOperator in (IEnumerable<string>) supportedOperators)
      {
        TokenType allLiteralToken = QueryScanner.AllLiteralTokens[supportedOperator];
        if (QueryParser.s_allComparisonTokens.Contains(allLiteralToken))
          this.m_supportedComparisonOperators.Add(allLiteralToken);
      }
    }

    public IExpression Parse(string whereClause)
    {
      if (string.IsNullOrEmpty(whereClause))
        return (IExpression) new EmptyExpression();
      using (StringReader reader = new StringReader(whereClause))
        this.Reset((TextReader) reader);
      return this.ParseQuery() ?? (IExpression) new EmptyExpression();
    }

    private void Reset(TextReader reader)
    {
      this.m_scanner.Reset(reader);
      this.m_scanner.Next();
    }

    private IExpression ParseQuery()
    {
      if (this.m_scanner.Current.Type == TokenType.End)
        return (IExpression) new EmptyExpression();
      List<IExpression> terms = new List<IExpression>();
      this.ParseAndExpressionAndAddTo(terms);
      while (this.m_scanner.Current.Type == TokenType.BooleanOperatorOr)
      {
        this.m_scanner.Next();
        this.ParseAndExpressionAndAddTo(terms);
      }
      return terms.Count == 1 ? terms[0] : (IExpression) new OrExpression(terms.ToArray());
    }

    private IExpression ParseAndExpression()
    {
      List<IExpression> expressionList = new List<IExpression>();
      IExpression term1 = this.ParseTerm();
      if (term1 == null)
        return (IExpression) new EmptyExpression();
      expressionList.Add(term1);
      while (this.m_scanner.Current.Type != TokenType.End && this.m_scanner.Current.Type != TokenType.BooleanOperatorOr)
      {
        if (this.m_scanner.Current.Type == TokenType.BooleanOperatorAnd)
          this.m_scanner.Next();
        IExpression term2 = this.ParseTerm();
        if (term2 != null)
          expressionList.Add(term2);
        else
          break;
      }
      return expressionList.Count == 1 ? expressionList[0] : (IExpression) new AndExpression(expressionList.ToArray());
    }

    private IExpression ParseTerm()
    {
      bool flag = false;
      if (this.m_scanner.Current.Type == TokenType.UnaryOperatorNot)
      {
        flag = true;
        this.m_scanner.Next();
      }
      IExpression part;
      if (this.m_scanner.Current.Type == TokenType.LeftParenthesis)
      {
        this.m_scanner.Next();
        part = this.ParseQuery();
        if (this.m_scanner.Current.Type == TokenType.RightParenthesis)
          this.m_scanner.Next();
      }
      else if (this.m_scanner.Current.Type == TokenType.LeftBracket)
        part = this.ParseTermExpressionOperatorAndValue(this.ParseNotCompareOperatorNorEndTokens(QueryParser.s_endTokensForBracketEscapedType));
      else if (this.m_scanner.Current.Type == TokenType.DoubleQuote)
        part = this.ParseTermExpressionOperatorAndValue(this.ParseUntilEndToken(TokenType.DoubleQuote));
      else if (this.IsCompareOperator(this.m_scanner.Current.Type))
      {
        this.m_scanner.Next();
        string str = this.ParseValue();
        if (string.IsNullOrEmpty(str))
          return (IExpression) null;
        part = (IExpression) new TermExpression("*", Operator.Matches, str);
      }
      else
      {
        string norCompareOperator = this.ParseNotSpaceParenthesisNorCompareOperator();
        if (string.IsNullOrEmpty(norCompareOperator))
          return (IExpression) null;
        part = this.ParseTermExpressionOperatorAndValue(norCompareOperator);
      }
      if (flag && part != null)
        part = (IExpression) new NotExpression(part);
      return part;
    }

    private IExpression ParseTermExpressionOperatorAndValue(string columnName)
    {
      IExpression operatorAndValue;
      if (!this.IsCompareOperator(this.m_scanner.Current.Type))
      {
        operatorAndValue = (IExpression) new TermExpression("*", Operator.Matches, columnName);
      }
      else
      {
        Operator op = this.ConvertToOperator(this.m_scanner.Current.Type);
        this.m_scanner.Next();
        string str = this.ParseValue();
        operatorAndValue = str == null ? (string.IsNullOrWhiteSpace(columnName) ? (IExpression) new EmptyExpression() : (IExpression) new TermExpression("*", Operator.Matches, columnName)) : (string.IsNullOrWhiteSpace(columnName) ? (IExpression) new TermExpression("*", Operator.Matches, str) : (IExpression) new TermExpression(columnName, op, str));
      }
      return operatorAndValue;
    }

    private string ParseValue()
    {
      if (this.m_scanner.Current.Type == TokenType.DoubleQuote)
        return this.ParseUntilEndToken(TokenType.DoubleQuote);
      string spaceNorParenthesis = this.ParseNotSpaceNorParenthesis();
      return string.IsNullOrWhiteSpace(spaceNorParenthesis) ? (string) null : spaceNorParenthesis;
    }

    private string ParseNotSpaceParenthesisNorCompareOperator()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (bool flag = true; this.m_scanner.Current.Type != TokenType.End && (flag || this.m_scanner.Current.Prefix.Length <= 0) && !this.IsParenthesis(this.m_scanner.Current.Type) && !this.IsCompareOperator(this.m_scanner.Current.Type); flag = false)
      {
        stringBuilder.Append(this.m_scanner.Current.Content);
        this.m_scanner.Next();
      }
      return stringBuilder.ToString();
    }

    private string ParseNotSpaceNorParenthesis()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (bool flag = true; this.m_scanner.Current.Type != TokenType.End && (flag || this.m_scanner.Current.Prefix.Length <= 0) && !this.IsParenthesis(this.m_scanner.Current.Type); flag = false)
      {
        stringBuilder.Append(this.m_scanner.Current.Content);
        this.m_scanner.Next();
      }
      return stringBuilder.ToString();
    }

    internal virtual string ParseUntilEndToken(TokenType endToken)
    {
      StringBuilder stringBuilder = new StringBuilder();
      this.m_scanner.Next();
      while (this.m_scanner.Current.Type != TokenType.End)
      {
        if (this.m_scanner.Current.Type == endToken)
        {
          stringBuilder.Append(this.m_scanner.Current.Prefix);
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

    private string ParseNotCompareOperatorNorEndTokens(ISet<TokenType> endTokens)
    {
      StringBuilder stringBuilder = new StringBuilder();
      this.m_scanner.Next();
      while (this.m_scanner.Current.Type != TokenType.End)
      {
        if (endTokens.Contains(this.m_scanner.Current.Type))
        {
          stringBuilder.Append(this.m_scanner.Current.Prefix);
          this.m_scanner.Next();
          if (endTokens.Contains(this.m_scanner.Current.Type) && this.m_scanner.Current.Prefix.Length == 0)
            stringBuilder.Append(this.m_scanner.Current.Content);
          else
            break;
        }
        else if (!this.IsCompareOperator(this.m_scanner.Current.Type))
        {
          stringBuilder.Append(this.m_scanner.Current.Prefix);
          stringBuilder.Append(this.m_scanner.Current.Content);
        }
        else
          break;
        this.m_scanner.Next();
      }
      return stringBuilder.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ParseAndExpressionAndAddTo(List<IExpression> terms)
    {
      IExpression andExpression = this.ParseAndExpression();
      if (andExpression is EmptyExpression)
        return;
      terms.Add(andExpression);
    }

    private bool IsCompareOperator(TokenType type) => this.m_supportedComparisonOperators.Contains(type);

    private Operator ConvertToOperator(TokenType type) => QueryParser.s_comparisonTokenToOperatorMap[type];

    private bool IsParenthesis(TokenType type) => type == TokenType.LeftParenthesis || type == TokenType.RightParenthesis;
  }
}
