// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.ConventionsHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder.Conventions
{
  internal static class ConventionsHelpers
  {
    public static IEnumerable<KeyValuePair<string, object>> GetEntityKey(
      ResourceContext resourceContext)
    {
      return !(resourceContext.StructuredType is IEdmEntityType structuredType) ? Enumerable.Empty<KeyValuePair<string, object>>() : structuredType.Key().Select<IEdmStructuralProperty, KeyValuePair<string, object>>((Func<IEdmStructuralProperty, KeyValuePair<string, object>>) (k => new KeyValuePair<string, object>(k.Name, ConventionsHelpers.GetKeyValue((IEdmProperty) k, resourceContext))));
    }

    private static object GetKeyValue(IEdmProperty key, ResourceContext resourceContext)
    {
      object propertyValue = resourceContext.GetPropertyValue(key.Name);
      if (propertyValue == null)
      {
        IEdmTypeReference edmType = resourceContext.EdmObject.GetEdmType();
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.KeyValueCannotBeNull, (object) key.Name, (object) edmType.Definition);
      }
      return ConventionsHelpers.ConvertValue(propertyValue);
    }

    public static object ConvertValue(object value)
    {
      Type type = value.GetType();
      value = !TypeHelper.IsEnum(type) ? ODataPrimitiveSerializer.ConvertUnsupportedPrimitives(value) : (object) new ODataEnumValue(value.ToString(), type.EdmFullName());
      return value;
    }

    public static string GetEntityKeyValue(ResourceContext resourceContext)
    {
      if (!(resourceContext.StructuredType is IEdmEntityType structuredType))
        return string.Empty;
      IEnumerable<IEdmProperty> source = (IEnumerable<IEdmProperty>) structuredType.Key();
      return source.Count<IEdmProperty>() == 1 ? ConventionsHelpers.GetUriRepresentationForKeyValue(source.First<IEdmProperty>(), resourceContext) : string.Join(",", source.Select<IEdmProperty, string>((Func<IEdmProperty, string>) (key => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
      {
        (object) key.Name,
        (object) ConventionsHelpers.GetUriRepresentationForKeyValue(key, resourceContext)
      }))));
    }

    public static IEnumerable<PropertyInfo> GetProperties(
      StructuralTypeConfiguration structural,
      bool includeReadOnly)
    {
      IEnumerable<PropertyInfo> allProperties1 = ConventionsHelpers.GetAllProperties(structural, includeReadOnly);
      if (structural.BaseTypeInternal == null)
        return allProperties1;
      IEnumerable<PropertyInfo> allProperties2 = ConventionsHelpers.GetAllProperties(structural.BaseTypeInternal, includeReadOnly);
      return allProperties1.Except<PropertyInfo>(allProperties2, (IEqualityComparer<PropertyInfo>) ConventionsHelpers.PropertyEqualityComparer.Instance);
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(
      StructuralTypeConfiguration type,
      bool includeReadOnly)
    {
      if (type == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      return ((IEnumerable<PropertyInfo>) type.ClrType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (!p.IsValidStructuralProperty() || type.IgnoredProperties().Any<PropertyInfo>((Func<PropertyInfo, bool>) (p1 => p1.Name == p.Name)))
          return false;
        return includeReadOnly || p.GetSetMethod() != (MethodInfo) null || TypeHelper.IsCollection(p.PropertyType);
      }));
    }

    public static bool IsValidStructuralProperty(this PropertyInfo propertyInfo)
    {
      if (propertyInfo == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertyInfo));
      return !((IEnumerable<ParameterInfo>) propertyInfo.GetIndexParameters()).Any<ParameterInfo>() && propertyInfo.CanRead && propertyInfo.GetGetMethod() != (MethodInfo) null && propertyInfo.PropertyType.IsValidStructuralPropertyType();
    }

    public static IEnumerable<PropertyInfo> IgnoredProperties(
      this StructuralTypeConfiguration structuralType)
    {
      if (structuralType == null)
        return Enumerable.Empty<PropertyInfo>();
      return structuralType is EntityTypeConfiguration typeConfiguration ? typeConfiguration.IgnoredProperties.Concat<PropertyInfo>(typeConfiguration.BaseType.IgnoredProperties()) : (IEnumerable<PropertyInfo>) structuralType.IgnoredProperties;
    }

    public static bool IsValidStructuralPropertyType(this Type type)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (type.IsGenericTypeDefinition() || type.IsPointer || type == typeof (object))
        return false;
      Type elementType;
      return !TypeHelper.IsCollection(type, out elementType) || !(elementType == typeof (object));
    }

    public static string GetUriRepresentationForValue(object value)
    {
      Type type = value.GetType();
      value = !TypeHelper.IsEnum(type) ? ODataPrimitiveSerializer.ConvertUnsupportedPrimitives(value) : (object) new ODataEnumValue(value.ToString(), type.EdmFullName());
      return ODataUriUtils.ConvertToUriLiteral(value, ODataVersion.V4);
    }

    private static string GetUriRepresentationForKeyValue(
      IEdmProperty key,
      ResourceContext resourceContext)
    {
      object propertyValue = resourceContext.GetPropertyValue(key.Name);
      if (propertyValue == null)
      {
        IEdmTypeReference edmType = resourceContext.EdmObject.GetEdmType();
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.KeyValueCannotBeNull, (object) key.Name, (object) edmType.Definition);
      }
      return ConventionsHelpers.GetUriRepresentationForValue(propertyValue);
    }

    private class PropertyEqualityComparer : IEqualityComparer<PropertyInfo>
    {
      public static ConventionsHelpers.PropertyEqualityComparer Instance = new ConventionsHelpers.PropertyEqualityComparer();

      public bool Equals(PropertyInfo x, PropertyInfo y) => x.Name == y.Name;

      public int GetHashCode(PropertyInfo obj) => obj.Name.GetHashCode();
    }
  }
}
