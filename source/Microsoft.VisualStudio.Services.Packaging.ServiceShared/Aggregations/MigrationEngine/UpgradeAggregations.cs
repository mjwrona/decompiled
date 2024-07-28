// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine.UpgradeAggregations
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine
{
  public class UpgradeAggregations : 
    IAsyncHandler,
    IAsyncHandler<NullRequest, NullResult>,
    IHaveInputType<NullRequest>,
    IHaveOutputType<NullResult>
  {
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IMigrationTransitionerInternal migrationTransitioner;
    private readonly IAsyncHandler<MigrationKickerRequest, JobResult> kickerJobHandler;
    private readonly IAsyncHandler<NullRequest, IEnumerable<FeedCore>> feedsFactory;
    private readonly IFactory<IProtocol, string> destinationMigrationFactory;
    private readonly IServicingContextLogger servicingLogger;
    private readonly IProtocol protocol;

    public UpgradeAggregations(
      IExecutionEnvironment executionEnvironment,
      IMigrationTransitionerInternal migrationTransitioner,
      IAsyncHandler<MigrationKickerRequest, JobResult> kickerJobHandler,
      IAsyncHandler<NullRequest, IEnumerable<FeedCore>> feedsFactory,
      IFactory<IProtocol, string> destinationMigrationFactory,
      IServicingContextLogger servicingLogger,
      IProtocol protocol)
    {
      this.executionEnvironment = executionEnvironment;
      this.migrationTransitioner = migrationTransitioner;
      this.kickerJobHandler = kickerJobHandler;
      this.feedsFactory = feedsFactory;
      this.destinationMigrationFactory = destinationMigrationFactory;
      this.servicingLogger = servicingLogger;
      this.protocol = protocol;
    }

    public async Task<NullResult> Handle(NullRequest r)
    {
      CollectionId collectionId = new CollectionId(this.executionEnvironment.HostId);
      string destinationMigration = this.destinationMigrationFactory.Get(this.protocol);
      this.LogInfo(string.Format("Collection Id: {0}; destinationMigration: {1}", (object) collectionId, (object) destinationMigration));
      IList<FeedCore> list = (IList<FeedCore>) (await this.feedsFactory.Handle((NullRequest) null)).ToList<FeedCore>();
      this.LogInfo(string.Format("Found {0} feeds in the account.", (object) list.Count));
      List<FeedCore> feedsToBeProcessed = new List<FeedCore>();
      foreach (FeedCore feedCore in (IEnumerable<FeedCore>) list)
      {
        FeedCore feed = feedCore;
        if (!((await this.migrationTransitioner.GetOrCreateState(collectionId, feed.Id, this.protocol)).CurrentMigration == destinationMigration))
        {
          feedsToBeProcessed.Add(feed);
          feed = (FeedCore) null;
        }
      }
      if (!feedsToBeProcessed.Any<FeedCore>())
      {
        this.LogInfo("No feeds to process. All the feeds are already in destination migration: " + destinationMigration);
        return (NullResult) null;
      }
      this.LogInfo(string.Format("Initiating migrations for {0} feeds: {1}", (object) feedsToBeProcessed.Count, (object) string.Join<Guid>(",", feedsToBeProcessed.Select<FeedCore, Guid>((Func<FeedCore, Guid>) (f => f.Id)))));
      await this.UpgradeFeedsTillJobLockStep((IList<FeedCore>) feedsToBeProcessed, destinationMigration);
      List<MarkAsVNextReadTransition> transitions = new List<MarkAsVNextReadTransition>();
      List<CompleteMigrationTransition> transtionsToComplete = new List<CompleteMigrationTransition>();
      foreach (FeedCore feedCore in feedsToBeProcessed)
      {
        transitions.Add(new MarkAsVNextReadTransition(destinationMigration, collectionId, feedCore.Id, this.protocol));
        transtionsToComplete.Add(new CompleteMigrationTransition(destinationMigration, collectionId, feedCore.Id, this.protocol));
      }
      this.LogInfo("Migration State Transition: LockStep -> ReadVNext");
      await this.migrationTransitioner.Apply((IReadOnlyCollection<IMigrationTransition>) transitions);
      this.LogInfo("Migration State Transition: ReadVNext -> Complete");
      await this.migrationTransitioner.Apply((IReadOnlyCollection<IMigrationTransition>) transtionsToComplete);
      this.LogInfo("Migration successful.");
      return (NullResult) null;
    }

    private void LogInfo(string message) => this.servicingLogger.Log(ServicingStepLogEntryKind.Informational, message);

    private async Task UpgradeFeedsTillJobLockStep(
      IList<FeedCore> feedsToBeProcessed,
      string destinationMigration)
    {
      JobResult jobResult = await this.kickerJobHandler.Handle(new MigrationKickerRequest()
      {
        CollectionIds = (List<Guid>) null,
        DestinationMigration = destinationMigration,
        Instruction = MigrationInstructionEnum.LockStep,
        Feeds = feedsToBeProcessed.Select<FeedCore, Guid>((Func<FeedCore, Guid>) (f => f.Id)).ToList<Guid>(),
        SourceMigration = (string) null
      });
      if (jobResult.Result != TeamFoundationJobExecutionResult.Succeeded)
        throw new Exception(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UnexpectedMigrationFailure((object) JsonConvert.SerializeObject((object) jobResult.Telemetry)));
      IDictionary<Guid, MigrationEntry> allStateDictionary = (IDictionary<Guid, MigrationEntry>) (await this.migrationTransitioner.GetStateEntries(new MigrationStateFilter(this.protocol))).ToDictionary<MigrationEntry, Guid>((Func<MigrationEntry, Guid>) (x => x.FeedId));
      List<FeedCore> list = feedsToBeProcessed.Where<FeedCore>((Func<FeedCore, bool>) (feed => allStateDictionary[feed.Id].VNextState != MigrationStateEnum.JobLockStep)).ToList<FeedCore>();
      if (list.Any<FeedCore>())
      {
        this.LogInfo("Following feeds failed to migrate: " + string.Join(",", list.Select<FeedCore, string>((Func<FeedCore, string>) (x => x.Id.ToString()))));
        throw new Exception(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UnexpectedMigrationState((object) MigrationStateEnum.JobLockStep));
      }
      this.LogInfo(string.Format("Migrated {0} feeds to JobLockStep.", (object) feedsToBeProcessed.Count<FeedCore>()));
    }
  }
}
