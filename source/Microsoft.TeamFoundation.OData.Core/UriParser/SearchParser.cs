// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SearchParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.UriParser
{
  internal sealed class SearchParser
  {
    private readonly int maxDepth;
    private int recursionDepth;
    private ExpressionLexer lexer;

    internal SearchParser(int maxDepth) => this.maxDepth = maxDepth;

    internal QueryToken ParseSearch(string expressionText)
    {
      this.recursionDepth = 0;
      this.lexer = (ExpressionLexer) new SearchLexer(expressionText);
      QueryToken expression = this.ParseExpression();
      this.lexer.ValidateToken(ExpressionTokenKind.End);
      return expression;
    }

    private static Exception ParseError(string message) => (Exception) new ODataException(message);

    private QueryToken ParseExpression()
    {
      this.RecurseEnter();
      QueryToken logicalOr = this.ParseLogicalOr();
      this.RecurseLeave();
      return logicalOr;
    }

    private QueryToken ParseLogicalOr()
    {
      this.RecurseEnter();
      QueryToken left = this.ParseLogicalAnd();
      while (this.TokenIdentifierIs("OR"))
      {
        this.lexer.NextToken();
        QueryToken logicalAnd = this.ParseLogicalAnd();
        left = (QueryToken) new BinaryOperatorToken(BinaryOperatorKind.Or, left, logicalAnd);
      }
      this.RecurseLeave();
      return left;
    }

    private QueryToken ParseLogicalAnd()
    {
      this.RecurseEnter();
      QueryToken left = this.ParseUnary();
      while (this.TokenIdentifierIs("AND") || this.TokenIdentifierIs("NOT") || this.lexer.CurrentToken.Kind == ExpressionTokenKind.StringLiteral || this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
      {
        if (this.TokenIdentifierIs("AND"))
          this.lexer.NextToken();
        QueryToken unary = this.ParseUnary();
        left = (QueryToken) new BinaryOperatorToken(BinaryOperatorKind.And, left, unary);
      }
      this.RecurseLeave();
      return left;
    }

    private QueryToken ParseUnary()
    {
      this.RecurseEnter();
      if (this.TokenIdentifierIs("NOT"))
      {
        this.lexer.NextToken();
        QueryToken unary = this.ParseUnary();
        this.RecurseLeave();
        return (QueryToken) new UnaryOperatorToken(UnaryOperatorKind.Not, unary);
      }
      this.RecurseLeave();
      return this.ParsePrimary();
    }

    private QueryToken ParsePrimary()
    {
      this.RecurseEnter();
      QueryToken primary;
      switch (this.lexer.CurrentToken.Kind)
      {
        case ExpressionTokenKind.StringLiteral:
          primary = (QueryToken) new StringLiteralToken(this.lexer.CurrentToken.Text);
          this.lexer.NextToken();
          break;
        case ExpressionTokenKind.OpenParen:
          primary = this.ParseParenExpression();
          break;
        default:
          throw new ODataException(Strings.UriQueryExpressionParser_ExpressionExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      }
      this.RecurseLeave();
      return primary;
    }

    private QueryToken ParseParenExpression()
    {
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
        throw SearchParser.ParseError(Strings.UriQueryExpressionParser_OpenParenExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      QueryToken expression = this.ParseExpression();
      if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
        throw SearchParser.ParseError(Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected((object) this.lexer.CurrentToken.Position, (object) this.lexer.ExpressionText));
      this.lexer.NextToken();
      return expression;
    }

    private bool TokenIdentifierIs(string id) => this.lexer.CurrentToken.IdentifierIs(id, false);

    private void RecurseEnter()
    {
      ++this.recursionDepth;
      if (this.recursionDepth > this.maxDepth)
        throw new ODataException(Strings.UriQueryExpressionParser_TooDeep);
    }

    private void RecurseLeave() => --this.recursionDepth;
  }
}
