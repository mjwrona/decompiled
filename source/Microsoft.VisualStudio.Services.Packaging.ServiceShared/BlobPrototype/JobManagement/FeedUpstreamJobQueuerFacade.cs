// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.FeedUpstreamJobQueuerFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class FeedUpstreamJobQueuerFacade : IFeedUpstreamRefreshJobQueuer
  {
    private readonly ITeamFoundationJobService jobService;
    private readonly IVssRequestContext requestContext;
    private readonly IFeedJobIdMap jobIdMap;
    private readonly JobCreationInfo jobCreationInfo;

    public FeedUpstreamJobQueuerFacade(
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
      UpstreamPackagesToRefreshInformation toRefreshInformation,
      Guid? correlationId = null,
      int numSplitJobs = 1,
      int maxDelaySeconds = 0)
    {
      FeedUpstreamJobQueuerFacade upstreamJobQueuerFacade = this;
      upstreamJobQueuerFacade.requestContext.CheckServiceHostType(upstreamJobQueuerFacade.jobCreationInfo.HostType, "class: " + upstreamJobQueuerFacade.GetType().FullName + "; jobCreationInfo : " + JsonConvert.SerializeObject((object) upstreamJobQueuerFacade.jobCreationInfo));
      FeedJobMapEntry jobId = await upstreamJobQueuerFacade.jobIdMap.GetJobId((IFeedRequest) new FeedRequest(feed, protocol));
      if (jobId == null || jobId.JobId == Guid.Empty)
        return Guid.Empty;
      new List<TeamFoundationJobReference>()
      {
        new TeamFoundationJobReference(jobId.JobId)
      };
      FeedUpstreamRequestJobData objectToSerialize = new FeedUpstreamRequestJobData(toRefreshInformation, correlationId ?? Guid.Empty, numSplitJobs);
      if (upstreamJobQueuerFacade.requestContext.IsFeatureEnabledWithLogging("Packaging.IgnoreUpstreamJobMaxDelaySeconds"))
        maxDelaySeconds = 0;
      upstreamJobQueuerFacade.jobService.QueueOneTimeJob(upstreamJobQueuerFacade.requestContext, string.Format("{0}_{1}_{2}", (object) upstreamJobQueuerFacade.jobCreationInfo.JobName, (object) feed.Id, (object) protocol), upstreamJobQueuerFacade.jobCreationInfo.JobExtensionName, TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize), jobPriority, TimeSpan.FromSeconds((double) maxDelaySeconds));
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

    private XmlNode returnFeedIdAndPackageList(FeedCore feed, List<IPackageName> packageNames)
    {
      Guid id = feed.Id;
      IPackageName packageName1 = packageNames.First<IPackageName>();
      IPackageName packageName2 = packageNames.Last<IPackageName>();
      IPackageName firstPackage = packageName1;
      IPackageName lastPackage = packageName2;
      return TeamFoundationSerializationUtility.SerializeToXml((object) new UpstreamPackagesToRefreshInformation(id, firstPackage, lastPackage));
    }
  }
}
