// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Registry.PublicUpstreamNpmClient
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Registry
{
  public class PublicUpstreamNpmClient : UpstreamNpmClientBase
  {
    private IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler = (IAsyncHandler<FeedRequest<Stream>, FileStream>) new TemporarilyStoreStreamAsFileHandler();
    private readonly IHttpClient client;
    private readonly ITracerService tracer;

    public PublicUpstreamNpmClient(
      IHttpClient httpClient,
      UpstreamSource source,
      ITracerService tracer,
      INpmUpstreamMetadataDocumentParser upstreamMetadataDocumentParser)
      : base(source, upstreamMetadataDocumentParser)
    {
      this.client = httpClient;
      this.tracer = tracer;
    }

    public override async Task<FileStream> GetPackageContentStreamAsync(
      FeedCore feed,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      CancellationToken cancellationToken)
    {
      PublicUpstreamNpmClient sendInTheThisObject = this;
      FileStream contentStreamAsync;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetPackageContentStreamAsync)))
      {
        cancellationToken.ThrowIfCancellationRequested();
        Uri tarballUri = await sendInTheThisObject.GetTarballUrl(packageName, packageVersion);
        if (tarballUri == (Uri) null)
          throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageVersionNotFound((object) packageName.FullName, (object) packageVersion, (object) feed.FullyQualifiedName));
        HttpResponseMessage response = (HttpResponseMessage) null;
        try
        {
          cancellationToken.ThrowIfCancellationRequested();
          response = await sendInTheThisObject.client.GetAsync(tarballUri, HttpCompletionOption.ResponseContentRead);
          await sendInTheThisObject.CheckAndThrowOnFailure(tarballUri, response, (Action<string>) (msg =>
          {
            throw new PublicUpstreamTgzNotFoundException(msg);
          }));
          cancellationToken.ThrowIfCancellationRequested();
          IAsyncHandler<FeedRequest<Stream>, FileStream> asyncHandler = sendInTheThisObject.temporarilyStorePackageHandler;
          FeedCore feed1 = feed;
          IProtocol protocol = (IProtocol) Protocol.npm;
          FileStream fileStream = await asyncHandler.Handle(new FeedRequest<Stream>(feed1, protocol, await response.Content.ReadAsStreamAsync()));
          asyncHandler = (IAsyncHandler<FeedRequest<Stream>, FileStream>) null;
          feed1 = (FeedCore) null;
          protocol = (IProtocol) null;
          cancellationToken.ThrowIfCancellationRequested();
          contentStreamAsync = fileStream;
        }
        catch (HttpRequestException ex)
        {
          throw new PublicUpstreamFailureException(sendInTheThisObject.CreateErrorMessageAsync(tarballUri, ex.Message), (Exception) ex, tarballUri);
        }
      }
      return contentStreamAsync;
    }

    public override async Task<IReadOnlyList<VersionWithSourceChain<SemanticVersion>>> GetVersionList(
      NpmPackageName packageName)
    {
      return (IReadOnlyList<VersionWithSourceChain<SemanticVersion>>) (await this.GetPackageRegistrationAsync(packageName)).Entries.Select<INpmMetadataEntry, VersionWithSourceChain<SemanticVersion>>((Func<INpmMetadataEntry, VersionWithSourceChain<SemanticVersion>>) (x => VersionWithSourceChain.FromExternalSource<SemanticVersion>(x.PackageIdentity.Version))).ToList<VersionWithSourceChain<SemanticVersion>>();
    }

    protected override async Task<string> GetPackageRegistrationTextAsync(NpmPackageName packageName)
    {
      PublicUpstreamNpmClient sendInTheThisObject = this;
      string registrationTextAsync;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetPackageRegistrationTextAsync)))
      {
        Uri packageUri = NpmUpstreamUtils.GetPackageUriFromUpstreamSource(packageName, sendInTheThisObject.UpstreamSource);
        HttpResponseMessage response = (HttpResponseMessage) null;
        try
        {
          response = await sendInTheThisObject.client.GetAsync(packageUri, HttpCompletionOption.ResponseContentRead);
          await sendInTheThisObject.CheckAndThrowOnFailure(packageUri, response, (Action<string>) (msg => this.ThrowIfNotFound(response.StatusCode, packageName)));
          registrationTextAsync = await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
          throw new PublicUpstreamFailureException(sendInTheThisObject.CreateErrorMessageAsync(packageUri, ex.Message), (Exception) ex, packageUri);
        }
      }
      return registrationTextAsync;
    }

    private async Task<Uri> GetTarballUrl(
      NpmPackageName packageName,
      SemanticVersion packageVersion)
    {
      PublicUpstreamNpmClient sendInTheThisObject = this;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetTarballUrl)))
      {
        INpmMetadataEntry registrationAsync = await sendInTheThisObject.GetPackageVersionRegistrationAsync(packageName, packageVersion);
        if (registrationAsync == null)
          return (Uri) null;
        return registrationAsync.PackageJson.Distribution?.Tarball ?? throw new InvalidUpstreamPackage(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_VersionWasMissingTarball(), (string) null);
      }
    }

    public override async Task<PackageVersionInternalMetadata> GetPackageInternalMetadata(
      NpmPackageName packageName,
      SemanticVersion packageVersion)
    {
      PublicUpstreamNpmClient sendInTheThisObject = this;
      PackageVersionInternalMetadata internalMetadata;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (GetPackageInternalMetadata)))
      {
        INpmMetadataEntry registrationAsync = await sendInTheThisObject.GetPackageVersionRegistrationAsync(packageName, packageVersion);
        UpstreamSourceInfo[] upstreamSourceInfoArray = new UpstreamSourceInfo[1]
        {
          UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(sendInTheThisObject.UpstreamSource)
        };
        internalMetadata = new PackageVersionInternalMetadata()
        {
          SourceChain = (IEnumerable<UpstreamSourceInfo>) upstreamSourceInfoArray,
          DeprecateMessage = registrationAsync.Deprecated
        };
      }
      return internalMetadata;
    }

    private string CreateErrorMessageAsync(Uri requestUri, string exceptionMessage) => Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamError((object) requestUri, (object) exceptionMessage);

    private async Task CheckAndThrowOnFailure(
      Uri requestUri,
      HttpResponseMessage response,
      Action<string> handleNotFound)
    {
      PublicUpstreamNpmClient sendInTheThisObject = this;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (CheckAndThrowOnFailure)))
      {
        if (response.IsSuccessStatusCode)
          ;
        else
        {
          sendInTheThisObject.ThrowIfUnavailable(response.StatusCode, (Func<string, VssServiceException>) (message => (VssServiceException) new PublicUpstreamFailureException(message, requestUri)));
          string str = await response.Content.ReadAsStringAsync();
          if (response.StatusCode != HttpStatusCode.NotFound)
            throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamWebExceptionMessage((object) requestUri, (object) response.StatusCode, (object) str, (object) string.Empty), requestUri);
          handleNotFound(str);
        }
      }
    }

    private async Task<INpmMetadataEntry> GetPackageVersionRegistrationAsync(
      NpmPackageName packageName,
      SemanticVersion packageVersion)
    {
      return (await this.GetPackageRegistrationAsync(packageName)).Entries.SingleOrDefault<INpmMetadataEntry>((Func<INpmMetadataEntry, bool>) (x => x.PackageIdentity.Version.Equals(packageVersion)));
    }
  }
}
