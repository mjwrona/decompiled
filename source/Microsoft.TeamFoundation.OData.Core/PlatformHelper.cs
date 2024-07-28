// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.PlatformHelper
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.OData
{
  internal static class PlatformHelper
  {
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static readonly Type[] EmptyTypes = new Type[0];
    internal static readonly Regex DateValidator = PlatformHelper.CreateCompiled("^(\\d{4})-(0?[1-9]|1[012])-(0?[1-9]|[12]\\d|3[0|1])$", RegexOptions.Singleline);
    internal static readonly Regex TimeOfDayValidator = PlatformHelper.CreateCompiled("^(0?\\d|1\\d|2[0-3]):(0?\\d|[1-5]\\d)(:(0?\\d|[1-5]\\d)(\\.\\d{1,7})?)?$", RegexOptions.Singleline);
    internal static readonly Regex PotentialDateTimeOffsetValidator = PlatformHelper.CreateCompiled("^(\\d{2,4})-(\\d{1,2})-(\\d{1,2})(T|(\\s+))(\\d{1,2}):(\\d{1,2})", RegexOptions.Singleline);
    internal static readonly string UriSchemeHttp = "http";
    internal static readonly string UriSchemeHttps = "https";

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static Assembly GetAssembly(this Type type) => type.GetTypeInfo().Assembly;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsValueType(this Type type) => type.GetTypeInfo().IsValueType;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsAbstract(this Type type) => type.GetTypeInfo().IsAbstract;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsGenericType(this Type type) => type.GetTypeInfo().IsGenericType;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsGenericTypeDefinition(this Type type) => type.GetTypeInfo().IsGenericTypeDefinition;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsVisible(this Type type) => type.GetTypeInfo().IsVisible;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsInterface(this Type type) => type.GetTypeInfo().IsInterface;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsClass(this Type type) => type.GetTypeInfo().IsClass;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsEnum(this Type type) => type.GetTypeInfo().IsEnum;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static Type GetBaseType(this Type type) => type.GetTypeInfo().BaseType;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool ContainsGenericParameters(this Type type) => type.GetTypeInfo().ContainsGenericParameters;

    internal static bool TryConvertStringToDate(string text, out Date date)
    {
      date = new Date();
      return text != null && PlatformHelper.DateValidator.IsMatch(text) && Date.TryParse(text, (IFormatProvider) CultureInfo.InvariantCulture, out date);
    }

    internal static Date ConvertStringToDate(string text)
    {
      Date date;
      if (!PlatformHelper.TryConvertStringToDate(text, out date))
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "String '{0}' was not recognized as a valid Edm.Date.", new object[1]
        {
          (object) text
        }));
      return date;
    }

    internal static bool TryConvertStringToTimeOfDay(string text, out TimeOfDay timeOfDay)
    {
      timeOfDay = new TimeOfDay();
      return text != null && PlatformHelper.TimeOfDayValidator.IsMatch(text) && TimeOfDay.TryParse(text, (IFormatProvider) CultureInfo.InvariantCulture, out timeOfDay);
    }

    internal static TimeOfDay ConvertStringToTimeOfDay(string text)
    {
      TimeOfDay timeOfDay;
      if (!PlatformHelper.TryConvertStringToTimeOfDay(text, out timeOfDay))
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "String '{0}' was not recognized as a valid Edm.TimeOfDay.", new object[1]
        {
          (object) text
        }));
      return timeOfDay;
    }

    internal static DateTimeOffset ConvertStringToDateTimeOffset(string text)
    {
      text = PlatformHelper.AddSecondsPaddingIfMissing(text);
      DateTimeOffset dateTimeOffset = XmlConvert.ToDateTimeOffset(text);
      PlatformHelper.ValidateTimeZoneInformationInDateTimeOffsetString(text);
      return dateTimeOffset;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    private static void ValidateTimeZoneInformationInDateTimeOffsetString(string text)
    {
      if ((text.Length <= 1 || text[text.Length - 1] != 'Z' && text[text.Length - 1] != 'z') && (text.Length <= 6 || text[text.Length - 6] != '-' && text[text.Length - 6] != '+'))
        throw new FormatException(Strings.PlatformHelper_DateTimeOffsetMustContainTimeZone((object) text));
    }

    internal static string AddSecondsPaddingIfMissing(string text)
    {
      int num1 = text.IndexOf("T", StringComparison.Ordinal);
      int num2 = num1 + 6;
      if (num1 > 0 && (text.Length == num2 || text.Length > num2 && text[num2] != ':'))
        text = text.Insert(num2, ":00");
      return text;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static Type GetTypeOrThrow(string typeName) => Type.GetType(typeName, true);

    internal static UnicodeCategory GetUnicodeCategory(char c) => CharUnicodeInfo.GetUnicodeCategory(c);

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsProperty(MemberInfo member) => member is PropertyInfo;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsPrimitive(this Type type) => type.GetTypeInfo().IsPrimitive;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsSealed(this Type type) => type.GetTypeInfo().IsSealed;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsMethod(MemberInfo member) => member is MethodInfo;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool AreMembersEqual(MemberInfo member1, MemberInfo member2) => member1 == member2;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<PropertyInfo> GetPublicProperties(this Type type, bool instanceOnly) => type.GetPublicProperties(instanceOnly, false);

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<PropertyInfo> GetPublicProperties(
      this Type type,
      bool instanceOnly,
      bool declaredOnly)
    {
      return (declaredOnly ? type.GetTypeInfo().DeclaredProperties : type.GetRuntimeProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (!PlatformHelper.IsPublic(p))
          return false;
        return !instanceOnly || PlatformHelper.IsInstance(p);
      }));
    }

    internal static IEnumerable<PropertyInfo> GetNonPublicProperties(
      this Type type,
      bool instanceOnly,
      bool declaredOnly)
    {
      return (declaredOnly ? type.GetTypeInfo().DeclaredProperties : type.GetRuntimeProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (PlatformHelper.IsPublic(p))
          return false;
        return !instanceOnly || PlatformHelper.IsInstance(p);
      }));
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<ConstructorInfo> GetInstanceConstructors(
      this Type type,
      bool isPublic)
    {
      return type.GetTypeInfo().DeclaredConstructors.Where<ConstructorInfo>((Func<ConstructorInfo, bool>) (c => !c.IsStatic && isPublic == c.IsPublic));
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static ConstructorInfo GetInstanceConstructor(
      this Type type,
      bool isPublic,
      Type[] argTypes)
    {
      return type.GetInstanceConstructors(isPublic).SingleOrDefault<ConstructorInfo>((Func<ConstructorInfo, bool>) (c => PlatformHelper.CheckTypeArgs(c, argTypes)));
    }

    internal static bool TryGetMethod(
      this Type type,
      string name,
      Type[] parameterTypes,
      out MethodInfo foundMethod)
    {
      foundMethod = (MethodInfo) null;
      try
      {
        foundMethod = type.GetMethod(name, parameterTypes);
        return foundMethod != (MethodInfo) null;
      }
      catch (ArgumentNullException ex)
      {
        return false;
      }
    }

    internal static IEnumerable<MethodInfo> GetMethods(this Type type) => type.GetRuntimeMethods();

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static MethodInfo GetMethod(
      this Type type,
      string name,
      bool isPublic,
      bool isStatic)
    {
      return type.GetRuntimeMethods().Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == name && isPublic == m.IsPublic && isStatic == m.IsStatic)).SingleOrDefault<MethodInfo>();
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static MethodInfo GetMethod(
      this Type type,
      string name,
      Type[] types,
      bool isPublic,
      bool isStatic)
    {
      MethodInfo method = type.GetMethod(name, types);
      return isPublic == method.IsPublic && isStatic == method.IsStatic ? method : (MethodInfo) null;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<MethodInfo> GetPublicStaticMethods(this Type type) => type.GetRuntimeMethods().Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.IsPublic && m.IsStatic));

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<Type> GetNonPublicNestedTypes(this Type type) => type.GetTypeInfo().DeclaredNestedTypes.Where<TypeInfo>((Func<TypeInfo, bool>) (t => !t.IsNestedPublic)).Select<TypeInfo, Type>((Func<TypeInfo, Type>) (t => t.AsType()));

    private static bool CheckTypeArgs(ConstructorInfo constructorInfo, Type[] types)
    {
      ParameterInfo[] parameters = constructorInfo.GetParameters();
      if (parameters.Length != types.Length)
        return false;
      for (int index = 0; index < parameters.Length; ++index)
      {
        if (parameters[index].ParameterType != types[index])
          return false;
      }
      return true;
    }

    internal static bool IsAssignableFrom(this Type thisType, Type otherType) => thisType.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());

    internal static bool IsSubclassOf(this Type thisType, Type otherType)
    {
      if (thisType == otherType)
        return false;
      for (Type baseType = thisType.GetTypeInfo().BaseType; baseType != (Type) null; baseType = baseType.GetTypeInfo().BaseType)
      {
        if (baseType == otherType)
          return true;
      }
      return false;
    }

    internal static MethodInfo GetMethod(this Type type, string name) => type.GetRuntimeMethods().Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.IsPublic && m.Name == name)).SingleOrDefault<MethodInfo>();

    internal static MethodInfo GetMethod(this Type type, string name, Type[] types) => type.GetRuntimeMethod(name, types);

    internal static MethodInfo GetMethodWithGenericArgs(
      this Type type,
      string name,
      bool isPublic,
      bool isStatic,
      int genericArgCount)
    {
      return type.GetRuntimeMethods().Single<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == name && m.IsPublic == isPublic && m.IsStatic == isStatic && ((IEnumerable<Type>) m.GetGenericArguments()).Count<Type>() == genericArgCount));
    }

    internal static PropertyInfo GetProperty(this Type type, string name, Type returnType)
    {
      PropertyInfo runtimeProperty = type.GetRuntimeProperty(name);
      return runtimeProperty != (PropertyInfo) null && runtimeProperty.PropertyType == returnType ? runtimeProperty : (PropertyInfo) null;
    }

    internal static PropertyInfo GetProperty(this Type type, string name) => type.GetRuntimeProperty(name);

    internal static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
    {
      MethodInfo getMethod = propertyInfo.GetMethod;
      return getMethod != (MethodInfo) null && getMethod.IsPublic ? getMethod : (MethodInfo) null;
    }

    internal static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
    {
      MethodInfo setMethod = propertyInfo.SetMethod;
      return setMethod != (MethodInfo) null && setMethod.IsPublic ? setMethod : (MethodInfo) null;
    }

    internal static MethodInfo GetBaseDefinition(this MethodInfo methodInfo) => methodInfo.GetRuntimeBaseDefinition();

    internal static IEnumerable<PropertyInfo> GetProperties(this Type type) => type.GetPublicProperties(false);

    internal static IEnumerable<FieldInfo> GetFields(this Type type) => type.GetRuntimeFields().Where<FieldInfo>((Func<FieldInfo, bool>) (m => m.IsPublic));

    internal static IEnumerable<object> GetCustomAttributes(
      this Type type,
      Type attributeType,
      bool inherit)
    {
      return (IEnumerable<object>) type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
    }

    internal static IEnumerable<object> GetCustomAttributes(this Type type, bool inherit) => (IEnumerable<object>) type.GetTypeInfo().GetCustomAttributes(inherit);

    internal static Type[] GetGenericArguments(this Type type) => type.GetTypeInfo().IsGenericTypeDefinition ? type.GetTypeInfo().GenericTypeParameters : type.GenericTypeArguments;

    internal static IEnumerable<Type> GetInterfaces(this Type type) => type.GetTypeInfo().ImplementedInterfaces;

    internal static bool IsInstanceOfType(this Type type, object obj) => type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());

    internal static Type GetType(this Assembly assembly, string typeName, bool throwOnError)
    {
      Type type = assembly.GetType(typeName);
      return !(type == (Type) null & throwOnError) ? type : throw new TypeLoadException();
    }

    internal static IEnumerable<Type> GetTypes(this Assembly assembly) => assembly.DefinedTypes.Select<TypeInfo, Type>((Func<TypeInfo, Type>) (dt => dt.AsType()));

    internal static FieldInfo GetField(this Type type, string name) => ((IEnumerable<FieldInfo>) type.GetFields()).SingleOrDefault<FieldInfo>((Func<FieldInfo, bool>) (field => field.Name == name));

    private static bool IsInstance(PropertyInfo propertyInfo)
    {
      if (propertyInfo.GetMethod != (MethodInfo) null && !propertyInfo.GetMethod.IsStatic)
        return true;
      return propertyInfo.SetMethod != (MethodInfo) null && !propertyInfo.SetMethod.IsStatic;
    }

    private static bool IsPublic(PropertyInfo propertyInfo)
    {
      if (propertyInfo.GetMethod != (MethodInfo) null && propertyInfo.GetMethod.IsPublic)
        return true;
      return propertyInfo.SetMethod != (MethodInfo) null && propertyInfo.SetMethod.IsPublic;
    }

    public static Regex CreateCompiled(string pattern, RegexOptions options)
    {
      options |= RegexOptions.None;
      return new Regex(pattern, options);
    }
  }
}
