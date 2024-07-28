// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackagingRegistration
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.NuGet.Server.ItemStore;
using Microsoft.VisualStudio.Services.NuGet.Server.Migration;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories;
using Microsoft.VisualStudio.Services.NuGet.Server.Upstreams;
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

namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public class PackagingRegistration
  {
    public static void Register(IProtocolRegistrationAcceptor acceptor)
    {
      acceptor.RegisterProtocol((IProtocol) Protocol.NuGet, (IRequiredProtocolBootstrappers) new PackagingRegistration.NuGetBootstrappers(), (IIdentityResolver) NuGetIdentityResolver.Instance);
      acceptor.RegisterItemStoreExperience("nuget", (Func<IVssRequestContext, IItemStore>) (requestContext => (IItemStore) requestContext.GetService<NuGetItemStore>()));
    }

    private class NuGetBootstrappers : IRequiredProtocolBootstrappers
    {
      public IBootstrapper<IMigrationDefinitionsProvider> GetMigrationDefinitionsProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IMigrationDefinitionsProvider>) new NuGetMigrationDefinitionsProviderBootstrapper(requestContext);
      }

      public IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer> GetCollectionPackageUpstreamRefreshJobQueuerBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer>) new NuGetCollectionPackageUpstreamRefreshJobQueuerBootstrapper(requestContext);
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>> GetUpstreamVersionListServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamVersionListService>>>) new UpstreamVersionListServiceFactoryBootstrapperGenericizingAdapter<VssNuGetPackageName, VssNuGetPackageVersion>(ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>>>(NuGetAggregationResolver.Bootstrap(requestContext).FactoryFor<IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>()));
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>> GetReadMetadataDocumentServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IReadMetadataDocumentService>>>(NuGetAggregationResolver.Bootstrap(requestContext).FactoryFor<IReadMetadataDocumentService>());
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>> GetVersionListWithSizeServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IVersionListWithSizeService>>>(NuGetAggregationResolver.Bootstrap(requestContext).FactoryFor<IVersionListWithSizeService>());
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>> GetProblemPackageReadServiceFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return ExistingInstanceBootstrapper.Create<IFactory<IFeedRequest, Task<IProblemPackagesReadService>>>(NuGetAggregationResolver.Bootstrap(requestContext).FactoryFor<IProblemPackagesReadService>());
      }

      public IBootstrapper<ICommitLogReader<CommitLogEntry>> GetCommitLogReaderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<ICommitLogReader<CommitLogEntry>>) new NuGetCommitLogFacadeBootstrapper(requestContext);
      }

      public IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>> GetManualUpstreamIngestionBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>) new NugetManualUpstreamIngestionBootstrapper(requestContext);
      }

      public IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>> GetUpstreamMetadataManagerFactoryBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>) new NuGetUpstreamMetadataManagerFactoryBootstrapper(requestContext);
      }

      public IBootstrapper<IPublicRepositoryProvider> GetPublicRepositoryProviderBootstrapper(
        IVssRequestContext requestContext)
      {
        return (IBootstrapper<IPublicRepositoryProvider>) ExistingInstanceBootstrapper.Create<IPublicRepositoryProvider<IUpstreamNuGetClient>>(NuGetPublicRepositoryProvider.Bootstrap(requestContext));
      }
    }
  }
}
