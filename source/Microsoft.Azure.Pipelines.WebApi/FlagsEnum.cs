// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.FlagsEnum
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  public static class FlagsEnum
  {
    private static readonly char[] s_enumSeparatorCharArray = new char[1]
    {
      ','
    };

    public static TEnum ParseKnownFlags<TEnum>(string stringValue) where TEnum : Enum => (TEnum) FlagsEnum.ParseKnownFlags(typeof (TEnum), stringValue);

    public static object ParseKnownFlags(Type enumType, string stringValue)
    {
      ArgumentUtility.CheckForNull<Type>(enumType, nameof (enumType));
      if (!enumType.IsEnum)
        throw new ArgumentException(PipelinesWebApiResources.FlagEnumTypeRequired());
      if (stringValue == null)
        throw new ArgumentNullException(stringValue);
      if (string.IsNullOrWhiteSpace(stringValue))
        throw new ArgumentException(PipelinesWebApiResources.NonEmptyEnumElementsRequired((object) stringValue));
      if (ulong.TryParse(stringValue, NumberStyles.AllowLeadingSign, (IFormatProvider) CultureInfo.InvariantCulture, out ulong _))
        return Enum.Parse(enumType, stringValue);
      HashSet<string> hashSet = ((IEnumerable<string>) Enum.GetNames(enumType)).ToHashSet<string, string>((Func<string, string>) (name => name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Lazy<IDictionary<string, string>> lazy = new Lazy<IDictionary<string, string>>((Func<IDictionary<string, string>>) (() =>
      {
        IDictionary<string, string> knownFlags = (IDictionary<string, string>) null;
        foreach (FieldInfo field in enumType.GetFields())
        {
          if (((IEnumerable<object>) field.GetCustomAttributes(typeof (EnumMemberAttribute), false)).FirstOrDefault<object>() is EnumMemberAttribute enumMemberAttribute2)
          {
            if (knownFlags == null)
              knownFlags = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            knownFlags.Add(enumMemberAttribute2.Value, field.GetValue((object) null).ToString());
          }
        }
        return knownFlags;
      }));
      string[] strArray = stringValue.Split(FlagsEnum.s_enumSeparatorCharArray);
      List<string> stringList = new List<string>();
      for (int index = 0; index < strArray.Length; ++index)
      {
        string key = strArray[index].Trim();
        if (string.IsNullOrEmpty(key))
          throw new ArgumentException(PipelinesWebApiResources.NonEmptyEnumElementsRequired((object) stringValue));
        if (hashSet.Contains(key))
        {
          stringList.Add(key);
        }
        else
        {
          string str;
          if (lazy.Value != null && lazy.Value.TryGetValue(key, out str))
            stringList.Add(str);
        }
      }
      if (!stringList.Any<string>())
        return Enum.Parse(enumType, "0");
      string str1 = string.Join(", ", (IEnumerable<string>) stringList);
      return Enum.Parse(enumType, str1, true);
    }
  }
}
