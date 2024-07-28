// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.CommonInformationHelper
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CommonInformationHelper
  {
    public static string GetString(IDictionary<string, string> dictionary, string fieldName)
    {
      string str;
      return dictionary.TryGetValue(fieldName, out str) ? str : string.Empty;
    }

    public static bool GetBool(IDictionary<string, string> dictionary, string fieldName)
    {
      string str;
      bool result;
      return dictionary.TryGetValue(fieldName, out str) && bool.TryParse(str, out result) && result;
    }

    public static void SetBool(
      IDictionary<string, string> dictionary,
      string fieldName,
      bool value)
    {
      dictionary[fieldName] = value.ToString();
    }

    public static DateTime GetDateTime(IDictionary<string, string> dictionary, string fieldName)
    {
      string str;
      return !dictionary.TryGetValue(fieldName, out str) ? DateTime.MinValue : CommonInformationHelper.ToDateTime(str);
    }

    public static void SetDateTime(
      IDictionary<string, string> dictionary,
      string fieldName,
      DateTime value)
    {
      dictionary[fieldName] = CommonInformationHelper.ToString(value);
    }

    public static T GetEnum<T>(
      IDictionary<string, string> dictionary,
      string fieldName,
      T defaultValue)
    {
      string str;
      return !dictionary.TryGetValue(fieldName, out str) ? defaultValue : CommonInformationHelper.ToEnum<T>(str, defaultValue);
    }

    public static void SetEnum<T>(
      IDictionary<string, string> dictionary,
      string fieldName,
      T value)
    {
      dictionary[fieldName] = CommonInformationHelper.EnumToString<T>(value);
    }

    public static int GetInt(IDictionary<string, string> dictionary, string fieldName) => CommonInformationHelper.GetInt(dictionary, fieldName, 0);

    public static int GetInt(
      IDictionary<string, string> dictionary,
      string fieldName,
      int invalidValue)
    {
      string str;
      return !dictionary.TryGetValue(fieldName, out str) ? invalidValue : CommonInformationHelper.ToInt(str);
    }

    public static void SetInt(IDictionary<string, string> dictionary, string fieldName, int value) => dictionary[fieldName] = CommonInformationHelper.ToString(value);

    public static Uri GetUri(IDictionary<string, string> dictionary, string fieldName)
    {
      string uriString;
      return !dictionary.TryGetValue(fieldName, out uriString) || string.IsNullOrEmpty(uriString) ? (Uri) null : new Uri(uriString);
    }

    public static void SetUri(IDictionary<string, string> dictionary, string fieldName, Uri value)
    {
      if (value != (Uri) null)
        dictionary[fieldName] = value.AbsoluteUri;
      else
        dictionary[fieldName] = string.Empty;
    }

    public static Guid GetGuid(IDictionary<string, string> dictionary, string fieldName)
    {
      string g;
      return !dictionary.TryGetValue(fieldName, out g) ? Guid.Empty : new Guid(g);
    }

    public static void SetGuid(
      IDictionary<string, string> dictionary,
      string fieldName,
      Guid value)
    {
      dictionary[fieldName] = value.ToString("D");
    }

    public static DateTime ToDateTime(string value)
    {
      DateTime result;
      return DateTime.TryParse(value, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out result) ? result.ToLocalTime() : DateTime.MinValue;
    }

    public static T ToEnum<T>(string value, T defaultValue)
    {
      try
      {
        return (T) Enum.Parse(typeof (T), value);
      }
      catch (ArgumentException ex)
      {
        return defaultValue;
      }
    }

    public static int ToInt(string value) => CommonInformationHelper.ToInt(value, 0);

    public static int ToInt(string value, int invalidValue)
    {
      int result;
      return int.TryParse(value, out result) ? result : invalidValue;
    }

    public static string EnumToString<T>(T value) => Enum.GetName(typeof (T), (object) value);

    public static string ToString(DateTime value) => value.ToUniversalTime().ToString("o", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);

    public static string ToString(int value) => value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
  }
}
