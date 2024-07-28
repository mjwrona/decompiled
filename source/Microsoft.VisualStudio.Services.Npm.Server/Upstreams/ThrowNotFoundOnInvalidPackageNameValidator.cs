// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.ThrowNotFoundOnInvalidPackageNameValidator
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class ThrowNotFoundOnInvalidPackageNameValidator : IValidator<IPackageNameRequest>
  {
    public void Validate(IPackageNameRequest packageNameRequest)
    {
      try
      {
        NpmPackageName npmPackageName = new NpmPackageName(packageNameRequest.PackageName.NormalizedName);
      }
      catch (Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException ex)
      {
        throw new PackageNotFoundException(Resources.Error_PackageNotFound((object) packageNameRequest.PackageName.NormalizedName, (object) packageNameRequest.Feed.FullyQualifiedName));
      }
    }
  }
}
