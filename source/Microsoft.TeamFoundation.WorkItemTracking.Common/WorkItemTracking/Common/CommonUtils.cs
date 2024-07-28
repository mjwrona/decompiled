// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.CommonUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  public static class CommonUtils
  {
    public static bool TryGetValue<TKey, TValue>(
      this IDictionary<TKey, object> dict,
      TKey key,
      out TValue value)
    {
      object obj;
      if (dict.TryGetValue(key, out obj))
      {
        value = (TValue) obj;
        return true;
      }
      value = default (TValue);
      return false;
    }

    public static bool TryGetValue<TKey, TValue>(
      this Dictionary<TKey, object> dict,
      TKey key,
      out TValue value)
    {
      return dict.TryGetValue<TKey, TValue>(key, out value);
    }

    public static IEnumerable<IEnumerable<T>> Page<T>(this IEnumerable<T> source, int count)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(source, nameof (source));
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 1);
      int count1 = 0;
      T[] result = new T[count];
      foreach (T obj in source)
      {
        result[count1++] = obj;
        if (count1 == count)
        {
          yield return (IEnumerable<T>) result;
          count1 = 0;
        }
      }
      if (count1 > 0)
        yield return ((IEnumerable<T>) result).Take<T>(count1);
    }

    public static int CombineHashCodes(int hashA, int hashB) => (hashA << 5) + hashA ^ hashB;

    public static string GetDisplayNameFromDistinctDisplayName(string distinctDisplayName)
    {
      if (distinctDisplayName == null)
        return (string) null;
      string distinctDisplayName1 = distinctDisplayName.Trim();
      int startIndex = 0;
      int length = distinctDisplayName.IndexOf(" <");
      if (length != -1)
        distinctDisplayName1 = distinctDisplayName1.Substring(startIndex, length);
      return distinctDisplayName1;
    }

    public static string DistinctDisplayName(IdentityRef identity) => !string.IsNullOrEmpty(identity.UniqueName) ? identity.DisplayName + " <" + identity.UniqueName + ">" : identity.DisplayName;
  }
}
