// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity.PyPiRawPackageRequestToIdentityConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity
{
  public class PyPiRawPackageRequestToIdentityConverter : 
    IConverter<IRawPackageRequest, PyPiPackageIdentity>,
    IHaveInputType<IRawPackageRequest>,
    IHaveOutputType<PyPiPackageIdentity>
  {
    public PyPiPackageIdentity Convert(IRawPackageRequest input)
    {
      PyPiPackageVersion parsedVersion;
      if (!PyPiPackageVersionParser.TryParse(input.PackageVersion, out parsedVersion))
        throw new InvalidPackageException(Resources.Error_InvalidPyPiPackageVersion());
      return new PyPiPackageIdentity(new PyPiPackageName(input.PackageName), parsedVersion);
    }
  }
}
