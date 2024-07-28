// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index.CargoIndexVersionRow
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index
{
  [DataContract]
  public class CargoIndexVersionRow : PackagingSecuredObject
  {
    [DataMember(Name = "name")]
    public string PackageName { get; init; }

    [DataMember(Name = "vers")]
    public string Version { get; init; }

    [DataMember(Name = "deps")]
    public IEnumerable<CargoIndexDependency>? Dependencies { get; init; }

    [DataMember(Name = "cksum")]
    public string? Sha256 { get; init; }

    [DataMember(Name = "features")]
    public IReadOnlyDictionary<string, IEnumerable<string>>? Features { get; init; }

    [DataMember(Name = "yanked")]
    public bool Yanked { get; init; }

    [DataMember(Name = "links")]
    public string? Links { get; init; }

    [DataMember(Name = "v")]
    public int SchemaVersion { get; init; } = 2;

    [DataMember(Name = "features2", EmitDefaultValue = false)]
    public IReadOnlyDictionary<string, IEnumerable<string>>? Features2 { get; init; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      if (this.Dependencies == null)
        return;
      foreach (PackagingSecuredObject dependency in this.Dependencies)
        dependency.SetSecuredObject(securedObject);
    }

    public static LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> LazyDeserialize(
      DeflateCompressibleBytes bytes)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>(bytes, CargoIndexVersionRow.\u003C\u003EO.\u003C0\u003E__Deserialize ?? (CargoIndexVersionRow.\u003C\u003EO.\u003C0\u003E__Deserialize = new Func<DeflateCompressibleBytes, CargoIndexVersionRow>(CargoIndexVersionRow.Deserialize)));
    }

    public static CargoIndexVersionRow Deserialize(DeflateCompressibleBytes bytes)
    {
      CargoIndexVersionRow cargoIndexVersionRow = JsonConvert.DeserializeObject<CargoIndexVersionRow>(Encoding.UTF8.GetString(bytes.AsUncompressedBytes())) ?? throw new InvalidOperationException("Deserialized a null CargoIndexVersionRow");
      if (cargoIndexVersionRow.PackageName == null || cargoIndexVersionRow.Version == null)
        throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_IndexRowMissingIdentity());
      return cargoIndexVersionRow;
    }

    public CargoPackageIdentity ExtractPackageIdentity() => CargoIdentityResolver.Instance.ResolvePackageIdentity(this.PackageName, this.Version);

    public static CargoIndexVersionRow FromMetadataEntry(ICargoMetadataEntry rv)
    {
      string sha256 = rv.Hashes.Single<HashAndType>((Func<HashAndType, bool>) (x => x.HashType == HashType.SHA256)).Value;
      return CargoIndexVersionRow.FromCargoPackageMetadata(rv.Metadata.Value, rv.PackageIdentity, sha256, rv.Yanked);
    }

    public static CargoIndexVersionRow FromCargoPackageMetadata(
      CargoPackageMetadata cargoPackageMetadata,
      CargoPackageIdentity packageIdentity,
      string sha256,
      bool yanked)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new CargoIndexVersionRow()
      {
        PackageName = packageIdentity.Name.DisplayName,
        Version = packageIdentity.Version.DisplayVersion,
        Dependencies = cargoPackageMetadata.Dependencies.Select<CargoPackageMetadataDependency, CargoIndexDependency>(CargoIndexVersionRow.\u003C\u003EO.\u003C1\u003E__FromMetadataDependency ?? (CargoIndexVersionRow.\u003C\u003EO.\u003C1\u003E__FromMetadataDependency = new Func<CargoPackageMetadataDependency, CargoIndexDependency>(CargoIndexDependency.FromMetadataDependency))),
        Sha256 = sha256.ToLowerInvariant(),
        Features = (IReadOnlyDictionary<string, IEnumerable<string>>) cargoPackageMetadata.Features.ToDictionary<KeyValuePair<string, ImmutableArray<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, ImmutableArray<string>>, string>) (x => x.Key), (Func<KeyValuePair<string, ImmutableArray<string>>, IEnumerable<string>>) (x => (IEnumerable<string>) x.Value)),
        Yanked = yanked,
        Links = cargoPackageMetadata?.Links,
        SchemaVersion = 2,
        Features2 = (IReadOnlyDictionary<string, IEnumerable<string>>) null
      };
    }

    public LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> LazySerialize() => new LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>(this, CargoIndexVersionRow.\u003C\u003EO.\u003C2\u003E__SerializeToDeflateCompressibleBytes ?? (CargoIndexVersionRow.\u003C\u003EO.\u003C2\u003E__SerializeToDeflateCompressibleBytes = new Func<CargoIndexVersionRow, DeflateCompressibleBytes>(CargoIndexVersionRow.SerializeToDeflateCompressibleBytes)));

    private static DeflateCompressibleBytes SerializeToDeflateCompressibleBytes(
      CargoIndexVersionRow x)
    {
      return DeflateCompressibleBytes.FromUncompressedBytes(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) x)));
    }

    public DeflateCompressibleBytes SerializeToDeflateCompressibleBytes() => CargoIndexVersionRow.SerializeToDeflateCompressibleBytes(this);
  }
}
