// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceSemaphoreInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ResourceSemaphoreInfo
  {
    public int ResourceSemaphoreId { get; set; }

    public long TargetMemoryKB { get; set; }

    public long MaxTargetMemoryKB { get; set; }

    public long TotalMemoryKB { get; set; }

    public long AvailableMemoryKB { get; set; }

    public long GrantedMemoryKB { get; set; }

    public long UsedMemoryKB { get; set; }

    public int GranteeCount { get; set; }

    public int WaiterCount { get; set; }

    public long TimeoutErrorCount { get; set; }

    public long ForcedGrantCount { get; set; }

    public int PoolId { get; set; }
  }
}
