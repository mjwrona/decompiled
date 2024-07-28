// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.FeedJobDefinitionFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public class FeedJobDefinitionFactory
  {
    public static TeamFoundationJobDefinition CreateJobDefinition(
      IVssRequestContext requestContext,
      IFeedJobDefinitionProvider jobDefinition,
      IFeedRequest feedRequest,
      JobPriorityClass jobPriorityClass = JobPriorityClass.AboveNormal)
    {
      FeedCore feed = feedRequest.Feed;
      Guid jobId = jobDefinition.GetJobId(requestContext, feedRequest);
      if (jobId == Guid.Empty)
        return (TeamFoundationJobDefinition) null;
      string name = string.Format("{0}_Feed_{1:N}", (object) jobDefinition.JobPrefix, (object) feed.Id);
      return new TeamFoundationJobDefinition(jobId, name, jobDefinition.Extension, FeedRequestJob.SerializeFeedJobDefinition(feed), TeamFoundationJobEnabledState.Enabled, false, jobPriorityClass);
    }

    public static IEnumerable<TeamFoundationJobDefinition> CreateJobDefinitions(
      IVssRequestContext requestContext,
      IProtocol protocol,
      IFeedJobDefinitionProvider jobDefinition,
      JobPriorityClass jobPriorityClass = JobPriorityClass.AboveNormal)
    {
      List<TeamFoundationJobDefinition> jobDefinitions = new List<TeamFoundationJobDefinition>();
      IFeedClientService feedClientService = requestContext.GetService<IFeedClientService>();
      foreach (Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed in AsyncPump.Run<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>((Func<Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>>) (() => feedClientService.GetFeedsAsync(requestContext))))
      {
        TeamFoundationJobDefinition jobDefinition1 = FeedJobDefinitionFactory.CreateJobDefinition(requestContext, jobDefinition, (IFeedRequest) new FeedRequest((FeedCore) feed, protocol), jobPriorityClass);
        if (jobDefinition1 != null)
          jobDefinitions.Add(jobDefinition1);
      }
      return (IEnumerable<TeamFoundationJobDefinition>) jobDefinitions;
    }
  }
}
