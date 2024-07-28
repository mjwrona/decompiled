// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsDirectValueAnnotation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsDirectValueAnnotation : 
    CsdlSemanticsElement,
    IEdmDirectValueAnnotation,
    IEdmNamedElement,
    IEdmElement
  {
    private readonly CsdlDirectValueAnnotation annotation;
    private readonly CsdlSemanticsModel model;
    private readonly Cache<CsdlSemanticsDirectValueAnnotation, IEdmValue> valueCache = new Cache<CsdlSemanticsDirectValueAnnotation, IEdmValue>();
    private static readonly Func<CsdlSemanticsDirectValueAnnotation, IEdmValue> ComputeValueFunc = (Func<CsdlSemanticsDirectValueAnnotation, IEdmValue>) (me => me.ComputeValue());

    public CsdlSemanticsDirectValueAnnotation(
      CsdlDirectValueAnnotation annotation,
      CsdlSemanticsModel model)
      : base((CsdlElement) annotation)
    {
      this.annotation = annotation;
      this.model = model;
    }

    public override CsdlElement Element => (CsdlElement) this.annotation;

    public override CsdlSemanticsModel Model => this.model;

    public string NamespaceUri => this.annotation.NamespaceName;

    public string Name => this.annotation.Name;

    public object Value => (object) this.valueCache.GetValue(this, CsdlSemanticsDirectValueAnnotation.ComputeValueFunc, (Func<CsdlSemanticsDirectValueAnnotation, IEdmValue>) null);

    private IEdmValue ComputeValue()
    {
      IEdmStringValue edmStringValue = (IEdmStringValue) new EdmStringConstant((IEdmStringTypeReference) new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), this.annotation.Value);
      edmStringValue.SetIsSerializedAsElement((IEdmModel) this.model, !this.annotation.IsAttribute);
      return (IEdmValue) edmStringValue;
    }
  }
}
