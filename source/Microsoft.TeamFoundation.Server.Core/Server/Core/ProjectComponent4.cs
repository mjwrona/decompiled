// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent4
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent4 : ProjectComponent3
  {
    internal override ProjectInfo GetProject(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_ProjectGet");
      this.BindGuid("@projectId", projectId);
      ProjectInfo project;
      IList<ProjectProperty> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        resultCollection.AddBinder<ProjectProperty>((ObjectBinder<ProjectProperty>) new ProjectPropertyColumns());
        project = resultCollection.GetCurrent<ProjectInfo>().Items.FirstOrDefault<ProjectInfo>();
        resultCollection.NextResult();
        items = (IList<ProjectProperty>) resultCollection.GetCurrent<ProjectProperty>().Items;
      }
      if (project == null)
        throw new ProjectDoesNotExistException(projectId.ToString());
      project.Properties = items;
      return project;
    }

    internal override IList<ProjectInfo> GetProjects() => this.GetProjects((IEnumerable<Guid>) null);

    internal override IList<ProjectInfo> GetProjects(IEnumerable<Guid> projectIds)
    {
      this.PrepareStoredProcedure("prc_ProjectGetMultiple");
      this.BindGuidTable("@projectIds", projectIds);
      IList<ProjectInfo> items1;
      IList<Tuple<Guid, ProjectProperty>> items2;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        resultCollection.AddBinder<Tuple<Guid, ProjectProperty>>((ObjectBinder<Tuple<Guid, ProjectProperty>>) new ProjectIdPropertyColumns());
        items1 = (IList<ProjectInfo>) resultCollection.GetCurrent<ProjectInfo>().Items;
        resultCollection.NextResult();
        items2 = (IList<Tuple<Guid, ProjectProperty>>) resultCollection.GetCurrent<Tuple<Guid, ProjectProperty>>().Items;
      }
      IDictionary<Guid, IList<ProjectProperty>> dictionary = (IDictionary<Guid, IList<ProjectProperty>>) new Dictionary<Guid, IList<ProjectProperty>>(items1.Count);
      foreach (Tuple<Guid, ProjectProperty> tuple in (IEnumerable<Tuple<Guid, ProjectProperty>>) items2)
      {
        IList<ProjectProperty> projectPropertyList;
        if (dictionary.TryGetValue(tuple.Item1, out projectPropertyList))
        {
          projectPropertyList.Add(tuple.Item2);
        }
        else
        {
          projectPropertyList = (IList<ProjectProperty>) new List<ProjectProperty>()
          {
            tuple.Item2
          };
          dictionary[tuple.Item1] = projectPropertyList;
        }
      }
      foreach (ProjectInfo projectInfo in (IEnumerable<ProjectInfo>) items1)
      {
        IList<ProjectProperty> projectPropertyList;
        if (dictionary.TryGetValue(projectInfo.Id, out projectPropertyList))
          projectInfo.Properties = projectPropertyList;
      }
      return items1;
    }

    internal override ProjectOperation ReserveProject(
      Guid pendingProjectGuid,
      string projectName,
      Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_ProjectReserve");
      this.BindGuid("@pendingProjectId", pendingProjectGuid);
      this.BindProjectName("@projectName", projectName);
      this.BindGuid("@writerId", this.Author);
      this.ExecuteNonQuery();
      return (ProjectOperation) null;
    }

    internal override ProjectOperation DeleteReservedProject(
      Guid pendingProjectGuid,
      Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_ProjectDeleteReserved");
      this.BindGuid("@pendingProjectId", pendingProjectGuid);
      this.BindGuid("@writerId", this.Author);
      this.ExecuteNonQuery();
      return (ProjectOperation) null;
    }

    internal override ProjectInfo CreateProject(
      TeamFoundationIdentity requestingIdentity,
      Guid projectId,
      string projectName,
      Guid adminGroupId,
      string nodes,
      DateTime timeStamp,
      out int nodeSeqId)
    {
      throw new NotSupportedException();
    }

    internal override ProjectInfo CreateProject(
      TeamFoundationIdentity requestingIdentity,
      Guid projectId,
      string projectName,
      Guid adminGroupId,
      string nodes,
      DateTime timeStamp,
      Guid pendingProjectGuid,
      out int nodeSeqId)
    {
      throw new NotSupportedException();
    }

    internal override ProjectInfo CreateProject(
      TeamFoundationIdentity requestingIdentity,
      Guid projectId,
      string projectName,
      string projectAbbreviation,
      string nodes,
      Guid pendingProjectGuid,
      out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_ProjectCreateLegacy");
      this.BindGuid("@projectId", projectId);
      this.BindProjectName("@projectName", projectName);
      this.BindProjectAbbreviation("@projectAbbreviation", projectAbbreviation);
      this.BindString("@nodes", nodes, 0, false, SqlDbType.NVarChar);
      this.BindGuid("@userId", requestingIdentity.TeamFoundationId);
      this.BindGuid("@pendingProjectId", pendingProjectGuid);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SequenceIdColumns());
        nodeSeqId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
      }
      return new ProjectInfo(projectId, projectName, ProjectState.New, projectAbbreviation);
    }

    internal override ProjectOperation CreateProject(
      Guid projectId,
      string projectName,
      string projectAbbreviation,
      string projectDescription,
      Guid pendingProjectGuid,
      Guid userIdentity,
      ProjectVisibility projectVisibility)
    {
      this.PrepareStoredProcedure("prc_ProjectCreate");
      this.BindGuid("@projectId", projectId);
      this.BindProjectName("@projectName", projectName);
      this.BindProjectAbbreviation("@projectAbbreviation", projectAbbreviation);
      this.BindGuid("@pendingProjectId", pendingProjectGuid);
      this.BindGuid("@writerId", this.Author);
      this.ExecuteNonQuery();
      return (ProjectOperation) null;
    }

    internal override void DeleteProject(
      Guid projectId,
      string userName,
      DateTime timeStamp,
      out int seqId)
    {
      throw new NotSupportedException();
    }

    internal override void DeleteProject(Guid projectId, string userName, out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_ProjectDeleteLegacy");
      this.BindGuid("@projectId", projectId);
      this.BindString("@userName", userName, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SequenceIdColumns());
        nodeSeqId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
      }
      this.ExecuteNonQuery();
    }

    internal override ProjectOperation DeleteProject(Guid projectId, Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_ProjectDelete");
      this.BindGuid("@projectId", projectId);
      this.BindString("@userName", userIdentity.ToString(), 256, false, SqlDbType.NVarChar);
      this.BindGuid("@writerId", this.Author);
      this.ExecuteNonQuery();
      return (ProjectOperation) null;
    }

    internal override IList<ProjectProperty> GetProjectProperties(Guid projectId) => throw new NotSupportedException();

    internal override void SetProjectProperty(Guid projectId, string name, string value) => throw new NotSupportedException();

    internal override ProjectOperation UpdateProject(
      ProjectInfo project,
      Guid userIdentity,
      out ProjectInfo updatedProject,
      bool mergeProperties = false)
    {
      IList<KeyValuePair<string, string>> rows = (IList<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>();
      if (project.Properties != null)
      {
        foreach (ProjectProperty property in (IEnumerable<ProjectProperty>) project.Properties)
        {
          if (!TFStringComparer.CssProjectPropertyName.Equals("TemplateName", property.Name))
            rows.Add(new KeyValuePair<string, string>(property.Name, property.Value != null ? (string) property.Value : string.Empty));
        }
      }
      else
        mergeProperties = true;
      this.PrepareStoredProcedure("prc_ProjectUpdate");
      this.BindGuid("@projectId", project.Id);
      this.BindString("@projectName", project.Name != null ? DBPath.UserToDatabasePath(project.Name) : (string) null, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.BindString("@projectAbbreviation", project.Abbreviation != null ? DBPath.UserToDatabasePath(project.Abbreviation) : (string) null, 3, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@state", project.State == ProjectState.Unchanged ? (string) null : project.State.ToString(), 64, true, SqlDbType.NVarChar);
      this.BindKeyValuePairStringTable("@properties", (IEnumerable<KeyValuePair<string, string>>) rows);
      this.BindBoolean("@mergeProperties", mergeProperties);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        resultCollection.AddBinder<ProjectProperty>((ObjectBinder<ProjectProperty>) new ProjectPropertyColumns());
        updatedProject = resultCollection.GetCurrent<ProjectInfo>().Items.FirstOrDefault<ProjectInfo>();
        resultCollection.NextResult();
        updatedProject.Properties = (IList<ProjectProperty>) resultCollection.GetCurrent<ProjectProperty>().Items;
      }
      return (ProjectOperation) null;
    }

    protected SqlParameter BindProjectAbbreviation(string parameterName, string projectAbbreviation) => this.BindString(parameterName, projectAbbreviation != null ? DBPath.UserToDatabasePath(projectAbbreviation) : (string) null, 3, true, SqlDbType.NVarChar);
  }
}
