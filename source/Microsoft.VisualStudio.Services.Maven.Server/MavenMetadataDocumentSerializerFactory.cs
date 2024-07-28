// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenMetadataDocumentSerializerFactory
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Views;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenMetadataDocumentSerializerFactory : 
    IFactory<PackageNameQuery<IMavenMetadataEntry>, ISerializer<MetadataDocument<IMavenMetadataEntry>>>
  {
    private readonly ICache<string, object> telemetryCache;
    private readonly ITracerService tracerService;

    public MavenMetadataDocumentSerializerFactory(
      ICache<string, object> telemetryCache,
      ITracerService tracerService)
    {
      this.telemetryCache = telemetryCache;
      this.tracerService = tracerService;
    }

    public ISerializer<MetadataDocument<IMavenMetadataEntry>> Get(
      PackageNameQuery<IMavenMetadataEntry> query)
    {
      StreamingPackageMetadataDocumentJsonConverter<MavenPackageVersion, IMavenMetadataEntryWritable, IMavenMetadataEntry> documentJsonConverter = new StreamingPackageMetadataDocumentJsonConverter<MavenPackageVersion, IMavenMetadataEntryWritable, IMavenMetadataEntry>(query.RequestData, (IConverter<FeedRequest<IMavenMetadataEntryWritable>, IMavenMetadataEntryWritable>) new LocalImplicitViewMetadataConverter<IMavenMetadataEntryWritable>(), query.Options, (Func<IPackageName, MavenPackageVersion, IMavenMetadataEntryWritable>) ((packageName, packageVersion) => (IMavenMetadataEntryWritable) new MavenMetadataEntry(new MavenPackageIdentity((MavenPackageName) packageName, packageVersion))), (Func<string, IPackageName>) (nameString => (IPackageName) new MavenPackageName(nameString)), (Func<string, MavenPackageVersion>) (versionString => new MavenPackageVersion(versionString)), (IEnumerable<KeyValuePair<string, PropertyDefinition<IMavenMetadataEntryWritable>>>) this.PopulateProperties(), this.telemetryCache, this.tracerService);
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.Converters.Add((JsonConverter) documentJsonConverter);
      return (ISerializer<MetadataDocument<IMavenMetadataEntry>>) new JsonSerializer<MetadataDocument<IMavenMetadataEntry>>(settings);
    }

    private Dictionary<string, PropertyDefinition<IMavenMetadataEntryWritable>> PopulateProperties()
    {
      Dictionary<string, PropertyDefinition<IMavenMetadataEntryWritable>> dictionary = new Dictionary<string, PropertyDefinition<IMavenMetadataEntryWritable>>();
      dictionary.Add("Files", new PropertyDefinition<IMavenMetadataEntryWritable>((Expression<Func<IMavenMetadataEntryWritable, object>>) (e => e.PackageFiles), (Action<JsonWriter, ICache<string, object>, IMavenMetadataEntryWritable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => this.WriteFiles(w, (IEnumerable<MavenPackageFileNew>) e.PackageFiles)), (Action<IMavenMetadataEntryWritable, MetadataDocumentProperties, ICache<string, object>, object>) null, new Action<IMavenMetadataEntryWritable, ICache<string, object>, JsonReader>(this.ReadFiles)));
      dictionary.Add("MD", new PropertyDefinition<IMavenMetadataEntryWritable>((Expression<Func<IMavenMetadataEntryWritable, object>>) (e => e.PomBytes), (Action<JsonWriter, ICache<string, object>, IMavenMetadataEntryWritable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(Convert.ToBase64String(CompressionHelper.DeflateByteArray(e.PomBytes)))), (Action<IMavenMetadataEntryWritable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.PomBytes = this.ReadPomBytes((string) val, dProps)), skipSerializingCondition: (Func<IMavenMetadataEntryWritable, bool>) (e => e.PomBytes == null || ((IMetadataEntryWritable) e).PermanentDeletedDate.HasValue)));
      return dictionary;
    }

    private byte[] ReadPomBytes(string metadataDocPomBytes, MetadataDocumentProperties properties)
    {
      try
      {
        return CompressionHelper.InflateByteArray(Convert.FromBase64String(metadataDocPomBytes));
      }
      catch (Exception ex)
      {
        using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (ReadPomBytes)))
        {
          tracerBlock.TraceException(ex);
          StringBuilder stringBuilder = new StringBuilder("Failed to decode/inflate POM bytes. ", 8192);
          stringBuilder.Append(string.Format("PackageName: {0}; ", (object) properties.PackageName));
          stringBuilder.Append(string.Format("PomBytes length: {0}; ", (object) (metadataDocPomBytes != null ? metadataDocPomBytes.Length : 0)));
          stringBuilder.Append("PomBytes (raw): " + (metadataDocPomBytes?.Substring(0, Math.Min(7000, metadataDocPomBytes.Length)) ?? "(null)"));
          tracerBlock.TraceVerbose(stringBuilder.ToString());
          throw;
        }
      }
    }

    private void ReadFiles(
      IMavenMetadataEntryWritable entry,
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
        IStorageId storageId1 = StorageId.Parse((string) reader.Value);
        reader.ReadExpectedPropertyNameOnly("Size");
        reader.Read();
        long num = (long) reader.Value;
        DateTime dateTime = SerializationUtils.ReadDate(reader.ReadExpectedStringProperty("AddD"));
        reader.ReadExpectedPropertyNameOnly("Hashes");
        IReadOnlyCollection<HashAndType> hashAndTypes = this.ReadHashes(reader);
        IStorageId storageId2 = storageId1;
        IReadOnlyCollection<HashAndType> hashes = hashAndTypes;
        long size = num;
        DateTime dateAdded = dateTime;
        MavenPackageFileNew packageFile = new MavenPackageFileNew(path, storageId2, (IEnumerable<HashAndType>) hashes, size, dateAdded);
        entry.AddPackageFile(packageFile);
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

    private void WriteFiles(JsonWriter writer, IEnumerable<MavenPackageFileNew> files)
    {
      writer.WriteStartArray();
      foreach (MavenPackageFileNew file in files)
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
        writer.WriteEndObject();
      }
      writer.WriteEndArray();
    }
  }
}
