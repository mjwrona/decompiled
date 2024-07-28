// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HttpStreams.Resources
// Assembly: Microsoft.VisualStudio.Services.HttpStreams, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08EEF7AF-2ADD-4A01-B7DB-5972BBFA47F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.HttpStreams.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.HttpStreams
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.HttpStreams.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.HttpStreams.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.HttpStreams.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.HttpStreams.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.HttpStreams.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.HttpStreams.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.HttpStreams.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.HttpStreams.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.HttpStreams.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.HttpStreams.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(resourceName, culture);
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

    public static string Error_CantDetermineLength() => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_CantDetermineLength));

    public static string Error_CantDetermineLength(CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_CantDetermineLength), culture);

    public static string Error_NoContentRangeHeader() => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_NoContentRangeHeader));

    public static string Error_NoContentRangeHeader(CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_NoContentRangeHeader), culture);

    public static string Error_NoContentRangeLength() => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_NoContentRangeLength));

    public static string Error_NoContentRangeLength(CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_NoContentRangeLength), culture);

    public static string Error_NoContentRangeRange() => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_NoContentRangeRange));

    public static string Error_NoContentRangeRange(CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_NoContentRangeRange), culture);

    public static string Error_NotRangeResponse() => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_NotRangeResponse));

    public static string Error_NotRangeResponse(CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_NotRangeResponse), culture);

    public static string Error_PositionNotInBuffer(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_PositionNotInBuffer), arg0, arg1, arg2);

    public static string Error_PositionNotInBuffer(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_PositionNotInBuffer), culture, arg0, arg1, arg2);
    }

    public static string Error_PositionNotInBuffer_NullBuffer(object arg0) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_PositionNotInBuffer_NullBuffer), arg0);

    public static string Error_PositionNotInBuffer_NullBuffer(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_PositionNotInBuffer_NullBuffer), culture, arg0);

    public static string Error_ReadPastEnd(object arg0, object arg1) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_ReadPastEnd), arg0, arg1);

    public static string Error_ReadPastEnd(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_ReadPastEnd), culture, arg0, arg1);

    public static string Error_ReturnedRangeTooLarge() => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_ReturnedRangeTooLarge));

    public static string Error_ReturnedRangeTooLarge(CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_ReturnedRangeTooLarge), culture);

    public static string Error_SeekPastBeginning() => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_SeekPastBeginning));

    public static string Error_SeekPastBeginning(CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_SeekPastBeginning), culture);

    public static string Error_ServerSentTooFewBytes(object arg0, object arg1) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_ServerSentTooFewBytes), arg0, arg1);

    public static string Error_ServerSentTooFewBytes(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_ServerSentTooFewBytes), culture, arg0, arg1);

    public static string Error_UnknownSeekOrigin(object arg0) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_UnknownSeekOrigin), arg0);

    public static string Error_UnknownSeekOrigin(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Format(nameof (Error_UnknownSeekOrigin), culture, arg0);

    public static string Error_WritesNotSupported() => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_WritesNotSupported));

    public static string Error_WritesNotSupported(CultureInfo culture) => Microsoft.VisualStudio.Services.HttpStreams.Resources.Get(nameof (Error_WritesNotSupported), culture);
  }
}
