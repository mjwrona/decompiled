// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiMetadataDocumentSerializerBase`3
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Views;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public abstract class PyPiMetadataDocumentSerializerBase<TWritableMetadata, TMetadata, TPackageFile> : 
    IFactory<PackageNameQuery<TMetadata>, ISerializer<MetadataDocument<TMetadata>>>
    where TWritableMetadata : class, TMetadata, IPackageFiles<TPackageFile>, IPyPiMetadataEntryWritableCore<TPackageFile>
    where TMetadata : class, IMetadataEntry<PyPiPackageIdentity>, ICreateWriteable<TWritableMetadata>
    where TPackageFile : PyPiPackageFile
  {
    private readonly ICache<string, object> telemetryCache;
    private readonly ITracerService tracerService;

    protected PyPiMetadataDocumentSerializerBase(
      ICache<string, object> telemetryCache,
      ITracerService tracerService)
    {
      this.telemetryCache = telemetryCache;
      this.tracerService = tracerService;
    }

    public ISerializer<MetadataDocument<TMetadata>> Get(PackageNameQuery<TMetadata> query)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      StreamingPackageMetadataDocumentJsonConverter<PyPiPackageVersion, TWritableMetadata, TMetadata> documentJsonConverter = new StreamingPackageMetadataDocumentJsonConverter<PyPiPackageVersion, TWritableMetadata, TMetadata>(query.RequestData, (IConverter<FeedRequest<TWritableMetadata>, TWritableMetadata>) new LocalImplicitViewMetadataConverter<TWritableMetadata>(), query.Options, (Func<IPackageName, PyPiPackageVersion, TWritableMetadata>) ((packageName, packageVersion) => this.CreateEntry(new PyPiPackageIdentity((PyPiPackageName) packageName, packageVersion))), (Func<string, IPackageName>) (nameString => (IPackageName) new PyPiPackageName(nameString)), PyPiMetadataDocumentSerializerBase<TWritableMetadata, TMetadata, TPackageFile>.\u003C\u003EO.\u003C0\u003E__Parse ?? (PyPiMetadataDocumentSerializerBase<TWritableMetadata, TMetadata, TPackageFile>.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, PyPiPackageVersion>(PyPiPackageVersionParser.Parse)), (IEnumerable<KeyValuePair<string, PropertyDefinition<TWritableMetadata>>>) this.PopulateProperties(), this.telemetryCache, this.tracerService);
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.Converters.Add((JsonConverter) documentJsonConverter);
      return (ISerializer<MetadataDocument<TMetadata>>) new JsonSerializer<MetadataDocument<TMetadata>>(settings);
    }

    protected abstract TWritableMetadata CreateEntry(PyPiPackageIdentity packageIdentity);

    protected abstract void WriteFileExtras(JsonWriter writer, TPackageFile file);

    protected abstract TPackageFile CreateFile(
      JsonReader reader,
      string path,
      IStorageId storageId,
      IReadOnlyCollection<HashAndType> hashes,
      long size,
      DateTime dateAdded,
      PyPiDistType distType);

    protected virtual Dictionary<string, PropertyDefinition<TWritableMetadata>> PopulateProperties()
    {
      Dictionary<string, PropertyDefinition<TWritableMetadata>> dictionary = new Dictionary<string, PropertyDefinition<TWritableMetadata>>();
      dictionary.Add("Files", new PropertyDefinition<TWritableMetadata>((Expression<Func<TWritableMetadata, object>>) (e => e.PackageFiles), (Action<JsonWriter, ICache<string, object>, TWritableMetadata, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => this.WriteFiles(w, (IEnumerable<TPackageFile>) e.PackageFiles)), (Action<TWritableMetadata, MetadataDocumentProperties, ICache<string, object>, object>) null, new Action<TWritableMetadata, ICache<string, object>, JsonReader>(this.ReadFiles)));
      dictionary.Add("RequiresPy", new PropertyDefinition<TWritableMetadata>((Expression<Func<TWritableMetadata, object>>) (e => e.RequiresPython), (Action<JsonWriter, ICache<string, object>, TWritableMetadata, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IPyPiMetadataEntryWritableCore<TPackageFile>) e).RequiresPython.ToString())), (Action<TWritableMetadata, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.RequiresPython = RequirementParser.ParseVersionConstraintList((string) val, TimeSpan.FromSeconds(3.0))), skipSerializingCondition: (Func<TWritableMetadata, bool>) (e => ((IPyPiMetadataEntryWritableCore<TPackageFile>) e).RequiresPython == null)));
      return dictionary;
    }

    private void ReadFiles(
      TWritableMetadata entry,
      ICache<string, object> telemetryCache,
      JsonReader reader)
    {
      if (reader.TokenType != JsonToken.StartArray)
        throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_TokenNotAProperty((object) reader.TokenType));
      reader.Read();
      while (reader.TokenType != JsonToken.EndArray)
      {
        string path = reader.ReadExpectedStringProperty("Path");
        reader.ReadExpectedPropertyNameOnly("BlobId");
        reader.Read();
        IStorageId storageId = StorageId.Parse((string) reader.Value);
        reader.ReadExpectedPropertyNameOnly("Size");
        reader.Read();
        long size = (long) reader.Value;
        DateTime dateAdded = SerializationUtils.ReadDate(reader.ReadExpectedStringProperty("AddD"));
        reader.ReadExpectedPropertyNameOnly("Hashes");
        IReadOnlyCollection<HashAndType> hashes = this.ReadHashes(reader);
        PyPiDistType distType = (PyPiDistType) Enum.Parse(typeof (PyPiDistType), reader.ReadExpectedStringProperty("Dist"));
        TPackageFile file = this.CreateFile(reader, path, storageId, hashes, size, dateAdded, distType);
        entry.AddPackageFile(file);
        reader.Read();
        reader.Read();
      }
      if (telemetryCache.TryGet("Packaging.Properties.FilesCount", out object _))
        return;
      telemetryCache.Set("Packaging.Properties.FilesCount", (object) entry.PackageFiles.Count);
    }

    protected IReadOnlyCollection<HashAndType> ReadHashes(JsonReader reader)
    {
      List<HashAndType> hashAndTypeList = new List<HashAndType>();
      reader.Read();
      if (reader.TokenType != JsonToken.StartObject)
        throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ExpectedStartOfArrayOrObject());
      while (reader.Read() && reader.TokenType != JsonToken.EndObject)
      {
        HashType result;
        if (!Enum.TryParse<HashType>(reader.Value.ToString(), out result))
          throw new InvalidDataException(string.Format("could not parse HashType enum value: {0}", reader.Value));
        reader.Read();
        hashAndTypeList.Add(new HashAndType()
        {
          HashType = result,
          Value = reader.Value.ToString()
        });
      }
      return (IReadOnlyCollection<HashAndType>) hashAndTypeList;
    }

    private void WriteFiles(JsonWriter writer, IEnumerable<TPackageFile> files)
    {
      writer.WriteStartArray();
      foreach (TPackageFile file in files)
      {
        writer.WriteStartObject();
        writer.WritePropertyName("Path");
        writer.WriteValue(file.Path);
        writer.WritePropertyName("BlobId");
        writer.WriteValue(file.StorageId.ValueString);
        writer.WritePropertyName("Size");
        writer.WriteValue(file.SizeInBytes);
        writer.WritePropertyName("AddD");
        writer.WriteValue(file.DateAdded.ToString("u"));
        writer.WritePropertyName("Hashes");
        writer.WriteStartObject();
        foreach (HashAndType hash in (IEnumerable<HashAndType>) file.Hashes)
        {
          writer.WritePropertyName(hash.HashType.ToString());
          writer.WriteValue(hash.Value);
        }
        writer.WriteEndObject();
        writer.WritePropertyName("Dist");
        writer.WriteValue(file.DistType.ToString());
        this.WriteFileExtras(writer, file);
        writer.WriteEndObject();
      }
      writer.WriteEndArray();
    }
  }
}
