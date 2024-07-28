// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmCoreModelEntityType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal sealed class EdmCoreModelEntityType : 
    EdmType,
    IEdmEntityType,
    IEdmStructuredType,
    IEdmType,
    IEdmElement,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmVocabularyAnnotatable,
    IEdmCoreModelElement,
    IEdmFullNamedElement
  {
    public static readonly EdmCoreModelEntityType Instance = new EdmCoreModelEntityType();

    private EdmCoreModelEntityType()
    {
    }

    public override EdmTypeKind TypeKind => EdmTypeKind.Entity;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Name => "EntityType";

    public string Namespace => "Edm";

    public string FullName => "Edm.EntityType";

    public bool HasStream => false;

    public bool IsAbstract => true;

    public bool IsOpen => false;

    public IEdmStructuredType BaseType => (IEdmStructuredType) null;

    public IEnumerable<IEdmProperty> DeclaredProperties => Enumerable.Empty<IEdmProperty>();

    public IEnumerable<IEdmStructuralProperty> DeclaredKey => Enumerable.Empty<IEdmStructuralProperty>();

    public IEdmProperty FindProperty(string name) => (IEdmProperty) null;
  }
}
