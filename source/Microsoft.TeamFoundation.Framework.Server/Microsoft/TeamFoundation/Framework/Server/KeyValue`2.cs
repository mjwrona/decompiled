// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KeyValue`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class KeyValue<TKeyType, TValueType>
  {
    public KeyValue()
    {
    }

    public KeyValue(TKeyType key, TValueType value)
    {
      this.Key = key;
      this.Value = value;
    }

    public KeyValue(KeyValuePair<TKeyType, TValueType> keyValuePair)
    {
      this.Key = keyValuePair.Key;
      this.Value = keyValuePair.Value;
    }

    internal static List<KeyValuePair<TKeyType, TValueType>> Convert(
      IEnumerable<KeyValue<TKeyType, TValueType>> keyValues)
    {
      List<KeyValuePair<TKeyType, TValueType>> keyValuePairList = new List<KeyValuePair<TKeyType, TValueType>>();
      if (keyValues != null)
      {
        foreach (KeyValue<TKeyType, TValueType> keyValue in keyValues)
          keyValuePairList.Add(new KeyValuePair<TKeyType, TValueType>(keyValue.Key, keyValue.Value));
      }
      return keyValuePairList;
    }

    internal static List<KeyValue<TKeyType, TValueType>> Convert(
      IEnumerable<KeyValuePair<TKeyType, TValueType>> keyValuePairs)
    {
      List<KeyValue<TKeyType, TValueType>> keyValueList = new List<KeyValue<TKeyType, TValueType>>();
      if (keyValuePairs != null)
      {
        foreach (KeyValuePair<TKeyType, TValueType> keyValuePair in keyValuePairs)
          keyValueList.Add(new KeyValue<TKeyType, TValueType>(keyValuePair));
      }
      return keyValueList;
    }

    internal static KeyValue<TKeyType, TValueType>[] ConvertToArray(
      ICollection<KeyValuePair<TKeyType, TValueType>> keyValuePairs)
    {
      KeyValue<TKeyType, TValueType>[] array = new KeyValue<TKeyType, TValueType>[keyValuePairs.Count];
      int num = 0;
      foreach (KeyValuePair<TKeyType, TValueType> keyValuePair in (IEnumerable<KeyValuePair<TKeyType, TValueType>>) keyValuePairs)
        array[num++] = new KeyValue<TKeyType, TValueType>(keyValuePair);
      return array;
    }

    internal static Dictionary<TKeyType, TValueType> ConvertToDictionary(
      IEnumerable<KeyValue<TKeyType, TValueType>> keyValues,
      IEqualityComparer<TKeyType> comparer)
    {
      Dictionary<TKeyType, TValueType> dictionary = new Dictionary<TKeyType, TValueType>(comparer);
      if (keyValues != null)
      {
        foreach (KeyValue<TKeyType, TValueType> keyValue in keyValues)
          dictionary.Add(keyValue.Key, keyValue.Value);
      }
      return dictionary;
    }

    internal static Dictionary<TKeyType, TValueType> ConvertToDictionary(
      IEnumerable<KeyValue<TKeyType, TValueType>> keyValues)
    {
      Dictionary<TKeyType, TValueType> dictionary = new Dictionary<TKeyType, TValueType>();
      if (keyValues != null)
      {
        foreach (KeyValue<TKeyType, TValueType> keyValue in keyValues)
          dictionary.Add(keyValue.Key, keyValue.Value);
      }
      return dictionary;
    }

    [ClientProperty(ClientVisibility.ProductInternal, ClientVisibility.ProductInternal)]
    public TKeyType Key { get; set; }

    [ClientProperty(ClientVisibility.ProductInternal, ClientVisibility.ProductInternal)]
    public TValueType Value { get; set; }
  }
}
