// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Utils.NuGetPackageRetrievalValidationUtils
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Utils
{
  public static class NuGetPackageRetrievalValidationUtils
  {
    public static VssNuGetPackageIdentity ValidateAndParsePackageIdentity(string id, string version) => new VssNuGetPackageIdentity(NuGetPackageRetrievalValidationUtils.ValidateAndParsePackageName(id), NuGetPackageRetrievalValidationUtils.ValidateAndParsePackageVersion(version));

    public static VssNuGetPackageVersion ValidateAndParsePackageVersion(string version) => (!string.IsNullOrWhiteSpace(version) ? VssNuGetPackageVersion.ParseOrDefault(version) : throw ControllerExceptionHelper.ArgumentRequiredPackageVersion()) ?? throw ControllerExceptionHelper.InvalidVersionFormat();

    public static VssNuGetPackageName ValidateAndParsePackageName(string id) => !string.IsNullOrWhiteSpace(id) ? new VssNuGetPackageName(id) : throw ControllerExceptionHelper.ArgumentRequiredPackageId();
  }
}
