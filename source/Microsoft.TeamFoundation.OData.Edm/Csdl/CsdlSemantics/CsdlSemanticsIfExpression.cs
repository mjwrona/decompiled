// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsIfExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsIfExpression : 
    CsdlSemanticsExpression,
    IEdmIfExpression,
    IEdmExpression,
    IEdmElement
  {
    private readonly CsdlIfExpression expression;
    private readonly IEdmEntityType bindingContext;
    private readonly Cache<CsdlSemanticsIfExpression, IEdmExpression> testCache = new Cache<CsdlSemanticsIfExpression, IEdmExpression>();
    private static readonly Func<CsdlSemanticsIfExpression, IEdmExpression> ComputeTestFunc = (Func<CsdlSemanticsIfExpression, IEdmExpression>) (me => me.ComputeTest());
    private readonly Cache<CsdlSemanticsIfExpression, IEdmExpression> ifTrueCache = new Cache<CsdlSemanticsIfExpression, IEdmExpression>();
    private static readonly Func<CsdlSemanticsIfExpression, IEdmExpression> ComputeIfTrueFunc = (Func<CsdlSemanticsIfExpression, IEdmExpression>) (me => me.ComputeIfTrue());
    private readonly Cache<CsdlSemanticsIfExpression, IEdmExpression> ifFalseCache = new Cache<CsdlSemanticsIfExpression, IEdmExpression>();
    private static readonly Func<CsdlSemanticsIfExpression, IEdmExpression> ComputeIfFalseFunc = (Func<CsdlSemanticsIfExpression, IEdmExpression>) (me => me.ComputeIfFalse());

    public CsdlSemanticsIfExpression(
      CsdlIfExpression expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
      this.bindingContext = bindingContext;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public IEdmEntityType BindingContext => this.bindingContext;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.If;

    public IEdmExpression TestExpression => this.testCache.GetValue(this, CsdlSemanticsIfExpression.ComputeTestFunc, (Func<CsdlSemanticsIfExpression, IEdmExpression>) null);

    public IEdmExpression TrueExpression => this.ifTrueCache.GetValue(this, CsdlSemanticsIfExpression.ComputeIfTrueFunc, (Func<CsdlSemanticsIfExpression, IEdmExpression>) null);

    public IEdmExpression FalseExpression => this.ifFalseCache.GetValue(this, CsdlSemanticsIfExpression.ComputeIfFalseFunc, (Func<CsdlSemanticsIfExpression, IEdmExpression>) null);

    private IEdmExpression ComputeTest() => CsdlSemanticsModel.WrapExpression(this.expression.Test, this.BindingContext, this.Schema);

    private IEdmExpression ComputeIfTrue() => CsdlSemanticsModel.WrapExpression(this.expression.IfTrue, this.BindingContext, this.Schema);

    private IEdmExpression ComputeIfFalse() => CsdlSemanticsModel.WrapExpression(this.expression.IfFalse, this.BindingContext, this.Schema);
  }
}
