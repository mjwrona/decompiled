// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ResourceLockComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ResourceLockComponent2 : ResourceLockComponent
  {
    public override async Task<List<ResourceLockRequest>> FreeResourceLocksAsync(
      Guid planId,
      string nodeName = null,
      int? nodeAttempt = null)
    {
      ResourceLockComponent2 component = this;
      List<ResourceLockRequest> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FreeResourceLocksAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_FreeResourceLockRequests");
        component.BindGuid("@planId", planId);
        component.BindString("@nodeName", nodeName, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindNullableInt("@nodeAttempt", nodeAttempt);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>(component.CreateResourceLockRequestBinder());
          items = resultCollection.GetCurrent<ResourceLockRequest>().Items;
        }
      }
      return items;
    }
  }
}
