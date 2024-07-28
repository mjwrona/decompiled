// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ExecutionContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class ExecutionContextExtensions
  {
    public static string GetConfigValue(this ExecutionContext executionContext, string key)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      return executionContext.RequestContext.GetConfigValue<string>(key);
    }

    public static string GetCollectionConfigValue(
      this ExecutionContext executionContext,
      string key)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      return executionContext.RequestContext.GetCurrentHostConfigValue<string>(key);
    }

    public static T GetConfigValue<T>(
      this ExecutionContext executionContext,
      string key,
      T defaultValue = null)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      return executionContext.RequestContext.GetConfigValue<T>(key, defaultValue);
    }

    public static void SetConfigValue<T>(
      this ExecutionContext executionContext,
      string key,
      T value)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      executionContext.RequestContext.SetConfigValue<T>(key, value);
    }

    public static void SetCollectionConfigValue<T>(
      this ExecutionContext executionContext,
      string key,
      T value)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      executionContext.RequestContext.SetCollectionConfigValue<T>(key, value);
    }
  }
}
