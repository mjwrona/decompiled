// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CommonUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class CommonUtility
  {
    private static List<TimeZoneInfoModel> s_allTimeZones;

    internal static bool ShouldIgnoreTeamContext(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.NoTeamContext");

    internal static Uri CreateUriFromRequest(string value, string requestParamName)
    {
      Uri result;
      if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out result))
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.InvalidParameterFormat, (object) requestParamName));
      return result;
    }

    internal static Guid CreateGuidFromRequest(string value, string requestParamName)
    {
      try
      {
        return new Guid(value);
      }
      catch (FormatException ex)
      {
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.InvalidParameterFormat, (object) requestParamName), (Exception) ex);
      }
    }

    internal static int CreateIntFromRequest(string value, string requestParamName)
    {
      try
      {
        return int.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (FormatException ex)
      {
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.InvalidParameterFormat, (object) requestParamName), (Exception) ex);
      }
    }

    public static int CombineHashCodes(int hashA, int hashB) => (hashA << 5) + hashA ^ hashB;

    public static void CheckCallbackName(string callbackName)
    {
      ArgumentUtility.CheckForNull<string>(callbackName, nameof (callbackName));
      foreach (char c in callbackName)
      {
        if (!char.IsLetterOrDigit(c))
          throw new ArgumentException(WACommonResources.InvalidCallbackName);
      }
    }

    public static void CheckEnumerableElements<T>(
      IEnumerable<T> enumerable,
      string enumerableName,
      Action<T, string> elementCheck)
    {
      CommonUtility.CheckEnumerableElements<T>(enumerable, enumerableName, false, elementCheck);
    }

    public static void CheckEnumerableElements<T>(
      IEnumerable<T> enumerable,
      string enumerableName,
      bool allowEmpty,
      Action<T, string> elementCheck)
    {
      if (allowEmpty)
        ArgumentUtility.CheckForNull<IEnumerable<T>>(enumerable, enumerableName);
      else
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) enumerable, enumerableName);
      int num = 0;
      foreach (T obj in enumerable)
      {
        elementCheck(obj, enumerableName + "[" + num.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + "]");
        ++num;
      }
    }

    public static ReadOnlyCollection<TimeZoneInfoModel> AllTimeZones
    {
      get
      {
        if (CommonUtility.s_allTimeZones == null)
        {
          CommonUtility.s_allTimeZones = new List<TimeZoneInfoModel>();
          CommonUtility.s_allTimeZones.AddRange(TimeZoneInfo.GetSystemTimeZones().Select<TimeZoneInfo, TimeZoneInfoModel>((Func<TimeZoneInfo, TimeZoneInfoModel>) (x => new TimeZoneInfoModel(x))));
        }
        return CommonUtility.s_allTimeZones.AsReadOnly();
      }
    }
  }
}
