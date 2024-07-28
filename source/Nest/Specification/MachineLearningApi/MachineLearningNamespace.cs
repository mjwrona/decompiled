// Decompiled with JetBrains decompiler
// Type: Nest.Specification.MachineLearningApi.MachineLearningNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.MachineLearningApi
{
  public class MachineLearningNamespace : Nest.NamespacedClientProxy
  {
    internal MachineLearningNamespace(ElasticClient client)
      : base(client)
    {
    }

    public CloseJobResponse CloseJob(
      Id jobId,
      Func<CloseJobDescriptor, ICloseJobRequest> selector = null)
    {
      return this.CloseJob(selector.InvokeOrDefault<CloseJobDescriptor, ICloseJobRequest>(new CloseJobDescriptor(jobId)));
    }

    public Task<CloseJobResponse> CloseJobAsync(
      Id jobId,
      Func<CloseJobDescriptor, ICloseJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CloseJobAsync(selector.InvokeOrDefault<CloseJobDescriptor, ICloseJobRequest>(new CloseJobDescriptor(jobId)), ct);
    }

    public CloseJobResponse CloseJob(ICloseJobRequest request) => this.DoRequest<ICloseJobRequest, CloseJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CloseJobResponse> CloseJobAsync(ICloseJobRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ICloseJobRequest, CloseJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public DeleteCalendarResponse DeleteCalendar(
      Id calendarId,
      Func<DeleteCalendarDescriptor, IDeleteCalendarRequest> selector = null)
    {
      return this.DeleteCalendar(selector.InvokeOrDefault<DeleteCalendarDescriptor, IDeleteCalendarRequest>(new DeleteCalendarDescriptor(calendarId)));
    }

    public Task<DeleteCalendarResponse> DeleteCalendarAsync(
      Id calendarId,
      Func<DeleteCalendarDescriptor, IDeleteCalendarRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteCalendarAsync(selector.InvokeOrDefault<DeleteCalendarDescriptor, IDeleteCalendarRequest>(new DeleteCalendarDescriptor(calendarId)), ct);
    }

    public DeleteCalendarResponse DeleteCalendar(IDeleteCalendarRequest request) => this.DoRequest<IDeleteCalendarRequest, DeleteCalendarResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteCalendarResponse> DeleteCalendarAsync(
      IDeleteCalendarRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteCalendarRequest, DeleteCalendarResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteCalendarEventResponse DeleteCalendarEvent(
      Id calendarId,
      Id eventId,
      Func<DeleteCalendarEventDescriptor, IDeleteCalendarEventRequest> selector = null)
    {
      return this.DeleteCalendarEvent(selector.InvokeOrDefault<DeleteCalendarEventDescriptor, IDeleteCalendarEventRequest>(new DeleteCalendarEventDescriptor(calendarId, eventId)));
    }

    public Task<DeleteCalendarEventResponse> DeleteCalendarEventAsync(
      Id calendarId,
      Id eventId,
      Func<DeleteCalendarEventDescriptor, IDeleteCalendarEventRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteCalendarEventAsync(selector.InvokeOrDefault<DeleteCalendarEventDescriptor, IDeleteCalendarEventRequest>(new DeleteCalendarEventDescriptor(calendarId, eventId)), ct);
    }

    public DeleteCalendarEventResponse DeleteCalendarEvent(IDeleteCalendarEventRequest request) => this.DoRequest<IDeleteCalendarEventRequest, DeleteCalendarEventResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteCalendarEventResponse> DeleteCalendarEventAsync(
      IDeleteCalendarEventRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteCalendarEventRequest, DeleteCalendarEventResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteCalendarJobResponse DeleteCalendarJob(
      Id calendarId,
      Id jobId,
      Func<DeleteCalendarJobDescriptor, IDeleteCalendarJobRequest> selector = null)
    {
      return this.DeleteCalendarJob(selector.InvokeOrDefault<DeleteCalendarJobDescriptor, IDeleteCalendarJobRequest>(new DeleteCalendarJobDescriptor(calendarId, jobId)));
    }

    public Task<DeleteCalendarJobResponse> DeleteCalendarJobAsync(
      Id calendarId,
      Id jobId,
      Func<DeleteCalendarJobDescriptor, IDeleteCalendarJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteCalendarJobAsync(selector.InvokeOrDefault<DeleteCalendarJobDescriptor, IDeleteCalendarJobRequest>(new DeleteCalendarJobDescriptor(calendarId, jobId)), ct);
    }

    public DeleteCalendarJobResponse DeleteCalendarJob(IDeleteCalendarJobRequest request) => this.DoRequest<IDeleteCalendarJobRequest, DeleteCalendarJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteCalendarJobResponse> DeleteCalendarJobAsync(
      IDeleteCalendarJobRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteCalendarJobRequest, DeleteCalendarJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteDatafeedResponse DeleteDatafeed(
      Id datafeedId,
      Func<DeleteDatafeedDescriptor, IDeleteDatafeedRequest> selector = null)
    {
      return this.DeleteDatafeed(selector.InvokeOrDefault<DeleteDatafeedDescriptor, IDeleteDatafeedRequest>(new DeleteDatafeedDescriptor(datafeedId)));
    }

    public Task<DeleteDatafeedResponse> DeleteDatafeedAsync(
      Id datafeedId,
      Func<DeleteDatafeedDescriptor, IDeleteDatafeedRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteDatafeedAsync(selector.InvokeOrDefault<DeleteDatafeedDescriptor, IDeleteDatafeedRequest>(new DeleteDatafeedDescriptor(datafeedId)), ct);
    }

    public DeleteDatafeedResponse DeleteDatafeed(IDeleteDatafeedRequest request) => this.DoRequest<IDeleteDatafeedRequest, DeleteDatafeedResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteDatafeedResponse> DeleteDatafeedAsync(
      IDeleteDatafeedRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteDatafeedRequest, DeleteDatafeedResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteExpiredDataResponse DeleteExpiredData(
      Func<DeleteExpiredDataDescriptor, IDeleteExpiredDataRequest> selector = null)
    {
      return this.DeleteExpiredData(selector.InvokeOrDefault<DeleteExpiredDataDescriptor, IDeleteExpiredDataRequest>(new DeleteExpiredDataDescriptor()));
    }

    public Task<DeleteExpiredDataResponse> DeleteExpiredDataAsync(
      Func<DeleteExpiredDataDescriptor, IDeleteExpiredDataRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteExpiredDataAsync(selector.InvokeOrDefault<DeleteExpiredDataDescriptor, IDeleteExpiredDataRequest>(new DeleteExpiredDataDescriptor()), ct);
    }

    public DeleteExpiredDataResponse DeleteExpiredData(IDeleteExpiredDataRequest request) => this.DoRequest<IDeleteExpiredDataRequest, DeleteExpiredDataResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteExpiredDataResponse> DeleteExpiredDataAsync(
      IDeleteExpiredDataRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteExpiredDataRequest, DeleteExpiredDataResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteFilterResponse DeleteFilter(
      Id filterId,
      Func<DeleteFilterDescriptor, IDeleteFilterRequest> selector = null)
    {
      return this.DeleteFilter(selector.InvokeOrDefault<DeleteFilterDescriptor, IDeleteFilterRequest>(new DeleteFilterDescriptor(filterId)));
    }

    public Task<DeleteFilterResponse> DeleteFilterAsync(
      Id filterId,
      Func<DeleteFilterDescriptor, IDeleteFilterRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteFilterAsync(selector.InvokeOrDefault<DeleteFilterDescriptor, IDeleteFilterRequest>(new DeleteFilterDescriptor(filterId)), ct);
    }

    public DeleteFilterResponse DeleteFilter(IDeleteFilterRequest request) => this.DoRequest<IDeleteFilterRequest, DeleteFilterResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteFilterResponse> DeleteFilterAsync(
      IDeleteFilterRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteFilterRequest, DeleteFilterResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteForecastResponse DeleteForecast(
      Id jobId,
      Ids forecastId,
      Func<DeleteForecastDescriptor, IDeleteForecastRequest> selector = null)
    {
      return this.DeleteForecast(selector.InvokeOrDefault<DeleteForecastDescriptor, IDeleteForecastRequest>(new DeleteForecastDescriptor(jobId, forecastId)));
    }

    public Task<DeleteForecastResponse> DeleteForecastAsync(
      Id jobId,
      Ids forecastId,
      Func<DeleteForecastDescriptor, IDeleteForecastRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteForecastAsync(selector.InvokeOrDefault<DeleteForecastDescriptor, IDeleteForecastRequest>(new DeleteForecastDescriptor(jobId, forecastId)), ct);
    }

    public DeleteForecastResponse DeleteForecast(IDeleteForecastRequest request) => this.DoRequest<IDeleteForecastRequest, DeleteForecastResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteForecastResponse> DeleteForecastAsync(
      IDeleteForecastRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteForecastRequest, DeleteForecastResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteJobResponse DeleteJob(
      Id jobId,
      Func<DeleteJobDescriptor, IDeleteJobRequest> selector = null)
    {
      return this.DeleteJob(selector.InvokeOrDefault<DeleteJobDescriptor, IDeleteJobRequest>(new DeleteJobDescriptor(jobId)));
    }

    public Task<DeleteJobResponse> DeleteJobAsync(
      Id jobId,
      Func<DeleteJobDescriptor, IDeleteJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteJobAsync(selector.InvokeOrDefault<DeleteJobDescriptor, IDeleteJobRequest>(new DeleteJobDescriptor(jobId)), ct);
    }

    public DeleteJobResponse DeleteJob(IDeleteJobRequest request) => this.DoRequest<IDeleteJobRequest, DeleteJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteJobResponse> DeleteJobAsync(IDeleteJobRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IDeleteJobRequest, DeleteJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public DeleteModelSnapshotResponse DeleteModelSnapshot(
      Id jobId,
      Id snapshotId,
      Func<DeleteModelSnapshotDescriptor, IDeleteModelSnapshotRequest> selector = null)
    {
      return this.DeleteModelSnapshot(selector.InvokeOrDefault<DeleteModelSnapshotDescriptor, IDeleteModelSnapshotRequest>(new DeleteModelSnapshotDescriptor(jobId, snapshotId)));
    }

    public Task<DeleteModelSnapshotResponse> DeleteModelSnapshotAsync(
      Id jobId,
      Id snapshotId,
      Func<DeleteModelSnapshotDescriptor, IDeleteModelSnapshotRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteModelSnapshotAsync(selector.InvokeOrDefault<DeleteModelSnapshotDescriptor, IDeleteModelSnapshotRequest>(new DeleteModelSnapshotDescriptor(jobId, snapshotId)), ct);
    }

    public DeleteModelSnapshotResponse DeleteModelSnapshot(IDeleteModelSnapshotRequest request) => this.DoRequest<IDeleteModelSnapshotRequest, DeleteModelSnapshotResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteModelSnapshotResponse> DeleteModelSnapshotAsync(
      IDeleteModelSnapshotRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteModelSnapshotRequest, DeleteModelSnapshotResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public EstimateModelMemoryResponse EstimateModelMemory<TDocument>(
      Func<EstimateModelMemoryDescriptor<TDocument>, IEstimateModelMemoryRequest> selector)
      where TDocument : class
    {
      return this.EstimateModelMemory(selector.InvokeOrDefault<EstimateModelMemoryDescriptor<TDocument>, IEstimateModelMemoryRequest>(new EstimateModelMemoryDescriptor<TDocument>()));
    }

    public Task<EstimateModelMemoryResponse> EstimateModelMemoryAsync<TDocument>(
      Func<EstimateModelMemoryDescriptor<TDocument>, IEstimateModelMemoryRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.EstimateModelMemoryAsync(selector.InvokeOrDefault<EstimateModelMemoryDescriptor<TDocument>, IEstimateModelMemoryRequest>(new EstimateModelMemoryDescriptor<TDocument>()), ct);
    }

    public EstimateModelMemoryResponse EstimateModelMemory(IEstimateModelMemoryRequest request) => this.DoRequest<IEstimateModelMemoryRequest, EstimateModelMemoryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<EstimateModelMemoryResponse> EstimateModelMemoryAsync(
      IEstimateModelMemoryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IEstimateModelMemoryRequest, EstimateModelMemoryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public FlushJobResponse FlushJob(
      Id jobId,
      Func<FlushJobDescriptor, IFlushJobRequest> selector = null)
    {
      return this.FlushJob(selector.InvokeOrDefault<FlushJobDescriptor, IFlushJobRequest>(new FlushJobDescriptor(jobId)));
    }

    public Task<FlushJobResponse> FlushJobAsync(
      Id jobId,
      Func<FlushJobDescriptor, IFlushJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.FlushJobAsync(selector.InvokeOrDefault<FlushJobDescriptor, IFlushJobRequest>(new FlushJobDescriptor(jobId)), ct);
    }

    public FlushJobResponse FlushJob(IFlushJobRequest request) => this.DoRequest<IFlushJobRequest, FlushJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<FlushJobResponse> FlushJobAsync(IFlushJobRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IFlushJobRequest, FlushJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ForecastJobResponse ForecastJob(
      Id jobId,
      Func<ForecastJobDescriptor, IForecastJobRequest> selector = null)
    {
      return this.ForecastJob(selector.InvokeOrDefault<ForecastJobDescriptor, IForecastJobRequest>(new ForecastJobDescriptor(jobId)));
    }

    public Task<ForecastJobResponse> ForecastJobAsync(
      Id jobId,
      Func<ForecastJobDescriptor, IForecastJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ForecastJobAsync(selector.InvokeOrDefault<ForecastJobDescriptor, IForecastJobRequest>(new ForecastJobDescriptor(jobId)), ct);
    }

    public ForecastJobResponse ForecastJob(IForecastJobRequest request) => this.DoRequest<IForecastJobRequest, ForecastJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ForecastJobResponse> ForecastJobAsync(
      IForecastJobRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IForecastJobRequest, ForecastJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetBucketsResponse GetBuckets(
      Id jobId,
      Func<GetBucketsDescriptor, IGetBucketsRequest> selector = null)
    {
      return this.GetBuckets(selector.InvokeOrDefault<GetBucketsDescriptor, IGetBucketsRequest>(new GetBucketsDescriptor(jobId)));
    }

    public Task<GetBucketsResponse> GetBucketsAsync(
      Id jobId,
      Func<GetBucketsDescriptor, IGetBucketsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetBucketsAsync(selector.InvokeOrDefault<GetBucketsDescriptor, IGetBucketsRequest>(new GetBucketsDescriptor(jobId)), ct);
    }

    public GetBucketsResponse GetBuckets(IGetBucketsRequest request) => this.DoRequest<IGetBucketsRequest, GetBucketsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetBucketsResponse> GetBucketsAsync(
      IGetBucketsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetBucketsRequest, GetBucketsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetCalendarEventsResponse GetCalendarEvents(
      Id calendarId,
      Func<GetCalendarEventsDescriptor, IGetCalendarEventsRequest> selector = null)
    {
      return this.GetCalendarEvents(selector.InvokeOrDefault<GetCalendarEventsDescriptor, IGetCalendarEventsRequest>(new GetCalendarEventsDescriptor(calendarId)));
    }

    public Task<GetCalendarEventsResponse> GetCalendarEventsAsync(
      Id calendarId,
      Func<GetCalendarEventsDescriptor, IGetCalendarEventsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetCalendarEventsAsync(selector.InvokeOrDefault<GetCalendarEventsDescriptor, IGetCalendarEventsRequest>(new GetCalendarEventsDescriptor(calendarId)), ct);
    }

    public GetCalendarEventsResponse GetCalendarEvents(IGetCalendarEventsRequest request) => this.DoRequest<IGetCalendarEventsRequest, GetCalendarEventsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetCalendarEventsResponse> GetCalendarEventsAsync(
      IGetCalendarEventsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetCalendarEventsRequest, GetCalendarEventsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetCalendarsResponse GetCalendars(
      Func<GetCalendarsDescriptor, IGetCalendarsRequest> selector = null)
    {
      return this.GetCalendars(selector.InvokeOrDefault<GetCalendarsDescriptor, IGetCalendarsRequest>(new GetCalendarsDescriptor()));
    }

    public Task<GetCalendarsResponse> GetCalendarsAsync(
      Func<GetCalendarsDescriptor, IGetCalendarsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetCalendarsAsync(selector.InvokeOrDefault<GetCalendarsDescriptor, IGetCalendarsRequest>(new GetCalendarsDescriptor()), ct);
    }

    public GetCalendarsResponse GetCalendars(IGetCalendarsRequest request) => this.DoRequest<IGetCalendarsRequest, GetCalendarsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetCalendarsResponse> GetCalendarsAsync(
      IGetCalendarsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetCalendarsRequest, GetCalendarsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetCategoriesResponse GetCategories(
      Id jobId,
      Func<GetCategoriesDescriptor, IGetCategoriesRequest> selector = null)
    {
      return this.GetCategories(selector.InvokeOrDefault<GetCategoriesDescriptor, IGetCategoriesRequest>(new GetCategoriesDescriptor(jobId)));
    }

    public Task<GetCategoriesResponse> GetCategoriesAsync(
      Id jobId,
      Func<GetCategoriesDescriptor, IGetCategoriesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetCategoriesAsync(selector.InvokeOrDefault<GetCategoriesDescriptor, IGetCategoriesRequest>(new GetCategoriesDescriptor(jobId)), ct);
    }

    public GetCategoriesResponse GetCategories(IGetCategoriesRequest request) => this.DoRequest<IGetCategoriesRequest, GetCategoriesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetCategoriesResponse> GetCategoriesAsync(
      IGetCategoriesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetCategoriesRequest, GetCategoriesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetDatafeedStatsResponse GetDatafeedStats(
      Func<GetDatafeedStatsDescriptor, IGetDatafeedStatsRequest> selector = null)
    {
      return this.GetDatafeedStats(selector.InvokeOrDefault<GetDatafeedStatsDescriptor, IGetDatafeedStatsRequest>(new GetDatafeedStatsDescriptor()));
    }

    public Task<GetDatafeedStatsResponse> GetDatafeedStatsAsync(
      Func<GetDatafeedStatsDescriptor, IGetDatafeedStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetDatafeedStatsAsync(selector.InvokeOrDefault<GetDatafeedStatsDescriptor, IGetDatafeedStatsRequest>(new GetDatafeedStatsDescriptor()), ct);
    }

    public GetDatafeedStatsResponse GetDatafeedStats(IGetDatafeedStatsRequest request) => this.DoRequest<IGetDatafeedStatsRequest, GetDatafeedStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetDatafeedStatsResponse> GetDatafeedStatsAsync(
      IGetDatafeedStatsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetDatafeedStatsRequest, GetDatafeedStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetDatafeedsResponse GetDatafeeds(
      Func<GetDatafeedsDescriptor, IGetDatafeedsRequest> selector = null)
    {
      return this.GetDatafeeds(selector.InvokeOrDefault<GetDatafeedsDescriptor, IGetDatafeedsRequest>(new GetDatafeedsDescriptor()));
    }

    public Task<GetDatafeedsResponse> GetDatafeedsAsync(
      Func<GetDatafeedsDescriptor, IGetDatafeedsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetDatafeedsAsync(selector.InvokeOrDefault<GetDatafeedsDescriptor, IGetDatafeedsRequest>(new GetDatafeedsDescriptor()), ct);
    }

    public GetDatafeedsResponse GetDatafeeds(IGetDatafeedsRequest request) => this.DoRequest<IGetDatafeedsRequest, GetDatafeedsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetDatafeedsResponse> GetDatafeedsAsync(
      IGetDatafeedsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetDatafeedsRequest, GetDatafeedsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetFiltersResponse GetFilters(
      Func<GetFiltersDescriptor, IGetFiltersRequest> selector = null)
    {
      return this.GetFilters(selector.InvokeOrDefault<GetFiltersDescriptor, IGetFiltersRequest>(new GetFiltersDescriptor()));
    }

    public Task<GetFiltersResponse> GetFiltersAsync(
      Func<GetFiltersDescriptor, IGetFiltersRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetFiltersAsync(selector.InvokeOrDefault<GetFiltersDescriptor, IGetFiltersRequest>(new GetFiltersDescriptor()), ct);
    }

    public GetFiltersResponse GetFilters(IGetFiltersRequest request) => this.DoRequest<IGetFiltersRequest, GetFiltersResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetFiltersResponse> GetFiltersAsync(
      IGetFiltersRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetFiltersRequest, GetFiltersResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetInfluencersResponse GetInfluencers(
      Id jobId,
      Func<GetInfluencersDescriptor, IGetInfluencersRequest> selector = null)
    {
      return this.GetInfluencers(selector.InvokeOrDefault<GetInfluencersDescriptor, IGetInfluencersRequest>(new GetInfluencersDescriptor(jobId)));
    }

    public Task<GetInfluencersResponse> GetInfluencersAsync(
      Id jobId,
      Func<GetInfluencersDescriptor, IGetInfluencersRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetInfluencersAsync(selector.InvokeOrDefault<GetInfluencersDescriptor, IGetInfluencersRequest>(new GetInfluencersDescriptor(jobId)), ct);
    }

    public GetInfluencersResponse GetInfluencers(IGetInfluencersRequest request) => this.DoRequest<IGetInfluencersRequest, GetInfluencersResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetInfluencersResponse> GetInfluencersAsync(
      IGetInfluencersRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetInfluencersRequest, GetInfluencersResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetJobStatsResponse GetJobStats(
      Func<GetJobStatsDescriptor, IGetJobStatsRequest> selector = null)
    {
      return this.GetJobStats(selector.InvokeOrDefault<GetJobStatsDescriptor, IGetJobStatsRequest>(new GetJobStatsDescriptor()));
    }

    public Task<GetJobStatsResponse> GetJobStatsAsync(
      Func<GetJobStatsDescriptor, IGetJobStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetJobStatsAsync(selector.InvokeOrDefault<GetJobStatsDescriptor, IGetJobStatsRequest>(new GetJobStatsDescriptor()), ct);
    }

    public GetJobStatsResponse GetJobStats(IGetJobStatsRequest request) => this.DoRequest<IGetJobStatsRequest, GetJobStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetJobStatsResponse> GetJobStatsAsync(
      IGetJobStatsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetJobStatsRequest, GetJobStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetJobsResponse GetJobs(Func<GetJobsDescriptor, IGetJobsRequest> selector = null) => this.GetJobs(selector.InvokeOrDefault<GetJobsDescriptor, IGetJobsRequest>(new GetJobsDescriptor()));

    public Task<GetJobsResponse> GetJobsAsync(
      Func<GetJobsDescriptor, IGetJobsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetJobsAsync(selector.InvokeOrDefault<GetJobsDescriptor, IGetJobsRequest>(new GetJobsDescriptor()), ct);
    }

    public GetJobsResponse GetJobs(IGetJobsRequest request) => this.DoRequest<IGetJobsRequest, GetJobsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetJobsResponse> GetJobsAsync(IGetJobsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetJobsRequest, GetJobsResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetModelSnapshotsResponse GetModelSnapshots(
      Id jobId,
      Func<GetModelSnapshotsDescriptor, IGetModelSnapshotsRequest> selector = null)
    {
      return this.GetModelSnapshots(selector.InvokeOrDefault<GetModelSnapshotsDescriptor, IGetModelSnapshotsRequest>(new GetModelSnapshotsDescriptor(jobId)));
    }

    public Task<GetModelSnapshotsResponse> GetModelSnapshotsAsync(
      Id jobId,
      Func<GetModelSnapshotsDescriptor, IGetModelSnapshotsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetModelSnapshotsAsync(selector.InvokeOrDefault<GetModelSnapshotsDescriptor, IGetModelSnapshotsRequest>(new GetModelSnapshotsDescriptor(jobId)), ct);
    }

    public GetModelSnapshotsResponse GetModelSnapshots(IGetModelSnapshotsRequest request) => this.DoRequest<IGetModelSnapshotsRequest, GetModelSnapshotsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetModelSnapshotsResponse> GetModelSnapshotsAsync(
      IGetModelSnapshotsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetModelSnapshotsRequest, GetModelSnapshotsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetOverallBucketsResponse GetOverallBuckets(
      Id jobId,
      Func<GetOverallBucketsDescriptor, IGetOverallBucketsRequest> selector = null)
    {
      return this.GetOverallBuckets(selector.InvokeOrDefault<GetOverallBucketsDescriptor, IGetOverallBucketsRequest>(new GetOverallBucketsDescriptor(jobId)));
    }

    public Task<GetOverallBucketsResponse> GetOverallBucketsAsync(
      Id jobId,
      Func<GetOverallBucketsDescriptor, IGetOverallBucketsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetOverallBucketsAsync(selector.InvokeOrDefault<GetOverallBucketsDescriptor, IGetOverallBucketsRequest>(new GetOverallBucketsDescriptor(jobId)), ct);
    }

    public GetOverallBucketsResponse GetOverallBuckets(IGetOverallBucketsRequest request) => this.DoRequest<IGetOverallBucketsRequest, GetOverallBucketsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetOverallBucketsResponse> GetOverallBucketsAsync(
      IGetOverallBucketsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetOverallBucketsRequest, GetOverallBucketsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetAnomalyRecordsResponse GetAnomalyRecords(
      Id jobId,
      Func<GetAnomalyRecordsDescriptor, IGetAnomalyRecordsRequest> selector = null)
    {
      return this.GetAnomalyRecords(selector.InvokeOrDefault<GetAnomalyRecordsDescriptor, IGetAnomalyRecordsRequest>(new GetAnomalyRecordsDescriptor(jobId)));
    }

    public Task<GetAnomalyRecordsResponse> GetAnomalyRecordsAsync(
      Id jobId,
      Func<GetAnomalyRecordsDescriptor, IGetAnomalyRecordsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetAnomalyRecordsAsync(selector.InvokeOrDefault<GetAnomalyRecordsDescriptor, IGetAnomalyRecordsRequest>(new GetAnomalyRecordsDescriptor(jobId)), ct);
    }

    public GetAnomalyRecordsResponse GetAnomalyRecords(IGetAnomalyRecordsRequest request) => this.DoRequest<IGetAnomalyRecordsRequest, GetAnomalyRecordsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetAnomalyRecordsResponse> GetAnomalyRecordsAsync(
      IGetAnomalyRecordsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetAnomalyRecordsRequest, GetAnomalyRecordsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public MachineLearningInfoResponse Info(
      Func<MachineLearningInfoDescriptor, IMachineLearningInfoRequest> selector = null)
    {
      return this.Info(selector.InvokeOrDefault<MachineLearningInfoDescriptor, IMachineLearningInfoRequest>(new MachineLearningInfoDescriptor()));
    }

    public Task<MachineLearningInfoResponse> InfoAsync(
      Func<MachineLearningInfoDescriptor, IMachineLearningInfoRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.InfoAsync(selector.InvokeOrDefault<MachineLearningInfoDescriptor, IMachineLearningInfoRequest>(new MachineLearningInfoDescriptor()), ct);
    }

    public MachineLearningInfoResponse Info(IMachineLearningInfoRequest request) => this.DoRequest<IMachineLearningInfoRequest, MachineLearningInfoResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<MachineLearningInfoResponse> InfoAsync(
      IMachineLearningInfoRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IMachineLearningInfoRequest, MachineLearningInfoResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public OpenJobResponse OpenJob(Id jobId, Func<OpenJobDescriptor, IOpenJobRequest> selector = null) => this.OpenJob(selector.InvokeOrDefault<OpenJobDescriptor, IOpenJobRequest>(new OpenJobDescriptor(jobId)));

    public Task<OpenJobResponse> OpenJobAsync(
      Id jobId,
      Func<OpenJobDescriptor, IOpenJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.OpenJobAsync(selector.InvokeOrDefault<OpenJobDescriptor, IOpenJobRequest>(new OpenJobDescriptor(jobId)), ct);
    }

    public OpenJobResponse OpenJob(IOpenJobRequest request) => this.DoRequest<IOpenJobRequest, OpenJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<OpenJobResponse> OpenJobAsync(IOpenJobRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IOpenJobRequest, OpenJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public PostCalendarEventsResponse PostCalendarEvents(
      Id calendarId,
      Func<PostCalendarEventsDescriptor, IPostCalendarEventsRequest> selector)
    {
      return this.PostCalendarEvents(selector.InvokeOrDefault<PostCalendarEventsDescriptor, IPostCalendarEventsRequest>(new PostCalendarEventsDescriptor(calendarId)));
    }

    public Task<PostCalendarEventsResponse> PostCalendarEventsAsync(
      Id calendarId,
      Func<PostCalendarEventsDescriptor, IPostCalendarEventsRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PostCalendarEventsAsync(selector.InvokeOrDefault<PostCalendarEventsDescriptor, IPostCalendarEventsRequest>(new PostCalendarEventsDescriptor(calendarId)), ct);
    }

    public PostCalendarEventsResponse PostCalendarEvents(IPostCalendarEventsRequest request) => this.DoRequest<IPostCalendarEventsRequest, PostCalendarEventsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PostCalendarEventsResponse> PostCalendarEventsAsync(
      IPostCalendarEventsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPostCalendarEventsRequest, PostCalendarEventsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PostJobDataResponse PostJobData(
      Id jobId,
      Func<PostJobDataDescriptor, IPostJobDataRequest> selector)
    {
      return this.PostJobData(selector.InvokeOrDefault<PostJobDataDescriptor, IPostJobDataRequest>(new PostJobDataDescriptor(jobId)));
    }

    public Task<PostJobDataResponse> PostJobDataAsync(
      Id jobId,
      Func<PostJobDataDescriptor, IPostJobDataRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PostJobDataAsync(selector.InvokeOrDefault<PostJobDataDescriptor, IPostJobDataRequest>(new PostJobDataDescriptor(jobId)), ct);
    }

    public PostJobDataResponse PostJobData(IPostJobDataRequest request) => this.DoRequest<IPostJobDataRequest, PostJobDataResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PostJobDataResponse> PostJobDataAsync(
      IPostJobDataRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPostJobDataRequest, PostJobDataResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PreviewDatafeedResponse<TDocument> PreviewDatafeed<TDocument>(
      Func<PreviewDatafeedDescriptor, IPreviewDatafeedRequest> selector = null)
      where TDocument : class
    {
      return this.PreviewDatafeed<TDocument>(selector.InvokeOrDefault<PreviewDatafeedDescriptor, IPreviewDatafeedRequest>(new PreviewDatafeedDescriptor()));
    }

    public Task<PreviewDatafeedResponse<TDocument>> PreviewDatafeedAsync<TDocument>(
      Func<PreviewDatafeedDescriptor, IPreviewDatafeedRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.PreviewDatafeedAsync<TDocument>(selector.InvokeOrDefault<PreviewDatafeedDescriptor, IPreviewDatafeedRequest>(new PreviewDatafeedDescriptor()), ct);
    }

    public PreviewDatafeedResponse<TDocument> PreviewDatafeed<TDocument>(
      IPreviewDatafeedRequest request)
      where TDocument : class
    {
      return this.DoRequest<IPreviewDatafeedRequest, PreviewDatafeedResponse<TDocument>>(request, this.ResponseBuilder(request.RequestParameters, (CustomResponseBuilderBase) PreviewDatafeedResponseBuilder<TDocument>.Instance));
    }

    public Task<PreviewDatafeedResponse<TDocument>> PreviewDatafeedAsync<TDocument>(
      IPreviewDatafeedRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<IPreviewDatafeedRequest, PreviewDatafeedResponse<TDocument>>(request, this.ResponseBuilder(request.RequestParameters, (CustomResponseBuilderBase) PreviewDatafeedResponseBuilder<TDocument>.Instance), ct);
    }

    public PutCalendarResponse PutCalendar(
      Id calendarId,
      Func<PutCalendarDescriptor, IPutCalendarRequest> selector = null)
    {
      return this.PutCalendar(selector.InvokeOrDefault<PutCalendarDescriptor, IPutCalendarRequest>(new PutCalendarDescriptor(calendarId)));
    }

    public Task<PutCalendarResponse> PutCalendarAsync(
      Id calendarId,
      Func<PutCalendarDescriptor, IPutCalendarRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutCalendarAsync(selector.InvokeOrDefault<PutCalendarDescriptor, IPutCalendarRequest>(new PutCalendarDescriptor(calendarId)), ct);
    }

    public PutCalendarResponse PutCalendar(IPutCalendarRequest request) => this.DoRequest<IPutCalendarRequest, PutCalendarResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutCalendarResponse> PutCalendarAsync(
      IPutCalendarRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutCalendarRequest, PutCalendarResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutCalendarJobResponse PutCalendarJob(
      Id calendarId,
      Id jobId,
      Func<PutCalendarJobDescriptor, IPutCalendarJobRequest> selector = null)
    {
      return this.PutCalendarJob(selector.InvokeOrDefault<PutCalendarJobDescriptor, IPutCalendarJobRequest>(new PutCalendarJobDescriptor(calendarId, jobId)));
    }

    public Task<PutCalendarJobResponse> PutCalendarJobAsync(
      Id calendarId,
      Id jobId,
      Func<PutCalendarJobDescriptor, IPutCalendarJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutCalendarJobAsync(selector.InvokeOrDefault<PutCalendarJobDescriptor, IPutCalendarJobRequest>(new PutCalendarJobDescriptor(calendarId, jobId)), ct);
    }

    public PutCalendarJobResponse PutCalendarJob(IPutCalendarJobRequest request) => this.DoRequest<IPutCalendarJobRequest, PutCalendarJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutCalendarJobResponse> PutCalendarJobAsync(
      IPutCalendarJobRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutCalendarJobRequest, PutCalendarJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutDatafeedResponse PutDatafeed<TDocument>(
      Id datafeedId,
      Func<PutDatafeedDescriptor<TDocument>, IPutDatafeedRequest> selector)
      where TDocument : class
    {
      return this.PutDatafeed(selector.InvokeOrDefault<PutDatafeedDescriptor<TDocument>, IPutDatafeedRequest>(new PutDatafeedDescriptor<TDocument>(datafeedId)));
    }

    public Task<PutDatafeedResponse> PutDatafeedAsync<TDocument>(
      Id datafeedId,
      Func<PutDatafeedDescriptor<TDocument>, IPutDatafeedRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.PutDatafeedAsync(selector.InvokeOrDefault<PutDatafeedDescriptor<TDocument>, IPutDatafeedRequest>(new PutDatafeedDescriptor<TDocument>(datafeedId)), ct);
    }

    public PutDatafeedResponse PutDatafeed(IPutDatafeedRequest request) => this.DoRequest<IPutDatafeedRequest, PutDatafeedResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutDatafeedResponse> PutDatafeedAsync(
      IPutDatafeedRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutDatafeedRequest, PutDatafeedResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutFilterResponse PutFilter(
      Id filterId,
      Func<PutFilterDescriptor, IPutFilterRequest> selector)
    {
      return this.PutFilter(selector.InvokeOrDefault<PutFilterDescriptor, IPutFilterRequest>(new PutFilterDescriptor(filterId)));
    }

    public Task<PutFilterResponse> PutFilterAsync(
      Id filterId,
      Func<PutFilterDescriptor, IPutFilterRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutFilterAsync(selector.InvokeOrDefault<PutFilterDescriptor, IPutFilterRequest>(new PutFilterDescriptor(filterId)), ct);
    }

    public PutFilterResponse PutFilter(IPutFilterRequest request) => this.DoRequest<IPutFilterRequest, PutFilterResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutFilterResponse> PutFilterAsync(IPutFilterRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPutFilterRequest, PutFilterResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public PutJobResponse PutJob<TDocument>(
      Id jobId,
      Func<PutJobDescriptor<TDocument>, IPutJobRequest> selector)
      where TDocument : class
    {
      return this.PutJob(selector.InvokeOrDefault<PutJobDescriptor<TDocument>, IPutJobRequest>(new PutJobDescriptor<TDocument>(jobId)));
    }

    public Task<PutJobResponse> PutJobAsync<TDocument>(
      Id jobId,
      Func<PutJobDescriptor<TDocument>, IPutJobRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.PutJobAsync(selector.InvokeOrDefault<PutJobDescriptor<TDocument>, IPutJobRequest>(new PutJobDescriptor<TDocument>(jobId)), ct);
    }

    public PutJobResponse PutJob(IPutJobRequest request) => this.DoRequest<IPutJobRequest, PutJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutJobResponse> PutJobAsync(IPutJobRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPutJobRequest, PutJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public RevertModelSnapshotResponse RevertModelSnapshot(
      Id jobId,
      Id snapshotId,
      Func<RevertModelSnapshotDescriptor, IRevertModelSnapshotRequest> selector = null)
    {
      return this.RevertModelSnapshot(selector.InvokeOrDefault<RevertModelSnapshotDescriptor, IRevertModelSnapshotRequest>(new RevertModelSnapshotDescriptor(jobId, snapshotId)));
    }

    public Task<RevertModelSnapshotResponse> RevertModelSnapshotAsync(
      Id jobId,
      Id snapshotId,
      Func<RevertModelSnapshotDescriptor, IRevertModelSnapshotRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RevertModelSnapshotAsync(selector.InvokeOrDefault<RevertModelSnapshotDescriptor, IRevertModelSnapshotRequest>(new RevertModelSnapshotDescriptor(jobId, snapshotId)), ct);
    }

    public RevertModelSnapshotResponse RevertModelSnapshot(IRevertModelSnapshotRequest request) => this.DoRequest<IRevertModelSnapshotRequest, RevertModelSnapshotResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RevertModelSnapshotResponse> RevertModelSnapshotAsync(
      IRevertModelSnapshotRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IRevertModelSnapshotRequest, RevertModelSnapshotResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public SetUpgradeModeResponse SetUpgradeMode(
      Func<SetUpgradeModeDescriptor, ISetUpgradeModeRequest> selector = null)
    {
      return this.SetUpgradeMode(selector.InvokeOrDefault<SetUpgradeModeDescriptor, ISetUpgradeModeRequest>(new SetUpgradeModeDescriptor()));
    }

    public Task<SetUpgradeModeResponse> SetUpgradeModeAsync(
      Func<SetUpgradeModeDescriptor, ISetUpgradeModeRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SetUpgradeModeAsync(selector.InvokeOrDefault<SetUpgradeModeDescriptor, ISetUpgradeModeRequest>(new SetUpgradeModeDescriptor()), ct);
    }

    public SetUpgradeModeResponse SetUpgradeMode(ISetUpgradeModeRequest request) => this.DoRequest<ISetUpgradeModeRequest, SetUpgradeModeResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SetUpgradeModeResponse> SetUpgradeModeAsync(
      ISetUpgradeModeRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ISetUpgradeModeRequest, SetUpgradeModeResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public StartDatafeedResponse StartDatafeed(
      Id datafeedId,
      Func<StartDatafeedDescriptor, IStartDatafeedRequest> selector = null)
    {
      return this.StartDatafeed(selector.InvokeOrDefault<StartDatafeedDescriptor, IStartDatafeedRequest>(new StartDatafeedDescriptor(datafeedId)));
    }

    public Task<StartDatafeedResponse> StartDatafeedAsync(
      Id datafeedId,
      Func<StartDatafeedDescriptor, IStartDatafeedRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StartDatafeedAsync(selector.InvokeOrDefault<StartDatafeedDescriptor, IStartDatafeedRequest>(new StartDatafeedDescriptor(datafeedId)), ct);
    }

    public StartDatafeedResponse StartDatafeed(IStartDatafeedRequest request) => this.DoRequest<IStartDatafeedRequest, StartDatafeedResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StartDatafeedResponse> StartDatafeedAsync(
      IStartDatafeedRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStartDatafeedRequest, StartDatafeedResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public StopDatafeedResponse StopDatafeed(
      Id datafeedId,
      Func<StopDatafeedDescriptor, IStopDatafeedRequest> selector = null)
    {
      return this.StopDatafeed(selector.InvokeOrDefault<StopDatafeedDescriptor, IStopDatafeedRequest>(new StopDatafeedDescriptor(datafeedId)));
    }

    public Task<StopDatafeedResponse> StopDatafeedAsync(
      Id datafeedId,
      Func<StopDatafeedDescriptor, IStopDatafeedRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StopDatafeedAsync(selector.InvokeOrDefault<StopDatafeedDescriptor, IStopDatafeedRequest>(new StopDatafeedDescriptor(datafeedId)), ct);
    }

    public StopDatafeedResponse StopDatafeed(IStopDatafeedRequest request) => this.DoRequest<IStopDatafeedRequest, StopDatafeedResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StopDatafeedResponse> StopDatafeedAsync(
      IStopDatafeedRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStopDatafeedRequest, StopDatafeedResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public UpdateDatafeedResponse UpdateDatafeed<TDocument>(
      Id datafeedId,
      Func<UpdateDatafeedDescriptor<TDocument>, IUpdateDatafeedRequest> selector)
      where TDocument : class
    {
      return this.UpdateDatafeed(selector.InvokeOrDefault<UpdateDatafeedDescriptor<TDocument>, IUpdateDatafeedRequest>(new UpdateDatafeedDescriptor<TDocument>(datafeedId)));
    }

    public Task<UpdateDatafeedResponse> UpdateDatafeedAsync<TDocument>(
      Id datafeedId,
      Func<UpdateDatafeedDescriptor<TDocument>, IUpdateDatafeedRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.UpdateDatafeedAsync(selector.InvokeOrDefault<UpdateDatafeedDescriptor<TDocument>, IUpdateDatafeedRequest>(new UpdateDatafeedDescriptor<TDocument>(datafeedId)), ct);
    }

    public UpdateDatafeedResponse UpdateDatafeed(IUpdateDatafeedRequest request) => this.DoRequest<IUpdateDatafeedRequest, UpdateDatafeedResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<UpdateDatafeedResponse> UpdateDatafeedAsync(
      IUpdateDatafeedRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IUpdateDatafeedRequest, UpdateDatafeedResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public UpdateFilterResponse UpdateFilter(
      Id filterId,
      Func<UpdateFilterDescriptor, IUpdateFilterRequest> selector)
    {
      return this.UpdateFilter(selector.InvokeOrDefault<UpdateFilterDescriptor, IUpdateFilterRequest>(new UpdateFilterDescriptor(filterId)));
    }

    public Task<UpdateFilterResponse> UpdateFilterAsync(
      Id filterId,
      Func<UpdateFilterDescriptor, IUpdateFilterRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.UpdateFilterAsync(selector.InvokeOrDefault<UpdateFilterDescriptor, IUpdateFilterRequest>(new UpdateFilterDescriptor(filterId)), ct);
    }

    public UpdateFilterResponse UpdateFilter(IUpdateFilterRequest request) => this.DoRequest<IUpdateFilterRequest, UpdateFilterResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<UpdateFilterResponse> UpdateFilterAsync(
      IUpdateFilterRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IUpdateFilterRequest, UpdateFilterResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public UpdateJobResponse UpdateJob<TDocument>(
      Id jobId,
      Func<UpdateJobDescriptor<TDocument>, IUpdateJobRequest> selector)
      where TDocument : class
    {
      return this.UpdateJob(selector.InvokeOrDefault<UpdateJobDescriptor<TDocument>, IUpdateJobRequest>(new UpdateJobDescriptor<TDocument>(jobId)));
    }

    public Task<UpdateJobResponse> UpdateJobAsync<TDocument>(
      Id jobId,
      Func<UpdateJobDescriptor<TDocument>, IUpdateJobRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.UpdateJobAsync(selector.InvokeOrDefault<UpdateJobDescriptor<TDocument>, IUpdateJobRequest>(new UpdateJobDescriptor<TDocument>(jobId)), ct);
    }

    public UpdateJobResponse UpdateJob(IUpdateJobRequest request) => this.DoRequest<IUpdateJobRequest, UpdateJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<UpdateJobResponse> UpdateJobAsync(IUpdateJobRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IUpdateJobRequest, UpdateJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public UpdateModelSnapshotResponse UpdateModelSnapshot(
      Id jobId,
      Id snapshotId,
      Func<UpdateModelSnapshotDescriptor, IUpdateModelSnapshotRequest> selector)
    {
      return this.UpdateModelSnapshot(selector.InvokeOrDefault<UpdateModelSnapshotDescriptor, IUpdateModelSnapshotRequest>(new UpdateModelSnapshotDescriptor(jobId, snapshotId)));
    }

    public Task<UpdateModelSnapshotResponse> UpdateModelSnapshotAsync(
      Id jobId,
      Id snapshotId,
      Func<UpdateModelSnapshotDescriptor, IUpdateModelSnapshotRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.UpdateModelSnapshotAsync(selector.InvokeOrDefault<UpdateModelSnapshotDescriptor, IUpdateModelSnapshotRequest>(new UpdateModelSnapshotDescriptor(jobId, snapshotId)), ct);
    }

    public UpdateModelSnapshotResponse UpdateModelSnapshot(IUpdateModelSnapshotRequest request) => this.DoRequest<IUpdateModelSnapshotRequest, UpdateModelSnapshotResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<UpdateModelSnapshotResponse> UpdateModelSnapshotAsync(
      IUpdateModelSnapshotRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IUpdateModelSnapshotRequest, UpdateModelSnapshotResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ValidateJobResponse ValidateJob<TDocument>(
      Func<ValidateJobDescriptor<TDocument>, IValidateJobRequest> selector)
      where TDocument : class
    {
      return this.ValidateJob(selector.InvokeOrDefault<ValidateJobDescriptor<TDocument>, IValidateJobRequest>(new ValidateJobDescriptor<TDocument>()));
    }

    public Task<ValidateJobResponse> ValidateJobAsync<TDocument>(
      Func<ValidateJobDescriptor<TDocument>, IValidateJobRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.ValidateJobAsync(selector.InvokeOrDefault<ValidateJobDescriptor<TDocument>, IValidateJobRequest>(new ValidateJobDescriptor<TDocument>()), ct);
    }

    public ValidateJobResponse ValidateJob(IValidateJobRequest request) => this.DoRequest<IValidateJobRequest, ValidateJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ValidateJobResponse> ValidateJobAsync(
      IValidateJobRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IValidateJobRequest, ValidateJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ValidateDetectorResponse ValidateDetector<TDocument>(
      Func<ValidateDetectorDescriptor<TDocument>, IValidateDetectorRequest> selector)
      where TDocument : class
    {
      return this.ValidateDetector(selector.InvokeOrDefault<ValidateDetectorDescriptor<TDocument>, IValidateDetectorRequest>(new ValidateDetectorDescriptor<TDocument>()));
    }

    public Task<ValidateDetectorResponse> ValidateDetectorAsync<TDocument>(
      Func<ValidateDetectorDescriptor<TDocument>, IValidateDetectorRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.ValidateDetectorAsync(selector.InvokeOrDefault<ValidateDetectorDescriptor<TDocument>, IValidateDetectorRequest>(new ValidateDetectorDescriptor<TDocument>()), ct);
    }

    public ValidateDetectorResponse ValidateDetector(IValidateDetectorRequest request) => this.DoRequest<IValidateDetectorRequest, ValidateDetectorResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ValidateDetectorResponse> ValidateDetectorAsync(
      IValidateDetectorRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IValidateDetectorRequest, ValidateDetectorResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
