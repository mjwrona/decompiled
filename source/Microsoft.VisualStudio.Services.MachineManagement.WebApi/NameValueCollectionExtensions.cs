// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.NameValueCollectionExtensions
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  internal static class NameValueCollectionExtensions
  {
    public static void AddIfNotEmpty<TValue>(
      this NameValueCollection queryParameters,
      string parameterName,
      IEnumerable<TValue> values)
    {
      if (values == null || !values.Any<TValue>())
        return;
      queryParameters.Add(parameterName, string.Join<TValue>(",", values));
    }

    public static void AddIfNotEmpty(
      this NameValueCollection queryParameters,
      string parameterName,
      string value)
    {
      if (string.IsNullOrEmpty(value))
        return;
      queryParameters.Add(parameterName, value);
    }

    public static void AddIfTrue(
      this NameValueCollection queryParameters,
      string parameterName,
      bool value)
    {
      if (!value)
        return;
      queryParameters.Add(parameterName, value.ToString().ToLowerInvariant());
    }

    public static void AddIfNotNull<T>(
      this NameValueCollection queryParameters,
      string parameterName,
      T value)
    {
      if ((object) value == null)
        return;
      queryParameters.Add(parameterName, value.ToString());
    }
  }
}
