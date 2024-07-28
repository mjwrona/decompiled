// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.ProtocolVersionValidatingHandler
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
  public class ProtocolVersionValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    private readonly string[] allowedProtocolVersions = new string[1]
    {
      "1"
    };

    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      if (!((IEnumerable<string>) this.allowedProtocolVersions).Contains<string>(request.ProtocolSpecificInfo.Metadata.ProtocolVersion))
        throw new InvalidPackageException(Resources.Error_UnsupportedProtocolVersion((object) request.ProtocolSpecificInfo.Metadata.ProtocolVersion, (object) string.Join(",", this.allowedProtocolVersions)));
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
