// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.PackagingRegistration
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.ItemStore;
using Microsoft.VisualStudio.Services.Npm.Server.Migration;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
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
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server
{
  public class PackagingRegistration
  {
    public static void Register(IProtocolRegistrationAcceptor acceptor)
    {
      acceptor.RegisterProtocol((IProtocol) Protocol.npm, (IRequiredProtocolBootstrappers) new PackagingRegistration.NpmBootstrappers(), (IIdentityResolver) NpmIdentityResolver.Instance);
      acceptor.RegisterItemStoreExperience("npm", (Func<IVssRequestContext, IItemStore>) (requestContext => (IItemStore) requestContext.GetService<NpmItemStore>()));
    }

    private class NpmBootstrappers : IRequiredProtocolBootstrappers
    {
      public IBootstrapper<IMigrationDefinitionsProvider> GetMigrationDefinitionsProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IMigrationDefinitionsProvider>) new NpmMigrationDefinitionsProviderBootstrapper(requestContext);
      }

      public IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer> GetCollectionPackageUpstreamRefreshJobQueuerBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer>) new NpmCollectionPackageUpstreamRefreshJobQueuerBootstrapper(requestContext);
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>> GetUpstreamVersionListServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>>) new UpstreamVersionListServiceFactoryBootstrapperGenericizingAdapter<NpmPackageName, SemanticVersion>(ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IUpstreamVersionListService<NpmPackageName, SemanticVersion>>>>(NpmAggregationResolver.Bootstrap(requestContext).FactoryFor<IUpstreamVersionListService<NpmPackageName, SemanticVersion>>()));
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>> GetReadMetadataDocumentServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>>(NpmAggregationResolver.Bootstrap(requestContext).FactoryFor<IReadMetadataDocumentService>());
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>> GetVersionListWithSizeServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>>(NpmAggregationResolver.Bootstrap(requestContext).FactoryFor<IVersionListWithSizeService>());
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>> GetProblemPackageReadServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>>(NpmAggregationResolver.Bootstrap(requestContext).FactoryFor<IProblemPackagesReadService>());
      }

      public IBootstrapper<ICommitLogReader<CommitLogEntry>> GetCommitLogReaderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICommitLogReader<CommitLogEntry>>) new NpmCommitLogFacadeBootstrapper(requestContext);
      }

      public IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>> GetManualUpstreamIngestionBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>) new NpmManualUpstreamIngestionBootstrapper(requestContext, BlockedIdentityContext.Download);
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>> GetUpstreamMetadataManagerFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>) new NpmUpstreamMetadataManagerFactoryBootstrapper(requestContext);
      }

      public IBootstrapper<IPublicRepositoryProvider> GetPublicRepositoryProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        throw new NotImplementedException();
      }
    }
  }
}
