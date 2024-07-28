// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails.CargoPackageVersionParser
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails
{
  public class CargoPackageVersionParser
  {
    private static readonly Regex VersionSplitterRegex = new Regex("^\r\n        (?<MajorMinorPatch>[^-+]*)\r\n        (?:-(?<Prerelease>[^+]*))?\r\n        (?:\\+(?<BuildMeta>.*))?\r\n        $", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(500.0));

    public static CargoPackageVersion Parse(string displayVersion)
    {
      CargoPackageVersion withoutValidating = CargoPackageVersionParser.ParseWithoutValidating(displayVersion);
      CargoPackageVersionValidator.Validate(withoutValidating, false);
      return withoutValidating;
    }

    public static CargoPackageVersion ParseWithoutValidating(string displayVersion)
    {
      Match match = CargoPackageVersionParser.VersionSplitterRegex.Match(displayVersion);
      if (!match.Success)
        throw new InvalidVersionException(Resources.Error_CrateVersionUnparseable((object) displayVersion));
      CargoMajorMinorPatchLabel majorMinorPatchLabel = new CargoMajorMinorPatchLabel(CargoPackageVersionParser.ParseLabelSegments(displayVersion, match.Groups["MajorMinorPatch"].Value, 0));
      Group group1 = match.Groups["Prerelease"];
      CargoPrereleaseLabel prereleaseLabel = group1.Success ? new CargoPrereleaseLabel(CargoPackageVersionParser.ParseLabelSegments(displayVersion, group1.Value, group1.Index)) : (CargoPrereleaseLabel) null;
      Group group2 = match.Groups["BuildMeta"];
      CargoBuildMetadataLabel buildMetadataLabel = group2.Success ? new CargoBuildMetadataLabel(CargoPackageVersionParser.ParseLabelSegments(displayVersion, group2.Value, group2.Index)) : (CargoBuildMetadataLabel) null;
      string normalizedVersion = CargoPackageVersionFormatter.FormatNormalized(majorMinorPatchLabel, prereleaseLabel, buildMetadataLabel);
      return new CargoPackageVersion(displayVersion, normalizedVersion, majorMinorPatchLabel, prereleaseLabel, buildMetadataLabel);
    }

    private static IEnumerable<IVersionLabelSegment> ParseLabelSegments(
      string fullVersionString,
      string label,
      int startPos)
    {
      string[] strArray = label.Split('.');
      for (int index = 0; index < strArray.Length; ++index)
      {
        string segment = strArray[index];
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        if (segment.Length > 0 && segment.All<char>(CargoPackageVersionParser.\u003C\u003EO.\u003C0\u003E__IsDigit ?? (CargoPackageVersionParser.\u003C\u003EO.\u003C0\u003E__IsDigit = new Func<char, bool>(char.IsDigit))))
        {
          int leadingZeroes = segment.TakeWhile<char>((Func<char, bool>) (x => x == '0')).Count<char>();
          ulong result;
          if (!ulong.TryParse(segment, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            throw new InvalidVersionException(Resources.Error_InvalidCrateVersionAtPosition((object) fullVersionString, (object) startPos, (object) Resources.Error_CrateVersionHasUnparseableNumber()));
          ulong num = result;
          if (num == 0UL)
            --leadingZeroes;
          yield return (IVersionLabelSegment) new NumericVersionLabelSegment(num, leadingZeroes, startPos);
        }
        else
          yield return (IVersionLabelSegment) new StringVersionLabelSegment(segment, startPos);
        startPos += segment.Length + 1;
        segment = (string) null;
      }
      strArray = (string[]) null;
    }
  }
}
