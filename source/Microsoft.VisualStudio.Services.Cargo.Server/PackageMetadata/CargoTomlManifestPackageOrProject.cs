// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.CargoTomlManifestPackageOrProject
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System.Collections.Generic;
using System.Collections.Immutable;
using Tomlyn.Model;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public record CargoTomlManifestPackageOrProject(
    string Name,
    string Version,
    string? License,
    string? LicenseFile,
    string? Links,
    StringOrBool? Readme,
    ImmutableArray<string> Authors,
    string? Description,
    string? Documentation,
    string? Homepage,
    string? Repository,
    ImmutableArray<string> Keywords,
    ImmutableArray<string> Categories)
  {
    public static CargoTomlManifestPackageOrProject FromToml(
      TomlTable packageTable,
      ImmutableList<string> path)
    {
      string castedValue1 = packageTable.GetCastedValue<string>(path, "name");
      string castedValue2 = packageTable.GetCastedValue<string>(path, "version");
      string castedValueOrDefault1 = packageTable.GetCastedValueOrDefault<string, string>("license");
      string castedValueOrDefault2 = packageTable.GetCastedValueOrDefault<string, string>("license-file");
      string castedValueOrDefault3 = packageTable.GetCastedValueOrDefault<string, string>("links");
      StringOrBool stringOrBool1;
      switch (packageTable.GetCastedValueOrDefault<string, object>("readme"))
      {
        case bool flag:
          stringOrBool1 = (StringOrBool) new StringOrBool.Bool(flag);
          break;
        case string str:
          stringOrBool1 = (StringOrBool) new StringOrBool.String(str);
          break;
        case null:
          stringOrBool1 = (StringOrBool) null;
          break;
        default:
          throw new InvalidPackageException(Resources.Error_CargoTomlWrongTypeElement((object) TomlHelpers.GetKeyString((IEnumerable<string>) path.Add("readme"))));
      }
      StringOrBool stringOrBool2 = stringOrBool1;
      ImmutableArray<string>? stringArrayOrDefault1 = TomlHelpers.ExtractStringArrayOrDefault(packageTable, "authors");
      string castedValueOrDefault4 = packageTable.GetCastedValueOrDefault<string, string>("description");
      string castedValueOrDefault5 = packageTable.GetCastedValueOrDefault<string, string>("documentation");
      string castedValueOrDefault6 = packageTable.GetCastedValueOrDefault<string, string>("homepage");
      string castedValueOrDefault7 = packageTable.GetCastedValueOrDefault<string, string>("repository");
      ImmutableArray<string>? stringArrayOrDefault2 = TomlHelpers.ExtractStringArrayOrDefault(packageTable, "keywords");
      ImmutableArray<string>? stringArrayOrDefault3 = TomlHelpers.ExtractStringArrayOrDefault(packageTable, "categories");
      string Version = castedValue2;
      string License = castedValueOrDefault1;
      string LicenseFile = castedValueOrDefault2;
      string Links = castedValueOrDefault3;
      StringOrBool Readme = stringOrBool2;
      ImmutableArray<string>? nullable = stringArrayOrDefault1;
      ImmutableArray<string> Authors = nullable ?? ImmutableArray<string>.Empty;
      string Description = castedValueOrDefault4;
      string Documentation = castedValueOrDefault5;
      string Homepage = castedValueOrDefault6;
      string Repository = castedValueOrDefault7;
      nullable = stringArrayOrDefault2;
      ImmutableArray<string> Keywords = nullable ?? ImmutableArray<string>.Empty;
      nullable = stringArrayOrDefault3;
      ImmutableArray<string> Categories = nullable ?? ImmutableArray<string>.Empty;
      return new CargoTomlManifestPackageOrProject(castedValue1, Version, License, LicenseFile, Links, Readme, Authors, Description, Documentation, Homepage, Repository, Keywords, Categories);
    }
  }
}
