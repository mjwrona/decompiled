// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.DictionaryExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal static class DictionaryExtensions
  {
    internal static bool EqualsTo(
      this IDictionary<string, JToken> dict1,
      IDictionary<string, JToken> dict2)
    {
      if (dict1 == null && dict2 == null)
        return true;
      if (dict1 == null || dict2 == null || dict1.Count != dict2.Count)
        return false;
      foreach (KeyValuePair<string, JToken> keyValuePair in (IEnumerable<KeyValuePair<string, JToken>>) dict1)
      {
        JToken t1;
        if (!dict2.TryGetValue(keyValuePair.Key, out t1) || !JToken.DeepEquals(t1, keyValuePair.Value))
          return false;
      }
      return true;
    }

    internal static bool EqualsTo<U, T>(this IDictionary<U, T> dict1, IDictionary<U, T> dict2)
    {
      if (dict1 == null && dict2 == null)
        return true;
      if (dict1 == null || dict2 == null || dict1.Count != dict2.Count)
        return false;
      foreach (KeyValuePair<U, T> keyValuePair in (IEnumerable<KeyValuePair<U, T>>) dict1)
      {
        T obj;
        if (!dict2.TryGetValue(keyValuePair.Key, out obj) || !keyValuePair.Value.Equals((object) obj))
          return false;
      }
      return true;
    }
  }
}
