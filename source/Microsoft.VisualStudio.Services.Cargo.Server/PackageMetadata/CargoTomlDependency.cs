// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.CargoTomlDependency
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Immutable;
using Tomlyn.Model;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public record CargoTomlDependency(
    string? Version,
    string? RegistryIndex,
    ImmutableArray<string> Features,
    bool? Optional,
    bool? DefaultFeatures,
    string? Package)
  {
    public static CargoTomlDependency FromBareVersion(string version) => new CargoTomlDependency(version, (string) null, ImmutableArray<string>.Empty, new bool?(), new bool?(), (string) null);

    public static CargoTomlDependency FromToml(TomlTable tomlTable)
    {
      string castedValueOrDefault1 = tomlTable.GetCastedValueOrDefault<string, string>("version");
      string castedValueOrDefault2 = tomlTable.GetCastedValueOrDefault<string, string>("registry-index");
      ImmutableArray<string>? stringArrayOrDefault = TomlHelpers.ExtractStringArrayOrDefault(tomlTable, "features");
      bool? castedValueOrDefault3 = tomlTable.GetCastedValueOrDefault<string, bool?>("optional");
      bool? castedValueOrDefault4 = tomlTable.GetCastedValueOrDefault<string, bool?>("default-features");
      bool? castedValueOrDefault5 = tomlTable.GetCastedValueOrDefault<string, bool?>("default_features");
      string castedValueOrDefault6 = tomlTable.GetCastedValueOrDefault<string, string>("package");
      string RegistryIndex = castedValueOrDefault2;
      ImmutableArray<string> Features = stringArrayOrDefault ?? ImmutableArray<string>.Empty;
      bool? Optional = castedValueOrDefault3;
      bool? DefaultFeatures = castedValueOrDefault4 ?? castedValueOrDefault5;
      string Package = castedValueOrDefault6;
      return new CargoTomlDependency(castedValueOrDefault1, RegistryIndex, Features, Optional, DefaultFeatures, Package);
    }
  }
}
