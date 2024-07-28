// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsLabeledExpressionReferenceExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsLabeledExpressionReferenceExpression : 
    CsdlSemanticsExpression,
    IEdmLabeledExpressionReferenceExpression,
    IEdmExpression,
    IEdmElement,
    IEdmCheckable
  {
    private readonly CsdlLabeledExpressionReferenceExpression expression;
    private readonly IEdmEntityType bindingContext;
    private readonly Cache<CsdlSemanticsLabeledExpressionReferenceExpression, IEdmLabeledExpression> elementCache = new Cache<CsdlSemanticsLabeledExpressionReferenceExpression, IEdmLabeledExpression>();
    private static readonly Func<CsdlSemanticsLabeledExpressionReferenceExpression, IEdmLabeledExpression> ComputeElementFunc = (Func<CsdlSemanticsLabeledExpressionReferenceExpression, IEdmLabeledExpression>) (me => me.ComputeElement());

    public CsdlSemanticsLabeledExpressionReferenceExpression(
      CsdlLabeledExpressionReferenceExpression expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
      this.bindingContext = bindingContext;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.LabeledExpressionReference;

    public IEdmLabeledExpression ReferencedLabeledExpression => this.elementCache.GetValue(this, CsdlSemanticsLabeledExpressionReferenceExpression.ComputeElementFunc, (Func<CsdlSemanticsLabeledExpressionReferenceExpression, IEdmLabeledExpression>) null);

    public IEnumerable<EdmError> Errors => this.ReferencedLabeledExpression is IUnresolvedElement ? this.ReferencedLabeledExpression.Errors() : Enumerable.Empty<EdmError>();

    private IEdmLabeledExpression ComputeElement() => this.Schema.FindLabeledElement(this.expression.Label, this.bindingContext) ?? (IEdmLabeledExpression) new UnresolvedLabeledElement(this.expression.Label, this.Location);
  }
}
