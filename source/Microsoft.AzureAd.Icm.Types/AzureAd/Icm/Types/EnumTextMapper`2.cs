// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.EnumTextMapper`2
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.AzureAd.Icm.Types
{
  [SuppressMessage("Microsoft.Design", "CA1000", Justification = "The class is designed to have different sets of static data per concrete instance")]
  public static class EnumTextMapper<TMapper, TEnum>
    where TMapper : class
    where TEnum : struct
  {
    private static readonly SortedList<string, TEnum> Map = new SortedList<string, TEnum>();

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Complex initialization")]
    static EnumTextMapper()
    {
      Type underlyingType = Enum.GetUnderlyingType(typeof (TEnum));
      Type type = typeof (TMapper);
      Type enumType = typeof (TEnum);
      foreach (FieldInfo field in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
      {
        if (!(field.FieldType != typeof (string)) && field.IsLiteral && Attribute.IsDefined((MemberInfo) field, typeof (EnumValueAttribute)))
        {
          EnumValueAttribute customAttribute = (EnumValueAttribute) field.GetCustomAttributes(typeof (EnumValueAttribute), false)[0];
          object enumValue;
          if (underlyingType == typeof (ulong))
            enumValue = (object) customAttribute.EnumValue;
          else if (underlyingType == typeof (long))
            enumValue = (object) customAttribute.EnumValue;
          else if (underlyingType == typeof (uint))
            enumValue = (object) (uint) customAttribute.EnumValue;
          else if (underlyingType == typeof (int))
            enumValue = (object) (int) customAttribute.EnumValue;
          else
            continue;
          if (Enum.IsDefined(enumType, enumValue))
          {
            TEnum @enum = (TEnum) Enum.ToObject(enumType, enumValue);
            string upperInvariant = field.GetValue((object) null).ToString().ToUpperInvariant();
            EnumTextMapper<TMapper, TEnum>.Map.Add(upperInvariant, @enum);
          }
        }
      }
    }

    public static bool TryGetTextValue(TEnum value, out string result)
    {
      SortedList<string, TEnum> map = EnumTextMapper<TMapper, TEnum>.Map;
      int index = map.IndexOfValue(value);
      if (index >= 0)
      {
        result = map.Keys[index];
        return true;
      }
      result = (string) null;
      return false;
    }

    public static string GetTextValue(TEnum value)
    {
      string result = (string) null;
      if (!EnumTextMapper<TMapper, TEnum>.TryGetTextValue(value, out result))
        throw new DataConversionException(TypeUtility.Format("Unable to find string mapping to value {1}.{0}", (object) value, (object) typeof (TEnum).Name));
      return result;
    }

    public static bool TryGetEnumValue(string value, out TEnum result) => EnumTextMapper<TMapper, TEnum>.Map.TryGetValue(value.ToUpperInvariant(), out result);

    public static TEnum GetEnumValue(string value)
    {
      TEnum result;
      if (!EnumTextMapper<TMapper, TEnum>.TryGetEnumValue(value, out result))
        throw new DataConversionException(TypeUtility.Format("Unable to find matching value for string '{0}' in enum {1}", (object) value, (object) typeof (TEnum).Name));
      return result;
    }
  }
}
