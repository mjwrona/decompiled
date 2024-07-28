// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VirtualMachineGroupComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class VirtualMachineGroupComponent2 : VirtualMachineGroupComponent
  {
    public override IList<VirtualMachineGroup> GetEnvironmentResourcesById(
      Guid projectId,
      int environmentId,
      IEnumerable<int> resourceIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentResourcesById)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVirtualMachineGroupsById");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindUniqueInt32Table("@resourceIds", resourceIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VirtualMachineGroup>((ObjectBinder<VirtualMachineGroup>) new VirtualMachineGroupBinder());
          return (IList<VirtualMachineGroup>) resultCollection.GetCurrent<VirtualMachineGroup>().ToArray<VirtualMachineGroup>();
        }
      }
    }
  }
}
