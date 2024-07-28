// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmToClrConverter
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmToClrConverter
  {
    private static readonly Type TypeICollectionOfT = typeof (ICollection<>);
    private static readonly Type TypeIListOfT = typeof (IList<>);
    private static readonly Type TypeListOfT = typeof (List<>);
    private static readonly Type TypeIEnumerableOfT = typeof (IEnumerable<>);
    private static readonly Type TypeNullableOfT = typeof (Nullable<>);
    private static readonly MethodInfo CastToClrTypeMethodInfo = typeof (EdmToClrConverter.CastHelper).GetMethod("CastToClrType");
    private static readonly MethodInfo EnumerableToListOfTMethodInfo = typeof (EdmToClrConverter.CastHelper).GetMethod("EnumerableToListOfT");
    private readonly Dictionary<IEdmStructuredValue, object> convertedObjects = new Dictionary<IEdmStructuredValue, object>();
    private readonly Dictionary<Type, MethodInfo> enumerableConverters = new Dictionary<Type, MethodInfo>();
    private readonly Dictionary<Type, MethodInfo> enumTypeConverters = new Dictionary<Type, MethodInfo>();
    private readonly TryCreateObjectInstance tryCreateObjectInstanceDelegate;
    private readonly TryGetClrPropertyInfo tryGetClrPropertyInfoDelegate;

    public EdmToClrConverter()
    {
    }

    public EdmToClrConverter(
      TryCreateObjectInstance tryCreateObjectInstanceDelegate)
    {
      EdmUtil.CheckArgumentNull<TryCreateObjectInstance>(tryCreateObjectInstanceDelegate, nameof (tryCreateObjectInstanceDelegate));
      this.tryCreateObjectInstanceDelegate = tryCreateObjectInstanceDelegate;
    }

    public EdmToClrConverter(
      TryCreateObjectInstance tryCreateObjectInstanceDelegate,
      TryGetClrPropertyInfo tryGetClrPropertyInfoDelegate,
      TryGetClrTypeName tryGetClrTypeNameDelegate)
    {
      this.tryCreateObjectInstanceDelegate = tryCreateObjectInstanceDelegate;
      this.tryGetClrPropertyInfoDelegate = tryGetClrPropertyInfoDelegate;
      this.TryGetClrTypeNameDelegate = tryGetClrTypeNameDelegate;
    }

    internal TryGetClrTypeName TryGetClrTypeNameDelegate { get; private set; }

    public T AsClrValue<T>(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      bool convertEnumValues = false;
      return (T) this.AsClrValue(edmValue, typeof (T), convertEnumValues);
    }

    public object AsClrValue(IEdmValue edmValue, Type clrType)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      EdmUtil.CheckArgumentNull<Type>(clrType, nameof (clrType));
      bool convertEnumValues = true;
      return this.AsClrValue(edmValue, clrType, convertEnumValues);
    }

    public void RegisterConvertedObject(IEdmStructuredValue edmValue, object clrObject) => this.convertedObjects.Add(edmValue, clrObject);

    internal static byte[] AsClrByteArray(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return edmValue is IEdmNullValue ? (byte[]) null : ((IEdmBinaryValue) edmValue).Value;
    }

    internal static string AsClrString(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return edmValue is IEdmNullValue ? (string) null : ((IEdmStringValue) edmValue).Value;
    }

    internal static bool AsClrBoolean(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return ((IEdmBooleanValue) edmValue).Value;
    }

    internal static long AsClrInt64(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return ((IEdmIntegerValue) edmValue).Value;
    }

    internal static char AsClrChar(IEdmValue edmValue) => checked ((char) EdmToClrConverter.AsClrInt64(edmValue));

    internal static byte AsClrByte(IEdmValue edmValue) => checked ((byte) EdmToClrConverter.AsClrInt64(edmValue));

    internal static short AsClrInt16(IEdmValue edmValue) => checked ((short) EdmToClrConverter.AsClrInt64(edmValue));

    internal static int AsClrInt32(IEdmValue edmValue) => checked ((int) EdmToClrConverter.AsClrInt64(edmValue));

    internal static double AsClrDouble(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return ((IEdmFloatingValue) edmValue).Value;
    }

    internal static float AsClrSingle(IEdmValue edmValue) => (float) EdmToClrConverter.AsClrDouble(edmValue);

    internal static TimeOfDay AsClrTimeOfDay(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return ((IEdmTimeOfDayValue) edmValue).Value;
    }

    internal static Date AsClrDate(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return ((IEdmDateValue) edmValue).Value;
    }

    internal static Decimal AsClrDecimal(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return ((IEdmDecimalValue) edmValue).Value;
    }

    internal static TimeSpan AsClrDuration(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return ((IEdmDurationValue) edmValue).Value;
    }

    internal static Guid AsClrGuid(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return ((IEdmGuidValue) edmValue).Value;
    }

    internal static DateTimeOffset AsClrDateTimeOffset(IEdmValue edmValue)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      return ((IEdmDateTimeOffsetValue) edmValue).Value;
    }

    private static bool TryConvertAsNonGuidPrimitiveType(
      Type clrType,
      IEdmValue edmValue,
      out object clrValue)
    {
      if (clrType == typeof (bool))
      {
        clrValue = (object) EdmToClrConverter.AsClrBoolean(edmValue);
        return true;
      }
      if (clrType == typeof (char))
      {
        clrValue = (object) EdmToClrConverter.AsClrChar(edmValue);
        return true;
      }
      if (clrType == typeof (sbyte))
      {
        clrValue = (object) checked ((sbyte) EdmToClrConverter.AsClrInt64(edmValue));
        return true;
      }
      if (clrType == typeof (byte))
      {
        clrValue = (object) EdmToClrConverter.AsClrByte(edmValue);
        return true;
      }
      if (clrType == typeof (short))
      {
        clrValue = (object) EdmToClrConverter.AsClrInt16(edmValue);
        return true;
      }
      if (clrType == typeof (ushort))
      {
        clrValue = (object) checked ((ushort) EdmToClrConverter.AsClrInt64(edmValue));
        return true;
      }
      if (clrType == typeof (int))
      {
        clrValue = (object) EdmToClrConverter.AsClrInt32(edmValue);
        return true;
      }
      if (clrType == typeof (uint))
      {
        clrValue = (object) checked ((uint) EdmToClrConverter.AsClrInt64(edmValue));
        return true;
      }
      if (clrType == typeof (long))
      {
        clrValue = (object) EdmToClrConverter.AsClrInt64(edmValue);
        return true;
      }
      if (clrType == typeof (ulong))
      {
        clrValue = (object) checked ((ulong) EdmToClrConverter.AsClrInt64(edmValue));
        return true;
      }
      if (clrType == typeof (float))
      {
        clrValue = (object) EdmToClrConverter.AsClrSingle(edmValue);
        return true;
      }
      if (clrType == typeof (double))
      {
        clrValue = (object) EdmToClrConverter.AsClrDouble(edmValue);
        return true;
      }
      if (clrType == typeof (Decimal))
      {
        clrValue = (object) EdmToClrConverter.AsClrDecimal(edmValue);
        return true;
      }
      if (clrType == typeof (string))
      {
        clrValue = (object) EdmToClrConverter.AsClrString(edmValue);
        return true;
      }
      clrValue = (object) null;
      return false;
    }

    private static MethodInfo FindICollectionOfElementTypeAddMethod(
      Type collectionType,
      Type elementType)
    {
      return typeof (ICollection<>).MakeGenericType(elementType).GetMethod("Add");
    }

    private PropertyInfo FindProperty(Type clrObjectType, string propertyName)
    {
      if (this.tryGetClrPropertyInfoDelegate != null)
      {
        PropertyInfo propertyInfo = (PropertyInfo) null;
        return this.tryGetClrPropertyInfoDelegate(clrObjectType, propertyName, out propertyInfo) ? propertyInfo : (PropertyInfo) null;
      }
      List<PropertyInfo> list = ((IEnumerable<PropertyInfo>) clrObjectType.GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name == propertyName)).ToList<PropertyInfo>();
      switch (list.Count)
      {
        case 0:
          return (PropertyInfo) null;
        case 1:
          return list[0];
        default:
          PropertyInfo property = list[0];
          for (int index = 1; index < list.Count; ++index)
          {
            PropertyInfo propertyInfo = list[index];
            if (property.DeclaringType.IsAssignableFrom(propertyInfo.DeclaringType))
              property = propertyInfo;
          }
          return property;
      }
    }

    private static string GetEdmValueInterfaceName(IEdmValue edmValue)
    {
      Type type = typeof (IEdmValue);
      foreach (Type c in (IEnumerable<Type>) ((IEnumerable<Type>) edmValue.GetType().GetInterfaces()).OrderBy<Type, string>((Func<Type, string>) (i => i.FullName)))
      {
        if (type.IsAssignableFrom(c) && type != c)
          type = c;
      }
      return type.Name;
    }

    private static bool IsBuiltInOrEnumType(Type type) => type.IsPrimitive() || type == typeof (string) || type == typeof (Decimal) || type.IsEnum();

    private object AsClrValue(IEdmValue edmValue, Type clrType, bool convertEnumValues)
    {
      if (!EdmToClrConverter.IsBuiltInOrEnumType(clrType))
      {
        if (clrType.IsGenericType() && clrType.GetGenericTypeDefinition() == EdmToClrConverter.TypeNullableOfT)
          return edmValue is IEdmNullValue ? (object) null : this.AsClrValue(edmValue, ((IEnumerable<Type>) clrType.GetGenericArguments()).Single<Type>());
        if (clrType == typeof (Guid))
          return (object) EdmToClrConverter.AsClrGuid(edmValue);
        if (clrType == typeof (Date))
          return (object) EdmToClrConverter.AsClrDate(edmValue);
        if (clrType == typeof (DateTimeOffset))
          return (object) EdmToClrConverter.AsClrDateTimeOffset(edmValue);
        if (clrType == typeof (TimeOfDay))
          return (object) EdmToClrConverter.AsClrTimeOfDay(edmValue);
        if (clrType == typeof (TimeSpan))
          return (object) EdmToClrConverter.AsClrDuration(edmValue);
        if (clrType == typeof (byte[]))
          return (object) EdmToClrConverter.AsClrByteArray(edmValue);
        return clrType.IsGenericType() && clrType.IsInterface() && (clrType.GetGenericTypeDefinition() == EdmToClrConverter.TypeICollectionOfT || clrType.GetGenericTypeDefinition() == EdmToClrConverter.TypeIListOfT || clrType.GetGenericTypeDefinition() == EdmToClrConverter.TypeIEnumerableOfT) ? this.AsListOfT(edmValue, clrType) : this.AsClrObject(edmValue, clrType);
      }
      if (clrType.IsEnum())
      {
        Type underlyingType = Enum.GetUnderlyingType(clrType);
        IEdmEnumValue edmEnumValue = edmValue as IEdmEnumValue;
        object clrValue = (object) null;
        if (edmEnumValue != null)
        {
          if (edmEnumValue.Value is EdmEnumMemberValue edmEnumMemberValue && !EdmToClrConverter.TryConvertEnumType(underlyingType, edmEnumMemberValue.Value, out clrValue))
            throw new InvalidCastException(Strings.EdmToClr_UnsupportedType((object) underlyingType));
        }
        else if (!EdmToClrConverter.TryConvertAsNonGuidPrimitiveType(underlyingType, edmValue, out clrValue))
          throw new InvalidCastException(Strings.EdmToClr_UnsupportedType((object) underlyingType));
        return !convertEnumValues ? clrValue : this.GetEnumValue(clrValue, clrType);
      }
      object clrValue1 = (object) null;
      if (!EdmToClrConverter.TryConvertAsNonGuidPrimitiveType(clrType, edmValue, out clrValue1))
        throw new InvalidCastException(Strings.EdmToClr_UnsupportedType((object) clrType));
      return clrValue1;
    }

    private static bool TryConvertEnumType(Type type, long enumValue, out object clrValue)
    {
      if (type == typeof (sbyte))
      {
        clrValue = (object) (sbyte) enumValue;
        return true;
      }
      if (type == typeof (byte))
      {
        clrValue = (object) (byte) enumValue;
        return true;
      }
      if (type == typeof (short))
      {
        clrValue = (object) (short) enumValue;
        return true;
      }
      if (type == typeof (ushort))
      {
        clrValue = (object) (ushort) enumValue;
        return true;
      }
      if (type == typeof (int))
      {
        clrValue = (object) (int) enumValue;
        return true;
      }
      if (type == typeof (uint))
      {
        clrValue = (object) (uint) enumValue;
        return true;
      }
      if (type == typeof (long))
      {
        clrValue = (object) enumValue;
        return true;
      }
      clrValue = (object) null;
      return false;
    }

    private object AsListOfT(IEdmValue edmValue, Type clrType)
    {
      if (edmValue is IEdmNullValue)
        return (object) null;
      Type type = ((IEnumerable<Type>) clrType.GetGenericArguments()).Single<Type>();
      MethodInfo methodInfo;
      if (!this.enumerableConverters.TryGetValue(type, out methodInfo))
      {
        methodInfo = EdmToClrConverter.EnumerableToListOfTMethodInfo.MakeGenericMethod(type);
        this.enumerableConverters.Add(type, methodInfo);
      }
      try
      {
        return methodInfo.Invoke((object) null, new object[1]
        {
          (object) this.AsIEnumerable(edmValue, type)
        });
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null && ex.InnerException is InvalidCastException)
          throw ex.InnerException;
        throw;
      }
    }

    private object GetEnumValue(object clrValue, Type clrType)
    {
      MethodInfo methodInfo;
      if (!this.enumTypeConverters.TryGetValue(clrType, out methodInfo))
      {
        methodInfo = EdmToClrConverter.CastToClrTypeMethodInfo.MakeGenericMethod(clrType);
        this.enumTypeConverters.Add(clrType, methodInfo);
      }
      try
      {
        return methodInfo.Invoke((object) null, new object[1]
        {
          clrValue
        });
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null && ex.InnerException is InvalidCastException)
          throw ex.InnerException;
        throw;
      }
    }

    private object AsClrObject(IEdmValue edmValue, Type clrObjectType)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(edmValue, nameof (edmValue));
      EdmUtil.CheckArgumentNull<Type>(clrObjectType, nameof (clrObjectType));
      switch (edmValue)
      {
        case IEdmNullValue _:
          return (object) null;
        case IEdmStructuredValue edmStructuredValue:
          object objectInstance;
          if (this.convertedObjects.TryGetValue(edmStructuredValue, out objectInstance))
            return objectInstance;
          if (!clrObjectType.IsClass())
            throw new InvalidCastException(Strings.EdmToClr_StructuredValueMappedToNonClass);
          bool objectInstanceInitialized;
          if (this.tryCreateObjectInstanceDelegate != null && this.tryCreateObjectInstanceDelegate(edmStructuredValue, clrObjectType, this, out objectInstance, out objectInstanceInitialized))
          {
            if (objectInstance != null)
            {
              Type type = objectInstance.GetType();
              clrObjectType = clrObjectType.IsAssignableFrom(type) ? type : throw new InvalidCastException(Strings.EdmToClr_TryCreateObjectInstanceReturnedWrongObject((object) type.FullName, (object) clrObjectType.FullName));
            }
          }
          else
          {
            objectInstance = Activator.CreateInstance(clrObjectType);
            objectInstanceInitialized = false;
          }
          this.convertedObjects[edmStructuredValue] = objectInstance;
          if (!objectInstanceInitialized && objectInstance != null)
            this.PopulateObjectProperties(edmStructuredValue, objectInstance, clrObjectType);
          return objectInstance;
        case IEdmCollectionValue _:
          throw new InvalidCastException(Strings.EdmToClr_CannotConvertEdmCollectionValueToClrType((object) clrObjectType.FullName));
        default:
          throw new InvalidCastException(Strings.EdmToClr_CannotConvertEdmValueToClrType((object) EdmToClrConverter.GetEdmValueInterfaceName(edmValue), (object) clrObjectType.FullName));
      }
    }

    private void PopulateObjectProperties(
      IEdmStructuredValue edmValue,
      object clrObject,
      Type clrObjectType)
    {
      HashSetInternal<string> hashSetInternal = new HashSetInternal<string>();
      foreach (IEdmPropertyValue propertyValue in edmValue.PropertyValues)
      {
        PropertyInfo property = this.FindProperty(clrObjectType, propertyValue.Name);
        if (property != (PropertyInfo) null)
        {
          if (hashSetInternal.Contains(propertyValue.Name))
            throw new InvalidCastException(Strings.EdmToClr_StructuredPropertyDuplicateValue((object) propertyValue.Name));
          if (!this.TrySetCollectionProperty(property, clrObject, propertyValue))
          {
            object obj = this.AsClrValue(propertyValue.Value, property.PropertyType);
            property.SetValue(clrObject, obj, (object[]) null);
          }
          hashSetInternal.Add(propertyValue.Name);
        }
      }
    }

    private bool TrySetCollectionProperty(
      PropertyInfo clrProperty,
      object clrObject,
      IEdmPropertyValue propertyValue)
    {
      if (propertyValue.Value is IEdmNullValue)
        return false;
      Type propertyType = clrProperty.PropertyType;
      if (propertyType.IsGenericType())
      {
        bool flag = propertyType.GetGenericTypeDefinition() == EdmToClrConverter.TypeIEnumerableOfT;
        IEnumerable<Type> interfaces = (IEnumerable<Type>) propertyType.GetInterfaces();
        if (flag || interfaces.Any<Type>((Func<Type, bool>) (t => t.GetGenericTypeDefinition() == EdmToClrConverter.TypeIEnumerableOfT)))
        {
          object instance = clrProperty.GetValue(clrObject, (object[]) null);
          Type elementType = ((IEnumerable<Type>) propertyType.GetGenericArguments()).Single<Type>();
          Type type;
          if (instance == null)
          {
            type = EdmToClrConverter.TypeListOfT.MakeGenericType(elementType);
            instance = Activator.CreateInstance(type);
            clrProperty.SetValue(clrObject, instance, (object[]) null);
          }
          else
          {
            if (flag)
              throw new InvalidCastException(Strings.EdmToClr_IEnumerableOfTPropertyAlreadyHasValue((object) clrProperty.Name, (object) clrProperty.DeclaringType.FullName));
            type = instance.GetType();
          }
          MethodInfo elementTypeAddMethod = EdmToClrConverter.FindICollectionOfElementTypeAddMethod(type, elementType);
          foreach (object obj in this.AsIEnumerable(propertyValue.Value, elementType))
          {
            try
            {
              elementTypeAddMethod.Invoke(instance, new object[1]
              {
                obj
              });
            }
            catch (TargetInvocationException ex)
            {
              if (ex.InnerException != null && ex.InnerException is InvalidCastException)
                throw ex.InnerException;
              throw;
            }
          }
          return true;
        }
      }
      return false;
    }

    private IEnumerable AsIEnumerable(IEdmValue edmValue, Type elementType)
    {
      foreach (IEdmDelayedValue element in ((IEdmCollectionValue) edmValue).Elements)
      {
        if (element.Value != null || elementType.IsGenericType() && elementType.GetGenericTypeDefinition() == EdmToClrConverter.TypeNullableOfT)
          yield return this.AsClrValue(element.Value, elementType);
      }
    }

    private static class CastHelper
    {
      public static T CastToClrType<T>(object obj) => (T) obj;

      public static List<T> EnumerableToListOfT<T>(IEnumerable enumerable) => enumerable.Cast<T>().ToList<T>();
    }
  }
}
