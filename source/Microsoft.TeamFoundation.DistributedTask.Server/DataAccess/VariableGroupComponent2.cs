// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VariableGroupComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class VariableGroupComponent2 : VariableGroupComponent
  {
    public override VariableGroup AddVariableGroup(Guid projectId, VariableGroup group)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddVariableGroup)))
      {
        this.PrepareForAuditingAction(VariableGroupAuditConstants.VariableGroupCreated, projectId: projectId, excludeParameters: (IEnumerable<string>) new string[1]
        {
          "@variables"
        });
        this.PrepareStoredProcedure("Task.prc_AddVariableGroup");
        this.BindDataspaceId(projectId);
        this.BindString("@type", group.Type, 256, false, SqlDbType.NVarChar);
        this.BindString("@name", group.Name, 256, false, SqlDbType.NVarChar);
        this.BindString("@description", group.Description, 1024, true, SqlDbType.NVarChar);
        this.BindString("@providerData", JsonUtility.ToString((object) group.ProviderData), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(group.CreatedBy.Id));
        this.BindString("@variables", JsonUtility.ToString((object) VariableGroupUtility.ClearSecrets(group.Variables)), int.MaxValue, true, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder2());
          return resultCollection.GetCurrent<VariableGroup>().FirstOrDefault<VariableGroup>();
        }
      }
    }

    public override VariableGroup DeleteVariableGroup(Guid projectId, int groupId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteVariableGroup)))
      {
        this.PrepareForAuditingAction(VariableGroupAuditConstants.VariableGroupDeleted, projectId: projectId, excludeParameters: (IEnumerable<string>) new string[1]
        {
          "@variables"
        });
        this.PrepareStoredProcedure("Task.prc_DeleteVariableGroup");
        this.BindDataspaceId(projectId);
        this.BindInt("@id", groupId);
        this.DataspaceRlsEnabled = false;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder2());
          return resultCollection.GetCurrent<VariableGroup>().Items.FirstOrDefault<VariableGroup>();
        }
      }
    }

    public override List<VariableGroup> GetVariableGroups(Guid projectId, IEnumerable<int> groupIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetVariableGroups)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVariableGroupsById");
        this.BindDataspaceId(projectId);
        this.BindUniqueInt32Table("@ids", groupIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder2());
          return resultCollection.GetCurrent<VariableGroup>().Items;
        }
      }
    }

    public override List<VariableGroup> GetVariableGroups(
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
        this.BindString("@name", groupName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder2());
          return resultCollection.GetCurrent<VariableGroup>().Items;
        }
      }
    }

    public override VariableGroup UpdateVariableGroup(Guid projectId, VariableGroup group)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateVariableGroup)))
      {
        this.PrepareForAuditingAction(VariableGroupAuditConstants.VariableGroupModified, projectId: projectId, excludeParameters: (IEnumerable<string>) new string[1]
        {
          "@variables"
        });
        this.PrepareStoredProcedure("Task.prc_UpdateVariableGroup");
        this.BindDataspaceId(projectId);
        this.BindInt("@id", group.Id);
        this.BindString("@type", group.Type, 256, false, SqlDbType.NVarChar);
        this.BindString("@name", group.Name, 256, false, SqlDbType.NVarChar);
        this.BindString("@description", group.Description, 1024, true, SqlDbType.NVarChar);
        this.BindString("@providerData", JsonUtility.ToString((object) group.ProviderData), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(group.ModifiedBy.Id));
        this.BindString("@variables", JsonUtility.ToString((object) VariableGroupUtility.ClearSecrets(group.Variables)), int.MaxValue, true, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder2());
          return resultCollection.GetCurrent<VariableGroup>().FirstOrDefault<VariableGroup>();
        }
      }
    }
  }
}
