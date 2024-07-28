// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ReadAggregationAccessorsFactory
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
  public class ReadAggregationAccessorsFactory : IAggregationAccessorFactory
  {
    private readonly IFactory<IAggregation, IAggregationAccessor> accessorFactory;
    private readonly IMigrationDefinitionsProvider migrationsProvider;
    private readonly IFactory<IFeedRequest, Task<string>> readMigrationNameFactory;

    public ReadAggregationAccessorsFactory(
      IFactory<IAggregation, IAggregationAccessor> accessorFactory,
      IMigrationDefinitionsProvider provider,
      IFactory<IFeedRequest, Task<string>> migrationNameFactory)
    {
      this.accessorFactory = accessorFactory;
      this.migrationsProvider = provider;
      this.readMigrationNameFactory = migrationNameFactory;
    }

    public async Task<IReadOnlyList<IAggregationAccessor>> GetAccessorsFor(IFeedRequest feedRequest)
    {
      ReadAggregationAccessorsFactory accessorsFactory = this;
      string migrationName = await accessorsFactory.readMigrationNameFactory.Get(feedRequest);
      // ISSUE: reference to a compiler-generated method
      return (IReadOnlyList<IAggregationAccessor>) accessorsFactory.migrationsProvider.GetMigration(migrationName, feedRequest.Protocol).Aggregations.Select<IAggregation, IAggregationAccessor>(new Func<IAggregation, IAggregationAccessor>(accessorsFactory.\u003CGetAccessorsFor\u003Eb__4_0)).ToList<IAggregationAccessor>();
    }
  }
}
