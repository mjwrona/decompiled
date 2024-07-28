// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EnumHelper
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Edm
{
  public static class EnumHelper
  {
    private const int MaxHashElements = 100;
    private static readonly IDictionary<IEdmEnumType, EnumHelper.HashEntry> fieldInfoHash = (IDictionary<IEdmEnumType, EnumHelper.HashEntry>) new Dictionary<IEdmEnumType, EnumHelper.HashEntry>();

    public static bool TryParseEnum(
      this IEdmEnumType enumType,
      string value,
      bool ignoreCase,
      out long parseResult)
    {
      char[] chArray = new char[1]{ ',' };
      IEdmEnumType enumType1 = enumType;
      parseResult = 0L;
      if (value == null)
        return false;
      value = value.Trim();
      if (value.Length == 0)
        return false;
      ulong num1 = 0;
      string[] strArray = value.Split(chArray);
      if (!enumType.IsFlags && strArray.Length > 1)
        return false;
      ulong[] values;
      string[] names;
      enumType1.GetCachedValuesAndNames(out values, out names, true, true);
      if (char.IsDigit(value[0]) || value[0] == '-' || value[0] == '+')
      {
        ulong num2 = 0;
        for (int index = 0; index < values.Length; ++index)
          num2 |= values[index];
        for (int index = 0; index < strArray.Length; ++index)
        {
          long result;
          if (!long.TryParse(strArray[index], out result))
            return false;
          num1 |= (ulong) result;
        }
      }
      else
      {
        for (int index1 = 0; index1 < strArray.Length; ++index1)
        {
          strArray[index1] = strArray[index1].Trim();
          bool flag = false;
          for (int index2 = 0; index2 < names.Length; ++index2)
          {
            if (ignoreCase)
            {
              if (string.Compare(names[index2], strArray[index1], StringComparison.OrdinalIgnoreCase) != 0)
                continue;
            }
            else if (!names[index2].Equals(strArray[index1]))
              continue;
            ulong num3 = values[index2];
            num1 |= num3;
            flag = true;
            break;
          }
          if (!flag)
            return false;
        }
      }
      parseResult = (long) num1;
      return true;
    }

    public static string ToStringLiteral(this IEdmEnumTypeReference type, long value)
    {
      if (type == null || !(type.Definition is IEdmEnumType definition))
        return value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return !definition.IsFlags ? definition.ToStringNoFlags(value) : definition.ToStringWithFlags(value);
    }

    private static string ToStringWithFlags(this IEdmEnumType enumType, long value)
    {
      ulong num1 = (ulong) value;
      ulong[] values;
      string[] names;
      enumType.GetCachedValuesAndNames(out values, out names, true, true);
      int index = values.Length - 1;
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      ulong num2 = num1;
      for (; index >= 0 && (index != 0 || values[index] != 0UL); --index)
      {
        if (((long) num1 & (long) values[index]) == (long) values[index])
        {
          num1 -= values[index];
          if (!flag)
            stringBuilder.Insert(0, ", ");
          stringBuilder.Insert(0, names[index]);
          flag = false;
        }
      }
      if (num1 != 0UL)
        return value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (num2 != 0UL)
        return stringBuilder.ToString();
      return values.Length != 0 && values[0] == 0UL ? names[0] : 0.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    private static string ToStringNoFlags(this IEdmEnumType enumType, long value)
    {
      ulong[] values;
      string[] names;
      enumType.GetCachedValuesAndNames(out values, out names, true, true);
      ulong num = (ulong) value;
      int index = Array.BinarySearch<ulong>(values, num);
      return index < 0 ? value.ToString((IFormatProvider) CultureInfo.InvariantCulture) : names[index];
    }

    private static void GetCachedValuesAndNames(
      this IEdmEnumType enumType,
      out ulong[] values,
      out string[] names,
      bool getValues,
      bool getNames)
    {
      EnumHelper.HashEntry hashEntry = EnumHelper.GetHashEntry(enumType);
      values = hashEntry.Values;
      if (values != null)
        getValues = false;
      names = hashEntry.Names;
      if (names != null)
        getNames = false;
      if (!getValues && !getNames)
        return;
      EnumHelper.GetEnumValuesAndNames(enumType, ref values, ref names, getValues, getNames);
      if (getValues)
        hashEntry.Values = values;
      if (!getNames)
        return;
      hashEntry.Names = names;
    }

    private static void GetEnumValuesAndNames(
      IEdmEnumType enumType,
      ref ulong[] values,
      ref string[] names,
      bool getValues,
      bool getNames)
    {
      Dictionary<string, ulong> source = new Dictionary<string, ulong>();
      foreach (IEdmEnumMember member in enumType.Members)
      {
        IEdmEnumMemberValue edmEnumMemberValue = member.Value;
        if (edmEnumMemberValue != null)
          source.Add(member.Name, (ulong) edmEnumMemberValue.Value);
      }
      Dictionary<string, ulong> dictionary = source.OrderBy<KeyValuePair<string, ulong>, ulong>((Func<KeyValuePair<string, ulong>, ulong>) (d => d.Value)).ToDictionary<KeyValuePair<string, ulong>, string, ulong>((Func<KeyValuePair<string, ulong>, string>) (d => d.Key), (Func<KeyValuePair<string, ulong>, ulong>) (d => d.Value));
      values = dictionary.Select<KeyValuePair<string, ulong>, ulong>((Func<KeyValuePair<string, ulong>, ulong>) (d => d.Value)).ToArray<ulong>();
      names = dictionary.Select<KeyValuePair<string, ulong>, string>((Func<KeyValuePair<string, ulong>, string>) (d => d.Key)).ToArray<string>();
    }

    private static EnumHelper.HashEntry GetHashEntry(IEdmEnumType enumType)
    {
      if (EnumHelper.fieldInfoHash.Count > 100)
      {
        lock (EnumHelper.fieldInfoHash)
        {
          if (EnumHelper.fieldInfoHash.Count > 100)
            EnumHelper.fieldInfoHash.Clear();
        }
      }
      return EdmUtil.DictionaryGetOrUpdate<IEdmEnumType, EnumHelper.HashEntry>(EnumHelper.fieldInfoHash, enumType, (Func<IEdmEnumType, EnumHelper.HashEntry>) (type => new EnumHelper.HashEntry((string[]) null, (ulong[]) null)));
    }

    private class HashEntry
    {
      public string[] Names;
      public ulong[] Values;

      public HashEntry(string[] names, ulong[] values)
      {
        this.Names = names;
        this.Values = values;
      }
    }
  }
}
