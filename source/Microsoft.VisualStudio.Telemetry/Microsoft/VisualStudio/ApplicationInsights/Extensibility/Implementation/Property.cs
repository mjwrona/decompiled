// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Property
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal static class Property
  {
    public const int MaxDictionaryNameLength = 150;
    public const int MaxValueLength = 65536;
    public const int MaxNameLength = 1024;
    public const int MaxMessageLength = 32768;
    public const int MaxUrlLength = 2048;
    private const RegexOptions SanitizeOptions = RegexOptions.Compiled;
    private static readonly Regex InvalidNameCharacters = new Regex("[^0-9a-zA-Z-._()\\/ ]", RegexOptions.Compiled);

    public static void Set<T>(ref T property, T value) where T : class => property = (object) value != null ? value : throw new ArgumentNullException(nameof (value));

    public static void Initialize<T>(ref T? property, T? value) where T : struct
    {
      if (property.HasValue)
        return;
      property = value;
    }

    public static void Initialize(ref string property, string value)
    {
      if (!string.IsNullOrEmpty(property))
        return;
      property = value;
    }

    public static string SanitizeName(this string name) => Property.TrimAndTruncate(name, 1024);

    public static string SanitizeValue(this string value) => Property.TrimAndTruncate(value, 65536);

    public static string SanitizeMessage(this string message) => Property.TrimAndTruncate(message, 32768);

    public static Uri SanitizeUri(this Uri uri)
    {
      if (uri != (Uri) null)
      {
        string str = uri.ToString();
        Uri result;
        if (str.Length > 2048 && Uri.TryCreate(str.Substring(0, 2048), UriKind.RelativeOrAbsolute, out result))
          uri = result;
      }
      return uri;
    }

    public static void SanitizeProperties(this IDictionary<string, string> dictionary)
    {
      if (dictionary == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in dictionary.ToArray<KeyValuePair<string, string>>())
      {
        dictionary.Remove(keyValuePair.Key);
        string key = Property.SanitizeKey<string>(keyValuePair.Key, dictionary);
        string str = keyValuePair.Value.SanitizeValue();
        dictionary.Add(key, str);
      }
    }

    public static void SanitizeMeasurements(this IDictionary<string, double> dictionary)
    {
      if (dictionary == null)
        return;
      foreach (KeyValuePair<string, double> keyValuePair in dictionary.ToArray<KeyValuePair<string, double>>())
      {
        dictionary.Remove(keyValuePair.Key);
        string key = Property.SanitizeKey<double>(keyValuePair.Key, dictionary);
        dictionary.Add(key, keyValuePair.Value);
      }
    }

    private static string TrimAndTruncate(string value, int maxLength)
    {
      if (value != null)
      {
        value = value.Trim();
        value = Property.Truncate(value, maxLength);
      }
      return value;
    }

    private static string Truncate(string value, int maxLength) => value.Length <= maxLength ? value : value.Substring(0, maxLength);

    private static string SanitizeKey<TValue>(string key, IDictionary<string, TValue> dictionary)
    {
      string input = Property.TrimAndTruncate(key, 150);
      return Property.MakeKeyUnique<TValue>(Property.MakeKeyNonEmpty(Property.InvalidNameCharacters.Replace(input, "_")), dictionary);
    }

    private static string MakeKeyNonEmpty(string key) => !string.IsNullOrEmpty(key) ? key : "(required property name is empty)";

    private static string MakeKeyUnique<TValue>(string key, IDictionary<string, TValue> dictionary)
    {
      if (dictionary.ContainsKey(key))
      {
        string str = Property.Truncate(key, 147);
        int num = 1;
        do
        {
          key = str + num.ToString((IFormatProvider) CultureInfo.InvariantCulture).PadLeft(3, '0');
          ++num;
        }
        while (dictionary.ContainsKey(key));
      }
      return key;
    }
  }
}
