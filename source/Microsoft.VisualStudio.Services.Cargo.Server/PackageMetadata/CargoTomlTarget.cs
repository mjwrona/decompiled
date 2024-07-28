// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.CargoTomlTarget
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using System.Collections.Immutable;
using Tomlyn.Model;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public record CargoTomlTarget(
    CargoTomlDependencyGroup Dependencies,
    CargoTomlDependencyGroup DevDependencies,
    CargoTomlDependencyGroup BuildDependencies) : IHaveCargoTomlDependencies
  {
    public static CargoTomlTarget FromToml(TomlTable tomlTable, ImmutableList<string> tomlTablePath)
    {
      IHaveCargoTomlDependencies allFromParentToml = CargoTomlDependencyGroup.ExtractAllFromParentToml(tomlTable, tomlTablePath);
      return new CargoTomlTarget(allFromParentToml.Dependencies, allFromParentToml.DevDependencies, allFromParentToml.BuildDependencies);
    }
  }
}
