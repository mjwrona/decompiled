// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote.TeamProjectPromoteJobExtension
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote
{
  public class TeamProjectPromoteJobExtension : ITeamFoundationJobExtension
  {
    private const string s_area = "TeamProjectPromote";
    private const string s_layer = "TeamProjectPromote";
    private const string s_delayKey = "/Service/Integration/Settings/PromoteRetryDelay";

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime jobQueueTime,
      out string resultMessage)
    {
      string internalResultMessage = (string) null;
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Blocked;
      requestContext.TraceBlock(1000026, 1000027, "TeamProjectPromote", "TeamProjectPromote", "TeamProjectPromoteJobExtension.Run", (Action) (() =>
      {
        TeamProjectPromoteJobData jobData = (TeamProjectPromoteJobData) null;
        try
        {
          if (TeamProjectUtilAzureBoards.IsTpcOptedOutOfPromote(requestContext))
          {
            internalResultMessage = "Opt-out of promote";
            result = TeamFoundationJobExecutionResult.Succeeded;
          }
          else
          {
            StringBuilder log = new StringBuilder();
            requestContext.Items.Add("isContextFromPromotion", (object) true);
            TeamFoundationProjectPromoteService service1 = requestContext.GetService<TeamFoundationProjectPromoteService>();
            jobData = this.GetJobData(requestContext, jobDefinition, service1);
            PromoteProjectInfo promoteProjectInfo1 = jobData.Projects.LastOrDefault<PromoteProjectInfo>();
            log.AppendLine("Template Type ID: " + jobData.ProcessTemplateTypeId.ToString());
            log.AppendLine(string.Format("Num Projects: {0}", (object) jobData.Projects.Count));
            log.AppendLine(string.Format("isXmlToInheritedPromote: {0}", (object) jobData.isXmlToInheritedPromote));
            log.AppendLine(string.Format("RemainingRetries: {0}", (object) jobData.RemainingRetries));
            foreach (PromoteProjectInfo project in jobData.Projects)
            {
              if (project.State == ProjectPromoteState.NotProcessed || project.State == ProjectPromoteState.Failed)
              {
                log.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BEGIN ProjectUri {0}]", (object) project.Id));
                string promoteLog;
                PromoteProjectInfo promoteProjectInfo2 = service1.Promote(requestContext, project.Id, project.CompletedSteps, jobData.isXmlToInheritedPromote, jobData.ProcessTemplateTypeId, out promoteLog);
                project.State = promoteProjectInfo2.State;
                project.CompletedSteps = promoteProjectInfo2.CompletedSteps;
                log.AppendLine(promoteLog);
                log.AppendLine("[END]");
                this.UpdatePromoteStatus(requestContext, jobDefinition, jobData);
                if (project.Id != promoteProjectInfo1.Id)
                  Thread.Sleep(requestContext.WitContext().ServerSettings.PromotePerProjectSleepTimeInSeconds * 1000);
              }
              else
                log.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[SKIP ProjectUri {0} with status {1}]", (object) project.Id, (object) project.State.ToString()));
            }
            if (jobData.Projects.Any<PromoteProjectInfo>((Func<PromoteProjectInfo, bool>) (p => p.CompletedSteps > 0)))
            {
              WorkItemMetadataCompatibilityService service2 = requestContext.GetService<WorkItemMetadataCompatibilityService>();
              using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
                component.DestroyMetadata();
              service2.IncreaseWorkItemMetadataBucketIds(requestContext);
            }
            if (jobData.IsSuccessful())
            {
              try
              {
                requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().SetIdentityFieldBit(requestContext);
              }
              catch (Exception ex)
              {
                internalResultMessage = this.BuildJobStateMessage(jobData, "Failed-NoRetries", log);
                this.UpdateAndRequeue(requestContext, jobDefinition, jobData, false);
                result = TeamFoundationJobExecutionResult.Failed;
                return;
              }
              internalResultMessage = this.BuildJobStateMessage(jobData, "Successful", log);
              this.UpdateAndRequeue(requestContext, jobDefinition, jobData, false);
              result = TeamFoundationJobExecutionResult.Succeeded;
            }
            else if (jobData.RemainingRetries <= 0)
            {
              internalResultMessage = this.BuildJobStateMessage(jobData, "Failed-NoRetries", log);
              this.UpdateAndRequeue(requestContext, jobDefinition, jobData, false);
              result = TeamFoundationJobExecutionResult.Failed;
            }
            else
            {
              internalResultMessage = this.BuildJobStateMessage(jobData, "Failed-Retrying", log);
              this.UpdateAndRequeue(requestContext, jobDefinition, jobData, true);
              result = TeamFoundationJobExecutionResult.Blocked;
            }
          }
        }
        catch (Exception ex1)
        {
          requestContext.TraceException(1000052, "TeamProjectPromote", "TeamProjectPromote", ex1);
          requestContext.Trace(1000057, TraceLevel.Error, "TeamProjectPromote", "TeamProjectPromote", internalResultMessage);
          try
          {
            if (jobData == null)
              return;
            this.UpdateAndRequeue(requestContext, jobDefinition, jobData, result == TeamFoundationJobExecutionResult.Blocked);
          }
          catch (Exception ex2)
          {
            requestContext.TraceException(1000053, "TeamProjectPromote", "TeamProjectPromote", ex2);
            throw ex1;
          }
        }
      }));
      resultMessage = internalResultMessage;
      return result;
    }

    private TeamProjectPromoteJobData GetJobData(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      TeamFoundationProjectPromoteService promoteService)
    {
      TeamProjectPromoteJobData jobData;
      if (jobDefinition.Data == null)
      {
        jobData = new TeamProjectPromoteJobData();
        promoteService.InitializeProjectsToPromote(requestContext, jobData);
      }
      else
        jobData = TeamFoundationSerializationUtility.Deserialize<TeamProjectPromoteJobData>(jobDefinition.Data);
      return jobData;
    }

    private string BuildJobStateMessage(
      TeamProjectPromoteJobData jobData,
      string state,
      StringBuilder log)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (PromoteProjectInfo project in jobData.Projects)
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[State={0}, Id={1}] ", (object) project.State.ToString(), (object) project.Id));
      log.Insert(0, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "State:{0} RemainingRetries={1}. Projects=[{2}]", (object) state, (object) jobData.RemainingRetries, (object) stringBuilder));
      return log.ToString();
    }

    private void UpdateAndRequeue(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      TeamProjectPromoteJobData jobData,
      bool requeue)
    {
      requestContext.TraceEnter(1000028, "TeamProjectPromote", "TeamProjectPromote", nameof (UpdateAndRequeue));
      TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
      bool flag1 = requeue && jobData.RemainingRetries > 0;
      bool flag2 = jobData.ProcessTemplateTypeId == Guid.Empty;
      if (flag1)
      {
        --jobData.RemainingRetries;
      }
      else
      {
        if (flag2)
          jobData = (TeamProjectPromoteJobData) null;
        jobDefinition.Schedule.Clear();
      }
      jobDefinition.Data = jobData != null ? TeamFoundationSerializationUtility.SerializeToXml((object) jobData) : (XmlNode) null;
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        jobDefinition
      });
      if (flag1)
      {
        if (!flag2)
        {
          service.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
          {
            jobDefinition.ToJobReference()
          });
        }
        else
        {
          int maxDelaySeconds = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Integration/Settings/PromoteRetryDelay", true, 900);
          service.QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
          {
            jobDefinition.ToJobReference()
          }, maxDelaySeconds);
        }
      }
      bool flag3 = jobData.IsSuccessful() || !flag1;
      if (((flag2 ? 0 : (jobData != null ? 1 : 0)) & (flag3 ? 1 : 0)) != 0 && !jobData.isXmlToInheritedPromote)
        requestContext.GetService<ITeamFoundationProcessService>().UpdateProcessStatus(requestContext, jobData.ProcessTemplateTypeId, ProcessStatus.Ready);
      requestContext.TraceLeave(1000029, "TeamProjectPromote", "TeamProjectPromote", nameof (UpdateAndRequeue));
    }

    private void UpdatePromoteStatus(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      TeamProjectPromoteJobData jobData)
    {
      requestContext.TraceBlock(1000032, 1000033, "TeamProjectPromote", "TeamProjectPromote", nameof (UpdatePromoteStatus), (Action) (() =>
      {
        if (!(jobData.ProcessTemplateTypeId != Guid.Empty) || jobData == null)
          return;
        TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
        jobDefinition.Data = TeamFoundationSerializationUtility.SerializeToXml((object) jobData);
        IVssRequestContext requestContext1 = requestContext;
        TeamFoundationJobDefinition[] jobUpdates = new TeamFoundationJobDefinition[1]
        {
          jobDefinition
        };
        service.UpdateJobDefinitions(requestContext1, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      }));
    }
  }
}
