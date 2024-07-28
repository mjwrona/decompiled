// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Utils.NuGetExtractInnerFileFromNupkgUriCalculator
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Utils
{
  public class NuGetExtractInnerFileFromNupkgUriCalculator : 
    INuGetExtractInnerFileFromNupkgUriCalculator
  {
    private readonly Lazy<IResourceUriBinder> unboundFlatContainerUri;

    public NuGetExtractInnerFileFromNupkgUriCalculator(ILocationFacade locationService) => this.unboundFlatContainerUri = new Lazy<IResourceUriBinder>((Func<IResourceUriBinder>) (() => locationService.GetUnboundResourceUri("nuget", ResourceIds.FlatContainer2GetFileResourceId)));

    public string GetExtractInnerFileFromNupkgUri(
      IPackageRequest<VssNuGetPackageIdentity> request,
      string innerFileName)
    {
      string normalizedName = request.PackageId.Name.NormalizedName;
      string normalizedVersion = request.PackageId.Version.NormalizedVersion;
      string nupkgFilePath = request.PackageId.ToNupkgFilePath();
      return this.unboundFlatContainerUri.Value.Bind((IFeedRequest) request, PackagingUriNamePreference.PreferCanonicalId, (object) new
      {
        id = normalizedName,
        version = normalizedVersion,
        file = nupkgFilePath
      }).AppendQuery("extract", innerFileName).AbsoluteUri;
    }
  }
}
