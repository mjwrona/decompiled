// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmCoreModelPrimitiveType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  internal sealed class EdmCoreModelPrimitiveType : 
    EdmType,
    IEdmPrimitiveType,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmType,
    IEdmCoreModelElement,
    IEdmFullNamedElement
  {
    public EdmCoreModelPrimitiveType(EdmPrimitiveTypeKind primitiveKind)
    {
      this.Name = primitiveKind.ToString();
      this.PrimitiveKind = primitiveKind;
      this.FullName = this.Namespace + "." + this.Name;
    }

    public string Name { get; }

    public string Namespace => "Edm";

    public override EdmTypeKind TypeKind => EdmTypeKind.Primitive;

    public EdmPrimitiveTypeKind PrimitiveKind { get; }

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string FullName { get; }
  }
}
