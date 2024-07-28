// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[10]
    {
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent>(1, true),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent2>(2),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent3>(3),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent4>(4),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent5>(5),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent6>(6),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent7>(7),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent8>(8),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent9>(9),
      (IComponentCreator) new ComponentCreator<MetaTaskDefinitionComponent10>(10)
    }, "DistributedTaskMetaTaskDefinition", "DistributedTask");
    protected Guid MetaTaskNamespaceId = new Guid("f6a4de49-dbe2-4704-86dc-f8ec1a294436");

    public MetaTaskDefinitionComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void AddMetaTaskDefinition(
      Guid projectId,
      Guid definitionId,
      string name,
      byte[] metadataDocument)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddMetaTaskDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_AddMetaTaskDefinition", projectId);
        this.BindGuid("@definitionId", definitionId);
        this.BindString("@name", name, 40, false, SqlDbType.NVarChar);
        this.BindBinary("@metadataDocument", metadataDocument, SqlDbType.VarBinary);
        this.BindGuid("@writerId", this.Author);
        this.ExecuteNonQuery();
      }
    }

    public virtual MetaTaskDefinitionData AddMetaTaskDefinition(Guid projectId, TaskGroup taskGroup)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddMetaTaskDefinition)))
      {
        byte[] metadataDocument = JsonUtility.Serialize((object) taskGroup);
        this.AddMetaTaskDefinition(projectId, taskGroup.Id, taskGroup.Name, metadataDocument);
        return (MetaTaskDefinitionData) null;
      }
    }

    public virtual void UpdateMetaTaskDefinition(
      Guid projectId,
      Guid definitionId,
      string name,
      byte[] metadataDocument)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateMetaTaskDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateMetaTaskDefinition", projectId);
        this.BindGuid("@definitionId", definitionId);
        this.BindString("@name", name, 40, false, SqlDbType.NVarChar);
        this.BindBinary("@metadataDocument", metadataDocument, SqlDbType.VarBinary);
        this.BindGuid("@writerId", this.Author);
        this.ExecuteNonQuery();
      }
    }

    public virtual MetaTaskDefinitionData PublishTaskGroup(
      Guid projectId,
      PublishTaskGroupMetadata taskGroupMetadata,
      Guid taskGroupId,
      Guid modifiedBy)
    {
      return (MetaTaskDefinitionData) null;
    }

    public virtual MetaTaskDefinitionData UpdateMetaTaskDefinitionAndDisableOldVersions(
      Guid projectId,
      TaskGroup taskGroup,
      bool disablePriorVersions,
      bool enablePriorVersionEdit,
      out List<int> disabledVersions)
    {
      disabledVersions = new List<int>();
      return (MetaTaskDefinitionData) null;
    }

    public virtual MetaTaskDefinitionData UpdateMetaTaskDefinition(
      Guid projectId,
      TaskGroup taskGroup,
      bool enablePriorVersionEdit)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateMetaTaskDefinition)))
      {
        byte[] metadataDocument = JsonUtility.Serialize((object) taskGroup);
        this.UpdateMetaTaskDefinition(projectId, taskGroup.Id, taskGroup.Name, metadataDocument);
        return (MetaTaskDefinitionData) null;
      }
    }

    public virtual void DeleteMetaTaskDefinition(Guid projectId, Guid definitionId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteMetaTaskDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteMetaTaskDefinition", projectId);
        this.BindGuid("@definitionId", definitionId);
        this.ExecuteNonQuery();
      }
    }

    public virtual List<MetaTaskDefinitionData> GetMetaTaskDefinitions(
      Guid projectId,
      Guid? definitionId = null,
      Guid? taskIdFilter = null,
      bool deleted = false,
      DateTime? continuationToken = null,
      int top = 0,
      TaskGroupQueryOrder queryOrder = TaskGroupQueryOrder.CreatedOnDescending)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetMetaTaskDefinitions)))
      {
        this.PrepareStoredProcedure("Task.prc_GetMetaTaskDefinitions", projectId);
        this.BindNullableGuid("@definitionId", definitionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<MetaTaskDefinitionData>((ObjectBinder<MetaTaskDefinitionData>) new MetaTaskDefinitionDataBinder());
          return resultCollection.GetCurrent<MetaTaskDefinitionData>().Items;
        }
      }
    }

    public virtual void SoftDeleteTaskGroup(
      Guid projectId,
      Guid taskGroupId,
      Guid modifiedBy,
      string comment = null)
    {
      this.DeleteMetaTaskDefinition(projectId, taskGroupId);
    }

    public virtual void UndeleteTaskGroup(
      Guid projectId,
      Guid taskGroupId,
      Guid modifiedBy,
      string comment = null)
    {
    }

    public virtual IList<int> HardDeleteTaskGroups(int daysToRetain) => (IList<int>) new List<int>();

    public virtual void DeleteTeamProject(Guid projectId)
    {
    }
  }
}
