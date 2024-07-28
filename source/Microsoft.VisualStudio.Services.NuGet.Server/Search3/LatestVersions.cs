// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.LatestVersions
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class LatestVersions
  {
    public VssNuGetPackageVersion LatestVersion { get; }

    public VssNuGetPackageVersion AbsoluteLatestVersion { get; }

    public LatestVersions(
      VssNuGetPackageVersion latestVersion,
      VssNuGetPackageVersion absoluteLatestVersion)
    {
      this.LatestVersion = latestVersion;
      this.AbsoluteLatestVersion = absoluteLatestVersion;
    }
  }
}
