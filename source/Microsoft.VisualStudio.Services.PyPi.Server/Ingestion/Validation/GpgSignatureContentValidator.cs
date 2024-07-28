// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.GpgSignatureContentValidator
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class GpgSignatureContentValidator : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    private static readonly byte[] ValidSignaturePrefix = Encoding.UTF8.GetBytes("-----BEGIN PGP SIGNATURE-----");

    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      DeflateCompressibleBytes gpgSignature = request.ProtocolSpecificInfo.GpgSignature;
      if (gpgSignature == null)
        return NullResult.NullTask;
      byte[] numArray = gpgSignature.AsUncompressedBytes();
      if (numArray.Length < GpgSignatureContentValidator.ValidSignaturePrefix.Length)
        throw new InvalidPackageException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_InvalidSignatureNotArmored());
      for (int index = 0; index < GpgSignatureContentValidator.ValidSignaturePrefix.Length; ++index)
      {
        if ((int) numArray[index] != (int) GpgSignatureContentValidator.ValidSignaturePrefix[index])
          throw new InvalidPackageException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_InvalidSignatureNotArmored());
      }
      return NullResult.NullTask;
    }
  }
}
