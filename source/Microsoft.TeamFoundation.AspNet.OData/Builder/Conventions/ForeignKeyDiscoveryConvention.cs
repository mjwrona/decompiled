// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.ForeignKeyDiscoveryConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder.Conventions
{
  internal class ForeignKeyDiscoveryConvention : 
    IEdmPropertyConvention<NavigationPropertyConfiguration>,
    IEdmPropertyConvention,
    IConvention
  {
    public void Apply(
      PropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      ODataConventionModelBuilder model)
    {
      if (edmProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmProperty));
      if (!(edmProperty is NavigationPropertyConfiguration edmProperty1))
        return;
      this.Apply(edmProperty1, structuralTypeConfiguration, model);
    }

    public void Apply(
      NavigationPropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      ODataConventionModelBuilder model)
    {
      if (edmProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmProperty));
      if (structuralTypeConfiguration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuralTypeConfiguration));
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      EntityTypeConfiguration principalEntityType = model.StructuralTypes.OfType<EntityTypeConfiguration>().FirstOrDefault<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => e.ClrType == edmProperty.RelatedClrType));
      if (principalEntityType == null || edmProperty.DependentProperties.Any<PropertyInfo>() || edmProperty.Multiplicity == EdmMultiplicity.Many || !(structuralTypeConfiguration is EntityTypeConfiguration dependentEntityType))
        return;
      IDictionary<PrimitivePropertyConfiguration, PrimitivePropertyConfiguration> foreignKeys = ForeignKeyDiscoveryConvention.GetForeignKeys(principalEntityType, dependentEntityType);
      if (!foreignKeys.Any<KeyValuePair<PrimitivePropertyConfiguration, PrimitivePropertyConfiguration>>() || foreignKeys.Count<KeyValuePair<PrimitivePropertyConfiguration, PrimitivePropertyConfiguration>>() != principalEntityType.Keys.Count<PrimitivePropertyConfiguration>())
        return;
      foreach (KeyValuePair<PrimitivePropertyConfiguration, PrimitivePropertyConfiguration> keyValuePair in (IEnumerable<KeyValuePair<PrimitivePropertyConfiguration, PrimitivePropertyConfiguration>>) foreignKeys)
        edmProperty.HasConstraint(keyValuePair.Key.PropertyInfo, keyValuePair.Value.PropertyInfo);
    }

    private static IDictionary<PrimitivePropertyConfiguration, PrimitivePropertyConfiguration> GetForeignKeys(
      EntityTypeConfiguration principalEntityType,
      EntityTypeConfiguration dependentEntityType)
    {
      IDictionary<PrimitivePropertyConfiguration, PrimitivePropertyConfiguration> foreignKeys = (IDictionary<PrimitivePropertyConfiguration, PrimitivePropertyConfiguration>) new Dictionary<PrimitivePropertyConfiguration, PrimitivePropertyConfiguration>();
      foreach (PrimitivePropertyConfiguration key1 in principalEntityType.Keys)
      {
        foreach (PrimitivePropertyConfiguration key2 in dependentEntityType.Properties.OfType<PrimitivePropertyConfiguration>())
        {
          Type type = Nullable.GetUnderlyingType(key2.PropertyInfo.PropertyType);
          if ((object) type == null)
            type = key2.PropertyInfo.PropertyType;
          if (type == key1.PropertyInfo.PropertyType)
          {
            if (string.Equals(key2.Name, principalEntityType.Name + key1.Name, StringComparison.Ordinal))
              foreignKeys.Add(key2, key1);
            else if (string.Equals(key2.Name, key1.Name, StringComparison.Ordinal) && string.Equals(key1.Name, principalEntityType.Name + "Id", StringComparison.OrdinalIgnoreCase))
              foreignKeys.Add(key2, key1);
          }
        }
      }
      return foreignKeys;
    }
  }
}
