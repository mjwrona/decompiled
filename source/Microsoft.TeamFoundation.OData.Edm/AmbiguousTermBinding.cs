// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.AmbiguousTermBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm
{
  internal class AmbiguousTermBinding : 
    AmbiguousBinding<IEdmTerm>,
    IEdmTerm,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly IEdmTerm first;
    private readonly string fullName;
    private readonly Cache<AmbiguousTermBinding, IEdmTypeReference> type = new Cache<AmbiguousTermBinding, IEdmTypeReference>();
    private static readonly Func<AmbiguousTermBinding, IEdmTypeReference> ComputeTypeFunc = (Func<AmbiguousTermBinding, IEdmTypeReference>) (me => me.ComputeType());
    private readonly string appliesTo;
    private readonly string defaultValue;

    public AmbiguousTermBinding(IEdmTerm first, IEdmTerm second)
      : base(first, second)
    {
      this.first = first;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Namespace, this.Name);
    }

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.Term;

    public string Namespace => this.first.Namespace ?? string.Empty;

    public string FullName => this.fullName;

    public IEdmTypeReference Type => this.type.GetValue(this, AmbiguousTermBinding.ComputeTypeFunc, (Func<AmbiguousTermBinding, IEdmTypeReference>) null);

    public string AppliesTo => this.appliesTo;

    public string DefaultValue => this.defaultValue;

    private IEdmTypeReference ComputeType() => (IEdmTypeReference) new BadTypeReference(new BadType(this.Errors), true);
  }
}
