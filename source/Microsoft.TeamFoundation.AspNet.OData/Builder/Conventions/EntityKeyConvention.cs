// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.EntityKeyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder.Conventions
{
  internal class EntityKeyConvention : EntityTypeConvention
  {
    public override void Apply(EntityTypeConfiguration entity, ODataConventionModelBuilder model)
    {
      if (entity == null)
        throw Error.ArgumentNull(nameof (entity));
      if (entity.Keys.Any<PrimitivePropertyConfiguration>() || entity.EnumKeys.Any<EnumPropertyConfiguration>() || entity.BaseType != null && entity.BaseType.Keys().Any<PropertyConfiguration>())
        return;
      PropertyConfiguration keyProperty = EntityKeyConvention.GetKeyProperty(entity);
      if (keyProperty == null)
        return;
      entity.HasKey(keyProperty.PropertyInfo);
    }

    private static PropertyConfiguration GetKeyProperty(EntityTypeConfiguration entityType)
    {
      IEnumerable<PropertyConfiguration> source = entityType.Properties.Where<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (p =>
      {
        if (!p.Name.Equals(entityType.Name + "Id", StringComparison.OrdinalIgnoreCase) && !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
          return false;
        return EdmLibHelpers.GetEdmPrimitiveTypeOrNull(p.PropertyInfo.PropertyType) != null || TypeHelper.IsEnum(p.PropertyInfo.PropertyType);
      }));
      return source.Count<PropertyConfiguration>() == 1 ? source.Single<PropertyConfiguration>() : (PropertyConfiguration) null;
    }
  }
}
