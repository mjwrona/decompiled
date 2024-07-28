// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails.CargoPackageVersionFormatter
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using System;
using System.Collections.Immutable;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails
{
  internal class CargoPackageVersionFormatter
  {
    public static string FormatNormalized(
      CargoMajorMinorPatchLabel majorMinorPatchLabel,
      CargoPrereleaseLabel? prereleaseLabel,
      CargoBuildMetadataLabel? buildMetadataLabel)
    {
      string str = CargoPackageVersionFormatter.FormatLabelNormalized(majorMinorPatchLabel.Segments);
      if (prereleaseLabel != null)
        str = str + "-" + CargoPackageVersionFormatter.FormatLabelNormalized(prereleaseLabel.Segments);
      if (buildMetadataLabel != null)
        str = str + "+" + CargoPackageVersionFormatter.FormatLabelNormalized(buildMetadataLabel.Segments);
      return str;
    }

    public static string FormatLabelNormalized(IImmutableList<IVersionLabelSegment> labelSegments) => string.Join(".", labelSegments.Select<IVersionLabelSegment, string>((Func<IVersionLabelSegment, string>) (x => x.StringValue.ToLowerInvariant())));
  }
}
