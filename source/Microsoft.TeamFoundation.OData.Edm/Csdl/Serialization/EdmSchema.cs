// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Serialization.EdmSchema
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
  internal class EdmSchema
  {
    private readonly string schemaNamespace;
    private readonly List<IEdmSchemaElement> schemaElements;
    private readonly List<IEdmEntityContainer> entityContainers;
    private readonly Dictionary<string, List<IEdmVocabularyAnnotation>> annotations;
    private readonly List<string> usedNamespaces;

    public EdmSchema(string namespaceString)
    {
      this.schemaNamespace = namespaceString;
      this.schemaElements = new List<IEdmSchemaElement>();
      this.entityContainers = new List<IEdmEntityContainer>();
      this.annotations = new Dictionary<string, List<IEdmVocabularyAnnotation>>();
      this.usedNamespaces = new List<string>();
    }

    public string Namespace => this.schemaNamespace;

    public List<IEdmSchemaElement> SchemaElements => this.schemaElements;

    public List<IEdmEntityContainer> EntityContainers => this.entityContainers;

    public IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> OutOfLineAnnotations => (IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>>) this.annotations;

    public void AddSchemaElement(IEdmSchemaElement element) => this.schemaElements.Add(element);

    public void AddEntityContainer(IEdmEntityContainer container) => this.entityContainers.Add(container);

    public void AddNamespaceUsing(string usedNamespace)
    {
      if (!(usedNamespace != "Edm") || this.usedNamespaces.Contains(usedNamespace))
        return;
      this.usedNamespaces.Add(usedNamespace);
    }

    public void AddVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
    {
      List<IEdmVocabularyAnnotation> vocabularyAnnotationList;
      if (!this.annotations.TryGetValue(annotation.TargetString(), out vocabularyAnnotationList))
      {
        vocabularyAnnotationList = new List<IEdmVocabularyAnnotation>();
        this.annotations[annotation.TargetString()] = vocabularyAnnotationList;
      }
      vocabularyAnnotationList.Add(annotation);
    }
  }
}
