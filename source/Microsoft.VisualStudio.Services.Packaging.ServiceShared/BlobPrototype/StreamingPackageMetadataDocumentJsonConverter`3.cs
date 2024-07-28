// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.StreamingPackageMetadataDocumentJsonConverter`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class StreamingPackageMetadataDocumentJsonConverter<TPackageVersion, TMetadataEntry, TMetadataEntryInterface> : 
    JsonConverter
    where TPackageVersion : class, IPackageVersion, IComparable<TPackageVersion>
    where TMetadataEntry : class, IMetadataEntryWriteable<IPackageIdentity>, TMetadataEntryInterface
    where TMetadataEntryInterface : class, IMetadataEntry<IPackageIdentity>
  {
    private readonly IPackageNameRequest packageNameRequest;
    private readonly IConverter<FeedRequest<TMetadataEntry>, TMetadataEntry> implicitViewDecoratingConverter;
    private readonly QueryOptions<TMetadataEntryInterface> queryOptions;
    private readonly IEnumerable<KeyValuePair<string, PropertyDefinition<TMetadataEntry>>> additionalProperties;
    private readonly ICache<string, object> requestItemsTelemetryCache;
    private readonly ITracerService tracerService;
    private readonly PropertyDefinition<IMetadataDocumentPropertiesWriteable> nameMetadataPropertyDefinition;
    private readonly Func<IPackageName, TPackageVersion, TMetadataEntry> entryCreationFunc;
    private readonly Func<string, IPackageName> nameParserFunc;
    private readonly Func<string, TPackageVersion> versionParserFunc;
    private IDictionary<string, DocumentPropertyDefinition> documentPropertyInfoMapBacking;
    private IDictionary<string, PropertyDefinition<TMetadataEntry>> propertyInfoMapBacking;

    public StreamingPackageMetadataDocumentJsonConverter(
      IPackageNameRequest packageNameRequest,
      IConverter<FeedRequest<TMetadataEntry>, TMetadataEntry> implicitViewDecoratingConverter,
      QueryOptions<TMetadataEntryInterface> queryOptions,
      Func<IPackageName, TPackageVersion, TMetadataEntry> entryCreationFunc,
      Func<string, IPackageName> nameParserFunc,
      Func<string, TPackageVersion> versionParserFunc,
      IEnumerable<KeyValuePair<string, PropertyDefinition<TMetadataEntry>>> additionalProperties,
      ICache<string, object> requestItemsTelemetryCache,
      ITracerService tracerService,
      PropertyDefinition<IMetadataDocumentPropertiesWriteable> nameMetadataPropertyDefinition = null)
    {
      this.packageNameRequest = packageNameRequest;
      this.implicitViewDecoratingConverter = implicitViewDecoratingConverter;
      this.queryOptions = queryOptions;
      this.entryCreationFunc = entryCreationFunc;
      this.nameParserFunc = nameParserFunc;
      this.versionParserFunc = versionParserFunc;
      this.additionalProperties = additionalProperties;
      this.requestItemsTelemetryCache = requestItemsTelemetryCache;
      this.tracerService = tracerService;
      this.nameMetadataPropertyDefinition = nameMetadataPropertyDefinition;
    }

    private IDictionary<string, DocumentPropertyDefinition> DocumentPropertyInfoMap
    {
      get
      {
        if (this.documentPropertyInfoMapBacking != null)
          return this.documentPropertyInfoMapBacking;
        this.documentPropertyInfoMapBacking = (IDictionary<string, DocumentPropertyDefinition>) new Dictionary<string, DocumentPropertyDefinition>()
        {
          {
            "UpstreamsConfigurationHash",
            new DocumentPropertyDefinition("UpstreamsConfigurationHash", true)
          },
          {
            "UpstreamsLastRefreshedUtc",
            new DocumentPropertyDefinition("UpstreamsLastRefreshedUtc", true)
          },
          {
            "SourceChainMap",
            new DocumentPropertyDefinition("SourceChainMap", true)
          },
          {
            "Entries",
            new DocumentPropertyDefinition("Entries", false)
          },
          {
            "NameMetadata",
            new DocumentPropertyDefinition("NameMetadata", true)
          }
        };
        return this.documentPropertyInfoMapBacking;
      }
    }

    private IDictionary<string, PropertyDefinition<TMetadataEntry>> PropertyInfoMap
    {
      get
      {
        if (this.propertyInfoMapBacking != null)
          return this.propertyInfoMapBacking;
        Dictionary<string, PropertyDefinition<TMetadataEntry>> dictionary = new Dictionary<string, PropertyDefinition<TMetadataEntry>>();
        dictionary.Add("Com", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.CommitId), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).CommitId.ToString())), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.CommitId = PackagingCommitId.Parse((string) val))));
        dictionary.Add("CreateD", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.CreatedDate), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).CreatedDate.ToString("u"))), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.CreatedDate = SerializationUtils.ReadDate((string) val))));
        dictionary.Add("ModD", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.ModifiedDate), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).ModifiedDate.ToString("u"))), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.ModifiedDate = SerializationUtils.ReadDate((string) val))));
        dictionary.Add("Create", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.CreatedBy), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).CreatedBy.ToString("D"))), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.CreatedBy = Guid.Parse((string) val))));
        dictionary.Add("Mod", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.ModifiedBy), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).ModifiedBy.ToString("D"))), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.ModifiedBy = Guid.Parse((string) val))));
        dictionary.Add("BlobId", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => e.PackageStorageId), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).PackageStorageId.ValueString)), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.PackageStorageId = StorageId.Parse((string) val)), skipSerializingCondition: (Func<TMetadataEntry, bool>) (e => ((IMetadataEntryWritable) e).PackageStorageId == null)));
        dictionary.Add("Size", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.PackageSize), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).PackageSize)), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.PackageSize = (long) val)));
        dictionary.Add("Views", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => e.Views), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) =>
        {
          if (((IMetadataEntryWritable) e).Views == null)
            return;
          w.WriteStartArray();
          foreach (Guid view in ((IMetadataEntryWritable) e).Views)
            w.WriteValue(view);
          w.WriteEndArray();
        }), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) null, (Action<TMetadataEntry, ICache<string, object>, JsonReader>) ((metadata, telemetryCache, reader) =>
        {
          if (reader.TokenType != JsonToken.StartArray)
            throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ExpectedStartOfArrayViews());
          List<Guid> guidList = new List<Guid>();
          reader.Read();
          while (reader.TokenType == JsonToken.String)
          {
            guidList.Add(Guid.Parse(reader.Value.ToString()));
            reader.Read();
          }
          if (reader.TokenType != JsonToken.EndArray)
            throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ExpectedEndOfArrayViews());
          metadata.Views = (IEnumerable<Guid>) guidList;
        }), (Func<TMetadataEntry, bool>) (e => ((IMetadataEntryWritable) e).Views == null || !((IMetadataEntryWritable) e).Views.Any<Guid>())));
        dictionary.Add("DelD", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.DeletedDate), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).DeletedDate.Value.ToString("u"))), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.DeletedDate = new DateTime?(SerializationUtils.ReadDate((string) val))), skipSerializingCondition: (Func<TMetadataEntry, bool>) (e => !((IMetadataEntryWritable) e).DeletedDate.HasValue)));
        dictionary.Add("SDelD", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.ScheduledPermanentDeleteDate), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).ScheduledPermanentDeleteDate.Value.ToString("u"))), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.ScheduledPermanentDeleteDate = new DateTime?(SerializationUtils.ReadDate((string) val))), skipSerializingCondition: (Func<TMetadataEntry, bool>) (e => !((IMetadataEntryWritable) e).DeletedDate.HasValue || !((IMetadataEntryWritable) e).ScheduledPermanentDeleteDate.HasValue || ((IMetadataEntryWritable) e).PermanentDeletedDate.HasValue)));
        dictionary.Add("PDelD", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.PermanentDeletedDate), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).PermanentDeletedDate.Value.ToString("u"))), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.PermanentDeletedDate = new DateTime?(SerializationUtils.ReadDate((string) val))), skipSerializingCondition: (Func<TMetadataEntry, bool>) (e => !((IMetadataEntryWritable) e).PermanentDeletedDate.HasValue)));
        dictionary.Add("QuarD", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => (object) e.QuarantineUntilDate), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(((IMetadataEntryWritable) e).QuarantineUntilDate.Value.ToString("u"))), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.QuarantineUntilDate = new DateTime?(SerializationUtils.ReadDate((string) val))), skipSerializingCondition: (Func<TMetadataEntry, bool>) (e => !((IMetadataEntryWritable) e).QuarantineUntilDate.HasValue)));
        dictionary.Add("SChainI", new PropertyDefinition<TMetadataEntry>((Expression<Func<TMetadataEntry, object>>) (e => e.SourceChain), (Action<JsonWriter, ICache<string, object>, TMetadataEntry, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(this.GetEntrySourceChainIndex(e, dProps))), (Action<TMetadataEntry, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.SourceChain = val == null ? (IEnumerable<UpstreamSourceInfo>) null : (IEnumerable<UpstreamSourceInfo>) this.GetEntrySourceChain(dProps, checked ((int) (long) val))), skipSerializingCondition: (Func<TMetadataEntry, bool>) (e => ((IMetadataEntryWritable) e).SourceChain == null || !((IMetadataEntryWritable) e).SourceChain.Any<UpstreamSourceInfo>()), applyDefaultFunc: (Action<TMetadataEntry>) (e => e.SourceChain = ((IMetadataEntryWritable) e).SourceChain ?? Enumerable.Empty<UpstreamSourceInfo>())));
        this.propertyInfoMapBacking = (IDictionary<string, PropertyDefinition<TMetadataEntry>>) dictionary;
        if (this.additionalProperties != null && this.additionalProperties.Any<KeyValuePair<string, PropertyDefinition<TMetadataEntry>>>())
        {
          foreach (KeyValuePair<string, PropertyDefinition<TMetadataEntry>> additionalProperty in this.additionalProperties)
            this.propertyInfoMapBacking.Add(additionalProperty);
        }
        return this.propertyInfoMapBacking;
      }
    }

    public override bool CanConvert(Type objectType) => typeof (MetadataDocument<TMetadataEntryInterface>).IsAssignableFrom(objectType);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      MetadataDocument<TMetadataEntryInterface> doc = (MetadataDocument<TMetadataEntryInterface>) value;
      InternalMetadataDocumentProperties internalDocumentProperties = new InternalMetadataDocumentProperties(doc.Properties, this.CreateSourceChainMapDictionary(doc.Properties?.SourceChainMap));
      this.DocumentPropertyInfoMap["NameMetadata"].WriterAction = (Action<JsonWriter>) (jsonWriter => this.nameMetadataPropertyDefinition.WriteFunc(jsonWriter, this.requestItemsTelemetryCache, (IMetadataDocumentPropertiesWriteable) (doc.Properties as MetadataDocumentProperties), (IInternalMetadataDocumentProperties) internalDocumentProperties));
      this.DocumentPropertyInfoMap["Entries"].WriterAction = (Action<JsonWriter>) (jsonWriter =>
      {
        jsonWriter.WriteStartArray();
        foreach (TMetadataEntryInterface entry in doc.Entries)
        {
          TMetadataEntry metadataEntry = (TMetadataEntry) (object) entry;
          jsonWriter.WriteStartObject();
          jsonWriter.WritePropertyName("V");
          jsonWriter.WriteValue(metadataEntry.PackageIdentity.Version.DisplayVersion);
          jsonWriter.WritePropertyName("DispN");
          jsonWriter.WriteValue(metadataEntry.PackageIdentity.Name.DisplayName);
          foreach (KeyValuePair<string, PropertyDefinition<TMetadataEntry>> propertyInfo in (IEnumerable<KeyValuePair<string, PropertyDefinition<TMetadataEntry>>>) this.PropertyInfoMap)
          {
            string key = propertyInfo.Key;
            PropertyDefinition<TMetadataEntry> propertyDefinition = propertyInfo.Value;
            if (propertyDefinition.SkipSerializingCondition == null || !propertyDefinition.SkipSerializingCondition(metadataEntry))
            {
              jsonWriter.WritePropertyName(key);
              propertyDefinition.WriteFunc(jsonWriter, this.requestItemsTelemetryCache, metadataEntry, (IInternalMetadataDocumentProperties) internalDocumentProperties);
            }
          }
          jsonWriter.WriteEndObject();
        }
        jsonWriter.WriteEndArray();
      });
      writer.WriteStartObject();
      this.DocumentPropertyInfoMap["UpstreamsConfigurationHash"].WritePropertyAndValue(writer, (object) doc.Properties?.UpstreamsConfigurationHash);
      DocumentPropertyDefinition documentPropertyInfo1 = this.DocumentPropertyInfoMap["UpstreamsLastRefreshedUtc"];
      JsonWriter writer1 = writer;
      IMetadataDocumentProperties properties1 = doc.Properties;
      string str1;
      if (properties1 == null)
      {
        str1 = (string) null;
      }
      else
      {
        DateTime? lastRefreshedUtc = properties1.UpstreamsLastRefreshedUtc;
        ref DateTime? local = ref lastRefreshedUtc;
        str1 = local.HasValue ? local.GetValueOrDefault().ToString("u") : (string) null;
      }
      documentPropertyInfo1.WritePropertyAndValue(writer1, (object) str1);
      DocumentPropertyDefinition documentPropertyInfo2 = this.DocumentPropertyInfoMap["SourceChainMap"];
      JsonWriter writer2 = writer;
      IMetadataDocumentProperties properties2 = doc.Properties;
      string str2;
      if (properties2 == null)
      {
        str2 = (string) null;
      }
      else
      {
        List<List<UpstreamSourceInfo>> sourceChainMap = properties2.SourceChainMap;
        str2 = sourceChainMap != null ? sourceChainMap.Serialize<List<List<UpstreamSourceInfo>>>() : (string) null;
      }
      documentPropertyInfo2.WritePropertyAndValue(writer2, (object) str2);
      if (this.nameMetadataPropertyDefinition != null && (this.nameMetadataPropertyDefinition.SkipSerializingCondition == null || !this.nameMetadataPropertyDefinition.SkipSerializingCondition((IMetadataDocumentPropertiesWriteable) (doc.Properties as MetadataDocumentProperties))))
        this.DocumentPropertyInfoMap["NameMetadata"].ProcessWrite(writer);
      this.DocumentPropertyInfoMap["Entries"].ProcessWrite(writer);
      writer.WriteEndObject();
      object val;
      if (this.requestItemsTelemetryCache.TryGet("Packaging.Properties.MetadataWritesCount", out val))
        this.requestItemsTelemetryCache.Set("Packaging.Properties.MetadataWritesCount", (object) ((int) val + 1));
      else
        this.requestItemsTelemetryCache.Set("Packaging.Properties.MetadataWritesCount", (object) 1);
      this.requestItemsTelemetryCache.Set("Packaging.Properties.VersionsWritten", (object) doc.Entries.Count);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      MetadataDocumentProperties docProperties = new MetadataDocumentProperties();
      string upstreamsConfigurationHash = (string) null;
      DateTime? upstreamsLastRefreshedUtc = new DateTime?();
      List<List<UpstreamSourceInfo>> sourceChainMap = (List<List<UpstreamSourceInfo>>) null;
      List<TMetadataEntryInterface> entries = new List<TMetadataEntryInterface>();
      if (reader.TokenType == JsonToken.StartObject)
      {
        this.DocumentPropertyInfoMap["UpstreamsConfigurationHash"].ReaderAction = (Action<JsonReader>) (jsonReader => upstreamsConfigurationHash = jsonReader.ReadStringOrNull());
        this.DocumentPropertyInfoMap["UpstreamsLastRefreshedUtc"].ReaderAction = (Action<JsonReader>) (jsonReader =>
        {
          string val = jsonReader.ReadStringOrNull();
          if (val == null)
            return;
          upstreamsLastRefreshedUtc = new DateTime?(SerializationUtils.ReadDate(val));
        });
        this.DocumentPropertyInfoMap["SourceChainMap"].ReaderAction = (Action<JsonReader>) (jsonReader =>
        {
          string str = jsonReader.ReadStringOrNull();
          if (str == null)
            return;
          sourceChainMap = JsonConvert.DeserializeObject<List<List<UpstreamSourceInfo>>>(str);
        });
        this.DocumentPropertyInfoMap["Entries"].ReaderAction = (Action<JsonReader>) (jsonReader =>
        {
          jsonReader.Read();
          entries = this.ReadEntries(jsonReader, new MetadataDocumentProperties(sourceChainMap: sourceChainMap));
        });
        this.DocumentPropertyInfoMap["NameMetadata"].ReaderAction = (Action<JsonReader>) (jsonReader =>
        {
          if (this.nameMetadataPropertyDefinition == null)
            throw new InvalidDataException("NameMetadata");
          if (this.nameMetadataPropertyDefinition.ApplyUsingReaderFunc != null)
            this.nameMetadataPropertyDefinition.ApplyUsingReaderFunc((IMetadataDocumentPropertiesWriteable) docProperties, this.requestItemsTelemetryCache, reader);
          else
            this.nameMetadataPropertyDefinition.ApplyFunc((IMetadataDocumentPropertiesWriteable) docProperties, docProperties, this.requestItemsTelemetryCache, reader.Value);
        });
        foreach (string key1 in (IEnumerable<string>) this.DocumentPropertyInfoMap.Keys)
        {
          string key2 = reader.ReadPropertyNameOnly();
          if (key2 != null)
          {
            this.DocumentPropertyInfoMap[key2].ProcessRead(reader);
            if (key2 == "Entries")
              break;
          }
          else
            break;
        }
        IEnumerable<KeyValuePair<string, DocumentPropertyDefinition>> source = this.DocumentPropertyInfoMap.Where<KeyValuePair<string, DocumentPropertyDefinition>>((Func<KeyValuePair<string, DocumentPropertyDefinition>, bool>) (x => !x.Value.Optional && !x.Value.ReadProcessed));
        if (source.Any<KeyValuePair<string, DocumentPropertyDefinition>>())
          throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_MissingDocumentProperties((object) source.Select<KeyValuePair<string, DocumentPropertyDefinition>, string>((Func<KeyValuePair<string, DocumentPropertyDefinition>, string>) (x => x.Key))));
      }
      else
        entries = reader.TokenType == JsonToken.StartArray ? this.ReadEntries(reader, (MetadataDocumentProperties) null) : throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ExpectedStartOfArrayOrObject());
      IMetadataDocumentProperties properties = (IMetadataDocumentProperties) new MetadataDocumentProperties(this.packageNameRequest.PackageName, upstreamsConfigurationHash, upstreamsLastRefreshedUtc, sourceChainMap, docProperties.NameMetadata);
      return (object) new MetadataDocument<TMetadataEntryInterface>(entries, properties);
    }

    private List<TMetadataEntryInterface> ReadEntries(
      JsonReader reader,
      MetadataDocumentProperties documentProperties)
    {
      if (reader.TokenType != JsonToken.StartArray)
        throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ExpectedStartOfArray());
      List<TMetadataEntryInterface> metadataEntryInterfaceList = new List<TMetadataEntryInterface>();
      bool continueReading = true;
      int val1 = 0;
      int totalEntriesSeen = 0;
      int val2 = 0;
      int val3 = 0;
      int val4 = 0;
      while (continueReading)
      {
        TMetadataEntry metadataEntry = this.ReadEntry(reader, documentProperties, out continueReading, ref totalEntriesSeen);
        if ((object) metadataEntry != null)
        {
          ++val1;
          if (metadataEntry.IsLocal)
            ++val3;
          if (this.MatchesFilter(metadataEntry))
          {
            if (metadataEntry.IsLocal)
              ++val4;
            ++val2;
            metadataEntryInterfaceList.Add((TMetadataEntryInterface) metadataEntry);
          }
        }
      }
      this.requestItemsTelemetryCache.Set("Packaging.Properties.LocalVersionsRead", (object) val3);
      this.requestItemsTelemetryCache.Set("Packaging.Properties.LocalVersionsMatched", (object) val4);
      this.requestItemsTelemetryCache.Set("Packaging.Properties.VersionsRead", (object) val1);
      this.requestItemsTelemetryCache.Set("Packaging.Properties.VersionsSeen", (object) totalEntriesSeen);
      this.requestItemsTelemetryCache.Set("Packaging.Properties.VersionsMatched", (object) val2);
      object val5;
      if (this.requestItemsTelemetryCache.TryGet("Packaging.Properties.MetadataReadsCount", out val5))
        this.requestItemsTelemetryCache.Set("Packaging.Properties.MetadataReadsCount", (object) ((int) val5 + 1));
      else
        this.requestItemsTelemetryCache.Set("Packaging.Properties.MetadataReadsCount", (object) 1);
      return metadataEntryInterfaceList;
    }

    private TMetadataEntry ReadEntry(
      JsonReader reader,
      MetadataDocumentProperties documentProperties,
      out bool continueReading,
      ref int totalEntriesSeen)
    {
      continueReading = true;
      TMetadataEntry metadataEntry1 = default (TMetadataEntry);
      reader.Read();
      if (reader.TokenType == JsonToken.EndArray)
      {
        continueReading = false;
        return default (TMetadataEntry);
      }
      ++totalEntriesSeen;
      int depth = reader.TokenType == JsonToken.StartObject ? reader.Depth : throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ExpectedStartOfObject());
      TPackageVersion other = this.versionParserFunc(reader.ReadExpectedStringProperty("V"));
      int? nullable1 = (this.queryOptions?.VersionLower as TPackageVersion)?.CompareTo(other);
      int? nullable2 = nullable1;
      int num1 = 0;
      if (nullable2.GetValueOrDefault() > num1 & nullable2.HasValue)
      {
        continueReading = false;
        return default (TMetadataEntry);
      }
      if (this.queryOptions?.VersionUpper is TPackageVersion versionUpper && other.CompareTo(versionUpper) > 0)
      {
        reader.MoveToDepth(depth);
        continueReading = true;
        return default (TMetadataEntry);
      }
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (ReadEntry)))
      {
        tracerBlock.TraceVerbose(string.Format("Reading entry: {0}", (object) other));
        TMetadataEntry metadataEntry2 = this.entryCreationFunc(this.nameParserFunc(reader.ReadExpectedStringProperty("DispN")), other);
        while (reader.Read() && reader.Depth > depth)
        {
          string serializedPropertyName = reader.TokenType == JsonToken.PropertyName ? reader.Value.ToString() : throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PropertiesExpectedInsideMetadata());
          tracerBlock.TraceVerbose("Reading property: " + serializedPropertyName);
          if (!reader.Read())
            throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ValueNotFoundForProperty((object) serializedPropertyName));
          this.ApplyChange(serializedPropertyName, metadataEntry2, documentProperties, reader);
        }
        foreach (PropertyDefinition<TMetadataEntry> propertyDefinition in this.PropertyInfoMap.Values.Where<PropertyDefinition<TMetadataEntry>>((Func<PropertyDefinition<TMetadataEntry>, bool>) (p => p.ApplyDefaultFunc != null)))
          propertyDefinition.ApplyDefaultFunc(metadataEntry2);
        if (reader.TokenType != JsonToken.EndObject)
          throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ExpectedLastTokenEndOfObject());
        int? nullable3 = nullable1;
        int num2 = 0;
        if (nullable3.GetValueOrDefault() == num2 & nullable3.HasValue)
          continueReading = false;
        FeedView view = this.packageNameRequest.Feed.View;
        if ((view != null ? (view.Type == FeedViewType.Implicit ? 1 : 0) : 0) != 0)
          metadataEntry2 = this.implicitViewDecoratingConverter.Convert(new FeedRequest<TMetadataEntry>((IFeedRequest) this.packageNameRequest, metadataEntry2));
        return metadataEntry2;
      }
    }

    private bool MatchesFilter(TMetadataEntry metadataEntry)
    {
      Guid? viewId = this.packageNameRequest.Feed.ViewId;
      if (viewId.HasValue && (((IMetadataEntryWritable) metadataEntry).Views == null || !((IMetadataEntryWritable) metadataEntry).Views.Contains<Guid>(viewId.Value)))
        return false;
      return this.queryOptions?.Filters == null || this.queryOptions.Filters.All<Func<TMetadataEntryInterface, bool>>((Func<Func<TMetadataEntryInterface, bool>, bool>) (filter => filter((TMetadataEntryInterface) metadataEntry)));
    }

    private void ApplyChange(
      string serializedPropertyName,
      TMetadataEntry metadataEntry,
      MetadataDocumentProperties documentProperties,
      JsonReader reader)
    {
      if (!this.IsProjected(serializedPropertyName))
      {
        reader.Skip();
      }
      else
      {
        PropertyDefinition<TMetadataEntry> propertyInfo = this.PropertyInfoMap[serializedPropertyName];
        if (propertyInfo.ApplyUsingReaderFunc != null)
          propertyInfo.ApplyUsingReaderFunc(metadataEntry, this.requestItemsTelemetryCache, reader);
        else
          propertyInfo.ApplyFunc(metadataEntry, documentProperties, this.requestItemsTelemetryCache, reader.Value);
      }
    }

    private bool IsProjected(string serializedPropertyName)
    {
      if (serializedPropertyName == "Views")
        return true;
      List<string> projections = this.queryOptions?.GetProjections();
      return projections == null || projections.Contains(this.ConvertSerializedNameToCodePropertyName(serializedPropertyName));
    }

    private string ConvertSerializedNameToCodePropertyName(string propertyName)
    {
      PropertyDefinition<TMetadataEntry> propertyDefinition;
      if (this.PropertyInfoMap.TryGetValue(propertyName, out propertyDefinition))
        return propertyDefinition.CodePropertyName;
      throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PropertyNotReadable((object) propertyName));
    }

    private IDictionary<string, int> CreateSourceChainMapDictionary(
      List<List<UpstreamSourceInfo>> sourceChainMap)
    {
      if (sourceChainMap == null)
        return (IDictionary<string, int>) null;
      Dictionary<string, int> chainMapDictionary = new Dictionary<string, int>();
      for (int index = 0; index < sourceChainMap.Count; ++index)
        chainMapDictionary.Add(UpstreamSourceInfoUtils.GetSourceChainKey((IEnumerable<UpstreamSourceInfo>) sourceChainMap[index]), index);
      return (IDictionary<string, int>) chainMapDictionary;
    }

    private List<UpstreamSourceInfo> GetEntrySourceChain(
      MetadataDocumentProperties documentProperties,
      int sourceChainIndex)
    {
      List<List<UpstreamSourceInfo>> source = documentProperties?.SourceChainMap ?? new List<List<UpstreamSourceInfo>>();
      if (sourceChainIndex < 0 || sourceChainIndex >= source.Count)
      {
        IEnumerable<string> values = source.Select<List<UpstreamSourceInfo>, string>((Func<List<UpstreamSourceInfo>, string>) (chain => string.Join<Guid>(",", chain.Select<UpstreamSourceInfo, Guid>((Func<UpstreamSourceInfo, Guid>) (s => s.Id)))));
        throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_SourceChainIndexOutOfBounds((object) sourceChainIndex, (object) ("[" + string.Join("],[", values) + "]")));
      }
      return source[sourceChainIndex];
    }

    private int? GetEntrySourceChainIndex(
      TMetadataEntry metadataEntry,
      IInternalMetadataDocumentProperties documentProperties)
    {
      if (((IMetadataEntryWritable) metadataEntry)?.SourceChain == null || !((IMetadataEntryWritable) metadataEntry).SourceChain.Any<UpstreamSourceInfo>())
        return new int?();
      string sourceChainKey = UpstreamSourceInfoUtils.GetSourceChainKey(((IMetadataEntryWritable) metadataEntry).SourceChain);
      if (!documentProperties.SourceChainMapDictionary.ContainsKey(sourceChainKey))
      {
        string str = documentProperties.SourceChainMapDictionary == null ? "null SourceChainMap" : string.Join("],[", (IEnumerable<string>) documentProperties.SourceChainMapDictionary.Keys);
        throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_SourceChainNotFoundInMap((object) ("[" + sourceChainKey + "]"), (object) ("[" + str + "]")));
      }
      return documentProperties?.SourceChainMapDictionary?[sourceChainKey];
    }
  }
}
