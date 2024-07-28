// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsRecordExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsRecordExpression : 
    CsdlSemanticsExpression,
    IEdmRecordExpression,
    IEdmExpression,
    IEdmElement
  {
    private readonly CsdlRecordExpression expression;
    private readonly IEdmEntityType bindingContext;
    private readonly Cache<CsdlSemanticsRecordExpression, IEdmStructuredTypeReference> declaredTypeCache = new Cache<CsdlSemanticsRecordExpression, IEdmStructuredTypeReference>();
    private static readonly Func<CsdlSemanticsRecordExpression, IEdmStructuredTypeReference> ComputeDeclaredTypeFunc = (Func<CsdlSemanticsRecordExpression, IEdmStructuredTypeReference>) (me => me.ComputeDeclaredType());
    private readonly Cache<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>> propertiesCache = new Cache<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>>();
    private static readonly Func<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>> ComputePropertiesFunc = (Func<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>>) (me => me.ComputeProperties());

    public CsdlSemanticsRecordExpression(
      CsdlRecordExpression expression,
      IEdmEntityType bindingContext,
      CsdlSemanticsSchema schema)
      : base(schema, (CsdlExpressionBase) expression)
    {
      this.expression = expression;
      this.bindingContext = bindingContext;
    }

    public override CsdlElement Element => (CsdlElement) this.expression;

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.Record;

    public IEdmStructuredTypeReference DeclaredType => this.declaredTypeCache.GetValue(this, CsdlSemanticsRecordExpression.ComputeDeclaredTypeFunc, (Func<CsdlSemanticsRecordExpression, IEdmStructuredTypeReference>) null);

    public IEnumerable<IEdmPropertyConstructor> Properties => this.propertiesCache.GetValue(this, CsdlSemanticsRecordExpression.ComputePropertiesFunc, (Func<CsdlSemanticsRecordExpression, IEnumerable<IEdmPropertyConstructor>>) null);

    public IEdmEntityType BindingContext => this.bindingContext;

    private IEnumerable<IEdmPropertyConstructor> ComputeProperties()
    {
      List<IEdmPropertyConstructor> properties = new List<IEdmPropertyConstructor>();
      foreach (CsdlPropertyValue propertyValue in this.expression.PropertyValues)
        properties.Add((IEdmPropertyConstructor) new CsdlSemanticsPropertyConstructor(propertyValue, this));
      return (IEnumerable<IEdmPropertyConstructor>) properties;
    }

    private IEdmStructuredTypeReference ComputeDeclaredType() => this.expression.Type == null ? (IEdmStructuredTypeReference) null : CsdlSemanticsModel.WrapTypeReference(this.Schema, this.expression.Type).AsStructured();
  }
}
