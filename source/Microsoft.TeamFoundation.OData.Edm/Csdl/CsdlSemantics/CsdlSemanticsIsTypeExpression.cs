// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsIsTypeExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsIsTypeExpression : 
    CsdlSemanticsExpression,
    IEdmIsTypeExpression,
    IEdmExpression,
    IEdmElement
  {
    private readonly CsdlIsTypeExpression expression;
    private readonly IEdmEntityType bindingContext;
    private readonly Cache<CsdlSemanticsIsTypeExpression, IEdmExpression> operandCache = new Cache<CsdlSemanticsIsTypeExpression, IEdmExpression>();
    private static readonly Func<CsdlSemanticsIsTypeExpression, IEdmExpression> ComputeOperandFunc = (Func<CsdlSemanticsIsTypeExpression, IEdmExpression>) (me => me.ComputeOperand());
    private readonly Cache<CsdlSemanticsIsTypeExpression, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsIsTypeExpression, IEdmTypeReference>();
    private static readonly Func<CsdlSemanticsIsTypeExpression, IEdmTypeReference> ComputeTypeFunc = (Func<CsdlSemanticsIsTypeExpression, IEdmTypeReference>) (me => me.ComputeType());

    public CsdlSemanticsIsTypeExpression(
      CsdlIsTypeExpression expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
      this.bindingContext = bindingContext;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.IsType;

    public IEdmExpression Operand => this.operandCache.GetValue(this, CsdlSemanticsIsTypeExpression.ComputeOperandFunc, (Func<CsdlSemanticsIsTypeExpression, IEdmExpression>) null);

    public IEdmTypeReference Type => this.typeCache.GetValue(this, CsdlSemanticsIsTypeExpression.ComputeTypeFunc, (Func<CsdlSemanticsIsTypeExpression, IEdmTypeReference>) null);

    private IEdmExpression ComputeOperand() => CsdlSemanticsModel.WrapExpression(this.expression.Operand, this.bindingContext, this.Schema);

    private IEdmTypeReference ComputeType() => CsdlSemanticsModel.WrapTypeReference(this.Schema, this.expression.Type);
  }
}
