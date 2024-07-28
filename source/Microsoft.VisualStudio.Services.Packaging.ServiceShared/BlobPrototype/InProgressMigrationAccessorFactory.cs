// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.InProgressMigrationAccessorFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class InProgressMigrationAccessorFactory : IAggregationAccessorFactory
  {
    private readonly IFactory<IAggregation, IAggregationAccessor> accessorFactory;
    private readonly IFactory<IProtocol, IMigrationTransitionerInternal> transitionerFactory;
    private readonly IMigrationDefinitionsProvider provider;
    private readonly IExecutionEnvironment executionEnvironment;

    public InProgressMigrationAccessorFactory(
      IFactory<IAggregation, IAggregationAccessor> accessorFactory,
      IFactory<IProtocol, IMigrationTransitionerInternal> transitionerFactory,
      IMigrationDefinitionsProvider provider,
      IExecutionEnvironment executionEnvironment)
    {
      this.accessorFactory = accessorFactory;
      this.transitionerFactory = transitionerFactory;
      this.provider = provider;
      this.executionEnvironment = executionEnvironment;
    }

    public async Task<IReadOnlyList<IAggregationAccessor>> GetAccessorsFor(IFeedRequest feedRequest)
    {
      MigrationEntry state = await this.transitionerFactory.Get(feedRequest.Protocol).GetOrCreateState((CollectionId) this.executionEnvironment.HostId, feedRequest.Feed.Id, feedRequest.Protocol);
      if (state.VNextState < MigrationStateEnum.Computing || state.VNextState > MigrationStateEnum.JobCatchup)
        return (IReadOnlyList<IAggregationAccessor>) Array.Empty<IAggregationAccessor>();
      MigrationDefinition vCurrMigration = this.provider.GetMigration(state.CurrentMigration, feedRequest.Protocol);
      return (IReadOnlyList<IAggregationAccessor>) this.provider.GetMigration(state.VNextMigration, feedRequest.Protocol).Aggregations.Where<IAggregation>((Func<IAggregation, bool>) (aggregationVersion => !vCurrMigration.Aggregations.Contains(aggregationVersion))).Select<IAggregation, IAggregationAccessor>((Func<IAggregation, IAggregationAccessor>) (agg => this.accessorFactory.Get(agg))).ToList<IAggregationAccessor>();
    }
  }
}
