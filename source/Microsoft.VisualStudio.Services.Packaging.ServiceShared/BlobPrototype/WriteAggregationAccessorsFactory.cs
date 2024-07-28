// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.WriteAggregationAccessorsFactory
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
  public class WriteAggregationAccessorsFactory : IAggregationAccessorFactory
  {
    private readonly IFactory<IAggregation, IAggregationAccessor> accessorFactory;
    private readonly IFactory<IProtocol, IMigrationTransitionerInternal> transitionerFactory;
    private readonly IMigrationDefinitionsProvider provider;
    private readonly IExecutionEnvironment executionEnvironment;

    public WriteAggregationAccessorsFactory(
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
      WriteAggregationAccessorsFactory accessorsFactory = this;
      MigrationEntry state = await accessorsFactory.transitionerFactory.Get(feedRequest.Protocol).GetOrCreateState((CollectionId) accessorsFactory.executionEnvironment.HostId, feedRequest.Feed.Id, feedRequest.Protocol);
      List<IAggregation> aggVersionsFor1 = accessorsFactory.GetAggVersionsFor(feedRequest, state.CurrentMigration);
      if (state.VNextState >= MigrationStateEnum.JobLockStep)
      {
        List<IAggregation> aggVersionsFor2 = accessorsFactory.GetAggVersionsFor(feedRequest, state.VNextMigration);
        aggVersionsFor1.AddRange((IEnumerable<IAggregation>) aggVersionsFor2);
      }
      // ISSUE: reference to a compiler-generated method
      return (IReadOnlyList<IAggregationAccessor>) aggVersionsFor1.Distinct<IAggregation>().Select<IAggregation, IAggregationAccessor>(new Func<IAggregation, IAggregationAccessor>(accessorsFactory.\u003CGetAccessorsFor\u003Eb__5_0)).ToList<IAggregationAccessor>();
    }

    private List<IAggregation> GetAggVersionsFor(IFeedRequest feedRequest, string migration) => new List<IAggregation>((IEnumerable<IAggregation>) this.provider.GetMigration(migration, feedRequest.Protocol).Aggregations);
  }
}
