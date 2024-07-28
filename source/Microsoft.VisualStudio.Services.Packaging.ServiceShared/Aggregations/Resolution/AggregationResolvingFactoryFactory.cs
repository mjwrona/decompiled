// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.AggregationResolvingFactoryFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public class AggregationResolvingFactoryFactory : IAggregationResolvingFactoryFactory
  {
    private readonly IFactory<IFeedRequest, Task<string>> migrationNameFactory;
    private readonly IMigrationDefinitionsProvider migrationsProvider;
    private readonly IFactory<IAggregation, IAggregationAccessor> accessorFactory;
    private readonly ITracerService tracerService;

    public AggregationResolvingFactoryFactory(
      IFactory<IFeedRequest, Task<string>> migrationNameFactory,
      IMigrationDefinitionsProvider migrationsProvider,
      IFactory<IAggregation, IAggregationAccessor> accessorFactory,
      ITracerService tracerService)
    {
      this.migrationNameFactory = migrationNameFactory;
      this.migrationsProvider = migrationsProvider;
      this.accessorFactory = accessorFactory;
      this.tracerService = tracerService;
    }

    public IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped>(
      AggregationHandlerPolicy policy,
      IEnumerable<IRequireAggBootstrapper<TBootstrapped>> bootstrapperSequence)
      where TBootstrapped : class
    {
      ReadAggregationAccessorsFactory readMigrationAccessorFactory = new ReadAggregationAccessorsFactory(this.accessorFactory, this.migrationsProvider, this.migrationNameFactory);
      return (IFactory<IFeedRequest, Task<TBootstrapped>>) new AggregationResolvingFactory<TBootstrapped>(policy, this.tracerService, bootstrapperSequence, (IAggregationAccessorFactory) readMigrationAccessorFactory);
    }

    public static IAggregationResolvingFactoryFactory Bootstrap(
      IVssRequestContext requestContext,
      IMigrationDefinitionsProvider migrationDefinitionsProvider)
    {
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      IFactory<IProtocol, IMigrationTransitionerInternal> transitionerFactory = new CachingMigrationTransitionerFactoryBootstrapper(requestContext, migrationDefinitionsProvider).Bootstrap();
      return (IAggregationResolvingFactoryFactory) new AggregationResolvingFactoryFactory((IFactory<IFeedRequest, Task<string>>) new ChooseFirstNonNullInputFactory<IFeedRequest, Task<string>>(new IFactory<IFeedRequest, Task<string>>[2]
      {
        (IFactory<IFeedRequest, Task<string>>) new MigrationFromRequestContextFactory(requestContext, (IRegistryService) requestContext.GetRegistryFacade()),
        (IFactory<IFeedRequest, Task<string>>) new LatestReadMigrationNameFactory(transitionerFactory, requestContext.GetExecutionEnvironmentFacade(), (ICache<string, MigrationState>) new MigrationStateCacheServiceFacade(requestContext, tracerFacade), (ICache<string, object>) new RequestContextItemsAsCacheFacade(requestContext), migrationDefinitionsProvider, true)
      }), migrationDefinitionsProvider, new AggregationAccessorFactoryBootstrapper(requestContext).Bootstrap(), tracerFacade);
    }
  }
}
