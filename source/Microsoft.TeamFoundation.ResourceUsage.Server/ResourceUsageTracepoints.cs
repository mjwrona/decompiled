// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ResourceUsage.Server.ResourceUsageTracepoints
// Assembly: Microsoft.TeamFoundation.ResourceUsage.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55568389-A340-4F60-8DD1-887E0E3F1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ResourceUsage.Server.dll

namespace Microsoft.TeamFoundation.ResourceUsage.Server
{
  public static class ResourceUsageTracepoints
  {
    public const int TracepointStart = 94000000;
    public const int Controller_GetTeamProjectCollectionLimitsEnter = 94000001;
    public const int Controller_GetTeamProjectCollectionLimitsLeave = 94000002;
    public const int Controller_GetProjectLimitsEnter = 94000011;
    public const int Controller_GetProjectLimitsLeave = 94000012;
    public const int Service_GetTeamProjectCollectionLimitsServiceEnter = 94000021;
    public const int Service_GetTeamProjectCollectionLimitaServiceLeave = 94000022;
    public const int Service_GetProjectLimitServiceEnter = 94000041;
    public const int Service_GetProjectLimitServiceLeave = 94000042;
    public const int Controller_GetTeamProjectCollectionLimitsException = 94000051;
  }
}
