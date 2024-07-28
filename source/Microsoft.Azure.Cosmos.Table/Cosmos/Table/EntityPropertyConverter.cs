// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.EntityPropertyConverter
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Table
{
  public static class EntityPropertyConverter
  {
    public const string DefaultPropertyNameDelimiter = "_";

    public static Dictionary<string, EntityProperty> Flatten(
      object root,
      OperationContext operationContext)
    {
      object root1 = root;
      EntityPropertyConverterOptions entityPropertyConverterOptions = new EntityPropertyConverterOptions();
      entityPropertyConverterOptions.PropertyNameDelimiter = "_";
      OperationContext operationContext1 = operationContext;
      return EntityPropertyConverter.Flatten(root1, entityPropertyConverterOptions, operationContext1);
    }

    public static Dictionary<string, EntityProperty> Flatten(
      object root,
      EntityPropertyConverterOptions entityPropertyConverterOptions,
      OperationContext operationContext)
    {
      if (root == null)
        return (Dictionary<string, EntityProperty>) null;
      Dictionary<string, EntityProperty> propertyDictionary = new Dictionary<string, EntityProperty>();
      HashSet<object> antecedents = new HashSet<object>((IEqualityComparer<object>) new EntityPropertyConverter.ObjectReferenceEqualityComparer());
      return !EntityPropertyConverter.Flatten(propertyDictionary, root, string.Empty, antecedents, entityPropertyConverterOptions, operationContext) ? (Dictionary<string, EntityProperty>) null : propertyDictionary;
    }

    public static T ConvertBack<T>(
      IDictionary<string, EntityProperty> flattenedEntityProperties,
      OperationContext operationContext)
    {
      IDictionary<string, EntityProperty> flattenedEntityProperties1 = flattenedEntityProperties;
      EntityPropertyConverterOptions entityPropertyConverterOptions = new EntityPropertyConverterOptions();
      entityPropertyConverterOptions.PropertyNameDelimiter = "_";
      OperationContext operationContext1 = operationContext;
      return EntityPropertyConverter.ConvertBack<T>(flattenedEntityProperties1, entityPropertyConverterOptions, operationContext1);
    }

    public static T ConvertBack<T>(
      IDictionary<string, EntityProperty> flattenedEntityProperties,
      EntityPropertyConverterOptions entityPropertyConverterOptions,
      OperationContext operationContext)
    {
      if (flattenedEntityProperties == null)
        return default (T);
      T instance = (T) Activator.CreateInstance(typeof (T));
      return flattenedEntityProperties.Aggregate<KeyValuePair<string, EntityProperty>, T>(instance, (Func<T, KeyValuePair<string, EntityProperty>, T>) ((current, kvp) => (T) EntityPropertyConverter.SetProperty((object) current, kvp.Key, kvp.Value.PropertyAsObject, entityPropertyConverterOptions, operationContext)));
    }

    private static bool Flatten(
      Dictionary<string, EntityProperty> propertyDictionary,
      object current,
      string objectPath,
      HashSet<object> antecedents,
      EntityPropertyConverterOptions entityPropertyConverterOptions,
      OperationContext operationContext)
    {
      if (current == null)
        return true;
      Type type = current.GetType();
      EntityProperty propertyWithType = EntityPropertyConverter.CreateEntityPropertyWithType(current, type);
      if (propertyWithType != null)
      {
        propertyDictionary.Add(objectPath, propertyWithType);
        return true;
      }
      PropertyInfo[] properties = type.GetProperties();
      if (!((IEnumerable<PropertyInfo>) properties).Any<PropertyInfo>())
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unsupported type : {0} encountered during conversion to EntityProperty. Object Path: {1}", (object) type, (object) objectPath));
      bool flag = false;
      if (!type.IsValueType)
      {
        if (antecedents.Contains(current))
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Recursive reference detected. Object Path: {0} Property Type: {1}.", (object) objectPath, (object) type));
        antecedents.Add(current);
        flag = true;
      }
      string propertyNameDelimiter = entityPropertyConverterOptions != null ? entityPropertyConverterOptions.PropertyNameDelimiter : "_";
      int num = ((IEnumerable<PropertyInfo>) properties).Where<PropertyInfo>((Func<PropertyInfo, bool>) (propertyInfo => !EntityPropertyConverter.ShouldSkip(propertyInfo, objectPath, operationContext))).All<PropertyInfo>((Func<PropertyInfo, bool>) (propertyInfo =>
      {
        if (propertyInfo.Name.Contains(propertyNameDelimiter))
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Property delimiter: {0} exists in property name: {1}. Object Path: {2}", (object) propertyNameDelimiter, (object) propertyInfo.Name, (object) objectPath));
        return EntityPropertyConverter.Flatten(propertyDictionary, propertyInfo.GetValue(current, (object[]) null), string.IsNullOrWhiteSpace(objectPath) ? propertyInfo.Name : objectPath + propertyNameDelimiter + propertyInfo.Name, antecedents, entityPropertyConverterOptions, operationContext);
      })) ? 1 : 0;
      if (!flag)
        return num != 0;
      antecedents.Remove(current);
      return num != 0;
    }

    private static EntityProperty CreateEntityPropertyWithType(object value, Type type)
    {
      if (type == typeof (string))
        return new EntityProperty((string) value);
      if (type == typeof (byte[]))
        return new EntityProperty((byte[]) value);
      if (type == typeof (bool))
        return new EntityProperty(new bool?((bool) value));
      if (type == typeof (bool?))
        return new EntityProperty((bool?) value);
      if (type == typeof (DateTime))
        return new EntityProperty(new DateTime?((DateTime) value));
      if (type == typeof (DateTime?))
        return new EntityProperty((DateTime?) value);
      if (type == typeof (DateTimeOffset))
        return new EntityProperty(new DateTimeOffset?((DateTimeOffset) value));
      if (type == typeof (DateTimeOffset?))
        return new EntityProperty((DateTimeOffset?) value);
      if (type == typeof (double))
        return new EntityProperty(new double?((double) value));
      if (type == typeof (double?))
        return new EntityProperty((double?) value);
      if (type == typeof (Guid?))
        return new EntityProperty((Guid?) value);
      if (type == typeof (Guid))
        return new EntityProperty(new Guid?((Guid) value));
      if (type == typeof (int))
        return new EntityProperty(new int?((int) value));
      if (type == typeof (int?))
        return new EntityProperty((int?) value);
      if (type == typeof (uint))
        return new EntityProperty(new int?((int) Convert.ToUInt32(value, (IFormatProvider) CultureInfo.InvariantCulture)));
      if (type == typeof (uint?))
        return new EntityProperty(new int?((int) Convert.ToUInt32(value, (IFormatProvider) CultureInfo.InvariantCulture)));
      if (type == typeof (long))
        return new EntityProperty(new long?((long) value));
      if (type == typeof (long?))
        return new EntityProperty((long?) value);
      if (type == typeof (ulong))
        return new EntityProperty(new long?((long) Convert.ToUInt64(value, (IFormatProvider) CultureInfo.InvariantCulture)));
      if (type == typeof (ulong?))
        return new EntityProperty(new long?((long) Convert.ToUInt64(value, (IFormatProvider) CultureInfo.InvariantCulture)));
      if (type.IsEnum)
        return new EntityProperty(value.ToString());
      if (type == typeof (TimeSpan))
        return new EntityProperty(value.ToString());
      return type == typeof (TimeSpan?) ? new EntityProperty(value?.ToString()) : (EntityProperty) null;
    }

    private static object SetProperty(
      object root,
      string propertyPath,
      object propertyValue,
      EntityPropertyConverterOptions entityPropertyConverterOptions,
      OperationContext operationContext)
    {
      if (root == null)
        throw new ArgumentNullException(nameof (root));
      if (propertyPath == null)
        throw new ArgumentNullException(nameof (propertyPath));
      try
      {
        string str = entityPropertyConverterOptions != null ? entityPropertyConverterOptions.PropertyNameDelimiter : "_";
        Stack<Tuple<object, object, PropertyInfo>> tupleStack = new Stack<Tuple<object, object, PropertyInfo>>();
        string[] source = propertyPath.Split(new string[1]
        {
          str
        }, StringSplitOptions.RemoveEmptyEntries);
        object obj = root;
        bool flag = false;
        for (int index = 0; index < source.Length - 1; ++index)
        {
          PropertyInfo property = obj.GetType().GetProperty(source[index]);
          object instance = property.GetValue(obj, (object[]) null);
          Type propertyType = property.PropertyType;
          if (instance == null)
          {
            instance = Activator.CreateInstance(propertyType);
            property.SetValue(obj, EntityPropertyConverter.ChangeType(instance, property.PropertyType), (object[]) null);
          }
          if (flag || propertyType.IsValueType)
          {
            flag = true;
            tupleStack.Push(new Tuple<object, object, PropertyInfo>(instance, obj, property));
          }
          obj = instance;
        }
        PropertyInfo property1 = obj.GetType().GetProperty(((IEnumerable<string>) source).Last<string>());
        if (property1 == (PropertyInfo) null)
          return root;
        property1.SetValue(obj, EntityPropertyConverter.ChangeType(propertyValue, property1.PropertyType), (object[]) null);
        object propertyValue1 = obj;
        while (tupleStack.Count != 0)
        {
          Tuple<object, object, PropertyInfo> tuple = tupleStack.Pop();
          tuple.Item3.SetValue(tuple.Item2, EntityPropertyConverter.ChangeType(propertyValue1, tuple.Item3.PropertyType), (object[]) null);
          propertyValue1 = tuple.Item2;
        }
        return root;
      }
      catch (Exception ex)
      {
        Logger.LogError(operationContext, "Exception thrown while trying to set property value. Property Path: {0} Property Value: {1}. Exception Message: {2}", (object) propertyPath, propertyValue, (object) ex.Message);
        throw;
      }
    }

    private static object ChangeType(object propertyValue, Type propertyType)
    {
      Type type1 = Nullable.GetUnderlyingType(propertyType);
      if ((object) type1 == null)
        type1 = propertyType;
      Type type2 = type1;
      if (type2.IsEnum)
        return Enum.Parse(type2, propertyValue.ToString());
      if (type2 == typeof (DateTimeOffset))
        return (object) new DateTimeOffset((DateTime) propertyValue);
      if (type2 == typeof (TimeSpan))
        return (object) TimeSpan.Parse(propertyValue.ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
      if (type2 == typeof (uint))
        return (object) (uint) (int) propertyValue;
      return type2 == typeof (ulong) ? (object) (ulong) (long) propertyValue : Convert.ChangeType(propertyValue, type2, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    private static bool ShouldSkip(
      PropertyInfo propertyInfo,
      string objectPath,
      OperationContext operationContext)
    {
      if (!propertyInfo.CanWrite)
      {
        Logger.LogInformational(operationContext, "Omitting property: {0} from serialization/de-serialization because the property does not have a setter. The property needs to have at least a private setter. Object Path: {1}", (object) propertyInfo.Name, (object) objectPath);
        return true;
      }
      if (propertyInfo.CanRead)
        return Attribute.IsDefined((MemberInfo) propertyInfo, typeof (IgnorePropertyAttribute));
      Logger.LogInformational(operationContext, "Omitting property: {0} from serialization/de-serialization because the property does not have a getter. Object path: {1}", (object) propertyInfo.Name, (object) objectPath);
      return true;
    }

    private class ObjectReferenceEqualityComparer : IEqualityComparer<object>
    {
      public bool Equals(object x, object y) => x == y;

      public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
  }
}
