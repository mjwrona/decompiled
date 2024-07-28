// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Expression.WorkItem.ExpressionBuilderUtils
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Expression.WorkItem
{
  internal static class ExpressionBuilderUtils
  {
    public static ExpressionBuilderUtils.DataType GetResolvableDataTypesOf(string value)
    {
      ExpressionBuilderUtils.DataType resolvableDataTypesOf = ExpressionBuilderUtils.DataType.String;
      string result;
      if (ExpressionBuilderUtils.TryNormalizeDateTimeAsString(value, out result))
        resolvableDataTypesOf |= ExpressionBuilderUtils.DataType.DateTime;
      if (ExpressionBuilderUtils.TryNormalizeDoubleAsString(value, out result))
        resolvableDataTypesOf |= ExpressionBuilderUtils.DataType.Double;
      if (ExpressionBuilderUtils.TryNormalizeIntegerAsString(value, out result))
        resolvableDataTypesOf |= ExpressionBuilderUtils.DataType.Int32;
      return resolvableDataTypesOf;
    }

    public static bool TryNormalizeDateTimeAsString(string value, out string result)
    {
      DateTime result1;
      if (DateTime.TryParse(value, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result1))
      {
        result = result1.ToUniversalTime().ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      result = string.Empty;
      return false;
    }

    public static bool TryNormalizeIntegerAsString(string value, out string result)
    {
      int result1;
      if (int.TryParse(value, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
      {
        result = result1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      result = string.Empty;
      return false;
    }

    public static bool TryNormalizeDoubleAsString(string value, out string result)
    {
      double result1;
      if (double.TryParse(value, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
      {
        result = result1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      result = string.Empty;
      return false;
    }

    public static bool IsInfinityForFloat(string value)
    {
      double result;
      return !double.TryParse(value, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result) || float.IsInfinity(Convert.ToSingle(result));
    }

    [Flags]
    public enum DataType
    {
      DateTime = 1,
      Double = 2,
      Int32 = 4,
      String = 8,
    }
  }
}
