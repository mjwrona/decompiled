// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.PersistedStageComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class PersistedStageComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PersistedStageComponent>(1)
    }, "DistributedTaskPersistedStage", "DistributedTask");
    private static readonly SqlMetaData[] typ_PersistedStageReference = new SqlMetaData[3]
    {
      new SqlMetaData("@definitionId", SqlDbType.Int),
      new SqlMetaData("@stageName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("@buildId", SqlDbType.Int)
    };

    public PersistedStageComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual PersistedStage AddPersistedStageGroupMapping(
      Guid projectId,
      PersistedStageReference stageRef)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddPersistedStageGroupMapping)))
      {
        this.PrepareStoredProcedure("Task.prc_AddPersistedStageGroupMapping");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@definitionId", stageRef.DefinitionId);
        this.BindInt("@buildId", stageRef.BuildId);
        this.BindString("@stageName", stageRef.Name.Literal, 256, false, SqlDbType.NVarChar);
        this.BindString("@group", DBPath.UserToDatabasePath(stageRef.GroupPath, true), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        return this.GetPersistedStageResponses().FirstOrDefault<PersistedStage>();
      }
    }

    public virtual IList<PersistedStage> GetPersistedStages(
      Guid projectId,
      ICollection<PersistedStageReference> stageRefs,
      bool mapToGroup)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetPersistedStages)))
      {
        this.PrepareStoredProcedure("Task.prc_GetPersistedStages");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindTable("@persistedStageReferences", "Task.typ_PersistedStageReferenceTable", stageRefs.Select<PersistedStageReference, SqlDataRecord>(new System.Func<PersistedStageReference, SqlDataRecord>(this.ConvertToSqlDataRecord)));
        this.BindBoolean("@mapToGroup", mapToGroup);
        return this.GetPersistedStageResponses();
      }
    }

    internal virtual IList<PersistedStage> GetPersistedStageResponses()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PersistedStage>((ObjectBinder<PersistedStage>) new PersistedStageBinder());
        resultCollection.AddBinder<PersistedStageGroup>((ObjectBinder<PersistedStageGroup>) new PersistedStageGroupBinder());
        List<PersistedStage> items = resultCollection.GetCurrent<PersistedStage>().Items;
        resultCollection.NextResult();
        Dictionary<long, PersistedStageGroup> dictionary = resultCollection.GetCurrent<PersistedStageGroup>().Items.ToDictionary<PersistedStageGroup, long, PersistedStageGroup>((System.Func<PersistedStageGroup, long>) (x => x.StageId), (System.Func<PersistedStageGroup, PersistedStageGroup>) (x => x));
        foreach (PersistedStage persistedStage in items)
        {
          PersistedStageGroup persistedStageGroup;
          if (dictionary.TryGetValue(persistedStage.Id, out persistedStageGroup))
            persistedStage.Group = persistedStageGroup;
        }
        return (IList<PersistedStage>) items;
      }
    }

    public virtual IList<PersistedStage> GetPersistedStagesByDefinition(
      Guid projectId,
      int pipelineDefinitionId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetPersistedStagesByDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_GetPersistedStagesByDefinitionId");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@definitionId", pipelineDefinitionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PersistedStage>((ObjectBinder<PersistedStage>) new PersistedStageBinder());
          return (IList<PersistedStage>) resultCollection.GetCurrent<PersistedStage>().Items;
        }
      }
    }

    protected SqlDataRecord ConvertToSqlDataRecord(PersistedStageReference persistedStageReference)
    {
      SqlDataRecord record = new SqlDataRecord(PersistedStageComponent.typ_PersistedStageReference);
      record.SetInt32(0, persistedStageReference.DefinitionId);
      record.SetString(1, persistedStageReference.Name.Literal);
      record.SetNullableInt32(2, new int?(persistedStageReference.BuildId));
      return record;
    }
  }
}
