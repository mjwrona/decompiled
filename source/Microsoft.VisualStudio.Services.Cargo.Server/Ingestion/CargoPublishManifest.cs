// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Ingestion.CargoPublishManifest
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Ingestion
{
  [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
  public class CargoPublishManifest
  {
    public string? name { get; set; }

    public string? vers { get; set; }

    public IEnumerable<CargoPublishManifestDependency>? deps { get; set; }

    public IReadOnlyDictionary<string, IEnumerable<string>?>? features { get; set; }

    public IEnumerable<string>? authors { get; set; }

    public string? description { get; set; }

    public string? documentation { get; set; }

    public string? homepage { get; set; }

    public string? readme { get; set; }

    public string? readme_file { get; set; }

    public IEnumerable<string>? keywords { get; set; }

    public IEnumerable<string>? categories { get; set; }

    public string? license { get; set; }

    public string? license_file { get; set; }

    public string? repository { get; set; }

    public string? links { get; set; }

    public static LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest> LazyDeserialize(
      DeflateCompressibleBytes manifestBytes)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest>(manifestBytes, CargoPublishManifest.\u003C\u003EO.\u003C0\u003E__Deserialize ?? (CargoPublishManifest.\u003C\u003EO.\u003C0\u003E__Deserialize = new Func<DeflateCompressibleBytes, CargoPublishManifest>(CargoPublishManifest.Deserialize)));
    }

    public static CargoPublishManifest Deserialize(DeflateCompressibleBytes manifestBytes)
    {
      using (MemoryStream memoryStream = new MemoryStream(manifestBytes.AsUncompressedBytes(), false))
      {
        using (StreamReader reader1 = new StreamReader((Stream) memoryStream, Encoding.UTF8))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
            return new JsonSerializer().Deserialize<CargoPublishManifest>((JsonReader) reader2) ?? throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_IngestionManifestNull());
        }
      }
    }

    public CargoPackageIdentity ExtractPackageIdentity()
    {
      if (string.IsNullOrWhiteSpace(this.name))
        throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_IngestionManifestNoName());
      return !string.IsNullOrWhiteSpace(this.vers) ? new CargoPackageIdentity(CargoPackageNameParser.Parse(this.name), CargoPackageVersionParser.Parse(this.vers)) : throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_IngestionManifestNoVersion());
    }

    public DeflateCompressibleBytes SerializeToDeflateCompressibleBytes() => DeflateCompressibleBytes.FromUncompressedBytes(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) this)));
  }
}
