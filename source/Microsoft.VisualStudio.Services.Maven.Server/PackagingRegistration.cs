// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.PackagingRegistration
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations;
using Microsoft.VisualStudio.Services.Maven.Server.Constants;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Upstreams;
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

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public static class PackagingRegistration
  {
    public static void Register(IProtocolRegistrationAcceptor acceptor)
    {
      acceptor.RegisterProtocol((IProtocol) Protocol.Maven, (IRequiredProtocolBootstrappers) new PackagingRegistration.MavenBootstrappers(), (IIdentityResolver) MavenIdentityResolver.Instance);
      acceptor.RegisterItemStoreExperience("maven", (Func<IVssRequestContext, IItemStore>) (requestContext => (IItemStore) requestContext.GetService<MavenItemStore>()));
      acceptor.RegisterBookmarkTokenKey(MavenServerConstants.SnapshotCleanupBookmarkTokenKey);
    }

    private class MavenBootstrappers : IRequiredProtocolBootstrappers
    {
      public IBootstrapper<IMigrationDefinitionsProvider> GetMigrationDefinitionsProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IMigrationDefinitionsProvider>) new MavenMigrationDefinitionsProviderBootstrapper(requestContext);
      }

      public IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer> GetCollectionPackageUpstreamRefreshJobQueuerBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer>) new MavenCollectionPackageUpstreamRefreshJobQueuerBootstrapper(requestContext);
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>> GetUpstreamVersionListServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>>) new UpstreamVersionListServiceFactoryBootstrapperGenericizingAdapter<MavenPackageName, MavenPackageVersion>(ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>>>>(MavenAggregationResolver.Bootstrap(requestContext).FactoryFor<IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>>()));
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>> GetReadMetadataDocumentServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>>(MavenAggregationResolver.Bootstrap(requestContext).FactoryFor<IReadMetadataDocumentService>());
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>> GetVersionListWithSizeServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>>(MavenAggregationResolver.Bootstrap(requestContext).FactoryFor<IVersionListWithSizeService>());
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>> GetProblemPackageReadServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>>(MavenAggregationResolver.Bootstrap(requestContext).FactoryFor<IProblemPackagesReadService>());
      }

      public IBootstrapper<ICommitLogReader<CommitLogEntry>> GetCommitLogReaderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICommitLogReader<CommitLogEntry>>) new MavenCommitLogFacadeBootstrapper(requestContext);
      }

      public IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>> GetManualUpstreamIngestionBootstrapper(
        IVssRequestContext requestContext)
      {
        throw new NotImplementedException();
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>> GetUpstreamMetadataManagerFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>) new MavenUpstreamMetadataManagerFactoryBootstrapper(requestContext);
      }

      public IBootstrapper<IPublicRepositoryProvider> GetPublicRepositoryProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        throw new NotImplementedException();
      }
    }
  }
}
