// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackagingRegistration
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
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
using Microsoft.VisualStudio.Services.PyPi.Server.CommitLog;
using Microsoft.VisualStudio.Services.PyPi.Server.Migration;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public class PackagingRegistration
  {
    public static void Register(IProtocolRegistrationAcceptor acceptor)
    {
      acceptor.RegisterProtocol((IProtocol) Protocol.PyPi, (IRequiredProtocolBootstrappers) new PackagingRegistration.PyPiBootstrappers(), (IIdentityResolver) PyPiIdentityResolver.Instance);
      acceptor.RegisterItemStoreExperience(PyPiItemStore.ExperienceName, (Func<IVssRequestContext, IItemStore>) (requestContext => (IItemStore) requestContext.GetService<PyPiItemStore>()));
    }

    private class PyPiBootstrappers : IRequiredProtocolBootstrappers
    {
      public IBootstrapper<IMigrationDefinitionsProvider> GetMigrationDefinitionsProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IMigrationDefinitionsProvider>) new PyPiMigrationDefinitionsProviderBootstrapper(requestContext);
      }

      public IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer> GetCollectionPackageUpstreamRefreshJobQueuerBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer>) new PyPiCollectionPackageUpstreamRefreshJobQueuerBootstrapper(requestContext);
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>> GetUpstreamVersionListServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>>) new UpstreamVersionListServiceFactoryBootstrapperGenericizingAdapter<PyPiPackageName, PyPiPackageVersion>(ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>>>(PyPiAggregationResolver.Bootstrap(requestContext).FactoryFor<IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>()));
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>> GetReadMetadataDocumentServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>>(PyPiAggregationResolver.Bootstrap(requestContext).FactoryFor<IPyPiMetadataAggregationAccessor>().ConvertBy<IFeedRequest, Task<IPyPiMetadataAggregationAccessor>, Task<IReadMetadataDocumentService>>((Func<Task<IPyPiMetadataAggregationAccessor>, Task<IReadMetadataDocumentService>>) (async x => (IReadMetadataDocumentService) await x)));
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>> GetVersionListWithSizeServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>>(PyPiAggregationResolver.Bootstrap(requestContext).FactoryFor<IVersionListWithSizeService>());
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>> GetProblemPackageReadServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>>(PyPiAggregationResolver.Bootstrap(requestContext).FactoryFor<IProblemPackagesReadService>());
      }

      public IBootstrapper<ICommitLogReader<CommitLogEntry>> GetCommitLogReaderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICommitLogReader<CommitLogEntry>>) new PyPiCommitLogFacadeBootstrapper(requestContext);
      }

      public IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>> GetManualUpstreamIngestionBootstrapper(
        IVssRequestContext requestContext)
      {
        throw new NotImplementedException();
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>> GetUpstreamMetadataManagerFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>) new PyPiUpstreamMetadataManagerFactoryBootstrapper(requestContext);
      }

      public IBootstrapper<IPublicRepositoryProvider> GetPublicRepositoryProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IPublicRepositoryProvider>) ExistingInstanceBootstrapper.Create<IPublicRepositoryProvider<IUpstreamPyPiClient>>(PyPiPublicRepositoryProvider.Bootstrap(requestContext));
      }
    }
  }
}
