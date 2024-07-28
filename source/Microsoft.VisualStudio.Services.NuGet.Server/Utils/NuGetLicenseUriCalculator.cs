// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Utils.NuGetLicenseUriCalculator
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Utils
{
  public class NuGetLicenseUriCalculator : INuGetLicenseUriCalculator
  {
    private readonly INuGetExtractInnerFileFromNupkgUriCalculator innerFileUriCalculator;

    public NuGetLicenseUriCalculator(
      INuGetExtractInnerFileFromNupkgUriCalculator innerFileUriCalculator)
    {
      this.innerFileUriCalculator = innerFileUriCalculator;
    }

    public string GetLicenseUriString(
      IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata> request)
    {
      if (!string.IsNullOrWhiteSpace(request.AdditionalData.LicenseFile))
        return this.innerFileUriCalculator.GetExtractInnerFileFromNupkgUri((IPackageRequest<VssNuGetPackageIdentity>) request, request.AdditionalData.LicenseFile);
      if (!string.IsNullOrWhiteSpace(request.AdditionalData.LicenseExpression))
        return "https://licenses.nuget.org/" + Uri.EscapeDataString(request.AdditionalData.LicenseExpression);
      return !string.IsNullOrWhiteSpace(request.AdditionalData.LicenseUrl) ? request.AdditionalData.LicenseUrl : string.Empty;
    }
  }
}
