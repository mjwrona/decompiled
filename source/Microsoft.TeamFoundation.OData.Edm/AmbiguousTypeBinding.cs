// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.AmbiguousTypeBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  internal class AmbiguousTypeBinding : 
    AmbiguousBinding<IEdmSchemaType>,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmType,
    IEdmFullNamedElement
  {
    private readonly string namespaceName;
    private readonly string fullName;

    public AmbiguousTypeBinding(IEdmSchemaType first, IEdmSchemaType second)
      : base(first, second)
    {
      this.namespaceName = first.Namespace ?? string.Empty;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.Name);
    }

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Namespace => this.namespaceName;

    public string FullName => this.fullName;

    public EdmTypeKind TypeKind => EdmTypeKind.None;
  }
}
