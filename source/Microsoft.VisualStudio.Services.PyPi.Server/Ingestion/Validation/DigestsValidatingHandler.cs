// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.DigestsValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class DigestsValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    private static readonly Regex ValidHexEncodedDigestRegex = new Regex("^[A-F0-9]{64}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant, new TimeSpan(0, 0, 10));

    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      PyPiUploadedPackageMetadata protocolSpecificInfo = request.ProtocolSpecificInfo;
      string md5 = protocolSpecificInfo.Metadata.Md5;
      string sha256 = protocolSpecificInfo.Metadata.Sha256;
      string blake2 = protocolSpecificInfo.Metadata.Blake2;
      int num = !md5.IsNullOrEmpty<char>() ? 1 : 0;
      bool flag = !sha256.IsNullOrEmpty<char>();
      if (num == 0 && !flag)
        throw new InvalidPackageException(Resources.Error_MissingContentValidationDigests());
      if (flag && !DigestsValidatingHandler.ValidHexEncodedDigestRegex.IsMatch(sha256))
        throw new InvalidPackageException(Resources.Error_InvalidSha256Digest());
      if (!blake2.IsNullOrEmpty<char>() && !DigestsValidatingHandler.ValidHexEncodedDigestRegex.IsMatch(blake2))
        throw new InvalidPackageException(Resources.Error_InvalidBlake2Digest());
      if (num != 0 && protocolSpecificInfo.ComputedMd5 != null && !protocolSpecificInfo.ComputedMd5.Equals(md5, StringComparison.OrdinalIgnoreCase))
        throw new InvalidPackageException(Resources.Error_InvalidContentDigest((object) "md5_digest", (object) protocolSpecificInfo.ComputedMd5));
      if (flag && protocolSpecificInfo.ComputedSha256 != null && !protocolSpecificInfo.ComputedSha256.Equals(sha256, StringComparison.OrdinalIgnoreCase))
        throw new InvalidPackageException(Resources.Error_InvalidContentDigest((object) "sha256_digest", (object) protocolSpecificInfo.ComputedSha256));
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
