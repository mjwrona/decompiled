// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationTransitioner
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MigrationTransitioner : 
    IMigrationTransitionerInternal,
    IMigrationTransitioner,
    IMigrationStateReader
  {
    private static readonly IReadOnlyList<int> allowedStateTransitionJumps = (IReadOnlyList<int>) new int[2]
    {
      0,
      1
    };
    private readonly IFactory<IMigrationStateWriter> migrationStateWriterFactory;
    private readonly IMigrationDefinitionsProvider migrationProvider;
    private readonly ITracerService tracerService;

    public MigrationTransitioner(
      IFactory<IMigrationStateWriter> migrationStateWriterFactory,
      IMigrationDefinitionsProvider migrationProvider,
      ITracerService tracerService)
    {
      this.migrationStateWriterFactory = migrationStateWriterFactory;
      this.migrationProvider = migrationProvider;
      this.tracerService = tracerService;
    }

    public async Task Apply(
      IReadOnlyCollection<IMigrationInstruction> instructions)
    {
      MigrationTransitioner sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, "ApplyInstructions"))
      {
        if (!instructions.Any<IMigrationInstruction>())
          return;
        IProtocol protocol = sendInTheThisObject.GetProtocolFrom((IReadOnlyCollection<IMigrationEdit>) instructions);
        List<IGrouping<CollectionId, IMigrationInstruction>> list = instructions.GroupBy<IMigrationInstruction, CollectionId>((Func<IMigrationInstruction, CollectionId>) (instruction => instruction.CollectionId)).ToList<IGrouping<CollectionId, IMigrationInstruction>>();
        List<MigrationTransitionException> exceptionList = (List<MigrationTransitionException>) null;
        foreach (IGrouping<CollectionId, IMigrationInstruction> grouping in list)
        {
          MigrationTransitioner migrationTransitioner = sendInTheThisObject;
          IGrouping<CollectionId, IMigrationInstruction> collectionInstructions = grouping;
          try
          {
            List<Guid> feedsListForCacheInvalidation = new List<Guid>();
            foreach (IMigrationInstruction migrationInstruction in (IEnumerable<IMigrationInstruction>) collectionInstructions)
            {
              if (migrationInstruction.InstructionName != MigrationInstructionEnum.Compute)
                feedsListForCacheInvalidation.Add(migrationInstruction.FeedId);
            }
            IEnumerable<MigrationEntry> migrationEntries = await sendInTheThisObject.ApplyUsingMap(protocol, collectionInstructions.Key, (Func<IDictionary<string, MigrationEntry>, TransformResult>) (states =>
            {
              migrationTransitioner.ApplyInstructionsInternal(states, (IReadOnlyCollection<IMigrationInstruction>) collectionInstructions.ToList<IMigrationInstruction>());
              return TransformResult.DirtyEdit;
            }), (IList<Guid>) feedsListForCacheInvalidation);
          }
          catch (AggregateMigrationTransitionException ex)
          {
            exceptionList = exceptionList ?? new List<MigrationTransitionException>();
            exceptionList.AddRange((IEnumerable<MigrationTransitionException>) ex.Exceptions);
          }
        }
        if (exceptionList != null)
          throw new AggregateMigrationTransitionException((IReadOnlyCollection<MigrationTransitionException>) exceptionList);
        protocol = (IProtocol) null;
        exceptionList = (List<MigrationTransitionException>) null;
      }
    }

    public async Task Apply(
      IReadOnlyCollection<IMigrationTransition> transitions)
    {
      MigrationTransitioner sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, "ApplyTransitions"))
      {
        if (!transitions.Any<IMigrationTransition>())
          return;
        IProtocol protocol = sendInTheThisObject.GetProtocolFrom((IReadOnlyCollection<IMigrationEdit>) transitions);
        foreach (IGrouping<CollectionId, IMigrationTransition> grouping in transitions.GroupBy<IMigrationTransition, CollectionId>((Func<IMigrationTransition, CollectionId>) (transition => transition.CollectionId)).ToList<IGrouping<CollectionId, IMigrationTransition>>())
        {
          MigrationTransitioner migrationTransitioner = sendInTheThisObject;
          IGrouping<CollectionId, IMigrationTransition> collectionTransitions = grouping;
          List<Guid> feedsListForCacheInvalidation = new List<Guid>();
          foreach (IMigrationTransition migrationTransition in (IEnumerable<IMigrationTransition>) collectionTransitions)
          {
            if (!(migrationTransition is MarkAsComputingTransition))
              feedsListForCacheInvalidation.Add(migrationTransition.FeedId);
          }
          IEnumerable<MigrationEntry> migrationEntries = await sendInTheThisObject.ApplyUsingMap(protocol, collectionTransitions.Key, (Func<IDictionary<string, MigrationEntry>, TransformResult>) (states =>
          {
            migrationTransitioner.ApplyStateTransitionsInternal(states, (IEnumerable<IMigrationTransition>) collectionTransitions);
            return TransformResult.DirtyEdit;
          }), (IList<Guid>) feedsListForCacheInvalidation);
        }
        protocol = (IProtocol) null;
      }
    }

    public async Task<IEnumerable<MigrationEntry>> WhatIf(
      IReadOnlyCollection<IMigrationInstruction> instructions)
    {
      if (!instructions.Any<IMigrationInstruction>())
        return (IEnumerable<MigrationEntry>) new MigrationEntry[0];
      IDictionary<string, MigrationEntry> allStateMap = await this.GetAllStateMap(this.GetProtocolFrom((IReadOnlyCollection<IMigrationEdit>) instructions));
      this.ApplyInstructionsInternal(allStateMap, instructions);
      return (IEnumerable<MigrationEntry>) allStateMap.Values;
    }

    private IProtocol GetProtocolFrom(IReadOnlyCollection<IMigrationEdit> editList)
    {
      List<IProtocol> list = editList.Select<IMigrationEdit, IProtocol>((Func<IMigrationEdit, IProtocol>) (i => i.Protocol)).Distinct<IProtocol>().ToList<IProtocol>();
      return list.Count <= 1 ? list.Single<IProtocol>() : throw new AggregateMigrationTransitionException((IReadOnlyCollection<MigrationTransitionException>) editList.Select<IMigrationEdit, MigrationTransitionException>((Func<IMigrationEdit, MigrationTransitionException>) (i => new MigrationTransitionException(i.Protocol, i.FeedId, i.CollectionId.Guid, "migration edit contained multiple protocols."))).ToList<MigrationTransitionException>());
    }

    public async Task<IEnumerable<MigrationEntry>> WhatIf(
      IReadOnlyCollection<IMigrationTransition> transitions)
    {
      if (!transitions.Any<IMigrationTransition>())
        return (IEnumerable<MigrationEntry>) new MigrationEntry[0];
      IDictionary<string, MigrationEntry> allStateMap = await this.GetAllStateMap(this.GetProtocolFrom((IReadOnlyCollection<IMigrationEdit>) transitions));
      this.ApplyStateTransitionsInternal(allStateMap, (IEnumerable<IMigrationTransition>) transitions);
      return (IEnumerable<MigrationEntry>) allStateMap.Values;
    }

    public Task<MigrationEntry> CommitState(
      CollectionId collectionId,
      Guid feed,
      IProtocol protocol)
    {
      return this.GetOrCreateState(collectionId, feed, protocol, (MigrationDefinition) null);
    }

    public async Task<MigrationEntry> GetOrCreateState(
      CollectionId collectionId,
      Guid feed,
      IProtocol protocol,
      MigrationDefinition defaultMigration = null)
    {
      MigrationTransitioner sendInTheThisObject = this;
      MigrationEntry state1;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetOrCreateState)))
      {
        string key = sendInTheThisObject.ToKey(feed, protocol.ToString());
        state1 = (await sendInTheThisObject.ApplyUsingMap(protocol, collectionId, (Func<IDictionary<string, MigrationEntry>, TransformResult>) (keyToStateMap =>
        {
          if (keyToStateMap.ContainsKey(key))
            return TransformResult.NoOp;
          MigrationDefinition migration = defaultMigration ?? this.migrationProvider.GetDefaultMigration(protocol, (FeedId) feed).ThrowIfNull<MigrationDefinition>(closure_3 ?? (closure_3 = (Func<Exception>) (() => (Exception) new InvalidMigrationException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_MigrationNotFound((object) string.Format("{0}", (object) protocol))))));
          if (migration.Protocol != protocol)
            throw new InvalidOperationException("Cannot set state for protocol " + protocol.CorrectlyCasedName + " to migration " + migration.Name + " because it is for protocol " + migration.Protocol.CorrectlyCasedName);
          keyToStateMap[key] = MigrationTransitioner.GetMigrationEntryForInitialState(collectionId, protocol, feed, migration);
          return TransformResult.DirtyEdit;
        }))).ToDictionary<MigrationEntry, string>((Func<MigrationEntry, string>) (state => this.ToKey(state.FeedId, state.Protocol)))[key];
      }
      return state1;
    }

    public async Task<IEnumerable<MigrationEntry>> GetOrCreateStates(
      CollectionId collectionId,
      IProtocol protocol,
      IEnumerable<Guid> feeds)
    {
      MigrationTransitioner sendInTheThisObject = this;
      IEnumerable<MigrationEntry> list;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetOrCreateStates)))
      {
        List<Guid> feedsList = feeds.ToList<Guid>();
        Dictionary<Guid, string> keyDictionary = feedsList.ToDictionary<Guid, Guid, string>((Func<Guid, Guid>) (feed => feed), (Func<Guid, string>) (feed => this.ToKey(feed, protocol.ToString())));
        Dictionary<string, MigrationEntry> updatedKeyToStateMap = (await sendInTheThisObject.ApplyUsingMap(protocol, collectionId, (Func<IDictionary<string, MigrationEntry>, TransformResult>) (keyToStateMap =>
        {
          TransformResult states = TransformResult.NoOp;
          foreach (Guid guid in feedsList)
          {
            if (!keyToStateMap.ContainsKey(keyDictionary[guid]))
            {
              MigrationDefinition defaultMigration = this.migrationProvider.GetDefaultMigration(protocol, (FeedId) guid);
              keyToStateMap[keyDictionary[guid]] = MigrationTransitioner.GetMigrationEntryForInitialState(collectionId, protocol, guid, defaultMigration);
              states = TransformResult.DirtyEdit;
            }
          }
          return states;
        }))).ToDictionary<MigrationEntry, string>((Func<MigrationEntry, string>) (state => this.ToKey(state.FeedId, state.Protocol)));
        list = (IEnumerable<MigrationEntry>) feedsList.Select<Guid, MigrationEntry>((Func<Guid, MigrationEntry>) (feed => updatedKeyToStateMap[keyDictionary[feed]])).ToList<MigrationEntry>();
      }
      return list;
    }

    private void ApplyInstructionsInternal(
      IDictionary<string, MigrationEntry> keyToStateMap,
      IReadOnlyCollection<IMigrationInstruction> instructions)
    {
      List<MigrationTransitionException> transitionExceptionList = new List<MigrationTransitionException>();
      foreach (IMigrationInstruction instruction in (IEnumerable<IMigrationInstruction>) instructions)
      {
        try
        {
          this.ApplyInstructionInternal(keyToStateMap, instruction);
        }
        catch (MigrationTransitionException ex)
        {
          transitionExceptionList.Add(ex);
        }
      }
      if (transitionExceptionList.Any<MigrationTransitionException>())
        throw new AggregateMigrationTransitionException((IReadOnlyCollection<MigrationTransitionException>) transitionExceptionList);
    }

    private void ApplyInstructionInternal(
      IDictionary<string, MigrationEntry> keyToStateMap,
      IMigrationInstruction instruction)
    {
      switch (instruction)
      {
        case ComputeInstruction _:
        case LockStepInstruction _:
          this.PromoteInstruction(keyToStateMap, instruction);
          break;
        case ReadVNextInstruction _:
          this.ApplyStateTransitionInternal(keyToStateMap, (IMigrationTransition) new MarkAsVNextReadTransition(instruction.DestinationMigration, instruction.CollectionId, instruction.FeedId, instruction.Protocol));
          keyToStateMap[this.ToKey(instruction.FeedId, instruction.Protocol.ToString())].Instruction = instruction.InstructionName;
          break;
        case RollbackInstruction _:
          this.ApplyStateTransitionInternal(keyToStateMap, (IMigrationTransition) new RollbackMigrationTransition(instruction.DestinationMigration, instruction.CollectionId, instruction.FeedId, instruction.Protocol));
          break;
        case RollbackToLockStepInstruction _:
          this.ApplyStateTransitionInternal(keyToStateMap, (IMigrationTransition) new MarkAsJobLockStepTransition(instruction.DestinationMigration, instruction.CollectionId, instruction.FeedId, instruction.Protocol));
          keyToStateMap[this.ToKey(instruction.FeedId, instruction.Protocol.ToString())].Instruction = MigrationInstructionEnum.LockStep;
          break;
        case CompleteInstruction _:
          this.ApplyStateTransitionInternal(keyToStateMap, (IMigrationTransition) new CompleteMigrationTransition(instruction.DestinationMigration, instruction.CollectionId, instruction.FeedId, instruction.Protocol));
          break;
        case FeedDeletedInstruction _:
          this.ApplyStateTransitionInternal(keyToStateMap, (IMigrationTransition) new FeedDeletedTransition(instruction.DestinationMigration, instruction.CollectionId, instruction.FeedId, instruction.Protocol));
          break;
        default:
          throw new MigrationTransitionException(instruction, string.Format("The instruction {0} is not supported", (object) instruction.InstructionName));
      }
    }

    private void PromoteInstruction(
      IDictionary<string, MigrationEntry> keyToStateMap,
      IMigrationInstruction instruction)
    {
      string key = this.ToKey(instruction.FeedId, instruction.Protocol.ToString());
      if (!keyToStateMap.ContainsKey(key))
      {
        MigrationDefinition defaultMigration = this.migrationProvider.GetDefaultMigration(instruction.Protocol, (FeedId) instruction.FeedId);
        MigrationEntry entryForInitialState = MigrationTransitioner.GetMigrationEntryForInitialState(instruction.CollectionId, instruction.Protocol, instruction.FeedId, defaultMigration);
        if (entryForInitialState.CurrentMigration != instruction.DestinationMigration)
        {
          entryForInitialState.VNextMigration = instruction.DestinationMigration;
          entryForInitialState.Instruction = instruction.InstructionName;
          entryForInitialState.VNextState = MigrationStateEnum.Computing;
        }
        keyToStateMap[key] = entryForInitialState;
      }
      else
      {
        MigrationEntry keyToState = keyToStateMap[key];
        if (keyToState.VNextState > MigrationStateEnum.Computing)
          throw new MigrationTransitionException(instruction, string.Format("cannot promote the migration instruction of an in progress migration. migration state is: {0}", (object) keyToState.VNextState));
        if (instruction.InstructionName < keyToState.Instruction)
          throw new MigrationTransitionException(instruction, string.Format("migration cannot go backwards. current instruction is: {0} and asking for {1}", (object) keyToState.Instruction, (object) instruction.InstructionName));
        if (keyToState.CurrentMigration == keyToState.VNextMigration)
        {
          keyToState.VNextMigration = instruction.DestinationMigration;
          keyToState.VNextState = MigrationStateEnum.Computing;
        }
        this.CheckStateExistsAndCorrectDestination(keyToStateMap, instruction.CollectionId, instruction.FeedId, instruction.Protocol, instruction.DestinationMigration);
        keyToState.Instruction = instruction.InstructionName;
        this.ThrowOnInvalidMigrationStateAfterPromote(instruction, (MigrationState) keyToState);
      }
    }

    private void ThrowOnInvalidMigrationStateAfterPromote(
      IMigrationInstruction instruction,
      MigrationState state)
    {
      MigrationDefinition migration1 = this.migrationProvider.GetMigration(state.CurrentMigration, instruction.Protocol);
      MigrationDefinition migration2 = this.migrationProvider.GetMigration(state.VNextMigration, instruction.Protocol);
      if (!migration1.CommitLogBaseline.Equals(migration2.CommitLogBaseline))
        throw new MigrationTransitionException(instruction, "curr and vnext do not have same commit log baseline");
      if (migration2.IsDeprecated)
        throw new MigrationTransitionException(instruction, "vnext migration to " + migration2.Name + " is deprecated and cannot be progressed.");
    }

    private static MigrationEntry GetMigrationEntryForInitialState(
      CollectionId collectionId,
      IProtocol protocol,
      Guid feed,
      MigrationDefinition migration)
    {
      MigrationEntry entryForInitialState = new MigrationEntry();
      entryForInitialState.CurrentMigration = migration.Name;
      entryForInitialState.VNextMigration = migration.Name;
      entryForInitialState.Instruction = MigrationInstructionEnum.None;
      entryForInitialState.VNextState = MigrationStateEnum.None;
      entryForInitialState.Protocol = protocol.ToString();
      entryForInitialState.CollectionId = collectionId.Guid;
      entryForInitialState.FeedId = feed;
      return entryForInitialState;
    }

    private void ApplyStateTransitionsInternal(
      IDictionary<string, MigrationEntry> keyToStateMap,
      IEnumerable<IMigrationTransition> transitions)
    {
      List<MigrationTransitionException> transitionExceptionList = new List<MigrationTransitionException>();
      foreach (IMigrationTransition transition in transitions)
      {
        try
        {
          this.ApplyStateTransitionInternal(keyToStateMap, transition);
        }
        catch (MigrationTransitionException ex)
        {
          transitionExceptionList.Add(ex);
        }
      }
      if (transitionExceptionList.Any<MigrationTransitionException>())
        throw new AggregateMigrationTransitionException((IReadOnlyCollection<MigrationTransitionException>) transitionExceptionList);
    }

    private void ApplyStateTransitionInternal(
      IDictionary<string, MigrationEntry> keyToStateMap,
      IMigrationTransition transition)
    {
      this.CheckStateExistsAndCorrectDestination(keyToStateMap, transition.CollectionId, transition.FeedId, transition.Protocol, transition.DestinationMigration);
      string key = this.ToKey(transition.FeedId, transition.Protocol.ToString());
      MigrationEntry keyToState = keyToStateMap[key];
      switch (transition)
      {
        case RollbackMigrationTransition _:
          this.Reset(keyToState);
          break;
        case FeedDeletedTransition _:
          this.Reset(keyToState);
          keyToState.FeedIsDeleted = true;
          break;
        case MarkAsComputingTransition _:
          MarkAsComputingTransition transition1 = (MarkAsComputingTransition) transition;
          this.MarkStateAndStatus(keyToState, (IMigrationTransition) transition1, MigrationStateEnum.Computing, transition1.MigrationStatus);
          break;
        case MarkAsJobCatchupTransition _:
          MarkAsJobCatchupTransition transition2 = (MarkAsJobCatchupTransition) transition;
          this.MarkStateAndStatus(keyToState, (IMigrationTransition) transition2, MigrationStateEnum.JobCatchup, transition2.MigrationStatus);
          break;
        case MarkAsJobLockStepTransition _:
          this.MarkState(keyToState, MigrationStateEnum.JobLockStep, transition);
          break;
        case MarkAsVNextReadTransition _:
          this.MarkState(keyToState, MigrationStateEnum.ReadVNext, transition);
          break;
        case CompleteMigrationTransition _:
          this.Complete(keyToState, (CompleteMigrationTransition) transition);
          break;
        default:
          throw new MigrationTransitionException(transition, "Transition " + transition?.GetType()?.Name + " is not supported");
      }
    }

    private void MarkStateAndStatus(
      MigrationEntry state,
      IMigrationTransition transition,
      MigrationStateEnum destinationState,
      MigrationProcessingStatus migrationStatus)
    {
      this.MarkState(state, destinationState, transition);
      state.MigrationProgress = migrationStatus;
    }

    private void MarkState(
      MigrationEntry currState,
      MigrationStateEnum destinationState,
      IMigrationTransition transition)
    {
      if (!MigrationTransitioner.allowedStateTransitionJumps.Contains<int>(destinationState - currState.VNextState) && (currState.VNextState != MigrationStateEnum.ReadVNext || destinationState != MigrationStateEnum.JobLockStep))
        throw new MigrationTransitionException(transition, string.Format("invalid state transition: {0} to {1}", (object) currState.VNextState, (object) destinationState));
      currState.VNextState = destinationState;
      currState.MigrationProgress = (MigrationProcessingStatus) null;
    }

    private void Complete(MigrationEntry state, CompleteMigrationTransition transition)
    {
      state.CurrentMigration = state.VNextState == MigrationStateEnum.ReadVNext ? state.VNextMigration : throw new MigrationTransitionException((IMigrationTransition) transition, string.Format("Can only complete a migration that is in {0}", (object) MigrationStateEnum.ReadVNext));
      this.Reset(state);
    }

    private void Reset(MigrationEntry state)
    {
      state.VNextMigration = state.CurrentMigration;
      state.MigrationProgress = (MigrationProcessingStatus) null;
      state.VNextState = MigrationStateEnum.None;
      state.Instruction = MigrationInstructionEnum.None;
    }

    private void CheckStateExistsAndCorrectDestination(
      IDictionary<string, MigrationEntry> keyToStateMap,
      CollectionId collectionId,
      Guid feedId,
      IProtocol protocol,
      string destinationMigration)
    {
      string key = this.ToKey(feedId, protocol.ToString());
      MigrationEntry migrationEntry = keyToStateMap.ContainsKey(key) ? keyToStateMap[key] : throw new MigrationTransitionException(protocol, feedId, collectionId.Guid, string.Format("no state for {0}+{1}, thus cannot change its state", (object) protocol, (object) feedId));
      if (keyToStateMap[key].VNextMigration != destinationMigration)
        throw new MigrationTransitionException(protocol, feedId, collectionId.Guid, string.Format("migration state change is not applicable because the destination migration {0} is not the same as the current state's vnext migration: {1} for this {2}+{3}", (object) destinationMigration, (object) migrationEntry.VNextMigration, (object) feedId, (object) protocol));
    }

    public async Task<MigrationEntry> Delete(
      CollectionId collectionId,
      Guid feed,
      IProtocol protocol)
    {
      MigrationTransitioner migrationTransitioner = this;
      string key = migrationTransitioner.ToKey(feed, protocol.ToString());
      MigrationEntry migrationEntryToDelete = (MigrationEntry) null;
      IEnumerable<MigrationEntry> migrationEntries = await migrationTransitioner.ApplyUsingMap(protocol, collectionId, (Func<IDictionary<string, MigrationEntry>, TransformResult>) (x =>
      {
        if (x == null || !x.ContainsKey(key))
          return TransformResult.NoOp;
        migrationEntryToDelete = x[key];
        x.Remove(key);
        return TransformResult.DirtyEdit;
      }), (IList<Guid>) new List<Guid>() { feed });
      return migrationEntryToDelete;
    }

    public async Task<IEnumerable<MigrationEntry>> GetStateEntries(MigrationStateFilter filter = null) => await this.migrationStateWriterFactory.Get().GetStateEntries(filter);

    public IConcurrentIterator<MigrationEntry> GetStateEntriesConcurrentIterator(
      MigrationStateFilter filter)
    {
      return this.migrationStateWriterFactory.Get().GetStateEntriesConcurrentIterator(filter);
    }

    private async Task<IDictionary<string, MigrationEntry>> GetAllStateMap(IProtocol protocol)
    {
      MigrationTransitioner migrationTransitioner = this;
      // ISSUE: reference to a compiler-generated method
      return (IDictionary<string, MigrationEntry>) (await migrationTransitioner.migrationStateWriterFactory.Get().GetStateEntries(new MigrationStateFilter(protocol))).ToDictionary<MigrationEntry, string>(new Func<MigrationEntry, string>(migrationTransitioner.\u003CGetAllStateMap\u003Eb__28_0));
    }

    private async Task<IEnumerable<MigrationEntry>> ApplyUsingMap(
      IProtocol protocol,
      CollectionId collectionId,
      Func<IDictionary<string, MigrationEntry>, TransformResult> applyTransform,
      IList<Guid> feedsListForCacheInvalidation = null)
    {
      return (IEnumerable<MigrationEntry>) await this.migrationStateWriterFactory.Get().Apply(protocol, collectionId, (ApplyTransform) ((ref IList<MigrationEntry> entries) =>
      {
        Dictionary<string, MigrationEntry> dictionary = entries.ToDictionary<MigrationEntry, string>((Func<MigrationEntry, string>) (entry => this.ToKey(entry.FeedId, entry.Protocol)));
        int num = (int) applyTransform((IDictionary<string, MigrationEntry>) dictionary);
        entries = (IList<MigrationEntry>) dictionary.Values.ToList<MigrationEntry>();
        return (TransformResult) num;
      }), feedsListForCacheInvalidation);
    }

    private string ToKey(Guid feed, string protocol) => string.Format("{0}_{1}", (object) protocol, (object) feed);
  }
}
