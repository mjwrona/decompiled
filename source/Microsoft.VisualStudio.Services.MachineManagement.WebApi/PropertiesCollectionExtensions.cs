// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.PropertiesCollectionExtensions
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class PropertiesCollectionExtensions
  {
    public static Uri GetUri(this PropertiesCollection properties, string key) => new Uri((string) properties[key]);

    public static Uri GetUri(this PropertiesCollection properties, string key, Uri defaultValue)
    {
      string uriString;
      return !properties.TryGetValue<string>(key, out uriString) ? defaultValue : new Uri(uriString);
    }

    public static void SetUri(this PropertiesCollection properties, string key, Uri value) => properties[key] = (object) value.OriginalString;

    public static T GetCascadedValue<T>(
      this PropertiesCollection properties,
      string key,
      PropertiesCollection secondary)
    {
      T obj;
      return !properties.TryGetValue<T>(key, out obj) ? (T) secondary[key] : obj;
    }

    public static T GetCascadedValue<T>(
      this PropertiesCollection properties,
      string key,
      PropertiesCollection secondary,
      T defaultValue)
    {
      T obj;
      return !properties.TryGetValue<T>(key, out obj) && !secondary.TryGetValue<T>(key, out obj) ? defaultValue : obj;
    }

    public static TimeSpan GetTimeSpan(
      this PropertiesCollection properties,
      string key,
      TimeSpan defaultValue)
    {
      string s;
      TimeSpan result;
      return properties.TryGetValue<string>(key, out s) && TimeSpan.TryParse(s, out result) ? result : defaultValue;
    }
  }
}
