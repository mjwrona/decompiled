// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UriEdmHelpers
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;

namespace Microsoft.OData.UriParser
{
  internal static class UriEdmHelpers
  {
    public static IEdmSchemaType FindTypeFromModel(
      IEdmModel model,
      string qualifiedName,
      ODataUriResolver resolver)
    {
      return resolver.ResolveType(model, qualifiedName);
    }

    public static IEdmEnumType FindEnumTypeFromModel(IEdmModel model, string qualifiedName) => UriEdmHelpers.FindTypeFromModel(model, qualifiedName, ODataUriResolver.GetUriResolver((IServiceProvider) null)) as IEdmEnumType;

    public static void CheckRelatedTo(IEdmType parentType, IEdmType childType)
    {
      if (!UriEdmHelpers.IsRelatedTo(parentType, childType))
      {
        string p1 = parentType != null ? parentType.FullTypeName() : "<null>";
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_HierarchyNotFollowed((object) childType.FullTypeName(), (object) p1));
      }
    }

    public static bool IsRelatedTo(IEdmType first, IEdmType second) => second.IsOrInheritsFrom(first) || first.IsOrInheritsFrom(second);

    public static IEdmNavigationProperty GetNavigationPropertyFromExpandPath(ODataPath path)
    {
      NavigationPropertySegment navigationPropertySegment = (NavigationPropertySegment) null;
      foreach (ODataPathSegment odataPathSegment in path)
      {
        TypeSegment typeSegment = odataPathSegment as TypeSegment;
        navigationPropertySegment = odataPathSegment as NavigationPropertySegment;
        if (typeSegment == null && navigationPropertySegment == null)
          throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
      }
      return navigationPropertySegment != null ? navigationPropertySegment.NavigationProperty : throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
    }

    public static IEdmType GetMostDerivedTypeFromPath(ODataPath path, IEdmType startingType)
    {
      IEdmType otherType = startingType;
      foreach (ODataPathSegment odataPathSegment in path)
      {
        if (odataPathSegment is TypeSegment typeSegment && typeSegment.EdmType.IsOrInheritsFrom(otherType))
          otherType = typeSegment.EdmType;
      }
      return otherType;
    }

    public static bool IsStructuredCollection(this IEdmTypeReference type)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(type, nameof (type));
      return type is IEdmCollectionTypeReference type1 && type1.ElementType().IsStructured();
    }

    public static bool IsEntityCollection(this IEdmTypeReference type)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(type, nameof (type));
      return type is IEdmCollectionTypeReference type1 && type1.ElementType().IsEntity();
    }

    public static IEdmStructuredTypeReference GetTypeReference(
      this IEdmStructuredType structuredType)
    {
      return !(structuredType is IEdmEntityType entityType) ? (IEdmStructuredTypeReference) new EdmComplexTypeReference(structuredType as IEdmComplexType, false) : (IEdmStructuredTypeReference) new EdmEntityTypeReference(entityType, false);
    }

    public static bool IsBindingTypeValid(IEdmType bindingType) => bindingType == null || bindingType.IsEntityOrEntityCollectionType() || bindingType.IsODataComplexTypeKind();

    internal static IEdmEnumType FindEnumTypeFromModel(
      IEdmModel model,
      string qualifiedName,
      ODataUriResolver resolver)
    {
      return UriEdmHelpers.FindTypeFromModel(model, qualifiedName, resolver ?? ODataUriResolver.GetUriResolver((IServiceProvider) null)) as IEdmEnumType;
    }
  }
}
