// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MetaTaskService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Server.Exceptions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class MetaTaskService : IMetaTaskService, IVssFrameworkService
  {
    private readonly Func<IVssRequestContext, List<TaskGroup>> getTaskGroupsFromExtensions;
    private readonly ISecurityProvider m_securityProvider;
    private const string c_layer = "MetaTaskService";

    internal MetaTaskService()
      : this((ISecurityProvider) new DefaultSecurityProvider(), MetaTaskService.\u003C\u003EO.\u003C0\u003E__GetAllTaskGroupTemplates ?? (MetaTaskService.\u003C\u003EO.\u003C0\u003E__GetAllTaskGroupTemplates = new Func<IVssRequestContext, List<TaskGroup>>(TaskGroupExtensionsRetriever.GetAllTaskGroupTemplates)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    internal MetaTaskService(
      ISecurityProvider security,
      Func<IVssRequestContext, List<TaskGroup>> getTaskGroupsFromExtensions)
    {
      this.m_securityProvider = security;
      this.getTaskGroupsFromExtensions = getTaskGroupsFromExtensions;
    }

    public TaskGroup AddTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroupCreateParameter taskGroupCreateParameter)
    {
      if (requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, "ms.vss-build-web.disable-classic-release-pipeline-creation"))
        throw new InvalidOperationException(TaskResources.ClassicPipelinesDisabled());
      TaskGroup taskGroup1 = taskGroupCreateParameter != null ? taskGroupCreateParameter.ToTaskGroup() : throw new InvalidRequestException(TaskResources.InvalidTaskGroupInput());
      using (new MethodScope(requestContext, nameof (MetaTaskService), nameof (AddTaskGroup)))
      {
        this.m_securityProvider.CheckMetaTaskPermission(requestContext, projectId, Guid.Empty, taskGroupCreateParameter.ParentDefinitionId, 2);
        this.m_securityProvider.CheckMetaTaskEndpointSecurity(requestContext, projectId, taskGroup1);
        taskGroup1.FixLatestMajorTaskVersions(requestContext);
        taskGroup1.Validate(requestContext, projectId);
        taskGroup1.Id = Guid.NewGuid();
        if (taskGroup1.Version != (TaskVersion) null && !taskGroup1.Version.IsTest)
          taskGroup1.ParentDefinitionId = new Guid?();
        taskGroup1.CreatedBy = requestContext.GetRequesterIdentity();
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo(10015173, "DistributedTask", "Publishing TaskGroupChangingEvent decision point: Adding task group {0}", (object) taskGroup1.Id);
        service.PublishDecisionPoint(requestContext, (object) new TaskGroupChangingEvent(projectId, AuditAction.Add, taskGroup1));
        MetaTaskDefinitionData taskDefinitionData;
        using (MetaTaskDefinitionComponent component = requestContext.CreateComponent<MetaTaskDefinitionComponent>())
          taskDefinitionData = component.AddMetaTaskDefinition(projectId, taskGroup1);
        if (taskDefinitionData != null)
        {
          TaskGroup taskGroup2 = taskDefinitionData.GetDefinition().ResolveIdentityRefs(requestContext);
          this.m_securityProvider.GrantAdministratorPermissionToTaskGroup(requestContext, projectId, taskGroup2.Id, taskGroup2.ParentDefinitionId);
          string comment = taskGroup1.Revision == 1 ? taskGroup1.Description : taskGroup1.Comment;
          this.SaveRevision(requestContext, projectId, taskGroup2, comment, AuditAction.Add);
          return taskGroup2;
        }
      }
      return taskGroup1;
    }

    public bool SoftDeleteTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string comment)
    {
      using (new MethodScope(requestContext, nameof (MetaTaskService), nameof (SoftDeleteTaskGroup)))
      {
        Guid? definitionIdIfAvailable = this.GetParentDefinitionIdIfAvailable(requestContext, projectId, taskGroupId);
        this.m_securityProvider.CheckMetaTaskPermission(requestContext, projectId, taskGroupId, definitionIdIfAvailable, 4);
        Guid userId = requestContext.GetUserId();
        using (MetaTaskDefinitionComponent component = requestContext.CreateComponent<MetaTaskDefinitionComponent>())
          component.SoftDeleteTaskGroup(projectId, taskGroupId, userId, comment);
      }
      return true;
    }

    private Guid? GetParentDefinitionIdIfAvailable(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId)
    {
      List<TaskGroup> taskGroups = this.GetTaskGroups(requestContext, projectId, new Guid?(taskGroupId), new bool?(false), new Guid?(), new bool?(false), new DateTime?(), 0, TaskGroupQueryOrder.CreatedOnDescending);
      return taskGroups.Any<TaskGroup>() ? taskGroups[0].ParentDefinitionId : throw new MetaTaskDefinitionNotFoundException(TaskResources.MetaTaskDefinitionNotFound((object) taskGroupId));
    }

    public List<TaskGroup> GetTaskGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? taskGroupId = null,
      bool? expanded = false,
      Guid? taskIdFilter = null,
      bool? deleted = false,
      DateTime? continuationToken = null,
      int top = 0,
      TaskGroupQueryOrder queryOrder = TaskGroupQueryOrder.CreatedOnDescending)
    {
      using (new MethodScope(requestContext, nameof (MetaTaskService), nameof (GetTaskGroups)))
      {
        this.m_securityProvider.EnsureMetaTaskPermissionsInitialized(requestContext, projectId);
        IEnumerable<MetaTaskDefinitionData> metaTaskDefinitions;
        using (MetaTaskDefinitionComponent component = requestContext.CreateComponent<MetaTaskDefinitionComponent>())
          metaTaskDefinitions = (IEnumerable<MetaTaskDefinitionData>) component.GetMetaTaskDefinitions(projectId, taskGroupId, taskIdFilter, deleted.HasValue && deleted.Value, continuationToken, top, queryOrder);
        List<TaskGroup> list = metaTaskDefinitions.Select<MetaTaskDefinitionData, TaskGroup>((Func<MetaTaskDefinitionData, TaskGroup>) (x => x.GetDefinition())).ToList<TaskGroup>();
        if (list.Any<TaskGroup>())
        {
          list = list.ResolveIdentityRefs(requestContext).ToList<TaskGroup>();
          bool? nullable = expanded;
          bool flag = true;
          if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
          {
            foreach (TaskGroup taskGroup in list)
            {
              List<TaskGroupStep> expandedTasks = new List<TaskGroupStep>();
              try
              {
                MetaTaskHelper.ExpandTasks(requestContext, projectId, taskGroup, (IList<TaskGroupStep>) expandedTasks);
                taskGroup.Tasks = (IList<TaskGroupStep>) expandedTasks;
              }
              catch (MetaTaskDefinitionNotFoundException ex)
              {
                requestContext.TraceError(10015203, "DistributedTask", "Unable to expand task group {0}", (object) taskGroup.Id);
              }
            }
          }
        }
        bool flag1 = taskGroupId.HasValue && !taskGroupId.Value.Equals(Guid.Empty);
        if ((!taskIdFilter.HasValue ? 0 : (!taskIdFilter.Value.Equals(Guid.Empty) ? 1 : 0)) != 0 || flag1 && list.Any<TaskGroup>())
          return list;
        IEnumerable<TaskGroup> groupFromExtensions = this.GetTaskGroupFromExtensions(requestContext, taskGroupId, expanded);
        return list.Concat<TaskGroup>(groupFromExtensions).ToList<TaskGroup>();
      }
    }

    private IEnumerable<TaskGroup> GetTaskGroupFromExtensions(
      IVssRequestContext requestContext,
      Guid? taskGroupId = null,
      bool? expanded = false)
    {
      int num;
      if (expanded.HasValue)
      {
        bool? nullable = expanded;
        bool flag = true;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        {
          num = 2;
          goto label_4;
        }
      }
      num = 0;
label_4:
      TaskGroupExpands taskGroupExpands = (TaskGroupExpands) num;
      return this.GetTaskGroupFromExtensions(requestContext, taskGroupId, new TaskGroupExpands?(taskGroupExpands));
    }

    private IEnumerable<TaskGroup> GetTaskGroupFromExtensions(
      IVssRequestContext requestContext,
      Guid? taskGroupId = null,
      TaskGroupExpands? expands = TaskGroupExpands.None)
    {
      int num = !taskGroupId.HasValue ? 0 : (!taskGroupId.Value.Equals(Guid.Empty) ? 1 : 0);
      List<TaskGroup> taskGroupList = this.getTaskGroupsFromExtensions(requestContext);
      List<TaskGroup> groupFromExtensions = num != 0 ? taskGroupList.Where<TaskGroup>((Func<TaskGroup, bool>) (x => x.Id == taskGroupId.Value)).ToList<TaskGroup>() : taskGroupList;
      if (expands.HasValue)
      {
        TaskGroupExpands? nullable = expands;
        TaskGroupExpands taskGroupExpands = TaskGroupExpands.None;
        if (nullable.GetValueOrDefault() == taskGroupExpands & nullable.HasValue)
          return (IEnumerable<TaskGroup>) groupFromExtensions;
      }
      foreach (TaskGroup taskGroup in groupFromExtensions)
      {
        List<TaskGroupStep> expandedTasks = new List<TaskGroupStep>();
        MetaTaskHelper.ExpandTasks(requestContext, taskGroup, taskGroupList, expandedTasks);
        taskGroup.Tasks = (IList<TaskGroupStep>) expandedTasks;
      }
      return (IEnumerable<TaskGroup>) groupFromExtensions;
    }

    public TaskGroup GetTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string versionSpec,
      bool? expanded = false)
    {
      int num;
      if (expanded.HasValue)
      {
        bool? nullable = expanded;
        bool flag = true;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        {
          num = 2;
          goto label_4;
        }
      }
      num = 0;
label_4:
      TaskGroupExpands taskGroupExpands = (TaskGroupExpands) num;
      return this.GetTaskGroup(requestContext, projectId, taskGroupId, versionSpec, new TaskGroupExpands?(taskGroupExpands));
    }

    public TaskGroup GetTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string versionSpec,
      TaskGroupExpands? expands = TaskGroupExpands.None)
    {
      using (new MethodScope(requestContext, nameof (MetaTaskService), nameof (GetTaskGroup)))
      {
        this.m_securityProvider.EnsureMetaTaskPermissionsInitialized(requestContext, projectId);
        IEnumerable<MetaTaskDefinitionData> metaTaskDefinitions;
        using (MetaTaskDefinitionComponent component = requestContext.CreateComponent<MetaTaskDefinitionComponent>())
          metaTaskDefinitions = (IEnumerable<MetaTaskDefinitionData>) component.GetMetaTaskDefinitions(projectId, new Guid?(taskGroupId));
        List<TaskGroup> list = metaTaskDefinitions.Select<MetaTaskDefinitionData, TaskGroup>((Func<MetaTaskDefinitionData, TaskGroup>) (x => x.GetDefinition())).ToList<TaskGroup>();
        if (!list.Any<TaskGroup>())
        {
          IEnumerable<TaskGroup> groupFromExtensions = this.GetTaskGroupFromExtensions(requestContext, new Guid?(taskGroupId), expands);
          if (!groupFromExtensions.Any<TaskGroup>())
            throw new MetaTaskDefinitionNotFoundException(TaskResources.MetaTaskDefinitionNotFound((object) taskGroupId));
          return groupFromExtensions.FirstOrDefault<TaskGroup>((Func<TaskGroup, bool>) (x => x.Id == taskGroupId));
        }
        TaskGroup taskGroup = MetaTaskHelper.ResolveTaskGroupVersion((IList<TaskGroup>) list, taskGroupId, versionSpec).ResolveIdentityRefs(requestContext);
        TaskGroupExpands? nullable = expands;
        TaskGroupExpands taskGroupExpands = TaskGroupExpands.Tasks;
        if (nullable.GetValueOrDefault() == taskGroupExpands & nullable.HasValue)
        {
          List<TaskGroupStep> expandedTasks = new List<TaskGroupStep>();
          MetaTaskHelper.ExpandTasks(requestContext, projectId, taskGroup, (IList<TaskGroupStep>) expandedTasks);
          taskGroup.Tasks = (IList<TaskGroupStep>) expandedTasks;
        }
        return taskGroup;
      }
    }

    public bool UndeleteTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string comment)
    {
      using (new MethodScope(requestContext, nameof (MetaTaskService), nameof (UndeleteTaskGroup)))
      {
        this.m_securityProvider.CheckMetaTaskPermission(requestContext, projectId, taskGroupId, new Guid?(), 4);
        Guid userId = requestContext.GetUserId();
        using (MetaTaskDefinitionComponent component = requestContext.CreateComponent<MetaTaskDefinitionComponent>())
          component.UndeleteTaskGroup(projectId, taskGroupId, userId, comment);
      }
      return true;
    }

    public TaskGroup UpdateTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroup taskGroup)
    {
      if (taskGroup == null)
        throw new InvalidRequestException(TaskResources.InvalidTaskGroupInput());
      return this.UpdatedTaskGroup(requestContext, projectId, taskGroup);
    }

    public TaskGroup UpdateTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroupUpdateParameter taskGroupUpdateParameter)
    {
      TaskGroup taskGroup = taskGroupUpdateParameter != null ? taskGroupUpdateParameter.ToTaskGroup() : throw new InvalidRequestException(TaskResources.InvalidTaskGroupInput());
      return this.UpdatedTaskGroup(requestContext, projectId, taskGroup);
    }

    public IEnumerable<TaskGroup> PublishTaskGroup(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      Guid taskGroupId,
      PublishTaskGroupMetadata taskGroupMetadata)
    {
      if (taskGroupMetadata == null)
        throw new InvalidRequestException(TaskResources.InvalidTaskGroupInput());
      this.m_securityProvider.CheckMetaTaskPermission(tfsRequestContext, projectId, taskGroupId, new Guid?(), 2);
      this.m_securityProvider.CheckMetaTaskPermission(tfsRequestContext, projectId, taskGroupMetadata.TaskGroupId, new Guid?(taskGroupId), 4);
      Guid userId = tfsRequestContext.GetUserId();
      MetaTaskDefinitionData taskDefinitionData;
      using (MetaTaskDefinitionComponent component = tfsRequestContext.CreateComponent<MetaTaskDefinitionComponent>())
        taskDefinitionData = component.PublishTaskGroup(projectId, taskGroupMetadata, taskGroupId, userId);
      if (taskDefinitionData != null)
      {
        TaskGroup taskGroup = taskDefinitionData.GetDefinition().ResolveIdentityRefs(tfsRequestContext);
        this.SaveRevision(tfsRequestContext, projectId, taskGroup, taskGroupMetadata.Comment, AuditAction.Update);
      }
      return (IEnumerable<TaskGroup>) this.GetTaskGroups(tfsRequestContext, projectId, new Guid?(taskGroupId), new bool?(false), new Guid?(), new bool?(false), new DateTime?(), 0, TaskGroupQueryOrder.CreatedOnDescending);
    }

    public IEnumerable<TaskGroup> PublishPreviewTaskGroup(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      Guid taskGroupId,
      TaskGroupPublishPreviewParameter publishPreviewParameter)
    {
      if (publishPreviewParameter == null)
        throw new InvalidRequestException(TaskResources.InvalidTaskGroupInput());
      this.m_securityProvider.CheckMetaTaskPermission(tfsRequestContext, projectId, taskGroupId, new Guid?(), 2);
      TaskGroup taskGroupVersion = this.GetTaskGroups(tfsRequestContext, projectId, new Guid?(taskGroupId), new bool?(false), new Guid?(), new bool?(false), new DateTime?(), 0, TaskGroupQueryOrder.CreatedOnDescending).FirstOrDefault<TaskGroup>((Func<TaskGroup, bool>) (t => t.Version.Equals(publishPreviewParameter.Version) && t.Preview));
      if (taskGroupVersion == null)
        throw new MetaTaskDefinitionNotFoundException(TaskResources.MetaTaskDefinitionNotFound((object) taskGroupId));
      if (!publishPreviewParameter.Preview)
      {
        taskGroupVersion.Preview = publishPreviewParameter.Preview;
        taskGroupVersion.Comment = publishPreviewParameter.Comment;
        taskGroupVersion.Revision = publishPreviewParameter.Revision;
        this.UpdateTaskGroupAndDisableOldVersions(tfsRequestContext, projectId, taskGroupVersion, publishPreviewParameter.DisablePriorVersions);
      }
      return (IEnumerable<TaskGroup>) this.GetTaskGroups(tfsRequestContext, projectId, new Guid?(taskGroupId), new bool?(false), new Guid?(), new bool?(false), new DateTime?(), 0, TaskGroupQueryOrder.CreatedOnDescending);
    }

    public bool DeleteRevisionHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId)
    {
      this.m_securityProvider.CheckMetaTaskPermission(requestContext, projectId, taskGroupId, new Guid?(), 4);
      IEnumerable<int> fileIds;
      using (MetaTaskDefinitionHistoryComponent component = requestContext.CreateComponent<MetaTaskDefinitionHistoryComponent>())
        fileIds = component.GetTaskGroupHistory(projectId, taskGroupId).Select<MetaTaskDefinitionRevisionData, int>((Func<MetaTaskDefinitionRevisionData, int>) (revision => revision.FileId));
      requestContext.GetService<TeamFoundationFileService>().DeleteFiles(requestContext, fileIds);
      using (MetaTaskDefinitionHistoryComponent component = requestContext.CreateComponent<MetaTaskDefinitionHistoryComponent>())
        component.DeleteTaskGroupHistory(projectId, taskGroupId);
      return true;
    }

    public Stream GetRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      int revision)
    {
      this.m_securityProvider.EnsureMetaTaskPermissionsInitialized(requestContext, projectId);
      MetaTaskDefinitionRevisionData taskGroupRevision;
      using (MetaTaskDefinitionHistoryComponent component = requestContext.CreateComponent<MetaTaskDefinitionHistoryComponent>())
        taskGroupRevision = component.GetTaskGroupRevision(projectId, taskGroupId, revision);
      if (taskGroupRevision != null)
        return requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) taskGroupRevision.FileId, false, out byte[] _, out long _, out CompressionType _);
      return Stream.Null;
    }

    public List<TaskGroupRevision> GetRevisionHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId)
    {
      this.m_securityProvider.EnsureMetaTaskPermissionsInitialized(requestContext, projectId);
      List<MetaTaskDefinitionRevisionData> taskGroupHistory;
      using (MetaTaskDefinitionHistoryComponent component = requestContext.CreateComponent<MetaTaskDefinitionHistoryComponent>())
        taskGroupHistory = component.GetTaskGroupHistory(projectId, taskGroupId);
      if (taskGroupHistory != null)
        return taskGroupHistory.OrderBy<MetaTaskDefinitionRevisionData, DateTime>((Func<MetaTaskDefinitionRevisionData, DateTime>) (r => r.ChangedDate)).Select<MetaTaskDefinitionRevisionData, TaskGroupRevision>((Func<MetaTaskDefinitionRevisionData, TaskGroupRevision>) (r => r.GetRevision())).ResolveIdentityRefs(requestContext).ToList<TaskGroupRevision>();
      return new List<TaskGroupRevision>();
    }

    public int SaveRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroup taskGroup,
      string comment,
      AuditAction changeType)
    {
      if (taskGroup == null)
        throw new InvalidRequestException(TaskResources.InvalidTaskGroupInput());
      taskGroup.ModifiedBy = requestContext.GetRequesterIdentity();
      int fileService = MetaTaskService.SaveTaskGroupToFileService(requestContext, taskGroup);
      using (MetaTaskDefinitionHistoryComponent component = requestContext.CreateComponent<MetaTaskDefinitionHistoryComponent>())
        component.AddTaskGroupRevision(projectId, taskGroup.Id, Guid.Parse(taskGroup.ModifiedBy.Id), taskGroup.Revision, taskGroup.Version.Major, fileService, comment, changeType);
      return fileService;
    }

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      using (MetaTaskDefinitionComponent component = requestContext.CreateComponent<MetaTaskDefinitionComponent>())
        component.DeleteTeamProject(projectId);
    }

    private void UpdateTaskGroupAndDisableOldVersions(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      TaskGroup taskGroupVersion,
      bool disablePriorVersions)
    {
      this.PrepareTaskGroupForUpdate(tfsRequestContext, projectId, taskGroupVersion);
      bool enablePriorVersionEdit = tfsRequestContext.IsFeatureEnabled("DistributedTask.EnableTaskGroupVersionEdit");
      List<int> disabledVersions;
      MetaTaskDefinitionData taskDefinitionData;
      using (MetaTaskDefinitionComponent component = tfsRequestContext.CreateComponent<MetaTaskDefinitionComponent>())
        taskDefinitionData = component.UpdateMetaTaskDefinitionAndDisableOldVersions(projectId, taskGroupVersion, disablePriorVersions, enablePriorVersionEdit, out disabledVersions);
      string comment = disabledVersions.Any<int>() ? MetaTaskService.AppendDisabledPirorVersionsInfoToComment(disabledVersions, taskGroupVersion.Comment) : taskGroupVersion.Comment;
      if (taskDefinitionData == null)
        return;
      TaskGroup taskGroup = taskDefinitionData.GetDefinition().ResolveIdentityRefs(tfsRequestContext);
      this.SaveRevision(tfsRequestContext, projectId, taskGroup, comment, AuditAction.Update);
    }

    private TaskGroup UpdatedTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroup taskGroup)
    {
      using (new MethodScope(requestContext, nameof (MetaTaskService), nameof (UpdatedTaskGroup)))
      {
        this.PrepareTaskGroupForUpdate(requestContext, projectId, taskGroup);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo(10015173, "DistributedTask", "Publishing TaskGroupChangingEvent decision point: Updating task group {0}", (object) taskGroup.Id);
        service.PublishDecisionPoint(requestContext, (object) new TaskGroupChangingEvent(projectId, AuditAction.Update, taskGroup));
        bool enablePriorVersionEdit = requestContext.IsFeatureEnabled("DistributedTask.EnableTaskGroupVersionEdit");
        MetaTaskDefinitionData taskDefinitionData;
        using (MetaTaskDefinitionComponent component = requestContext.CreateComponent<MetaTaskDefinitionComponent>())
          taskDefinitionData = component.UpdateMetaTaskDefinition(projectId, taskGroup, enablePriorVersionEdit);
        if (taskDefinitionData != null)
        {
          TaskGroup taskGroup1 = taskDefinitionData.GetDefinition().ResolveIdentityRefs(requestContext);
          this.SaveRevision(requestContext, projectId, taskGroup1, taskGroup.Comment, AuditAction.Update);
          return taskGroup1;
        }
      }
      return taskGroup;
    }

    private static string AppendDisabledPirorVersionsInfoToComment(
      List<int> disabledVersions,
      string comment)
    {
      string str1 = TaskResources.DisabledPriorVersionsComment((object) string.Join<int>(",", (IEnumerable<int>) disabledVersions));
      string str2;
      if (!string.IsNullOrEmpty(comment))
        str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}. {1}", (object) comment.Trim().TrimEnd('.'), (object) str1);
      else
        str2 = str1;
      comment = str2;
      return comment;
    }

    private void PrepareTaskGroupForUpdate(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroup taskGroup)
    {
      if (taskGroup.DefinitionType.IsNullOrEmpty<char>())
        taskGroup.DefinitionType = "metaTask";
      this.m_securityProvider.CheckMetaTaskPermission(requestContext, projectId, taskGroup.Id, taskGroup.ParentDefinitionId, 2);
      taskGroup.FixLatestMajorTaskVersions(requestContext);
      taskGroup.Validate(requestContext, projectId);
      taskGroup.ModifiedBy = requestContext.GetRequesterIdentity();
      TaskGroup taskGroup1 = this.GetTaskGroups(requestContext, projectId, new Guid?(taskGroup.Id), new bool?(false), new Guid?(), new bool?(false), new DateTime?(), 0, TaskGroupQueryOrder.CreatedOnDescending).FirstOrDefault<TaskGroup>((Func<TaskGroup, bool>) (t => t.Version.Equals(taskGroup.Version)));
      if (taskGroup1 == null)
        throw new MetaTaskDefinitionNotFoundException(TaskResources.MetaTaskDefinitionNotFound((object) taskGroup.Id));
      if (taskGroup1.Tasks.Count == taskGroup.Tasks.Count)
      {
        for (int index = 0; index < taskGroup1.Tasks.Count; ++index)
        {
          if (!TaskGroupStep.EqualsAndOldTaskInputsAreSubsetOfNewTaskInputs(taskGroup1.Tasks[index], taskGroup.Tasks[index]))
          {
            this.m_securityProvider.CheckMetaTaskEndpointSecurity(requestContext, projectId, taskGroup);
            break;
          }
        }
      }
      else
        this.m_securityProvider.CheckMetaTaskEndpointSecurity(requestContext, projectId, taskGroup);
    }

    private static int SaveTaskGroupToFileService(
      IVssRequestContext requestContext,
      TaskGroup taskGroup)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
      };
      byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) taskGroup, settings));
      return requestContext.GetService<TeamFoundationFileService>().UploadFile(requestContext, bytes);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
