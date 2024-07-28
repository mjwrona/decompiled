// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Converters.Name.PyPiRawPackageNameRequestToRequestConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Converters.Name
{
  public class PyPiRawPackageNameRequestToRequestConverter : 
    IConverter<RawPackageNameRequest, PackageNameRequest<PyPiPackageName>>,
    IHaveInputType<RawPackageNameRequest>,
    IHaveOutputType<PackageNameRequest<PyPiPackageName>>
  {
    private readonly IConverter<string, PyPiPackageName> nameConverter;

    public PyPiRawPackageNameRequestToRequestConverter(
      IConverter<string, PyPiPackageName> nameConverter)
    {
      this.nameConverter = nameConverter;
    }

    public PackageNameRequest<PyPiPackageName> Convert(RawPackageNameRequest rawRequest) => new PackageNameRequest<PyPiPackageName>((IFeedRequest) rawRequest, this.nameConverter.Convert(rawRequest.PackageName));
  }
}
