// Decompiled with JetBrains decompiler
// Type: Nest.PropertiesExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  internal static class PropertiesExtensions
  {
    internal static IProperties AutoMap(
      this IProperties existingProperties,
      Type documentType,
      IPropertyVisitor visitor = null,
      int maxRecursion = 0)
    {
      Properties properties = new Properties();
      foreach (KeyValuePair<PropertyName, IProperty> property in (IEnumerable<KeyValuePair<PropertyName, IProperty>>) new PropertyWalker(documentType, visitor, maxRecursion).GetProperties())
        properties[property.Key] = property.Value;
      if (existingProperties == null)
        return (IProperties) properties;
      foreach (KeyValuePair<PropertyName, IProperty> existingProperty in (IEnumerable<KeyValuePair<PropertyName, IProperty>>) existingProperties)
        properties[existingProperty.Key] = existingProperty.Value;
      return (IProperties) properties;
    }

    internal static IProperties AutoMap<T>(
      this IProperties existingProperties,
      IPropertyVisitor visitor = null,
      int maxRecursion = 0)
      where T : class
    {
      return existingProperties.AutoMap(typeof (T), visitor, maxRecursion);
    }
  }
}
