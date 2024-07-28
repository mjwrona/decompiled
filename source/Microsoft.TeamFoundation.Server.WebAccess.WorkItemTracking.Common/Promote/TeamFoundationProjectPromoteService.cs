// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote.TeamFoundationProjectPromoteService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote
{
  public class TeamFoundationProjectPromoteService : 
    ITeamFoundationProjectPromoteService,
    IVssFrameworkService
  {
    private static readonly IProjectPromoteStepFactory[] s_stepFactories = new IProjectPromoteStepFactory[2]
    {
      WorkItemTrackingPromoteStepFactory.Instance,
      AgilePromoteStepFactory.Instance
    };
    private static readonly HashSet<Guid> s_oobProcessTemplateTypeIds = new HashSet<Guid>();
    public const string Area = "TeamProjectPromote";
    public const string Layer = "TeamProjectPromote";

    static TeamFoundationProjectPromoteService()
    {
      TeamFoundationProjectPromoteService.s_oobProcessTemplateTypeIds.Add(OutOfBoxProcessTemplateTypeIds.Agile);
      TeamFoundationProjectPromoteService.s_oobProcessTemplateTypeIds.Add(OutOfBoxProcessTemplateTypeIds.Scrum);
      TeamFoundationProjectPromoteService.s_oobProcessTemplateTypeIds.Add(OutOfBoxProcessTemplateTypeIds.Cmmi);
      TeamFoundationProjectPromoteService.s_oobProcessTemplateTypeIds.Add(OutOfBoxProcessTemplateTypeIds.Basic);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) systemRequestContext.ServiceHost.HostType.ToString()));
    }

    public void RenameFields(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<string, string>> fields)
    {
      requestContext.GetService<IProvisioningService>().RenameFields(requestContext, fields);
    }

    public virtual void QueuePromoteJob(
      IVssRequestContext requestContext,
      Guid processTemplateTypeId,
      bool isXmlToInheritedPromote = false,
      Guid? xmlBackedProjectToPromote = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTemplateTypeId, nameof (processTemplateTypeId));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) requestContext.ServiceHost.HostType.ToString()));
      if (TeamFoundationProjectPromoteService.s_oobProcessTemplateTypeIds.Contains(processTemplateTypeId) && !isXmlToInheritedPromote)
        throw new InvalidOperationException();
      ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processTemplateTypeId);
      if (!isXmlToInheritedPromote && processDescriptor.ProcessStatus != ProcessStatus.Updating)
        throw new InvalidOperationException();
      if (isXmlToInheritedPromote && !xmlBackedProjectToPromote.HasValue)
        throw new InvalidOperationException();
      if (isXmlToInheritedPromote)
        this.QuickCheckIsValidProcessForXMLBackedProjectPromote(requestContext, processTemplateTypeId, xmlBackedProjectToPromote.Value);
      if (isXmlToInheritedPromote)
      {
        ProjectProcessConfiguration processSettings = requestContext.GetService<ProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(xmlBackedProjectToPromote.ToString()), false);
        processSettings.IsTeamFieldAreaPath();
        if (processSettings != null && !processSettings.IsTeamFieldAreaPath())
          throw new InvalidOperationException(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.TeamFieldInUse);
      }
      TeamProjectPromoteJobData projectPromoteJobData = new TeamProjectPromoteJobData(processTemplateTypeId, isXmlToInheritedPromote: isXmlToInheritedPromote);
      this.InitializeProjectsToPromote(requestContext, projectPromoteJobData, xmlBackedProjectToPromote);
      TeamFoundationJobDefinition foundationJobDefinition1 = new TeamFoundationJobDefinition(processTemplateTypeId, "Team Project Promote Job", typeof (TeamProjectPromoteJobExtension).FullName, TeamFoundationSerializationUtility.SerializeToXml((object) projectPromoteJobData), TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.AboveNormal);
      foundationJobDefinition1.DisableDuringUpgrade = false;
      foundationJobDefinition1.Schedule.Add(new TeamFoundationJobSchedule()
      {
        ScheduledTime = DateTime.UtcNow,
        Interval = 900,
        PriorityLevel = JobPriorityLevel.Normal
      });
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      if (service.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        processTemplateTypeId
      }).Any<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (x => x != null)))
        throw new InvalidOperationException(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.JobAlreadyQueued);
      TeamFoundationJobDefinition foundationJobDefinition2 = service.QueryJobDefinition(requestContext, processTemplateTypeId);
      if (foundationJobDefinition2 != null && (!string.Equals(foundationJobDefinition2.Name, foundationJobDefinition1.Name, StringComparison.OrdinalIgnoreCase) || !string.Equals(foundationJobDefinition2.ExtensionName, foundationJobDefinition1.ExtensionName, StringComparison.Ordinal)))
        throw new InvalidOperationException(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.JobCurrentlyRunning);
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition1
      });
      service.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        foundationJobDefinition1.ToJobReference()
      });
    }

    internal void InitializeProjectsToPromote(
      IVssRequestContext requestContext,
      TeamProjectPromoteJobData jobData,
      Guid? projectIdToPromote = null)
    {
      IProjectService service1 = requestContext.GetService<IProjectService>();
      ITeamFoundationProcessService service2 = requestContext.GetService<ITeamFoundationProcessService>();
      jobData.Projects = new List<PromoteProjectInfo>();
      if (projectIdToPromote.HasValue)
      {
        ProjectInfo project = service1.GetProject(requestContext, projectIdToPromote.Value);
        if (project == null)
          throw new InvalidOperationException(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.ProjectToPromoteCouldNotBeFound);
        ProcessDescriptor templateDescriptor = TeamProjectUtil.GetProjectProcessTemplateDescriptor(requestContext, project.Id);
        if (templateDescriptor != null && !jobData.ProcessTemplateTypeId.Equals(templateDescriptor.TypeId))
        {
          jobData.Projects.Add(new PromoteProjectInfo(project.Id, ProjectPromoteState.NotProcessed));
        }
        else
        {
          if (templateDescriptor != null)
            return;
          requestContext.Trace(1000040, TraceLevel.Error, "TeamProjectPromote", "TeamProjectPromote", "Encountered project {0} with no corresponding process", (object) project.Id.ToString());
        }
      }
      else
      {
        foreach (ProjectInfo project in service1.GetProjects(requestContext, ProjectState.WellFormed))
        {
          ProcessDescriptor templateDescriptor = TeamProjectUtil.GetProjectProcessTemplateDescriptor(requestContext, project.Id);
          if (templateDescriptor != null && TeamFoundationProjectPromoteService.HasMatchingTemplateTypeId(jobData.ProcessTemplateTypeId, templateDescriptor.TypeId))
          {
            ProcessDescriptor descriptor;
            if (service2.TryGetProcessDescriptor(requestContext, templateDescriptor.TypeId, out descriptor) && descriptor.RowId != templateDescriptor.RowId)
              jobData.Projects.Add(new PromoteProjectInfo(project.Id, ProjectPromoteState.NotProcessed));
            else
              jobData.Projects.Add(new PromoteProjectInfo(project.Id, ProjectPromoteState.AlreadyAtTip));
          }
          else if (templateDescriptor == null)
            requestContext.Trace(1000039, TraceLevel.Error, "TeamProjectPromote", "TeamProjectPromote", "Encountered project {0} with no corresponding process", (object) project.Id.ToString());
        }
      }
    }

    internal static bool IsOutOfBoxProcessTemplate(Guid processTemplateTypeId) => TeamFoundationProjectPromoteService.s_oobProcessTemplateTypeIds.Contains(processTemplateTypeId);

    private static bool HasMatchingTemplateTypeId(Guid jobTypeId, Guid projectTypeId) => jobTypeId == Guid.Empty ? TeamFoundationProjectPromoteService.IsOutOfBoxProcessTemplate(projectTypeId) : jobTypeId == projectTypeId;

    private void QuickCheckIsValidProcessForXMLBackedProjectPromote(
      IVssRequestContext requestContext,
      Guid processTemplateTypeId,
      Guid ProjectId)
    {
      ITeamFoundationProcessService service1 = requestContext.GetService<ITeamFoundationProcessService>();
      TeamProjectUtil.FixProjectProcessTemplateDescriptorsForOOBProjects(requestContext, ProjectId);
      ProcessDescriptor templateDescriptor = TeamProjectUtil.GetProjectProcessTemplateDescriptor(requestContext, ProjectId);
      IVssRequestContext requestContext1 = requestContext;
      Guid processTypeId = processTemplateTypeId;
      ProcessDescriptor processDescriptor = service1.GetProcessDescriptor(requestContext1, processTypeId, bypassCache: true);
      if (!processDescriptor.IsDerived && !TeamFoundationProjectPromoteService.s_oobProcessTemplateTypeIds.Contains(processDescriptor.TypeId))
        throw new InvalidOperationException(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.CannotPromoteXMLBackedProjectToXmlProcess);
      if (templateDescriptor == null)
        throw new InvalidOperationException(string.Format(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.NullRefInQuickCheckIsValidProcessForXMLBackedProjectPromote, (object) "currentProcessDescriptor", (object) processTemplateTypeId, (object) ProjectId));
      if (processDescriptor == null)
        throw new InvalidOperationException(string.Format(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.NullRefInQuickCheckIsValidProcessForXMLBackedProjectPromote, (object) "targetDescriptor", (object) processTemplateTypeId, (object) ProjectId));
      IProcessWorkItemTypeService service2 = requestContext.GetService<IProcessWorkItemTypeService>();
      IReadOnlyCollection<ComposedWorkItemType> allWorkItemTypes = service2.GetAllWorkItemTypes(requestContext, templateDescriptor.TypeId);
      HashSet<string> hashSet = (service2.GetAllWorkItemTypes(requestContext, processTemplateTypeId) ?? throw new InvalidOperationException(string.Format(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.NullRefInQuickCheckIsValidProcessForXMLBackedProjectPromote, (object) "workItemTypesInTargetProcess", (object) processTemplateTypeId, (object) ProjectId))).ToHashSet<ComposedWorkItemType, string>((Func<ComposedWorkItemType, string>) (w => w.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      IWorkItemTypeService service3 = requestContext.GetService<IWorkItemTypeService>();
      List<string> stringList = new List<string>();
      foreach (ComposedWorkItemType composedWorkItemType in (IEnumerable<ComposedWorkItemType>) allWorkItemTypes)
      {
        if (!hashSet.Contains(composedWorkItemType.Name) && service3.HasAnyWorkItemsOfTypeForProject(requestContext, ProjectId, composedWorkItemType.Name))
          stringList.Add(composedWorkItemType.Name);
      }
      if (stringList.Any<string>())
      {
        stringList.Sort();
        throw new InvalidOperationException(string.Format(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.ExpectedWorkItemTypeNotPresentInTargetProcess, (object) string.Join(", ", (IEnumerable<string>) stringList)));
      }
    }

    public PromoteProjectInfo Promote(
      IVssRequestContext requestContext,
      Guid projectId,
      bool isXmlToInheritedPromote,
      Guid targetProcessTemplateTypeIdForXmlToInheritedPromote,
      out string promoteLog)
    {
      return this.Promote(requestContext, projectId, 0, isXmlToInheritedPromote, targetProcessTemplateTypeIdForXmlToInheritedPromote, out promoteLog);
    }

    internal PromoteProjectInfo Promote(
      IVssRequestContext requestContext,
      Guid projectId,
      int stepsToSkip,
      bool isXmlToInheritedPromote,
      Guid targetProcessTemplateTypeIdForXmlToInheritedPromote,
      out string promoteLog)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) requestContext.ServiceHost.HostType.ToString()));
      PromoteProjectInfo promoteInfo = new PromoteProjectInfo()
      {
        Id = projectId,
        CompletedSteps = stepsToSkip,
        State = ProjectPromoteState.NotProcessed
      };
      bool flag = false;
      StringBuilder log = new StringBuilder();
      try
      {
        requestContext.TraceEnter(1000006, "TeamProjectPromote", "TeamProjectPromote", "TeamProjectPromoter.Promote");
        IProjectService service1 = requestContext.GetService<IProjectService>();
        ProjectInfo project;
        try
        {
          project = service1.GetProject(requestContext, projectId);
        }
        catch (ProjectDoesNotExistException ex)
        {
          log.Append("Project not found");
          promoteInfo.State = ProjectPromoteState.DeletedOrNotExist;
          return promoteInfo;
        }
        log.AppendFormat("Team project state: {0}", (object) project.State.ToString());
        requestContext.Trace(1000032, TraceLevel.Verbose, "TeamProjectPromote", "TeamProjectPromote", "Team project state: {0}", (object) project.State.ToString());
        if (project.State == ProjectState.Deleting || project.State == ProjectState.Deleted)
        {
          log.AppendLine("Project is deleted or being deleted");
          promoteInfo.State = ProjectPromoteState.DeletedOrNotExist;
          return promoteInfo;
        }
        if (project.State != ProjectState.WellFormed)
        {
          log.AppendLine("Project is not well formed");
          promoteInfo.State = ProjectPromoteState.NotProcessed;
          return promoteInfo;
        }
        ITeamFoundationProcessService service2 = requestContext.GetService<ITeamFoundationProcessService>();
        IWorkItemTrackingProcessService service3 = requestContext.GetService<IWorkItemTrackingProcessService>();
        ProcessDescriptor currentProcess;
        ProcessDescriptor processDescriptor1;
        if (!isXmlToInheritedPromote)
        {
          requestContext.TraceEnter(1000009, "TeamProjectPromote", "TeamProjectPromote", "TeamProjectPromoter.DetectingProcessTemplate");
          currentProcess = TeamProjectUtil.GetProjectProcessTemplateDescriptor(requestContext, project.Id);
          if (currentProcess == null)
            return this.HandleError(requestContext, promoteInfo, log, "Could not determine project process.");
          ProcessVersion version = currentProcess.Version;
          log.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Detected Project Version '{0}'.", (object) version));
          requestContext.TraceLeave(1000010, "TeamProjectPromote", "TeamProjectPromote", "TeamProjectPromoter.DetectingProcessTemplate");
          requestContext.TraceEnter(1000011, "TeamProjectPromote", "TeamProjectPromote", "TeamProjectPromoter.GetTipProcessTemplate");
          processDescriptor1 = service2.GetProcessDescriptor(requestContext, currentProcess.TypeId, bypassCache: true);
          if (processDescriptor1 == null)
            return this.HandleError(requestContext, promoteInfo, log, "No template found matching the project process template.");
          if (processDescriptor1.RowId == currentProcess.RowId)
          {
            log.AppendLine("Project is already at tip.");
            promoteInfo.State = ProjectPromoteState.AlreadyAtTip;
            return promoteInfo;
          }
          if (currentProcess.RevisedDate > processDescriptor1.RevisedDate)
            return this.HandleError(requestContext, promoteInfo, log, "Project version is newer than that process template version.");
        }
        else
        {
          processDescriptor1 = service2.GetProcessDescriptor(requestContext, targetProcessTemplateTypeIdForXmlToInheritedPromote);
          currentProcess = (ProcessDescriptor) null;
          project.PopulateProperties(requestContext, ProcessTemplateIdPropertyNames.ProcessTemplateType, ProcessTemplateIdPropertyNames.CurrentProcessTemplateId);
          if (processDescriptor1.Inherits != Guid.Empty)
          {
            ProcessDescriptor processDescriptor2 = service2.GetProcessDescriptor(requestContext, processDescriptor1.Inherits);
            service3.UpdateProjectProcess(requestContext, project, processDescriptor2);
            flag = true;
          }
          else
            service3.UpdateProjectProcess(requestContext, project, processDescriptor1);
        }
        IEnumerable<IProjectPromoteStep> promoteSteps = this.GetPromoteSteps(requestContext, currentProcess, processDescriptor1, projectId, log);
        requestContext.Trace(1000019, TraceLevel.Verbose, "TeamProjectPromote", "TeamProjectPromote", "Execute promote steps");
        int num = 0;
        foreach (IProjectPromoteStep projectPromoteStep in promoteSteps)
        {
          if (++num > stepsToSkip)
          {
            projectPromoteStep.Execute(requestContext, project, log);
            ++promoteInfo.CompletedSteps;
          }
        }
        if (flag)
        {
          service3.UpdateProjectProcess(requestContext, project, processDescriptor1);
          flag = false;
        }
        TeamFoundationProjectPromoteService.StampProject(requestContext, project, processDescriptor1.TypeId);
        if (isXmlToInheritedPromote)
          TeamFoundationProjectPromoteService.CleanupProvisionedXmlRecordsForProject(requestContext, projectId);
        log.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Stamped Project with Version '{0}'.", (object) processDescriptor1.Version));
        promoteInfo.State = ProjectPromoteState.Promoted;
        return promoteInfo;
      }
      catch (Exception ex)
      {
        if (flag)
        {
          ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
          project.PopulateProperties(requestContext, ProcessTemplateIdPropertyNames.ProcessTemplateType, ProcessTemplateIdPropertyNames.CurrentProcessTemplateId);
          ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, targetProcessTemplateTypeIdForXmlToInheritedPromote);
          requestContext.GetService<IWorkItemTrackingProcessService>().UpdateProjectProcess(requestContext, project, processDescriptor);
        }
        return this.HandleError(requestContext, promoteInfo, log, ex.Message, ex.StackTrace);
      }
      finally
      {
        promoteLog = log.ToString();
        requestContext.TraceLeave(1000013, "TeamProjectPromote", "TeamProjectPromote", "TeamProjectPromoter.Promote");
      }
    }

    private static void CleanupProvisionedXmlRecordsForProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      int id = requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectId, projectId).Id;
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.CleanupProvisionedRecordsForInheritedProject(id);
    }

    public void PromoteImportedProjectToOOBProcess(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid oobProcessTypeId,
      bool skipProcessStamp = false)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(oobProcessTypeId, nameof (oobProcessTypeId));
      if (!TeamFoundationProjectPromoteService.IsOutOfBoxProcessTemplate(oobProcessTypeId))
        throw new InvalidOperationException(string.Format(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Resources.PromoteInvalidTemplate, (object) oobProcessTypeId));
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
      ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, oobProcessTypeId);
      StringBuilder log = new StringBuilder();
      IEnumerable<IProjectPromoteStep> promoteSteps = this.GetPromoteSteps(requestContext, (ProcessDescriptor) null, processDescriptor, projectId, log);
      requestContext.Trace(1000019, TraceLevel.Verbose, "TeamProjectPromote", "TeamProjectPromote", "Execute promote steps");
      foreach (IProjectPromoteStep projectPromoteStep in promoteSteps)
        projectPromoteStep.Execute(requestContext, project, log);
      requestContext.Trace(1000021, TraceLevel.Verbose, "TeamProjectPromote", "TeamProjectPromote", "Stamped Project with Version '{0}'.", (object) processDescriptor.Version);
      if (skipProcessStamp)
        return;
      TeamFoundationProjectPromoteService.StampProject(requestContext, project, processDescriptor.TypeId);
    }

    public void ForcePromoteHostedXmlProjectTOInherited(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid oobProcessTypeId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(oobProcessTypeId, nameof (oobProcessTypeId));
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
      ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, oobProcessTypeId);
      StringBuilder log = new StringBuilder();
      IEnumerable<IProjectPromoteStep> promoteSteps = this.GetPromoteSteps(requestContext, (ProcessDescriptor) null, processDescriptor, projectId, log);
      requestContext.Trace(1000019, TraceLevel.Verbose, "TeamProjectPromote", "TeamProjectPromote", "Execute promote steps");
      requestContext.Items.Add("PromoteHostedProjectToInheritedUser", (object) requestContext.GetUserId());
      foreach (IProjectPromoteStep projectPromoteStep in promoteSteps)
        projectPromoteStep.Execute(requestContext, project, log);
      requestContext.Items.Remove("PromoteHostedProjectToInheritedUser");
      requestContext.Trace(1000021, TraceLevel.Verbose, "TeamProjectPromote", "TeamProjectPromote", "Stamped Project with Version '{0}'.", (object) processDescriptor.Version);
      TeamFoundationProjectPromoteService.StampProject(requestContext, project, processDescriptor.TypeId);
    }

    private IEnumerable<IProjectPromoteStep> GetPromoteSteps(
      IVssRequestContext requestContext,
      ProcessDescriptor currentProcess,
      ProcessDescriptor latestProcess,
      Guid projectId,
      StringBuilder log)
    {
      IVssRequestContext vssRequestContext = requestContext;
      if (latestProcess.Scope == ProcessScope.Deployment)
        vssRequestContext = vssRequestContext.To(TeamFoundationHostType.Deployment);
      requestContext.Trace(1000016, TraceLevel.Verbose, "TeamProjectPromote", "TeamProjectPromote", "Initialize cache service context");
      TeamFoundationProjectPromoteService.PromoteStepDataCacheService service1 = vssRequestContext.GetService<TeamFoundationProjectPromoteService.PromoteStepDataCacheService>();
      TeamFoundationProjectPromoteService.PromoteStepDataKey key = new TeamFoundationProjectPromoteService.PromoteStepDataKey(currentProcess != null ? currentProcess.RowId : Guid.Empty, latestProcess.RowId);
      IEnumerable<IProjectPromoteStep> steps1;
      if (!service1.TryGetValue(vssRequestContext, key, out steps1))
      {
        requestContext.Trace(1000017, TraceLevel.Verbose, "TeamProjectPromote", "TeamProjectPromote", "Get process templates");
        ITeamFoundationProcessService service2 = requestContext.GetService<ITeamFoundationProcessService>();
        IProcessTemplate legacyProcess = service2.GetLegacyProcess(requestContext, latestProcess);
        IProcessTemplate processTemplate = (IProcessTemplate) null;
        if (currentProcess != null)
          processTemplate = service2.GetLegacyProcess(requestContext, currentProcess);
        List<IProjectPromoteStep> projectPromoteStepList = new List<IProjectPromoteStep>();
        foreach (IProjectPromoteStepFactory stepFactory in TeamFoundationProjectPromoteService.s_stepFactories)
        {
          requestContext.Trace(1000018, TraceLevel.Verbose, "TeamProjectPromote", "TeamProjectPromote", "Compute promote steps");
          IVssRequestContext requestContext1 = requestContext;
          IProcessTemplate source = processTemplate;
          IProcessTemplate destination = legacyProcess;
          StringBuilder log1 = log;
          IEnumerable<IProjectPromoteStep> steps2 = stepFactory.GenerateSteps(requestContext1, source, destination, log1);
          if (steps2 != null)
            projectPromoteStepList.AddRange(steps2);
        }
        steps1 = (IEnumerable<IProjectPromoteStep>) projectPromoteStepList;
        service1.TryAdd(vssRequestContext, key, steps1);
      }
      foreach (IProjectPromoteStepFactory stepFactory in TeamFoundationProjectPromoteService.s_stepFactories)
        stepFactory.ValidateSteps(requestContext, steps1, projectId, log);
      return steps1;
    }

    private PromoteProjectInfo HandleError(
      IVssRequestContext requestContext,
      PromoteProjectInfo promoteInfo,
      StringBuilder log,
      string errorMessage,
      string stackTrace = null)
    {
      string str = string.Format("[Error]: {0}", (object) errorMessage);
      log.AppendLine(str);
      requestContext.Trace(1000038, TraceLevel.Error, "TeamProjectPromote", "TeamProjectPromote", string.Format("[StackTrace]: {0}", (object) (stackTrace ?? Environment.StackTrace)));
      promoteInfo.State = ProjectPromoteState.Failed;
      return promoteInfo;
    }

    private static void StampProject(
      IVssRequestContext requestContext,
      ProjectInfo project,
      Guid processTemplateTypeId)
    {
      requestContext.TraceBlock(1000014, 1000015, "TeamProjectPromote", "TeamProjectPromote", nameof (StampProject), (Action) (() => TeamProjectUtil.UpdateProjectProcessTemplate(requestContext, project.Id, processTemplateTypeId)));
    }

    private class PromoteStepDataKey
    {
      public PromoteStepDataKey(
        Guid sourceProcessTemplateDescriptorRowId,
        Guid destinationProcessTemplateDescriptorRowId)
      {
        this.SourceProcessTemplateDescriptorRowId = sourceProcessTemplateDescriptorRowId;
        this.DestinationProcessTemplateDescriptorRowId = destinationProcessTemplateDescriptorRowId;
      }

      public Guid SourceProcessTemplateDescriptorRowId { get; private set; }

      public Guid DestinationProcessTemplateDescriptorRowId { get; private set; }
    }

    private class PromoteStepDataKeyComparer : 
      IEqualityComparer<TeamFoundationProjectPromoteService.PromoteStepDataKey>
    {
      public bool Equals(
        TeamFoundationProjectPromoteService.PromoteStepDataKey first,
        TeamFoundationProjectPromoteService.PromoteStepDataKey second)
      {
        return first == second || first != null && second != null && !(first.SourceProcessTemplateDescriptorRowId != second.SourceProcessTemplateDescriptorRowId) && !(first.DestinationProcessTemplateDescriptorRowId != second.DestinationProcessTemplateDescriptorRowId);
      }

      public int GetHashCode(
        TeamFoundationProjectPromoteService.PromoteStepDataKey key)
      {
        if (key == null)
          return 0;
        Guid templateDescriptorRowId = key.SourceProcessTemplateDescriptorRowId;
        int hashCode1 = templateDescriptorRowId.GetHashCode();
        templateDescriptorRowId = key.DestinationProcessTemplateDescriptorRowId;
        int hashCode2 = templateDescriptorRowId.GetHashCode();
        return hashCode1 ^ hashCode2;
      }
    }

    private class PromoteStepDataCacheService : 
      VssMemoryCacheService<TeamFoundationProjectPromoteService.PromoteStepDataKey, IEnumerable<IProjectPromoteStep>>
    {
      private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(600.0);
      private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(1200.0);
      private const int c_maxCacheSize = 20;

      public PromoteStepDataCacheService()
        : base((IEqualityComparer<TeamFoundationProjectPromoteService.PromoteStepDataKey>) new TeamFoundationProjectPromoteService.PromoteStepDataKeyComparer(), new MemoryCacheConfiguration<TeamFoundationProjectPromoteService.PromoteStepDataKey, IEnumerable<IProjectPromoteStep>>().WithMaxElements(20).WithCleanupInterval(TeamFoundationProjectPromoteService.PromoteStepDataCacheService.s_cacheCleanupInterval))
      {
        this.InactivityInterval.Value = TeamFoundationProjectPromoteService.PromoteStepDataCacheService.s_maxCacheInactivityAge;
      }
    }
  }
}
