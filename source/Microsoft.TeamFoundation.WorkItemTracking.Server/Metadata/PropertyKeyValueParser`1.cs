// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.PropertyKeyValueParser`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal abstract class PropertyKeyValueParser<T>
  {
    private const string WildCharReplacementString = "\n";

    public Dictionary<string, T> ParseProperty(string propertyValue) => !string.IsNullOrEmpty(propertyValue) ? this.ExtractKeyValueDictionaryFromStringPair(this.GetStringPairs(propertyValue)) : new Dictionary<string, T>();

    public string CreatePropertyValue(IDictionary<string, T> keyValuePairs)
    {
      List<string> values = new List<string>();
      foreach (KeyValuePair<string, T> keyValuePair in (IEnumerable<KeyValuePair<string, T>>) keyValuePairs.OrderBy<KeyValuePair<string, T>, string>((Func<KeyValuePair<string, T>, string>) (s => s.Key)))
      {
        string str = keyValuePair.Key.Replace(",", ",,");
        values.Add(string.Format("{0}{1}{2}", (object) str, (object) "=", (object) keyValuePair.Value));
      }
      return string.Join(",", (IEnumerable<string>) values);
    }

    protected abstract IEqualityComparer<string> KeyComparer { get; }

    protected abstract string ParsePropertyKey(
      string key,
      string currentPair,
      Func<string, bool> isKeyDuplicated);

    protected abstract T ParsePropertyValue(string value, string currentPair);

    private string[] GetStringPairs(string propertyValue)
    {
      string[] stringPairs;
      if (propertyValue.IndexOf(",,", StringComparison.OrdinalIgnoreCase) == -1)
      {
        stringPairs = propertyValue.Split(new string[1]
        {
          ","
        }, StringSplitOptions.RemoveEmptyEntries);
      }
      else
      {
        propertyValue = PropertyKeyValueParser<T>.ReverseString(propertyValue);
        propertyValue = propertyValue.Replace(",,", "\n");
        propertyValue = PropertyKeyValueParser<T>.ReverseString(propertyValue);
        stringPairs = ((IEnumerable<string>) propertyValue.Split(new string[1]
        {
          ","
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (val => val.Replace("\n", ","))).ToArray<string>();
      }
      return stringPairs;
    }

    private Dictionary<string, T> ExtractKeyValueDictionaryFromStringPair(
      string[] splittedStringPairs)
    {
      Dictionary<string, T> keyValuePairs = new Dictionary<string, T>(this.KeyComparer);
      foreach (string splittedStringPair in splittedStringPairs)
      {
        string currentPair = splittedStringPair.Trim();
        if (currentPair.Length != 0)
        {
          int length = currentPair.LastIndexOf("=", StringComparison.OrdinalIgnoreCase);
          if (length == -1)
            throw new IndexOutOfRangeException(ServerResources.Validation_ProcessProperty_KeyValueDelimeter_NotFound((object) currentPair));
          string propertyKey = this.ParsePropertyKey(currentPair.Substring(0, length).Trim(), currentPair, (Func<string, bool>) (k => keyValuePairs.ContainsKey(k)));
          T propertyValue = this.ParsePropertyValue(currentPair.Substring(length + 1).Trim(), currentPair);
          keyValuePairs.Add(propertyKey, propertyValue);
        }
      }
      return keyValuePairs;
    }

    private static string ReverseString(string input)
    {
      char[] charArray = input.ToCharArray();
      Array.Reverse((Array) charArray);
      return new string(charArray);
    }
  }
}
