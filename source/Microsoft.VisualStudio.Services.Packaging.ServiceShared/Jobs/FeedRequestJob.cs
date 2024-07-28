// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.FeedRequestJob
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public abstract class FeedRequestJob : VssAsyncJobExtension
  {
    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      IFactory<IFeedRequest, IAsyncHandler<IFeedRequest, JobResult>> handlerFactory = this.DecorateHandler(requestContext, this.BootstrapHandler(requestContext, jobDefinition.JobId));
      (IFeedRequest feedRequest, JobResult jobResult) = await this.GetFeedRequest(requestContext, jobDefinition);
      if (feedRequest == null)
        return jobResult.ToVssJobResult();
      if (!requestContext.IsCanceled())
        return (await handlerFactory.Get(feedRequest).Handle(feedRequest)).ToVssJobResult();
      FeedJobTelemetry feedJobTelemetry = new FeedJobTelemetry();
      feedJobTelemetry.FeedId = feedRequest.Feed.Id;
      feedJobTelemetry.Message = string.Format("The requestContext got canceled. Job with id {0}, Feed id: {1}", (object) jobDefinition.JobId, (object) feedRequest.Feed.Id);
      return new VssJobResult(TeamFoundationJobExecutionResult.Stopped, feedJobTelemetry.Serialize<FeedJobTelemetry>(true));
    }

    protected virtual IFactory<IFeedRequest, IAsyncHandler<IFeedRequest, JobResult>> DecorateHandler(
      IVssRequestContext requestContext,
      IAsyncHandler<IFeedRequest, JobResult> handler)
    {
      IFeatureFlagService featureFlagFacade = requestContext.GetFeatureFlagFacade();
      return (IFactory<IFeedRequest, IAsyncHandler<IFeedRequest, JobResult>>) new ByFuncInputFactory<IFeedRequest, IAsyncHandler<IFeedRequest, JobResult>>((Func<IFeedRequest, IAsyncHandler<IFeedRequest, JobResult>>) (feedRequest => UntilNonNullHandler.Create<IFeedRequest, JobResult>((IAsyncHandler<IFeedRequest, JobResult>) new FeatureFlagCheckingJobHandler<IFeedRequest>(featureFlagFacade, feedRequest.Protocol.ReadOnlyFeatureFlagName, true), handler)));
    }

    protected abstract IAsyncHandler<IFeedRequest, JobResult> BootstrapHandler(
      IVssRequestContext requestContext,
      Guid jobId);

    protected abstract IProtocol GetProtocol();

    protected virtual bool ShouldProcessSoftDeletedFeeds { get; }

    private async Task<(IFeedRequest, JobResult)> GetFeedRequest(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      FeedRequestJob sendInTheThisObject = this;
      string feedId = FeedRequestJob.ParseFeedJobDefinition(jobDefinition.Data.InnerText);
      IProtocol protocol = sendInTheThisObject.GetProtocol();
      JobTelemetry.SetRequestContextEtwTracingData(requestContext, feedId, protocol);
      (IFeedRequest, JobResult) feedRequest1;
      using (requestContext.GetTracerFacade().Enter((object) sendInTheThisObject, nameof (GetFeedRequest)))
      {
        IFeedService feedServiceFacade = (IFeedService) new FeedServiceFacade(requestContext);
        FeedJobTelemetry result = new FeedJobTelemetry()
        {
          FeedId = Guid.Parse(feedId)
        };
        IFeedRequest feedRequest = (IFeedRequest) null;
        FeedCore feed = (FeedCore) null;
        JobResult jobResult = await new JobErrorHandlerBootstrapper<NullRequest>(requestContext, (IAsyncHandler<NullRequest, JobResult>) new ByFuncAsyncHandler<NullRequest, JobResult>((Func<NullRequest, JobResult>) (r =>
        {
          feed = feedServiceFacade.GetFeedByIdForAnyScope(Guid.Parse(feedId), this.ShouldProcessSoftDeletedFeeds);
          feedRequest = (IFeedRequest) new FeedRequest(feed, protocol);
          return JobResult.Succeeded((JobTelemetry) result);
        })), (JobId) jobDefinition.JobId).Bootstrap().Handle((NullRequest) null);
        if (feed != null)
        {
          requestContext.SetFeedForPackagingTraces(feed);
        }
        else
        {
          FeedJobTelemetry feedJobTelemetry1 = new FeedJobTelemetry();
          feedJobTelemetry1.FeedId = Guid.Parse(feedId);
          feedJobTelemetry1.Message = jobResult.Telemetry?.Message;
          FeedJobTelemetry feedJobTelemetry2 = feedJobTelemetry1;
          feedJobTelemetry2.LogException(jobResult.Telemetry?.Exception);
          jobResult = new JobResult()
          {
            Result = jobResult.Result,
            Telemetry = (JobTelemetry) feedJobTelemetry2
          };
        }
        if (jobResult?.Telemetry?.Exception?.GetType() == typeof (FeedIdNotFoundException))
          await sendInTheThisObject.RemediateDeletedFeed(requestContext, feedId);
        feedRequest1 = (feedRequest, jobResult);
      }
      return feedRequest1;
    }

    protected virtual Task RemediateDeletedFeed(IVssRequestContext requestContext, string feedId) => Task.CompletedTask;

    public static string ParseFeedJobDefinition(string jobDataInnerText) => jobDataInnerText.Split(',')[0];

    public static XmlNode SerializeFeedJobDefinition(FeedCore feed) => TeamFoundationSerializationUtility.SerializeToXml((object) feed.Id);
  }
}
