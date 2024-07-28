// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.Tags
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  internal static class Tags
  {
    internal static bool? GetTagBoolValueOrNull(
      this IDictionary<string, string> tags,
      string tagKey)
    {
      string tagValueOrNull = tags.GetTagValueOrNull(tagKey);
      return string.IsNullOrEmpty(tagValueOrNull) ? new bool?() : new bool?(bool.Parse(tagValueOrNull));
    }

    internal static int? GetTagIntValueOrNull(this IDictionary<string, string> tags, string tagKey)
    {
      string tagValueOrNull = tags.GetTagValueOrNull(tagKey);
      return string.IsNullOrEmpty(tagValueOrNull) ? new int?() : new int?(int.Parse(tagValueOrNull, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    internal static DateTimeOffset? GetTagDateTimeOffsetValueOrNull(
      this IDictionary<string, string> tags,
      string tagKey)
    {
      string tagValueOrNull = tags.GetTagValueOrNull(tagKey);
      return string.IsNullOrEmpty(tagValueOrNull) ? new DateTimeOffset?() : new DateTimeOffset?(DateTimeOffset.Parse(tagValueOrNull, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
    }

    internal static void SetStringValueOrRemove(
      this IDictionary<string, string> tags,
      string tagKey,
      string tagValue)
    {
      tags.SetTagValueOrRemove(tagKey, tagValue);
    }

    internal static void SetDateTimeOffsetValueOrRemove(
      this IDictionary<string, string> tags,
      string tagKey,
      DateTimeOffset? tagValue)
    {
      if (!tagValue.HasValue)
      {
        tags.SetTagValueOrRemove<DateTimeOffset?>(tagKey, tagValue);
      }
      else
      {
        string tagValue1 = tagValue.Value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
        tags.SetTagValueOrRemove(tagKey, tagValue1);
      }
    }

    internal static void SetTagValueOrRemove<T>(
      this IDictionary<string, string> tags,
      string tagKey,
      T tagValue)
    {
      tags.SetTagValueOrRemove(tagKey, Convert.ToString((object) tagValue, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    internal static void InitializeTagValue<T>(
      this IDictionary<string, string> tags,
      string tagKey,
      T tagValue)
    {
      if (tags.ContainsKey(tagKey))
        return;
      tags.SetTagValueOrRemove<T>(tagKey, tagValue);
    }

    internal static void InitializeTagDateTimeOffsetValue(
      this IDictionary<string, string> tags,
      string tagKey,
      DateTimeOffset? tagValue)
    {
      if (tags.ContainsKey(tagKey))
        return;
      tags.SetDateTimeOffsetValueOrRemove(tagKey, tagValue);
    }

    internal static string GetTagValueOrNull(this IDictionary<string, string> tags, string tagKey)
    {
      string str;
      return tags.TryGetValue(tagKey, out str) ? str : (string) null;
    }

    private static void SetTagValueOrRemove(
      this IDictionary<string, string> tags,
      string tagKey,
      string tagValue)
    {
      if (string.IsNullOrEmpty(tagValue))
        tags.Remove(tagKey);
      else
        tags[tagKey] = tagValue;
    }
  }
}
