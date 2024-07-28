// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpressionLexerLiteralExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  internal static class ExpressionLexerLiteralExtensions
  {
    internal static bool IsLiteralType(this ExpressionTokenKind tokenKind)
    {
      if ((uint) (tokenKind - 4) > 14U)
      {
        switch (tokenKind)
        {
          case ExpressionTokenKind.DateLiteral:
          case ExpressionTokenKind.TimeOfDayLiteral:
            break;
          default:
            return false;
        }
      }
      return true;
    }

    internal static object ReadLiteralToken(this ExpressionLexer expressionLexer)
    {
      expressionLexer.NextToken();
      if (expressionLexer.CurrentToken.Kind.IsLiteralType())
        return expressionLexer.TryParseLiteral();
      throw new ODataException(Microsoft.OData.Strings.ExpressionLexer_ExpectedLiteralToken((object) expressionLexer.CurrentToken.Text));
    }

    private static object ParseNullLiteral(this ExpressionLexer expressionLexer)
    {
      expressionLexer.NextToken();
      return (object) new ODataNullValue();
    }

    private static object ParseTypedLiteral(
      this ExpressionLexer expressionLexer,
      IEdmTypeReference targetTypeReference)
    {
      UriLiteralParsingException parsingException;
      object uriStringToType = DefaultUriLiteralParser.Instance.ParseUriStringToType(expressionLexer.CurrentToken.Text, targetTypeReference, out parsingException);
      if (uriStringToType == null)
      {
        if (parsingException == null)
          throw new ODataException(Microsoft.OData.Strings.UriQueryExpressionParser_UnrecognizedLiteral((object) targetTypeReference.FullName(), (object) expressionLexer.CurrentToken.Text, (object) expressionLexer.CurrentToken.Position, (object) expressionLexer.ExpressionText));
        throw new ODataException(Microsoft.OData.Strings.UriQueryExpressionParser_UnrecognizedLiteralWithReason((object) targetTypeReference.FullName(), (object) expressionLexer.CurrentToken.Text, (object) expressionLexer.CurrentToken.Position, (object) expressionLexer.ExpressionText, (object) parsingException.Message), (Exception) parsingException);
      }
      expressionLexer.NextToken();
      return uriStringToType;
    }

    private static object TryParseLiteral(this ExpressionLexer expressionLexer)
    {
      switch (expressionLexer.CurrentToken.Kind)
      {
        case ExpressionTokenKind.NullLiteral:
          return expressionLexer.ParseNullLiteral();
        case ExpressionTokenKind.BooleanLiteral:
        case ExpressionTokenKind.StringLiteral:
        case ExpressionTokenKind.IntegerLiteral:
        case ExpressionTokenKind.Int64Literal:
        case ExpressionTokenKind.SingleLiteral:
        case ExpressionTokenKind.DateTimeOffsetLiteral:
        case ExpressionTokenKind.DurationLiteral:
        case ExpressionTokenKind.DecimalLiteral:
        case ExpressionTokenKind.DoubleLiteral:
        case ExpressionTokenKind.GuidLiteral:
        case ExpressionTokenKind.BinaryLiteral:
        case ExpressionTokenKind.GeographyLiteral:
        case ExpressionTokenKind.GeometryLiteral:
        case ExpressionTokenKind.QuotedLiteral:
        case ExpressionTokenKind.DateLiteral:
        case ExpressionTokenKind.TimeOfDayLiteral:
        case ExpressionTokenKind.CustomTypeLiteral:
          return expressionLexer.ParseTypedLiteral(expressionLexer.CurrentToken.GetLiteralEdmTypeReference());
        default:
          return (object) null;
      }
    }
  }
}
