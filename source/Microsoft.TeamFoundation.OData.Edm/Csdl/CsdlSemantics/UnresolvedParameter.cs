// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.UnresolvedParameter
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class UnresolvedParameter : 
    BadElement,
    IEdmOperationParameter,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IUnresolvedElement
  {
    private readonly Cache<UnresolvedParameter, IEdmTypeReference> type = new Cache<UnresolvedParameter, IEdmTypeReference>();
    private static readonly Func<UnresolvedParameter, IEdmTypeReference> ComputeTypeFunc = (Func<UnresolvedParameter, IEdmTypeReference>) (me => me.ComputeType());

    public UnresolvedParameter(IEdmOperation declaringOperation, string name, EdmLocation location)
      : base((IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(location, EdmErrorCode.BadUnresolvedParameter, Strings.Bad_UnresolvedParameter((object) name))
      })
    {
      this.Name = name ?? string.Empty;
      this.DeclaringOperation = declaringOperation;
    }

    public string Name { get; private set; }

    public IEdmTypeReference Type => this.type.GetValue(this, UnresolvedParameter.ComputeTypeFunc, (Func<UnresolvedParameter, IEdmTypeReference>) null);

    public IEdmOperation DeclaringOperation { get; private set; }

    private IEdmTypeReference ComputeType() => (IEdmTypeReference) new BadTypeReference(new BadType(this.Errors), true);
  }
}
