// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.PlatformHelper
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.Spatial
{
  internal static class PlatformHelper
  {
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static readonly Type[] EmptyTypes = new Type[0];
    internal static readonly Regex DateValidator = PlatformHelper.CreateCompiled("^(\\d{4})-(0?[1-9]|1[012])-(0?[1-9]|[12]\\d|3[0|1])$", RegexOptions.Singleline);
    internal static readonly Regex TimeOfDayValidator = PlatformHelper.CreateCompiled("^(0?\\d|1\\d|2[0-3]):(0?\\d|[1-5]\\d)(:(0?\\d|[1-5]\\d)(\\.\\d{1,7})?)?$", RegexOptions.Singleline);
    internal static readonly Regex PotentialDateTimeOffsetValidator = PlatformHelper.CreateCompiled("^(\\d{2,4})-(\\d{1,2})-(\\d{1,2})(T|(\\s+))(\\d{1,2}):(\\d{1,2})", RegexOptions.Singleline);
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static readonly string UriSchemeHttp = Uri.UriSchemeHttp;
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static readonly string UriSchemeHttps = Uri.UriSchemeHttps;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static Assembly GetAssembly(this Type type) => type.Assembly;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsValueType(this Type type) => type.IsValueType;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsAbstract(this Type type) => type.IsAbstract;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsGenericType(this Type type) => type.IsGenericType;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsGenericTypeDefinition(this Type type) => type.IsGenericTypeDefinition;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsVisible(this Type type) => type.IsVisible;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsInterface(this Type type) => type.IsInterface;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsClass(this Type type) => type.IsClass;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsEnum(this Type type) => type.IsEnum;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static Type GetBaseType(this Type type) => type.BaseType;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool ContainsGenericParameters(this Type type) => type.ContainsGenericParameters;

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

    internal static UnicodeCategory GetUnicodeCategory(char c) => char.GetUnicodeCategory(c);

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsProperty(MemberInfo member) => member.MemberType == MemberTypes.Property;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsPrimitive(this Type type) => type.IsPrimitive;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsSealed(this Type type) => type.IsSealed;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool IsMethod(MemberInfo member) => member.MemberType == MemberTypes.Method;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static bool AreMembersEqual(MemberInfo member1, MemberInfo member2) => member1.MetadataToken == member2.MetadataToken;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<PropertyInfo> GetPublicProperties(this Type type, bool instanceOnly) => type.GetPublicProperties(instanceOnly, false);

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<PropertyInfo> GetPublicProperties(
      this Type type,
      bool instanceOnly,
      bool declaredOnly)
    {
      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
      if (!instanceOnly)
        bindingAttr |= BindingFlags.Static;
      if (declaredOnly)
        bindingAttr |= BindingFlags.DeclaredOnly;
      return (IEnumerable<PropertyInfo>) type.GetProperties(bindingAttr);
    }

    internal static IEnumerable<PropertyInfo> GetNonPublicProperties(
      this Type type,
      bool instanceOnly,
      bool declaredOnly)
    {
      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
      if (!instanceOnly)
        bindingAttr |= BindingFlags.Static;
      if (declaredOnly)
        bindingAttr |= BindingFlags.DeclaredOnly;
      return (IEnumerable<PropertyInfo>) type.GetProperties(bindingAttr);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<ConstructorInfo> GetInstanceConstructors(
      this Type type,
      bool isPublic)
    {
      BindingFlags bindingAttr = (BindingFlags) (4 | (isPublic ? 16 : 32));
      return (IEnumerable<ConstructorInfo>) type.GetConstructors(bindingAttr);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static ConstructorInfo GetInstanceConstructor(
      this Type type,
      bool isPublic,
      Type[] argTypes)
    {
      BindingFlags bindingAttr = (BindingFlags) (4 | (isPublic ? 16 : 32));
      return type.GetConstructor(bindingAttr, (Binder) null, argTypes, (ParameterModifier[]) null);
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

    internal static IEnumerable<MethodInfo> GetMethods(this Type type) => (IEnumerable<MethodInfo>) type.GetMethods();

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static MethodInfo GetMethod(
      this Type type,
      string name,
      bool isPublic,
      bool isStatic)
    {
      BindingFlags bindingAttr = (BindingFlags) (0 | (isPublic ? 16 : 32) | (isStatic ? 8 : 4));
      return type.GetMethod(name, bindingAttr);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static MethodInfo GetMethod(
      this Type type,
      string name,
      Type[] types,
      bool isPublic,
      bool isStatic)
    {
      BindingFlags bindingAttr = (BindingFlags) (0 | (isPublic ? 16 : 32) | (isStatic ? 8 : 4));
      return type.GetMethod(name, bindingAttr, (Binder) null, types, (ParameterModifier[]) null);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<MethodInfo> GetPublicStaticMethods(this Type type) => (IEnumerable<MethodInfo>) type.GetMethods(BindingFlags.Static | BindingFlags.Public);

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
    internal static IEnumerable<Type> GetNonPublicNestedTypes(this Type type) => (IEnumerable<Type>) type.GetNestedTypes(BindingFlags.NonPublic);

    public static Regex CreateCompiled(string pattern, RegexOptions options)
    {
      options |= RegexOptions.Compiled;
      return new Regex(pattern, options);
    }
  }
}
