// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpressionTokenKind
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  internal enum ExpressionTokenKind
  {
    Unknown,
    End,
    Equal,
    Identifier,
    NullLiteral,
    BooleanLiteral,
    StringLiteral,
    IntegerLiteral,
    Int64Literal,
    SingleLiteral,
    DateTimeLiteral,
    DateTimeOffsetLiteral,
    DurationLiteral,
    DecimalLiteral,
    DoubleLiteral,
    GuidLiteral,
    BinaryLiteral,
    GeographyLiteral,
    GeometryLiteral,
    Exclamation,
    OpenParen,
    CloseParen,
    Comma,
    Colon,
    Minus,
    Slash,
    Question,
    Dot,
    Star,
    SemiColon,
    ParameterAlias,
    BracedExpression,
    BracketedExpression,
    QuotedLiteral,
    DateLiteral,
    TimeOfDayLiteral,
    CustomTypeLiteral,
    ParenthesesExpression,
  }
}
