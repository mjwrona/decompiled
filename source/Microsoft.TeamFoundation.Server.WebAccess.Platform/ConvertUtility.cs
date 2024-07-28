// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ConvertUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class ConvertUtility
  {
    public const char DefaultDelimiter = ';';

    public static T FromString<T>(string value) => ConvertUtility.FromString<T>(value, default (T));

    public static T FromString<T>(string value, T defaultValue)
    {
      if (value == null)
        return defaultValue;
      if (typeof (T).IsAssignableFrom(typeof (string)))
        return (T) value;
      if (value == string.Empty)
        return defaultValue;
      TypeConverter converter = TypeDescriptor.GetConverter(typeof (T));
      if (!converter.CanConvertFrom(typeof (string)))
        return defaultValue;
      try
      {
        return (T) converter.ConvertFromInvariantString(value);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ConvertUtility: Exception={0} CallerStack={1} ExceptionStack={2}", (object) ex.Message, (object) Environment.StackTrace, (object) ex.StackTrace), (object) ex);
        return defaultValue;
      }
    }

    public static string ToString(object value) => value == null ? (string) null : TypeDescriptor.GetConverter(value.GetType()).ConvertToInvariantString(value);

    public static string ToString<T>(T value) => TypeDescriptor.GetConverter(typeof (T)).ConvertToInvariantString((object) value);

    public static string StringJoin<T>(this IEnumerable<T> values) => values.StringJoin<T>(';');

    public static string StringJoin<T>(this IEnumerable<T> values, char delimiter)
    {
      List<string> stringList = new List<string>();
      foreach (T obj in values)
        stringList.Add(ConvertUtility.QuoteString(ConvertUtility.ToString<T>(obj), delimiter));
      return string.Join(delimiter.ToString(), stringList.ToArray());
    }

    public static IEnumerable<T> Split<T>(this string value) => value.Split<T>(';', StringSplitOptions.None);

    public static IEnumerable<T> Split<T>(this string value, char delimiter) => value.Split<T>(delimiter, StringSplitOptions.None);

    public static IEnumerable<T> Split<T>(
      this string value,
      char delimiter,
      StringSplitOptions options)
    {
      if (!string.IsNullOrEmpty(value))
      {
        StringBuilder temp = (StringBuilder) null;
        int startIndex = 0;
        int num = 0;
        bool skip = false;
        for (int i = 0; i < value.Length; ++i)
        {
          if (skip)
          {
            skip = false;
            ++num;
          }
          else
          {
            char ch = value[i];
            if (ch == '\\')
            {
              if (temp == null)
                temp = new StringBuilder(64 + num);
              if (num > 0)
                temp.Append(value, startIndex, num);
              skip = true;
              startIndex = i + 1;
              num = 0;
            }
            else if ((int) ch == (int) delimiter)
            {
              if (num > 0)
              {
                if (temp == null)
                {
                  yield return ConvertUtility.FromString<T>(value.Substring(startIndex, num));
                }
                else
                {
                  temp.Append(value, startIndex, num);
                  yield return ConvertUtility.FromString<T>(temp.ToString());
                  temp = (StringBuilder) null;
                }
              }
              else if ((options & StringSplitOptions.RemoveEmptyEntries) == StringSplitOptions.None)
                yield return ConvertUtility.FromString<T>(string.Empty);
              startIndex = i + 1;
              num = 0;
            }
            else
              ++num;
          }
        }
        if (num > 0)
        {
          if (temp == null)
          {
            yield return ConvertUtility.FromString<T>(value.Substring(startIndex, num));
          }
          else
          {
            temp.Append(value, startIndex, num);
            yield return ConvertUtility.FromString<T>(temp.ToString());
          }
        }
        else if ((options & StringSplitOptions.RemoveEmptyEntries) == StringSplitOptions.None)
          yield return ConvertUtility.FromString<T>(string.Empty);
      }
    }

    internal static string QuoteString(string value, char delimiter)
    {
      if (string.IsNullOrEmpty(value))
        return string.Empty;
      StringBuilder stringBuilder = (StringBuilder) null;
      int startIndex = 0;
      int count = 0;
      for (int index = 0; index < value.Length; ++index)
      {
        char ch = value[index];
        if (ch == '\\' || (int) ch == (int) delimiter)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder(value.Length + 5);
          if (count > 0)
            stringBuilder.Append(value, startIndex, count);
          stringBuilder.Append('\\');
          stringBuilder.Append(ch);
          startIndex = index + 1;
          count = 0;
        }
        else
          ++count;
      }
      if (stringBuilder == null)
        return value;
      if (count > 0)
        stringBuilder.Append(value, startIndex, count);
      return stringBuilder.ToString();
    }

    public static T ParseEnumDefault<T>(this string value) => value.ParseEnumDefault<T>(true, default (T));

    public static T ParseEnumDefault<T>(this string value, T defaultValue) => value.ParseEnumDefault<T>(true, defaultValue);

    public static T ParseEnumDefault<T>(this string value, bool ignoreCase, T defaultValue)
    {
      T enumDefault = defaultValue;
      if (!string.IsNullOrEmpty(value))
      {
        try
        {
          enumDefault = value.ParseEnum<T>(ignoreCase);
        }
        catch (ArgumentException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (ParseEnumDefault), (Exception) ex);
        }
        catch (OverflowException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (ParseEnumDefault), (Exception) ex);
        }
      }
      return enumDefault;
    }

    public static T ParseEnum<T>(this string value) => value.ParseEnum<T>(true);

    public static T ParseEnum<T>(this string value, bool ignoreCase) => (T) Enum.Parse(typeof (T), value, ignoreCase);

    public static bool TryParseEnum<T>(this string value, out T result) => value.TryParseEnum<T>(true, out result);

    public static bool TryParseEnum<T>(this string value, bool ignoreCase, out T result)
    {
      result = default (T);
      if (!string.IsNullOrEmpty(value))
      {
        try
        {
          result = value.ParseEnum<T>(ignoreCase);
          return true;
        }
        catch (ArgumentException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (TryParseEnum), (Exception) ex);
        }
        catch (OverflowException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (TryParseEnum), (Exception) ex);
        }
      }
      return false;
    }

    public static string ToXml<T>(T data) => ConvertUtility.ToXml(typeof (T), (object) data);

    public static string ToXml(Type type, object data)
    {
      if (data == null)
        return "";
      using (StringWriter stringWriter = new StringWriter())
      {
        new XmlSerializer(type).Serialize((TextWriter) stringWriter, data);
        return stringWriter.ToString();
      }
    }

    public static T FromXml<T>(string value)
    {
      if (value == null)
        return default (T);
      using (StringReader stringReader = new StringReader(value))
        return (T) new XmlSerializer(typeof (T)).Deserialize((TextReader) stringReader);
    }

    public static string DataContractJson<T>(T data) => ConvertUtility.DataContractJson(typeof (T), (object) data);

    public static string DataContractJson(Type type, object data, bool useSimpleDictionaryFormat = false)
    {
      if (data == null)
        return "null";
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(type, new DataContractJsonSerializerSettings()
        {
          UseSimpleDictionaryFormat = useSimpleDictionaryFormat
        }).WriteObject((Stream) memoryStream, data);
        return Encoding.UTF8.GetString(memoryStream.ToArray()).ToString();
      }
    }

    public static T FromDataContractJson<T>(string value)
    {
      if (value == null)
        return default (T);
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
        return (T) new DataContractJsonSerializer(typeof (T)).ReadObject((Stream) memoryStream);
    }

    public static object ConvertTo(object value, Type destinationType, CultureInfo culture)
    {
      if (value == null || destinationType.IsInstanceOfType(value))
        return value;
      TypeConverter converter1 = TypeDescriptor.GetConverter(destinationType);
      if (converter1.CanConvertFrom(value.GetType()))
        return converter1.ConvertFrom((ITypeDescriptorContext) null, culture, value);
      TypeConverter converter2 = TypeDescriptor.GetConverter(value.GetType());
      if (converter2.CanConvertTo(destinationType))
        return converter2.ConvertTo((ITypeDescriptorContext) null, culture, value, destinationType);
      throw new InvalidOperationException("No converters exist.".FormatCurrent((object) value.GetType().FullName, (object) destinationType.FullName));
    }

    public static IEnumerable<object> ToCompactJsArray(int[] ids)
    {
      int num1 = ids != null ? ids.Length : throw new ArgumentNullException(nameof (ids));
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int index1 = 0;
      List<object> compactJsArray = new List<object>();
      if (num1 > 0)
      {
        num2 = ids[index1];
        compactJsArray.Add((object) num2);
        num3 = num2;
      }
      for (int index2 = index1 + 1; index2 < num1; ++index2)
      {
        int id = ids[index2];
        int num5 = id - num2;
        num2 = id;
        if (num3 == num5)
        {
          ++num4;
        }
        else
        {
          if (num4 > 1)
          {
            compactJsArray.Add((object) "r");
            compactJsArray.Add((object) num4);
          }
          else if (num4 > 0)
            compactJsArray.Add((object) num3);
          num4 = 0;
          compactJsArray.Add((object) num5);
        }
        num3 = num5;
      }
      if (num4 > 0)
      {
        compactJsArray.Add((object) "r");
        compactJsArray.Add((object) num4);
      }
      return (IEnumerable<object>) compactJsArray;
    }
  }
}
