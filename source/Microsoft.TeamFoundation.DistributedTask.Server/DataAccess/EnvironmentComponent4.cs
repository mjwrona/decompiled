// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentComponent4
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class EnvironmentComponent4 : EnvironmentComponent3
  {
    public override IList<EnvironmentInstance> GetEnvironmentsByModifiedTime(
      Guid projectId,
      DateTime? lastModifiedOn,
      int batchSize)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentsByModifiedTime)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironmentsByModifiedTime");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindNullableDateTime2("@lastModifiedOn", lastModifiedOn);
        this.BindInt("@batchSize", batchSize);
        List<EnvironmentInstance> environmentsByModifiedTime = new List<EnvironmentInstance>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
          environmentsByModifiedTime.AddRange((IEnumerable<EnvironmentInstance>) resultCollection.GetCurrent<EnvironmentInstance>().Items);
          return (IList<EnvironmentInstance>) environmentsByModifiedTime;
        }
      }
    }
  }
}
