// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOrchestrationConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServicingOrchestrationConstants
  {
    public const string Area = "/ServicingOrchestration";
    public const int DefaultJobRetryThreshold = 10;
    public static readonly TimeSpan DefaultRetryInterval = TimeSpan.FromMinutes(10.0);
    public static readonly TimeSpan DefaultJobRetryMinWait = TimeSpan.FromMinutes(200.0);
    public static readonly int DefaultServicingLogsRetryThreshold = 3;
    public static readonly TimeSpan DefaultServicingLogsRetryInterval = TimeSpan.FromMinutes(7.0);
    public static string OrchestrationLock = "/ServicingOrchestration/OrchestrationLock";
    public static string DataImportOrchestrationLock = ServicingOrchestrationConstants.OrchestrationLock + "/DataImport";
    private const string c_servicingOrchestrationSettingsRegistryPath = "/Configuration";
    private const string c_servicingOrchestrationRegionRegistryPath = "/ServicingOrchestration/RetryRegion";

    public static string ServicingOrchestrationSettingsPath(string areaName) => "/Configuration/" + areaName;

    public static string JobRetryThreshold(string areaName) => ServicingOrchestrationConstants.ServicingOrchestrationSettingsPath(areaName) + "/RetryThreshold";

    public static string JobRetryInterval(string areaName) => ServicingOrchestrationConstants.ServicingOrchestrationSettingsPath(areaName) + "/RetryInterval";

    public static string JobRetryMinWait(string areaName) => ServicingOrchestrationConstants.ServicingOrchestrationSettingsPath(areaName) + "/RetryMinWait";

    public static string ServicingLogsRetryThreshold(string areaName) => ServicingOrchestrationConstants.ServicingOrchestrationSettingsPath(areaName) + "/LogRetryThreshold";

    public static string ServicingLogsRetryInterval(string areaName) => ServicingOrchestrationConstants.ServicingOrchestrationSettingsPath(areaName) + "/LogRetryInterval";

    public static string RetryRegionCurrentRegistryKey(Guid servicingJobId) => "/ServicingOrchestration/RetryRegion/" + servicingJobId.ToString("D") + "/Current";

    public static string RetryRegionAllPropertiesRegistyKey(Guid servicingJobId, string name) => ServicingOrchestrationConstants.RetryRegionRootRegistryKey(servicingJobId, name) + "**";

    public static string RetryRegionRootRegistryKey(Guid servicingJobId, string name) => "/ServicingOrchestration/RetryRegion/" + servicingJobId.ToString("D") + "/" + name + "/";
  }
}
