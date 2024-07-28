// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CachingMigrationTransitioner
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class CachingMigrationTransitioner : 
    IMigrationTransitionerInternal,
    IMigrationTransitioner,
    IMigrationStateReader
  {
    private readonly IMigrationTransitionerInternal transitioner;
    private readonly ICache<string, MigrationEntry> cache;

    public CachingMigrationTransitioner(
      IMigrationTransitionerInternal transitioner,
      ICache<string, MigrationEntry> cache)
    {
      this.transitioner = transitioner;
      this.cache = cache;
    }

    public async Task<IEnumerable<MigrationEntry>> WhatIf(
      IReadOnlyCollection<IMigrationInstruction> instructions)
    {
      return await this.transitioner.WhatIf(instructions);
    }

    public async Task Apply(
      IReadOnlyCollection<IMigrationInstruction> instructions)
    {
      CachingMigrationTransitioner migrationTransitioner = this;
      List<IMigrationInstruction> instructionList = instructions.ToList<IMigrationInstruction>();
      await migrationTransitioner.transitioner.Apply((IReadOnlyCollection<IMigrationInstruction>) instructionList);
      // ISSUE: reference to a compiler-generated method
      foreach (string key in instructionList.Select<IMigrationInstruction, string>(new Func<IMigrationInstruction, string>(migrationTransitioner.\u003CApply\u003Eb__4_0)))
        migrationTransitioner.cache.Invalidate(key);
      instructionList = (List<IMigrationInstruction>) null;
    }

    public async Task<MigrationEntry> GetOrCreateState(
      CollectionId collectionId,
      Guid feed,
      IProtocol protocol,
      MigrationDefinition defaultMigration = null)
    {
      string key = this.ToKey(feed, protocol.ToString());
      MigrationEntry val;
      if (this.cache.TryGet(key, out val))
        return val;
      MigrationEntry state = await this.transitioner.GetOrCreateState(collectionId, feed, protocol, defaultMigration);
      this.cache.Set(key, state);
      return state;
    }

    public async Task<IEnumerable<MigrationEntry>> GetOrCreateStates(
      CollectionId collectionId,
      IProtocol protocol,
      IEnumerable<Guid> feeds)
    {
      List<Guid> feedsList = feeds.ToList<Guid>();
      Dictionary<Guid, string> keyDictionary = feedsList.ToDictionary<Guid, Guid, string>((Func<Guid, Guid>) (feed => feed), (Func<Guid, string>) (feed => this.ToKey(feed, protocol.ToString())));
      List<MigrationEntry> states = new List<MigrationEntry>();
      bool flag = false;
      foreach (Guid key in feedsList)
      {
        MigrationEntry val;
        if (!this.cache.TryGet(keyDictionary[key], out val))
        {
          flag = true;
          break;
        }
        states.Add(val);
      }
      if (!flag)
        return (IEnumerable<MigrationEntry>) states;
      Dictionary<Guid, MigrationEntry> dictionary = (await this.transitioner.GetOrCreateStates(collectionId, protocol, (IEnumerable<Guid>) feedsList)).ToDictionary<MigrationEntry, Guid>((Func<MigrationEntry, Guid>) (x => x.FeedId));
      foreach (Guid key in feedsList)
        this.cache.Set(keyDictionary[key], dictionary[key]);
      return dictionary.Select<KeyValuePair<Guid, MigrationEntry>, MigrationEntry>((Func<KeyValuePair<Guid, MigrationEntry>, MigrationEntry>) (x => x.Value));
    }

    public async Task<MigrationEntry> CommitState(
      CollectionId collectionId,
      Guid feed,
      IProtocol protocol)
    {
      MigrationEntry val = await this.transitioner.CommitState(collectionId, feed, protocol);
      this.cache.Set(this.ToKey(feed, protocol.ToString()), val);
      return val;
    }

    public async Task<IEnumerable<MigrationEntry>> GetStateEntries(MigrationStateFilter filter)
    {
      List<MigrationEntry> entriesToReturn = new List<MigrationEntry>();
      foreach (MigrationEntry val1 in (await this.transitioner.GetStateEntries(filter)).ToList<MigrationEntry>())
      {
        string key = this.ToKey(val1.FeedId, val1.Protocol);
        MigrationEntry val2;
        if (this.cache.TryGet(key, out val2))
        {
          entriesToReturn.Add(val2);
        }
        else
        {
          entriesToReturn.Add(val1);
          this.cache.Set(key, val1);
        }
      }
      IEnumerable<MigrationEntry> stateEntries = (IEnumerable<MigrationEntry>) entriesToReturn;
      entriesToReturn = (List<MigrationEntry>) null;
      return stateEntries;
    }

    public IConcurrentIterator<MigrationEntry> GetStateEntriesConcurrentIterator(
      MigrationStateFilter filter)
    {
      return this.transitioner.GetStateEntriesConcurrentIterator(filter).Select<MigrationEntry, MigrationEntry>((Func<MigrationEntry, MigrationEntry>) (entry => this.cache.GetOrAddNonAtomic<string, MigrationEntry>(this.ToKey(entry.FeedId, entry.Protocol), (Func<MigrationEntry>) (() => entry))));
    }

    public async Task<IEnumerable<MigrationEntry>> WhatIf(
      IReadOnlyCollection<IMigrationTransition> transitions)
    {
      return await this.transitioner.WhatIf(transitions);
    }

    public async Task Apply(
      IReadOnlyCollection<IMigrationTransition> transitions)
    {
      CachingMigrationTransitioner migrationTransitioner = this;
      List<IMigrationTransition> transitionList = transitions.ToList<IMigrationTransition>();
      await migrationTransitioner.transitioner.Apply((IReadOnlyCollection<IMigrationTransition>) transitionList);
      // ISSUE: reference to a compiler-generated method
      foreach (string key in transitionList.Select<IMigrationTransition, string>(new Func<IMigrationTransition, string>(migrationTransitioner.\u003CApply\u003Eb__11_0)))
        migrationTransitioner.cache.Invalidate(key);
      transitionList = (List<IMigrationTransition>) null;
    }

    private string ToKey(Guid feed, string protocol) => feed.ToString("N") + protocol;

    public async Task<MigrationEntry> Delete(
      CollectionId collectionId,
      Guid feed,
      IProtocol protocol)
    {
      MigrationEntry migrationEntry = await this.transitioner.Delete(collectionId, feed, protocol);
      this.cache.Invalidate(this.ToKey(feed, protocol.ToString()));
      return migrationEntry;
    }
  }
}
