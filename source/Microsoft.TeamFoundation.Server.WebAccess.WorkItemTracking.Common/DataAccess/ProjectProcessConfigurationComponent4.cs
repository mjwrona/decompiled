// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.ProjectProcessConfigurationComponent4
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class ProjectProcessConfigurationComponent4 : ProjectProcessConfigurationComponent3
  {
    protected override void BindDataspaceIdOrProjectId(Guid dataspaceIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));

    internal override IEnumerable<ProjectCategoryStateMap> GetCategoryStates(
      WorkItemTypeEnum categoryType)
    {
      this.PrepareStoredProcedure("prc_GetStateMapForCategory");
      this.BindInt("@workItemCategoryType", (int) categoryType);
      List<ProjectProcessConfigurationComponent4.ProjectWorkItemStateRow> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectProcessConfigurationComponent4.ProjectWorkItemStateRow>((ObjectBinder<ProjectProcessConfigurationComponent4.ProjectWorkItemStateRow>) new ProjectProcessConfigurationComponent4.ProjectWorkItemStateRowBinder());
        items = resultCollection.GetCurrent<ProjectProcessConfigurationComponent4.ProjectWorkItemStateRow>().Items;
      }
      Dictionary<Guid, ProjectCategoryStateMap> dictionary = new Dictionary<Guid, ProjectCategoryStateMap>();
      foreach (ProjectProcessConfigurationComponent4.ProjectWorkItemStateRow workItemStateRow in items)
      {
        ProjectCategoryStateMap categoryStateMap = (ProjectCategoryStateMap) null;
        Guid dataspaceIdentifier = this.GetDataspaceIdentifier(workItemStateRow.DataspaceId);
        if (!dictionary.TryGetValue(dataspaceIdentifier, out categoryStateMap))
        {
          categoryStateMap = new ProjectCategoryStateMap()
          {
            ProjectId = dataspaceIdentifier,
            CategoryReferenceName = workItemStateRow.CategoryRefName,
            States = (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>) new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>()
          };
          dictionary[dataspaceIdentifier] = categoryStateMap;
        }
        (categoryStateMap.States as List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>).Add(new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State()
        {
          Type = workItemStateRow.StateType,
          Value = workItemStateRow.StateValue
        });
      }
      return (IEnumerable<ProjectCategoryStateMap>) dictionary.Values;
    }

    internal override IDictionary<Guid, string> GetTeamFieldsForProjects(
      IEnumerable<Guid> projectIds)
    {
      string sqlStatement = "\r\nSELECT DISTINCT f.DataspaceId, f.FieldRefName\r\nFROM @dataspaceIdTable d\r\nINNER JOIN tbl_ProjectConfigurationTypeFields f\r\nON f.DataspaceId = d.Val\r\n    AND f.PartitionId = @partitionId\r\n    AND f.FieldType = 4\r\n";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      return this.GetTeamFieldsForProjectsInternal(projectIds);
    }

    protected IDictionary<Guid, string> GetTeamFieldsForProjectsInternal(
      IEnumerable<Guid> projectIds)
    {
      this.BindInt32Table("@dataspaceIdTable", (IEnumerable<int>) this.GetDataspaceIds(projectIds.Distinct<Guid>().ToArray<Guid>()));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectProcessConfigurationComponent4.ProjectTeamFieldRow>((ObjectBinder<ProjectProcessConfigurationComponent4.ProjectTeamFieldRow>) new ProjectProcessConfigurationComponent4.ProjectTeamFieldRowBinder());
        return (IDictionary<Guid, string>) resultCollection.GetCurrent<ProjectProcessConfigurationComponent4.ProjectTeamFieldRow>().ToDictionary<ProjectProcessConfigurationComponent4.ProjectTeamFieldRow, Guid, string>((System.Func<ProjectProcessConfigurationComponent4.ProjectTeamFieldRow, Guid>) (row => this.GetDataspaceIdentifier(row.DataspaceId)), (System.Func<ProjectProcessConfigurationComponent4.ProjectTeamFieldRow, string>) (row => row.TeamField));
      }
    }

    protected class ProjectWorkItemStateRow : ProjectProcessConfigurationComponent2.WorkItemStateRow
    {
      public int DataspaceId { get; set; }
    }

    protected class ProjectWorkItemStateRowBinder : 
      ObjectBinder<ProjectProcessConfigurationComponent4.ProjectWorkItemStateRow>
    {
      private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder WorkItemTypeColumn = new SqlColumnBinder("WorkItemType");
      private SqlColumnBinder StateTypeColumn = new SqlColumnBinder("StateType");
      private SqlColumnBinder StateValueColumn = new SqlColumnBinder("StateValue");
      private SqlColumnBinder CategoryRefNameColumn = new SqlColumnBinder("CategoryRefName");

      protected override ProjectProcessConfigurationComponent4.ProjectWorkItemStateRow Bind()
      {
        ProjectProcessConfigurationComponent4.ProjectWorkItemStateRow workItemStateRow = new ProjectProcessConfigurationComponent4.ProjectWorkItemStateRow();
        workItemStateRow.DataspaceId = this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader);
        workItemStateRow.WorkItemType = (WorkItemTypeEnum) this.WorkItemTypeColumn.GetByte((IDataReader) this.Reader);
        workItemStateRow.StateType = (StateTypeEnum) this.StateTypeColumn.GetByte((IDataReader) this.Reader);
        workItemStateRow.StateValue = this.StateValueColumn.GetString((IDataReader) this.Reader, false);
        workItemStateRow.CategoryRefName = this.CategoryRefNameColumn.GetString((IDataReader) this.Reader, false);
        return workItemStateRow;
      }
    }

    protected class ProjectTeamFieldRow
    {
      public int DataspaceId { get; set; }

      public string TeamField { get; set; }
    }

    protected class ProjectTeamFieldRowBinder : 
      ObjectBinder<ProjectProcessConfigurationComponent4.ProjectTeamFieldRow>
    {
      private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");
      private SqlColumnBinder TeamFieldColumn = new SqlColumnBinder("FieldRefName");

      protected override ProjectProcessConfigurationComponent4.ProjectTeamFieldRow Bind() => new ProjectProcessConfigurationComponent4.ProjectTeamFieldRow()
      {
        DataspaceId = this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader),
        TeamField = this.TeamFieldColumn.GetString((IDataReader) this.Reader, true)
      };
    }
  }
}
