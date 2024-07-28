// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification.GdprDataWriter`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification
{
  public class GdprDataWriter<TReq> : 
    IAsyncHandler<(TReq, ICommitLogEntry), bool>,
    IHaveInputType<(TReq, ICommitLogEntry)>,
    IHaveOutputType<bool>
    where TReq : IFeedRequest
  {
    private IPackageGdprDataStore gdprDataStore;
    private CollectionId collectionId;
    private PackageGdprData gdprData;
    private ITracerService tracerService;
    private IFeatureFlagService featureFlagService;
    private IExecutionEnvironment executionEnvironment;

    public GdprDataWriter(
      CollectionId collectionId,
      IPackageGdprDataStore gdprDataStore,
      PackageGdprData packageGdprData,
      IFeatureFlagService featureFlagService,
      IExecutionEnvironment executionEnvironment,
      ITracerService tracerService)
    {
      this.collectionId = collectionId;
      this.gdprDataStore = gdprDataStore;
      this.gdprData = packageGdprData;
      this.tracerService = tracerService;
      this.featureFlagService = featureFlagService;
      this.executionEnvironment = executionEnvironment;
    }

    public Task<bool> Handle((TReq, ICommitLogEntry) request)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Handle)))
      {
        TReq req = request.Item1;
        ICommitLogEntry commitLogEntry = request.Item2;
        if (this.executionEnvironment.IsHosted() && this.featureFlagService.IsEnabled("Packaging.ContentVerification"))
        {
          if (commitLogEntry.CommitOperationData is IAddOperationData)
          {
            try
            {
              this.gdprDataStore.StoreUploaderData(this.collectionId, (FeedId) req.Feed.Id, req.Protocol, new CommitLogBookmark(commitLogEntry.CommitId, new long?(commitLogEntry.SequenceNumber)), commitLogEntry.CreatedDate, this.gdprData);
              goto label_9;
            }
            catch (Exception ex)
            {
              tracerBlock.TraceException(ex);
              goto label_9;
            }
          }
        }
        return Task.FromResult<bool>(true);
      }
label_9:
      return Task.FromResult<bool>(true);
    }
  }
}
