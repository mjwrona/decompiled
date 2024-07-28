// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent7
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent7 : ProjectComponent6
  {
    internal override ProjectInfo GetProject(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_ProjectGet");
      this.BindGuid("@projectId", projectId);
      ProjectInfo project;
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        project = this.GetProject(rc);
      return project != null ? project : throw new ProjectDoesNotExistException(projectId.ToString());
    }

    internal override IList<ProjectInfo> GetProjects(IEnumerable<Guid> projectIds)
    {
      this.PrepareStoredProcedure("prc_ProjectGetMultiple");
      this.BindGuidTable("@projectIds", projectIds);
      IList<ProjectInfo> items;
      IList<Tuple<Guid, ProjectProperty>> propertiesForProjects;
      IList<Tuple<Guid, string>> list;
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        this.AddProjectIdPropertyColumnsBinder(rc);
        rc.AddBinder<Tuple<Guid, string>>((ObjectBinder<Tuple<Guid, string>>) new ProjectIdNameColumns());
        items = (IList<ProjectInfo>) rc.GetCurrent<ProjectInfo>().Items;
        propertiesForProjects = (IList<Tuple<Guid, ProjectProperty>>) this.GetPropertiesForProjects(rc);
        rc.NextResult();
        list = (IList<Tuple<Guid, string>>) rc.GetCurrent<Tuple<Guid, string>>().Items.Where<Tuple<Guid, string>>((System.Func<Tuple<Guid, string>, bool>) (item => item.Item2 != null)).ToList<Tuple<Guid, string>>();
      }
      Dictionary<Guid, List<ProjectProperty>> dictionary1 = propertiesForProjects.GroupBy<Tuple<Guid, ProjectProperty>, Guid, ProjectProperty>((System.Func<Tuple<Guid, ProjectProperty>, Guid>) (property => property.Item1), (System.Func<Tuple<Guid, ProjectProperty>, ProjectProperty>) (property => property.Item2)).ToDictionary<IGrouping<Guid, ProjectProperty>, Guid, List<ProjectProperty>>((System.Func<IGrouping<Guid, ProjectProperty>, Guid>) (group => group.Key), (System.Func<IGrouping<Guid, ProjectProperty>, List<ProjectProperty>>) (group => group.ToList<ProjectProperty>()));
      Dictionary<Guid, List<string>> dictionary2 = list.GroupBy<Tuple<Guid, string>, Guid, string>((System.Func<Tuple<Guid, string>, Guid>) (name => name.Item1), (System.Func<Tuple<Guid, string>, string>) (name => name.Item2)).ToDictionary<IGrouping<Guid, string>, Guid, List<string>>((System.Func<IGrouping<Guid, string>, Guid>) (group => group.Key), (System.Func<IGrouping<Guid, string>, List<string>>) (group => group.ToList<string>()));
      foreach (ProjectInfo project in (IEnumerable<ProjectInfo>) items)
      {
        List<ProjectProperty> projectPropertyList;
        if (dictionary1.TryGetValue(project.Id, out projectPropertyList))
          project.Properties = (IList<ProjectProperty>) projectPropertyList;
        List<string> stringList;
        if (dictionary2.TryGetValue(project.Id, out stringList))
        {
          project.KnownNames = (IList<string>) stringList;
          this.AddCurrentNameToKnownNames(project);
        }
      }
      return items;
    }

    internal override ProjectOperation ReserveProject(
      Guid projectId,
      string projectName,
      Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_ProjectCreate");
      this.BindGuid("@projectId", projectId);
      this.BindProjectName("@projectName", projectName);
      this.BindProjectAbbreviation("@projectAbbreviation", (string) null);
      this.BindString("@projectState", "CreatePending", 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullValue("@pendingProjectId", SqlDbType.UniqueIdentifier);
      this.BindGuid("@userId", userIdentity);
      this.BindGuid("@writerId", this.Author);
      this.BindDescription("@projectDescription", (string) null);
      this.BindProjectVisibility("@projectVisibility", new byte?());
      this.ExecuteNonQuery();
      return (ProjectOperation) null;
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
      this.BindString("@projectState", (string) null, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@pendingProjectId", pendingProjectGuid);
      this.BindGuid("@userId", userIdentity);
      this.BindGuid("@writerId", this.Author);
      this.BindProjectVisibility("@projectVisibility", new byte?((byte) projectVisibility));
      this.ExecuteNonQuery();
      return (ProjectOperation) null;
    }

    internal override ProjectOperation UpdateProject(
      ProjectInfo project,
      Guid userIdentity,
      out ProjectInfo updatedProject,
      bool mergeProperties = false)
    {
      this.PrepareUpdate(project, userIdentity, mergeProperties);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        updatedProject = this.GetProject(rc);
      return (ProjectOperation) null;
    }

    protected void PrepareUpdate(ProjectInfo project, Guid userIdentity, bool mergeProperties)
    {
      this.PrepareStoredProcedure("prc_ProjectUpdate");
      this.BindGuid("@projectId", project.Id);
      this.BindString("@projectName", project.Name != null ? DBPath.UserToDatabasePath(project.Name) : (string) null, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@projectAbbreviation", project.Abbreviation != null ? DBPath.UserToDatabasePath(project.Abbreviation) : (string) null, 3, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@state", project.State == ProjectState.Unchanged ? (string) null : project.State.ToString(), 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindProperties(project);
      this.BindMergeProperties(mergeProperties);
      this.BindGuid("@userId", userIdentity);
      this.BindGuid("@writerId", this.Author);
      this.BindDescription("@projectDescription", project.Description);
      this.BindProjectVisibility("@projectVisibility", project.Visibility == ProjectVisibility.Unchanged ? new byte?() : new byte?((byte) project.Visibility));
    }

    internal override ProjectOperation DeleteReservedProject(
      Guid pendingProjectGuid,
      Guid userIdentity)
    {
      this.DeleteProject("prc_ProjectDeleteReserved", pendingProjectGuid, userIdentity);
      return (ProjectOperation) null;
    }

    internal override ProjectOperation DeleteProject(Guid projectId, Guid userIdentity)
    {
      this.DeleteProject("prc_ProjectDelete", projectId, userIdentity);
      return (ProjectOperation) null;
    }

    internal override List<ProjectInfo> GetProjectHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      long minRevision = 0)
    {
      this.PrepareStoredProcedure("prc_ProjectGetHistory");
      this.BindGuid("@projectId", projectId);
      this.BindMinRevision("@minRevision", minRevision);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        this.AddProjectPropertyColumnsBinder(rc);
        rc.AddBinder<ProjectInfo>(this.CreateProjectHistoryColumnsBinder());
        ProjectInfo projectInfo = rc.GetCurrent<ProjectInfo>().Items.FirstOrDefault<ProjectInfo>();
        List<ProjectProperty> propertiesForProject = this.GetPropertiesForProject(rc);
        rc.NextResult();
        List<ProjectInfo> items = rc.GetCurrent<ProjectInfo>().Items;
        if (projectInfo != null)
        {
          projectInfo.Properties = (IList<ProjectProperty>) propertiesForProject;
          items.Add(projectInfo);
        }
        this.ResolveProjectPropertyDeltas(requestContext, (IList<ProjectInfo>) items);
        return items;
      }
    }

    protected virtual void ResolveProjectPropertyDeltas(
      IVssRequestContext requestContext,
      IList<ProjectInfo> projectHistory)
    {
      if (projectHistory.Count <= 1)
        return;
      Dictionary<string, object> dictionary = projectHistory.Last<ProjectInfo>().Properties.ToDictionary<ProjectProperty, string, object>((System.Func<ProjectProperty, string>) (item => item.Name), (System.Func<ProjectProperty, object>) (item => item.Value));
      for (int index = projectHistory.Count - 2; index >= 0; --index)
      {
        foreach (ProjectProperty property in (IEnumerable<ProjectProperty>) projectHistory[index].Properties)
        {
          if (property.Value == null)
            dictionary.Remove(property.Name);
          else
            dictionary[property.Name] = property.Value;
        }
        projectHistory[index].Properties = (IList<ProjectProperty>) dictionary.Select<KeyValuePair<string, object>, ProjectProperty>((System.Func<KeyValuePair<string, object>, ProjectProperty>) (item => new ProjectProperty(item.Key, item.Value))).ToList<ProjectProperty>();
      }
    }

    internal override void GetChangedProjects(
      IVssRequestContext requestContext,
      long modifiedRevision,
      long deletedRevision,
      out IList<ProjectInfo> modifiedProjects,
      out IList<ProjectInfo> deletedProjects)
    {
      this.PrepareStoredProcedure("prc_ProjectGetChanged");
      this.BindModifiedRevision("@modifiedRevision", modifiedRevision);
      this.BindDeletedRevision("@deletedRevision", deletedRevision);
      IList<ProjectInfo> items;
      IList<Tuple<Guid, ProjectProperty>> propertiesForProjects;
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        this.AddProjectIdPropertyColumnsBinder(rc);
        rc.AddBinder<ProjectInfo>(this.CreateProjectHistoryColumnsBinder());
        items = (IList<ProjectInfo>) rc.GetCurrent<ProjectInfo>().Items;
        propertiesForProjects = (IList<Tuple<Guid, ProjectProperty>>) this.GetPropertiesForProjects(rc);
        rc.NextResult();
        deletedProjects = (IList<ProjectInfo>) rc.GetCurrent<ProjectInfo>().Items;
      }
      Dictionary<Guid, List<ProjectProperty>> dictionary = propertiesForProjects.GroupBy<Tuple<Guid, ProjectProperty>, Guid, ProjectProperty>((System.Func<Tuple<Guid, ProjectProperty>, Guid>) (property => property.Item1), (System.Func<Tuple<Guid, ProjectProperty>, ProjectProperty>) (property => property.Item2)).ToDictionary<IGrouping<Guid, ProjectProperty>, Guid, List<ProjectProperty>>((System.Func<IGrouping<Guid, ProjectProperty>, Guid>) (group => group.Key), (System.Func<IGrouping<Guid, ProjectProperty>, List<ProjectProperty>>) (group => group.ToList<ProjectProperty>()));
      foreach (ProjectInfo projectInfo in (IEnumerable<ProjectInfo>) items)
      {
        List<ProjectProperty> projectPropertyList;
        if (dictionary.TryGetValue(projectInfo.Id, out projectPropertyList))
          projectInfo.Properties = (IList<ProjectProperty>) projectPropertyList;
      }
      modifiedProjects = items;
    }

    private void DeleteProject(string procName, Guid projectId, Guid userIdentity)
    {
      this.PrepareStoredProcedure(procName);
      this.BindGuid("@projectId", projectId);
      this.BindGuid("@userId", userIdentity);
      this.BindGuid("@writerId", this.Author);
      this.ExecuteNonQuery();
    }

    protected ProjectInfo GetProject(ResultCollection rc)
    {
      rc.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
      this.AddProjectPropertyColumnsBinder(rc);
      rc.AddBinder<string>((ObjectBinder<string>) new ProjectNameColumns());
      ProjectInfo project = rc.GetCurrent<ProjectInfo>().FirstOrDefault<ProjectInfo>();
      if (project == null)
        return (ProjectInfo) null;
      project.Properties = (IList<ProjectProperty>) this.GetPropertiesForProject(rc);
      rc.NextResult();
      project.KnownNames = (IList<string>) rc.GetCurrent<string>().Where<string>((System.Func<string, bool>) (item => item != null)).ToList<string>();
      this.AddCurrentNameToKnownNames(project);
      return project;
    }

    private void AddCurrentNameToKnownNames(ProjectInfo project) => project.KnownNames.Add(project.Name);

    protected virtual void BindDescription(string parameterName, string projectDescription)
    {
    }

    protected virtual void BindMinRevision(string paramName, long revision)
    {
    }

    protected virtual void BindModifiedRevision(string paramName, long revision) => this.BindBinary(paramName, BitConverter.GetBytes(IPAddress.HostToNetworkOrder(revision)), SqlDbType.Timestamp);

    protected virtual void BindDeletedRevision(string paramName, long revision) => this.BindBinary(paramName, BitConverter.GetBytes(IPAddress.HostToNetworkOrder(revision)), SqlDbType.Timestamp);

    protected virtual void AddProjectIdPropertyColumnsBinder(ResultCollection rc) => rc.AddBinder<Tuple<Guid, ProjectProperty>>((ObjectBinder<Tuple<Guid, ProjectProperty>>) new ProjectIdPropertyColumns());

    protected virtual void AddProjectPropertyColumnsBinder(ResultCollection rc) => rc.AddBinder<ProjectProperty>((ObjectBinder<ProjectProperty>) new ProjectPropertyColumns());

    protected virtual List<Tuple<Guid, ProjectProperty>> GetPropertiesForProjects(
      ResultCollection rc)
    {
      rc.NextResult();
      return rc.GetCurrent<Tuple<Guid, ProjectProperty>>().Items;
    }

    protected virtual List<ProjectProperty> GetPropertiesForProject(ResultCollection rc)
    {
      rc.NextResult();
      return rc.GetCurrent<ProjectProperty>().Items;
    }

    protected virtual void BindProperties(ProjectInfo project)
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
      this.BindKeyValuePairStringTable("@properties", (IEnumerable<KeyValuePair<string, string>>) rows);
    }

    protected virtual void BindMergeProperties(bool mergeProperties) => this.BindBoolean("@mergeProperties", mergeProperties);

    protected virtual void BindProjectVisibility(string parameterName, byte? projectVisibility)
    {
    }
  }
}
