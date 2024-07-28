// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ResourceLockComponent4
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
  internal class ResourceLockComponent4 : ResourceLockComponent3
  {
    public override ResourceLockRequest GetResourceLockRequestByCheckRun(
      Guid projectId,
      Guid checkRunId)
    {
      return this.GetResourceLockRequestsByCheckRuns(projectId, (IEnumerable<Guid>) new Guid[1]
      {
        checkRunId
      }).FirstOrDefault<ResourceLockRequest>();
    }

    public override List<ResourceLockRequest> GetResourceLockRequestsByCheckRuns(
      Guid projectId,
      IEnumerable<Guid> checkRunIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetResourceLockRequestsByCheckRuns)))
      {
        this.PrepareStoredProcedure("Task.prc_GetResourceLockRequestByCheckRuns");
        this.BindGuid("@projectId", projectId);
        this.BindGuidTable("@checkRunIds", checkRunIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>((ObjectBinder<ResourceLockRequest>) new ResourceLockRequestBinder());
          return resultCollection.GetCurrent<ResourceLockRequest>().Items;
        }
      }
    }
  }
}
