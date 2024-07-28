// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentComponent2
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
  internal class EnvironmentComponent2 : EnvironmentComponent
  {
    public override IList<EnvironmentInstance> GetEnvironmentsByIds(
      Guid projectId,
      IEnumerable<int> environmentIds,
      bool includeResourceReferences = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentsByIds)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironmentsByIds");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt32Table("@environmentIds", environmentIds);
        this.BindBoolean("@includeResourceReferences", includeResourceReferences);
        List<EnvironmentInstance> environments = new List<EnvironmentInstance>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
          environments.AddRange((IEnumerable<EnvironmentInstance>) resultCollection.GetCurrent<EnvironmentInstance>().Items);
          if (includeResourceReferences)
          {
            resultCollection.NextResult();
            resultCollection.AddBinder<EnvironmentResourceData>((ObjectBinder<EnvironmentResourceData>) new EnvironmentResourceDataBinder());
            List<EnvironmentResourceData> items = resultCollection.GetCurrent<EnvironmentResourceData>().Items;
            this.MapResourcesToEnvironments((IList<EnvironmentInstance>) environments, (IList<EnvironmentResourceData>) items);
          }
          return (IList<EnvironmentInstance>) environments;
        }
      }
    }
  }
}
