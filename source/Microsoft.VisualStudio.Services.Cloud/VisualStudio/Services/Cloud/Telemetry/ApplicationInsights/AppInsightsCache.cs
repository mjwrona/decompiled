// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Telemetry.ApplicationInsights.AppInsightsCache
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Cloud.Telemetry.ApplicationInsights
{
  internal static class AppInsightsCache
  {
    public static ConcurrentDictionary<string, string> storageDependencyCache = new ConcurrentDictionary<string, string>();
  }
}
