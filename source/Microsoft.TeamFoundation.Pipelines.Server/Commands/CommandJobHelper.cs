// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Commands.CommandJobHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Pipelines.Server.Commands
{
  internal static class CommandJobHelper
  {
    private const string c_layer = "CommandJobHelper";
    private const string c_intervalSecondsRegistryPath = "/Service/Pipelines/PullRequestHandlerJob/IntervalSeconds";
    private const string c_jobName = "GitHub comment handler job";
    private const string c_jobClass = "Microsoft.TeamFoundation.Pipelines.Server.Extensions.CommentHandlerJobExtension";
    private const int c_jobMinimumIntervalSec = 15;
    private const int c_initialIntervalSec = 3;

    public static void ExectueCommentCommandInJob(
      IVssRequestContext requestContext,
      JObject authentication,
      IPipelineSourceProvider provider,
      ExternalPullRequestCommentEvent commentEvent)
    {
      PullRequestHandlerJobData jobData = new PullRequestHandlerJobData()
      {
        HostId = requestContext.ServiceHost.InstanceId,
        Authentication = authentication,
        ProviderId = provider.ProviderId,
        CommentEvent = commentEvent,
        ExecutionCount = 0
      };
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition handlerJobDefinition = CommandJobHelper.GetCommentHandlerJobDefinition(requestContext, jobData.SerializeToJsonXmlNode(), service.IsIgnoreDormancyPermitted);
      using (requestContext.AllowAnonymousWrites())
        service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          handlerJobDefinition
        });
    }

    public static bool IsCommentAfterCommit(
      IVssRequestContext requestContext,
      ExternalGitPullRequest pullRequest,
      string commentTimestamp)
    {
      DateTime dateTime = DateTime.Parse(commentTimestamp, (IFormatProvider) null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
      DateTime result;
      return DateTime.TryParse(pullRequest.UpdatedAt, (IFormatProvider) null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result) && result == dateTime;
    }

    public static List<string> GetCommandArguments(string arguementList)
    {
      if (string.IsNullOrWhiteSpace(arguementList))
        return new List<string>();
      return ((IEnumerable<string>) arguementList.Split(',')).Select<string, string>((Func<string, string>) (t => t.Trim())).ToList<string>();
    }

    private static TeamFoundationJobDefinition GetCommentHandlerJobDefinition(
      IVssRequestContext requestContext,
      XmlNode jobData,
      bool isIgnoreDormancyPermitted)
    {
      TeamFoundationJobDefinition handlerJobDefinition = new TeamFoundationJobDefinition();
      handlerJobDefinition.Data = jobData;
      handlerJobDefinition.EnabledState = TeamFoundationJobEnabledState.Enabled;
      handlerJobDefinition.ExtensionName = "Microsoft.TeamFoundation.Pipelines.Server.Extensions.CommentHandlerJobExtension";
      handlerJobDefinition.JobId = Guid.NewGuid();
      handlerJobDefinition.Name = "GitHub comment handler job";
      handlerJobDefinition.IgnoreDormancy = isIgnoreDormancyPermitted;
      handlerJobDefinition.PriorityClass = JobPriorityClass.Normal;
      int jobInterval = CommandJobHelper.GetJobInterval(requestContext);
      handlerJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
      {
        Interval = jobInterval,
        ScheduledTime = DateTime.UtcNow.AddSeconds(3.0),
        PriorityLevel = JobPriorityLevel.Normal
      });
      return handlerJobDefinition;
    }

    private static int GetJobInterval(IVssRequestContext requestContext)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Pipelines/PullRequestHandlerJob/IntervalSeconds", 15);
      return num < 15 ? 15 : num;
    }
  }
}
