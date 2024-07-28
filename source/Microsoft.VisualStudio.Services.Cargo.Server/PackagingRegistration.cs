// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackagingRegistration
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.CommitLog;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.Upstreams;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server
{
  public class PackagingRegistration
  {
    public static void Register(IProtocolRegistrationAcceptor acceptor)
    {
      acceptor.RegisterProtocol((IProtocol) Protocol.Cargo, (IRequiredProtocolBootstrappers) new PackagingRegistration.CargoBootstrappers(), (IIdentityResolver) CargoIdentityResolver.Instance);
      acceptor.RegisterItemStoreExperience(CargoItemStore.ExperienceName, (Func<IVssRequestContext, IItemStore>) (requestContext => (IItemStore) requestContext.GetService<CargoItemStore>()));
    }

    private class CargoBootstrappers : IRequiredProtocolBootstrappers
    {
      public IBootstrapper<IMigrationDefinitionsProvider> GetMigrationDefinitionsProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IMigrationDefinitionsProvider>) new CargoMigrationDefinitionsProviderBootstrapper(requestContext);
      }

      public IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer> GetCollectionPackageUpstreamRefreshJobQueuerBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer>) new CargoUpstreamMetadataCacheJobQueuerBootstrapper(requestContext);
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>> GetUpstreamVersionListServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>>) new UpstreamVersionListServiceFactoryBootstrapperGenericizingAdapter<CargoPackageName, CargoPackageVersion>(ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>>>(CargoAggregationResolver.Bootstrap(requestContext).FactoryFor<IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>()));
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>> GetReadMetadataDocumentServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>>(CargoAggregationResolver.Bootstrap(requestContext).FactoryFor<IReadMetadataDocumentService>());
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>> GetVersionListWithSizeServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>>(CargoAggregationResolver.Bootstrap(requestContext).FactoryFor<IVersionListWithSizeService>());
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>> GetProblemPackageReadServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>>(CargoAggregationResolver.Bootstrap(requestContext).FactoryFor<IProblemPackagesReadService>());
      }

      public IBootstrapper<ICommitLogReader<CommitLogEntry>> GetCommitLogReaderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICommitLogReader<CommitLogEntry>>) new CargoCommitLogFacadeBootstrapper(requestContext);
      }

      public IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>> GetManualUpstreamIngestionBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>) new CargoManualUpstreamIngestionBootstrapper(requestContext);
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>> GetUpstreamMetadataManagerFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>) new CargoUpstreamMetadataManagerFactoryBootstrapper(requestContext);
      }

      public IBootstrapper<IPublicRepositoryProvider> GetPublicRepositoryProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        throw new NotImplementedException();
      }
    }
  }
}
