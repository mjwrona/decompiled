// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.NavigationPropertyExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  internal static class NavigationPropertyExtensions
  {
    public static void FindAllNavigationProperties(
      this ODataModelBuilder builder,
      StructuralTypeConfiguration configuration,
      IList<Tuple<StructuralTypeConfiguration, IList<MemberInfo>, NavigationPropertyConfiguration>> navigations,
      Stack<MemberInfo> path)
    {
      builder.FindAllNavigationPropertiesRecursive(configuration, navigations, path, new HashSet<Type>());
    }

    private static void FindAllNavigationPropertiesRecursive(
      this ODataModelBuilder builder,
      StructuralTypeConfiguration configuration,
      IList<Tuple<StructuralTypeConfiguration, IList<MemberInfo>, NavigationPropertyConfiguration>> navigations,
      Stack<MemberInfo> path,
      HashSet<Type> typesAlreadyProcessed)
    {
      if (builder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (builder));
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      if (navigations == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigations));
      if (path == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (path));
      foreach (StructuralTypeConfiguration thisAndBaseType in configuration.ThisAndBaseTypes())
        builder.FindNavigationProperties(thisAndBaseType, navigations, path, typesAlreadyProcessed);
      foreach (StructuralTypeConfiguration derivedType in builder.DerivedTypes(configuration))
      {
        StructuralTypeConfiguration config = derivedType;
        if (!path.OfType<Type>().Any<Type>((Func<Type, bool>) (p => p == config.ClrType)))
        {
          path.Push(TypeHelper.AsMemberInfo(config.ClrType));
          builder.FindNavigationProperties(config, navigations, path, typesAlreadyProcessed);
          path.Pop();
        }
      }
    }

    private static void FindNavigationProperties(
      this ODataModelBuilder builder,
      StructuralTypeConfiguration configuration,
      IList<Tuple<StructuralTypeConfiguration, IList<MemberInfo>, NavigationPropertyConfiguration>> navs,
      Stack<MemberInfo> path,
      HashSet<Type> typesAlreadyProcessed)
    {
      foreach (PropertyConfiguration property in configuration.Properties)
      {
        path.Push((MemberInfo) property.PropertyInfo);
        NavigationPropertyConfiguration propertyConfiguration1 = property as NavigationPropertyConfiguration;
        ComplexPropertyConfiguration propertyConfiguration2 = property as ComplexPropertyConfiguration;
        CollectionPropertyConfiguration propertyConfiguration3 = property as CollectionPropertyConfiguration;
        if (propertyConfiguration1 != null)
        {
          IList<MemberInfo> list = (IList<MemberInfo>) path.Reverse<MemberInfo>().ToList<MemberInfo>();
          navs.Add(new Tuple<StructuralTypeConfiguration, IList<MemberInfo>, NavigationPropertyConfiguration>(configuration, list, propertyConfiguration1));
        }
        else if (propertyConfiguration2 != null && !typesAlreadyProcessed.Contains(propertyConfiguration2.RelatedClrType))
        {
          StructuralTypeConfiguration configurationOrNull = builder.GetTypeConfigurationOrNull(propertyConfiguration2.RelatedClrType) as StructuralTypeConfiguration;
          typesAlreadyProcessed.Add(propertyConfiguration2.RelatedClrType);
          builder.FindAllNavigationPropertiesRecursive(configurationOrNull, navs, path, typesAlreadyProcessed);
          typesAlreadyProcessed.Remove(propertyConfiguration2.RelatedClrType);
        }
        else if (propertyConfiguration3 != null && !typesAlreadyProcessed.Contains(propertyConfiguration3.ElementType))
        {
          IEdmTypeConfiguration configurationOrNull = builder.GetTypeConfigurationOrNull(propertyConfiguration3.ElementType);
          if (configurationOrNull != null && configurationOrNull.Kind == EdmTypeKind.Complex)
          {
            StructuralTypeConfiguration configuration1 = (StructuralTypeConfiguration) configurationOrNull;
            typesAlreadyProcessed.Add(propertyConfiguration3.ElementType);
            builder.FindAllNavigationPropertiesRecursive(configuration1, navs, path, typesAlreadyProcessed);
            typesAlreadyProcessed.Remove(propertyConfiguration3.ElementType);
          }
        }
        path.Pop();
      }
    }
  }
}
