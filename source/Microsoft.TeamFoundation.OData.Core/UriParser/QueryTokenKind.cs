// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.QueryTokenKind
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.UriParser
{
  [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
  public enum QueryTokenKind
  {
    BinaryOperator = 3,
    UnaryOperator = 4,
    Literal = 5,
    FunctionCall = 6,
    EndPath = 7,
    OrderBy = 8,
    CustomQueryOption = 9,
    Select = 10, // 0x0000000A
    Star = 11, // 0x0000000B
    Expand = 13, // 0x0000000D
    TypeSegment = 14, // 0x0000000E
    Any = 15, // 0x0000000F
    InnerPath = 16, // 0x00000010
    DottedIdentifier = 17, // 0x00000011
    RangeVariable = 18, // 0x00000012
    All = 19, // 0x00000013
    ExpandTerm = 20, // 0x00000014
    FunctionParameter = 21, // 0x00000015
    FunctionParameterAlias = 22, // 0x00000016
    StringLiteral = 23, // 0x00000017
    Aggregate = 24, // 0x00000018
    AggregateExpression = 25, // 0x00000019
    AggregateGroupBy = 26, // 0x0000001A
    Compute = 27, // 0x0000001B
    ComputeExpression = 28, // 0x0000001C
    EntitySetAggregateExpression = 29, // 0x0000001D
    In = 30, // 0x0000001E
    SelectTerm = 31, // 0x0000001F
  }
}
