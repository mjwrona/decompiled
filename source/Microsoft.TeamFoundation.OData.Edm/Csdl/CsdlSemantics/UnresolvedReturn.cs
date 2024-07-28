// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.UnresolvedReturn
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class UnresolvedReturn : 
    BadElement,
    IEdmOperationReturn,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IUnresolvedElement
  {
    private readonly Cache<UnresolvedReturn, IEdmTypeReference> type = new Cache<UnresolvedReturn, IEdmTypeReference>();
    private static readonly Func<UnresolvedReturn, IEdmTypeReference> ComputeTypeFunc = (Func<UnresolvedReturn, IEdmTypeReference>) (me => me.ComputeType());

    public UnresolvedReturn(IEdmOperation declaringOperation, EdmLocation location)
      : base((IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(location, EdmErrorCode.BadUnresolvedReturn, Strings.Bad_UnresolvedReturn((object) declaringOperation.Name))
      })
    {
      this.DeclaringOperation = declaringOperation;
    }

    public IEdmTypeReference Type => this.type.GetValue(this, UnresolvedReturn.ComputeTypeFunc, (Func<UnresolvedReturn, IEdmTypeReference>) null);

    public IEdmOperation DeclaringOperation { get; private set; }

    private IEdmTypeReference ComputeType() => (IEdmTypeReference) new BadTypeReference(new BadType(this.Errors), true);
  }
}
