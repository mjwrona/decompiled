// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Registry.CrossCollectionUpstreamNpmClient
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Client.Internal;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Registry
{
  public class CrossCollectionUpstreamNpmClient : UpstreamNpmClientBase, IUpstreamNpmClient
  {
    private readonly ITracerService tracer;
    private readonly NpmApiClient npmHttpClient;
    private readonly IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler = (IAsyncHandler<FeedRequest<Stream>, FileStream>) new TemporarilyStoreStreamAsFileHandler();
    private readonly Guid aadTenantId;

    private string FeedId { get; }

    private string ProjectId { get; }

    public CrossCollectionUpstreamNpmClient(
      NpmApiClient httpClient,
      UpstreamSource source,
      ITracerService tracer,
      Guid aadTenantId,
      INpmUpstreamMetadataDocumentParser upstreamMetadataDocumentParser)
      : base(source, upstreamMetadataDocumentParser)
    {
      this.tracer = tracer;
      this.FeedId = string.Format("{0}@{1}", (object) this.UpstreamSource.InternalUpstreamFeedId, (object) this.UpstreamSource.InternalUpstreamViewId);
      Guid? upstreamProjectId = this.UpstreamSource.InternalUpstreamProjectId;
      ref Guid? local = ref upstreamProjectId;
      this.ProjectId = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      this.npmHttpClient = httpClient;
      this.aadTenantId = aadTenantId;
    }

    protected override async Task<string> GetPackageRegistrationTextAsync(NpmPackageName packageName)
    {
      CrossCollectionUpstreamNpmClient sendInTheThisObject = this;
      string registrationTextAsync;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetPackageRegistrationTextAsync)))
      {
        try
        {
          registrationTextAsync = (await sendInTheThisObject.npmHttpClient.GetUnscopedPackageInternalRegistrationAsync(sendInTheThisObject.ProjectId, sendInTheThisObject.FeedId, packageName.FullName, sendInTheThisObject.aadTenantId)).ToString();
        }
        catch (VssServiceResponseException ex)
        {
          sendInTheThisObject.ThrowIfNotFound(ex.HttpStatusCode, packageName);
          sendInTheThisObject.ThrowIfForbidden(ex.HttpStatusCode);
          sendInTheThisObject.ThrowIfUnavailable(ex.HttpStatusCode, (Func<string, VssServiceException>) (message => (VssServiceException) new UpstreamUnavailableException(message, (Exception) ex)));
          throw new UnknownUpstreamErrorException(sendInTheThisObject.GetErrorMessage(ex), (Exception) ex);
        }
      }
      return registrationTextAsync;
    }

    public override async Task<IReadOnlyList<VersionWithSourceChain<SemanticVersion>>> GetVersionList(
      NpmPackageName packageName)
    {
      return (IReadOnlyList<VersionWithSourceChain<SemanticVersion>>) (await this.npmHttpClient.GetPackageVersionsExposedToDownstreamsAsync(this.ProjectId, this.FeedId, packageName.FullName, this.aadTenantId)).VersionInfo.Select<RawVersionWithSourceChain, VersionWithSourceChain<SemanticVersion>>((Func<RawVersionWithSourceChain, VersionWithSourceChain<SemanticVersion>>) (x => VersionWithSourceChain.FromInternalSource<SemanticVersion>(SemanticVersion.Parse(x.NormalizedVersion), x.SourceChain))).ToImmutableList<VersionWithSourceChain<SemanticVersion>>();
    }

    public override async Task<PackageVersionInternalMetadata> GetPackageInternalMetadata(
      NpmPackageName packageName,
      SemanticVersion packageVersion)
    {
      CrossCollectionUpstreamNpmClient sendInTheThisObject = this;
      PackageVersionInternalMetadata internalMetadata;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetPackageInternalMetadata)))
      {
        Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package infoInternalAsync;
        try
        {
          infoInternalAsync = await sendInTheThisObject.npmHttpClient.GetPackageInfoInternalAsync(sendInTheThisObject.ProjectId, sendInTheThisObject.FeedId, packageName.FullName, packageVersion.NormalizedVersion, sendInTheThisObject.aadTenantId);
        }
        catch (VssServiceResponseException ex)
        {
          sendInTheThisObject.ThrowIfNotFound(ex.HttpStatusCode, packageName, packageVersion);
          sendInTheThisObject.ThrowIfUnavailable(ex.HttpStatusCode, (Func<string, VssServiceException>) (message => (VssServiceException) new UpstreamUnavailableException(message, (Exception) ex)));
          sendInTheThisObject.ThrowIfForbidden(ex.HttpStatusCode);
          throw new UnknownUpstreamErrorException(sendInTheThisObject.GetErrorMessage(ex), (Exception) ex);
        }
        IEnumerable<UpstreamSourceInfo> sourceChain = infoInternalAsync.SourceChain;
        List<UpstreamSourceInfo> upstreamSourceInfoList = (sourceChain != null ? sourceChain.ToList<UpstreamSourceInfo>() : (List<UpstreamSourceInfo>) null) ?? new List<UpstreamSourceInfo>();
        upstreamSourceInfoList.Insert(0, UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(sendInTheThisObject.UpstreamSource));
        internalMetadata = new PackageVersionInternalMetadata()
        {
          SourceChain = (IEnumerable<UpstreamSourceInfo>) upstreamSourceInfoList,
          DeprecateMessage = infoInternalAsync.DeprecateMessage
        };
      }
      return internalMetadata;
    }

    public override async Task<FileStream> GetPackageContentStreamAsync(
      FeedCore feed,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      CancellationToken cancellationToken)
    {
      CrossCollectionUpstreamNpmClient sendInTheThisObject = this;
      FileStream contentStreamAsync;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetPackageContentStreamAsync)))
      {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
          using (Stream packageContent = await sendInTheThisObject.npmHttpClient.GetContentUnscopedPackageInternalAsync(sendInTheThisObject.ProjectId, sendInTheThisObject.FeedId, packageName.FullName, packageVersion.DisplayVersion, sendInTheThisObject.aadTenantId, (object) null, new CancellationToken()))
          {
            cancellationToken.ThrowIfCancellationRequested();
            contentStreamAsync = await sendInTheThisObject.temporarilyStorePackageHandler.Handle(new FeedRequest<Stream>(feed, (IProtocol) Protocol.npm, packageContent));
          }
        }
        catch (VssServiceResponseException ex)
        {
          sendInTheThisObject.ThrowIfNotFound(ex.HttpStatusCode, packageName, packageVersion);
          sendInTheThisObject.ThrowIfUnavailable(ex.HttpStatusCode, (Func<string, VssServiceException>) (message => (VssServiceException) new UpstreamUnavailableException(message, (Exception) ex)));
          sendInTheThisObject.ThrowIfForbidden(ex.HttpStatusCode);
          throw new UnknownUpstreamErrorException(sendInTheThisObject.GetErrorMessage(ex), (Exception) ex);
        }
      }
      return contentStreamAsync;
    }

    private string GetErrorMessage(VssServiceResponseException e) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamVssServiceExceptionMessage((object) this.UpstreamSourceUri, (object) e.HttpStatusCode, (object) e.Message);
  }
}
