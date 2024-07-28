// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.ValidationHelper
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Edm.Validation
{
  internal static class ValidationHelper
  {
    internal static bool IsEdmSystemNamespace(string namespaceName) => namespaceName == "Transient" || namespaceName == "Edm";

    internal static bool AddMemberNameToHashSet(
      IEdmNamedElement item,
      HashSetInternal<string> memberNameList,
      ValidationContext context,
      EdmErrorCode errorCode,
      string errorString,
      bool suppressError)
    {
      string thingToAdd = item is IEdmSchemaElement element ? element.FullName() : item.Name;
      if (memberNameList.Add(thingToAdd))
        return true;
      if (!suppressError)
        context.AddError(item.Location(), errorCode, errorString);
      return false;
    }

    internal static bool AllPropertiesAreNullable(IEnumerable<IEdmStructuralProperty> properties) => properties.Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (p => !p.Type.IsNullable)).Count<IEdmStructuralProperty>() == 0;

    internal static bool HasNullableProperty(IEnumerable<IEdmStructuralProperty> properties) => properties.Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (p => p.Type.IsNullable)).Count<IEdmStructuralProperty>() > 0;

    internal static bool PropertySetIsSubset(
      IEnumerable<IEdmStructuralProperty> set,
      IEnumerable<IEdmStructuralProperty> subset)
    {
      return subset.Except<IEdmStructuralProperty>(set).Count<IEdmStructuralProperty>() <= 0;
    }

    internal static bool PropertySetsAreEquivalent(
      IEnumerable<IEdmStructuralProperty> set1,
      IEnumerable<IEdmStructuralProperty> set2)
    {
      if (set1.Count<IEdmStructuralProperty>() != set2.Count<IEdmStructuralProperty>())
        return false;
      IEnumerator<IEdmStructuralProperty> enumerator = set2.GetEnumerator();
      foreach (IEdmStructuralProperty structuralProperty in set1)
      {
        enumerator.MoveNext();
        if (structuralProperty != enumerator.Current)
          return false;
      }
      return true;
    }

    internal static bool ValidateValueCanBeWrittenAsXmlElementAnnotation(
      IEdmValue value,
      string annotationNamespace,
      string annotationName,
      out EdmError error)
    {
      if (!(value is IEdmStringValue edmStringValue))
      {
        error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationNotIEdmStringValue);
        return false;
      }
      XmlReader xmlReader = XmlReader.Create((TextReader) new StringReader(edmStringValue.Value));
      try
      {
        if (xmlReader.NodeType != XmlNodeType.Element)
        {
          while (xmlReader.Read() && xmlReader.NodeType != XmlNodeType.Element)
            ;
        }
        if (xmlReader.EOF)
        {
          error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationValueInvalidXml);
          return false;
        }
        string namespaceUri = xmlReader.NamespaceURI;
        string localName = xmlReader.LocalName;
        if (EdmUtil.IsNullOrWhiteSpaceInternal(namespaceUri) || EdmUtil.IsNullOrWhiteSpaceInternal(localName))
        {
          error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationNullNamespaceOrName);
          return false;
        }
        if (annotationNamespace != null && !(namespaceUri == annotationNamespace) || annotationName != null && !(localName == annotationName))
        {
          error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationMismatchedTerm);
          return false;
        }
        do
          ;
        while (xmlReader.Read());
        error = (EdmError) null;
        return true;
      }
      catch (XmlException ex)
      {
        error = new EdmError(value.Location(), EdmErrorCode.InvalidElementAnnotation, Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationValueInvalidXml);
        return false;
      }
    }

    internal static bool IsInterfaceCritical(EdmError error) => error.ErrorCode >= EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull && error.ErrorCode <= EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy;

    internal static bool ItemExistsInReferencedModel(
      this IEdmModel model,
      string fullName,
      bool checkEntityContainer)
    {
      foreach (IEdmModel referencedModel in model.ReferencedModels)
      {
        if (referencedModel.FindDeclaredType(fullName) != null || referencedModel.FindDeclaredTerm(fullName) != null || checkEntityContainer && referencedModel.ExistsContainer(fullName) || (referencedModel.FindDeclaredOperations(fullName) ?? Enumerable.Empty<IEdmOperation>()).FirstOrDefault<IEdmOperation>() != null)
          return true;
      }
      return false;
    }

    internal static bool OperationOrNameExistsInReferencedModel(
      this IEdmModel model,
      IEdmOperation operation,
      string operationFullName)
    {
      foreach (IEdmModel referencedModel in model.ReferencedModels)
      {
        if (referencedModel.FindDeclaredType(operationFullName) != null || referencedModel.ExistsContainer(operationFullName) || referencedModel.FindDeclaredTerm(operationFullName) != null)
          return true;
        IEnumerable<IEdmOperation> candidateDuplicateOperations = referencedModel.FindDeclaredOperations(operationFullName) ?? Enumerable.Empty<IEdmOperation>();
        if (DuplicateOperationValidator.IsDuplicateOperation(operation, candidateDuplicateOperations))
          return true;
      }
      return false;
    }

    internal static bool TypeIndirectlyContainsTarget(
      IEdmEntityType source,
      IEdmEntityType target,
      HashSetInternal<IEdmEntityType> visited,
      IEdmModel context)
    {
      if (visited.Add(source))
      {
        if (source.IsOrInheritsFrom((IEdmType) target))
          return true;
        foreach (IEdmNavigationProperty navigationProperty in source.NavigationProperties())
        {
          if (navigationProperty.ContainsTarget && ValidationHelper.TypeIndirectlyContainsTarget(navigationProperty.ToEntityType(), target, visited, context))
            return true;
        }
        foreach (IEdmStructuredType allDerivedType in context.FindAllDerivedTypes((IEdmStructuredType) source))
        {
          if (allDerivedType is IEdmEntityType source1 && ValidationHelper.TypeIndirectlyContainsTarget(source1, target, visited, context))
            return true;
        }
      }
      return false;
    }

    internal static IEdmEntityType ComputeNavigationPropertyTarget(IEdmNavigationProperty property)
    {
      IEdmType definition = property.Type.Definition;
      if (definition.TypeKind == EdmTypeKind.Collection)
        definition = ((IEdmCollectionType) definition).ElementType.Definition;
      return (IEdmEntityType) definition;
    }
  }
}
