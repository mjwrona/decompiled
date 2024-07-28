// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.BadEntityContainer
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal class BadEntityContainer : 
    BadElement,
    IEdmEntityContainer,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly string namespaceName;
    private readonly string name;
    private readonly string fullName;

    public BadEntityContainer(string qualifiedName, IEnumerable<EdmError> errors)
      : base(errors)
    {
      qualifiedName = qualifiedName ?? string.Empty;
      EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name, out this.fullName);
    }

    public IEnumerable<IEdmEntityContainerElement> Elements => Enumerable.Empty<IEdmEntityContainerElement>();

    public string Namespace => this.namespaceName;

    public string Name => this.name;

    public string FullName => this.fullName;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.EntityContainer;

    public IEdmEntitySet FindEntitySet(string setName) => (IEdmEntitySet) null;

    public IEdmSingleton FindSingleton(string singletonName) => (IEdmSingleton) null;

    public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName) => (IEnumerable<IEdmOperationImport>) null;
  }
}
