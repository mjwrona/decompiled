// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.CargoTomlDependencyGroup
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Tomlyn.Model;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public record CargoTomlDependencyGroup(
    ImmutableDictionary<string, CargoTomlDependency> Entries)
  {
    public static CargoTomlDependencyGroup FromTomlOrEmpty(
      TomlTable? tomlTable,
      ImmutableList<string> path)
    {
      return tomlTable == null ? CargoTomlDependencyGroup.Empty : new CargoTomlDependencyGroup(tomlTable.ToImmutableDictionary<KeyValuePair<string, object>, string, CargoTomlDependency>((Func<KeyValuePair<string, object>, string>) (x => x.Key), (Func<KeyValuePair<string, object>, CargoTomlDependency>) (x =>
      {
        switch (x.Value)
        {
          case string version2:
            return CargoTomlDependency.FromBareVersion(version2);
          case TomlTable tomlTable2:
            return CargoTomlDependency.FromToml(tomlTable2);
          default:
            throw new InvalidPackageException(Resources.Error_CargoTomlWrongTypeElement((object) TomlHelpers.GetKeyString((IEnumerable<string>) path.Add(x.Key))));
        }
      })));
    }

    public static IHaveCargoTomlDependencies ExtractAllFromParentToml(
      TomlTable parentTable,
      ImmutableList<string> parentTablePath)
    {
      return (IHaveCargoTomlDependencies) new CargoTomlDependencyGroup.AllDependencies(ExtractFromParentTomlOrEmpty("dependencies"), ExtractFromParentTomlOrEmpty("dev-dependencies"), ExtractFromParentTomlOrEmpty("build-dependencies"));

      CargoTomlDependencyGroup ExtractFromParentTomlOrEmpty(string key) => CargoTomlDependencyGroup.FromTomlOrEmpty(parentTable.GetCastedValueThrowIfWrongTypeNullIfMissing<TomlTable>(parentTablePath, key), parentTablePath.Add(key));
    }

    public static CargoTomlDependencyGroup Empty { get; } = new CargoTomlDependencyGroup(ImmutableDictionary<string, CargoTomlDependency>.Empty);

    private record AllDependencies(
      CargoTomlDependencyGroup Dependencies,
      CargoTomlDependencyGroup DevDependencies,
      CargoTomlDependencyGroup BuildDependencies) : IHaveCargoTomlDependencies
    ;
  }
}
