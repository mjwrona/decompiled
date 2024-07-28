// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Serialization.EdmModelSchemaSeparationSerializationVisitor
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
  internal class EdmModelSchemaSeparationSerializationVisitor : EdmModelVisitor
  {
    private bool visitCompleted;
    private Dictionary<string, EdmSchema> modelSchemas = new Dictionary<string, EdmSchema>();
    private EdmSchema activeSchema;

    public EdmModelSchemaSeparationSerializationVisitor(IEdmModel visitedModel)
      : base(visitedModel)
    {
    }

    public IEnumerable<EdmSchema> GetSchemas()
    {
      if (!this.visitCompleted)
        this.Visit();
      return (IEnumerable<EdmSchema>) this.modelSchemas.Values;
    }

    protected void Visit()
    {
      this.VisitEdmModel();
      this.visitCompleted = true;
    }

    protected override void ProcessModel(IEdmModel model)
    {
      this.ProcessElement((IEdmElement) model);
      this.VisitSchemaElements(model.SchemaElements);
      this.VisitVocabularyAnnotations(model.VocabularyAnnotations.Where<IEdmVocabularyAnnotation>((Func<IEdmVocabularyAnnotation, bool>) (a => !a.IsInline(this.Model))));
    }

    protected override void ProcessVocabularyAnnotatable(IEdmVocabularyAnnotatable element)
    {
      this.VisitAnnotations(this.Model.DirectValueAnnotations((IEdmElement) element));
      this.VisitVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(element).Where<IEdmVocabularyAnnotation>((Func<IEdmVocabularyAnnotation, bool>) (a => a.IsInline(this.Model))));
    }

    protected override void ProcessSchemaElement(IEdmSchemaElement element)
    {
      string empty = element.Namespace;
      if (EdmUtil.IsNullOrWhiteSpaceInternal(empty))
        empty = string.Empty;
      EdmSchema edmSchema;
      if (!this.modelSchemas.TryGetValue(empty, out edmSchema))
      {
        edmSchema = new EdmSchema(empty);
        this.modelSchemas.Add(empty, edmSchema);
      }
      edmSchema.AddSchemaElement(element);
      this.activeSchema = edmSchema;
      base.ProcessSchemaElement(element);
    }

    protected override void ProcessVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
    {
      if (!annotation.IsInline(this.Model))
      {
        string str = annotation.GetSchemaNamespace(this.Model) ?? this.modelSchemas.Select<KeyValuePair<string, EdmSchema>, string>((Func<KeyValuePair<string, EdmSchema>, string>) (s => s.Key)).FirstOrDefault<string>() ?? string.Empty;
        EdmSchema edmSchema;
        if (!this.modelSchemas.TryGetValue(str, out edmSchema))
        {
          edmSchema = new EdmSchema(str);
          this.modelSchemas.Add(edmSchema.Namespace, edmSchema);
        }
        edmSchema.AddVocabularyAnnotation(annotation);
        this.activeSchema = edmSchema;
      }
      if (annotation.Term != null)
        this.CheckSchemaElementReference((IEdmSchemaElement) annotation.Term);
      base.ProcessVocabularyAnnotation(annotation);
    }

    protected override void ProcessEntityContainer(IEdmEntityContainer element)
    {
      string str = element.Namespace;
      EdmSchema edmSchema;
      if (!this.modelSchemas.TryGetValue(str, out edmSchema))
      {
        edmSchema = new EdmSchema(str);
        this.modelSchemas.Add(edmSchema.Namespace, edmSchema);
      }
      edmSchema.AddEntityContainer(element);
      this.activeSchema = edmSchema;
      base.ProcessEntityContainer(element);
    }

    protected override void ProcessComplexTypeReference(IEdmComplexTypeReference element) => this.CheckSchemaElementReference((IEdmSchemaElement) element.ComplexDefinition());

    protected override void ProcessEntityTypeReference(IEdmEntityTypeReference element) => this.CheckSchemaElementReference((IEdmSchemaElement) element.EntityDefinition());

    protected override void ProcessEntityReferenceTypeReference(
      IEdmEntityReferenceTypeReference element)
    {
      this.CheckSchemaElementReference((IEdmSchemaElement) element.EntityType());
    }

    protected override void ProcessEnumTypeReference(IEdmEnumTypeReference element) => this.CheckSchemaElementReference((IEdmSchemaElement) element.EnumDefinition());

    protected override void ProcessTypeDefinitionReference(IEdmTypeDefinitionReference element) => this.CheckSchemaElementReference((IEdmSchemaElement) element.TypeDefinition());

    protected override void ProcessEntityType(IEdmEntityType element)
    {
      base.ProcessEntityType(element);
      if (element.BaseEntityType() == null)
        return;
      this.CheckSchemaElementReference((IEdmSchemaElement) element.BaseEntityType());
    }

    protected override void ProcessComplexType(IEdmComplexType element)
    {
      base.ProcessComplexType(element);
      if (element.BaseComplexType() == null)
        return;
      this.CheckSchemaElementReference((IEdmSchemaElement) element.BaseComplexType());
    }

    protected override void ProcessEnumType(IEdmEnumType element)
    {
      base.ProcessEnumType(element);
      this.CheckSchemaElementReference((IEdmSchemaElement) element.UnderlyingType);
    }

    protected override void ProcessTypeDefinition(IEdmTypeDefinition element)
    {
      base.ProcessTypeDefinition(element);
      this.CheckSchemaElementReference((IEdmSchemaElement) element.UnderlyingType);
    }

    private void CheckSchemaElementReference(IEdmSchemaElement element) => this.CheckSchemaElementReference(element.Namespace);

    private void CheckSchemaElementReference(string namespaceName)
    {
      if (this.activeSchema == null)
        return;
      this.activeSchema.AddNamespaceUsing(namespaceName);
    }
  }
}
