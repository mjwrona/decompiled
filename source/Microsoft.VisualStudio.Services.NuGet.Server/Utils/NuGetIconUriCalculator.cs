// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Utils.NuGetIconUriCalculator
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Utils
{
  public class NuGetIconUriCalculator : INuGetIconUriCalculator
  {
    private readonly INuGetExtractInnerFileFromNupkgUriCalculator innerFileUriCalculator;

    public NuGetIconUriCalculator(
      INuGetExtractInnerFileFromNupkgUriCalculator innerFileUriCalculator)
    {
      this.innerFileUriCalculator = innerFileUriCalculator;
    }

    public string GetIconUriString(
      IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata> request)
    {
      if (!string.IsNullOrWhiteSpace(request.AdditionalData.IconFile))
        return this.innerFileUriCalculator.GetExtractInnerFileFromNupkgUri((IPackageRequest<VssNuGetPackageIdentity>) request, request.AdditionalData.IconFile);
      return !string.IsNullOrWhiteSpace(request.AdditionalData.IconUrl) ? request.AdditionalData.IconUrl : string.Empty;
    }
  }
}
