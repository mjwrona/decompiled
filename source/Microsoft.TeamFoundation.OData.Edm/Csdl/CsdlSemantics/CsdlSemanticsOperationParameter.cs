// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsOperationParameter
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsOperationParameter : 
    CsdlSemanticsElement,
    IEdmOperationParameter,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly CsdlSemanticsOperation declaringOperation;
    private readonly CsdlOperationParameter parameter;
    private readonly Cache<CsdlSemanticsOperationParameter, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsOperationParameter, IEdmTypeReference>();
    private static readonly Func<CsdlSemanticsOperationParameter, IEdmTypeReference> ComputeTypeFunc = (Func<CsdlSemanticsOperationParameter, IEdmTypeReference>) (me => me.ComputeType());

    public CsdlSemanticsOperationParameter(
      CsdlSemanticsOperation declaringOperation,
      CsdlOperationParameter parameter)
      : base((CsdlElement) parameter)
    {
      this.parameter = parameter;
      this.declaringOperation = declaringOperation;
    }

    public override CsdlSemanticsModel Model => this.declaringOperation.Model;

    public override CsdlElement Element => (CsdlElement) this.parameter;

    public IEdmTypeReference Type => this.typeCache.GetValue(this, CsdlSemanticsOperationParameter.ComputeTypeFunc, (Func<CsdlSemanticsOperationParameter, IEdmTypeReference>) null);

    public string Name => this.parameter.Name;

    public IEdmOperation DeclaringOperation => (IEdmOperation) this.declaringOperation;

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.declaringOperation.Context);

    private IEdmTypeReference ComputeType() => CsdlSemanticsModel.WrapTypeReference(this.declaringOperation.Context, this.parameter.Type);
  }
}
