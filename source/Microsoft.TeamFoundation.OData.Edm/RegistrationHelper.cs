// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.RegistrationHelper
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal static class RegistrationHelper
  {
    internal static void RegisterSchemaElement(
      IEdmSchemaElement element,
      Dictionary<string, IEdmSchemaType> schemaTypeDictionary,
      Dictionary<string, IEdmTerm> valueTermDictionary,
      Dictionary<string, IList<IEdmOperation>> functionGroupDictionary,
      Dictionary<string, IEdmEntityContainer> containerDictionary)
    {
      string name = element.FullName();
      switch (element.SchemaElementKind)
      {
        case EdmSchemaElementKind.None:
          throw new InvalidOperationException(Strings.EdmModel_CannotUseElementWithTypeNone);
        case EdmSchemaElementKind.TypeDefinition:
          RegistrationHelper.AddElement<IEdmSchemaType>((IEdmSchemaType) element, name, schemaTypeDictionary, new Func<IEdmSchemaType, IEdmSchemaType, IEdmSchemaType>(RegistrationHelper.CreateAmbiguousTypeBinding));
          break;
        case EdmSchemaElementKind.Term:
          RegistrationHelper.AddElement<IEdmTerm>((IEdmTerm) element, name, valueTermDictionary, new Func<IEdmTerm, IEdmTerm, IEdmTerm>(RegistrationHelper.CreateAmbiguousTermBinding));
          break;
        case EdmSchemaElementKind.Action:
        case EdmSchemaElementKind.Function:
          RegistrationHelper.AddOperation((IEdmOperation) element, name, functionGroupDictionary);
          break;
        case EdmSchemaElementKind.EntityContainer:
          if (containerDictionary.Count > 0)
            throw new InvalidOperationException(Strings.EdmModel_CannotAddMoreThanOneEntityContainerToOneEdmModel);
          IEdmEntityContainer element1 = (IEdmEntityContainer) element;
          RegistrationHelper.AddElement<IEdmEntityContainer>(element1, name, containerDictionary, new Func<IEdmEntityContainer, IEdmEntityContainer, IEdmEntityContainer>(RegistrationHelper.CreateAmbiguousEntityContainerBinding));
          RegistrationHelper.AddElement<IEdmEntityContainer>(element1, element.Name, containerDictionary, new Func<IEdmEntityContainer, IEdmEntityContainer, IEdmEntityContainer>(RegistrationHelper.CreateAmbiguousEntityContainerBinding));
          break;
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_SchemaElementKind((object) element.SchemaElementKind));
      }
    }

    internal static void RegisterProperty(
      IEdmProperty element,
      string name,
      Dictionary<string, IEdmProperty> dictionary)
    {
      RegistrationHelper.AddElement<IEdmProperty>(element, name, dictionary, new Func<IEdmProperty, IEdmProperty, IEdmProperty>(RegistrationHelper.CreateAmbiguousPropertyBinding));
    }

    internal static void AddElement<T>(
      T element,
      string name,
      Dictionary<string, T> elementDictionary,
      Func<T, T, T> ambiguityCreator)
      where T : class, IEdmElement
    {
      T obj;
      if (elementDictionary.TryGetValue(name, out obj))
        elementDictionary[name] = ambiguityCreator(obj, element);
      else
        elementDictionary[name] = element;
    }

    internal static void AddOperation(
      IEdmOperation operation,
      string name,
      Dictionary<string, IList<IEdmOperation>> operationListDictionary)
    {
      IList<IEdmOperation> edmOperationList = (IList<IEdmOperation>) null;
      if (!operationListDictionary.TryGetValue(name, out edmOperationList))
      {
        edmOperationList = (IList<IEdmOperation>) new List<IEdmOperation>();
        operationListDictionary.Add(name, edmOperationList);
      }
      edmOperationList.Add(operation);
    }

    internal static void AddOperationImport(
      IEdmOperationImport operationImport,
      string name,
      Dictionary<string, object> operationListDictionary)
    {
      object obj = (object) null;
      if (operationListDictionary.TryGetValue(name, out obj))
      {
        if (!(obj is List<IEdmOperationImport> edmOperationImportList))
        {
          IEdmOperationImport edmOperationImport = (IEdmOperationImport) obj;
          edmOperationImportList = new List<IEdmOperationImport>();
          edmOperationImportList.Add(edmOperationImport);
          operationListDictionary[name] = (object) edmOperationImportList;
        }
        edmOperationImportList.Add(operationImport);
      }
      else
        operationListDictionary[name] = (object) operationImport;
    }

    internal static IEdmSchemaType CreateAmbiguousTypeBinding(
      IEdmSchemaType first,
      IEdmSchemaType second)
    {
      if (first == second)
        return first;
      if (!(first is AmbiguousTypeBinding ambiguousTypeBinding))
        return (IEdmSchemaType) new AmbiguousTypeBinding(first, second);
      ambiguousTypeBinding.AddBinding(second);
      return (IEdmSchemaType) ambiguousTypeBinding;
    }

    internal static IEdmTerm CreateAmbiguousTermBinding(IEdmTerm first, IEdmTerm second)
    {
      if (!(first is AmbiguousTermBinding ambiguousTermBinding))
        return (IEdmTerm) new AmbiguousTermBinding(first, second);
      ambiguousTermBinding.AddBinding(second);
      return (IEdmTerm) ambiguousTermBinding;
    }

    internal static IEdmEntitySet CreateAmbiguousEntitySetBinding(
      IEdmEntitySet first,
      IEdmEntitySet second)
    {
      if (!(first is AmbiguousEntitySetBinding entitySetBinding))
        return (IEdmEntitySet) new AmbiguousEntitySetBinding(first, second);
      entitySetBinding.AddBinding(second);
      return (IEdmEntitySet) entitySetBinding;
    }

    internal static IEdmSingleton CreateAmbiguousSingletonBinding(
      IEdmSingleton first,
      IEdmSingleton second)
    {
      if (!(first is AmbiguousSingletonBinding singletonBinding))
        return (IEdmSingleton) new AmbiguousSingletonBinding(first, second);
      singletonBinding.AddBinding(second);
      return (IEdmSingleton) singletonBinding;
    }

    internal static IEdmEntityContainer CreateAmbiguousEntityContainerBinding(
      IEdmEntityContainer first,
      IEdmEntityContainer second)
    {
      if (!(first is AmbiguousEntityContainerBinding containerBinding))
        return (IEdmEntityContainer) new AmbiguousEntityContainerBinding(first, second);
      containerBinding.AddBinding(second);
      return (IEdmEntityContainer) containerBinding;
    }

    private static IEdmProperty CreateAmbiguousPropertyBinding(
      IEdmProperty first,
      IEdmProperty second)
    {
      if (!(first is AmbiguousPropertyBinding ambiguousPropertyBinding))
        return (IEdmProperty) new AmbiguousPropertyBinding(first.DeclaringType, first, second);
      ambiguousPropertyBinding.AddBinding(second);
      return (IEdmProperty) ambiguousPropertyBinding;
    }
  }
}
