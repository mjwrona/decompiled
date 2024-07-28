// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Utilities.TypeConverter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;

namespace YamlDotNet.Serialization.Utilities
{
  public static class TypeConverter
  {
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    public static void RegisterTypeConverter<TConvertible, TConverter>() where TConverter : System.ComponentModel.TypeConverter
    {
      if (TypeDescriptor.GetAttributes(typeof (TConvertible)).OfType<TypeConverterAttribute>().Any<TypeConverterAttribute>((Func<TypeConverterAttribute, bool>) (a => a.ConverterTypeName == typeof (TConverter).AssemblyQualifiedName)))
        return;
      TypeDescriptor.AddAttributes(typeof (TConvertible), new Attribute[1]
      {
        (Attribute) new TypeConverterAttribute(typeof (TConverter))
      });
    }

    public static T ChangeType<T>(object value) => (T) TypeConverter.ChangeType(value, typeof (T));

    public static T ChangeType<T>(object value, IFormatProvider provider) => (T) TypeConverter.ChangeType(value, typeof (T), provider);

    public static T ChangeType<T>(object value, CultureInfo culture) => (T) TypeConverter.ChangeType(value, typeof (T), culture);

    public static object ChangeType(object value, Type destinationType) => TypeConverter.ChangeType(value, destinationType, CultureInfo.InvariantCulture);

    public static object ChangeType(object value, Type destinationType, IFormatProvider provider) => TypeConverter.ChangeType(value, destinationType, (CultureInfo) new CultureInfoAdapter(CultureInfo.CurrentCulture, provider));

    public static object ChangeType(object value, Type destinationType, CultureInfo culture)
    {
      if (value == null || value.IsDbNull())
        return !destinationType.IsValueType() ? (object) null : Activator.CreateInstance(destinationType);
      Type type1 = value.GetType();
      if ((object) destinationType == (object) type1 || destinationType.IsAssignableFrom(type1))
        return value;
      if (destinationType.IsGenericType() && (object) destinationType.GetGenericTypeDefinition() == (object) typeof (Nullable<>))
      {
        Type genericArgument = destinationType.GetGenericArguments()[0];
        object obj = TypeConverter.ChangeType(value, genericArgument, culture);
        return Activator.CreateInstance(destinationType, obj);
      }
      if (destinationType.IsEnum())
        return !(value is string str) ? value : Enum.Parse(destinationType, str, true);
      if ((object) destinationType == (object) typeof (bool))
      {
        if ("0".Equals(value))
          return (object) false;
        if ("1".Equals(value))
          return (object) true;
      }
      System.ComponentModel.TypeConverter converter1 = TypeDescriptor.GetConverter(type1);
      if (converter1 != null && converter1.CanConvertTo(destinationType))
        return converter1.ConvertTo((ITypeDescriptorContext) null, culture, value, destinationType);
      System.ComponentModel.TypeConverter converter2 = TypeDescriptor.GetConverter(destinationType);
      if (converter2 != null && converter2.CanConvertFrom(type1))
        return converter2.ConvertFrom((ITypeDescriptorContext) null, culture, value);
      Type[] typeArray = new Type[2]
      {
        type1,
        destinationType
      };
      foreach (Type type2 in typeArray)
      {
        foreach (MethodInfo publicStaticMethod in type2.GetPublicStaticMethods())
        {
          if ((!publicStaticMethod.IsSpecialName || !(publicStaticMethod.Name == "op_Implicit") && !(publicStaticMethod.Name == "op_Explicit") ? 0 : (destinationType.IsAssignableFrom(publicStaticMethod.ReturnParameter.ParameterType) ? 1 : 0)) != 0)
          {
            ParameterInfo[] parameters = publicStaticMethod.GetParameters();
            if ((parameters.Length != 1 ? 0 : (parameters[0].ParameterType.IsAssignableFrom(type1) ? 1 : 0)) != 0)
            {
              try
              {
                return publicStaticMethod.Invoke((object) null, new object[1]
                {
                  value
                });
              }
              catch (TargetInvocationException ex)
              {
                throw ex.Unwrap();
              }
            }
          }
        }
      }
      if ((object) type1 == (object) typeof (string))
      {
        try
        {
          MethodInfo publicStaticMethod1 = destinationType.GetPublicStaticMethod("Parse", typeof (string), typeof (IFormatProvider));
          if ((object) publicStaticMethod1 != null)
            return publicStaticMethod1.Invoke((object) null, new object[2]
            {
              value,
              (object) culture
            });
          MethodInfo publicStaticMethod2 = destinationType.GetPublicStaticMethod("Parse", typeof (string));
          if ((object) publicStaticMethod2 != null)
            return publicStaticMethod2.Invoke((object) null, new object[1]
            {
              value
            });
        }
        catch (TargetInvocationException ex)
        {
          throw ex.Unwrap();
        }
      }
      return (object) destinationType == (object) typeof (TimeSpan) ? (object) TimeSpan.Parse((string) TypeConverter.ChangeType(value, typeof (string), CultureInfo.InvariantCulture)) : Convert.ChangeType(value, destinationType, (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
