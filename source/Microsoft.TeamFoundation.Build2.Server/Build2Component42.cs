// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component42
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component42 : Build2Component41
  {
    public override BuildData GetLatestSuccessfulBuildForBranch(
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName,
      DateTime? maxFinishTime)
    {
      using (this.TraceScope(method: nameof (GetLatestSuccessfulBuildForBranch)))
      {
        this.PrepareStoredProcedure("Build.prc_GetLatestSuccessfulBuildForBranch");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@definitionId", definitionId);
        this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@branchName", branchName, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindUtcDateTime2("@maxFinishTime", maxFinishTime ?? DateTime.UtcNow);
        BuildData successfulBuildForBranch;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          successfulBuildForBranch = resultCollection.GetCurrent<BuildData>().FirstOrDefault<BuildData>();
          resultCollection.NextResult();
          foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
          {
            if (successfulBuildForBranch.Id == buildTagData.BuildId)
              successfulBuildForBranch.Tags.Add(buildTagData.Tag);
          }
          resultCollection.NextResult();
          foreach (BuildOrchestrationData orchestrationData in resultCollection.GetCurrent<BuildOrchestrationData>())
          {
            int? orchestrationType = orchestrationData.Plan.OrchestrationType;
            int num = 1;
            if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
              successfulBuildForBranch.OrchestrationPlan = orchestrationData.Plan;
            successfulBuildForBranch.Plans.Add(orchestrationData.Plan);
          }
        }
        return successfulBuildForBranch;
      }
    }

    protected virtual ObjectBinder<BuildTagData> GetCollectionBuildTagBinder() => (ObjectBinder<BuildTagData>) new CollectionBuildTagBinder();

    protected virtual ObjectBinder<BuildOrchestrationData> GetCollectionBuildOrchestrationDataBinder() => (ObjectBinder<BuildOrchestrationData>) new CollectionBuildOrchestrationDataBinder();
  }
}
