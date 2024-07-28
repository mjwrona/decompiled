// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.ForeignKeyAttributeConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class ForeignKeyAttributeConvention : 
    AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    public ForeignKeyAttributeConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (ForeignKeyAttribute)), false)
    {
    }

    public override void Apply(
      PropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      Attribute attribute,
      ODataConventionModelBuilder model)
    {
      if (edmProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmProperty));
      if (structuralTypeConfiguration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuralTypeConfiguration));
      if (attribute == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (attribute));
      if (!(structuralTypeConfiguration is EntityTypeConfiguration entityType))
        return;
      ForeignKeyAttribute foreignKeyAttribute = (ForeignKeyAttribute) attribute;
      switch (edmProperty.Kind)
      {
        case PropertyKind.Primitive:
          ForeignKeyAttributeConvention.ApplyPrimitive((PrimitivePropertyConfiguration) edmProperty, entityType, foreignKeyAttribute);
          break;
        case PropertyKind.Navigation:
          ForeignKeyAttributeConvention.ApplyNavigation((NavigationPropertyConfiguration) edmProperty, entityType, foreignKeyAttribute);
          break;
      }
    }

    private static void ApplyNavigation(
      NavigationPropertyConfiguration navProperty,
      EntityTypeConfiguration entityType,
      ForeignKeyAttribute foreignKeyAttribute)
    {
      if (navProperty.AddedExplicitly || navProperty.Multiplicity == EdmMultiplicity.Many)
        return;
      EntityTypeConfiguration typeConfiguration = entityType.ModelBuilder.StructuralTypes.OfType<EntityTypeConfiguration>().FirstOrDefault<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => e.ClrType == navProperty.RelatedClrType));
      if (typeConfiguration == null)
        return;
      foreach (string str in ((IEnumerable<string>) foreignKeyAttribute.Name.Split(',')).Select<string, string>((Func<string, string>) (p => p.Trim())))
      {
        string dependentPropertyName = str;
        if (!string.IsNullOrWhiteSpace(dependentPropertyName))
        {
          PrimitivePropertyConfiguration propertyConfiguration1 = entityType.Properties.OfType<PrimitivePropertyConfiguration>().SingleOrDefault<PrimitivePropertyConfiguration>((Func<PrimitivePropertyConfiguration, bool>) (p => p.Name.Equals(dependentPropertyName, StringComparison.Ordinal)));
          if (propertyConfiguration1 != null)
          {
            Type type = Nullable.GetUnderlyingType(propertyConfiguration1.PropertyInfo.PropertyType);
            if ((object) type == null)
              type = propertyConfiguration1.PropertyInfo.PropertyType;
            Type dependentType = type;
            PrimitivePropertyConfiguration propertyConfiguration2 = typeConfiguration.Keys.FirstOrDefault<PrimitivePropertyConfiguration>((Func<PrimitivePropertyConfiguration, bool>) (k => k.PropertyInfo.PropertyType == dependentType && navProperty.PrincipalProperties.All<PropertyInfo>((Func<PropertyInfo, bool>) (p => p != k.PropertyInfo))));
            if (propertyConfiguration2 != null)
              navProperty.HasConstraint(propertyConfiguration1.PropertyInfo, propertyConfiguration2.PropertyInfo);
          }
        }
      }
    }

    private static void ApplyPrimitive(
      PrimitivePropertyConfiguration dependent,
      EntityTypeConfiguration entityType,
      ForeignKeyAttribute foreignKeyAttribute)
    {
      string navName = foreignKeyAttribute.Name.Trim();
      NavigationPropertyConfiguration navProperty = entityType.NavigationProperties.FirstOrDefault<NavigationPropertyConfiguration>((Func<NavigationPropertyConfiguration, bool>) (n => n.Name.Equals(navName, StringComparison.Ordinal)));
      if (navProperty == null || navProperty.Multiplicity == EdmMultiplicity.Many || navProperty.AddedExplicitly)
        return;
      EntityTypeConfiguration typeConfiguration = entityType.ModelBuilder.StructuralTypes.OfType<EntityTypeConfiguration>().FirstOrDefault<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => e.ClrType == navProperty.RelatedClrType));
      if (typeConfiguration == null)
        return;
      Type type = Nullable.GetUnderlyingType(dependent.PropertyInfo.PropertyType);
      if ((object) type == null)
        type = dependent.PropertyInfo.PropertyType;
      Type dependentType = type;
      PrimitivePropertyConfiguration propertyConfiguration = typeConfiguration.Keys.FirstOrDefault<PrimitivePropertyConfiguration>((Func<PrimitivePropertyConfiguration, bool>) (k => k.PropertyInfo.PropertyType == dependentType && navProperty.PrincipalProperties.All<PropertyInfo>((Func<PropertyInfo, bool>) (p => p != k.PropertyInfo))));
      if (propertyConfiguration == null)
        return;
      navProperty.HasConstraint(dependent.PropertyInfo, propertyConfiguration.PropertyInfo);
    }
  }
}
