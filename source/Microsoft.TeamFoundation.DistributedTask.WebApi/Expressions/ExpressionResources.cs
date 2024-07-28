// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ExpressionResources
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal static class ExpressionResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ExpressionResources), typeof (ExpressionResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ExpressionResources.s_resMgr;

    private static string Get(string resourceName) => ExpressionResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ExpressionResources.Get(resourceName) : ExpressionResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ExpressionResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ExpressionResources.GetInt(resourceName) : (int) ExpressionResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ExpressionResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ExpressionResources.GetBool(resourceName) : (bool) ExpressionResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ExpressionResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ExpressionResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string ExceededAllowedMemory(object arg0) => ExpressionResources.Format(nameof (ExceededAllowedMemory), arg0);

    public static string ExceededAllowedMemory(object arg0, CultureInfo culture) => ExpressionResources.Format(nameof (ExceededAllowedMemory), culture, arg0);

    public static string ExceededMaxExpressionDepth(object arg0) => ExpressionResources.Format(nameof (ExceededMaxExpressionDepth), arg0);

    public static string ExceededMaxExpressionDepth(object arg0, CultureInfo culture) => ExpressionResources.Format(nameof (ExceededMaxExpressionDepth), culture, arg0);

    public static string ExceededMaxExpressionLength(object arg0) => ExpressionResources.Format(nameof (ExceededMaxExpressionLength), arg0);

    public static string ExceededMaxExpressionLength(object arg0, CultureInfo culture) => ExpressionResources.Format(nameof (ExceededMaxExpressionLength), culture, arg0);

    public static string ExpectedPropertyName() => ExpressionResources.Get(nameof (ExpectedPropertyName));

    public static string ExpectedPropertyName(CultureInfo culture) => ExpressionResources.Get(nameof (ExpectedPropertyName), culture);

    public static string ExpectedStartParameter() => ExpressionResources.Get(nameof (ExpectedStartParameter));

    public static string ExpectedStartParameter(CultureInfo culture) => ExpressionResources.Get(nameof (ExpectedStartParameter), culture);

    public static string InvalidFormatArgIndex(object arg0) => ExpressionResources.Format(nameof (InvalidFormatArgIndex), arg0);

    public static string InvalidFormatArgIndex(object arg0, CultureInfo culture) => ExpressionResources.Format(nameof (InvalidFormatArgIndex), culture, arg0);

    public static string InvalidFormatSpecifiers(object arg0, object arg1) => ExpressionResources.Format(nameof (InvalidFormatSpecifiers), arg0, arg1);

    public static string InvalidFormatSpecifiers(object arg0, object arg1, CultureInfo culture) => ExpressionResources.Format(nameof (InvalidFormatSpecifiers), culture, arg0, arg1);

    public static string InvalidFormatString(object arg0) => ExpressionResources.Format(nameof (InvalidFormatString), arg0);

    public static string InvalidFormatString(object arg0, CultureInfo culture) => ExpressionResources.Format(nameof (InvalidFormatString), culture, arg0);

    public static string KeyNotFound(object arg0) => ExpressionResources.Format(nameof (KeyNotFound), arg0);

    public static string KeyNotFound(object arg0, CultureInfo culture) => ExpressionResources.Format(nameof (KeyNotFound), culture, arg0);

    public static string ParseErrorWithFwlink(object arg0) => ExpressionResources.Format(nameof (ParseErrorWithFwlink), arg0);

    public static string ParseErrorWithFwlink(object arg0, CultureInfo culture) => ExpressionResources.Format(nameof (ParseErrorWithFwlink), culture, arg0);

    public static string ParseErrorWithTokenInfo(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ExpressionResources.Format(nameof (ParseErrorWithTokenInfo), arg0, arg1, arg2, arg3);
    }

    public static string ParseErrorWithTokenInfo(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ExpressionResources.Format(nameof (ParseErrorWithTokenInfo), culture, arg0, arg1, arg2, arg3);
    }

    public static string TypeCastError(object arg0, object arg1, object arg2) => ExpressionResources.Format(nameof (TypeCastError), arg0, arg1, arg2);

    public static string TypeCastError(object arg0, object arg1, object arg2, CultureInfo culture) => ExpressionResources.Format(nameof (TypeCastError), culture, arg0, arg1, arg2);

    public static string TypeCastErrorNoValue(object arg0, object arg1) => ExpressionResources.Format(nameof (TypeCastErrorNoValue), arg0, arg1);

    public static string TypeCastErrorNoValue(object arg0, object arg1, CultureInfo culture) => ExpressionResources.Format(nameof (TypeCastErrorNoValue), culture, arg0, arg1);

    public static string TypeCastErrorWithError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ExpressionResources.Format(nameof (TypeCastErrorWithError), arg0, arg1, arg2, arg3);
    }

    public static string TypeCastErrorWithError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ExpressionResources.Format(nameof (TypeCastErrorWithError), culture, arg0, arg1, arg2, arg3);
    }

    public static string UnclosedFunction() => ExpressionResources.Get(nameof (UnclosedFunction));

    public static string UnclosedFunction(CultureInfo culture) => ExpressionResources.Get(nameof (UnclosedFunction), culture);

    public static string UnclosedIndexer() => ExpressionResources.Get(nameof (UnclosedIndexer));

    public static string UnclosedIndexer(CultureInfo culture) => ExpressionResources.Get(nameof (UnclosedIndexer), culture);

    public static string UnexpectedSymbol() => ExpressionResources.Get(nameof (UnexpectedSymbol));

    public static string UnexpectedSymbol(CultureInfo culture) => ExpressionResources.Get(nameof (UnexpectedSymbol), culture);

    public static string UnrecognizedValue() => ExpressionResources.Get(nameof (UnrecognizedValue));

    public static string UnrecognizedValue(CultureInfo culture) => ExpressionResources.Get(nameof (UnrecognizedValue), culture);
  }
}
