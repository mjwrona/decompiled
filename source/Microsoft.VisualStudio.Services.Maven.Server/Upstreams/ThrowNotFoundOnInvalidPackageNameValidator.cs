// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.ThrowNotFoundOnInvalidPackageNameValidator
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class ThrowNotFoundOnInvalidPackageNameValidator : IValidator<IPackageNameRequest>
  {
    public void Validate(IPackageNameRequest packageNameRequest)
    {
      if (string.IsNullOrWhiteSpace(packageNameRequest.PackageName.NormalizedName))
        throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageNotFound((object) packageNameRequest.PackageName, (object) packageNameRequest.Feed));
    }
  }
}
