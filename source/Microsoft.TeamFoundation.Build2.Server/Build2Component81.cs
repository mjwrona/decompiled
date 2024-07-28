// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component81
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component81 : Build2Component80
  {
    public override IEnumerable<BuildData> GetBuilds(
      Guid projectId,
      IEnumerable<int> definitionIds,
      IEnumerable<int> queueIds,
      string buildNumber,
      DateTime? minFinishTime,
      DateTime? maxFinishTime,
      IEnumerable<Guid> requestedForIds,
      BuildReason? reasonFilter,
      BuildStatus? statusFilter,
      BuildResult? resultFilter,
      IEnumerable<string> tagFilters,
      int maxBuilds,
      QueryDeletedOption queryDeletedOption,
      BuildQueryOrder queryOrder,
      IList<int> excludedDefinitionIds,
      string repositoryId,
      string repositoryType,
      string branchName,
      int? maxBuildsPerDefinition)
    {
      using (this.TraceScope(method: nameof (GetBuilds)))
      {
        this.PrepareStoredProcedure("Build.prc_GetBuilds2");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindUniqueInt32Table("@definitionIdTable", definitionIds);
        this.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        this.BindUniqueInt32Table("@queueIdTable", queueIds);
        this.BindString("@buildNumber", DBHelper.ServerPathToDBPath(buildNumber), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUtcDateTime2("@minTime", minFinishTime);
        this.BindNullableUtcDateTime2("@maxTime", maxFinishTime);
        this.BindGuidTable("@requestedForIdTable", requestedForIds);
        BuildReason? nullable1 = reasonFilter;
        this.BindNullableInt32("@reasonFilter", nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?());
        BuildStatus? nullable2 = statusFilter;
        this.BindNullableByte("@statusFilter", nullable2.HasValue ? new byte?((byte) nullable2.GetValueOrDefault()) : new byte?());
        BuildResult? nullable3 = resultFilter;
        this.BindNullableByte("@resultFilter", nullable3.HasValue ? new byte?((byte) nullable3.GetValueOrDefault()) : new byte?());
        this.BindNullableInt32("@maxBuilds", new int?(maxBuilds));
        this.BindInt("@queryOrder", (int) queryOrder);
        this.BindInt("@queryDeletedOption", (int) queryDeletedOption);
        this.BindStringTable("@tagFilters", tagFilters);
        this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@branchName", branchName, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableInt32("@maxBuildsPerDefinition", maxBuildsPerDefinition);
        List<BuildData> items;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
          {
            BuildData buildData;
            if (dictionary.TryGetValue(buildTagData.BuildId, out buildData))
              buildData.Tags.Add(buildTagData.Tag);
          }
          resultCollection.NextResult();
          foreach (BuildOrchestrationData orchestrationData in resultCollection.GetCurrent<BuildOrchestrationData>())
          {
            BuildData buildData;
            if (dictionary.TryGetValue(orchestrationData.BuildId, out buildData))
            {
              int? orchestrationType = orchestrationData.Plan.OrchestrationType;
              int num = 1;
              if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
                buildData.OrchestrationPlan = orchestrationData.Plan;
              buildData.Plans.Add(orchestrationData.Plan);
            }
          }
        }
        return (IEnumerable<BuildData>) items;
      }
    }
  }
}
