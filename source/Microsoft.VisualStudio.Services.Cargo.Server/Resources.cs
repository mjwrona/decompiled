// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Cargo.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Cargo.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Cargo.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Cargo.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Cargo.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Cargo.Server.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Cargo.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Cargo.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Cargo.Server.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Cargo.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string CrateIdLocation_CargoToml() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (CrateIdLocation_CargoToml));

    public static string CrateIdLocation_CargoToml(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (CrateIdLocation_CargoToml), culture);

    public static string CrateIdLocation_PublishManifest() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (CrateIdLocation_PublishManifest));

    public static string CrateIdLocation_PublishManifest(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (CrateIdLocation_PublishManifest), culture);

    public static string CrateIdLocation_UpstreamIndexRow() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (CrateIdLocation_UpstreamIndexRow));

    public static string CrateIdLocation_UpstreamIndexRow(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (CrateIdLocation_UpstreamIndexRow), culture);

    public static string Error_CargoTomlInvalidToml(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CargoTomlInvalidToml), arg0);

    public static string Error_CargoTomlInvalidToml(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CargoTomlInvalidToml), culture, arg0);

    public static string Error_CargoTomlMissingElement(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CargoTomlMissingElement), arg0);

    public static string Error_CargoTomlMissingElement(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CargoTomlMissingElement), culture, arg0);

    public static string Error_CargoTomlWrongTypeElement(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CargoTomlWrongTypeElement), arg0);

    public static string Error_CargoTomlWrongTypeElement(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CargoTomlWrongTypeElement), culture, arg0);

    public static string Error_CrateMissingRequiredFile(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateMissingRequiredFile), arg0);

    public static string Error_CrateMissingRequiredFile(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateMissingRequiredFile), culture, arg0);

    public static string Error_CrateNameHasInvalidCharacter() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateNameHasInvalidCharacter));

    public static string Error_CrateNameHasInvalidCharacter(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateNameHasInvalidCharacter), culture);

    public static string Error_CrateNameHasNonAsciiCharacter() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateNameHasNonAsciiCharacter));

    public static string Error_CrateNameHasNonAsciiCharacter(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateNameHasNonAsciiCharacter), culture);

    public static string Error_CrateNameHasNonLetterFirstCharacter() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateNameHasNonLetterFirstCharacter));

    public static string Error_CrateNameHasNonLetterFirstCharacter(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateNameHasNonLetterFirstCharacter), culture);

    public static string Error_CrateNameHasReservedName() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateNameHasReservedName));

    public static string Error_CrateNameHasReservedName(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateNameHasReservedName), culture);

    public static string Error_CrateNameHasTooManyChars(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateNameHasTooManyChars), arg0);

    public static string Error_CrateNameHasTooManyChars(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateNameHasTooManyChars), culture, arg0);

    public static string Error_CrateVersionHasEmptySegment() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasEmptySegment));

    public static string Error_CrateVersionHasEmptySegment(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasEmptySegment), culture);

    public static string Error_CrateVersionHasLeadingZeroes(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionHasLeadingZeroes), arg0);

    public static string Error_CrateVersionHasLeadingZeroes(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionHasLeadingZeroes), culture, arg0);

    public static string Error_CrateVersionHasProhibitedCharacters() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasProhibitedCharacters));

    public static string Error_CrateVersionHasProhibitedCharacters(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasProhibitedCharacters), culture);

    public static string Error_CrateVersionHasTooBigNumber(object arg0, object arg1) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionHasTooBigNumber), arg0, arg1);

    public static string Error_CrateVersionHasTooBigNumber(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionHasTooBigNumber), culture, arg0, arg1);
    }

    public static string Error_CrateVersionHasTooManyChars(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionHasTooManyChars), arg0);

    public static string Error_CrateVersionHasTooManyChars(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionHasTooManyChars), culture, arg0);

    public static string Error_CrateVersionHasTooManyLeadingZeroes(object arg0, object arg1) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionHasTooManyLeadingZeroes), arg0, arg1);

    public static string Error_CrateVersionHasTooManyLeadingZeroes(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionHasTooManyLeadingZeroes), culture, arg0, arg1);
    }

    public static string Error_CrateVersionHasTooManyPluses() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasTooManyPluses));

    public static string Error_CrateVersionHasTooManyPluses(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasTooManyPluses), culture);

    public static string Error_CrateVersionHasUnparseableNumber() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasUnparseableNumber));

    public static string Error_CrateVersionHasUnparseableNumber(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasUnparseableNumber), culture);

    public static string Error_CrateVersionHasWrongNumberOrTypeOfMajorMinorPatchSegments() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasWrongNumberOrTypeOfMajorMinorPatchSegments));

    public static string Error_CrateVersionHasWrongNumberOrTypeOfMajorMinorPatchSegments(
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_CrateVersionHasWrongNumberOrTypeOfMajorMinorPatchSegments), culture);
    }

    public static string Error_CrateVersionUnparseable(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionUnparseable), arg0);

    public static string Error_CrateVersionUnparseable(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_CrateVersionUnparseable), culture, arg0);

    public static string Error_IndexRowMissingIdentity() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_IndexRowMissingIdentity));

    public static string Error_IndexRowMissingIdentity(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_IndexRowMissingIdentity), culture);

    public static string Error_IngestionManifestNoName() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_IngestionManifestNoName));

    public static string Error_IngestionManifestNoName(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_IngestionManifestNoName), culture);

    public static string Error_IngestionManifestNoVersion() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_IngestionManifestNoVersion));

    public static string Error_IngestionManifestNoVersion(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_IngestionManifestNoVersion), culture);

    public static string Error_IngestionManifestNull() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_IngestionManifestNull));

    public static string Error_IngestionManifestNull(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_IngestionManifestNull), culture);

    public static string Error_InvalidCrateNameAtPosition(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_InvalidCrateNameAtPosition), arg0, arg1, arg2);

    public static string Error_InvalidCrateNameAtPosition(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_InvalidCrateNameAtPosition), culture, arg0, arg1, arg2);
    }

    public static string Error_InvalidCrateVersionAtPosition(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_InvalidCrateVersionAtPosition), arg0, arg1, arg2);

    public static string Error_InvalidCrateVersionAtPosition(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_InvalidCrateVersionAtPosition), culture, arg0, arg1, arg2);
    }

    public static string Error_ManifestLengthExceeded(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_ManifestLengthExceeded), arg0);

    public static string Error_ManifestLengthExceeded(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_ManifestLengthExceeded), culture, arg0);

    public static string Error_MismatchedCrateName(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_MismatchedCrateName), arg0, arg1, arg2);

    public static string Error_MismatchedCrateName(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_MismatchedCrateName), culture, arg0, arg1, arg2);
    }

    public static string Error_MismatchedCrateVersion(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_MismatchedCrateVersion), arg0, arg1, arg2);

    public static string Error_MismatchedCrateVersion(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_MismatchedCrateVersion), culture, arg0, arg1, arg2);
    }

    public static string Error_PackageDoesNotMatchHash() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_PackageDoesNotMatchHash));

    public static string Error_PackageDoesNotMatchHash(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_PackageDoesNotMatchHash), culture);

    public static string Error_PackageVersionNotFound(object arg0, object arg1) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_PackageVersionNotFound), arg0, arg1);

    public static string Error_PackageVersionNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_PackageVersionNotFound), culture, arg0, arg1);
    }

    public static string Error_UpstreamFailure(object arg0, object arg1) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_UpstreamFailure), arg0, arg1);

    public static string Error_UpstreamFailure(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_UpstreamFailure), culture, arg0, arg1);

    public static string Error_UpstreamIngestion_CannotSkipIngestion() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion));

    public static string Error_UpstreamIngestion_CannotSkipIngestion(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (Error_UpstreamIngestion_CannotSkipIngestion), culture);

    public static string Error_UpstreamReturnedNotFound(object arg0) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_UpstreamReturnedNotFound), arg0);

    public static string Error_UpstreamReturnedNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Format(nameof (Error_UpstreamReturnedNotFound), culture, arg0);

    public static string LabelKind_BuildMetadata() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (LabelKind_BuildMetadata));

    public static string LabelKind_BuildMetadata(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (LabelKind_BuildMetadata), culture);

    public static string LabelKind_MajorMinorPatch() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (LabelKind_MajorMinorPatch));

    public static string LabelKind_MajorMinorPatch(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (LabelKind_MajorMinorPatch), culture);

    public static string LabelKind_Prerelease() => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (LabelKind_Prerelease));

    public static string LabelKind_Prerelease(CultureInfo culture) => Microsoft.VisualStudio.Services.Cargo.Server.Resources.Get(nameof (LabelKind_Prerelease), culture);
  }
}
