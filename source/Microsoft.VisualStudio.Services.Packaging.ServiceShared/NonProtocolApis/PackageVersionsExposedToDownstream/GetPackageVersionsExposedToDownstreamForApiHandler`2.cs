// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream.GetPackageVersionsExposedToDownstreamForApiHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream
{
  public class GetPackageVersionsExposedToDownstreamForApiHandler<TPackageName, TPackageVersion> : 
    IAsyncHandler<IRawPackageNameRequest, VersionsExposedToDownstreamsResponse>,
    IHaveInputType<IRawPackageNameRequest>,
    IHaveOutputType<VersionsExposedToDownstreamsResponse>
    where TPackageName : IPackageName
    where TPackageVersion : IPackageVersion
  {
    private readonly IAsyncHandler<IPackageNameRequest<TPackageName>, IReadOnlyList<VersionWithSourceChain<TPackageVersion>>> innerHandler;
    private readonly IConverter<IRawPackageNameRequest, IPackageNameRequest<TPackageName>> requestConverter;

    public GetPackageVersionsExposedToDownstreamForApiHandler(
      IConverter<IRawPackageNameRequest, IPackageNameRequest<TPackageName>> requestConverter,
      IAsyncHandler<IPackageNameRequest<TPackageName>, IReadOnlyList<VersionWithSourceChain<TPackageVersion>>> innerHandler)
    {
      this.innerHandler = innerHandler;
      this.requestConverter = requestConverter;
    }

    public async Task<VersionsExposedToDownstreamsResponse> Handle(IRawPackageNameRequest rawRequest)
    {
      IReadOnlyList<VersionWithSourceChain<TPackageVersion>> source = await this.innerHandler.Handle(this.requestConverter.Convert(rawRequest));
      return new VersionsExposedToDownstreamsResponse()
      {
        Versions = (IReadOnlyList<string>) source.Select<VersionWithSourceChain<TPackageVersion>, string>((Func<VersionWithSourceChain<TPackageVersion>, string>) (x => x.Version.NormalizedVersion)).ToList<string>(),
        VersionInfo = (IReadOnlyList<RawVersionWithSourceChain>) source.Select<VersionWithSourceChain<TPackageVersion>, RawVersionWithSourceChain>((Func<VersionWithSourceChain<TPackageVersion>, RawVersionWithSourceChain>) (x => new RawVersionWithSourceChain()
        {
          NormalizedVersion = x.Version.NormalizedVersion,
          SourceChain = (IEnumerable<UpstreamSourceInfo>) x.SourceChain
        })).ToList<RawVersionWithSourceChain>()
      };
    }
  }
}
