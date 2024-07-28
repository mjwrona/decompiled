// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.AmbiguousOperationBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal class AmbiguousOperationBinding : 
    AmbiguousBinding<IEdmOperation>,
    IEdmOperation,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly string fullName;
    private IEdmOperation first;

    public AmbiguousOperationBinding(IEdmOperation first, IEdmOperation second)
      : base(first, second)
    {
      this.first = first;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Namespace, this.Name);
    }

    public IEdmTypeReference ReturnType => (IEdmTypeReference) null;

    public string Namespace => this.first.Namespace;

    public string FullName => this.fullName;

    public IEnumerable<IEdmOperationParameter> Parameters => this.first.Parameters;

    public bool IsBound => this.first.IsBound;

    public IEdmPathExpression EntitySetPath => this.first.EntitySetPath;

    public EdmSchemaElementKind SchemaElementKind => this.first.SchemaElementKind;

    public IEdmOperationParameter FindParameter(string name) => this.first.FindParameter(name);
  }
}
