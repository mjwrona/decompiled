// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.NuGetPackageIngestionValidationUtils
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public static class NuGetPackageIngestionValidationUtils
  {
    internal const int MaxPackageIdLength = 100;
    internal const int MaxPackageVersionLength = 90;
    private static readonly Regex ValidIdRegex = new Regex("^\\w+([_.-]\\w+)*$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant, new TimeSpan(0, 0, 10));

    public static void ValidatePackageId(string packageId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageId, nameof (packageId));
      if (packageId.Length > 100)
        throw InvalidPackageExceptionHelper.PackageIdTooLong(100);
      if (!NuGetPackageIngestionValidationUtils.ValidIdRegex.IsMatch(packageId))
        throw InvalidPackageExceptionHelper.PackageIdInvalid();
    }

    internal static void ValidateNuspecSize(int nuspecSize, int maxNuspecSize)
    {
      if (nuspecSize > maxNuspecSize)
        throw InvalidPackageExceptionHelper.NuspecTooLong(nuspecSize, maxNuspecSize);
    }
  }
}
