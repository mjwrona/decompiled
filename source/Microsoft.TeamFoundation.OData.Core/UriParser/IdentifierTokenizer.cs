// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.IdentifierTokenizer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class IdentifierTokenizer
  {
    private readonly ExpressionLexer lexer;
    private readonly HashSet<string> parameters;
    private readonly IFunctionCallParser functionCallParser;

    public IdentifierTokenizer(HashSet<string> parameters, IFunctionCallParser functionCallParser)
    {
      ExceptionUtils.CheckArgumentNotNull<HashSet<string>>(parameters, nameof (parameters));
      ExceptionUtils.CheckArgumentNotNull<IFunctionCallParser>(functionCallParser, nameof (functionCallParser));
      this.lexer = functionCallParser.Lexer;
      this.parameters = parameters;
      this.functionCallParser = functionCallParser;
    }

    public QueryToken ParseIdentifier(QueryToken parent)
    {
      this.lexer.ValidateToken(ExpressionTokenKind.Identifier);
      QueryToken result;
      if (this.lexer.ExpandIdentifierAsFunction() && this.functionCallParser.TryParseIdentifierAsFunction(parent, out result))
        return result;
      return this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Dot ? (QueryToken) new DottedIdentifierToken(this.lexer.ReadDottedIdentifier(false), parent) : this.ParseMemberAccess(parent);
    }

    public QueryToken ParseMemberAccess(QueryToken instance)
    {
      if (this.lexer.CurrentToken.Text == "*")
        return this.ParseStarMemberAccess(instance);
      string identifier = this.lexer.CurrentToken.GetIdentifier();
      if (instance == null && this.parameters.Contains(identifier))
      {
        this.lexer.NextToken();
        return (QueryToken) new RangeVariableToken(identifier);
      }
      this.lexer.NextToken();
      return (QueryToken) new EndPathToken(identifier, instance);
    }

    public QueryToken ParseStarMemberAccess(QueryToken instance)
    {
      if (this.lexer.CurrentToken.Text != "*")
        throw IdentifierTokenizer.ParseError(Strings.UriQueryExpressionParser_CannotCreateStarTokenFromNonStar((object) this.lexer.CurrentToken.Text));
      this.lexer.NextToken();
      return (QueryToken) new StarToken(instance);
    }

    private static Exception ParseError(string message) => (Exception) new ODataException(message);
  }
}
