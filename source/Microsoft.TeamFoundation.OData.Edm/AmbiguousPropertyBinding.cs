// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.AmbiguousPropertyBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm
{
  internal class AmbiguousPropertyBinding : 
    AmbiguousBinding<IEdmProperty>,
    IEdmProperty,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly IEdmStructuredType declaringType;
    private readonly Cache<AmbiguousPropertyBinding, IEdmTypeReference> type = new Cache<AmbiguousPropertyBinding, IEdmTypeReference>();
    private static readonly Func<AmbiguousPropertyBinding, IEdmTypeReference> ComputeTypeFunc = (Func<AmbiguousPropertyBinding, IEdmTypeReference>) (me => me.ComputeType());

    public AmbiguousPropertyBinding(
      IEdmStructuredType declaringType,
      IEdmProperty first,
      IEdmProperty second)
      : base(first, second)
    {
      this.declaringType = declaringType;
    }

    public EdmPropertyKind PropertyKind => EdmPropertyKind.None;

    public IEdmTypeReference Type => this.type.GetValue(this, AmbiguousPropertyBinding.ComputeTypeFunc, (Func<AmbiguousPropertyBinding, IEdmTypeReference>) null);

    public IEdmStructuredType DeclaringType => this.declaringType;

    private IEdmTypeReference ComputeType() => (IEdmTypeReference) new BadTypeReference(new BadType(this.Errors), true);
  }
}
