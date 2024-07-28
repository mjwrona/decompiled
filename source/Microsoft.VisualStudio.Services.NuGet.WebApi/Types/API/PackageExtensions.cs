// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.PackageExtensions
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API
{
  public static class PackageExtensions
  {
    public static NuGetPackageVersionDeletionState ToNuGetPackageVersionDeletionState(
      this Package package)
    {
      if (!package.DeletedDate.HasValue)
        throw new ArgumentException("DeletedDate");
      return new NuGetPackageVersionDeletionState(package.Name, package.Version, package.DeletedDate.Value);
    }
  }
}
