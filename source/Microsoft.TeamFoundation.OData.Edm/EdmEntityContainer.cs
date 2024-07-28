// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmEntityContainer
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  public class EdmEntityContainer : 
    EdmElement,
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
    private readonly List<IEdmEntityContainerElement> containerElements = new List<IEdmEntityContainerElement>();
    private readonly Dictionary<string, IEdmEntitySet> entitySetDictionary = new Dictionary<string, IEdmEntitySet>();
    private readonly Dictionary<string, IEdmSingleton> singletonDictionary = new Dictionary<string, IEdmSingleton>();
    private readonly Dictionary<string, object> operationImportDictionary = new Dictionary<string, object>();

    public EdmEntityContainer(string namespaceName, string name)
    {
      EdmUtil.CheckArgumentNull<string>(namespaceName, nameof (namespaceName));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.namespaceName = namespaceName;
      this.name = name;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.Name);
    }

    public IEnumerable<IEdmEntityContainerElement> Elements => (IEnumerable<IEdmEntityContainerElement>) this.containerElements;

    public string Namespace => this.namespaceName;

    public string Name => this.name;

    public string FullName => this.fullName;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.EntityContainer;

    public void AddElement(IEdmEntityContainerElement element)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityContainerElement>(element, nameof (element));
      this.containerElements.Add(element);
      switch (element.ContainerElementKind)
      {
        case EdmContainerElementKind.None:
          throw new InvalidOperationException(Strings.EdmEntityContainer_CannotUseElementWithTypeNone);
        case EdmContainerElementKind.EntitySet:
          RegistrationHelper.AddElement<IEdmEntitySet>((IEdmEntitySet) element, element.Name, this.entitySetDictionary, new Func<IEdmEntitySet, IEdmEntitySet, IEdmEntitySet>(RegistrationHelper.CreateAmbiguousEntitySetBinding));
          break;
        case EdmContainerElementKind.ActionImport:
        case EdmContainerElementKind.FunctionImport:
          RegistrationHelper.AddOperationImport((IEdmOperationImport) element, element.Name, this.operationImportDictionary);
          break;
        case EdmContainerElementKind.Singleton:
          RegistrationHelper.AddElement<IEdmSingleton>((IEdmSingleton) element, element.Name, this.singletonDictionary, new Func<IEdmSingleton, IEdmSingleton, IEdmSingleton>(RegistrationHelper.CreateAmbiguousSingletonBinding));
          break;
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_ContainerElementKind((object) element.ContainerElementKind));
      }
    }

    public virtual EdmEntitySet AddEntitySet(string name, IEdmEntityType elementType)
    {
      EdmEntitySet element = new EdmEntitySet((IEdmEntityContainer) this, name, elementType);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual EdmEntitySet AddEntitySet(
      string name,
      IEdmEntityType elementType,
      bool includeInServiceDocument)
    {
      EdmEntitySet element = new EdmEntitySet((IEdmEntityContainer) this, name, elementType, includeInServiceDocument);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual EdmSingleton AddSingleton(string name, IEdmEntityType entityType)
    {
      EdmSingleton element = new EdmSingleton((IEdmEntityContainer) this, name, entityType);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual EdmFunctionImport AddFunctionImport(IEdmFunction function)
    {
      EdmFunctionImport element = new EdmFunctionImport((IEdmEntityContainer) this, function.Name, function);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual EdmFunctionImport AddFunctionImport(string name, IEdmFunction function)
    {
      EdmFunctionImport element = new EdmFunctionImport((IEdmEntityContainer) this, name, function);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual EdmFunctionImport AddFunctionImport(
      string name,
      IEdmFunction function,
      IEdmExpression entitySet)
    {
      EdmFunctionImport element = new EdmFunctionImport((IEdmEntityContainer) this, name, function, entitySet, false);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual EdmOperationImport AddFunctionImport(
      string name,
      IEdmFunction function,
      IEdmExpression entitySet,
      bool includeInServiceDocument)
    {
      EdmOperationImport element = (EdmOperationImport) new EdmFunctionImport((IEdmEntityContainer) this, name, function, entitySet, includeInServiceDocument);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual EdmActionImport AddActionImport(
      string name,
      IEdmAction action,
      IEdmExpression entitySet)
    {
      EdmActionImport element = new EdmActionImport((IEdmEntityContainer) this, name, action, entitySet);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual EdmActionImport AddActionImport(IEdmAction action)
    {
      EdmActionImport element = new EdmActionImport((IEdmEntityContainer) this, action.Name, action, (IEdmExpression) null);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual EdmActionImport AddActionImport(string name, IEdmAction action)
    {
      EdmActionImport element = new EdmActionImport((IEdmEntityContainer) this, name, action, (IEdmExpression) null);
      this.AddElement((IEdmEntityContainerElement) element);
      return element;
    }

    public virtual IEdmEntitySet FindEntitySet(string setName)
    {
      if (string.IsNullOrEmpty(setName))
        return (IEdmEntitySet) null;
      IEdmEntitySet edmEntitySet;
      return !this.entitySetDictionary.TryGetValue(setName, out edmEntitySet) ? (IEdmEntitySet) null : edmEntitySet;
    }

    public virtual IEdmSingleton FindSingleton(string singletonName)
    {
      IEdmSingleton edmSingleton;
      return !this.singletonDictionary.TryGetValue(singletonName, out edmSingleton) ? (IEdmSingleton) null : edmSingleton;
    }

    public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
    {
      object obj;
      if (!this.operationImportDictionary.TryGetValue(operationName, out obj))
        return Enumerable.Empty<IEdmOperationImport>();
      if (obj is List<IEdmOperationImport> operationImports)
        return (IEnumerable<IEdmOperationImport>) operationImports;
      return (IEnumerable<IEdmOperationImport>) new IEdmOperationImport[1]
      {
        (IEdmOperationImport) obj
      };
    }
  }
}
