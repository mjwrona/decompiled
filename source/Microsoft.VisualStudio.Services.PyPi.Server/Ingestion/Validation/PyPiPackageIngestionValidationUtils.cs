// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.PyPiPackageIngestionValidationUtils
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public static class PyPiPackageIngestionValidationUtils
  {
    private static readonly Regex ValidNameRegex = new Regex("^([A-Z0-9]|[A-Z0-9][A-Z0-9._-]*[A-Z0-9])$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant, new TimeSpan(0, 0, 10));

    public static void ValidatePackageName(string packageName)
    {
      if (string.IsNullOrWhiteSpace(packageName) || packageName.Length > (int) byte.MaxValue || !PyPiPackageIngestionValidationUtils.ValidNameRegex.IsMatch(packageName))
        throw new InvalidPackageException(Resources.Error_InvalidPyPiPackageName());
    }

    public static PyPiPackageVersion ValidateAndParseVersion(string packageVersion)
    {
      PyPiPackageVersion parsedVersion;
      Exception version = PyPiPackageIngestionValidationUtils.TryValidateAndParseVersion(packageVersion, out parsedVersion);
      if (version != null)
        throw version;
      return parsedVersion;
    }

    public static Exception TryValidateAndParseVersion(
      string packageVersion,
      out PyPiPackageVersion parsedVersion)
    {
      parsedVersion = (PyPiPackageVersion) null;
      if (string.IsNullOrWhiteSpace(packageVersion) || char.IsWhiteSpace(packageVersion[0]) || char.IsWhiteSpace(packageVersion[packageVersion.Length - 1]))
        return (Exception) new InvalidPackageException(Resources.Error_VersionHasLeadingOrTrailingWhitespace());
      if (packageVersion.Length > (int) sbyte.MaxValue)
        return (Exception) new InvalidPackageException(Resources.Error_InvalidPyPiPackageVersion());
      if (!PyPiPackageVersionParser.TryParse(packageVersion, out parsedVersion))
        return (Exception) new InvalidPackageException(Resources.Error_InvalidPyPiPackageVersion());
      try
      {
        if (parsedVersion.SortableVersion.IsNullOrEmpty<char>())
          return (Exception) new InvalidPackageException(Resources.Error_InvalidPyPiPackageVersion());
      }
      catch (InvalidVersionException ex)
      {
        return (Exception) new InvalidPackageException(ex.Message, (Exception) ex);
      }
      return (Exception) null;
    }
  }
}
