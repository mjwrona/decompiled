// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationKickerJobHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MigrationKickerJobHandler : 
    IAsyncHandler<MigrationKickerRequest, JobResult>,
    IHaveInputType<MigrationKickerRequest>,
    IHaveOutputType<JobResult>
  {
    private readonly IFactory<IMigrationTransitionerInternal> transitionerFactory;
    private readonly IFactory<CollectionId, IDisposingFeedJobQueuer> jobQueuerFactory;
    private readonly IProtocol protocol;
    private readonly ITracerService tracerService;

    public MigrationKickerJobHandler(
      IFactory<IMigrationTransitionerInternal> transitionerFactory,
      IFactory<CollectionId, IDisposingFeedJobQueuer> jobQueuerFactory,
      IProtocol protocol,
      ITracerService tracerService)
    {
      this.transitionerFactory = transitionerFactory;
      this.jobQueuerFactory = jobQueuerFactory;
      this.protocol = protocol;
      this.tracerService = tracerService;
    }

    public async Task<JobResult> Handle(MigrationKickerRequest request)
    {
      MigrationKickerJobHandler sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        JobTelemetry telemetry = new JobTelemetry();
        try
        {
          int migrationJobsScheduled = 0;
          int migrationJobsNotScheduled = 0;
          int totalFeedsChanged = 0;
          IMigrationTransitionerInternal transitionerInternal1 = sendInTheThisObject.transitionerFactory.Get();
          try
          {
            IMigrationTransitionerInternal transitionerInternal2 = transitionerInternal1;
            MigrationStateFilter filter = new MigrationStateFilter(sendInTheThisObject.protocol);
            List<Guid> collectionIds = request.CollectionIds;
            filter.CollectionIds = collectionIds != null ? (IEnumerable<CollectionId>) collectionIds.Select<Guid, CollectionId>((Func<Guid, CollectionId>) (c => new CollectionId(c))).ToList<CollectionId>() : (IEnumerable<CollectionId>) null;
            (int num1, int num2, int num3) = await ApplyChangesTo((IReadOnlyCollection<MigrationEntry>) (await transitionerInternal2.GetStateEntries(filter)).Where<MigrationEntry>((Func<MigrationEntry, bool>) (e => FeedIsSelected(e) && FeedShouldBeUpdated(e))).ToList<MigrationEntry>());
            migrationJobsScheduled += num1;
            migrationJobsNotScheduled += num2;
            totalFeedsChanged += num3;
          }
          catch (Exception ex)
          {
            telemetry.LogException(ex);
            telemetry.Message = string.Format("Job failed. {0} feed/protocol migration jobs scheduled, {1} jobs not scheduled due to HostDoesNotExistException/HostShutdownException, {2} actions taken for feeds.", (object) migrationJobsScheduled, (object) migrationJobsNotScheduled, (object) totalFeedsChanged);
            return JobResult.Failed(telemetry);
          }
          return new JobResult()
          {
            Result = TeamFoundationJobExecutionResult.Succeeded,
            Telemetry = new JobTelemetry()
            {
              Message = string.Format("{0} feed/protocol migration jobs scheduled, {1} jobs not scheduled due to HostDoesNotExistException/HostShutdownException, {2} actions taken for feeds.", (object) migrationJobsScheduled, (object) migrationJobsNotScheduled, (object) totalFeedsChanged)
            }
          };
        }
        catch (Exception ex)
        {
          throw new JobFailedException(ex);
        }
        // ISSUE: variable of a compiler-generated type
        MigrationKickerJobHandler.\u003C\u003Ec__DisplayClass5_0 cDisplayClass50;

        async Task<(int scheduled, int notSchedulable, int feedsUpdated)> ApplyChangesTo(
          IReadOnlyCollection<MigrationEntry> states)
        {
          IMigrationTransitionerInternal transitionerInternal = this.transitionerFactory.Get();
          // ISSUE: reference to a compiler-generated method
          if (!cDisplayClass50.\u003CHandle\u003Eg__RequestInitiatesMigration\u007C2())
          {
            // ISSUE: reference to a compiler-generated method
            List<IMigrationInstruction> list = states.Select<MigrationEntry, IMigrationInstruction>(new Func<MigrationEntry, IMigrationInstruction>(cDisplayClass50.\u003CHandle\u003Eb__9)).ToList<IMigrationInstruction>();
            await transitionerInternal.Apply((IReadOnlyCollection<IMigrationInstruction>) list);
            return (0, 0, states.Count);
          }
          int migrationJobsScheduled = 0;
          int migrationJobsNotScheduled = 0;
          // ISSUE: reference to a compiler-generated method
          List<LockStepInstruction> markInstructionLockstep = states.Where<MigrationEntry>((Func<MigrationEntry, bool>) (e => e.Instruction != MigrationInstructionEnum.LockStep)).Select<MigrationEntry, LockStepInstruction>(new Func<MigrationEntry, LockStepInstruction>(cDisplayClass50.\u003CHandle\u003Eb__8)).ToList<LockStepInstruction>();
          await transitionerInternal.Apply((IReadOnlyCollection<IMigrationInstruction>) markInstructionLockstep);
          foreach (MigrationEntry stateEntry in (IEnumerable<MigrationEntry>) states)
          {
            try
            {
              using (IDisposingFeedJobQueuer jobQueuer = this.jobQueuerFactory.Get((CollectionId) stateEntry.CollectionId))
              {
                Guid guid = await jobQueuer.QueueJob(ProjectFeedsConversionHelper.ExplicitlyCreateFeedCoreWithoutCallingGetFeed(new FeedIdentity(new Guid?(), stateEntry.FeedId)), this.protocol, JobPriorityLevel.Normal);
              }
              ++migrationJobsScheduled;
            }
            catch (HostDoesNotExistException ex)
            {
              tracer.TraceError(string.Format("Could not queue migration job on host {0} because it does not exist (probably migrated or deleted).", (object) stateEntry.CollectionId));
              ++migrationJobsNotScheduled;
            }
            catch (HostShutdownException ex)
            {
              tracer.TraceError(string.Format("Could not queue migration job on host {0} because it is shut down (probably migrating).", (object) stateEntry.CollectionId));
              ++migrationJobsNotScheduled;
            }
          }
          return (migrationJobsScheduled, migrationJobsNotScheduled, markInstructionLockstep.Count);
        }
      }

      bool FeedIsSelected(MigrationEntry entry) => request.Feeds.IsNullOrEmpty<Guid>() || request.Feeds.Contains(entry.FeedId);

      bool FeedShouldBeUpdated(MigrationEntry entry)
      {
        bool flag1 = request.DestinationMigration.Equals(entry.VNextMigration, StringComparison.OrdinalIgnoreCase);
        if (RequestInitiatesMigration())
        {
          bool flag2 = request.SourceMigration == null || request.SourceMigration.Equals(entry.CurrentMigration, StringComparison.OrdinalIgnoreCase);
          return ((entry.VNextState < MigrationStateEnum.Computing ? 0 : (entry.VNextState < MigrationStateEnum.JobLockStep ? 1 : 0)) & (flag2 ? 1 : 0) & (flag1 ? 1 : 0)) != 0 || flag2 && entry.VNextMigration == entry.CurrentMigration && entry.CurrentMigration != request.DestinationMigration;
        }
        return flag1 && TransitionIsValid(entry);
      }

      bool RequestInitiatesMigration() => request.Instruction == MigrationInstructionEnum.LockStep;

      bool TransitionIsValid(MigrationEntry entry)
      {
        switch (request.Instruction)
        {
          case MigrationInstructionEnum.None:
          case MigrationInstructionEnum.Compute:
          case MigrationInstructionEnum.LockStep:
            throw new Exception(string.Format("invalid transition attempted against feed: {0}, instruction: {1}.", (object) entry.FeedId, (object) entry.Instruction));
          case MigrationInstructionEnum.ReadVNext:
            return entry.VNextState == MigrationStateEnum.JobLockStep;
          case MigrationInstructionEnum.Rollback:
            return entry.VNextState != 0;
          case MigrationInstructionEnum.Complete:
            return entry.VNextState == MigrationStateEnum.ReadVNext;
          case MigrationInstructionEnum.RollbackToLockStep:
            return entry.VNextState >= MigrationStateEnum.ReadVNext;
          default:
            throw new Exception(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UnrecognizedMigrationEnum((object) "MigrationInstructionEnum"));
        }
      }
    }
  }
}
