// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsPropertyConstructor
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsPropertyConstructor : 
    CsdlSemanticsElement,
    IEdmPropertyConstructor,
    IEdmElement
  {
    private readonly CsdlPropertyValue property;
    private readonly CsdlSemanticsRecordExpression context;
    private readonly Cache<CsdlSemanticsPropertyConstructor, IEdmExpression> valueCache = new Cache<CsdlSemanticsPropertyConstructor, IEdmExpression>();
    private static readonly Func<CsdlSemanticsPropertyConstructor, IEdmExpression> ComputeValueFunc = (Func<CsdlSemanticsPropertyConstructor, IEdmExpression>) (me => me.ComputeValue());

    public CsdlSemanticsPropertyConstructor(
      CsdlPropertyValue property,
      CsdlSemanticsRecordExpression context)
      : base((CsdlElement) property)
    {
      this.property = property;
      this.context = context;
    }

    public string Name => this.property.Property;

    public IEdmExpression Value => this.valueCache.GetValue(this, CsdlSemanticsPropertyConstructor.ComputeValueFunc, (Func<CsdlSemanticsPropertyConstructor, IEdmExpression>) null);

    public override CsdlElement Element => (CsdlElement) this.property;

    public override CsdlSemanticsModel Model => this.context.Model;

    private IEdmExpression ComputeValue() => CsdlSemanticsModel.WrapExpression(this.property.Expression, this.context.BindingContext, this.context.Schema);
  }
}
