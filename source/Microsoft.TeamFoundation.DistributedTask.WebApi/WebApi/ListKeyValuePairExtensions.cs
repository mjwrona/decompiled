// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ListKeyValuePairExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal static class ListKeyValuePairExtensions
  {
    public static void AddIfNotEmpty<TValue>(
      this IList<KeyValuePair<string, string>> queryParameters,
      string parameterName,
      IEnumerable<TValue> values)
    {
      if (values == null || !values.Any<TValue>())
        return;
      queryParameters.Add(new KeyValuePair<string, string>(parameterName, string.Join<TValue>(",", values)));
    }

    public static void AddIfNotEmpty(
      this IList<KeyValuePair<string, string>> queryParameters,
      string parameterName,
      string value)
    {
      if (string.IsNullOrEmpty(value))
        return;
      queryParameters.Add(new KeyValuePair<string, string>(parameterName, value));
    }

    public static void AddIfNotZero(
      this IList<KeyValuePair<string, string>> queryParameters,
      string parameterName,
      int value)
    {
      if (value == 0)
        return;
      queryParameters.Add(new KeyValuePair<string, string>(parameterName, value.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
    }

    public static void AddIfTrue(
      this IList<KeyValuePair<string, string>> queryParameters,
      string parameterName,
      bool value)
    {
      if (!value)
        return;
      queryParameters.Add(new KeyValuePair<string, string>(parameterName, value.ToString().ToLowerInvariant()));
    }

    public static void AddIfNotNull<T>(
      this IList<KeyValuePair<string, string>> queryParameters,
      string parameterName,
      T value)
    {
      if ((object) value == null)
        return;
      queryParameters.Add(new KeyValuePair<string, string>(parameterName, value.ToString()));
    }
  }
}
