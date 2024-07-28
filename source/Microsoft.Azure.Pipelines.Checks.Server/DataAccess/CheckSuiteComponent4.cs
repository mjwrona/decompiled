// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteComponent4
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckSuiteComponent4 : CheckSuiteComponent3
  {
    public override IList<CheckRun> GetCheckRuns(Guid projectId, CheckRunFilter checkRunFilter)
    {
      string scopeString = checkRunFilter.Resource.GetScopeString();
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (GetCheckRuns)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_GetCheckRuns");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindInt("@status", (int) checkRunFilter.Status);
        this.BindGuid("@typeId", checkRunFilter.Type);
        this.BindString("@scope", scopeString, 256, false, SqlDbType.NVarChar);
        return this.GetCheckRunsResponse();
      }
    }

    public override CheckSuite AddCheckRuns(
      Guid projectId,
      Guid checkSuiteId,
      IList<CheckRun> checkRuns)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (AddCheckRuns)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddCheckRuns");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuid("@batchRequestId", checkSuiteId);
        this.BindTable("@createParameters", "PipelinePolicy.typ_PolicyEvaluationCreateParametersTable2", checkRuns.Select<CheckRun, SqlDataRecord>(new System.Func<CheckRun, SqlDataRecord>(((CheckSuiteComponent) this).ConvertToCheckSuiteCreateTableSqlDataRecord)));
        return this.GetCheckSuiteResponses().FirstOrDefault<CheckSuite>();
      }
    }

    public override IList<CheckSuite> DeleteCheckSuites(Guid projectId, IList<Guid> checkSuiteIds)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (DeleteCheckSuites)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_DeleteCheckSuites");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuidTable("@checkSuiteIds", (IEnumerable<Guid>) checkSuiteIds);
        return (IList<CheckSuite>) this.GetCheckSuiteResponses();
      }
    }

    protected virtual IList<CheckRun> GetCheckRunsResponse()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CheckRun>((ObjectBinder<CheckRun>) this.GetCheckSuiteBinder());
        return (IList<CheckRun>) resultCollection.GetCurrent<CheckRun>().Items;
      }
    }

    protected override CheckSuiteBinder GetCheckSuiteBinder() => (CheckSuiteBinder) new CheckSuiteBinder3();
  }
}
