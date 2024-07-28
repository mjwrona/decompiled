// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.FeedJobQueuerFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class FeedJobQueuerFacade : IFeedJobQueuer
  {
    private readonly ITeamFoundationJobService jobService;
    private readonly IVssRequestContext requestContext;
    private readonly IFeedJobIdMap jobIdMap;
    private readonly JobCreationInfo jobCreationInfo;

    public FeedJobQueuerFacade(
      IVssRequestContext requestContext,
      IFeedJobIdMap jobIdMap,
      JobCreationInfo jobCreationInfo)
    {
      this.jobService = requestContext.GetService<ITeamFoundationJobService>();
      this.requestContext = requestContext;
      this.jobIdMap = jobIdMap;
      this.jobCreationInfo = jobCreationInfo;
    }

    public async Task<Guid> QueueJob(
      FeedCore feed,
      IProtocol protocol,
      JobPriorityLevel jobPriority,
      int maxDelaySeconds = 0)
    {
      FeedJobQueuerFacade feedJobQueuerFacade = this;
      feedJobQueuerFacade.requestContext.CheckServiceHostType(feedJobQueuerFacade.jobCreationInfo.HostType, "class: " + feedJobQueuerFacade.GetType().FullName + "; jobCreationInfo : " + JsonConvert.SerializeObject((object) feedJobQueuerFacade.jobCreationInfo));
      FeedJobMapEntry jobId = await feedJobQueuerFacade.jobIdMap.GetJobId((IFeedRequest) new FeedRequest(feed, protocol));
      if (jobId == null || jobId.JobId == Guid.Empty)
        return Guid.Empty;
      List<TeamFoundationJobReference> jobReferences = new List<TeamFoundationJobReference>()
      {
        new TeamFoundationJobReference(jobId.JobId)
      };
      try
      {
        feedJobQueuerFacade.jobService.QueueJobs(feedJobQueuerFacade.requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, jobPriority, maxDelaySeconds, false);
      }
      catch (JobDefinitionNotFoundException ex)
      {
        feedJobQueuerFacade.CreateJobDefinition(feed, protocol, jobId);
        feedJobQueuerFacade.jobService.QueueJobs(feedJobQueuerFacade.requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, jobPriority, maxDelaySeconds, false);
      }
      return jobId.JobId;
    }

    private void CreateJobDefinition(
      FeedCore feed,
      IProtocol protocol,
      FeedJobMapEntry feedJobMapEntry)
    {
      this.jobService.UpdateJobDefinitions(this.requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
      {
        new TeamFoundationJobDefinition(feedJobMapEntry.JobId, string.Format("{0}_{1}_{2}", (object) this.jobCreationInfo.JobName, (object) feed.Id, (object) protocol), this.jobCreationInfo.JobExtensionName, FeedRequestJob.SerializeFeedJobDefinition(feed), TeamFoundationJobEnabledState.Enabled, false, this.jobCreationInfo.PriorityClass)
      });
    }
  }
}
