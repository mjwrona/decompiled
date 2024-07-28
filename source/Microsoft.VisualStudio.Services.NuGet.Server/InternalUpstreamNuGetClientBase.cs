// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.InternalUpstreamNuGetClientBase
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Client.Internal;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public abstract class InternalUpstreamNuGetClientBase
  {
    protected readonly 
    #nullable disable
    IFactory<VssNuGetPackageName, Task<ImmutableDictionary<VssNuGetPackageVersion, InternalUpstreamNuGetClientBase.UpstreamVersionInfo>>> VersionInfoFactory;

    protected InternalUpstreamNuGetClientBase() => this.VersionInfoFactory = ByFuncInputFactory.For<VssNuGetPackageName, Task<ImmutableDictionary<VssNuGetPackageVersion, InternalUpstreamNuGetClientBase.UpstreamVersionInfo>>>((Func<VssNuGetPackageName, Task<ImmutableDictionary<VssNuGetPackageVersion, InternalUpstreamNuGetClientBase.UpstreamVersionInfo>>>) (async packageName => await this.GetVersionInfoFromUpstreamAsync(packageName))).SingleElementCache<VssNuGetPackageName, Task<ImmutableDictionary<VssNuGetPackageVersion, InternalUpstreamNuGetClientBase.UpstreamVersionInfo>>>();

    protected abstract Task<IReadOnlyList<NuGetRawVersionWithSourceChainAndListed>> GetRawVersionInfoFromUpstreamAsync(
      VssNuGetPackageName packageId);

    public async Task<IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>> GetPackageVersions(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName)
    {
      return (IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>) (await this.VersionInfoFactory.Get(packageName)).Values.Select<InternalUpstreamNuGetClientBase.UpstreamVersionInfo, VersionWithSourceChain<VssNuGetPackageVersion>>((Func<InternalUpstreamNuGetClientBase.UpstreamVersionInfo, VersionWithSourceChain<VssNuGetPackageVersion>>) (x => VersionWithSourceChain.FromInternalSource<VssNuGetPackageVersion>(x.Version, (IEnumerable<UpstreamSourceInfo>) x.SourceChain))).ToList<VersionWithSourceChain<VssNuGetPackageVersion>>();
    }

    public async Task<NuGetPackageRegistrationState> GetRegistrationState(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName,
      IEnumerable<VssNuGetPackageVersion> versions)
    {
      ImmutableDictionary<VssNuGetPackageVersion, InternalUpstreamNuGetClientBase.UpstreamVersionInfo> immutableDictionary = await this.VersionInfoFactory.Get(packageName);
      ImmutableDictionary<VssNuGetPackageVersion, NuGetRegistrationState>.Builder builder = ImmutableDictionary.CreateBuilder<VssNuGetPackageVersion, NuGetRegistrationState>();
      foreach (VssNuGetPackageVersion version in versions)
      {
        InternalUpstreamNuGetClientBase.UpstreamVersionInfo upstreamVersionInfo;
        if (immutableDictionary.TryGetValue(version, out upstreamVersionInfo))
          builder.Add(version, new NuGetRegistrationState(new VssNuGetPackageIdentity(packageName, upstreamVersionInfo.Version), (NuGetDeprecation) null, upstreamVersionInfo.Listed, new DateTime?(), (IImmutableList<NuGetVulnerability>) ImmutableArray<NuGetVulnerability>.Empty, (NuGetCatalogCursor) null));
      }
      return new NuGetPackageRegistrationState((IImmutableDictionary<VssNuGetPackageVersion, NuGetRegistrationState>) builder.ToImmutable(), (NuGetCatalogCursor) null);
    }

    private async Task<ImmutableDictionary<VssNuGetPackageVersion, InternalUpstreamNuGetClientBase.UpstreamVersionInfo>> GetVersionInfoFromUpstreamAsync(
      VssNuGetPackageName packageId)
    {
      return (await this.GetRawVersionInfoFromUpstreamAsync(packageId)).Select<NuGetRawVersionWithSourceChainAndListed, InternalUpstreamNuGetClientBase.UpstreamVersionInfo>((Func<NuGetRawVersionWithSourceChainAndListed, InternalUpstreamNuGetClientBase.UpstreamVersionInfo>) (x => new InternalUpstreamNuGetClientBase.UpstreamVersionInfo(new VssNuGetPackageVersion(x.DisplayVersion ?? x.NormalizedVersion), x.SourceChain.ToImmutableArray<UpstreamSourceInfo>(), ((int) x.Listed ?? 1) != 0))).ToImmutableDictionary<InternalUpstreamNuGetClientBase.UpstreamVersionInfo, VssNuGetPackageVersion>((Func<InternalUpstreamNuGetClientBase.UpstreamVersionInfo, VssNuGetPackageVersion>) (x => x.Version));
    }

    protected record UpstreamVersionInfo(
      VssNuGetPackageVersion Version,
      ImmutableArray<UpstreamSourceInfo> SourceChain,
      bool Listed)
    ;
  }
}
