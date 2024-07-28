// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails.CargoSortableVersionBuilder
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails
{
  public class CargoSortableVersionBuilder
  {
    private readonly IConverter<ulong, string> majorMinorPatchNumberConverter;
    private readonly IConverter<ulong, string> preAndBuildNumberConverter;
    public const string PrereleasePrefix = "-";
    public const string NoPrerelease = "~";
    public const string BuildPrefix = "+";
    public const string NoBuild = "&";
    public const string NumericSegmentPrefix = "#";
    public const string StringSegmentPrefix = "$";

    public static CargoSortableVersionBuilder Instance { get; } = new CargoSortableVersionBuilder((IConverter<ulong, string>) new VersionUInt64ToPaddedStringConverter((IConverter<ArraySegment<byte>, string>) new LexicalBase64Converter(), 8), (IConverter<ulong, string>) new VersionUInt64ToPaddedStringConverter((IConverter<ArraySegment<byte>, string>) new LexicalBase64Converter(), 6));

    public CargoSortableVersionBuilder(
      IConverter<ulong, string> majorMinorPatchNumberConverter,
      IConverter<ulong, string> preAndBuildNumberConverter)
    {
      this.majorMinorPatchNumberConverter = majorMinorPatchNumberConverter;
      this.preAndBuildNumberConverter = preAndBuildNumberConverter;
    }

    public string GenerateSortableVersion(CargoPackageVersion version) => this.FormatLabel<CargoMajorMinorPatchLabel>((CargoVersionLabel<CargoMajorMinorPatchLabel>) version.MajorMinorPatchLabel, this.majorMinorPatchNumberConverter, false, false) + "!" + (version.PrereleaseLabel != null ? "-" + this.FormatLabel<CargoPrereleaseLabel>((CargoVersionLabel<CargoPrereleaseLabel>) version.PrereleaseLabel, this.preAndBuildNumberConverter, true, true) : "~") + "!" + (version.BuildMetadataLabel != null ? "+" + this.FormatLabel<CargoBuildMetadataLabel>((CargoVersionLabel<CargoBuildMetadataLabel>) version.BuildMetadataLabel, this.preAndBuildNumberConverter, true, true) : "&");

    private string FormatLabel<T>(
      CargoVersionLabel<T> label,
      IConverter<ulong, string> numberConverter,
      bool leadingZeroes,
      bool needSegmentTypePrefixes)
      where T : CargoVersionLabel<T>
    {
      return string.Concat(label.Segments.Select<IVersionLabelSegment, string>((Func<IVersionLabelSegment, string>) (x => this.FormatSegment(x, leadingZeroes, numberConverter, needSegmentTypePrefixes))));
    }

    private string FormatSegment(
      IVersionLabelSegment segment,
      bool leadingZeroes,
      IConverter<ulong, string> numberConverter,
      bool needSegmentTypePrefixes)
    {
      switch (segment)
      {
        case NumericVersionLabelSegment versionLabelSegment1:
          string str = (needSegmentTypePrefixes ? "#" : string.Empty) + numberConverter.Convert(versionLabelSegment1.Value);
          if (leadingZeroes)
            str += this.FormatSingleHexDigit(versionLabelSegment1.LeadingZeroes);
          return str;
        case StringVersionLabelSegment versionLabelSegment2:
          return (needSegmentTypePrefixes ? "$" : string.Empty) + versionLabelSegment2.StringValue.ToLowerInvariant();
        default:
          throw new ArgumentOutOfRangeException(nameof (segment), (object) segment, (string) null);
      }
    }

    private string FormatSingleHexDigit(int value)
    {
      char ch;
      if (value < 10)
      {
        if (value >= 0)
        {
          ch = (char) (48 + value);
          goto label_6;
        }
      }
      else if (value <= 15)
      {
        ch = (char) (65 + value - 10);
        goto label_6;
      }
      throw new ArgumentOutOfRangeException(nameof (value), CommonResources.ValueOutOfRange((object) value, (object) nameof (value), (object) 0, (object) 15));
label_6:
      return ch.ToString();
    }
  }
}
