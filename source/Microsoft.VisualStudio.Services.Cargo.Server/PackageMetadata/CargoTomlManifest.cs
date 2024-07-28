// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.CargoTomlManifest
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Tomlyn;
using Tomlyn.Model;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public record CargoTomlManifest(
    CargoTomlManifestPackageOrProject Package,
    CargoTomlDependencyGroup Dependencies,
    CargoTomlDependencyGroup DevDependencies,
    CargoTomlDependencyGroup BuildDependencies,
    CargoTomlTargetGroup Target,
    ImmutableDictionary<string, ImmutableArray<string>> Features) : IHaveCargoTomlDependencies
  {
    public static LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest> LazyDeserialize(
      DeflateCompressibleBytes bytes)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest>(bytes, CargoTomlManifest.\u003C\u003EO.\u003C0\u003E__Deserialize ?? (CargoTomlManifest.\u003C\u003EO.\u003C0\u003E__Deserialize = new Func<DeflateCompressibleBytes, CargoTomlManifest>(CargoTomlManifest.Deserialize)));
    }

    private static CargoTomlManifest Deserialize(DeflateCompressibleBytes bytes)
    {
      TomlTable model;
      try
      {
        model = Toml.ToModel(Encoding.UTF8.GetString(bytes.AsUncompressedBytes()), "Cargo.toml");
      }
      catch (TomlException ex)
      {
        throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_CargoTomlInvalidToml((object) ex.Message), (Exception) ex);
      }
      return CargoTomlManifest.FromToml(model);
    }

    public static CargoTomlManifest FromToml(TomlTable rootTable)
    {
      CargoTomlManifestPackageOrProject Package = ExtractPackageTableOrDefault("package");
      if ((object) Package == null)
        Package = ExtractPackageTableOrDefault("project") ?? throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_CargoTomlMissingElement((object) "package"));
      IHaveCargoTomlDependencies allFromParentToml = CargoTomlDependencyGroup.ExtractAllFromParentToml(rootTable, ImmutableList<string>.Empty);
      CargoTomlTargetGroup Target = CargoTomlTargetGroup.FromTomlOrEmpty(rootTable.GetCastedValueThrowIfWrongTypeNullIfMissing<TomlTable>(ImmutableList<string>.Empty, "target"), ImmutableList.Create<string>("target"));
      ImmutableDictionary<string, ImmutableArray<string>> featuresOrEmpty = ExtractFeaturesOrEmpty(rootTable.GetCastedValueThrowIfWrongTypeNullIfMissing<TomlTable>(ImmutableList<string>.Empty, "features"));
      return new CargoTomlManifest(Package, allFromParentToml.Dependencies, allFromParentToml.DevDependencies, allFromParentToml.BuildDependencies, Target, featuresOrEmpty);

      CargoTomlManifestPackageOrProject? ExtractPackageTableOrDefault(string key)
      {
        TomlTable typeNullIfMissing = rootTable.GetCastedValueThrowIfWrongTypeNullIfMissing<TomlTable>(ImmutableList<string>.Empty, key);
        return typeNullIfMissing == null ? (CargoTomlManifestPackageOrProject) null : CargoTomlManifestPackageOrProject.FromToml(typeNullIfMissing, ImmutableList.Create<string>(key));
      }

      static ImmutableDictionary<string, ImmutableArray<string>> ExtractFeaturesOrEmpty(
        TomlTable? tomlTable)
      {
        return tomlTable == null ? ImmutableDictionary<string, ImmutableArray<string>>.Empty : tomlTable.Select(x => new
        {
          Feature = x.Key,
          Values = !(x.Value is TomlArray source) ? new ImmutableArray<string>() : source.OfType<string>().ToImmutableArray<string>()
        }).Where(x => !x.Values.IsDefault).ToImmutableDictionary(x => x.Feature, x => x.Values);
      }
    }
  }
}
