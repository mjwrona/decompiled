// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata.CargoMetadataDocumentSerializerFactory
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Views;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata
{
  public class CargoMetadataDocumentSerializerFactory : 
    IFactory<PackageNameQuery<ICargoMetadataEntry>, ISerializer<MetadataDocument<ICargoMetadataEntry>>>
  {
    private readonly Dictionary<string, PropertyDefinition<ICargoMetadataEntryWriteable>> cargoProperties;
    private readonly ICache<string, object> requestItemsAsCacheFacade;
    private readonly ITracerService tracerService;

    public CargoMetadataDocumentSerializerFactory(
      ICache<string, object> requestItemsAsCacheFacade,
      ITracerService tracerService)
    {
      this.requestItemsAsCacheFacade = requestItemsAsCacheFacade;
      this.tracerService = tracerService;
      this.cargoProperties = this.PopulateCargoProperties();
    }

    private Dictionary<string, PropertyDefinition<ICargoMetadataEntryWriteable>> PopulateCargoProperties()
    {
      Dictionary<string, PropertyDefinition<ICargoMetadataEntryWriteable>> dictionary = new Dictionary<string, PropertyDefinition<ICargoMetadataEntryWriteable>>();
      dictionary.Add("MD2", new PropertyDefinition<ICargoMetadataEntryWriteable>((Expression<Func<ICargoMetadataEntryWriteable, object>>) (e => e.Metadata), (Action<JsonWriter, ICache<string, object>, ICargoMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => new JsonSerializer().Serialize(w, (object) CargoMetadataDocumentSerializerFactory.Metadata2Json.Pack(e.Metadata.Serialized))), (Action<ICargoMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) null, (Action<ICargoMetadataEntryWriteable, ICache<string, object>, JsonReader>) ((e, dProps, reader) => e.Metadata = CargoPackageMetadata.LazyDeserialize(new JsonSerializer().Deserialize<CargoMetadataDocumentSerializerFactory.Metadata2Json>(reader).Unpack())), (Func<ICargoMetadataEntryWriteable, bool>) (e => e.IsPermanentlyDeleted())));
      dictionary.Add("MD", new PropertyDefinition<ICargoMetadataEntryWriteable>((Expression<Func<ICargoMetadataEntryWriteable, object>>) (e => e.Metadata), (Action<JsonWriter, ICache<string, object>, ICargoMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(e.Metadata.Value.SynthesizePublishManifestFromIndexProperties().SerializeToDeflateCompressibleBytes().AsDeflatedBase64String())), (Action<ICargoMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) =>
      {
        if (e.Metadata != null)
          return;
        e.Metadata = CargoPackageMetadata.LazyDeserialize(new CargoRawPackageMetadata(CargoPublishManifest.LazyDeserialize(DeflateCompressibleBytes.FromDeflatedBase64String((string) val)), (LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>) null, (LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest>) null, (string) null, (string) null));
      }), skipSerializingCondition: (Func<ICargoMetadataEntryWriteable, bool>) (e => e.IsPermanentlyDeleted())));
      dictionary.Add("Hash", new PropertyDefinition<ICargoMetadataEntryWriteable>((Expression<Func<ICargoMetadataEntryWriteable, object>>) (e => e.Hashes), (Action<JsonWriter, ICache<string, object>, ICargoMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, _1, e, _2) => CargoMetadataDocumentSerializerFactory.WriteHashes(w, (IEnumerable<HashAndType>) e.Hashes)), (Action<ICargoMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) null, (Action<ICargoMetadataEntryWriteable, ICache<string, object>, JsonReader>) ((e, _, reader) => e.Hashes = (IReadOnlyList<HashAndType>) CargoMetadataDocumentSerializerFactory.ReadHashes(reader))));
      dictionary.Add("Yank", new PropertyDefinition<ICargoMetadataEntryWriteable>((Expression<Func<ICargoMetadataEntryWriteable, object>>) (e => (object) e.Yanked), (Action<JsonWriter, ICache<string, object>, ICargoMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(e.Yanked)), (Action<ICargoMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.Yanked = (bool) val)));
      dictionary.Add("Inner", new PropertyDefinition<ICargoMetadataEntryWriteable>((Expression<Func<ICargoMetadataEntryWriteable, object>>) (e => (object) e.CrateInnerFiles), (Action<JsonWriter, ICache<string, object>, ICargoMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, _3, e, _4) => PackageFileSerialization.SerializeInnerFileReferences(new JsonSerializer(), w, (IEnumerable<InnerFileReference>) e.CrateInnerFiles)), (Action<ICargoMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) null, (Action<ICargoMetadataEntryWriteable, ICache<string, object>, JsonReader>) ((e, _, reader) => e.CrateInnerFiles = PackageFileSerialization.DeserializeInnerFileReferences(new JsonSerializer(), reader))));
      return dictionary;
    }

    private static ImmutableArray<HashAndType> ReadHashes(JsonReader reader)
    {
      ImmutableArray<HashAndType>.Builder builder = JToken.ReadFrom(reader) is JObject jobject ? ImmutableArray.CreateBuilder<HashAndType>(jobject.Count) : throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ExpectedStartOfObject());
      foreach (JProperty property in jobject.Properties())
      {
        HashType result;
        if (!Enum.TryParse<HashType>(property.Name, out result))
          throw new InvalidDataException(string.Format("could not parse HashType enum value: {0}", reader.Value));
        if (property.Value.Type != JTokenType.String)
          throw new InvalidDataException(string.Format("Expected a string for hash value, but found {0}", (object) property.Value.Type));
        builder.Add(new HashAndType()
        {
          HashType = result,
          Value = (string) property.Value
        });
      }
      return builder.MoveToImmutable();
    }

    private static void WriteHashes(JsonWriter writer, IEnumerable<HashAndType> hashes)
    {
      writer.WriteStartObject();
      foreach (HashAndType hash in hashes)
      {
        writer.WritePropertyName(hash.HashType.ToString());
        writer.WriteValue(hash.Value);
      }
      writer.WriteEndObject();
    }

    public ISerializer<MetadataDocument<ICargoMetadataEntry>> Get(
      PackageNameQuery<ICargoMetadataEntry> queryRequest)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      StreamingPackageMetadataDocumentJsonConverter<CargoPackageVersion, ICargoMetadataEntryWriteable, ICargoMetadataEntry> documentJsonConverter = new StreamingPackageMetadataDocumentJsonConverter<CargoPackageVersion, ICargoMetadataEntryWriteable, ICargoMetadataEntry>(queryRequest.RequestData, (IConverter<FeedRequest<ICargoMetadataEntryWriteable>, ICargoMetadataEntryWriteable>) new LocalImplicitViewMetadataConverter<ICargoMetadataEntryWriteable>(), queryRequest.Options, (Func<IPackageName, CargoPackageVersion, ICargoMetadataEntryWriteable>) ((packageName, packageVersion) => (ICargoMetadataEntryWriteable) new CargoMetadataEntry(new CargoPackageIdentity((CargoPackageName) packageName, packageVersion))), CargoMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C0\u003E__Parse ?? (CargoMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, IPackageName>(CargoPackageNameParser.Parse)), CargoMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C1\u003E__Parse ?? (CargoMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C1\u003E__Parse = new Func<string, CargoPackageVersion>(CargoPackageVersionParser.Parse)), (IEnumerable<KeyValuePair<string, PropertyDefinition<ICargoMetadataEntryWriteable>>>) this.cargoProperties, this.requestItemsAsCacheFacade, this.tracerService);
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.Converters.Add((JsonConverter) documentJsonConverter);
      return (ISerializer<MetadataDocument<ICargoMetadataEntry>>) new JsonSerializer<MetadataDocument<ICargoMetadataEntry>>(settings);
    }

    [DataContract]
    private record Metadata2Json(
      string? PublishManifest,
      string? CargoToml,
      string? IndexRow,
      string? ActualReadmePath,
      string? ActualLicensePath)
    {
      public static CargoMetadataDocumentSerializerFactory.Metadata2Json Pack(
        CargoRawPackageMetadata rawPackageMetadata)
      {
        return new CargoMetadataDocumentSerializerFactory.Metadata2Json(rawPackageMetadata.PublishManifest?.Serialized.AsDeflatedBase64String(), rawPackageMetadata.CargoToml?.Serialized.AsDeflatedBase64String(), rawPackageMetadata.UpstreamIndexRow?.Serialized.AsDeflatedBase64String(), rawPackageMetadata.ActualReadmeFilePath, rawPackageMetadata.ActualLicenseFilePath);
      }

      public CargoRawPackageMetadata Unpack() => new CargoRawPackageMetadata(string.IsNullOrWhiteSpace(this.PublishManifest) ? (LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest>) null : CargoPublishManifest.LazyDeserialize(DeflateCompressibleBytes.FromDeflatedBase64String(this.PublishManifest)), string.IsNullOrWhiteSpace(this.IndexRow) ? (LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>) null : CargoIndexVersionRow.LazyDeserialize(DeflateCompressibleBytes.FromDeflatedBase64String(this.IndexRow)), string.IsNullOrWhiteSpace(this.CargoToml) ? (LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest>) null : CargoTomlManifest.LazyDeserialize(DeflateCompressibleBytes.FromDeflatedBase64String(this.CargoToml)), this.ActualReadmePath, this.ActualLicensePath);
    }
  }
}
