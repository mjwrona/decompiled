// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsCollectionExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsCollectionExpression : 
    CsdlSemanticsExpression,
    IEdmCollectionExpression,
    IEdmExpression,
    IEdmElement
  {
    private readonly CsdlCollectionExpression expression;
    private readonly IEdmEntityType bindingContext;
    private readonly Cache<CsdlSemanticsCollectionExpression, IEdmTypeReference> declaredTypeCache = new Cache<CsdlSemanticsCollectionExpression, IEdmTypeReference>();
    private static readonly Func<CsdlSemanticsCollectionExpression, IEdmTypeReference> ComputeDeclaredTypeFunc = (Func<CsdlSemanticsCollectionExpression, IEdmTypeReference>) (me => me.ComputeDeclaredType());
    private readonly Cache<CsdlSemanticsCollectionExpression, IEnumerable<IEdmExpression>> elementsCache = new Cache<CsdlSemanticsCollectionExpression, IEnumerable<IEdmExpression>>();
    private static readonly Func<CsdlSemanticsCollectionExpression, IEnumerable<IEdmExpression>> ComputeElementsFunc = (Func<CsdlSemanticsCollectionExpression, IEnumerable<IEdmExpression>>) (me => me.ComputeElements());

    public CsdlSemanticsCollectionExpression(
      CsdlCollectionExpression expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
      this.bindingContext = bindingContext;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.Collection;

    public IEdmTypeReference DeclaredType => this.declaredTypeCache.GetValue(this, CsdlSemanticsCollectionExpression.ComputeDeclaredTypeFunc, (Func<CsdlSemanticsCollectionExpression, IEdmTypeReference>) null);

    public IEnumerable<IEdmExpression> Elements => this.elementsCache.GetValue(this, CsdlSemanticsCollectionExpression.ComputeElementsFunc, (Func<CsdlSemanticsCollectionExpression, IEnumerable<IEdmExpression>>) null);

    private IEnumerable<IEdmExpression> ComputeElements()
    {
      List<IEdmExpression> elements = new List<IEdmExpression>();
      foreach (CsdlExpressionBase elementValue in this.expression.ElementValues)
        elements.Add(CsdlSemanticsModel.WrapExpression(elementValue, this.bindingContext, this.Schema));
      return (IEnumerable<IEdmExpression>) elements;
    }

    private IEdmTypeReference ComputeDeclaredType() => this.expression.Type == null ? (IEdmTypeReference) null : CsdlSemanticsModel.WrapTypeReference(this.Schema, this.expression.Type);
  }
}
