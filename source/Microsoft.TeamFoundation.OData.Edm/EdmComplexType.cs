// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmComplexType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public class EdmComplexType : 
    EdmStructuredType,
    IEdmComplexType,
    IEdmStructuredType,
    IEdmType,
    IEdmElement,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly string namespaceName;
    private readonly string name;
    private readonly string fullName;

    public EdmComplexType(string namespaceName, string name)
      : this(namespaceName, name, (IEdmComplexType) null, false)
    {
    }

    public EdmComplexType(string namespaceName, string name, IEdmComplexType baseType)
      : this(namespaceName, name, baseType, false, false)
    {
    }

    public EdmComplexType(
      string namespaceName,
      string name,
      IEdmComplexType baseType,
      bool isAbstract)
      : this(namespaceName, name, baseType, isAbstract, false)
    {
    }

    public EdmComplexType(
      string namespaceName,
      string name,
      IEdmComplexType baseType,
      bool isAbstract,
      bool isOpen)
      : base(isAbstract, isOpen, (IEdmStructuredType) baseType)
    {
      EdmUtil.CheckArgumentNull<string>(namespaceName, nameof (namespaceName));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.namespaceName = namespaceName;
      this.name = name;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.Name);
    }

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Namespace => this.namespaceName;

    public string Name => this.name;

    public string FullName => this.fullName;

    public override EdmTypeKind TypeKind => EdmTypeKind.Complex;
  }
}
