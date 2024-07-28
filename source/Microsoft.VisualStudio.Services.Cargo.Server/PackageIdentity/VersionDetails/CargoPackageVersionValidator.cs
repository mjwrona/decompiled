// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails.CargoPackageVersionValidator
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails
{
  public static class CargoPackageVersionValidator
  {
    public const ulong MaxNumericSegmentValueForMajorMinorPatch = 18446744073709551615;
    public const ulong MaxNumericSegmentValueForPrereleaseAndBuildMetadata = 281474976710655;
    public const int MaxLeadingZeroes = 15;
    private static readonly Regex ProhibitedChars = new Regex("[^-0-9A-Za-z]", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(500.0));

    public static void Validate(CargoPackageVersion version, bool strict)
    {
      IVersionLabelSegment versionLabelSegment1 = version.MajorMinorPatchLabel.Segments.FirstOrDefault<IVersionLabelSegment>((Func<IVersionLabelSegment, bool>) (x => !(x is NumericVersionLabelSegment)));
      if (version.MajorMinorPatchLabel.Segments.Count != 3 || versionLabelSegment1 != null)
      {
        IVersionLabelSegment versionLabelSegment2 = versionLabelSegment1 ?? version.MajorMinorPatchLabel.Segments.Last<IVersionLabelSegment>();
        throw CargoPackageVersionValidator.GetInvalidVersionException(version.DisplayVersion, versionLabelSegment2.StartPos, Resources.Error_CrateVersionHasWrongNumberOrTypeOfMajorMinorPatchSegments());
      }
      CargoPackageVersionValidator.ValidateLabel<CargoMajorMinorPatchLabel>(version.DisplayVersion, (CargoVersionLabel<CargoMajorMinorPatchLabel>) version.MajorMinorPatchLabel, false, ulong.MaxValue);
      if (version.PrereleaseLabel != null)
        CargoPackageVersionValidator.ValidateLabel<CargoPrereleaseLabel>(version.DisplayVersion, (CargoVersionLabel<CargoPrereleaseLabel>) version.PrereleaseLabel, !strict, 281474976710655UL);
      if (version.BuildMetadataLabel != null)
        CargoPackageVersionValidator.ValidateLabel<CargoBuildMetadataLabel>(version.DisplayVersion, (CargoVersionLabel<CargoBuildMetadataLabel>) version.BuildMetadataLabel, true, 281474976710655UL);
      if (version.NormalizedVersion.Length > (int) sbyte.MaxValue)
        throw CargoPackageVersionValidator.GetInvalidVersionException(version.DisplayVersion, (int) sbyte.MaxValue, Resources.Error_CrateVersionHasTooManyChars((object) (int) sbyte.MaxValue));
    }

    private static string PickNameForLabelType<T>(CargoVersionLabel<T> label) where T : CargoVersionLabel<T>
    {
      switch (label)
      {
        case CargoMajorMinorPatchLabel _:
          return Resources.LabelKind_MajorMinorPatch();
        case CargoBuildMetadataLabel _:
          return Resources.LabelKind_BuildMetadata();
        case CargoPrereleaseLabel _:
          return Resources.LabelKind_Prerelease();
        default:
          throw new ArgumentOutOfRangeException(nameof (label));
      }
    }

    private static void ValidateLabel<T>(
      string fullVersionString,
      CargoVersionLabel<T> label,
      bool allowLeadingZeroes,
      ulong maxNumericSegmentValue)
      where T : CargoVersionLabel<T>
    {
      foreach (IVersionLabelSegment segment in (IEnumerable<IVersionLabelSegment>) label.Segments)
      {
        switch (segment)
        {
          case NumericVersionLabelSegment versionLabelSegment1:
            if (!allowLeadingZeroes && versionLabelSegment1.LeadingZeroes > 0)
              throw CargoPackageVersionValidator.GetInvalidVersionException(fullVersionString, segment.StartPos, Resources.Error_CrateVersionHasLeadingZeroes((object) CargoPackageVersionValidator.PickNameForLabelType<T>(label)));
            if (versionLabelSegment1.Value > maxNumericSegmentValue)
              throw CargoPackageVersionValidator.GetInvalidVersionException(fullVersionString, segment.StartPos, Resources.Error_CrateVersionHasTooBigNumber((object) maxNumericSegmentValue, (object) CargoPackageVersionValidator.PickNameForLabelType<T>(label)));
            if (versionLabelSegment1.LeadingZeroes > 15)
              throw CargoPackageVersionValidator.GetInvalidVersionException(fullVersionString, segment.StartPos, Resources.Error_CrateVersionHasTooManyLeadingZeroes((object) 15, (object) CargoPackageVersionValidator.PickNameForLabelType<T>(label)));
            continue;
          case StringVersionLabelSegment versionLabelSegment2:
            string stringValue = versionLabelSegment2.StringValue;
            Match match = !string.IsNullOrWhiteSpace(stringValue) ? CargoPackageVersionValidator.ProhibitedChars.Match(stringValue) : throw CargoPackageVersionValidator.GetInvalidVersionException(fullVersionString, segment.StartPos, Resources.Error_CrateVersionHasEmptySegment());
            if (match.Success)
              throw CargoPackageVersionValidator.GetInvalidVersionException(fullVersionString, segment.StartPos, match.Value == "+" ? Resources.Error_CrateVersionHasTooManyPluses() : Resources.Error_CrateVersionHasProhibitedCharacters());
            continue;
          default:
            throw new ArgumentOutOfRangeException("segment");
        }
      }
    }

    private static InvalidVersionException GetInvalidVersionException(
      string fullVersionString,
      int pos,
      string specificMessage)
    {
      return new InvalidVersionException(Resources.Error_InvalidCrateVersionAtPosition((object) fullVersionString, (object) pos, (object) specificMessage));
    }
  }
}
