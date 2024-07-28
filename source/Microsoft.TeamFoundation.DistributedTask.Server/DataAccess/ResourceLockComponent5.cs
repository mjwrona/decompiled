// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ResourceLockComponent5
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ResourceLockComponent5 : ResourceLockComponent4
  {
    public override List<ResourceLockRequest> QueueResourceLockRequestsV2(
      IEnumerable<ResourceLockRequest> requests)
    {
      List<ResourceLockRequest> resourceLockRequestList = new List<ResourceLockRequest>();
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (QueueResourceLockRequestsV2)))
      {
        this.PrepareStoredProcedure("Task.prc_QueueResourceLockRequestsV2");
        this.BindResourceLockTable("@requests", requests);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>(this.CreateResourceLockRequestBinder());
          resourceLockRequestList.AddRange((IEnumerable<ResourceLockRequest>) resultCollection.GetCurrent<ResourceLockRequest>().Items);
        }
      }
      return resourceLockRequestList;
    }
  }
}
