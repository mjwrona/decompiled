// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentPoolData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class TaskAgentPoolData
  {
    public TaskAgentPoolData()
    {
    }

    private TaskAgentPoolData(TaskAgentPoolData toClone)
    {
      this.Pool = toClone.Pool.Clone();
      this.ServiceAccountId = toClone.ServiceAccountId;
      this.ServiceIdentityId = toClone.ServiceIdentityId;
      this.PoolMetadataFileId = toClone.PoolMetadataFileId;
    }

    public TaskAgentPool Pool { get; set; }

    public Guid ServiceAccountId { get; set; }

    public Guid? ServiceIdentityId { get; set; }

    public int? PoolMetadataFileId { get; set; }

    public TaskAgentPoolData Clone() => new TaskAgentPoolData(this);
  }
}
