// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.RegistryUtility
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class RegistryUtility
  {
    private static char[] s_singleSlash = new char[1]{ '/' };
    private static string[] s_depthOneSegments = new string[1]
    {
      "/*/"
    };
    private static string[] s_fullDepthSegments = new string[2]
    {
      "/**/",
      "/.../"
    };

    public static string ToString<T>(T value)
    {
      TypeConverter converter = TypeDescriptor.GetConverter(typeof (T));
      return converter.CanConvertFrom(typeof (string)) ? converter.ConvertToInvariantString((object) value) : throw new ArgumentException(TFCommonResources.InvalidRegistryValue((object) typeof (T).FullName, (object) typeof (string).FullName));
    }

    public static T FromString<T>(string value)
    {
      if (string.IsNullOrEmpty(value))
        return default (T);
      if (typeof (T).IsAssignableFrom(typeof (string)))
        return (T) value;
      if (typeof (T) == typeof (bool))
        return (T) (ValueType) bool.Parse(value);
      if (!(typeof (T) == typeof (int)))
        return (T) TypeDescriptor.GetConverter(typeof (T)).ConvertFromInvariantString(value);
      value = value.Trim();
      if (value.Length > 0 && value[0] == '#')
        return (T) (ValueType) Convert.ToInt32(value.Substring(1), 16);
      return value.Length >= 2 && (value[0] == '0' && (value[1] == 'x' || value[1] == 'X') || value[0] == '&' && (value[1] == 'h' || value[1] == 'H')) ? (T) (ValueType) Convert.ToInt32(value.Substring(2), 16) : (T) (ValueType) int.Parse(value, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static T FromString<T>(string value, T defaultValue)
    {
      if (value == null)
        return defaultValue;
      if (typeof (T).IsAssignableFrom(typeof (string)))
        return (T) value;
      if (typeof (T) == typeof (bool))
      {
        bool result;
        return bool.TryParse(value, out result) ? (T) (ValueType) result : defaultValue;
      }
      if (typeof (T) == typeof (int))
      {
        value = value.Trim();
        int result1;
        int result2;
        int result3;
        return value.Length > 0 && value[0] == '#' ? (int.TryParse(value.Substring(1), NumberStyles.HexNumber, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1) ? (T) (ValueType) result1 : defaultValue) : (value.Length >= 2 && (value[0] == '0' && (value[1] == 'x' || value[1] == 'X') || value[0] == '&' && (value[1] == 'h' || value[1] == 'H')) ? (int.TryParse(value.Substring(2), NumberStyles.HexNumber, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result2) ? (T) (ValueType) result2 : defaultValue) : (int.TryParse(value, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result3) ? (T) (ValueType) result3 : defaultValue));
      }
      TypeConverter converter = TypeDescriptor.GetConverter(typeof (T));
      if (!converter.CanConvertFrom(typeof (string)))
        return defaultValue;
      try
      {
        return (T) converter.ConvertFromInvariantString(value);
      }
      catch (Exception ex)
      {
        return defaultValue;
      }
    }

    public static string GetKeyName(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      int num = path.LastIndexOf('/', path.Length - 2, path.Length - 2);
      if (num < 0)
        return string.Empty;
      return path.EndsWith("/", StringComparison.Ordinal) ? path.Substring(num + 1, path.Length - num - 2) : path.Substring(num + 1, path.Length - num - 1);
    }

    public static string ToAbsolute(string rootPath, string path)
    {
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (string.IsNullOrEmpty(path))
        return rootPath;
      if (!path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
        path = !rootPath.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) rootPath, (object) path) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) rootPath, (object) path);
      return path;
    }

    public static void Parse(
      string pathPattern,
      out string path,
      out string pattern,
      out int depth)
    {
      depth = 0;
      path = (string) null;
      pattern = (string) null;
      string[] strArray = pathPattern.Split(RegistryUtility.s_singleSlash, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
      {
        depth = 0;
        path = "/";
        pattern = string.Empty;
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder("/");
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (index == strArray.Length - 1)
          {
            if (VssStringComparer.RegistryPath.Equals(strArray[index], "**") || VssStringComparer.RegistryPath.Equals(strArray[index], "..."))
            {
              depth = int.MaxValue;
              pattern = "*";
              break;
            }
            if (VssStringComparer.RegistryPath.Contains(strArray[index], "*"))
            {
              depth = 1;
              pattern = strArray[index];
              break;
            }
            pattern = strArray[index];
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) strArray[index]);
          }
          else
          {
            if (index == strArray.Length - 2 && (VssStringComparer.RegistryPath.Equals(strArray[index], "**") || VssStringComparer.RegistryPath.Equals(strArray[index], "...")))
            {
              depth = int.MaxValue;
              pattern = strArray[index + 1];
              break;
            }
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}/", (object) strArray[index]);
          }
        }
        if (stringBuilder[stringBuilder.Length - 1] == '/' && stringBuilder.Length > 1)
          --stringBuilder.Length;
        path = stringBuilder.ToString();
      }
    }
  }
}
