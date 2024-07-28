// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VariableGroupComponent6
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
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
  internal class VariableGroupComponent6 : VariableGroupComponent5
  {
    protected static SqlMetaData[] typ_VariableGroupProjectReference = new SqlMetaData[3]
    {
      new SqlMetaData("@dataspaceId", SqlDbType.Int),
      new SqlMetaData("@name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("@description", SqlDbType.NVarChar, 1024L)
    };

    public override void ShareVariableGroup(
      int groupId,
      IList<VariableGroupProjectReference> variableGroupProjectReferences)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (ShareVariableGroup)))
      {
        if (variableGroupProjectReferences != null && variableGroupProjectReferences.Count == 1)
        {
          VariableGroupProjectReference projectReference = variableGroupProjectReferences[0];
          if ((projectReference != null ? (projectReference.ProjectReference?.Id.HasValue ? 1 : 0) : 0) != 0 && variableGroupProjectReferences[0].ProjectReference.Id != Guid.Empty)
          {
            this.PrepareForAuditingAction(VariableGroupAuditConstants.VariableGroupCreated, projectId: variableGroupProjectReferences[0].ProjectReference.Id);
            goto label_5;
          }
        }
        this.PrepareForAuditingAction(VariableGroupAuditConstants.VariableGroupCreatedForProjects);
label_5:
        this.PrepareStoredProcedure("Task.prc_AddVariableGroupProjectReferences");
        this.BindInt("@groupId", groupId);
        this.BindTable("@variableGroupProjectReferences", "Task.typ_VariableGroupProjectReferenceTable", variableGroupProjectReferences != null ? variableGroupProjectReferences.Select<VariableGroupProjectReference, SqlDataRecord>((System.Func<VariableGroupProjectReference, SqlDataRecord>) (x => this.ConvertToSqlDataRecord(x))) : (IEnumerable<SqlDataRecord>) null);
        this.ExecuteNonQuery();
      }
    }

    public override VariableGroup GetVariableGroup(int groupId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetVariableGroup)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVariableGroupFromCollection");
        this.BindInt("@groupId", groupId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder2());
          resultCollection.AddBinder<VariableGroupProjectReference>((ObjectBinder<VariableGroupProjectReference>) new VariableGroupProjectReferenceBinder((VariableGroupComponent) this));
          VariableGroup variableGroup = resultCollection.GetCurrent<VariableGroup>().FirstOrDefault<VariableGroup>();
          resultCollection.NextResult();
          List<VariableGroupProjectReference> items = resultCollection.GetCurrent<VariableGroupProjectReference>().Items;
          this.StitchVariableGroupObject(variableGroup, items);
          return variableGroup;
        }
      }
    }

    public override VariableGroup AddVariableGroupCollection(VariableGroup group)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddVariableGroupCollection)))
      {
        if (group != null)
        {
          int? count = group.VariableGroupProjectReferences?.Count;
          int num = 1;
          if (count.GetValueOrDefault() == num & count.HasValue)
          {
            VariableGroupProjectReference projectReference = group.VariableGroupProjectReferences[0];
            if ((projectReference != null ? (projectReference.ProjectReference?.Id.HasValue ? 1 : 0) : 0) != 0 && group.VariableGroupProjectReferences[0].ProjectReference.Id != Guid.Empty)
            {
              this.PrepareForAuditingAction(VariableGroupAuditConstants.VariableGroupCreated, projectId: group.VariableGroupProjectReferences[0].ProjectReference.Id, excludeParameters: (IEnumerable<string>) new string[1]
              {
                "@variables"
              });
              goto label_6;
            }
          }
        }
        string createdForProjects = VariableGroupAuditConstants.VariableGroupCreatedForProjects;
        IEnumerable<string> strings = (IEnumerable<string>) new string[1]
        {
          "@variables"
        };
        Guid projectId = new Guid();
        IEnumerable<string> excludeParameters = strings;
        this.PrepareForAuditingAction(createdForProjects, projectId: projectId, excludeParameters: excludeParameters);
label_6:
        this.PrepareStoredProcedure("Task.prc_AddVariableGroupCollection");
        this.BindString("@type", group.Type, 256, false, SqlDbType.NVarChar);
        this.BindString("@name", group.Name, 256, false, SqlDbType.NVarChar);
        this.BindString("@description", group.Description, 1024, true, SqlDbType.NVarChar);
        this.BindString("@providerData", JsonUtility.ToString((object) group.ProviderData), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(group.CreatedBy.Id));
        this.BindString("@variables", JsonUtility.ToString((object) VariableGroupUtility.ClearSecrets(group.Variables)), int.MaxValue, true, SqlDbType.NVarChar);
        IList<VariableGroupProjectReference> projectReferences = group.VariableGroupProjectReferences;
        this.BindTable("@variableGroupProjectReferences", "Task.typ_VariableGroupProjectReferenceTable", projectReferences != null ? projectReferences.Select<VariableGroupProjectReference, SqlDataRecord>((System.Func<VariableGroupProjectReference, SqlDataRecord>) (x => this.ConvertToSqlDataRecord(x))) : (IEnumerable<SqlDataRecord>) null);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder2());
          resultCollection.AddBinder<VariableGroupProjectReference>((ObjectBinder<VariableGroupProjectReference>) new VariableGroupProjectReferenceBinder((VariableGroupComponent) this));
          VariableGroup variableGroup = resultCollection.GetCurrent<VariableGroup>().FirstOrDefault<VariableGroup>();
          resultCollection.NextResult();
          List<VariableGroupProjectReference> items = resultCollection.GetCurrent<VariableGroupProjectReference>().Items;
          this.StitchVariableGroupObject(variableGroup, items);
          return variableGroup;
        }
      }
    }

    public override VariableGroup UpdateVariableGroupCollection(
      VariableGroup group,
      bool isCollectionLevelVariableGroupChanged)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateVariableGroupCollection)))
      {
        if (group != null)
        {
          int? count = group.VariableGroupProjectReferences?.Count;
          int num = 1;
          if (count.GetValueOrDefault() == num & count.HasValue)
          {
            VariableGroupProjectReference projectReference = group.VariableGroupProjectReferences[0];
            if ((projectReference != null ? (projectReference.ProjectReference?.Id.HasValue ? 1 : 0) : 0) != 0 && group.VariableGroupProjectReferences[0].ProjectReference.Id != Guid.Empty)
            {
              this.PrepareForAuditingAction(VariableGroupAuditConstants.VariableGroupModified, projectId: group.VariableGroupProjectReferences[0].ProjectReference.Id, excludeParameters: (IEnumerable<string>) new string[1]
              {
                "@variables"
              });
              goto label_6;
            }
          }
        }
        string modifiedForProjects = VariableGroupAuditConstants.VariableGroupModifiedForProjects;
        IEnumerable<string> strings = (IEnumerable<string>) new string[1]
        {
          "@variables"
        };
        Guid projectId = new Guid();
        IEnumerable<string> excludeParameters = strings;
        this.PrepareForAuditingAction(modifiedForProjects, projectId: projectId, excludeParameters: excludeParameters);
label_6:
        this.PrepareStoredProcedure("Task.prc_UpdateVariableGroupCollection");
        this.BindInt("@id", group.Id);
        this.BindString("@type", group.Type, 256, false, SqlDbType.NVarChar);
        this.BindString("@providerData", JsonUtility.ToString((object) group.ProviderData), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(group.ModifiedBy.Id));
        this.BindString("@variables", JsonUtility.ToString((object) VariableGroupUtility.ClearSecrets(group.Variables)), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindBoolean("@isCollectionLevelVariableGroupChanged", isCollectionLevelVariableGroupChanged);
        IList<VariableGroupProjectReference> projectReferences = group.VariableGroupProjectReferences;
        this.BindTable("@variableGroupProjectReferences", "Task.typ_VariableGroupProjectReferenceTable", projectReferences != null ? projectReferences.Select<VariableGroupProjectReference, SqlDataRecord>((System.Func<VariableGroupProjectReference, SqlDataRecord>) (x => this.ConvertToSqlDataRecord(x))) : (IEnumerable<SqlDataRecord>) null);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroup>((ObjectBinder<VariableGroup>) new VariableGroupBinder2());
          resultCollection.AddBinder<VariableGroupProjectReference>((ObjectBinder<VariableGroupProjectReference>) new VariableGroupProjectReferenceBinder((VariableGroupComponent) this));
          VariableGroup variableGroup = resultCollection.GetCurrent<VariableGroup>().FirstOrDefault<VariableGroup>();
          resultCollection.NextResult();
          List<VariableGroupProjectReference> items = resultCollection.GetCurrent<VariableGroupProjectReference>().Items;
          this.StitchVariableGroupObject(variableGroup, items);
          return variableGroup;
        }
      }
    }

    public override IList<VariableGroupProjectReference> GetVariableGroupProjectReferences(
      int variableGroupId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetVariableGroupProjectReferences)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVariableGroupProjectReferences");
        this.BindInt("@groupId", variableGroupId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VariableGroupProjectReference>((ObjectBinder<VariableGroupProjectReference>) new VariableGroupProjectReferenceBinder((VariableGroupComponent) this));
          return (IList<VariableGroupProjectReference>) resultCollection.GetCurrent<VariableGroupProjectReference>().Items;
        }
      }
    }

    public override void DeleteVariableGroupFromProjects(int groupId, IList<Guid> projectIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteVariableGroupFromProjects)))
      {
        if (projectIds != null && projectIds.Count == 1 && projectIds[0] != Guid.Empty)
        {
          this.PrepareForAuditingAction(VariableGroupAuditConstants.VariableGroupDeleted, projectId: projectIds[0], excludeParameters: (IEnumerable<string>) new string[1]
          {
            "@variables"
          });
        }
        else
        {
          string deletedFromProjects = VariableGroupAuditConstants.VariableGroupDeletedFromProjects;
          IEnumerable<string> strings = (IEnumerable<string>) new string[1]
          {
            "@variables"
          };
          Guid projectId = new Guid();
          IEnumerable<string> excludeParameters = strings;
          this.PrepareForAuditingAction(deletedFromProjects, projectId: projectId, excludeParameters: excludeParameters);
        }
        this.PrepareStoredProcedure("Task.prc_DeleteVariableGroupFromProjects");
        List<int> rows = new List<int>();
        foreach (Guid projectId in (IEnumerable<Guid>) projectIds)
          rows.Add(this.GetDataspaceId(projectId, true));
        this.BindInt32Table("@dataspaceIds", (IEnumerable<int>) rows);
        this.BindInt("@groupId", groupId);
        this.DataspaceRlsEnabled = false;
        this.ExecuteNonQuery();
      }
    }

    protected SqlDataRecord ConvertToSqlDataRecord(
      VariableGroupProjectReference variableGroupProjectReference)
    {
      SqlDataRecord record = new SqlDataRecord(VariableGroupComponent6.typ_VariableGroupProjectReference);
      record.SetInt32(0, this.GetDataspaceId(variableGroupProjectReference.ProjectReference.Id, true));
      record.SetString(1, variableGroupProjectReference.Name);
      record.SetNullableString(2, variableGroupProjectReference.Description);
      return record;
    }

    protected void StitchVariableGroupObject(
      VariableGroup variableGroup,
      List<VariableGroupProjectReference> variableGroupProjectReferences)
    {
      if (variableGroup == null)
        return;
      variableGroup.VariableGroupProjectReferences = (IList<VariableGroupProjectReference>) variableGroupProjectReferences;
      if (variableGroupProjectReferences.Count<VariableGroupProjectReference>() <= 1)
        return;
      variableGroup.IsShared = true;
    }
  }
}
