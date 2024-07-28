// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsStringConstantExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsStringConstantExpression : 
    CsdlSemanticsExpression,
    IEdmStringConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmStringValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly CsdlConstantExpression expression;
    private readonly Cache<CsdlSemanticsStringConstantExpression, string> valueCache = new Cache<CsdlSemanticsStringConstantExpression, string>();
    private static readonly Func<CsdlSemanticsStringConstantExpression, string> ComputeValueFunc = (Func<CsdlSemanticsStringConstantExpression, string>) (me => me.ComputeValue());

    public CsdlSemanticsStringConstantExpression(
      CsdlConstantExpression expression,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public string Value => this.valueCache.GetValue(this, CsdlSemanticsStringConstantExpression.ComputeValueFunc, (Func<CsdlSemanticsStringConstantExpression, string>) null);

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.StringConstant;

    public IEdmTypeReference Type => (IEdmTypeReference) null;

    public EdmValueKind ValueKind => this.expression.ValueKind;

    private string ComputeValue() => this.expression.Value;
  }
}
