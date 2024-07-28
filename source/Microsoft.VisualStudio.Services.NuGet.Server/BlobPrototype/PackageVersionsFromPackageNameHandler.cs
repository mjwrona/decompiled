// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.PackageVersionsFromPackageNameHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Client.Internal;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class PackageVersionsFromPackageNameHandler : 
    TracingHandler<
    #nullable disable
    PackageNameRequest<VssNuGetPackageName>, NuGetVersionsExposedToDownstreamsResponse>
  {
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;

    public PackageVersionsFromPackageNameHandler(
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      ITracerService tracerService)
      : base(tracerService)
    {
      this.metadataService = metadataService;
    }

    public override async Task<NuGetVersionsExposedToDownstreamsResponse> Handle(
      PackageNameRequest<VssNuGetPackageName> request,
      ITracerBlock tracer)
    {
      NuGetPackageNameQuery packageNameQueryRequest = new NuGetPackageNameQuery((IPackageNameRequest<IPackageName>) request);
      packageNameQueryRequest.Options = new QueryOptions<INuGetMetadataEntry>().WithFilter((Func<INuGetMetadataEntry, bool>) (x => !x.IsDeleted())).OnlyProjecting((Expression<Func<INuGetMetadataEntry, object>>) (v => (object) v.DeletedDate)).OnlyProjecting((Expression<Func<INuGetMetadataEntry, object>>) (v => (object) v.PermanentDeletedDate)).OnlyProjecting((Expression<Func<INuGetMetadataEntry, object>>) (v => v.SourceChain)).OnlyProjecting((Expression<Func<INuGetMetadataEntry, object>>) (v => (object) v.Listed));
      List<INuGetMetadataEntry> versionStatesAsync = await this.metadataService.GetPackageVersionStatesAsync((PackageNameQuery<INuGetMetadataEntry>) packageNameQueryRequest);
      if (versionStatesAsync.Count == 0)
        throw ControllerExceptionHelper.PackageNotFound_LegacyNuGetSpecificType((IPackageName) request.PackageName, request.Feed);
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(request.Feed);
      NuGetVersionsExposedToDownstreamsResponse downstreamsResponse = new NuGetVersionsExposedToDownstreamsResponse();
      downstreamsResponse.Versions = (IReadOnlyList<string>) versionStatesAsync.Select<INuGetMetadataEntry, string>((Func<INuGetMetadataEntry, string>) (x => x.PackageIdentity.Version.NormalizedOriginalCaseVersion)).ToArray<string>();
      downstreamsResponse.VersionInfo = (IReadOnlyList<NuGetRawVersionWithSourceChainAndListed>) versionStatesAsync.Select<INuGetMetadataEntry, NuGetRawVersionWithSourceChainAndListed>((Func<INuGetMetadataEntry, NuGetRawVersionWithSourceChainAndListed>) (x => new NuGetRawVersionWithSourceChainAndListed()
      {
        DisplayVersion = x.PackageIdentity.Version.DisplayVersion,
        NormalizedVersion = x.PackageIdentity.Version.NormalizedOriginalCaseVersion,
        SourceChain = x.SourceChain,
        Listed = new bool?(x.Listed)
      })).ToList<NuGetRawVersionWithSourceChainAndListed>();
      downstreamsResponse.SetSecuredObject(securedObjectReadOnly);
      return downstreamsResponse;
    }
  }
}
