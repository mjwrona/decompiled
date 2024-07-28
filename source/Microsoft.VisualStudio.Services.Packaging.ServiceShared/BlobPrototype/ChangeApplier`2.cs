// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ChangeApplier`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ChangeApplier<TReq, TOp> : 
    IAsyncHandler<
    #nullable disable
    TReq, ICommitLogEntry>,
    IHaveInputType<TReq>,
    IHaveOutputType<ICommitLogEntry>
    where TReq : IFeedRequest
    where TOp : ICommitOperationData
  {
    private readonly ITracerService tracerService;
    private readonly bool applyToAggregations;
    private readonly ICommitLogWriter<ICommitLogEntry> commitLogService;
    private readonly IAggregationAccessorFactory readAggregationAccessorFactory;
    private readonly IAggregationAccessorFactory writeAggregationAccessorFactory;
    private readonly IFeedJobQueuer feedChangeProcessingJobQueuer;
    private readonly IRequireAggHandlerBootstrapper<TReq, TOp> requestToOperationHandlerBootstrapper;
    private readonly IFeedPerms permsFacade;
    private readonly IAggregationCommitApplier aggregationCommitApplier;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IAsyncHandler<TReq, NullResult> finallyHandler;
    private readonly ICache<string, object> telemetryCache;

    public ChangeApplier(
      ICommitLogWriter<ICommitLogEntry> commitLogService,
      IAggregationAccessorFactory readAggregationAccessorFactory,
      IAggregationAccessorFactory writeAggregationAccessorFactory,
      IFeedJobQueuer feedChangeProcessingJobQueuer,
      IRequireAggHandlerBootstrapper<TReq, TOp> requestToOperationHandlerBootstrapper,
      IFeedPerms permsFacade,
      bool applyToAggregations,
      ITracerService tracerService,
      IAggregationCommitApplier aggregationCommitApplier,
      IFeatureFlagService featureFlagService,
      IAsyncHandler<TReq, NullResult> finallyHandler,
      ICache<string, object> telemetryCache)
    {
      ArgumentUtility.CheckForNull<IFeedPerms>(permsFacade, nameof (permsFacade));
      this.commitLogService = commitLogService;
      this.readAggregationAccessorFactory = readAggregationAccessorFactory;
      this.writeAggregationAccessorFactory = writeAggregationAccessorFactory;
      this.feedChangeProcessingJobQueuer = feedChangeProcessingJobQueuer;
      this.requestToOperationHandlerBootstrapper = requestToOperationHandlerBootstrapper;
      this.permsFacade = permsFacade;
      this.applyToAggregations = applyToAggregations;
      this.tracerService = tracerService;
      this.aggregationCommitApplier = aggregationCommitApplier;
      this.featureFlagService = featureFlagService;
      this.finallyHandler = finallyHandler;
      this.telemetryCache = telemetryCache;
    }

    public async Task<ICommitLogEntry> Handle(TReq request)
    {
      ChangeApplier<TReq, TOp> sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        object obj = (object) null;
        int num = 0;
        ICommitLogEntry commitLogEntry1;
        try
        {
          if (sendInTheThisObject.featureFlagService.IsEnabled(request.Protocol.ReadOnlyFeatureFlagName))
            throw new FeatureReadOnlyException(Resources.Error_ServiceReadOnly());
          if (request.Feed.IsReadOnly)
            throw new ReadOnlyFeedOperationException(Resources.Error_FeedIsReadOnly());
          IReadOnlyList<IAggregationAccessor> accessorsFor1 = await sendInTheThisObject.readAggregationAccessorFactory.GetAccessorsFor((IFeedRequest) request);
          IAsyncHandler<TReq, TOp> requestToOperationHandler = sendInTheThisObject.requestToOperationHandlerBootstrapper.Bootstrap((IReadOnlyCollection<IAggregationAccessor>) accessorsFor1);
          TOp commitOperationData = await requestToOperationHandler.Handle(request);
          try
          {
            if ((object) commitOperationData == null)
              throw new CommitLogProcessingException(Resources.Error_NoDataGeneratedFromRequest((object) "commitOperationData", (object) request.GetType().FullName, (object) requestToOperationHandler.GetType().FullName));
            sendInTheThisObject.permsFacade.Validate(request.Feed, commitOperationData.PermissionDemand);
          }
          catch (Exception ex)
          {
            tracer.TraceException(ex);
            throw;
          }
          ICommitLogEntry commitLogEntry2 = await sendInTheThisObject.commitLogService.AppendEntryAsync(request.Feed, (ICommitOperationData) commitOperationData);
          tracer.TraceInfo(string.Format("commited entry: {0} for operation: {1} for feed: {2}", (object) commitLogEntry2.CommitId, (object) commitOperationData, (object) request.Feed.Id));
          try
          {
            int num1;
            int num2 = num1 - 3;
            try
            {
              if (sendInTheThisObject.applyToAggregations)
              {
                IReadOnlyList<IAggregationAccessor> accessorsFor2 = await sendInTheThisObject.writeAggregationAccessorFactory.GetAccessorsFor((IFeedRequest) request);
                AggregationApplyTimings aggregationApplyTimings = await sendInTheThisObject.aggregationCommitApplier.ApplyCommitAsync(accessorsFor2, (IFeedRequest) request, (IReadOnlyList<ICommitLogEntry>) new ICommitLogEntry[1]
                {
                  commitLogEntry2
                });
              }
            }
            catch (Exception ex)
            {
              tracer.TraceException(ex);
              sendInTheThisObject.telemetryCache.Set("Packaging.Properties.CommitDidNotApply", (object) 1);
            }
          }
          finally
          {
            Guid guid = await sendInTheThisObject.feedChangeProcessingJobQueuer.QueueJob(request.Feed, request.Protocol, JobPriorityLevel.Highest);
          }
          commitLogEntry1 = commitLogEntry2;
          num = 1;
        }
        catch (object ex)
        {
          obj = ex;
        }
        NullResult nullResult = await sendInTheThisObject.finallyHandler.Handle(request);
        object obj1 = obj;
        if (obj1 != null)
        {
          if (!(obj1 is Exception source))
            throw obj1;
          ExceptionDispatchInfo.Capture(source).Throw();
        }
        if (num == 1)
          return commitLogEntry1;
        obj = (object) null;
        commitLogEntry1 = (ICommitLogEntry) null;
      }
      ICommitLogEntry commitLogEntry;
      return commitLogEntry;
    }
  }
}
