// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.DeserializationHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  internal static class DeserializationHelpers
  {
    internal static void ApplyProperty(
      ODataProperty property,
      IEdmStructuredTypeReference resourceType,
      object resource,
      ODataDeserializerProvider deserializerProvider,
      ODataDeserializerContext readContext)
    {
      IEdmProperty property1 = resourceType.FindProperty(property.Name);
      bool flag = false;
      string propertyName = property.Name;
      if (property1 != null)
      {
        propertyName = EdmLibHelpers.GetClrPropertyName(property1, readContext.Model);
      }
      else
      {
        IEdmStructuredType edmStructuredType = resourceType.StructuredDefinition();
        flag = edmStructuredType != null && edmStructuredType.IsOpen;
      }
      if (!flag && property1 == null)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotDeserializeUnknownProperty, (object) property.Name, (object) resourceType.Definition));
      IEdmTypeReference type = property1?.Type;
      EdmTypeKind typeKind;
      object propertyValue = DeserializationHelpers.ConvertValue(property.Value, ref type, deserializerProvider, readContext, out typeKind);
      if (flag)
        DeserializationHelpers.SetDynamicProperty(resource, resourceType, typeKind, propertyName, propertyValue, type, readContext.Model);
      else
        DeserializationHelpers.SetDeclaredProperty(resource, typeKind, propertyName, propertyValue, property1, readContext);
    }

    internal static void SetDynamicProperty(
      object resource,
      IEdmStructuredTypeReference resourceType,
      EdmTypeKind propertyKind,
      string propertyName,
      object propertyValue,
      IEdmTypeReference propertyType,
      IEdmModel model)
    {
      if (propertyKind == EdmTypeKind.Collection && propertyValue.GetType() != typeof (EdmComplexObjectCollection) && propertyValue.GetType() != typeof (EdmEnumObjectCollection))
        DeserializationHelpers.SetDynamicCollectionProperty(resource, propertyName, propertyValue, propertyType.AsCollection(), resourceType.StructuredDefinition(), model);
      else
        DeserializationHelpers.SetDynamicProperty(resource, propertyName, propertyValue, resourceType.StructuredDefinition(), model);
    }

    internal static void SetDeclaredProperty(
      object resource,
      EdmTypeKind propertyKind,
      string propertyName,
      object propertyValue,
      IEdmProperty edmProperty,
      ODataDeserializerContext readContext)
    {
      if (propertyKind == EdmTypeKind.Collection)
      {
        DeserializationHelpers.SetCollectionProperty(resource, edmProperty, propertyValue, propertyName);
      }
      else
      {
        if (!readContext.IsUntyped && propertyKind == EdmTypeKind.Primitive)
          propertyValue = EdmPrimitiveHelpers.ConvertPrimitiveValue(propertyValue, DeserializationHelpers.GetPropertyType(resource, propertyName));
        DeserializationHelpers.SetProperty(resource, propertyName, propertyValue);
      }
    }

    internal static void SetCollectionProperty(
      object resource,
      IEdmProperty edmProperty,
      object value,
      string propertyName)
    {
      DeserializationHelpers.SetCollectionProperty(resource, propertyName, edmProperty.Type.AsCollection(), value, false);
    }

    internal static void SetCollectionProperty(
      object resource,
      string propertyName,
      IEdmCollectionTypeReference edmPropertyType,
      object value,
      bool clearCollection)
    {
      if (value == null)
        return;
      IEnumerable items = value as IEnumerable;
      Type type = resource.GetType();
      Type propertyType = DeserializationHelpers.GetPropertyType(resource, propertyName);
      Type elementType;
      if (!TypeHelper.IsCollection(propertyType, out elementType))
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyIsNotCollection, (object) propertyType.FullName, (object) propertyName, (object) type.FullName));
      IEnumerable instance;
      if (DeserializationHelpers.CanSetProperty(resource, propertyName) && CollectionDeserializationHelpers.TryCreateInstance(propertyType, edmPropertyType, elementType, out instance))
      {
        items.AddToCollection(instance, elementType, type, propertyName, propertyType);
        if (propertyType.IsArray)
          instance = CollectionDeserializationHelpers.ToArray(instance, elementType);
        DeserializationHelpers.SetProperty(resource, propertyName, (object) instance);
      }
      else
      {
        if (!(DeserializationHelpers.GetProperty(resource, propertyName) is IEnumerable property))
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotAddToNullCollection, (object) propertyName, (object) type.FullName));
        if (clearCollection)
          property.Clear(propertyName, type);
        items.AddToCollection(property, elementType, type, propertyName, propertyType);
      }
    }

    internal static void SetDynamicCollectionProperty(
      object resource,
      string propertyName,
      object value,
      IEdmCollectionTypeReference edmPropertyType,
      IEdmStructuredType structuredType,
      IEdmModel model)
    {
      IEnumerable items = value as IEnumerable;
      Type type1 = resource.GetType();
      Type clrType = EdmLibHelpers.GetClrType(edmPropertyType.ElementType(), model);
      Type type2 = typeof (ICollection<>).MakeGenericType(clrType);
      IEnumerable instance;
      if (!CollectionDeserializationHelpers.TryCreateInstance(type2, edmPropertyType, clrType, out instance))
        return;
      items.AddToCollection(instance, clrType, type1, propertyName, type2);
      DeserializationHelpers.SetDynamicProperty(resource, propertyName, (object) instance, structuredType, model);
    }

    internal static void SetProperty(object resource, string propertyName, object value)
    {
      if (!(resource is IDelta delta))
        resource.GetType().GetProperty(propertyName).SetValue(resource, value, (object[]) null);
      else
        delta.TrySetPropertyValue(propertyName, value);
    }

    internal static void SetDynamicProperty(
      object resource,
      string propertyName,
      object value,
      IEdmStructuredType structuredType,
      IEdmModel model)
    {
      if (resource is IDelta delta)
      {
        delta.TrySetPropertyValue(propertyName, value);
      }
      else
      {
        PropertyInfo propertyDictionary = EdmLibHelpers.GetDynamicPropertyDictionary(structuredType, model);
        if (propertyDictionary == (PropertyInfo) null)
          return;
        object obj = propertyDictionary.GetValue(resource);
        IDictionary<string, object> dictionary;
        if (obj == null)
        {
          if (!propertyDictionary.CanWrite)
            throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CannotSetDynamicPropertyDictionary, (object) propertyName, (object) resource.GetType().FullName);
          dictionary = (IDictionary<string, object>) new Dictionary<string, object>();
          propertyDictionary.SetValue(resource, (object) dictionary);
        }
        else
          dictionary = (IDictionary<string, object>) obj;
        if (dictionary.ContainsKey(propertyName))
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.DuplicateDynamicPropertyNameFound, (object) propertyName, (object) structuredType.FullTypeName());
        dictionary.Add(propertyName, value);
      }
    }

    internal static object ConvertValue(
      object oDataValue,
      ref IEdmTypeReference propertyType,
      ODataDeserializerProvider deserializerProvider,
      ODataDeserializerContext readContext,
      out EdmTypeKind typeKind)
    {
      switch (oDataValue)
      {
        case null:
          typeKind = EdmTypeKind.None;
          return (object) null;
        case ODataEnumValue enumValue:
          typeKind = EdmTypeKind.Enum;
          return DeserializationHelpers.ConvertEnumValue(enumValue, ref propertyType, deserializerProvider, readContext);
        case ODataCollectionValue collection:
          typeKind = EdmTypeKind.Collection;
          return DeserializationHelpers.ConvertCollectionValue(collection, ref propertyType, deserializerProvider, readContext);
        case ODataUntypedValue odataUntypedValue:
          oDataValue = !odataUntypedValue.RawValue.StartsWith("[", StringComparison.Ordinal) && !odataUntypedValue.RawValue.StartsWith("{", StringComparison.Ordinal) ? DeserializationHelpers.ConvertPrimitiveValue(odataUntypedValue.RawValue) : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidODataUntypedValue, (object) odataUntypedValue.RawValue));
          break;
      }
      typeKind = EdmTypeKind.Primitive;
      return oDataValue;
    }

    internal static Type GetPropertyType(object resource, string propertyName)
    {
      if (resource is IDelta delta)
      {
        Type type;
        delta.TryGetPropertyType(propertyName, out type);
        return type;
      }
      PropertyInfo property = resource.GetType().GetProperty(propertyName);
      return !(property == (PropertyInfo) null) ? property.PropertyType : (Type) null;
    }

    private static bool CanSetProperty(object resource, string propertyName)
    {
      if (resource is IDelta)
        return true;
      PropertyInfo property = resource.GetType().GetProperty(propertyName);
      return property != (PropertyInfo) null && property.GetSetMethod() != (MethodInfo) null;
    }

    private static object GetProperty(object resource, string propertyName)
    {
      if (!(resource is IDelta delta))
        return resource.GetType().GetProperty(propertyName).GetValue(resource, (object[]) null);
      object property;
      delta.TryGetPropertyValue(propertyName, out property);
      return property;
    }

    private static object ConvertCollectionValue(
      ODataCollectionValue collection,
      ref IEdmTypeReference propertyType,
      ODataDeserializerProvider deserializerProvider,
      ODataDeserializerContext readContext)
    {
      IEdmCollectionTypeReference edmType;
      if (propertyType == null)
      {
        string collectionElementTypeName = DeserializationHelpers.GetCollectionElementTypeName(collection.TypeName, false);
        edmType = (IEdmCollectionTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType(readContext.Model.FindType(collectionElementTypeName).ToEdmTypeReference(false)));
        propertyType = (IEdmTypeReference) edmType;
      }
      else
        edmType = propertyType as IEdmCollectionTypeReference;
      return deserializerProvider.GetEdmTypeDeserializer((IEdmTypeReference) edmType).ReadInline((object) collection, (IEdmTypeReference) edmType, readContext);
    }

    private static object ConvertPrimitiveValue(string value)
    {
      if (string.CompareOrdinal(value, "null") == 0)
        return (object) null;
      int result1;
      if (int.TryParse(value, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1))
        return (object) result1;
      Decimal result2;
      if (Decimal.TryParse(value, NumberStyles.Number, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result2))
        return (object) result2;
      double result3;
      if (double.TryParse(value, NumberStyles.Float, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result3))
        return (object) result3;
      return value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal) ? (object) value.Substring(1, value.Length - 2) : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidODataUntypedValue, (object) value));
    }

    private static object ConvertEnumValue(
      ODataEnumValue enumValue,
      ref IEdmTypeReference propertyType,
      ODataDeserializerProvider deserializerProvider,
      ODataDeserializerContext readContext)
    {
      IEdmEnumTypeReference edmType;
      if (propertyType == null)
      {
        edmType = (IEdmEnumTypeReference) new EdmEnumTypeReference(readContext.Model.FindType(enumValue.TypeName) as IEdmEnumType, true);
        propertyType = (IEdmTypeReference) edmType;
      }
      else
        edmType = propertyType.AsEnum();
      return deserializerProvider.GetEdmTypeDeserializer((IEdmTypeReference) edmType).ReadInline((object) enumValue, propertyType, readContext);
    }

    internal static string GetCollectionElementTypeName(string typeName, bool isNested)
    {
      int length = "Collection".Length;
      if (typeName == null || !typeName.StartsWith("Collection(", StringComparison.Ordinal) || typeName[typeName.Length - 1] != ')' || typeName.Length == length + 2)
        return (string) null;
      if (isNested)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NestedCollectionsNotSupported, (object) typeName));
      string typeName1 = typeName.Substring(length + 1, typeName.Length - (length + 2));
      DeserializationHelpers.GetCollectionElementTypeName(typeName1, true);
      return typeName1;
    }
  }
}
