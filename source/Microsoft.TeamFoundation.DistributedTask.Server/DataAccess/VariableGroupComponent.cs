// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VariableGroupComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class VariableGroupComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<VariableGroupComponent>(1),
      (IComponentCreator) new ComponentCreator<VariableGroupComponent2>(2),
      (IComponentCreator) new ComponentCreator<VariableGroupComponent3>(3),
      (IComponentCreator) new ComponentCreator<VariableGroupComponent4>(4),
      (IComponentCreator) new ComponentCreator<VariableGroupComponent5>(5),
      (IComponentCreator) new ComponentCreator<VariableGroupComponent6>(6)
    }, "DistributedTaskVariableGroup", "DistributedTask");

    public VariableGroupComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual VariableGroup AddVariableGroup(Guid projectId, VariableGroup group)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddVariableGroup)))
      {
        this.PrepareStoredProcedure("Task.prc_AddVariableGroup");
        this.BindDataspaceId(projectId);
        this.BindString("@groupName", group.Name, 256, false, SqlDbType.NVarChar);
        this.BindString("@groupDescription", group.Description, 1024, true, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(group.CreatedBy.Id));
        this.BindString("@groupVariables", JsonUtility.ToString((object) VariableGroupUtility.ClearSecrets(group.Variables)), int.MaxValue, true, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder());
          return resultCollection.GetCurrent<VariableGroup>().FirstOrDefault<VariableGroup>();
        }
      }
    }

    public virtual VariableGroup DeleteVariableGroup(Guid projectId, int groupId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteVariableGroup)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteVariableGroup");
        this.BindDataspaceId(projectId);
        this.BindInt("@groupId", groupId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder());
          return resultCollection.GetCurrent<VariableGroup>().Items.FirstOrDefault<VariableGroup>();
        }
      }
    }

    public virtual void DeleteVariableGroupFromProjects(int groupId, IList<Guid> projectIds)
    {
    }

    public virtual VariableGroup GetVariableGroup(Guid projectId, int groupId) => this.GetVariableGroups(projectId, (IEnumerable<int>) new int[1]
    {
      groupId
    }).FirstOrDefault<VariableGroup>();

    public virtual VariableGroup GetVariableGroup(int groupId) => (VariableGroup) null;

    public virtual List<VariableGroup> GetVariableGroups(Guid projectId, IEnumerable<int> groupIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetVariableGroups)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVariableGroupsById");
        this.BindDataspaceId(projectId);
        this.BindUniqueInt32Table("@groupIds", groupIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder());
          return resultCollection.GetCurrent<VariableGroup>().Items;
        }
      }
    }

    public virtual List<VariableGroup> GetVariableGroups(
      Guid projectId,
      string groupName,
      int? continuationToken = null,
      int top = 0,
      VariableGroupQueryOrder queryOrder = VariableGroupQueryOrder.IdDescending)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetVariableGroups)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVariableGroupsByName");
        this.BindDataspaceId(projectId);
        this.BindString("@groupName", groupName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder());
          return resultCollection.GetCurrent<VariableGroup>().Items;
        }
      }
    }

    public virtual VariableGroup UpdateVariableGroup(Guid projectId, VariableGroup group)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateVariableGroup)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateVariableGroup");
        this.BindDataspaceId(projectId);
        this.BindInt("@groupId", group.Id);
        this.BindString("@groupName", group.Name, 256, false, SqlDbType.NVarChar);
        this.BindString("@groupDescription", group.Description, 1024, true, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(group.ModifiedBy.Id));
        this.BindString("@groupVariables", JsonUtility.ToString((object) VariableGroupUtility.ClearSecrets(group.Variables)), int.MaxValue, true, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder());
          return resultCollection.GetCurrent<VariableGroup>().FirstOrDefault<VariableGroup>();
        }
      }
    }

    public virtual VariableGroup AddVariableGroupCollection(VariableGroup group) => (VariableGroup) null;

    public virtual VariableGroup UpdateVariableGroupCollection(
      VariableGroup group,
      bool isCollectionLevelVariableGroupChanged)
    {
      return (VariableGroup) null;
    }

    public virtual void DeleteTeamProject(Guid projectId)
    {
    }

    public virtual void ShareVariableGroup(
      int groupId,
      IList<VariableGroupProjectReference> variableGroupProjectReferences)
    {
    }

    public virtual List<int> GetSharedVariableGroups(Guid projectId, IEnumerable<int> groupIds) => new List<int>();

    public virtual IList<VariableGroupProjectReference> GetVariableGroupProjectReferences(
      int variableGroupId)
    {
      return (IList<VariableGroupProjectReference>) new List<VariableGroupProjectReference>();
    }
  }
}
