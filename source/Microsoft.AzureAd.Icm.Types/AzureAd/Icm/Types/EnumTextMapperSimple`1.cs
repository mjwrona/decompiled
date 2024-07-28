// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.EnumTextMapperSimple`1
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AzureAd.Icm.Types
{
  [SuppressMessage("Microsoft.Design", "CA1000", Justification = "The class is designed to have different sets of static data per concrete instance")]
  public static class EnumTextMapperSimple<TEnum>
  {
    public static bool TryGetTextValue(TEnum value, out string result)
    {
      if (Enum.IsDefined(typeof (TEnum), (object) value))
      {
        result = value.ToString().ToUpperInvariant();
        return true;
      }
      result = (string) null;
      return false;
    }

    public static string GetTextValue(TEnum value)
    {
      string result = (string) null;
      if (!EnumTextMapperSimple<TEnum>.TryGetTextValue(value, out result))
        throw new DataConversionException(TypeUtility.Format("Unable to find string mapping to value {1}.{0}", (object) value, (object) typeof (TEnum).Name));
      return result;
    }

    public static bool TryGetEnumValue(string value, out TEnum result)
    {
      try
      {
        result = (TEnum) Enum.Parse(typeof (TEnum), value, true);
        return true;
      }
      catch (ArgumentException ex)
      {
        result = default (TEnum);
        return false;
      }
    }

    public static TEnum GetEnumValue(string value)
    {
      TEnum result;
      if (!EnumTextMapperSimple<TEnum>.TryGetEnumValue(value, out result))
        throw new DataConversionException(TypeUtility.Format("Unable to find matching value for string [{0}] in enum {1}", (object) value, (object) typeof (TEnum).Name));
      return result;
    }
  }
}
