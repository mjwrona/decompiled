// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.UriValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class UriValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      if (request.IngestionDirection == IngestionDirection.PullFromUpstream)
        return Task.FromResult<NullResult>((NullResult) null);
      if (request.ProtocolSpecificInfo.Metadata.HomePage != null && !UriValidator.IsValidUri(request.ProtocolSpecificInfo.Metadata.HomePage))
        throw new InvalidPackageException(Resources.Error_InvalidUrl((object) request.ProtocolSpecificInfo.Metadata.HomePage));
      if (request.ProtocolSpecificInfo.Metadata.DownloadUrl != null && !UriValidator.IsValidUri(request.ProtocolSpecificInfo.Metadata.DownloadUrl))
        throw new InvalidPackageException(Resources.Error_InvalidUrl((object) request.ProtocolSpecificInfo.Metadata.DownloadUrl));
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
