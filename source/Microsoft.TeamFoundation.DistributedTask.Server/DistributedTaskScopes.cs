// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskScopes
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class DistributedTaskScopes
  {
    public static readonly string AgentListenScope = "DistributedTask.AgentListen";
    public static readonly string AgentCloudRequestUse = "DistributedTask.AgentCloudRequestUse";
    public static readonly string AgentCloudRequestListen = "DistributedTask.AgentCloudRequestListen";
    public static readonly string PoolManage = "DistributedTask.PoolManage";
    public static readonly string ReadIdentityRefs = "Identity.ReadRefs";
    public static readonly string ConnectLocationService = "LocationService.Connect";
    public static readonly string FrameworkGlobalSecurity = nameof (FrameworkGlobalSecurity);
  }
}
