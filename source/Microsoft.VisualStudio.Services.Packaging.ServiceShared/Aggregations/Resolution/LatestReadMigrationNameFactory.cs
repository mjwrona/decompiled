// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.LatestReadMigrationNameFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public class LatestReadMigrationNameFactory : IFactory<IFeedRequest, Task<string>>
  {
    private readonly IFactory<IProtocol, IMigrationTransitionerInternal> transitionerFactory;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly ICache<string, MigrationState> cacheService;
    private readonly ICache<string, object> requestItemsCache;
    private readonly IMigrationDefinitionsProvider migrationDefinitionsProvider;
    private readonly bool useMigrationStateCache;

    public LatestReadMigrationNameFactory(
      IFactory<IProtocol, IMigrationTransitionerInternal> transitionerFactory,
      IExecutionEnvironment executionEnvironment,
      ICache<string, MigrationState> cacheService,
      ICache<string, object> requestItemsCache,
      IMigrationDefinitionsProvider migrationDefinitionsProvider,
      bool useMigrationStateCache)
    {
      this.transitionerFactory = transitionerFactory;
      this.executionEnvironment = executionEnvironment;
      this.cacheService = cacheService;
      this.requestItemsCache = requestItemsCache;
      this.migrationDefinitionsProvider = migrationDefinitionsProvider;
      this.useMigrationStateCache = useMigrationStateCache;
    }

    public async Task<string> Get(IFeedRequest request)
    {
      string cacheKey = this.ToKey(this.executionEnvironment.HostId, request.Feed.Id, request.Protocol.ToString());
      MigrationState val;
      if (this.useMigrationStateCache && this.cacheService.TryGet(cacheKey, out val))
      {
        PackagingEtwTracesUtils.TraceMigrationState(val, request.Protocol, this.requestItemsCache, this.migrationDefinitionsProvider);
        return this.GetReadMigrationNameFrom(val);
      }
      MigrationState state = (MigrationState) await this.transitionerFactory.Get(request.Protocol).GetOrCreateState((CollectionId) this.executionEnvironment.HostId, request.Feed.Id, request.Protocol);
      this.cacheService.Set(cacheKey, state);
      PackagingEtwTracesUtils.TraceMigrationState(state, request.Protocol, this.requestItemsCache, this.migrationDefinitionsProvider);
      return this.GetReadMigrationNameFrom(state);
    }

    private string GetReadMigrationNameFrom(MigrationState state) => state.VNextState == MigrationStateEnum.ReadVNext ? state.VNextMigration : state.CurrentMigration;

    private string ToKey(Guid collectionId, Guid feedId, string protocol) => collectionId.ToString("N") + feedId.ToString("N") + protocol;
  }
}
