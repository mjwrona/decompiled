// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsLabeledExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsLabeledExpression : 
    CsdlSemanticsElement,
    IEdmLabeledExpression,
    IEdmNamedElement,
    IEdmElement,
    IEdmExpression
  {
    private readonly string name;
    private readonly CsdlExpressionBase sourceElement;
    private readonly CsdlSemanticsSchema schema;
    private readonly IEdmEntityType bindingContext;
    private readonly Cache<CsdlSemanticsLabeledExpression, IEdmExpression> expressionCache = new Cache<CsdlSemanticsLabeledExpression, IEdmExpression>();
    private static readonly Func<CsdlSemanticsLabeledExpression, IEdmExpression> ComputeExpressionFunc = (Func<CsdlSemanticsLabeledExpression, IEdmExpression>) (me => me.ComputeExpression());

    public CsdlSemanticsLabeledExpression(
      string name,
      CsdlExpressionBase element,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base((CsdlElement) element)
    {
      this.name = name;
      this.sourceElement = element;
      this.bindingContext = bindingContext;
      this.schema = schema;
    }

    public override CsdlElement Element => (CsdlElement) this.sourceElement;

    public override CsdlSemanticsModel Model => this.schema.Model;

    public IEdmEntityType BindingContext => this.bindingContext;

    public IEdmExpression Expression => this.expressionCache.GetValue(this, CsdlSemanticsLabeledExpression.ComputeExpressionFunc, (Func<CsdlSemanticsLabeledExpression, IEdmExpression>) null);

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.Labeled;

    public string Name => this.name;

    private IEdmExpression ComputeExpression() => CsdlSemanticsModel.WrapExpression(this.sourceElement, this.BindingContext, this.schema);
  }
}
