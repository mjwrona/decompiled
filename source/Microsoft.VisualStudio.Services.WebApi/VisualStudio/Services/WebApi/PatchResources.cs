// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.PatchResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class PatchResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (PatchResources), typeof (PatchResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => PatchResources.s_resMgr;

    private static string Get(string resourceName) => PatchResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? PatchResources.Get(resourceName) : PatchResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PatchResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PatchResources.GetInt(resourceName) : (int) PatchResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PatchResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PatchResources.GetBool(resourceName) : (bool) PatchResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => PatchResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PatchResources.Get(resourceName, culture);
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

    public static string CannotReplaceNonExistantValue(object arg0) => PatchResources.Format(nameof (CannotReplaceNonExistantValue), arg0);

    public static string CannotReplaceNonExistantValue(object arg0, CultureInfo culture) => PatchResources.Format(nameof (CannotReplaceNonExistantValue), culture, arg0);

    public static string IndexOutOfRange(object arg0) => PatchResources.Format(nameof (IndexOutOfRange), arg0);

    public static string IndexOutOfRange(object arg0, CultureInfo culture) => PatchResources.Format(nameof (IndexOutOfRange), culture, arg0);

    public static string InsertNotSupported(object arg0) => PatchResources.Format(nameof (InsertNotSupported), arg0);

    public static string InsertNotSupported(object arg0, CultureInfo culture) => PatchResources.Format(nameof (InsertNotSupported), culture, arg0);

    public static string InvalidOperation() => PatchResources.Get(nameof (InvalidOperation));

    public static string InvalidOperation(CultureInfo culture) => PatchResources.Get(nameof (InvalidOperation), culture);

    public static string InvalidValue(object arg0, object arg1) => PatchResources.Format(nameof (InvalidValue), arg0, arg1);

    public static string InvalidValue(object arg0, object arg1, CultureInfo culture) => PatchResources.Format(nameof (InvalidValue), culture, arg0, arg1);

    public static string MoveCopyNotImplemented() => PatchResources.Get(nameof (MoveCopyNotImplemented));

    public static string MoveCopyNotImplemented(CultureInfo culture) => PatchResources.Get(nameof (MoveCopyNotImplemented), culture);

    public static string NullOrEmptyOperations() => PatchResources.Get(nameof (NullOrEmptyOperations));

    public static string NullOrEmptyOperations(CultureInfo culture) => PatchResources.Get(nameof (NullOrEmptyOperations), culture);

    public static string PathCannotBeNull() => PatchResources.Get(nameof (PathCannotBeNull));

    public static string PathCannotBeNull(CultureInfo culture) => PatchResources.Get(nameof (PathCannotBeNull), culture);

    public static string PathInvalidEndValue() => PatchResources.Get(nameof (PathInvalidEndValue));

    public static string PathInvalidEndValue(CultureInfo culture) => PatchResources.Get(nameof (PathInvalidEndValue), culture);

    public static string PathInvalidStartValue() => PatchResources.Get(nameof (PathInvalidStartValue));

    public static string PathInvalidStartValue(CultureInfo culture) => PatchResources.Get(nameof (PathInvalidStartValue), culture);

    public static string TargetCannotBeNull() => PatchResources.Get(nameof (TargetCannotBeNull));

    public static string TargetCannotBeNull(CultureInfo culture) => PatchResources.Get(nameof (TargetCannotBeNull), culture);

    public static string TestFailed(object arg0, object arg1, object arg2) => PatchResources.Format(nameof (TestFailed), arg0, arg1, arg2);

    public static string TestFailed(object arg0, object arg1, object arg2, CultureInfo culture) => PatchResources.Format(nameof (TestFailed), culture, arg0, arg1, arg2);

    public static string TestNotImplementedForDictionary() => PatchResources.Get(nameof (TestNotImplementedForDictionary));

    public static string TestNotImplementedForDictionary(CultureInfo culture) => PatchResources.Get(nameof (TestNotImplementedForDictionary), culture);

    public static string TestNotImplementedForList() => PatchResources.Get(nameof (TestNotImplementedForList));

    public static string TestNotImplementedForList(CultureInfo culture) => PatchResources.Get(nameof (TestNotImplementedForList), culture);

    public static string UnableToEvaluatePath(object arg0) => PatchResources.Format(nameof (UnableToEvaluatePath), arg0);

    public static string UnableToEvaluatePath(object arg0, CultureInfo culture) => PatchResources.Format(nameof (UnableToEvaluatePath), culture, arg0);

    public static string ValueCannotBeNull() => PatchResources.Get(nameof (ValueCannotBeNull));

    public static string ValueCannotBeNull(CultureInfo culture) => PatchResources.Get(nameof (ValueCannotBeNull), culture);

    public static string ValueNotNull() => PatchResources.Get(nameof (ValueNotNull));

    public static string ValueNotNull(CultureInfo culture) => PatchResources.Get(nameof (ValueNotNull), culture);

    public static string JsonPatchNull() => PatchResources.Get(nameof (JsonPatchNull));

    public static string JsonPatchNull(CultureInfo culture) => PatchResources.Get(nameof (JsonPatchNull), culture);

    public static string InvalidFieldName(object arg0) => PatchResources.Format(nameof (InvalidFieldName), arg0);

    public static string InvalidFieldName(object arg0, CultureInfo culture) => PatchResources.Format(nameof (InvalidFieldName), culture, arg0);
  }
}
