// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentDeploymentExecutionHistoryComponent3
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
  internal class EnvironmentDeploymentExecutionHistoryComponent3 : 
    EnvironmentDeploymentExecutionHistoryComponent2
  {
    public override IList<EnvironmentDeploymentExecutionRecord> GetLastSuccessfulDeploymentByRunIdOrJobs(
      Guid scopeId,
      string planType,
      int environmentId,
      int definitionId,
      int ownerId,
      string stageName,
      IEnumerable<string> jobs)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLastSuccessfulDeploymentByRunIdOrJobs)))
      {
        this.PrepareStoredProcedure("Task.prc_GetLastSuccessfulDeploymentByRunIdOrJobs");
        this.BindGuid("@scopeId", scopeId);
        this.BindString("@planType", planType, 260, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@definitionId", definitionId);
        this.BindInt("@ownerId", ownerId);
        this.BindString("@stageName", stageName, 260, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindStringTable("@jobs", jobs == null ? (IEnumerable<string>) null : jobs.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
        List<EnvironmentDeploymentExecutionRecord> deploymentByRunIdOrJobs = new List<EnvironmentDeploymentExecutionRecord>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentDeploymentExecutionRecord>((ObjectBinder<EnvironmentDeploymentExecutionRecord>) new EnvironmentDeploymentExecutionRecordBinder((EnvironmentDeploymentExecutionHistoryComponent) this));
          deploymentByRunIdOrJobs.AddRange((IEnumerable<EnvironmentDeploymentExecutionRecord>) resultCollection.GetCurrent<EnvironmentDeploymentExecutionRecord>().Items);
          return (IList<EnvironmentDeploymentExecutionRecord>) deploymentByRunIdOrJobs;
        }
      }
    }
  }
}
