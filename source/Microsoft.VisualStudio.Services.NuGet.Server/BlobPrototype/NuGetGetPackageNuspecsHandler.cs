// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetGetPackageNuspecsHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetGetPackageNuspecsHandler : 
    IAsyncHandler<
    #nullable disable
    NuGetGetNuspecsRequest, IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>>,
    IHaveInputType<NuGetGetNuspecsRequest>,
    IHaveOutputType<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>>
  {
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;

    public NuGetGetPackageNuspecsHandler(
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService)
    {
      this.metadataService = metadataService;
    }

    public async Task<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>> Handle(
      NuGetGetNuspecsRequest request)
    {
      List<INuGetMetadataEntry> versionStatesAsync = await this.metadataService.GetPackageVersionStatesAsync(new PackageNameQuery<INuGetMetadataEntry>((IPackageNameRequest) request));
      if (versionStatesAsync == null)
        return (IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>) ImmutableDictionary<VssNuGetPackageVersion, ContentBytes>.Empty;
      HashSet<VssNuGetPackageVersion> versionsSet = request.Versions.ToHashSet<VssNuGetPackageVersion>();
      return (IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>) versionStatesAsync.Where<INuGetMetadataEntry>((Func<INuGetMetadataEntry, bool>) (metadataEntry => versionsSet.Contains(metadataEntry.PackageIdentity.Version) && metadataEntry.NuspecBytes != null)).ToDictionary<INuGetMetadataEntry, VssNuGetPackageVersion, ContentBytes>((Func<INuGetMetadataEntry, VssNuGetPackageVersion>) (metadataEntry => metadataEntry.PackageIdentity.Version), (Func<INuGetMetadataEntry, ContentBytes>) (metadataEntry => new ContentBytes(metadataEntry.NuspecBytes, metadataEntry.AreBytesCompressed)));
    }
  }
}
