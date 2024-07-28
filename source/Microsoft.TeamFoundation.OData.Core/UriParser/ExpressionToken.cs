// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpressionToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.OData.UriParser
{
  [DebuggerDisplay("{InternalKind} @ {Position}: [{Text}]")]
  internal struct ExpressionToken
  {
    internal static readonly ExpressionToken GreaterThan = new ExpressionToken()
    {
      Text = "gt",
      Kind = ExpressionTokenKind.Identifier,
      Position = 0
    };
    internal static readonly ExpressionToken EqualsTo = new ExpressionToken()
    {
      Text = "eq",
      Kind = ExpressionTokenKind.Identifier,
      Position = 0
    };
    internal static readonly ExpressionToken LessThan = new ExpressionToken()
    {
      Text = "lt",
      Kind = ExpressionTokenKind.Identifier,
      Position = 0
    };
    internal ExpressionTokenKind Kind;
    internal string Text;
    internal int Position;
    private IEdmTypeReference LiteralEdmType;

    internal bool IsKeyValueToken => this.Kind == ExpressionTokenKind.BinaryLiteral || this.Kind == ExpressionTokenKind.BooleanLiteral || this.Kind == ExpressionTokenKind.DateLiteral || this.Kind == ExpressionTokenKind.DateTimeLiteral || this.Kind == ExpressionTokenKind.DateTimeOffsetLiteral || this.Kind == ExpressionTokenKind.DurationLiteral || this.Kind == ExpressionTokenKind.GuidLiteral || this.Kind == ExpressionTokenKind.StringLiteral || this.Kind == ExpressionTokenKind.GeographyLiteral || this.Kind == ExpressionTokenKind.GeometryLiteral || this.Kind == ExpressionTokenKind.QuotedLiteral || this.Kind == ExpressionTokenKind.TimeOfDayLiteral || ExpressionLexerUtils.IsNumeric(this.Kind);

    internal bool IsFunctionParameterToken => this.IsKeyValueToken || this.Kind == ExpressionTokenKind.BracketedExpression || this.Kind == ExpressionTokenKind.BracedExpression || this.Kind == ExpressionTokenKind.NullLiteral;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} @ {1}: [{2}]", new object[3]
    {
      (object) this.Kind,
      (object) this.Position,
      (object) this.Text
    });

    internal string GetIdentifier()
    {
      if (this.Kind != ExpressionTokenKind.Identifier)
        throw new ODataException(Microsoft.OData.Strings.ExpressionToken_IdentifierExpected((object) this.Position));
      return this.Text;
    }

    internal bool IdentifierIs(string id, bool enableCaseInsensitive) => this.Kind == ExpressionTokenKind.Identifier && string.Equals(this.Text, id, enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    internal void SetCustomEdmTypeLiteral(IEdmTypeReference edmType)
    {
      this.Kind = ExpressionTokenKind.CustomTypeLiteral;
      this.LiteralEdmType = edmType;
    }

    internal IEdmTypeReference GetLiteralEdmTypeReference()
    {
      if (this.LiteralEdmType == null && this.Kind != ExpressionTokenKind.CustomTypeLiteral)
        this.LiteralEdmType = UriParserHelper.GetLiteralEdmTypeReference(this.Kind);
      return this.LiteralEdmType;
    }
  }
}
