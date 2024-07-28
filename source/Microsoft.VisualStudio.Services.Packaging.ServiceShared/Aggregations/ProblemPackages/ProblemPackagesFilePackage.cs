// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages.ProblemPackagesFilePackage
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages
{
  public class ProblemPackagesFilePackage
  {
    public static 
    #nullable disable
    ProblemPackagesFilePackage Empty { get; } = new ProblemPackagesFilePackage((IEnumerable<KeyValuePair<IPackageVersion, ProblemPackagesFilePackageVersion>>) ImmutableDictionary.Create<IPackageVersion, ProblemPackagesFilePackageVersion>((IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion));

    public IImmutableDictionary<IPackageVersion, ProblemPackagesFilePackageVersion> Versions { get; }

    public ProblemPackagesFilePackage(
      IEnumerable<KeyValuePair<IPackageVersion, ProblemPackagesFilePackageVersion>> versions)
    {
      this.Versions = (IImmutableDictionary<IPackageVersion, ProblemPackagesFilePackageVersion>) versions.ToImmutableDictionary<IPackageVersion, ProblemPackagesFilePackageVersion>((IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
    }

    public ProblemPackagesFilePackage WithAddedOrUpdatedVersion(
      IPackageVersion packageVersion,
      ProblemPackagesFilePackageVersion versionDetails)
    {
      return new ProblemPackagesFilePackage((IEnumerable<KeyValuePair<IPackageVersion, ProblemPackagesFilePackageVersion>>) this.Versions.SetItem(packageVersion, versionDetails));
    }

    internal ProblemPackagesFilePackage.Stored Pack() => new ProblemPackagesFilePackage.Stored()
    {
      Versions = (IDictionary<string, ProblemPackagesFilePackageVersion.Stored>) this.Versions.ToDictionary<KeyValuePair<IPackageVersion, ProblemPackagesFilePackageVersion>, string, ProblemPackagesFilePackageVersion.Stored>((Func<KeyValuePair<IPackageVersion, ProblemPackagesFilePackageVersion>, string>) (x => x.Key.DisplayVersion), (Func<KeyValuePair<IPackageVersion, ProblemPackagesFilePackageVersion>, ProblemPackagesFilePackageVersion.Stored>) (x => x.Value.Pack()))
    };

    internal class Stored
    {
      public IDictionary<string, ProblemPackagesFilePackageVersion.Stored> Versions { get; set; }

      internal ProblemPackagesFilePackage Unpack(Func<string, IPackageVersion> versionParser) => new ProblemPackagesFilePackage((IEnumerable<KeyValuePair<IPackageVersion, ProblemPackagesFilePackageVersion>>) this.Versions.ToImmutableDictionary<KeyValuePair<string, ProblemPackagesFilePackageVersion.Stored>, IPackageVersion, ProblemPackagesFilePackageVersion>((Func<KeyValuePair<string, ProblemPackagesFilePackageVersion.Stored>, IPackageVersion>) (x => versionParser(x.Key)), (Func<KeyValuePair<string, ProblemPackagesFilePackageVersion.Stored>, ProblemPackagesFilePackageVersion>) (x => x.Value.Unpack()), (IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion));
    }
  }
}
