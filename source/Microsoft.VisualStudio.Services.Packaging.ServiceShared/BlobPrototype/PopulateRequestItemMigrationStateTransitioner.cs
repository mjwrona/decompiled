// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.PopulateRequestItemMigrationStateTransitioner
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class PopulateRequestItemMigrationStateTransitioner : 
    IMigrationTransitionerInternal,
    IMigrationTransitioner,
    IMigrationStateReader
  {
    private readonly IMigrationTransitionerInternal delegateToTransitioner;
    private readonly ICache<string, object> cache;

    public PopulateRequestItemMigrationStateTransitioner(
      IMigrationTransitionerInternal delegateToTransitioner,
      ICache<string, object> cache)
    {
      this.delegateToTransitioner = delegateToTransitioner;
      this.cache = cache;
    }

    public async Task<IEnumerable<MigrationEntry>> WhatIf(
      IReadOnlyCollection<IMigrationInstruction> instructions)
    {
      return await this.delegateToTransitioner.WhatIf(instructions);
    }

    public async Task Apply(
      IReadOnlyCollection<IMigrationInstruction> instructions)
    {
      await this.delegateToTransitioner.Apply(instructions);
    }

    public async Task<MigrationEntry> GetOrCreateState(
      CollectionId collectionId,
      Guid feed,
      IProtocol protocol,
      MigrationDefinition defaultMigration = null)
    {
      MigrationEntry state = await this.delegateToTransitioner.GetOrCreateState(collectionId, feed, protocol, defaultMigration);
      this.cache.Set("Packaging.DataCurrentVersion", (object) state?.CurrentMigration);
      this.cache.Set("Packaging.DataDestinationVersion", (object) state?.VNextMigration);
      this.cache.Set("Packaging.DataMigrationState", (object) state?.VNextState);
      return state;
    }

    public async Task<IEnumerable<MigrationEntry>> GetOrCreateStates(
      CollectionId collectionId,
      IProtocol protocol,
      IEnumerable<Guid> feeds)
    {
      return await this.delegateToTransitioner.GetOrCreateStates(collectionId, protocol, feeds);
    }

    public async Task<MigrationEntry> CommitState(
      CollectionId collectionId,
      Guid feed,
      IProtocol protocol)
    {
      return await this.delegateToTransitioner.CommitState(collectionId, feed, protocol);
    }

    public async Task<IEnumerable<MigrationEntry>> GetStateEntries(MigrationStateFilter filter) => await this.delegateToTransitioner.GetStateEntries(filter);

    public IConcurrentIterator<MigrationEntry> GetStateEntriesConcurrentIterator(
      MigrationStateFilter filter)
    {
      return this.delegateToTransitioner.GetStateEntriesConcurrentIterator(filter);
    }

    public async Task<IEnumerable<MigrationEntry>> WhatIf(
      IReadOnlyCollection<IMigrationTransition> transitions)
    {
      return await this.delegateToTransitioner.WhatIf(transitions);
    }

    public async Task Apply(
      IReadOnlyCollection<IMigrationTransition> transitions)
    {
      await this.delegateToTransitioner.Apply(transitions);
    }

    public async Task<MigrationEntry> Delete(
      CollectionId collectionId,
      Guid feed,
      IProtocol protocol)
    {
      return await this.delegateToTransitioner.Delete(collectionId, feed, protocol);
    }
  }
}
