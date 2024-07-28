// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.BlobServiceFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class BlobServiceFactoryBootstrapper : 
    IBootstrapper<
    #nullable disable
    IFactory<ContainerAddress, IBlobService>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IItemStoreBlobEncodingStrategy onPremEncodingStrategy;
    private readonly bool sharded;

    private BlobServiceFactoryBootstrapper(
      IVssRequestContext requestContext,
      IItemStoreBlobEncodingStrategy onPremEncodingStrategy,
      bool sharded)
    {
      this.requestContext = requestContext;
      this.onPremEncodingStrategy = onPremEncodingStrategy;
      this.sharded = sharded;
    }

    public static BlobServiceFactoryBootstrapper CreateSharded(
      IVssRequestContext requestContext,
      IItemStoreBlobEncodingStrategy onPremEncodingStrategy = null)
    {
      return new BlobServiceFactoryBootstrapper(requestContext, onPremEncodingStrategy, true);
    }

    public static BlobServiceFactoryBootstrapper CreateLegacyUnsharded(
      IVssRequestContext requestContext,
      IItemStoreBlobEncodingStrategy onPremEncodingStrategy = null)
    {
      return new BlobServiceFactoryBootstrapper(requestContext, onPremEncodingStrategy, false);
    }

    public IFactory<ContainerAddress, IBlobService> Bootstrap()
    {
      IExecutionEnvironment environmentFacade = this.requestContext.GetExecutionEnvironmentFacade();
      if (!environmentFacade.IsHosted())
        this.requestContext.CheckProjectCollectionRequestContext();
      ITracerService tracerService = this.requestContext.GetTracerFacade();
      IAsyncInvoker azureBlobAsyncInvoker;
      IFactory<IRetryPolicy> azureBlobRetryPolicyFactory;
      if (this.requestContext.IsFeatureEnabledWithLogging("Packaging.UseExponentialBackoffForAzureBlobRetries"))
      {
        azureBlobAsyncInvoker = (IAsyncInvoker) PassThroughInvoker.Instance;
        azureBlobRetryPolicyFactory = new AzureBlobRetryPolicyFactory(this.requestContext.GetExecutionEnvironmentFacade(), (IRegistryService) this.requestContext.GetRegistryFacade()).ExecuteOnceAndKeepReturningSameResult<IRetryPolicy>();
      }
      else
      {
        azureBlobAsyncInvoker = (IAsyncInvoker) new RetryHelper(tracerService, 2, TimeSpan.FromSeconds(2.0), (Func<Exception, bool>) (ex =>
        {
          int? nullable1 = ex is StorageException storageException2 ? new int?(storageException2.RequestInformation.HttpStatusCode) : new int?();
          if (!nullable1.HasValue)
            return false;
          int? nullable2 = nullable1;
          int num = 500;
          return nullable2.GetValueOrDefault() >= num & nullable2.HasValue;
        }));
        azureBlobRetryPolicyFactory = (IFactory<IRetryPolicy>) null;
      }
      return !environmentFacade.IsHosted() ? (IFactory<ContainerAddress, IBlobService>) BootstrapOnPrem() : (IFactory<ContainerAddress, IBlobService>) BootstrapHosted();

      ByFuncInputFactory<ContainerAddress, IBlobService> BootstrapOnPrem()
      {
        IFactory<PackagingItemStore> packagingItemStore = ReturnSameInstanceFactory.For<PackagingItemStore>(this.requestContext.GetService<PackagingItemStore>());
        IItemStoreBlobEncodingStrategy encodingStrategy = this.onPremEncodingStrategy ?? (IItemStoreBlobEncodingStrategy) new Utf8ItemStoreBlobEncodingStrategy();
        return new ByFuncInputFactory<ContainerAddress, IBlobService>((Func<ContainerAddress, IBlobService>) (containerAddress => (IBlobService) new ItemStoreBlobService(this.requestContext, containerAddress, (IFactory<IItemStore>) packagingItemStore, tracerService, encodingStrategy)));
      }

      ByFuncInputFactory<ContainerAddress, IBlobService> BootstrapHosted()
      {
        IPackagingShardProvider<string, IResolvedCloudBlobContainerFactory> shardProvider = this.sharded ? BlobShardProvider.Bootstrap(this.requestContext) : (IPackagingShardProvider<string, IResolvedCloudBlobContainerFactory>) LegacySingleShardProvider.Bootstrap(this.requestContext);
        CancellationFacade cancellation = new CancellationFacade(this.requestContext);
        return new ByFuncInputFactory<ContainerAddress, IBlobService>((Func<ContainerAddress, IBlobService>) (containerAddress => (IBlobService) new AzureBlobService(this.requestContext.ActivityId, this.requestContext.E2EId, shardProvider, containerAddress, azureBlobAsyncInvoker, tracerService, azureBlobRetryPolicyFactory, (ICancellationFacade) cancellation)));
      }
    }
  }
}
