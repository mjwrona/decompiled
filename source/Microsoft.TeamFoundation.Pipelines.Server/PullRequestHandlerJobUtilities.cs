// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PullRequestHandlerJobUtilities
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class PullRequestHandlerJobUtilities
  {
    private const string c_layer = "PullRequestHandlerJobUtilities";
    private const string c_jobName = "GitHub pull request handler job";
    private const string c_jobClass = "Microsoft.TeamFoundation.Pipelines.Server.Extensions.PullRequestHandlerJobExtension";
    private const int c_jobMinimumInterval = 15;
    private const int c_initialInterval = 5;

    public static XmlNode SerializeToJsonXmlNode(this PullRequestHandlerJobData jobData) => TeamFoundationSerializationUtility.SerializeToXml((object) JsonConvert.SerializeObject((object) jobData));

    public static PullRequestHandlerJobData DeserializeFromJsonXmlNode(XmlNode jobData) => JsonConvert.DeserializeObject<PullRequestHandlerJobData>(TeamFoundationSerializationUtility.Deserialize<string>(jobData));

    public static Guid QueuePullRequestHandlerJob(
      IVssRequestContext requestContext,
      XmlNode jobData)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition handlerJobDefinition = PullRequestHandlerJobUtilities.GetPullRequestHandlerJobDefinition(requestContext, jobData, service.IsIgnoreDormancyPermitted);
      PullRequestHandlerJobUtilities.UpdatePullRequestHandlerJob(requestContext, service, handlerJobDefinition);
      return handlerJobDefinition.JobId;
    }

    public static bool UpdateJobExecutionCount(
      this TeamFoundationJobDefinition jobDefinition,
      IVssRequestContext requestContext,
      PullRequestHandlerJobData pullRequestHandlerJobData,
      int maxExecutionCount)
    {
      ++pullRequestHandlerJobData.ExecutionCount;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      if (pullRequestHandlerJobData.ExecutionCount >= maxExecutionCount)
        return false;
      XmlNode jsonXmlNode = pullRequestHandlerJobData.SerializeToJsonXmlNode();
      jobDefinition.Data = jsonXmlNode;
      PullRequestHandlerJobUtilities.UpdatePullRequestHandlerJob(requestContext, service, jobDefinition);
      return true;
    }

    public static void DeletePullRequestHandlerJob(
      this TeamFoundationJobDefinition jobDefinition,
      IVssRequestContext requestContext)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      PullRequestHandlerJobUtilities.UpdatePullRequestHandlerJob(requestContext, service, jobDefinition, true);
    }

    public static TeamFoundationJobDefinition GetPullRequestHandlerJobDefinition(
      IVssRequestContext requestContext,
      XmlNode jobData,
      bool isIgnoreDormancyPermitted)
    {
      TeamFoundationJobDefinition handlerJobDefinition = new TeamFoundationJobDefinition();
      handlerJobDefinition.Data = jobData;
      handlerJobDefinition.EnabledState = TeamFoundationJobEnabledState.Enabled;
      handlerJobDefinition.ExtensionName = "Microsoft.TeamFoundation.Pipelines.Server.Extensions.PullRequestHandlerJobExtension";
      handlerJobDefinition.JobId = Guid.NewGuid();
      handlerJobDefinition.Name = "GitHub pull request handler job";
      handlerJobDefinition.IgnoreDormancy = isIgnoreDormancyPermitted;
      handlerJobDefinition.PriorityClass = JobPriorityClass.Normal;
      int jobInterval = GetJobInterval();
      handlerJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
      {
        Interval = jobInterval,
        ScheduledTime = DateTime.UtcNow.AddSeconds(5.0),
        PriorityLevel = JobPriorityLevel.Normal
      });
      return handlerJobDefinition;

      int GetJobInterval()
      {
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Pipelines/PullRequestHandlerJob/IntervalSeconds", 15);
        return num < 15 ? 15 : num;
      }
    }

    private static void UpdatePullRequestHandlerJob(
      IVssRequestContext requestContext,
      ITeamFoundationJobService jobService,
      TeamFoundationJobDefinition jobDefinition = null,
      bool delete = false)
    {
      using (requestContext.AllowAnonymousWrites())
      {
        if (delete)
          jobService.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            jobDefinition.JobId
          }, (IEnumerable<TeamFoundationJobDefinition>) null);
        else
          jobService.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            jobDefinition.JobId
          }, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
          {
            jobDefinition
          });
      }
    }
  }
}
