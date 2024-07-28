// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmUntypedStructuredType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public sealed class EdmUntypedStructuredType : 
    EdmStructuredType,
    IEdmStructuredType,
    IEdmType,
    IEdmElement,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmVocabularyAnnotatable,
    IEdmSchemaType,
    IEdmFullNamedElement
  {
    private readonly string namespaceName;
    private readonly string name;
    private readonly string fullName;

    public EdmUntypedStructuredType(string namespaceName, string name)
      : base(true, true, (IEdmStructuredType) null)
    {
      EdmUtil.CheckArgumentNull<string>(namespaceName, nameof (namespaceName));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.namespaceName = namespaceName;
      this.name = name;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.name);
    }

    public EdmUntypedStructuredType()
      : this("Edm", "Untyped")
    {
    }

    public string Namespace => this.namespaceName;

    public string FullName => this.fullName;

    public string Name => this.name;

    public override EdmTypeKind TypeKind => EdmTypeKind.Untyped;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;
  }
}
