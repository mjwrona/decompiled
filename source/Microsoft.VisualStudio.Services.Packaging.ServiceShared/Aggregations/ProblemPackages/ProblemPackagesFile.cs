// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages.ProblemPackagesFile
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
  public class ProblemPackagesFile
  {
    public static 
    #nullable disable
    ProblemPackagesFile Empty { get; } = new ProblemPackagesFile((IEnumerable<KeyValuePair<IPackageName, ProblemPackagesFilePackage>>) ImmutableDictionary.Create<IPackageName, ProblemPackagesFilePackage>((IEqualityComparer<IPackageName>) PackageNameComparer.NormalizedName));

    public IImmutableDictionary<IPackageName, ProblemPackagesFilePackage> Packages { get; }

    public ProblemPackagesFile(
      IEnumerable<KeyValuePair<IPackageName, ProblemPackagesFilePackage>> packages)
    {
      this.Packages = (IImmutableDictionary<IPackageName, ProblemPackagesFilePackage>) packages.ToImmutableDictionary<IPackageName, ProblemPackagesFilePackage>((IEqualityComparer<IPackageName>) PackageNameComparer.NormalizedName);
    }

    public ProblemPackagesFile WithAddedOrUpdatedPackage(
      IPackageName packageName,
      ProblemPackagesFilePackage packageDetails)
    {
      return new ProblemPackagesFile((IEnumerable<KeyValuePair<IPackageName, ProblemPackagesFilePackage>>) this.Packages.SetItem(packageName, packageDetails));
    }

    internal ProblemPackagesFile.Stored Pack() => new ProblemPackagesFile.Stored()
    {
      Packages = (IDictionary<string, ProblemPackagesFilePackage.Stored>) this.Packages.ToDictionary<KeyValuePair<IPackageName, ProblemPackagesFilePackage>, string, ProblemPackagesFilePackage.Stored>((Func<KeyValuePair<IPackageName, ProblemPackagesFilePackage>, string>) (x => x.Key.DisplayName), (Func<KeyValuePair<IPackageName, ProblemPackagesFilePackage>, ProblemPackagesFilePackage.Stored>) (x => x.Value.Pack()))
    };

    internal class Stored
    {
      public IDictionary<string, ProblemPackagesFilePackage.Stored> Packages { get; set; }

      internal ProblemPackagesFile Unpack(
        Func<string, IPackageName> nameParser,
        Func<string, IPackageVersion> versionParser)
      {
        return new ProblemPackagesFile((IEnumerable<KeyValuePair<IPackageName, ProblemPackagesFilePackage>>) this.Packages.ToImmutableDictionary<KeyValuePair<string, ProblemPackagesFilePackage.Stored>, IPackageName, ProblemPackagesFilePackage>((Func<KeyValuePair<string, ProblemPackagesFilePackage.Stored>, IPackageName>) (x => nameParser(x.Key)), (Func<KeyValuePair<string, ProblemPackagesFilePackage.Stored>, ProblemPackagesFilePackage>) (x => x.Value.Unpack(versionParser)), (IEqualityComparer<IPackageName>) PackageNameComparer.NormalizedName));
      }
    }
  }
}
