// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.EdmExtensionMethods
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal static class EdmExtensionMethods
  {
    public static IEdmNavigationSource FindNavigationTarget(
      this IEdmNavigationSource navigationSource,
      IEdmNavigationProperty navigationProperty,
      Func<IEdmPathExpression, bool> matchBindingPath)
    {
      if (navigationProperty.ContainsTarget)
        return navigationSource.FindNavigationTarget(navigationProperty);
      IEnumerable<IEdmNavigationPropertyBinding> propertyBindings = navigationSource.FindNavigationPropertyBindings(navigationProperty);
      if (propertyBindings != null)
      {
        foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in propertyBindings)
        {
          if (matchBindingPath(navigationPropertyBinding.Path))
            return navigationPropertyBinding.Target;
        }
      }
      return (IEdmNavigationSource) new UnknownEntitySet(navigationSource, navigationProperty);
    }

    public static IEdmNavigationSource FindNavigationTarget(
      this IEdmNavigationSource navigationSource,
      IEdmNavigationProperty navigationProperty,
      Func<IEdmPathExpression, List<ODataPathSegment>, bool> matchBindingPath,
      List<ODataPathSegment> parsedSegments,
      out IEdmPathExpression bindingPath)
    {
      bindingPath = (IEdmPathExpression) null;
      if (navigationProperty.ContainsTarget)
        return navigationSource.FindNavigationTarget(navigationProperty);
      IEnumerable<IEdmNavigationPropertyBinding> propertyBindings = navigationSource.FindNavigationPropertyBindings(navigationProperty);
      if (propertyBindings != null)
      {
        foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in propertyBindings)
        {
          if (matchBindingPath(navigationPropertyBinding.Path, parsedSegments))
          {
            bindingPath = navigationPropertyBinding.Path;
            return navigationPropertyBinding.Target;
          }
        }
      }
      return (IEdmNavigationSource) new UnknownEntitySet(navigationSource, navigationProperty);
    }

    public static bool HasKey(
      IEdmNavigationSource currentNavigationSource,
      IEdmStructuredType currentResourceType)
    {
      if (currentResourceType is IEdmComplexType)
        return false;
      switch (currentNavigationSource)
      {
        case IEdmEntitySet _:
          return true;
        case IEdmContainedEntitySet containedEntitySet:
          if (containedEntitySet.NavigationProperty.Type.TypeKind() == EdmTypeKind.Collection)
            return true;
          break;
      }
      return false;
    }
  }
}
