// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ResourceUsageData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class ResourceUsageData
  {
    public Guid HostId { get; set; }

    public string ResourceType { get; set; }

    public int RunningRequestsCount { get; set; }
  }
}
