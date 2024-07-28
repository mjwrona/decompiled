// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsOperationReturn
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsOperationReturn : 
    CsdlSemanticsElement,
    IEdmOperationReturn,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly CsdlSemanticsOperation declaringOperation;
    private readonly CsdlOperationReturn operationReturn;
    private readonly Cache<CsdlSemanticsOperationReturn, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsOperationReturn, IEdmTypeReference>();
    private static readonly Func<CsdlSemanticsOperationReturn, IEdmTypeReference> ComputeTypeFunc = (Func<CsdlSemanticsOperationReturn, IEdmTypeReference>) (me => me.ComputeType());

    public CsdlSemanticsOperationReturn(
      CsdlSemanticsOperation declaringOperation,
      CsdlOperationReturn operationReturn)
      : base((CsdlElement) operationReturn)
    {
      this.declaringOperation = declaringOperation;
      this.operationReturn = operationReturn;
    }

    public override CsdlSemanticsModel Model => this.declaringOperation.Model;

    public override CsdlElement Element => (CsdlElement) this.operationReturn;

    public IEdmTypeReference Type => this.typeCache.GetValue(this, CsdlSemanticsOperationReturn.ComputeTypeFunc, (Func<CsdlSemanticsOperationReturn, IEdmTypeReference>) null);

    public IEdmOperation DeclaringOperation => (IEdmOperation) this.declaringOperation;

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.declaringOperation.Context);

    private IEdmTypeReference ComputeType() => CsdlSemanticsModel.WrapTypeReference(this.declaringOperation.Context, this.operationReturn.ReturnType);
  }
}
