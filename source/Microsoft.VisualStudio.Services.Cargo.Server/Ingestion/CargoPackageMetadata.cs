// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Ingestion.CargoPackageMetadata
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Ingestion
{
  public record CargoPackageMetadata(
    CargoPackageIdentity Identity,
    ImmutableArray<CargoPackageMetadataDependency> Dependencies,
    ImmutableDictionary<string, ImmutableArray<string>> Features,
    ImmutableArray<string> Authors,
    string? Description,
    string? DocumentationUrl,
    string? HomepageUrl,
    string? ReadmeText,
    string? ReadmeFilePath,
    ImmutableArray<string> Keywords,
    ImmutableArray<string> Categories,
    string? LicenseExpression,
    string? LicenseFilePath,
    string? RepositoryUrl,
    string? Links)
  {
    public static CargoPackageMetadata FromPublishManifest(CargoPublishManifest parsedJson)
    {
      CargoPackageIdentity Identity = parsedJson != null ? parsedJson.ExtractPackageIdentity() : throw new InvalidPackageException(Resources.Error_IngestionManifestNull());
      IEnumerable<CargoPublishManifestDependency> source = parsedJson.deps ?? Enumerable.Empty<CargoPublishManifestDependency>();
      ImmutableDictionary<string, ImmutableArray<string>> immutableDictionary = parsedJson.features != null ? parsedJson.features.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (x => x.Key != null)).ToImmutableDictionary<KeyValuePair<string, IEnumerable<string>>, string, ImmutableArray<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (x => x.Key), (Func<KeyValuePair<string, IEnumerable<string>>, ImmutableArray<string>>) (x => CargoPackageMetadata.ToNonNullImmutableWithNoContainedNulls<string>(x.Value))) : ImmutableDictionary<string, ImmutableArray<string>>.Empty;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ImmutableArray<CargoPackageMetadataDependency> immutableArray = source.Select<CargoPublishManifestDependency, CargoPackageMetadataDependency>(CargoPackageMetadata.\u003C\u003EO.\u003C0\u003E__TransformDependency ?? (CargoPackageMetadata.\u003C\u003EO.\u003C0\u003E__TransformDependency = new Func<CargoPublishManifestDependency, CargoPackageMetadataDependency>(CargoPackageMetadata.TransformDependency))).ToImmutableArray<CargoPackageMetadataDependency>();
      ImmutableDictionary<string, ImmutableArray<string>> Features = immutableDictionary;
      ImmutableArray<string> noContainedNulls1 = CargoPackageMetadata.ToNonNullImmutableWithNoContainedNulls<string>(parsedJson.authors);
      string description = parsedJson.description;
      string documentation = parsedJson.documentation;
      string homepage = parsedJson.homepage;
      string readme = parsedJson.readme;
      string readmeFile = parsedJson.readme_file;
      ImmutableArray<string> noContainedNulls2 = CargoPackageMetadata.ToNonNullImmutableWithNoContainedNulls<string>(parsedJson.keywords);
      ImmutableArray<string> noContainedNulls3 = CargoPackageMetadata.ToNonNullImmutableWithNoContainedNulls<string>(parsedJson.categories);
      string license = parsedJson.license;
      string licenseFile = parsedJson.license_file;
      string repository = parsedJson.repository;
      string links = parsedJson.links;
      return new CargoPackageMetadata(Identity, immutableArray, Features, noContainedNulls1, description, documentation, homepage, readme, readmeFile, noContainedNulls2, noContainedNulls3, license, licenseFile, repository, links);
    }

    public static CargoPackageMetadata FromCargoToml(CargoTomlManifest cargoToml)
    {
      CargoPackageIdentity Identity = CargoIdentityResolver.Instance.ResolvePackageIdentity(cargoToml.Package.Name, cargoToml.Package.Version);
      ImmutableArray<CargoPackageMetadataDependency> immutableArray = FromIHaveCargoTomlDependencies((IHaveCargoTomlDependencies) cargoToml, (string) null).Concat<CargoPackageMetadataDependency>(cargoToml.Target.Entries.SelectMany<KeyValuePair<string, CargoTomlTarget>, CargoPackageMetadataDependency>((Func<KeyValuePair<string, CargoTomlTarget>, IEnumerable<CargoPackageMetadataDependency>>) (x => FromIHaveCargoTomlDependencies((IHaveCargoTomlDependencies) x.Value, x.Key)))).ToImmutableArray<CargoPackageMetadataDependency>();
      ImmutableDictionary<string, ImmutableArray<string>> features = cargoToml.Features;
      ImmutableArray<string> authors = cargoToml.Package.Authors;
      string description = cargoToml.Package.Description;
      string documentation = cargoToml.Package.Documentation;
      string homepage = cargoToml.Package.Homepage;
      StringOrBool readme = cargoToml.Package.Readme;
      StringOrBool.Bool @bool = readme as StringOrBool.Bool;
      string str1;
      if ((object) @bool != null)
      {
        bool flag;
        @bool.Deconstruct(out flag);
        str1 = flag ? "README.md" : (string) null;
      }
      else
      {
        StringOrBool.String @string = readme as StringOrBool.String;
        if ((object) @string != null)
        {
          string str2;
          @string.Deconstruct(out str2);
          str1 = str2;
        }
        else
        {
          if ((object) readme != null)
            throw new InvalidOperationException();
          str1 = (string) null;
        }
      }
      ImmutableArray<CargoPackageMetadataDependency> Dependencies = immutableArray;
      ImmutableDictionary<string, ImmutableArray<string>> Features = features;
      ImmutableArray<string> Authors = authors;
      string Description = description;
      string DocumentationUrl = documentation;
      string HomepageUrl = homepage;
      string ReadmeFilePath = str1;
      ImmutableArray<string> keywords = cargoToml.Package.Keywords;
      ImmutableArray<string> categories = cargoToml.Package.Categories;
      string license = cargoToml.Package.License;
      string licenseFile = cargoToml.Package.LicenseFile;
      string repository = cargoToml.Package.Repository;
      string links = cargoToml.Package.Links;
      return new CargoPackageMetadata(Identity, Dependencies, Features, Authors, Description, DocumentationUrl, HomepageUrl, (string) null, ReadmeFilePath, keywords, categories, license, licenseFile, repository, links);

      static IEnumerable<CargoPackageMetadataDependency> FromIHaveCargoTomlDependencies(
        IHaveCargoTomlDependencies objWithDeps,
        string? targetPlatform)
      {
        return FromCargoTomlDependencyGroup(objWithDeps.Dependencies, targetPlatform, "normal").Concat<CargoPackageMetadataDependency>(FromCargoTomlDependencyGroup(objWithDeps.DevDependencies, targetPlatform, "dev")).Concat<CargoPackageMetadataDependency>(FromCargoTomlDependencyGroup(objWithDeps.BuildDependencies, targetPlatform, "build"));
      }

      static IEnumerable<CargoPackageMetadataDependency> FromCargoTomlDependencyGroup(
        CargoTomlDependencyGroup group,
        string? targetPlatform,
        string kind)
      {
        return group.Entries.Select<KeyValuePair<string, CargoTomlDependency>, CargoPackageMetadataDependency>((Func<KeyValuePair<string, CargoTomlDependency>, CargoPackageMetadataDependency>) (x => FromCargoTomlDependency(x.Key, x.Value, targetPlatform, kind)));
      }

      static CargoPackageMetadataDependency FromCargoTomlDependency(
        string declaredName,
        CargoTomlDependency dep,
        string? targetPlatform,
        string kind)
      {
        return new CargoPackageMetadataDependency(CargoIdentityResolver.Instance.ResolvePackageName(dep.Package ?? declaredName), dep.Version ?? "*", (IImmutableList<string>) dep.Features, dep.Optional.GetValueOrDefault(), ((int) dep.DefaultFeatures ?? 1) != 0, targetPlatform, kind, dep.RegistryIndex, dep.Package != null ? declaredName : (string) null);
      }
    }

    public static CargoPackageMetadata FromIndex(CargoIndexVersionRow indexRow)
    {
      CargoPackageIdentity packageIdentity = indexRow.ExtractPackageIdentity();
      IEnumerable<CargoIndexDependency> dependencies = indexRow.Dependencies;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ImmutableArray<CargoPackageMetadataDependency> immutableArray = dependencies != null ? dependencies.Select<CargoIndexDependency, CargoPackageMetadataDependency>(CargoPackageMetadata.\u003C\u003EO.\u003C1\u003E__TransformDependencyFromIndex ?? (CargoPackageMetadata.\u003C\u003EO.\u003C1\u003E__TransformDependencyFromIndex = new Func<CargoIndexDependency, CargoPackageMetadataDependency>(CargoPackageMetadata.TransformDependencyFromIndex))).ToImmutableArray<CargoPackageMetadataDependency>() : ImmutableArray<CargoPackageMetadataDependency>.Empty;
      ImmutableDictionary<string, ImmutableArray<string>> immutableDictionary = ((IEnumerable<KeyValuePair<string, IEnumerable<string>>>) indexRow.Features ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>()).Concat<KeyValuePair<string, IEnumerable<string>>>((IEnumerable<KeyValuePair<string, IEnumerable<string>>>) indexRow.Features2 ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>()).Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (x => x.Key != null)).GroupBy<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (x => x.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (x => x.Value)).ToImmutableDictionary<IGrouping<string, IEnumerable<string>>, string, ImmutableArray<string>>((Func<IGrouping<string, IEnumerable<string>>, string>) (x => x.Key), (Func<IGrouping<string, IEnumerable<string>>, ImmutableArray<string>>) (x => CargoPackageMetadata.ToNonNullImmutableWithNoContainedNulls<string>(x.SelectMany<IEnumerable<string>, string>((Func<IEnumerable<string>, IEnumerable<string>>) (y => y)).Distinct<string>())));
      ImmutableArray<CargoPackageMetadataDependency> Dependencies = immutableArray;
      ImmutableDictionary<string, ImmutableArray<string>> Features = immutableDictionary;
      ImmutableArray<string> empty1 = ImmutableArray<string>.Empty;
      ImmutableArray<string> empty2 = ImmutableArray<string>.Empty;
      ImmutableArray<string> empty3 = ImmutableArray<string>.Empty;
      string links = indexRow.Links;
      return new CargoPackageMetadata(packageIdentity, Dependencies, Features, empty1, (string) null, (string) null, (string) null, (string) null, (string) null, empty2, empty3, (string) null, (string) null, (string) null, links);
    }

    public CargoPublishManifest SynthesizePublishManifestFromIndexProperties()
    {
      ImmutableDictionary<string, IEnumerable<string>> immutableDictionary = this.Features.Where<KeyValuePair<string, ImmutableArray<string>>>((Func<KeyValuePair<string, ImmutableArray<string>>, bool>) (x => x.Key != null)).ToImmutableDictionary<KeyValuePair<string, ImmutableArray<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, ImmutableArray<string>>, string>) (x => x.Key), (Func<KeyValuePair<string, ImmutableArray<string>>, IEnumerable<string>>) (x => (IEnumerable<string>) CargoPackageMetadata.ToNonNullImmutableWithNoContainedNulls<string>((IEnumerable<string>) x.Value)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new CargoPublishManifest()
      {
        name = this.Identity.Name.DisplayName,
        vers = this.Identity.Version.DisplayVersion,
        deps = (IEnumerable<CargoPublishManifestDependency>) this.Dependencies.Select<CargoPackageMetadataDependency, CargoPublishManifestDependency>(CargoPackageMetadata.\u003C\u003EO.\u003C2\u003E__TransformDependencyFromPublishManifest ?? (CargoPackageMetadata.\u003C\u003EO.\u003C2\u003E__TransformDependencyFromPublishManifest = new Func<CargoPackageMetadataDependency, CargoPublishManifestDependency>(CargoPackageMetadata.TransformDependencyFromPublishManifest))).ToImmutableArray<CargoPublishManifestDependency>(),
        features = (IReadOnlyDictionary<string, IEnumerable<string>>) immutableDictionary,
        links = this.Links,
        authors = (IEnumerable<string>) ImmutableArray<string>.Empty,
        description = (string) null,
        documentation = (string) null,
        homepage = (string) null,
        readme = (string) null,
        readme_file = (string) null,
        keywords = (IEnumerable<string>) ImmutableArray<string>.Empty,
        categories = (IEnumerable<string>) ImmutableArray<string>.Empty,
        license = (string) null,
        license_file = (string) null,
        repository = (string) null
      };
    }

    private static ImmutableArray<T> ToNonNullImmutableWithNoContainedNulls<T>(IEnumerable<T?>? array) => array != null ? ImmutableArray.ToImmutableArray<T>(array.Where<T>((Func<T, bool>) (x => (object) x != null))) : ImmutableArray<T>.Empty;

    private static CargoPackageMetadataDependency TransformDependency(
      CargoPublishManifestDependency dep)
    {
      if (string.IsNullOrWhiteSpace(dep.name))
        throw new InvalidPackageException("name is required in dep");
      if (string.IsNullOrWhiteSpace(dep.version_req))
        throw new InvalidPackageException("version_req is required in dep");
      if (string.IsNullOrWhiteSpace(dep.kind))
        throw new InvalidPackageException("kind is required in dep");
      return new CargoPackageMetadataDependency(CargoPackageNameParser.Parse(dep.name), dep.version_req, (IImmutableList<string>) CargoPackageMetadata.ToNonNullImmutableWithNoContainedNulls<string>((IEnumerable<string>) dep.features), dep.optional, dep.default_features, dep.target, dep.kind, dep.registry, dep.explicit_name_in_toml);
    }

    private static CargoPackageMetadataDependency TransformDependencyFromIndex(
      CargoIndexDependency dep)
    {
      if (string.IsNullOrWhiteSpace(dep.DeclaredName))
        throw new InvalidPackageException("name is required in dep");
      if (string.IsNullOrWhiteSpace(dep.VersionRequirement))
        throw new InvalidPackageException("req is required in dep");
      string Kind = dep.Kind ?? "normal";
      if (string.IsNullOrWhiteSpace(Kind))
        throw new InvalidPackageException("kind in dep may not be an empty string or one that contains only whitespace");
      string declaredName = dep.ActualPackageName == null ? (string) null : dep.DeclaredName;
      return new CargoPackageMetadataDependency(CargoPackageNameParser.Parse(dep.ActualPackageName ?? dep.DeclaredName), dep.VersionRequirement, (IImmutableList<string>) dep.Features.ToImmutableArray<string>(), dep.Optional, dep.DefaultFeaturesEnabled, dep.TargetPlatform, Kind, dep.Registry, declaredName);
    }

    private static CargoPublishManifestDependency TransformDependencyFromPublishManifest(
      CargoPackageMetadataDependency dep)
    {
      if (string.IsNullOrWhiteSpace(dep.ActualPackageName.DisplayName))
        throw new InvalidPackageException("name is required in dep");
      if (string.IsNullOrWhiteSpace(dep.VersionRequirement))
        throw new InvalidPackageException("req is required in dep");
      if (string.IsNullOrWhiteSpace(dep.Kind))
        throw new InvalidPackageException("kind is required in dep");
      return new CargoPublishManifestDependency()
      {
        name = dep.ActualPackageName.DisplayName,
        version_req = dep.VersionRequirement,
        features = dep.Features.ToArray<string>(),
        optional = dep.Optional,
        default_features = dep.DefaultFeaturesEnabled,
        target = dep.TargetPlatform,
        kind = dep.Kind,
        registry = dep.RegistryIndex,
        explicit_name_in_toml = dep.DeclaredName
      };
    }

    public static LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> LazyDeserialize(
      CargoRawPackageMetadata raw)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata>(raw, CargoPackageMetadata.\u003C\u003EO.\u003C3\u003E__Deserialize ?? (CargoPackageMetadata.\u003C\u003EO.\u003C3\u003E__Deserialize = new Func<CargoRawPackageMetadata, CargoPackageMetadata>(CargoPackageMetadata.Deserialize)));
    }

    public static CargoPackageMetadata Deserialize(CargoRawPackageMetadata raw)
    {
      return CombineMajorInfoSources() with
      {
        LicenseFilePath = raw.ActualLicenseFilePath,
        ReadmeFilePath = raw.ActualReadmeFilePath
      };

      CargoPackageMetadata CombineMajorInfoSources()
      {
        CargoTomlManifest cargoToml1 = raw.CargoToml?.Value;
        CargoPublishManifest parsedJson = raw.PublishManifest?.Value;
        CargoIndexVersionRow indexRow = raw.UpstreamIndexRow?.Value;
        if ((object) cargoToml1 == null)
        {
          if (parsedJson == null)
            return indexRow != null ? CargoPackageMetadata.FromIndex(indexRow) : throw new InvalidOperationException("Cannot deserialize Cargo package metadata without at least one of Cargo.toml, publish manifest, or upstream index row");
          return indexRow == null ? CargoPackageMetadata.FromPublishManifest(parsedJson) : CargoPackageMetadata.FromPublishManifest(parsedJson);
        }
        if (parsedJson == null)
        {
          if (indexRow == null)
            return CargoPackageMetadata.FromCargoToml(cargoToml1);
          CargoTomlManifest cargoToml2 = cargoToml1;
          CargoPackageMetadata cargoPackageMetadata = CargoPackageMetadata.FromIndex(indexRow);
          return CargoPackageMetadata.FromCargoToml(cargoToml2) with
          {
            Dependencies = cargoPackageMetadata.Dependencies,
            Features = cargoPackageMetadata.Features,
            Links = cargoPackageMetadata.Links
          };
        }
        if (indexRow == null)
        {
          CargoTomlManifest cargoToml3 = cargoToml1;
          CargoPackageMetadata cargoPackageMetadata = CargoPackageMetadata.FromPublishManifest(parsedJson);
          return CargoPackageMetadata.FromCargoToml(cargoToml3) with
          {
            Dependencies = cargoPackageMetadata.Dependencies,
            Features = cargoPackageMetadata.Features,
            Links = cargoPackageMetadata.Links
          };
        }
        CargoTomlManifest cargoToml4 = cargoToml1;
        CargoPackageMetadata cargoPackageMetadata1 = CargoPackageMetadata.FromPublishManifest(parsedJson);
        return CargoPackageMetadata.FromCargoToml(cargoToml4) with
        {
          Dependencies = cargoPackageMetadata1.Dependencies,
          Features = cargoPackageMetadata1.Features,
          Links = cargoPackageMetadata1.Links
        };
      }
    }
  }
}
