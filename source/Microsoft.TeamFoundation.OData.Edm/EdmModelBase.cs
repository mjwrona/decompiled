// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmModelBase
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  public abstract class EdmModelBase : EdmElement, IEdmModel, IEdmElement
  {
    private readonly List<IEdmModel> referencedEdmModels;
    private readonly IEdmDirectValueAnnotationsManager annotationsManager;
    private readonly Dictionary<string, IEdmEntityContainer> containersDictionary = new Dictionary<string, IEdmEntityContainer>();
    private readonly Dictionary<string, IEdmSchemaType> schemaTypeDictionary = new Dictionary<string, IEdmSchemaType>();
    private readonly Dictionary<string, IEdmTerm> termDictionary = new Dictionary<string, IEdmTerm>();
    private readonly Dictionary<string, IList<IEdmOperation>> functionDictionary = new Dictionary<string, IList<IEdmOperation>>();

    protected EdmModelBase(
      IEnumerable<IEdmModel> referencedModels,
      IEdmDirectValueAnnotationsManager annotationsManager)
      : this(referencedModels, annotationsManager, true)
    {
    }

    protected EdmModelBase(
      IEnumerable<IEdmModel> referencedModels,
      IEdmDirectValueAnnotationsManager annotationsManager,
      bool includeDefaultVocabularies)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmModel>>(referencedModels, nameof (referencedModels));
      EdmUtil.CheckArgumentNull<IEdmDirectValueAnnotationsManager>(annotationsManager, nameof (annotationsManager));
      this.referencedEdmModels = new List<IEdmModel>(referencedModels);
      this.referencedEdmModels.Insert(0, (IEdmModel) EdmCoreModel.Instance);
      if (includeDefaultVocabularies)
        this.referencedEdmModels.AddRange(VocabularyModelProvider.VocabularyModels);
      this.annotationsManager = annotationsManager;
    }

    public abstract IEnumerable<IEdmSchemaElement> SchemaElements { get; }

    public abstract IEnumerable<string> DeclaredNamespaces { get; }

    public virtual IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations => Enumerable.Empty<IEdmVocabularyAnnotation>();

    public IEnumerable<IEdmModel> ReferencedModels => (IEnumerable<IEdmModel>) this.referencedEdmModels;

    public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager => this.annotationsManager;

    public IEdmEntityContainer EntityContainer => this.containersDictionary.Values.FirstOrDefault<IEdmEntityContainer>();

    public IEdmSchemaType FindDeclaredType(string qualifiedName)
    {
      IEdmSchemaType declaredType;
      this.schemaTypeDictionary.TryGetValue(qualifiedName, out declaredType);
      return declaredType;
    }

    public IEdmTerm FindDeclaredTerm(string qualifiedName)
    {
      IEdmTerm declaredTerm;
      this.termDictionary.TryGetValue(qualifiedName, out declaredTerm);
      return declaredTerm;
    }

    public IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName)
    {
      IList<IEdmOperation> edmOperationList;
      return this.functionDictionary.TryGetValue(qualifiedName, out edmOperationList) ? (IEnumerable<IEdmOperation>) edmOperationList : Enumerable.Empty<IEdmOperation>();
    }

    public virtual IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
    {
      foreach (IEnumerable<IEdmOperation> source in this.functionDictionary.Values.Distinct<IList<IEdmOperation>>())
      {
        foreach (IEdmOperation declaredBoundOperation in source.Where<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.HasEquivalentBindingType(bindingType))))
          yield return declaredBoundOperation;
      }
    }

    public virtual IEnumerable<IEdmOperation> FindDeclaredBoundOperations(
      string qualifiedName,
      IEdmType bindingType)
    {
      IEnumerable<IEdmOperation> declaredOperations = this.FindDeclaredOperations(qualifiedName);
      if (!(declaredOperations is IList<IEdmOperation> edmOperationList))
        return declaredOperations.Where<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.HasEquivalentBindingType(bindingType)));
      IList<IEdmOperation> declaredBoundOperations = (IList<IEdmOperation>) new List<IEdmOperation>();
      for (int index = 0; index < edmOperationList.Count; ++index)
      {
        if (edmOperationList[index].HasEquivalentBindingType(bindingType))
          declaredBoundOperations.Add(edmOperationList[index]);
      }
      return (IEnumerable<IEdmOperation>) declaredBoundOperations;
    }

    public virtual IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(
      IEdmVocabularyAnnotatable element)
    {
      return Enumerable.Empty<IEdmVocabularyAnnotation>();
    }

    public abstract IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(
      IEdmStructuredType baseType);

    protected void RegisterElement(IEdmSchemaElement element)
    {
      EdmUtil.CheckArgumentNull<IEdmSchemaElement>(element, nameof (element));
      RegistrationHelper.RegisterSchemaElement(element, this.schemaTypeDictionary, this.termDictionary, this.functionDictionary, this.containersDictionary);
    }

    protected void AddReferencedModel(IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      this.referencedEdmModels.Add(model);
    }
  }
}
