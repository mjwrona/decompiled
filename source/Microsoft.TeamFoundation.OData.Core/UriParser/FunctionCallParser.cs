// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FunctionCallParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal sealed class FunctionCallParser : IFunctionCallParser
  {
    private readonly ExpressionLexer lexer;
    private readonly UriQueryExpressionParser parser;
    private readonly bool restoreStateIfFail;

    public FunctionCallParser(ExpressionLexer lexer, UriQueryExpressionParser parser)
      : this(lexer, parser, false)
    {
    }

    public FunctionCallParser(
      ExpressionLexer lexer,
      UriQueryExpressionParser parser,
      bool restoreStateIfFail)
    {
      ExceptionUtils.CheckArgumentNotNull<ExpressionLexer>(lexer, nameof (lexer));
      ExceptionUtils.CheckArgumentNotNull<UriQueryExpressionParser>(parser, nameof (parser));
      this.lexer = lexer;
      this.parser = parser;
      this.restoreStateIfFail = restoreStateIfFail;
    }

    public UriQueryExpressionParser UriQueryExpressionParser => this.parser;

    public ExpressionLexer Lexer => this.lexer;

    public bool TryParseIdentifierAsFunction(QueryToken parent, out QueryToken result)
    {
      result = (QueryToken) null;
      ExpressionLexer.ExpressionLexerPosition position = this.lexer.SnapshotPosition();
      string name;
      if (this.Lexer.PeekNextToken().Kind == ExpressionTokenKind.Dot)
      {
        name = this.Lexer.ReadDottedIdentifier(false);
      }
      else
      {
        name = this.Lexer.CurrentToken.Text;
        this.Lexer.NextToken();
      }
      FunctionParameterToken[] listOrEntityKeyList = this.ParseArgumentListOrEntityKeyList((Action) (() => this.lexer.RestorePosition(position)));
      if (listOrEntityKeyList != null)
        result = (QueryToken) new FunctionCallToken(name, (IEnumerable<FunctionParameterToken>) listOrEntityKeyList, parent);
      return result != null;
    }

    public FunctionParameterToken[] ParseArgumentListOrEntityKeyList(Action restoreAction = null)
    {
      if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
      {
        if (!this.restoreStateIfFail || restoreAction == null)
          throw new ODataException(Strings.UriQueryExpressionParser_OpenParenExpected((object) this.Lexer.CurrentToken.Position, (object) this.Lexer.ExpressionText));
        restoreAction();
        return (FunctionParameterToken[]) null;
      }
      this.Lexer.NextToken();
      FunctionParameterToken[] listOrEntityKeyList = this.Lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen ? this.ParseArguments() : FunctionParameterToken.EmptyParameterList;
      if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
      {
        if (!this.restoreStateIfFail || restoreAction == null)
          throw new ODataException(Strings.UriQueryExpressionParser_CloseParenOrCommaExpected((object) this.Lexer.CurrentToken.Position, (object) this.Lexer.ExpressionText));
        restoreAction();
        return (FunctionParameterToken[]) null;
      }
      this.Lexer.NextToken();
      return listOrEntityKeyList;
    }

    public FunctionParameterToken[] ParseArguments()
    {
      ICollection<FunctionParameterToken> argList;
      return this.TryReadArgumentsAsNamedValues(out argList) ? argList.ToArray<FunctionParameterToken>() : this.ReadArgumentsAsPositionalValues().ToArray();
    }

    private List<FunctionParameterToken> ReadArgumentsAsPositionalValues()
    {
      List<FunctionParameterToken> functionParameterTokenList = new List<FunctionParameterToken>();
      while (true)
      {
        functionParameterTokenList.Add(new FunctionParameterToken((string) null, this.parser.ParseExpression()));
        if (this.Lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
          this.Lexer.NextToken();
        else
          break;
      }
      return functionParameterTokenList;
    }

    private bool TryReadArgumentsAsNamedValues(out ICollection<FunctionParameterToken> argList)
    {
      if (!this.parser.TrySplitFunctionParameters(out argList))
        return false;
      if (argList.Select<FunctionParameterToken, string>((Func<FunctionParameterToken, string>) (t => t.ParameterName)).Distinct<string>().Count<string>() != argList.Count)
        throw new ODataException(Strings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
      return true;
    }
  }
}
