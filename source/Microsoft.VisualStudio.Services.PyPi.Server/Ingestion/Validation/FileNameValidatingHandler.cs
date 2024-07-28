// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.FileNameValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

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
  public class FileNameValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    private const string ValidWheelRegexPattern = "\r\n            ^\r\n            (?<namever>(?<name>.+?)-(?<ver>.*?))\r\n            (\r\n                (-(?<build>\\d[^-]*?))?\r\n                -(?<pyver>.+?)\r\n                -(?<abi>.+?)\r\n                -(?<plat>.+?)\r\n                \\.whl\r\n                |\\.dist-info)\r\n            $\r\n            ";
    private static readonly Regex ValidWheelRegex = new Regex("\r\n            ^\r\n            (?<namever>(?<name>.+?)-(?<ver>.*?))\r\n            (\r\n                (-(?<build>\\d[^-]*?))?\r\n                -(?<pyver>.+?)\r\n                -(?<abi>.+?)\r\n                -(?<plat>.+?)\r\n                \\.whl\r\n                |\\.dist-info)\r\n            $\r\n            ", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant, new TimeSpan(0, 0, 10));
    private static readonly Regex ValidMacPlatformRegex = new Regex("macosx_10_(\\d+)+_(?<arch>.*)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant, new TimeSpan(0, 0, 10));
    private static readonly string[] AllowedPlatforms = new string[17]
    {
      "any",
      "win32",
      "win_amd64",
      "win_ia64",
      "manylinux1_x86_64",
      "manylinux1_i686",
      "manylinux2010_x86_64",
      "manylinux2010_i686",
      "manylinux2014_x86_64",
      "manylinux2014_i686",
      "manylinux2014_aarch64",
      "manylinux2014_armv7l",
      "manylinux2014_ppc64",
      "manylinux2014_ppc64le",
      "manylinux2014_s390x",
      "linux_armv6l",
      "linux_armv7l"
    };
    private static readonly string[] MacosxArches = new string[9]
    {
      "ppc",
      "ppc64",
      "i386",
      "x86_64",
      "intel",
      "fat",
      "fat32",
      "fat64",
      "universal"
    };

    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      int distType = (int) request.ProtocolSpecificInfo.Metadata.DistType;
      string filePath = request.ProtocolSpecificInfo.PackageFileStream.FilePath;
      if (filePath.Contains("\\") || filePath.Contains("/"))
        throw new InvalidPackageException(Resources.Error_InvalidFilename((object) filePath));
      string normalizedName = request.PackageId.Name.NormalizedName;
      if (distType != 3)
      {
        if (!PyPiPackageName.MakeNameSafe(filePath).StartsWith(normalizedName))
          throw new InvalidPackageException(Resources.Error_InvalidFilename((object) filePath));
        return Task.FromResult<NullResult>((NullResult) null);
      }
      Match match = FileNameValidatingHandler.ValidWheelRegex.Match(filePath);
      if (!match.Success)
        throw new InvalidPackageException(Resources.Error_InvalidFilename((object) filePath));
      string str = PyPiPackageName.MakeNameSafe(match.Groups["name"].Value);
      string rawVersion = match.Groups["ver"].Value;
      if (!str.Equals(normalizedName, StringComparison.Ordinal))
        throw new InvalidPackageException(Resources.Error_InvalidFilename((object) filePath));
      PyPiPackageVersion parsedVersion;
      if (request.IngestionDirection != IngestionDirection.PullFromUpstream && (!PyPiPackageVersionParser.TryParse(rawVersion, out parsedVersion) || !parsedVersion.CanonicalVersion.Equals(request.PackageId.Version.CanonicalVersion)))
        throw new InvalidPackageException(Resources.Error_InvalidFilename((object) filePath));
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
