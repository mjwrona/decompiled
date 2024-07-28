// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationCatchupHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MigrationCatchupHandler : 
    IAsyncHandler<MigrationCatchupRequest, int>,
    IHaveInputType<MigrationCatchupRequest>,
    IHaveOutputType<int>
  {
    private readonly IFactory<IMigrationTransitionerInternal> transitionerFactory;
    private readonly IFactory<ICommitLogReader<CommitLogEntry>> commitLogService;
    private readonly IAggregationAccessorFactory aggregationAccessorFactory;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IAggregationCommitApplier aggregationCommitApplier;

    public MigrationCatchupHandler(
      IFactory<IMigrationTransitionerInternal> transitionerFactory,
      IFactory<ICommitLogReader<CommitLogEntry>> commitLogService,
      IAggregationAccessorFactory aggregationAccessorFactory,
      IExecutionEnvironment executionEnvironment,
      IAggregationCommitApplier aggregationCommitApplier)
    {
      this.transitionerFactory = transitionerFactory;
      this.commitLogService = commitLogService;
      this.aggregationAccessorFactory = aggregationAccessorFactory;
      this.executionEnvironment = executionEnvironment;
      this.aggregationCommitApplier = aggregationCommitApplier;
    }

    public async Task<int> Handle(MigrationCatchupRequest request)
    {
      CollectionId collectionId = (CollectionId) this.executionEnvironment.HostId;
      int processedCommits = 0;
      MigrationState state = (MigrationState) await this.transitionerFactory.Get().GetOrCreateState(collectionId, request.Feed.Id, request.Protocol);
      if (state.VNextState != MigrationStateEnum.JobCatchup)
        throw new InvalidDataException(Resources.Error_UnexpectedJobState((object) MigrationStateEnum.JobCatchup, (object) state.VNextState));
      if (PackagingCommitId.Empty == request.CatchupUntilCommitId)
      {
        await this.transitionerFactory.Get().Apply((IReadOnlyCollection<IMigrationTransition>) new MarkAsJobLockStepTransition[1]
        {
          new MarkAsJobLockStepTransition(state.VNextMigration, collectionId, request.Feed.Id, request.Protocol)
        });
        return processedCommits;
      }
      PackagingCommitId previousCommitId;
      PackagingCommitId currentMigrationCommitId;
      if (state.MigrationProgress?.CommitToken != null && !PackagingCommitId.Empty.ToString().Equals(state.MigrationProgress.CommitToken))
      {
        previousCommitId = PackagingCommitId.Parse(state.MigrationProgress.CommitToken);
        currentMigrationCommitId = (await this.commitLogService.Get().GetEntryAsync(request.Feed, previousCommitId)).NextCommitId;
      }
      else
      {
        previousCommitId = PackagingCommitId.Empty;
        currentMigrationCommitId = (await this.commitLogService.Get().GetOldestCommitBookmarkAsync(request.Feed)).CommitId;
      }
      while (previousCommitId != request.CatchupUntilCommitId)
      {
        CommitLogEntry commitEntry = await this.commitLogService.Get().GetEntryAsync(request.Feed, currentMigrationCommitId);
        AggregationApplyTimings aggregationApplyTimings = await this.aggregationCommitApplier.ApplyCommitAsync(await this.aggregationAccessorFactory.GetAccessorsFor((IFeedRequest) request), (IFeedRequest) request, (IReadOnlyList<ICommitLogEntry>) new CommitLogEntry[1]
        {
          commitEntry
        });
        ++processedCommits;
        previousCommitId = currentMigrationCommitId;
        currentMigrationCommitId = commitEntry.NextCommitId;
        commitEntry = (CommitLogEntry) null;
      }
      await this.transitionerFactory.Get().Apply((IReadOnlyCollection<IMigrationTransition>) new MarkAsJobLockStepTransition[1]
      {
        new MarkAsJobLockStepTransition(state.VNextMigration, collectionId, request.Feed.Id, request.Protocol)
      });
      return processedCommits;
    }
  }
}
