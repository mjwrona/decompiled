// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.ThrowNotFoundOnInvalidPackageNameValidator
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class ThrowNotFoundOnInvalidPackageNameValidator : IValidator<IPackageNameRequest>
  {
    public void Validate(IPackageNameRequest packageNameRequest)
    {
      try
      {
        PyPiPackageIngestionValidationUtils.ValidatePackageName(packageNameRequest.PackageName.NormalizedName);
      }
      catch (InvalidPackageException ex)
      {
        throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageNotFound((object) packageNameRequest.PackageName, (object) packageNameRequest.Feed));
      }
    }
  }
}
