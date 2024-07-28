// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.PackageIngestion.IngestionValidationUtils
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Npm.Server.PackageIndex;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Npm.Server.PackageIngestion
{
  public static class IngestionValidationUtils
  {
    public const long MaxPushSizeDefault = 524288000;
    private const long MaxCommitLogEntrySize = 384000;
    public const int MaxNameLength = 214;
    private static readonly RegistryQuery MaxPushSizeRegistryPath = (RegistryQuery) "/Configuration/Packaging/Npm/Ingestion/MaxSize";
    private static readonly NpmSortableVersionBuilder SortableVersionBuilder = new NpmSortableVersionBuilder();

    public static long GetMaxPackageSize(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<long>(requestContext, in IngestionValidationUtils.MaxPushSizeRegistryPath, true, 524288000L);

    public static void ValidatePackageSize(
      IVssRequestContext requestContext,
      long packageSize,
      bool isBase64Length = true)
    {
      long maxPackageSize = IngestionValidationUtils.GetMaxPackageSize(requestContext);
      if ((isBase64Length ? packageSize / 4L * 3L : packageSize) > IngestionValidationUtils.GetMaxPackageSize(requestContext))
        throw new PackageTooLargeException(Resources.Error_PackageTooLarge((object) maxPackageSize));
    }

    public static void ValidatePackageJsonSize(long packageJsonSize)
    {
      if (packageJsonSize > 384000L)
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageJsonTooLarge((object) 384000L));
    }

    public static NpmPackageName ParseAndValidatePackageName(string rawPackageName)
    {
      if (string.IsNullOrEmpty(rawPackageName))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageNameMustNotBeNullOrEmpty());
      if (SpecialPackageNames.ExemptNames.Contains(rawPackageName))
        return new NpmPackageName(rawPackageName);
      if (rawPackageName.Length > 214)
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageNameTooLong((object) 214));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (rawPackageName.Any<char>(IngestionValidationUtils.\u003C\u003EO.\u003C0\u003E__IsUpper ?? (IngestionValidationUtils.\u003C\u003EO.\u003C0\u003E__IsUpper = new Func<char, bool>(char.IsUpper))))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageNameMustBeLowercase());
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (rawPackageName.Any<char>(IngestionValidationUtils.\u003C\u003EO.\u003C1\u003E__IsWhiteSpace ?? (IngestionValidationUtils.\u003C\u003EO.\u003C1\u003E__IsWhiteSpace = new Func<char, bool>(char.IsWhiteSpace))))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageNameMustNotContainWhitespace());
      Match match = !SpecialPackageNames.ReservedNames.Contains(rawPackageName) ? NpmPackageName.AllowedNamePattern.Match(rawPackageName) : throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageNameReserved());
      if (!match.Success)
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_InvalidPackageName((object) rawPackageName));
      string source = match.Groups["packageName"].Value;
      string empty = string.Empty;
      if (match.Groups["packageScope"] != null && match.Groups["packageScope"].Success)
        empty = match.Groups["packageScope"].Value;
      if (empty.Any<char>((Func<char, bool>) (c => !IngestionValidationUtils.IsAllowedCharacter(c))))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageNameMustBeAlphaNumericOrDashUnderscoreDot());
      if (source.Any<char>((Func<char, bool>) (c => !IngestionValidationUtils.IsAllowedCharacter(c))))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageNameMustBeAlphaNumericOrDashUnderscoreDot());
      if (!IngestionValidationUtils.IsAllowedStartingCharacter(source[0]))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageNameMustNotStartWithSpecialCharacter());
      return new NpmPackageName(rawPackageName);
    }

    public static SemanticVersion ParseAndValidatePackageVersion(string version)
    {
      SemanticVersion version1;
      if (!NpmVersionUtils.TryParseNpmPackageVersion(version, out version1))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_InvalidPackageVersion((object) version));
      IngestionValidationUtils.SortableVersionBuilder.GetSortableVersion(version1.NormalizedVersion);
      if (version1.NormalizedVersion.Length > (int) sbyte.MaxValue || version1.DisplayVersion.Length > (int) sbyte.MaxValue)
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_InvalidPackageVersionTooLong((object) version));
      return version1;
    }

    public static void ValidatePackageJsonAgainstRequest(
      PackageJson packageJson,
      string packageJsonParsedVersionString,
      VersionMetadata versionMetadata)
    {
      ArgumentUtility.CheckForNull<PackageJson>(packageJson, nameof (packageJson));
      ArgumentUtility.CheckStringForNullOrEmpty(packageJsonParsedVersionString, nameof (packageJsonParsedVersionString));
      ArgumentUtility.CheckForNull<VersionMetadata>(versionMetadata, nameof (versionMetadata));
      if (packageJson.Name != versionMetadata.Name)
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageJsonAndRequestMustMatch((object) "name"));
      if (packageJsonParsedVersionString != versionMetadata.Version)
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Resources.Error_PackageJsonAndRequestMustMatch((object) "version"));
    }

    private static bool IsAllowedCharacter(char c) => c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '-' || c == '_' || c == '.';

    private static bool IsAllowedStartingCharacter(char c)
    {
      if (c >= '0' && c <= '9' || c >= 'a' && c <= 'z')
        return true;
      return c >= 'A' && c <= 'Z';
    }
  }
}
