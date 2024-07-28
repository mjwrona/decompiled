// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmTypeDefinition
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public class EdmTypeDefinition : 
    EdmType,
    IEdmTypeDefinition,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmType,
    IEdmFullNamedElement
  {
    private readonly IEdmPrimitiveType underlyingType;
    private readonly string namespaceName;
    private readonly string name;
    private readonly string fullName;

    public EdmTypeDefinition(
      string namespaceName,
      string name,
      EdmPrimitiveTypeKind underlyingType)
      : this(namespaceName, name, EdmCoreModel.Instance.GetPrimitiveType(underlyingType))
    {
    }

    public EdmTypeDefinition(string namespaceName, string name, IEdmPrimitiveType underlyingType)
    {
      EdmUtil.CheckArgumentNull<IEdmPrimitiveType>(underlyingType, nameof (underlyingType));
      EdmUtil.CheckArgumentNull<string>(namespaceName, nameof (namespaceName));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.underlyingType = underlyingType;
      this.name = name;
      this.namespaceName = namespaceName;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.name);
    }

    public override EdmTypeKind TypeKind => EdmTypeKind.TypeDefinition;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Namespace => this.namespaceName;

    public string FullName => this.fullName;

    public string Name => this.name;

    public IEdmPrimitiveType UnderlyingType => this.underlyingType;
  }
}
