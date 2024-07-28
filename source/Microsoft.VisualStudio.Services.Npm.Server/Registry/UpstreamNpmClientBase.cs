// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Registry.UpstreamNpmClientBase
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Registry
{
  public abstract class UpstreamNpmClientBase : IUpstreamNpmClient
  {
    private readonly INpmUpstreamMetadataDocumentParser upstreamMetadataDocumentParser;
    private readonly IFactory<NpmPackageName, Task<MetadataDocument<INpmMetadataEntry>>> metadataDocFactory;

    public UpstreamSource UpstreamSource { get; }

    public UpstreamNpmClientBase(
      UpstreamSource upstreamSource,
      INpmUpstreamMetadataDocumentParser upstreamMetadataDocumentParser)
    {
      this.UpstreamSource = upstreamSource;
      this.upstreamMetadataDocumentParser = upstreamMetadataDocumentParser;
      this.metadataDocFactory = ByFuncInputFactory.For<NpmPackageName, Task<MetadataDocument<INpmMetadataEntry>>>((Func<NpmPackageName, Task<MetadataDocument<INpmMetadataEntry>>>) (async packageName => await this.GetPackageRegistrationDocumentFromUpstreamAsync(packageName))).SingleElementCache<NpmPackageName, Task<MetadataDocument<INpmMetadataEntry>>>();
    }

    public Uri UpstreamSourceUri
    {
      get
      {
        Uri result;
        return !Uri.TryCreate(this.UpstreamSource.Location, UriKind.Absolute, out result) ? (Uri) null : result;
      }
    }

    public void ThrowIfNotFound(
      HttpStatusCode status,
      NpmPackageName packageName,
      SemanticVersion version = null)
    {
      if (status == HttpStatusCode.NotFound)
      {
        string str = string.IsNullOrWhiteSpace(this.UpstreamSource.Name) ? this.UpstreamSource.Location : this.UpstreamSource.Name;
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(version == (SemanticVersion) null ? Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageNotFound((object) packageName.FullName, (object) str) : Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageVersionNotFound((object) packageName.FullName, (object) version.NormalizedVersion, (object) str));
      }
    }

    public void ThrowIfUnavailable(
      HttpStatusCode status,
      Func<string, VssServiceException> createException)
    {
      if (status == HttpStatusCode.ServiceUnavailable)
        throw createException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamUnavailableAt((object) this.UpstreamSource.Location));
    }

    public void ThrowIfForbidden(HttpStatusCode status)
    {
      if (status == HttpStatusCode.Forbidden)
        throw new FeedNeedsPermissionsException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamUnavailableAt((object) this.UpstreamSource.Location));
    }

    protected abstract Task<string> GetPackageRegistrationTextAsync(NpmPackageName packageName);

    public async Task<MetadataDocument<INpmMetadataEntry>> GetPackageRegistrationAsync(
      NpmPackageName packageName)
    {
      return await this.metadataDocFactory.Get(packageName);
    }

    public abstract Task<PackageVersionInternalMetadata> GetPackageInternalMetadata(
      NpmPackageName packageName,
      SemanticVersion packageVersion);

    public abstract Task<FileStream> GetPackageContentStreamAsync(
      FeedCore feed,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      CancellationToken cancellationToken);

    public abstract Task<IReadOnlyList<VersionWithSourceChain<SemanticVersion>>> GetVersionList(
      NpmPackageName packageName);

    private async Task<MetadataDocument<INpmMetadataEntry>> GetPackageRegistrationDocumentFromUpstreamAsync(
      NpmPackageName packageName)
    {
      string registrationTextAsync = await this.GetPackageRegistrationTextAsync(packageName);
      return this.upstreamMetadataDocumentParser.ParseUpstreamMetadataDocument(this.UpstreamSource, packageName, registrationTextAsync);
    }
  }
}
