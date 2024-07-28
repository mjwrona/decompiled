// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.ThrowNotFoundOnInvalidPackageNameValidator
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class ThrowNotFoundOnInvalidPackageNameValidator : IValidator<IPackageNameRequest>
  {
    public void Validate(IPackageNameRequest packageNameRequest)
    {
      try
      {
        CargoPackageNameParser.Parse(packageNameRequest.PackageName.DisplayName);
      }
      catch (InvalidPackageException ex)
      {
        throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageNotFound((object) packageNameRequest.PackageName, (object) packageNameRequest.Feed));
      }
    }
  }
}
