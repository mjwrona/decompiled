// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsEntityContainer
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsEntityContainer : 
    CsdlSemanticsElement,
    IEdmEntityContainer,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmCheckable,
    IEdmFullNamedElement
  {
    private readonly string fullName;
    private readonly CsdlEntityContainer entityContainer;
    private readonly CsdlSemanticsSchema context;
    private readonly Cache<CsdlSemanticsEntityContainer, IEnumerable<IEdmEntityContainerElement>> elementsCache = new Cache<CsdlSemanticsEntityContainer, IEnumerable<IEdmEntityContainerElement>>();
    private static readonly Func<CsdlSemanticsEntityContainer, IEnumerable<IEdmEntityContainerElement>> ComputeElementsFunc = (Func<CsdlSemanticsEntityContainer, IEnumerable<IEdmEntityContainerElement>>) (me => me.ComputeElements());
    private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>> entitySetDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>>();
    private static readonly Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>> ComputeEntitySetDictionaryFunc = (Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>>) (me => me.ComputeEntitySetDictionary());
    private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmSingleton>> singletonDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmSingleton>>();
    private static readonly Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmSingleton>> ComputeSingletonDictionaryFunc = (Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmSingleton>>) (me => me.ComputeSingletonDictionary());
    private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, object>> operationImportsDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, object>>();
    private static readonly Func<CsdlSemanticsEntityContainer, Dictionary<string, object>> ComputeOperationImportsDictionaryFunc = (Func<CsdlSemanticsEntityContainer, Dictionary<string, object>>) (me => me.ComputeOperationImportsDictionary());
    private readonly Cache<CsdlSemanticsEntityContainer, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsEntityContainer, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsEntityContainer, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsEntityContainer, IEnumerable<EdmError>>) (me => me.ComputeErrors());
    private readonly Cache<CsdlSemanticsEntityContainer, IEdmEntityContainer> extendsCache = new Cache<CsdlSemanticsEntityContainer, IEdmEntityContainer>();
    private static readonly Func<CsdlSemanticsEntityContainer, IEdmEntityContainer> ComputeExtendsFunc = (Func<CsdlSemanticsEntityContainer, IEdmEntityContainer>) (me => me.ComputeExtends());
    private static readonly Func<CsdlSemanticsEntityContainer, IEdmEntityContainer> OnCycleExtendsFunc = (Func<CsdlSemanticsEntityContainer, IEdmEntityContainer>) (me => (IEdmEntityContainer) new CyclicEntityContainer(me.entityContainer.Extends, me.Location));

    public CsdlSemanticsEntityContainer(
      CsdlSemanticsSchema context,
      CsdlEntityContainer entityContainer)
      : base((CsdlElement) entityContainer)
    {
      this.context = context;
      this.entityContainer = entityContainer;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.context?.Namespace, this.entityContainer?.Name);
    }

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.EntityContainer;

    public override CsdlSemanticsModel Model => this.context.Model;

    public IEnumerable<IEdmEntityContainerElement> Elements => this.elementsCache.GetValue(this, CsdlSemanticsEntityContainer.ComputeElementsFunc, (Func<CsdlSemanticsEntityContainer, IEnumerable<IEdmEntityContainerElement>>) null);

    public string Namespace => this.context.Namespace;

    public string Name => this.entityContainer.Name;

    public string FullName => this.fullName;

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsEntityContainer.ComputeErrorsFunc, (Func<CsdlSemanticsEntityContainer, IEnumerable<EdmError>>) null);

    public override CsdlElement Element => (CsdlElement) this.entityContainer;

    internal CsdlSemanticsSchema Context => this.context;

    internal IEdmEntityContainer Extends => this.extendsCache.GetValue(this, CsdlSemanticsEntityContainer.ComputeExtendsFunc, CsdlSemanticsEntityContainer.OnCycleExtendsFunc);

    private Dictionary<string, IEdmEntitySet> EntitySetDictionary => this.entitySetDictionaryCache.GetValue(this, CsdlSemanticsEntityContainer.ComputeEntitySetDictionaryFunc, (Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>>) null);

    private Dictionary<string, IEdmSingleton> SingletonDictionary => this.singletonDictionaryCache.GetValue(this, CsdlSemanticsEntityContainer.ComputeSingletonDictionaryFunc, (Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmSingleton>>) null);

    private Dictionary<string, object> OperationImportsDictionary => this.operationImportsDictionaryCache.GetValue(this, CsdlSemanticsEntityContainer.ComputeOperationImportsDictionaryFunc, (Func<CsdlSemanticsEntityContainer, Dictionary<string, object>>) null);

    public IEdmEntitySet FindEntitySet(string name)
    {
      IEdmEntitySet edmEntitySet;
      return !this.EntitySetDictionary.TryGetValue(name, out edmEntitySet) ? (IEdmEntitySet) null : edmEntitySet;
    }

    public IEdmSingleton FindSingleton(string name)
    {
      IEdmSingleton edmSingleton;
      return !this.SingletonDictionary.TryGetValue(name, out edmSingleton) ? (IEdmSingleton) null : edmSingleton;
    }

    public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
    {
      object obj;
      if (!this.OperationImportsDictionary.TryGetValue(operationName, out obj))
        return Enumerable.Empty<IEdmOperationImport>();
      if (obj is List<IEdmOperationImport> operationImports)
        return (IEnumerable<IEdmOperationImport>) operationImports;
      return (IEnumerable<IEdmOperationImport>) new IEdmOperationImport[1]
      {
        (IEdmOperationImport) obj
      };
    }

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.Context);

    private IEnumerable<IEdmEntityContainerElement> ComputeElements()
    {
      List<IEdmEntityContainerElement> elements = new List<IEdmEntityContainerElement>();
      foreach (CsdlEntitySet entitySet in this.entityContainer.EntitySets)
      {
        CsdlSemanticsEntitySet semanticsEntitySet = new CsdlSemanticsEntitySet(this, entitySet);
        elements.Add((IEdmEntityContainerElement) semanticsEntitySet);
      }
      foreach (CsdlSingleton singleton in this.entityContainer.Singletons)
      {
        CsdlSemanticsSingleton semanticsSingleton = new CsdlSemanticsSingleton(this, singleton);
        elements.Add((IEdmEntityContainerElement) semanticsSingleton);
      }
      foreach (CsdlOperationImport operationImport in this.entityContainer.OperationImports)
        this.AddOperationImport(operationImport, elements);
      return (IEnumerable<IEdmEntityContainerElement>) elements;
    }

    private void AddOperationImport(
      CsdlOperationImport operationImport,
      List<IEdmEntityContainerElement> elements)
    {
      CsdlFunctionImport functionImport = operationImport as CsdlFunctionImport;
      CsdlActionImport actionImport = operationImport as CsdlActionImport;
      EdmSchemaElementKind filterKind = EdmSchemaElementKind.Action;
      if (functionImport != null)
        filterKind = EdmSchemaElementKind.Function;
      IEnumerable<IEdmOperation> edmOperations = this.context.FindOperations(operationImport.SchemaOperationQualifiedTypeName).Where<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.SchemaElementKind == filterKind && !o.IsBound));
      int num = 0;
      foreach (IEdmOperation edmOperation in edmOperations)
      {
        CsdlSemanticsOperationImport semanticsOperationImport = functionImport == null ? (CsdlSemanticsOperationImport) new CsdlSemanticsActionImport(this, actionImport, (IEdmAction) edmOperation) : (CsdlSemanticsOperationImport) new CsdlSemanticsFunctionImport(this, functionImport, (IEdmFunction) edmOperation);
        ++num;
        elements.Add((IEdmEntityContainerElement) semanticsOperationImport);
      }
      if (num != 0)
        return;
      CsdlSemanticsOperationImport semanticsOperationImport1;
      if (filterKind == EdmSchemaElementKind.Action)
      {
        UnresolvedAction backingAction = new UnresolvedAction(operationImport.SchemaOperationQualifiedTypeName, Strings.Bad_UnresolvedOperation((object) operationImport.SchemaOperationQualifiedTypeName), operationImport.Location);
        semanticsOperationImport1 = (CsdlSemanticsOperationImport) new CsdlSemanticsActionImport(this, actionImport, (IEdmAction) backingAction);
      }
      else
      {
        UnresolvedFunction backingfunction = new UnresolvedFunction(operationImport.SchemaOperationQualifiedTypeName, Strings.Bad_UnresolvedOperation((object) operationImport.SchemaOperationQualifiedTypeName), operationImport.Location);
        semanticsOperationImport1 = (CsdlSemanticsOperationImport) new CsdlSemanticsFunctionImport(this, functionImport, (IEdmFunction) backingfunction);
      }
      elements.Add((IEdmEntityContainerElement) semanticsOperationImport1);
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      List<EdmError> errors = new List<EdmError>();
      if (this.Extends != null && this.Extends.IsBad())
        errors.AddRange(((IEdmCheckable) this.Extends).Errors);
      return (IEnumerable<EdmError>) errors;
    }

    private Dictionary<string, IEdmEntitySet> ComputeEntitySetDictionary()
    {
      Dictionary<string, IEdmEntitySet> elementDictionary = new Dictionary<string, IEdmEntitySet>();
      foreach (IEdmEntitySet element in this.Elements.OfType<IEdmEntitySet>())
        RegistrationHelper.AddElement<IEdmEntitySet>(element, element.Name, elementDictionary, new Func<IEdmEntitySet, IEdmEntitySet, IEdmEntitySet>(RegistrationHelper.CreateAmbiguousEntitySetBinding));
      return elementDictionary;
    }

    private Dictionary<string, IEdmSingleton> ComputeSingletonDictionary()
    {
      Dictionary<string, IEdmSingleton> elementDictionary = new Dictionary<string, IEdmSingleton>();
      foreach (IEdmSingleton element in this.Elements.OfType<IEdmSingleton>())
        RegistrationHelper.AddElement<IEdmSingleton>(element, element.Name, elementDictionary, new Func<IEdmSingleton, IEdmSingleton, IEdmSingleton>(RegistrationHelper.CreateAmbiguousSingletonBinding));
      return elementDictionary;
    }

    private Dictionary<string, object> ComputeOperationImportsDictionary()
    {
      Dictionary<string, object> operationListDictionary = new Dictionary<string, object>();
      foreach (IEdmOperationImport operationImport in this.Elements.OfType<IEdmOperationImport>())
        RegistrationHelper.AddOperationImport(operationImport, operationImport.Name, operationListDictionary);
      return operationListDictionary;
    }

    private IEdmEntityContainer ComputeExtends()
    {
      string extends = this.entityContainer.Extends;
      return extends != null ? this.Context.FindEntityContainer(extends) ?? (IEdmEntityContainer) new UnresolvedEntityContainer(this.entityContainer.Extends, this.Location) : (IEdmEntityContainer) null;
    }
  }
}
