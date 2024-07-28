// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.ThrowNotFoundOnInvalidPackageNameValidator
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class ThrowNotFoundOnInvalidPackageNameValidator : IValidator<IPackageNameRequest>
  {
    public void Validate(IPackageNameRequest packageNameRequest)
    {
      try
      {
        NuGetPackageIngestionValidationUtils.ValidatePackageId(packageNameRequest.PackageName.NormalizedName);
      }
      catch (InvalidPackageException ex)
      {
        throw ControllerExceptionHelper.PackageNotFound_LegacyNuGetSpecificType(packageNameRequest.PackageName, packageNameRequest.Feed);
      }
    }
  }
}
