// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard.TaskboardHubDispatcher
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.AspNet.SignalR;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard
{
  public class TaskboardHubDispatcher : ITaskboardHubDispatcher, IVssFrameworkService
  {
    private IHubContext<ITaskboardHubClient> m_hubContext;

    public async Task WatchTaskboard(
      IVssRequestContext requestContext,
      Guid teamId,
      string connectionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      ArgumentUtility.CheckForNull<string>(connectionId, nameof (connectionId));
      foreach (string namesByTeamField in TaskboardHubDispatcher.GetGroupNamesByTeamFields(requestContext, TaskboardHubDispatcher.GetTeamFieldValues(requestContext, teamId)))
        await this.m_hubContext.Groups.Add(connectionId, namesByTeamField);
      await this.m_hubContext.Groups.Add(connectionId, teamId.ToString());
    }

    public async Task UnwatchTaskboard(
      IVssRequestContext requestContext,
      Guid teamId,
      string connectionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      ArgumentUtility.CheckForNull<string>(connectionId, nameof (connectionId));
      foreach (string namesByTeamField in TaskboardHubDispatcher.GetGroupNamesByTeamFields(requestContext, TaskboardHubDispatcher.GetTeamFieldValues(requestContext, teamId)))
        await this.m_hubContext.Groups.Remove(connectionId, namesByTeamField);
      await this.m_hubContext.Groups.Remove(connectionId, teamId.ToString());
    }

    public void NotifyWorkItemChanged(
      IVssRequestContext requestContext,
      WorkItemChangedEvent workItemChangedEvent)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, nameof (workItemChangedEvent));
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "NotifyWorkItemColumnChanged"))
      {
        try
        {
          (int ParentId, ParentChangeType ChangeStatus) parentChangedStatus = WorkItemChangedHelper.GetParentChangedStatus(workItemChangedEvent);
          if (workItemChangedEvent.HasOnlyLinkUpdates && parentChangedStatus.ChangeStatus == ParentChangeType.Unchanged && workItemChangedEvent.WorkItemComment == null)
            return;
          Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, workItemChangedEvent.ProjectId);
          Guid projectId = workItemChangedEvent.ProjectId;
          string workItemType = workItemChangedEvent.WorkItemType;
          Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
          ProjectProcessConfiguration projectProcessSettings = requestContext.GetProjectProcessSettings(project, true);
          BacklogLevelConfiguration backlogLevel;
          if (!(projectProcessSettings.BugWorkItems == null ? new HashSet<string>() : new HashSet<string>(projectProcessSettings.BugWorkItems.GetWorkItemTypes(requestContext, projectId), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)).Contains(workItemType) && (!backlogConfiguration.TryGetBacklogByWorkItemTypeName(workItemType, out backlogLevel) || !backlogLevel.IsRequirementsBacklog && !backlogLevel.IsTaskBacklog))
            return;
          string teamFieldRefName = backlogConfiguration.BacklogFields.TypeFields[FieldTypeEnum.Team];
          string teamFieldValue = (string) null;
          using (PerformanceTimer.StartMeasure(requestContext, "GetTeamFieldValue"))
            teamFieldValue = WorkItemChangedHelper.GetCoreOrCustomFieldValue(requestContext, workItemChangedEvent, teamFieldRefName);
          if (teamFieldValue == null)
            return;
          ChangeTypes changeType1 = WorkItemChangedHelper.GetChangeType(workItemChangedEvent);
          int intField1 = WorkItemChangedHelper.GetIntField(workItemChangedEvent, "System.Id");
          int intField2 = WorkItemChangedHelper.GetIntField(workItemChangedEvent, "System.Rev");
          string associatedWithTheChange = WorkItemChangedHelper.GetStackRankAssociatedWithTheChange(workItemChangedEvent);
          string stringField1 = WorkItemChangedHelper.GetStringField(workItemChangedEvent, "System.IterationPath");
          IList<string> stringList = this.GetGroupNames(requestContext, teamFieldValue);
          ChangedFieldsType changedFields = workItemChangedEvent.ChangedFields;
          StringField stringField2;
          if (changedFields == null)
          {
            stringField2 = (StringField) null;
          }
          else
          {
            StringField[] stringFields = changedFields.StringFields;
            stringField2 = stringFields != null ? ((IEnumerable<StringField>) stringFields).FirstOrDefault<StringField>((Func<StringField, bool>) (fld => fld.ReferenceName.Equals(teamFieldRefName, StringComparison.Ordinal))) : (StringField) null;
          }
          StringField stringField3 = stringField2;
          if (stringField3 != null)
            stringList = stringList.AddRange<string, IList<string>>((IEnumerable<string>) this.GetGroupNames(requestContext, stringField3.OldValue));
          performanceTimer.AddProperty("GroupNameCount", (object) stringList.Count);
          int revision = intField2;
          int changeType2 = (int) changeType1;
          string stackRank = associatedWithTheChange;
          string iterationPath = stringField1;
          int parentId = parentChangedStatus.ParentId;
          int changeStatus = (int) parentChangedStatus.ChangeStatus;
          TaskboardWorkItemUpdateEvent workItemChangedEvent1 = new TaskboardWorkItemUpdateEvent(intField1, revision, (ChangeTypes) changeType2, stackRank, iterationPath, parentId, (ParentChangeType) changeStatus);
          this.m_hubContext.Clients.Groups(stringList).OnWorkItemUpdated(requestContext, workItemChangedEvent1);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(90002002, "Agile", TfsTraceLayers.BusinessLogic, ex);
        }
      }
      PerformanceTimer.SendCustomerIntelligenceData(requestContext);
    }

    public void NotifyWorkItemColumnChanged(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumnChangedEvent workItemColumnChangedEvent)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumnChangedEvent>(workItemColumnChangedEvent, nameof (workItemColumnChangedEvent));
      this.m_hubContext.Clients.Group(workItemColumnChangedEvent.TeamId.ToString()).OnWorkItemColumnUpdated(requestContext, new TaskboardWorkItemColumnChangedEvent()
      {
        ColumnId = workItemColumnChangedEvent.ColumnId,
        TeamId = workItemColumnChangedEvent.TeamId,
        ProjectId = workItemColumnChangedEvent.ProjectId,
        WorkItemId = workItemColumnChangedEvent.WorkItemId
      });
    }

    public void NotifyColumnOptionsChanged(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      this.m_hubContext.Clients.Group(teamId.ToString()).OnTaskboardColumnOptionsChanged(requestContext);
    }

    public void NotifyTeamSettingsChanged(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Guid> teamIds)
    {
      this.m_hubContext.Clients.Groups((IList<string>) teamIds.Select<Guid, string>((Func<Guid, string>) (t => t.ToString())).ToList<string>()).OnTeamSettingsChanged(requestContext);
    }

    public void NotifyTaskboardCardSettingsChanged(IVssRequestContext requestContext, Guid teamId) => this.m_hubContext.Clients.Group(teamId.ToString()).OnTaskboardCardSettingsChanged(requestContext);

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<TaskboardHub, ITaskboardHubClient>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => this.m_hubContext = (IHubContext<ITaskboardHubClient>) null;

    private IList<string> GetGroupNames(IVssRequestContext requestContext, string teamFieldValue)
    {
      List<string> groupNames = new List<string>();
      if (string.IsNullOrWhiteSpace(teamFieldValue))
        return (IList<string>) groupNames;
      groupNames.Add(TaskboardHubDispatcher.GetGroupNameByTeamFieldValue(requestContext, teamFieldValue));
      string[] strArray = teamFieldValue.Split('\\');
      bool flag = teamFieldValue.StartsWith("\\");
      string str = string.Format("{0}_", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId);
      for (int index = 0; index < strArray.Length - 1; ++index)
      {
        if (!flag || index != 0)
        {
          str = str + "\\" + strArray[index];
          groupNames.Add(str + "\\*");
        }
      }
      return (IList<string>) groupNames;
    }

    private static IEnumerable<string> GetGroupNamesByTeamFields(
      IVssRequestContext requestContext,
      ITeamFieldValue[] teamFieldValues)
    {
      List<string> list = ((IEnumerable<ITeamFieldValue>) teamFieldValues).Select<ITeamFieldValue, string>((Func<ITeamFieldValue, string>) (teamFieldValue => TaskboardHubDispatcher.GetGroupNameByTeamFieldValue(requestContext, teamFieldValue.Value))).ToList<string>();
      list.AddRange(((IEnumerable<ITeamFieldValue>) teamFieldValues).Where<ITeamFieldValue>((Func<ITeamFieldValue, bool>) (teamFieldValue => teamFieldValue.IncludeChildren)).Select<ITeamFieldValue, string>((Func<ITeamFieldValue, string>) (teamFieldValue => TaskboardHubDispatcher.GetGroupNameByTeamFieldValueIncludingChildren(requestContext, teamFieldValue.Value))));
      return (IEnumerable<string>) list;
    }

    private static string GetGroupNameByTeamFieldValueIncludingChildren(
      IVssRequestContext requestContext,
      string teamFieldValue)
    {
      string str = teamFieldValue.StartsWith("\\") ? teamFieldValue + "\\*" : "\\" + teamFieldValue + "\\*";
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) str);
    }

    private static string GetGroupNameByTeamFieldValue(
      IVssRequestContext requestContext,
      string teamFieldValue)
    {
      string str = teamFieldValue.StartsWith("\\") ? teamFieldValue ?? "" : "\\" + teamFieldValue;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) str);
    }

    private static ITeamFieldValue[] GetTeamFieldValues(
      IVssRequestContext requestContext,
      Guid teamId)
    {
      ITeamService service1 = requestContext.GetService<ITeamService>();
      ITeamConfigurationService service2 = requestContext.GetService<ITeamConfigurationService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid teamGuid = teamId;
      WebApiTeam teamByGuid = service1.GetTeamByGuid(requestContext1, teamGuid);
      return service2.GetTeamSettings(requestContext, teamByGuid, true, false).TeamFieldConfig.TeamFieldValues;
    }
  }
}
