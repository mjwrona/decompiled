// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlConstantExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlConstantExpression : CsdlExpressionBase
  {
    private readonly EdmValueKind kind;
    private readonly string value;

    public CsdlConstantExpression(EdmValueKind kind, string value, CsdlLocation location)
      : base(location)
    {
      this.kind = kind;
      this.value = value;
    }

    public override EdmExpressionKind ExpressionKind
    {
      get
      {
        switch (this.kind)
        {
          case EdmValueKind.Binary:
            return EdmExpressionKind.BinaryConstant;
          case EdmValueKind.Boolean:
            return EdmExpressionKind.BooleanConstant;
          case EdmValueKind.DateTimeOffset:
            return EdmExpressionKind.DateTimeOffsetConstant;
          case EdmValueKind.Decimal:
            return EdmExpressionKind.DecimalConstant;
          case EdmValueKind.Floating:
            return EdmExpressionKind.FloatingConstant;
          case EdmValueKind.Guid:
            return EdmExpressionKind.GuidConstant;
          case EdmValueKind.Integer:
            return EdmExpressionKind.IntegerConstant;
          case EdmValueKind.Null:
            return EdmExpressionKind.Null;
          case EdmValueKind.String:
            return EdmExpressionKind.StringConstant;
          case EdmValueKind.Duration:
            return EdmExpressionKind.DurationConstant;
          case EdmValueKind.Date:
            return EdmExpressionKind.DateConstant;
          case EdmValueKind.TimeOfDay:
            return EdmExpressionKind.TimeOfDayConstant;
          default:
            return EdmExpressionKind.None;
        }
      }
    }

    public EdmValueKind ValueKind => this.kind;

    public string Value => this.value;
  }
}
