// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.IEdmModel
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  public interface IEdmModel : IEdmElement
  {
    IEnumerable<IEdmSchemaElement> SchemaElements { get; }

    IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations { get; }

    IEnumerable<IEdmModel> ReferencedModels { get; }

    IEnumerable<string> DeclaredNamespaces { get; }

    IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager { get; }

    IEdmEntityContainer EntityContainer { get; }

    IEdmSchemaType FindDeclaredType(string qualifiedName);

    IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType);

    IEnumerable<IEdmOperation> FindDeclaredBoundOperations(
      string qualifiedName,
      IEdmType bindingType);

    IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName);

    IEdmTerm FindDeclaredTerm(string qualifiedName);

    IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(
      IEdmVocabularyAnnotatable element);

    IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType);
  }
}
