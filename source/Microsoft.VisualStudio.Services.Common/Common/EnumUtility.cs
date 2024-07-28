// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.EnumUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class EnumUtility
  {
    public static string ToString<T>(T value) where T : struct, Enum => EnumUtility.Implementation<T>.ToString(value);

    public static bool TryParse<T>(string s, out T result) where T : struct, Enum => EnumUtility.Implementation<T>.TryParse(s, false, out result);

    public static bool TryParse<T>(string s, bool ignoreCase, out T result) where T : struct, Enum => EnumUtility.Implementation<T>.TryParse(s, ignoreCase, out result);

    public static T Parse<T>(string s) where T : struct, Enum
    {
      T result;
      if (EnumUtility.TryParse<T>(s, out result))
        return result;
      throw new ArgumentException("Requested value '" + s + "' was not found.", nameof (s));
    }

    public static bool IsDefined<T>(T value) where T : struct, Enum => EnumUtility.Implementation<T>.IsDefined(value);

    public static T Parse<T>(string s, bool ignoreCase) where T : struct, Enum
    {
      T result;
      if (EnumUtility.TryParse<T>(s, ignoreCase, out result))
        return result;
      throw new ArgumentException("Requested value '" + s + "' was not found.", nameof (s));
    }

    private static class Implementation<T> where T : struct, Enum
    {
      private static Dictionary<string, T> s_toValueLookup;
      private static Dictionary<string, T> s_toValueLookupIgnoreCase;
      private static ConcurrentDictionary<string, T> s_dynamicToValueLookup;
      private static int s_dynamicToValueLookupCount;
      private static ConcurrentDictionary<string, T> s_dynamicToValueLookupIgnoreCase;
      private static int s_dynamicToValueLookupIgnoreCaseCount;
      private static Dictionary<T, string> s_toStringLookup;
      private static ConcurrentDictionary<T, string> s_dynamicToStringLookup;
      private static int s_dynamicToStringLookupCount;
      private const int c_dictionaryMaxSize = 50;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static string ToString(T value)
      {
        if (EnumUtility.Implementation<T>.s_toStringLookup == null)
          EnumUtility.Implementation<T>.FillLookups();
        string str;
        return EnumUtility.Implementation<T>.s_toStringLookup.TryGetValue(value, out str) ? str : EnumUtility.Implementation<T>.ToStringDynamic(value);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool TryParse(string s, bool ignoreCase, out T result)
      {
        if (s == null)
        {
          result = default (T);
          return false;
        }
        s = s.Trim();
        if (s.Length == 0)
        {
          result = default (T);
          return false;
        }
        if (EnumUtility.Implementation<T>.s_toValueLookup == null)
          EnumUtility.Implementation<T>.FillLookups();
        if (ignoreCase)
        {
          if (EnumUtility.Implementation<T>.s_toValueLookup.TryGetValue(s, out result) || EnumUtility.Implementation<T>.s_toValueLookupIgnoreCase.TryGetValue(s, out result))
            return true;
          ConcurrentDictionary<string, T> lookupIgnoreCase = EnumUtility.Implementation<T>.s_dynamicToValueLookupIgnoreCase;
          // ISSUE: explicit non-virtual call
          if ((lookupIgnoreCase != null ? (__nonvirtual (lookupIgnoreCase.TryGetValue(s, out result)) ? 1 : 0) : 0) != 0)
            return true;
          if (!Enum.TryParse<T>(s, ignoreCase, out result))
            return false;
          if (EnumUtility.Implementation<T>.s_dynamicToValueLookupIgnoreCase == null)
            EnumUtility.Implementation<T>.s_dynamicToValueLookupIgnoreCase = new ConcurrentDictionary<string, T>(2, 4, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          if (EnumUtility.Implementation<T>.s_dynamicToValueLookupIgnoreCase.TryAdd(s, result) && ++EnumUtility.Implementation<T>.s_dynamicToValueLookupIgnoreCaseCount > 50)
          {
            EnumUtility.Implementation<T>.s_dynamicToValueLookupIgnoreCase.Clear();
            EnumUtility.Implementation<T>.s_dynamicToValueLookupIgnoreCase.TryAdd(s, result);
            EnumUtility.Implementation<T>.s_dynamicToValueLookupIgnoreCaseCount = 1;
          }
          return true;
        }
        if (EnumUtility.Implementation<T>.s_toValueLookup.TryGetValue(s, out result))
          return true;
        ConcurrentDictionary<string, T> dynamicToValueLookup = EnumUtility.Implementation<T>.s_dynamicToValueLookup;
        // ISSUE: explicit non-virtual call
        if ((dynamicToValueLookup != null ? (__nonvirtual (dynamicToValueLookup.TryGetValue(s, out result)) ? 1 : 0) : 0) != 0)
          return true;
        if (!Enum.TryParse<T>(s, ignoreCase, out result))
          return false;
        if (EnumUtility.Implementation<T>.s_dynamicToValueLookup == null)
          EnumUtility.Implementation<T>.s_dynamicToValueLookup = new ConcurrentDictionary<string, T>(2, 4, (IEqualityComparer<string>) StringComparer.Ordinal);
        if (EnumUtility.Implementation<T>.s_dynamicToValueLookup.TryAdd(s, result) && ++EnumUtility.Implementation<T>.s_dynamicToValueLookupCount > 50)
        {
          EnumUtility.Implementation<T>.s_dynamicToValueLookup.Clear();
          EnumUtility.Implementation<T>.s_dynamicToValueLookup.TryAdd(s, result);
          EnumUtility.Implementation<T>.s_dynamicToValueLookupCount = 1;
        }
        return true;
      }

      public static bool IsDefined(T value)
      {
        if (EnumUtility.Implementation<T>.s_toStringLookup == null)
          EnumUtility.Implementation<T>.FillLookups();
        return EnumUtility.Implementation<T>.s_toStringLookup.ContainsKey(value);
      }

      private static string ToStringDynamic(T value)
      {
        if (EnumUtility.Implementation<T>.s_dynamicToStringLookup == null)
          EnumUtility.Implementation<T>.s_dynamicToStringLookup = new ConcurrentDictionary<T, string>();
        string stringDynamic;
        if (!EnumUtility.Implementation<T>.s_dynamicToStringLookup.TryGetValue(value, out stringDynamic))
        {
          stringDynamic = value.ToString();
          if (EnumUtility.Implementation<T>.s_dynamicToStringLookup.TryAdd(value, stringDynamic) && ++EnumUtility.Implementation<T>.s_dynamicToStringLookupCount > 50)
          {
            EnumUtility.Implementation<T>.s_dynamicToStringLookup.Clear();
            EnumUtility.Implementation<T>.s_dynamicToStringLookup.TryAdd(value, stringDynamic);
            EnumUtility.Implementation<T>.s_dynamicToStringLookupCount = 1;
          }
        }
        return stringDynamic;
      }

      private static void FillLookups()
      {
        Array values = Enum.GetValues(typeof (T));
        string[] names = Enum.GetNames(typeof (T));
        Dictionary<T, string> dictionary1 = new Dictionary<T, string>(values.Length);
        Dictionary<string, T> dictionary2 = new Dictionary<string, T>(values.Length, (IEqualityComparer<string>) StringComparer.Ordinal);
        Dictionary<string, T> dictionary3 = new Dictionary<string, T>(values.Length, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        for (int index = 0; index < values.Length; ++index)
        {
          T key = (T) values.GetValue(index);
          dictionary1[key] = names[index];
          dictionary2.Add(names[index], key);
          dictionary3[names[index]] = key;
        }
        EnumUtility.Implementation<T>.s_toStringLookup = dictionary1;
        EnumUtility.Implementation<T>.s_toValueLookup = dictionary2;
        EnumUtility.Implementation<T>.s_toValueLookupIgnoreCase = dictionary3;
      }
    }
  }
}
