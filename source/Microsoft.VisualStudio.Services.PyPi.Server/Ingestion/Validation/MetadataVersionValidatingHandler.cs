// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.MetadataVersionValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class MetadataVersionValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    private readonly string[] allowedMetadataVersions = new string[5]
    {
      "1.0",
      "1.1",
      "1.2",
      "2.0",
      "2.1"
    };

    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      if (!((IEnumerable<string>) this.allowedMetadataVersions).Contains<string>(request.ProtocolSpecificInfo.Metadata.MetadataVersion))
        throw new InvalidPackageException(Resources.Error_UnsupportedMetadataVersion((object) request.ProtocolSpecificInfo.Metadata.MetadataVersion, (object) string.Join(",", this.allowedMetadataVersions)));
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
