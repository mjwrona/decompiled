// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmCoreModelPathType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  internal sealed class EdmCoreModelPathType : 
    EdmType,
    IEdmPathType,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmType,
    IEdmCoreModelElement,
    IEdmFullNamedElement
  {
    public EdmCoreModelPathType(EdmPathTypeKind pathKind)
    {
      this.Name = pathKind.ToString();
      this.PathKind = pathKind;
    }

    public EdmPathTypeKind PathKind { get; }

    public string FullName => this.Namespace + "." + this.Name;

    public override EdmTypeKind TypeKind => EdmTypeKind.Path;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Namespace => "Edm";

    public string Name { get; }
  }
}
