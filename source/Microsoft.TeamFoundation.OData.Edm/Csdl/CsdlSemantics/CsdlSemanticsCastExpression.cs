// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsCastExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsCastExpression : 
    CsdlSemanticsExpression,
    IEdmCastExpression,
    IEdmExpression,
    IEdmElement
  {
    private readonly CsdlCastExpression expression;
    private readonly IEdmEntityType bindingContext;
    private readonly Cache<CsdlSemanticsCastExpression, IEdmExpression> operandCache = new Cache<CsdlSemanticsCastExpression, IEdmExpression>();
    private static readonly Func<CsdlSemanticsCastExpression, IEdmExpression> ComputeOperandFunc = (Func<CsdlSemanticsCastExpression, IEdmExpression>) (me => me.ComputeOperand());
    private readonly Cache<CsdlSemanticsCastExpression, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsCastExpression, IEdmTypeReference>();
    private static readonly Func<CsdlSemanticsCastExpression, IEdmTypeReference> ComputeTypeFunc = (Func<CsdlSemanticsCastExpression, IEdmTypeReference>) (me => me.ComputeType());

    public CsdlSemanticsCastExpression(
      CsdlCastExpression expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
      this.bindingContext = bindingContext;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.Cast;

    public IEdmExpression Operand => this.operandCache.GetValue(this, CsdlSemanticsCastExpression.ComputeOperandFunc, (Func<CsdlSemanticsCastExpression, IEdmExpression>) null);

    public IEdmTypeReference Type => this.typeCache.GetValue(this, CsdlSemanticsCastExpression.ComputeTypeFunc, (Func<CsdlSemanticsCastExpression, IEdmTypeReference>) null);

    private IEdmExpression ComputeOperand() => CsdlSemanticsModel.WrapExpression(this.expression.Operand, this.bindingContext, this.Schema);

    private IEdmTypeReference ComputeType() => CsdlSemanticsModel.WrapTypeReference(this.Schema, this.expression.Type);
  }
}
