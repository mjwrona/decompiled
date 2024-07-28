// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ReadAggregationAccessorFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ReadAggregationAccessorFactoryBootstrapper : 
    IBootstrapper<IAggregationAccessorFactory>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IMigrationDefinitionsProvider migrationsProvider;
    private readonly IFactory<IAggregation, IAggregationAccessor> accessorFactory;

    public ReadAggregationAccessorFactoryBootstrapper(
      IVssRequestContext requestContext,
      IMigrationDefinitionsProvider migrationsProvider,
      IFactory<IAggregation, IAggregationAccessor> accessorFactory)
    {
      this.requestContext = requestContext;
      this.migrationsProvider = migrationsProvider;
      this.accessorFactory = accessorFactory;
    }

    public IAggregationAccessorFactory Bootstrap()
    {
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      return (IAggregationAccessorFactory) new ReadAggregationAccessorsFactory(this.accessorFactory, this.migrationsProvider, (IFactory<IFeedRequest, Task<string>>) new LatestReadMigrationNameFactory(new CachingMigrationTransitionerFactoryBootstrapper(this.requestContext, this.migrationsProvider).Bootstrap(), this.requestContext.GetExecutionEnvironmentFacade(), (ICache<string, MigrationState>) new MigrationStateCacheServiceFacade(this.requestContext, tracerFacade), (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext), this.migrationsProvider, this.requestContext.IsFeatureEnabled("Packaging.UseMigrationStateCache")));
    }
  }
}
