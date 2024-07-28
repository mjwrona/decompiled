// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.AssociationSetDiscoveryConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder.Conventions
{
  internal class AssociationSetDiscoveryConvention : INavigationSourceConvention, IConvention
  {
    public void Apply(NavigationSourceConfiguration configuration, ODataModelBuilder model)
    {
      IList<Tuple<StructuralTypeConfiguration, IList<MemberInfo>, NavigationPropertyConfiguration>> navigations = (IList<Tuple<StructuralTypeConfiguration, IList<MemberInfo>, NavigationPropertyConfiguration>>) new List<Tuple<StructuralTypeConfiguration, IList<MemberInfo>, NavigationPropertyConfiguration>>();
      Stack<MemberInfo> path = new Stack<MemberInfo>();
      model.FindAllNavigationProperties((StructuralTypeConfiguration) configuration.EntityType, navigations, path);
      foreach (Tuple<StructuralTypeConfiguration, IList<MemberInfo>, NavigationPropertyConfiguration> tuple in (IEnumerable<Tuple<StructuralTypeConfiguration, IList<MemberInfo>, NavigationPropertyConfiguration>>) navigations)
      {
        NavigationSourceConfiguration navigationSource = AssociationSetDiscoveryConvention.GetTargetNavigationSource(tuple.Item3, model);
        if (navigationSource != null)
          configuration.AddBinding(tuple.Item3, navigationSource, tuple.Item2);
      }
    }

    internal static NavigationSourceConfiguration GetTargetNavigationSource(
      NavigationPropertyConfiguration navigationProperty,
      ODataModelBuilder model)
    {
      EntityTypeConfiguration targetEntityType = model.StructuralTypes.OfType<EntityTypeConfiguration>().SingleOrDefault<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => e.ClrType == navigationProperty.RelatedClrType));
      if (targetEntityType == null)
        throw Error.InvalidOperation(SRResources.TargetEntityTypeMissing, (object) navigationProperty.Name, (object) TypeHelper.GetReflectedType((MemberInfo) navigationProperty.PropertyInfo).FullName);
      bool isSingleton = navigationProperty.PropertyInfo.GetCustomAttributes<SingletonAttribute>().Any<SingletonAttribute>();
      return AssociationSetDiscoveryConvention.GetDefaultNavigationSource(targetEntityType, model, isSingleton);
    }

    private static NavigationSourceConfiguration GetDefaultNavigationSource(
      EntityTypeConfiguration targetEntityType,
      ODataModelBuilder model,
      bool isSingleton)
    {
      if (targetEntityType == null)
        return (NavigationSourceConfiguration) null;
      NavigationSourceConfiguration[] sourceConfigurationArray = !isSingleton ? (NavigationSourceConfiguration[]) model.EntitySets.Where<EntitySetConfiguration>((Func<EntitySetConfiguration, bool>) (e => e.EntityType == targetEntityType)).ToArray<EntitySetConfiguration>() : (NavigationSourceConfiguration[]) model.Singletons.Where<SingletonConfiguration>((Func<SingletonConfiguration, bool>) (e => e.EntityType == targetEntityType)).ToArray<SingletonConfiguration>();
      return sourceConfigurationArray.Length > 1 ? (model.BindingOptions == NavigationPropertyBindingOption.Auto ? sourceConfigurationArray[0] : (NavigationSourceConfiguration) null) : (sourceConfigurationArray.Length == 1 ? sourceConfigurationArray[0] : AssociationSetDiscoveryConvention.GetDefaultNavigationSource(targetEntityType.BaseType, model, isSingleton));
    }
  }
}
