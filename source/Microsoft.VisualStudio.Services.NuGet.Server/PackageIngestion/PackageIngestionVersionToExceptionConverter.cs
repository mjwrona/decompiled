// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.PackageIngestionVersionToExceptionConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageIndex;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public class PackageIngestionVersionToExceptionConverter : 
    IConverter<string, Exception>,
    IHaveInputType<string>,
    IHaveOutputType<Exception>
  {
    private static readonly NuGetSortableVersionBuilder SortableVersionBuilder = new NuGetSortableVersionBuilder();
    private readonly IFeatureFlagService featureFlagService;

    public PackageIngestionVersionToExceptionConverter(IFeatureFlagService featureFlagService) => this.featureFlagService = featureFlagService;

    public Exception Convert(string packageVersion)
    {
      if (packageVersion == null)
        return (Exception) new ArgumentNullException(nameof (packageVersion));
      if (string.IsNullOrWhiteSpace(packageVersion))
        return (Exception) new ArgumentException(Resources.EmptyVersionNotAllowed(), nameof (packageVersion));
      return packageVersion.Length > 90 ? (Exception) InvalidPackageExceptionHelper.InvalidVersionLength(90) : PackageIngestionVersionToExceptionConverter.SortableVersionBuilder.TryGetSortableVersion(packageVersion, out string _);
    }
  }
}
