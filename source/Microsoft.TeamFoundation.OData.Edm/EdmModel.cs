// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmModel
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  public class EdmModel : EdmModelBase
  {
    private readonly List<IEdmSchemaElement> elements = new List<IEdmSchemaElement>();
    private readonly Dictionary<IEdmVocabularyAnnotatable, List<IEdmVocabularyAnnotation>> vocabularyAnnotationsDictionary = new Dictionary<IEdmVocabularyAnnotatable, List<IEdmVocabularyAnnotation>>();
    private readonly Dictionary<IEdmStructuredType, List<IEdmStructuredType>> derivedTypeMappings = new Dictionary<IEdmStructuredType, List<IEdmStructuredType>>();
    private readonly HashSet<string> declaredNamespaces = new HashSet<string>();

    public EdmModel()
      : this(true)
    {
    }

    public EdmModel(bool includeDefaultVocabularies)
      : base(Enumerable.Empty<IEdmModel>(), (IEdmDirectValueAnnotationsManager) new EdmDirectValueAnnotationsManager(), includeDefaultVocabularies)
    {
    }

    public override IEnumerable<IEdmSchemaElement> SchemaElements => (IEnumerable<IEdmSchemaElement>) this.elements;

    public override IEnumerable<string> DeclaredNamespaces => (IEnumerable<string>) this.declaredNamespaces;

    public override IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations => this.vocabularyAnnotationsDictionary.SelectMany<KeyValuePair<IEdmVocabularyAnnotatable, List<IEdmVocabularyAnnotation>>, IEdmVocabularyAnnotation>((Func<KeyValuePair<IEdmVocabularyAnnotatable, List<IEdmVocabularyAnnotation>>, IEnumerable<IEdmVocabularyAnnotation>>) (kvp => (IEnumerable<IEdmVocabularyAnnotation>) kvp.Value));

    public new void AddReferencedModel(IEdmModel model) => base.AddReferencedModel(model);

    public void AddElement(IEdmSchemaElement element)
    {
      if (!this.declaredNamespaces.Contains(element.Namespace))
        this.declaredNamespaces.Add(element.Namespace);
      EdmUtil.CheckArgumentNull<IEdmSchemaElement>(element, nameof (element));
      this.elements.Add(element);
      if (element is IEdmStructuredType edmStructuredType && edmStructuredType.BaseType != null)
      {
        List<IEdmStructuredType> edmStructuredTypeList;
        if (!this.derivedTypeMappings.TryGetValue(edmStructuredType.BaseType, out edmStructuredTypeList))
        {
          edmStructuredTypeList = new List<IEdmStructuredType>();
          this.derivedTypeMappings[edmStructuredType.BaseType] = edmStructuredTypeList;
        }
        edmStructuredTypeList.Add(edmStructuredType);
      }
      this.RegisterElement(element);
    }

    public void AddElements(IEnumerable<IEdmSchemaElement> newElements)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmSchemaElement>>(newElements, nameof (newElements));
      foreach (IEdmSchemaElement newElement in newElements)
        this.AddElement(newElement);
    }

    public void AddVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
    {
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotation>(annotation, nameof (annotation));
      if (annotation.Target == null)
        throw new InvalidOperationException(Strings.Constructable_VocabularyAnnotationMustHaveTarget);
      List<IEdmVocabularyAnnotation> vocabularyAnnotationList;
      if (!this.vocabularyAnnotationsDictionary.TryGetValue(annotation.Target, out vocabularyAnnotationList))
      {
        vocabularyAnnotationList = new List<IEdmVocabularyAnnotation>();
        this.vocabularyAnnotationsDictionary.Add(annotation.Target, vocabularyAnnotationList);
      }
      vocabularyAnnotationList.Add(annotation);
    }

    public override IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(
      IEdmVocabularyAnnotatable element)
    {
      List<IEdmVocabularyAnnotation> vocabularyAnnotationList;
      return !this.vocabularyAnnotationsDictionary.TryGetValue(element, out vocabularyAnnotationList) ? Enumerable.Empty<IEdmVocabularyAnnotation>() : (IEnumerable<IEdmVocabularyAnnotation>) vocabularyAnnotationList;
    }

    public override IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(
      IEdmStructuredType baseType)
    {
      List<IEdmStructuredType> edmStructuredTypeList;
      return this.derivedTypeMappings.TryGetValue(baseType, out edmStructuredTypeList) ? (IEnumerable<IEdmStructuredType>) edmStructuredTypeList : Enumerable.Empty<IEdmStructuredType>();
    }

    public void SetVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
    {
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotation>(annotation, nameof (annotation));
      if (annotation.Target == null)
        throw new InvalidOperationException(Strings.Constructable_VocabularyAnnotationMustHaveTarget);
      List<IEdmVocabularyAnnotation> vocabularyAnnotationList;
      if (!this.vocabularyAnnotationsDictionary.TryGetValue(annotation.Target, out vocabularyAnnotationList))
      {
        vocabularyAnnotationList = new List<IEdmVocabularyAnnotation>();
        this.vocabularyAnnotationsDictionary.Add(annotation.Target, vocabularyAnnotationList);
      }
      vocabularyAnnotationList.RemoveAll((Predicate<IEdmVocabularyAnnotation>) (p => p.Term.FullName() == annotation.Term.FullName()));
      vocabularyAnnotationList.Add(annotation);
    }
  }
}
