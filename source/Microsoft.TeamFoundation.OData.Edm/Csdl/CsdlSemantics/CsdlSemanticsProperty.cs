// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsProperty
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsProperty : 
    CsdlSemanticsElement,
    IEdmStructuralProperty,
    IEdmProperty,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    protected CsdlProperty property;
    private readonly CsdlSemanticsStructuredTypeDefinition declaringType;
    private readonly Cache<CsdlSemanticsProperty, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsProperty, IEdmTypeReference>();
    private static readonly Func<CsdlSemanticsProperty, IEdmTypeReference> ComputeTypeFunc = (Func<CsdlSemanticsProperty, IEdmTypeReference>) (me => me.ComputeType());

    public CsdlSemanticsProperty(
      CsdlSemanticsStructuredTypeDefinition declaringType,
      CsdlProperty property)
      : base((CsdlElement) property)
    {
      this.property = property;
      this.declaringType = declaringType;
    }

    public string Name => this.property.Name;

    public IEdmStructuredType DeclaringType => (IEdmStructuredType) this.declaringType;

    public IEdmTypeReference Type => this.typeCache.GetValue(this, CsdlSemanticsProperty.ComputeTypeFunc, (Func<CsdlSemanticsProperty, IEdmTypeReference>) null);

    public override CsdlSemanticsModel Model => this.declaringType.Model;

    public string DefaultValueString => this.property.DefaultValue;

    public EdmPropertyKind PropertyKind => EdmPropertyKind.Structural;

    public override CsdlElement Element => (CsdlElement) this.property;

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.declaringType.Context);

    private IEdmTypeReference ComputeType() => CsdlSemanticsModel.WrapTypeReference(this.declaringType.Context, this.property.Type);
  }
}
