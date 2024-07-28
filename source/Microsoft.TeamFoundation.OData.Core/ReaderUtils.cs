// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ReaderUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
  internal static class ReaderUtils
  {
    internal static EdmTypeKind GetExpectedTypeKind(
      IEdmTypeReference expectedTypeReference,
      bool enablePrimitiveTypeConversion)
    {
      IEdmType definition;
      if (expectedTypeReference == null || (definition = expectedTypeReference.Definition) == null)
        return EdmTypeKind.None;
      EdmTypeKind typeKind = definition.TypeKind;
      return !enablePrimitiveTypeConversion && typeKind == EdmTypeKind.Primitive && !definition.IsStream() ? EdmTypeKind.None : typeKind;
    }

    internal static ODataResource CreateNewResource()
    {
      ODataResource newResource = new ODataResource();
      newResource.Properties = (IEnumerable<ODataProperty>) new ReadOnlyEnumerable<ODataProperty>();
      return newResource;
    }

    internal static ODataDeletedResource CreateDeletedResource(
      Uri id,
      DeltaDeletedEntryReason reason)
    {
      ODataDeletedResource deletedResource = new ODataDeletedResource(id, reason);
      deletedResource.Properties = (IEnumerable<ODataProperty>) new ReadOnlyEnumerable<ODataProperty>();
      return deletedResource;
    }

    internal static void CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      ODataNestedResourceInfo nestedResourceInfo)
    {
      Uri associationLink = propertyAndAnnotationCollector.ValidatePropertyUniquenessAndGetAssociationLink(nestedResourceInfo);
      if (!(associationLink != (Uri) null) || !(nestedResourceInfo.AssociationLinkUrl == (Uri) null))
        return;
      nestedResourceInfo.AssociationLinkUrl = associationLink;
    }

    internal static void CheckForDuplicateAssociationLinkAndUpdateNestedResourceInfo(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      string associationLinkName,
      Uri associationLinkUrl)
    {
      ODataNestedResourceInfo nestedResourceInfo = propertyAndAnnotationCollector.ValidatePropertyOpenForAssociationLinkAndGetNestedResourceInfo(associationLinkName, associationLinkUrl);
      if (nestedResourceInfo == null || !(nestedResourceInfo.AssociationLinkUrl == (Uri) null) || !(associationLinkUrl != (Uri) null))
        return;
      nestedResourceInfo.AssociationLinkUrl = associationLinkUrl;
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Ignoring violation because of Debug.Assert.")]
    internal static string GetExpectedPropertyName(IEdmStructuralProperty expectedProperty) => expectedProperty?.Name;

    internal static string RemovePrefixOfTypeName(string typeName)
    {
      string str = typeName;
      if (!string.IsNullOrEmpty(typeName) && typeName.StartsWith("#", StringComparison.Ordinal))
        str = typeName.Substring("#".Length);
      return str;
    }

    internal static string AddEdmPrefixOfTypeName(string typeName)
    {
      if (!string.IsNullOrEmpty(typeName))
      {
        string collectionItemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(typeName);
        if (collectionItemTypeName == null)
        {
          IEdmSchemaType element = EdmLibraryExtensions.ResolvePrimitiveTypeName(typeName);
          if (element != null)
            return element.FullName();
        }
        else
        {
          IEdmSchemaType element = EdmLibraryExtensions.ResolvePrimitiveTypeName(collectionItemTypeName);
          if (element != null)
            return EdmLibraryExtensions.GetCollectionTypeName(element.FullName());
        }
      }
      return typeName;
    }
  }
}
