// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoWellKnownDataSources
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class KustoWellKnownDataSources
  {
    public static readonly string ActivityLog = nameof (ActivityLog);
    public static readonly string ActivityLogMapping = nameof (ActivityLogMapping);
    public static readonly string IisLogs = nameof (IisLogs);
    public static readonly string ProductTrace = nameof (ProductTrace);
    public static readonly string ActivityLogFiltered = nameof (ActivityLogFiltered);
    public static readonly string ActivityLogMappingFiltered = nameof (ActivityLogMappingFiltered);
    public static readonly string XEventDataRPCCompleted = nameof (XEventDataRPCCompleted);
    public static readonly string XEventDataRPCCompletedFiltered = nameof (XEventDataRPCCompletedFiltered);
    public static readonly string DatabasePerformanceStatistics = nameof (DatabasePerformanceStatistics);
    public static readonly string Databases = "databases";
    public static readonly string DatabaseRecentlyUpdated = nameof (DatabaseRecentlyUpdated);
    public static readonly string OrchestrationPlanContext = nameof (OrchestrationPlanContext);
    public static readonly string OrchestrationPlanContextFiltered = nameof (OrchestrationPlanContext);
    public static readonly string ServiceHostAggregated = "ServiceHostAggregated()";
    public static readonly string CustomerImpact5M = "CustomerImpact_5M";
  }
}
