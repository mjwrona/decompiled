// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Registry.IUpstreamNpmClient
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Registry
{
  public interface IUpstreamNpmClient
  {
    Task<MetadataDocument<INpmMetadataEntry>> GetPackageRegistrationAsync(NpmPackageName packageName);

    Task<PackageVersionInternalMetadata> GetPackageInternalMetadata(
      NpmPackageName packageName,
      SemanticVersion packageVersion);

    Task<FileStream> GetPackageContentStreamAsync(
      FeedCore feed,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      CancellationToken cancellationToken);

    Task<IReadOnlyList<VersionWithSourceChain<SemanticVersion>>> GetVersionList(
      NpmPackageName packageName);
  }
}
