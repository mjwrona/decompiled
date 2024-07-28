// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionServiceJobHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestExecutionServiceJobHelper : ITestExecutionServiceJobHelper
  {
    public Guid QueueWorkflowJob(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TeamProjectReference project)
    {
      XmlNode forTestWorkflowJob = this.GetXmlJobDataForTestWorkflowJob(requestContext, testRunId);
      TimeSpan zero = TimeSpan.Zero;
      return requestContext.RequestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext.RequestContext, requestContext.WorkFlowJobName, requestContext.WorkFlowJobExtensionName, forTestWorkflowJob, JobPriorityLevel.Highest, zero);
    }

    public void WakeUpWorkflowJob(
      TestExecutionRequestContext requestContext,
      Guid jobGuid,
      int testRunId)
    {
      int num = requestContext.RequestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext.RequestContext, (IEnumerable<Guid>) new List<Guid>()
      {
        jobGuid
      });
      if (num == 1)
        return;
      new DtaLogger(requestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer).Error(6200241, "Failed to wakeup the test workflow job. TestRunId : {0}, JobsQueued : {1}", (object) testRunId, (object) num);
    }

    public void QueueDelayedJobs(
      TestExecutionRequestContext requestContext,
      IEnumerable<Guid> jobIds,
      int maxDelaySeconds,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal)
    {
      requestContext.RequestContext.GetService<TeamFoundationJobService>().QueueDelayedJobs(requestContext.RequestContext, jobIds, maxDelaySeconds);
    }

    public TimeSpan GetAutomatedTestRunTimeout(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      TimeSpan result = TestRunPropertiesService.GetTestRunTimeout(requestContext, testRunId);
      if (result == TimeSpan.Zero)
      {
        string s = requestContext.RequestContext.GetService<IVssRegistryService>().GetValue(requestContext.RequestContext, (RegistryQuery) DtaConstants.TfsRegistryPathForRunTimeOut, false, (string) null);
        new DtaLogger(requestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer).Info(6200205, "RegistryValue : {0}\nTimestamp: {1}\nTcmRunId : {2}", (object) s, (object) DateTimeOffset.Now, (object) testRunId);
        if (!TimeSpan.TryParse(s, out result))
          result = DtaConstants.DefaultTestRunTimeout;
      }
      return result;
    }

    public XmlNode GetXmlJobDataForTestWorkflowJob(
      TestExecutionRequestContext context,
      int testRunId)
    {
      return TeamFoundationSerializationUtility.SerializeToXml((object) new WorkflowJobData()
      {
        TestRunId = testRunId
      });
    }
  }
}
