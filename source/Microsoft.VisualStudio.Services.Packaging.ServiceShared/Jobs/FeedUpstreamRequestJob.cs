// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.FeedUpstreamRequestJob
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public abstract class FeedUpstreamRequestJob : VssAsyncJobExtension
  {
    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      IFactory<IFeedRequest<FeedUpstreamRequestJobData>, IAsyncHandler<IFeedRequest<FeedUpstreamRequestJobData>, JobResult>> handlerFactory = this.DecorateHandler(requestContext, this.BootstrapHandler(requestContext, jobDefinition.JobId));
      (IFeedRequest<FeedUpstreamRequestJobData> feedRequest, JobResult jobResult) = await this.GetFeedRequest(requestContext, jobDefinition);
      if (feedRequest == null)
        return jobResult.ToVssJobResult();
      if (!requestContext.IsCanceled())
        return (await handlerFactory.Get(feedRequest).Handle(feedRequest)).ToVssJobResult();
      FeedJobTelemetry feedJobTelemetry = new FeedJobTelemetry();
      feedJobTelemetry.FeedId = feedRequest.Feed.Id;
      feedJobTelemetry.Message = string.Format("The requestContext got canceled. Job with id {0}, Feed id: {1}", (object) jobDefinition.JobId, (object) feedRequest.Feed.Id);
      return new VssJobResult(TeamFoundationJobExecutionResult.Stopped, feedJobTelemetry.Serialize<FeedJobTelemetry>(true));
    }

    protected virtual IFactory<IFeedRequest<FeedUpstreamRequestJobData>, IAsyncHandler<IFeedRequest<FeedUpstreamRequestJobData>, JobResult>> DecorateHandler(
      IVssRequestContext requestContext,
      IAsyncHandler<IFeedRequest<FeedUpstreamRequestJobData>, JobResult> handler)
    {
      IFeatureFlagService featureFlagFacade = requestContext.GetFeatureFlagFacade();
      return (IFactory<IFeedRequest<FeedUpstreamRequestJobData>, IAsyncHandler<IFeedRequest<FeedUpstreamRequestJobData>, JobResult>>) new ByFuncInputFactory<IFeedRequest<FeedUpstreamRequestJobData>, IAsyncHandler<IFeedRequest<FeedUpstreamRequestJobData>, JobResult>>((Func<IFeedRequest<FeedUpstreamRequestJobData>, IAsyncHandler<IFeedRequest<FeedUpstreamRequestJobData>, JobResult>>) (feedRequest => UntilNonNullHandler.Create<IFeedRequest<FeedUpstreamRequestJobData>, JobResult>((IAsyncHandler<IFeedRequest<FeedUpstreamRequestJobData>, JobResult>) new FeatureFlagCheckingJobHandler<IFeedRequest<FeedUpstreamRequestJobData>>(featureFlagFacade, feedRequest.Protocol.ReadOnlyFeatureFlagName, true), handler)));
    }

    protected abstract IAsyncHandler<IFeedRequest<FeedUpstreamRequestJobData>, JobResult> BootstrapHandler(
      IVssRequestContext requestContext,
      Guid jobId);

    protected abstract IProtocol GetProtocol();

    protected void SetJobTimeout(IVssRequestContext requestContext)
    {
      int num = new RegistryServiceFacade(requestContext).GetValue<int>((RegistryQuery) string.Format("/Configuration/Packaging/{0}/UpstreamMetadataCache/RefreshFeedJobTimeoutSeconds", (object) this.GetProtocol().CorrectlyCasedName), 10200, true);
      requestContext.RequestTimeout = TimeSpan.FromSeconds((double) num);
    }

    protected virtual bool ShouldProcessSoftDeletedFeeds { get; }

    private async Task<(IFeedRequest<FeedUpstreamRequestJobData>, JobResult)> GetFeedRequest(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      FeedUpstreamRequestJob sendInTheThisObject = this;
      FeedUpstreamRequestJobData jobData = FeedUpstreamRequestJob.ParseFeedJobDefinition(jobDefinition.Data);
      Guid feedIdGuid = jobData.UpstreamPackagesToRefresh.FeedId;
      IProtocol protocol = sendInTheThisObject.GetProtocol();
      JobTelemetry.SetRequestContextEtwTracingData(requestContext, feedIdGuid.ToString(), protocol);
      (IFeedRequest<FeedUpstreamRequestJobData>, JobResult) feedRequest1;
      using (requestContext.GetTracerFacade().Enter((object) sendInTheThisObject, nameof (GetFeedRequest)))
      {
        IFeedService feedServiceFacade = (IFeedService) new FeedServiceFacade(requestContext);
        FeedJobTelemetry result = new FeedJobTelemetry()
        {
          FeedId = feedIdGuid
        };
        IFeedRequest<FeedUpstreamRequestJobData> feedRequest = (IFeedRequest<FeedUpstreamRequestJobData>) null;
        FeedCore feed = (FeedCore) null;
        JobResult jobResult = await new JobErrorHandlerBootstrapper<NullRequest>(requestContext, (IAsyncHandler<NullRequest, JobResult>) new ByFuncAsyncHandler<NullRequest, JobResult>((Func<NullRequest, JobResult>) (r =>
        {
          feed = feedServiceFacade.GetFeedByIdForAnyScope(feedIdGuid, this.ShouldProcessSoftDeletedFeeds);
          feedRequest = new FeedRequest(feed, protocol).WithData<FeedUpstreamRequestJobData>(jobData);
          return JobResult.Succeeded((JobTelemetry) result);
        })), (JobId) jobDefinition.JobId).Bootstrap().Handle((NullRequest) null);
        if (feed != null)
        {
          requestContext.SetFeedForPackagingTraces(feed);
        }
        else
        {
          FeedJobTelemetry feedJobTelemetry1 = new FeedJobTelemetry();
          feedJobTelemetry1.FeedId = feedIdGuid;
          feedJobTelemetry1.Message = jobResult.Telemetry?.Message;
          FeedJobTelemetry feedJobTelemetry2 = feedJobTelemetry1;
          feedJobTelemetry2.LogException(jobResult.Telemetry?.Exception);
          jobResult = new JobResult()
          {
            Result = jobResult.Result,
            Telemetry = (JobTelemetry) feedJobTelemetry2
          };
        }
        feedRequest1 = (feedRequest, jobResult);
      }
      return feedRequest1;
    }

    private static FeedUpstreamRequestJobData ParseFeedJobDefinition(XmlNode jobData)
    {
      if (!(jobData is XmlElement xmlElement))
        throw new InvalidOperationException("Expected job data to be an XML element");
      switch (xmlElement.Name)
      {
        case "guid":
          string innerText = xmlElement.InnerText;
          Guid result1;
          if (!Guid.TryParse(innerText, out result1) || result1 == Guid.Empty)
            throw new InvalidOperationException("Failed to parse '" + innerText + "' as a feed GUID (guid-form job data: " + jobData.OuterXml + ")");
          return new FeedUpstreamRequestJobData(new UpstreamPackagesToRefreshInformation(result1, (IPackageName) null, (IPackageName) null), Guid.Empty, 0);
        case "string":
          string input = xmlElement.InnerText.Split(',')[0];
          Guid result2;
          if (!Guid.TryParse(input, out result2) || result2 == Guid.Empty)
            throw new InvalidOperationException("Failed to parse '" + input + "' as a feed GUID (string-form job data: " + jobData.OuterXml + ")");
          return new FeedUpstreamRequestJobData(new UpstreamPackagesToRefreshInformation(result2, (IPackageName) null, (IPackageName) null), Guid.Empty, 0);
        case "UpstreamPackagesToRefreshInformation":
          UpstreamPackagesToRefreshInformation upstreamPackagesToRefresh = TeamFoundationSerializationUtility.Deserialize<UpstreamPackagesToRefreshInformation>(jobData);
          if (upstreamPackagesToRefresh.FeedId == Guid.Empty)
            throw new InvalidOperationException("Job data did not contain a valid feed ID: " + jobData.OuterXml);
          return new FeedUpstreamRequestJobData(upstreamPackagesToRefresh, Guid.Empty, 0);
        case "FeedUpstreamRequestJobData":
          FeedUpstreamRequestJobData feedJobDefinition = TeamFoundationSerializationUtility.Deserialize<FeedUpstreamRequestJobData>(jobData);
          if (feedJobDefinition.UpstreamPackagesToRefresh == null)
            throw new InvalidOperationException("Job data did not contain upstream packages to refresh: " + jobData.OuterXml);
          if (feedJobDefinition.UpstreamPackagesToRefresh.FeedId == Guid.Empty)
            throw new InvalidOperationException("Job data did not contain a valid feed ID: " + jobData.OuterXml);
          return feedJobDefinition;
        default:
          throw new InvalidOperationException("Could not determine format of job data: " + jobData.OuterXml);
      }
    }
  }
}
