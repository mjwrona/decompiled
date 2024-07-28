// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.BadPrimitiveType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal class BadPrimitiveType : 
    BadType,
    IEdmPrimitiveType,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmType,
    IEdmFullNamedElement
  {
    private readonly EdmPrimitiveTypeKind primitiveKind;
    private readonly string name;
    private readonly string namespaceName;
    private readonly string fullName;

    public BadPrimitiveType(
      string qualifiedName,
      EdmPrimitiveTypeKind primitiveKind,
      IEnumerable<EdmError> errors)
      : base(errors)
    {
      this.primitiveKind = primitiveKind;
      qualifiedName = qualifiedName ?? string.Empty;
      EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name, out this.fullName);
    }

    public EdmPrimitiveTypeKind PrimitiveKind => this.primitiveKind;

    public string Namespace => this.namespaceName;

    public string Name => this.name;

    public string FullName => this.fullName;

    public override EdmTypeKind TypeKind => EdmTypeKind.Primitive;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;
  }
}
