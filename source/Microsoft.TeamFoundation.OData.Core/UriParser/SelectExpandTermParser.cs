// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandTermParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class SelectExpandTermParser
  {
    private readonly ExpressionLexer lexer;
    private readonly int maxPathLength;
    private readonly bool isSelect;

    internal SelectExpandTermParser(ExpressionLexer lexer, int maxPathLength, bool isSelect)
    {
      this.lexer = lexer;
      this.maxPathLength = maxPathLength;
      this.isSelect = isSelect;
    }

    internal PathSegmentToken ParseTerm(bool allowRef = false)
    {
      PathSegmentToken segment = this.ParseSegment((PathSegmentToken) null, allowRef);
      if (segment == null)
        return (PathSegmentToken) null;
      int pathLength = 1;
      this.CheckPathLength(pathLength);
      while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Slash)
      {
        this.lexer.NextToken();
        if (pathLength <= 1 || this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
        {
          segment = this.ParseSegment(segment, allowRef);
          if (segment != null)
            this.CheckPathLength(++pathLength);
        }
        else
          break;
      }
      return segment;
    }

    private void CheckPathLength(int pathLength)
    {
      if (pathLength > this.maxPathLength)
        throw new ODataException(Strings.UriQueryExpressionParser_TooDeep);
    }

    private PathSegmentToken ParseSegment(PathSegmentToken previousSegment, bool allowRef)
    {
      if (this.lexer.CurrentToken.Text.StartsWith("$", StringComparison.Ordinal) && (!allowRef || this.lexer.CurrentToken.Text != "$ref"))
        throw new ODataException(Strings.UriSelectParser_SystemTokenInSelectExpand((object) this.lexer.CurrentToken.Text, (object) this.lexer.ExpressionText));
      if (!this.isSelect)
      {
        if (previousSegment != null && previousSegment.Identifier == "*" && this.lexer.CurrentToken.GetIdentifier() != "$ref")
          throw new ODataException(Strings.ExpressionToken_OnlyRefAllowWithStarInExpand);
        if (previousSegment != null && previousSegment.Identifier == "$ref")
          throw new ODataException(Strings.ExpressionToken_NoPropAllowedAfterRef);
      }
      string identifier;
      if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Dot)
        identifier = this.lexer.ReadDottedIdentifier(this.isSelect);
      else if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Star)
      {
        if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash && this.isSelect)
          throw new ODataException(Strings.ExpressionToken_IdentifierExpected((object) this.lexer.Position));
        if (previousSegment != null && !this.isSelect)
          throw new ODataException(Strings.ExpressionToken_NoSegmentAllowedBeforeStarInExpand);
        identifier = this.lexer.CurrentToken.Text;
        this.lexer.NextToken();
      }
      else
      {
        identifier = this.lexer.CurrentToken.GetIdentifier();
        this.lexer.NextToken();
      }
      return (PathSegmentToken) new NonSystemToken(identifier, (IEnumerable<NamedValue>) null, previousSegment);
    }
  }
}
