// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmCoreModelComplexType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal sealed class EdmCoreModelComplexType : 
    EdmType,
    IEdmComplexType,
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
    public static readonly EdmCoreModelComplexType Instance = new EdmCoreModelComplexType();
    public IEdmStructuredType BaseType;

    private EdmCoreModelComplexType()
    {
    }

    public override EdmTypeKind TypeKind => EdmTypeKind.Complex;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Name => "ComplexType";

    public string Namespace => "Edm";

    public string FullName => "Edm.ComplexType";

    public bool IsAbstract => true;

    public bool IsOpen => false;

    public IEnumerable<IEdmProperty> DeclaredProperties => Enumerable.Empty<IEdmProperty>();

    IEdmStructuredType IEdmStructuredType.BaseType => (IEdmStructuredType) null;

    public IEdmProperty FindProperty(string name) => (IEdmProperty) null;
  }
}
