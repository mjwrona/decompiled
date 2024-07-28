// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TypeUtility
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Globalization;
using System.Linq;

namespace Microsoft.AzureAd.Icm.Types
{
  public static class TypeUtility
  {
    public static bool IsDerivedFrom(Type type, Type @base)
    {
      if (type == (Type) null || @base == (Type) null)
        return false;
      if ((object) type == (object) @base)
        return true;
      if (@base.IsInterface)
      {
        foreach (Type type1 in type.GetInterfaces())
        {
          if (TypeUtility.IsDerivedFrom(type1, @base))
            return true;
        }
      }
      return TypeUtility.IsDerivedFrom(type.BaseType, @base);
    }

    public static string GenerateAdMetricsFriendlyName(string metricName) => IcmConstants.PerformanceMetrics.MetricNameForbiddenCharacters.Aggregate<char, string>(metricName, (Func<string, char, string>) ((str, cItem) => str.Replace(cItem, '_')));

    internal static string Format(string format, params object[] args) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);

    internal static bool IsNullEmptyOrWhitespace(string s) => string.IsNullOrEmpty(s) || s.Trim().Length == 0;

    internal static void ValidateStringParameter(
      string text,
      string parameterName,
      int minLength,
      int maxLength,
      bool allowNull)
    {
      if (text == null)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName, parameterName + "may not be null.");
      }
      else
        ArgumentCheck.ThrowIfNotInRangeInclusive<int>(text.Length, minLength, maxLength, parameterName + ".Length", nameof (ValidateStringParameter), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\TypeUtility.cs");
    }

    internal static void ValidateStringParameter(
      string text,
      string parameterName,
      int minLength,
      int maxLength)
    {
      TypeUtility.ValidateStringParameter(text, parameterName, minLength, maxLength, false);
    }
  }
}
