// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Registry.SameCollectionUpstreamNpmClient
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.PackageDownload;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Registry
{
  public class SameCollectionUpstreamNpmClient : UpstreamNpmClientBase
  {
    private readonly 
    #nullable disable
    string upstreamFeedId;
    private readonly IVssRequestContext requestContext;
    private readonly ITracerService tracer;
    private readonly IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler = (IAsyncHandler<FeedRequest<Stream>, FileStream>) new TemporarilyStoreStreamAsFileHandler();
    private readonly FeedCore upstreamFeed;

    public SameCollectionUpstreamNpmClient(
      IVssRequestContext requestContext,
      UpstreamSource source,
      ITracerService tracer,
      INpmUpstreamMetadataDocumentParser upstreamMetadataDocumentParser)
      : base(source, upstreamMetadataDocumentParser)
    {
      using (tracer.Enter((object) this, ".ctor"))
      {
        Guid projectId = this.UpstreamSource.InternalUpstreamProjectId ?? Guid.Empty;
        this.upstreamFeedId = string.Format("{0}@{1}", (object) this.UpstreamSource.InternalUpstreamFeedId, (object) this.UpstreamSource.InternalUpstreamViewId);
        this.requestContext = requestContext;
        this.tracer = tracer;
        this.upstreamFeed = this.requestContext.GetService<IFeedCacheService>().GetFeed(this.requestContext, projectId, this.upstreamFeedId);
      }
    }

    protected override async Task<string> GetPackageRegistrationTextAsync(NpmPackageName packageName)
    {
      SameCollectionUpstreamNpmClient sendInTheThisObject = this;
      string registrationTextAsync;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetPackageRegistrationTextAsync)))
      {
        try
        {
          FeedRequest feed = new FeedRequest(sendInTheThisObject.upstreamFeed, (IProtocol) Protocol.npm);
          registrationTextAsync = await NpmAggregationResolver.Bootstrap(sendInTheThisObject.requestContext).HandlerFor<RawPackageNameRequest, string>((IRequireAggBootstrapper<IAsyncHandler<RawPackageNameRequest, string>>) new GetPackageRegistrationHandlerBootstrapper(sendInTheThisObject.requestContext)).TaskYieldOnException<RawPackageNameRequest, string>().Handle(new RawPackageNameRequest((IFeedRequest) feed, RawNpmPackageName.Create(packageName.Scope, packageName.UnscopedName)));
        }
        catch (Exception ex) when (!(ex is Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException))
        {
          throw new UnknownUpstreamErrorException(sendInTheThisObject.GetErrorMessage(ex), ex);
        }
      }
      return registrationTextAsync;
    }

    public override async Task<IReadOnlyList<VersionWithSourceChain<SemanticVersion>>> GetVersionList(
      NpmPackageName packageName)
    {
      IPackageNameRequest<NpmPackageName> request = new FeedRequest(this.upstreamFeed, (IProtocol) Protocol.npm).WithPackageName<NpmPackageName>(packageName);
      IReadOnlyList<VersionWithSourceChain<SemanticVersion>> versionList = await (await NpmAggregationResolver.Bootstrap(this.requestContext).FactoryFor<IAsyncHandler<IPackageNameRequest<NpmPackageName>, IReadOnlyList<VersionWithSourceChain<SemanticVersion>>>>((IRequireAggBootstrapper<IAsyncHandler<IPackageNameRequest<NpmPackageName>, IReadOnlyList<VersionWithSourceChain<SemanticVersion>>>>) new GetPackageVersionsExposedToDownstreamHandlerBootstrapper<NpmPackageIdentity, NpmPackageName, SemanticVersion, INpmMetadataEntry>()).Get((IFeedRequest) request)).Handle(request);
      request = (IPackageNameRequest<NpmPackageName>) null;
      return versionList;
    }

    public override async Task<PackageVersionInternalMetadata> GetPackageInternalMetadata(
      NpmPackageName packageName,
      SemanticVersion packageVersion)
    {
      SameCollectionUpstreamNpmClient sendInTheThisObject = this;
      PackageVersionInternalMetadata internalMetadata;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetPackageInternalMetadata)))
      {
        try
        {
          FeedRequest feedRequest = new FeedRequest(sendInTheThisObject.upstreamFeed, (IProtocol) Protocol.npm);
          INpmMetadataEntry npmMetadataEntry = await new NpmMetadataHandlerBootstrapper(sendInTheThisObject.requestContext).Bootstrap().Handle((IPackageRequest<NpmPackageIdentity>) new PackageRequest<NpmPackageIdentity>((IFeedRequest) feedRequest, new NpmPackageIdentity(packageName, packageVersion)));
          IEnumerable<UpstreamSourceInfo> sourceChain = npmMetadataEntry.SourceChain;
          List<UpstreamSourceInfo> upstreamSourceInfoList = (sourceChain != null ? sourceChain.ToList<UpstreamSourceInfo>() : (List<UpstreamSourceInfo>) null) ?? new List<UpstreamSourceInfo>();
          upstreamSourceInfoList.Insert(0, UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(sendInTheThisObject.UpstreamSource));
          internalMetadata = new PackageVersionInternalMetadata()
          {
            SourceChain = (IEnumerable<UpstreamSourceInfo>) upstreamSourceInfoList,
            DeprecateMessage = npmMetadataEntry.Deprecated
          };
        }
        catch (Exception ex) when (!(ex is Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException))
        {
          throw new UnknownUpstreamErrorException(sendInTheThisObject.GetErrorMessage(ex), ex);
        }
      }
      return internalMetadata;
    }

    public override async Task<FileStream> GetPackageContentStreamAsync(
      FeedCore feed,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      try
      {
        FeedRequest feedRequest = new FeedRequest(this.upstreamFeed, (IProtocol) Protocol.npm);
        string tgzFilePath = new NpmPackageIdentity(packageName, packageVersion).ToTgzFilePath();
        HttpResponseMessage httpResponseMessage = await NpmAggregationResolver.Bootstrap(this.requestContext).HandlerFor<RawNpmPackageNameWithFileRequest, HttpResponseMessage>((IRequireAggBootstrapper<IAsyncHandler<RawNpmPackageNameWithFileRequest, HttpResponseMessage>>) new NpmDownloadPackageFileHandlerBootstrapper(this.requestContext)).TaskYieldOnException<RawNpmPackageNameWithFileRequest, HttpResponseMessage>().Handle(new RawNpmPackageNameWithFileRequest((IFeedRequest) feedRequest, packageName.Scope, packageName.UnscopedName, tgzFilePath));
        Uri location = httpResponseMessage.Headers.Location;
        Stream stream;
        if (location != (Uri) null)
        {
          stream = await new HttpClient().GetStreamAsync(location);
          try
          {
            return await this.temporarilyStorePackageHandler.Handle(new FeedRequest<Stream>(feed, (IProtocol) Protocol.npm, stream));
          }
          finally
          {
            stream?.Dispose();
          }
        }
        else
        {
          httpResponseMessage.EnsureSuccessStatusCode();
          stream = await httpResponseMessage.Content.ReadAsStreamAsync();
          try
          {
            cancellationToken.ThrowIfCancellationRequested();
            return await this.temporarilyStorePackageHandler.Handle(new FeedRequest<Stream>(feed, (IProtocol) Protocol.npm, stream));
          }
          finally
          {
            stream?.Dispose();
          }
        }
      }
      catch (Exception ex) when (!(ex is Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException))
      {
        throw new UnknownUpstreamErrorException(this.GetErrorMessage(ex), ex);
      }
    }

    private string GetErrorMessage(Exception e) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamInternalFeedExceptionMessage((object) (this.upstreamFeed?.FullyQualifiedName ?? this.upstreamFeedId), (object) e.Message);
  }
}
