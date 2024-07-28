// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FunctionParameterParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class FunctionParameterParser
  {
    internal static bool TrySplitFunctionParameters(
      this UriQueryExpressionParser parser,
      out ICollection<FunctionParameterToken> splitParameters)
    {
      return parser.TrySplitOperationParameters(ExpressionTokenKind.CloseParen, out splitParameters);
    }

    internal static bool TrySplitOperationParameters(
      string parenthesisExpression,
      ODataUriParserConfiguration configuration,
      out ICollection<FunctionParameterToken> splitParameters)
    {
      ExpressionLexer lexer = new ExpressionLexer(parenthesisExpression, true, false, true);
      bool flag = new UriQueryExpressionParser(configuration.Settings.FilterLimit, lexer).TrySplitOperationParameters(ExpressionTokenKind.End, out splitParameters);
      if (splitParameters.Select<FunctionParameterToken, string>((Func<FunctionParameterToken, string>) (t => t.ParameterName)).Distinct<string>().Count<string>() != splitParameters.Count)
        throw new ODataException(Strings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
      return flag;
    }

    private static bool TrySplitOperationParameters(
      this UriQueryExpressionParser parser,
      ExpressionTokenKind endTokenKind,
      out ICollection<FunctionParameterToken> splitParameters)
    {
      ExpressionLexer lexer = parser.Lexer;
      List<FunctionParameterToken> functionParameterTokenList = new List<FunctionParameterToken>();
      splitParameters = (ICollection<FunctionParameterToken>) functionParameterTokenList;
      ExpressionToken currentToken = lexer.CurrentToken;
      if (currentToken.Kind == endTokenKind)
        return true;
      if (currentToken.Kind != ExpressionTokenKind.Identifier || lexer.PeekNextToken().Kind != ExpressionTokenKind.Equal)
        return false;
      while (currentToken.Kind != endTokenKind)
      {
        lexer.ValidateToken(ExpressionTokenKind.Identifier);
        string identifier = lexer.CurrentToken.GetIdentifier();
        lexer.NextToken();
        lexer.ValidateToken(ExpressionTokenKind.Equal);
        lexer.NextToken();
        QueryToken expression = parser.ParseExpression();
        functionParameterTokenList.Add(new FunctionParameterToken(identifier, expression));
        currentToken = lexer.CurrentToken;
        if (currentToken.Kind == ExpressionTokenKind.Comma)
        {
          lexer.NextToken();
          currentToken = lexer.CurrentToken;
          if (currentToken.Kind == endTokenKind)
            throw new ODataException(Strings.ExpressionLexer_SyntaxError((object) lexer.Position, (object) lexer.ExpressionText));
        }
      }
      return true;
    }
  }
}
