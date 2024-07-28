// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Metadata.NpmMetadataDocumentSerializerFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Views;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.Npm.Server.Metadata
{
  public class NpmMetadataDocumentSerializerFactory : 
    IFactory<PackageNameQuery<INpmMetadataEntry>, ISerializer<MetadataDocument<INpmMetadataEntry>>>
  {
    private readonly Dictionary<string, PropertyDefinition<INpmMetadataEntryWriteable>> NpmProperties;
    private readonly ICache<string, object> requestItemsAsCacheFacade;
    private readonly ITracerService tracerService;

    public NpmMetadataDocumentSerializerFactory(
      ICache<string, object> requestItemsAsCacheFacade,
      ITracerService tracerService)
    {
      this.requestItemsAsCacheFacade = requestItemsAsCacheFacade;
      this.tracerService = tracerService;
      this.NpmProperties = this.PopulateNpmProperties();
    }

    private void ReadManifestFiles(
      INpmMetadataEntryWriteable entry,
      ICache<string, object> telemetryCache,
      JsonReader reader)
    {
      if (reader.TokenType != JsonToken.StartObject)
        throw new InvalidDataException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_TokenNotAProperty((object) reader.TokenType));
      Dictionary<string, PackageFileMetadata> dictionary = new Dictionary<string, PackageFileMetadata>();
      while (true)
      {
        string key = reader.ReadPropertyNameOnly();
        if (key != null)
        {
          string valueIncludingAlgorithm = reader.ReadAsString();
          dictionary[key] = new PackageFileMetadata()
          {
            BlobIdentifier = BlobIdentifier.Deserialize(valueIncludingAlgorithm)
          };
        }
        else
          break;
      }
      entry.PackageManifest = new PackageManifest()
      {
        FilesMetadata = dictionary
      };
    }

    private void WriteManifestFiles(
      JsonWriter jsonWriter,
      ICache<string, object> telemetryCache,
      INpmMetadataEntryWriteable entry)
    {
      jsonWriter.WriteStartObject();
      foreach (KeyValuePair<string, PackageFileMetadata> keyValuePair in entry.PackageManifest.FilesMetadata)
      {
        jsonWriter.WritePropertyName(keyValuePair.Key);
        jsonWriter.WriteValue(keyValuePair.Value.BlobIdentifier.ValueString);
      }
      jsonWriter.WriteEndObject();
    }

    private void ReadJsonBytes(
      INpmMetadataEntryWriteable entry,
      ICache<string, object> telemetryCache,
      string strVal)
    {
      telemetryCache.Set("Packaging.Properties.MetadataLengthSample", (object) strVal.Length);
      entry.SetPackageJsonBytes(Convert.FromBase64String(strVal), true);
    }

    private void WriteJsonBytes(
      JsonWriter jsonWriter,
      ICache<string, object> telemetryCache,
      INpmMetadataEntryWriteable entry)
    {
      jsonWriter.WriteValue(Convert.ToBase64String(entry.PackageJsonContentBytes.Content));
    }

    private Dictionary<string, PropertyDefinition<INpmMetadataEntryWriteable>> PopulateNpmProperties()
    {
      Dictionary<string, PropertyDefinition<INpmMetadataEntryWriteable>> dictionary = new Dictionary<string, PropertyDefinition<INpmMetadataEntryWriteable>>();
      dictionary.Add("Files", new PropertyDefinition<INpmMetadataEntryWriteable>((Expression<Func<INpmMetadataEntryWriteable, object>>) (e => e.PackageManifest), (Action<JsonWriter, ICache<string, object>, INpmMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => this.WriteManifestFiles(w, tel, e)), (Action<INpmMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) null, new Action<INpmMetadataEntryWriteable, ICache<string, object>, JsonReader>(this.ReadManifestFiles), (Func<INpmMetadataEntryWriteable, bool>) (e => e.PackageManifest?.FilesMetadata == null || !e.PackageManifest.FilesMetadata.Any<KeyValuePair<string, PackageFileMetadata>>() || NpmMetadataDocumentSerializerFactory.SkipOnPermDeleted(e))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      dictionary.Add("Sha1", new PropertyDefinition<INpmMetadataEntryWriteable>((Expression<Func<INpmMetadataEntryWriteable, object>>) (e => e.PackageSha1Sum), (Action<JsonWriter, ICache<string, object>, INpmMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(e.PackageSha1Sum)), (Action<INpmMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.PackageSha1Sum = (string) val), skipSerializingCondition: NpmMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C0\u003E__SkipOnPermDeleted ?? (NpmMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C0\u003E__SkipOnPermDeleted = new Func<INpmMetadataEntryWriteable, bool>(NpmMetadataDocumentSerializerFactory.SkipOnPermDeleted))));
      dictionary.Add("Deprecated", new PropertyDefinition<INpmMetadataEntryWriteable>((Expression<Func<INpmMetadataEntryWriteable, object>>) (e => e.PackageManifest), (Action<JsonWriter, ICache<string, object>, INpmMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(e.Deprecated)), (Action<INpmMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.Deprecated = (string) val), skipSerializingCondition: (Func<INpmMetadataEntryWriteable, bool>) (e => e.Deprecated == null || NpmMetadataDocumentSerializerFactory.SkipOnPermDeleted(e))));
      dictionary.Add("GitHead", new PropertyDefinition<INpmMetadataEntryWriteable>((Expression<Func<INpmMetadataEntryWriteable, object>>) (e => e.GitHead), (Action<JsonWriter, ICache<string, object>, INpmMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => e.GitHead.WriteTo(w)), (Action<INpmMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) null, (Action<INpmMetadataEntryWriteable, ICache<string, object>, JsonReader>) ((e, tel, reader) =>
      {
        e.PackageJsonOptions = e.PackageJsonOptions?.Clone() ?? new PackageJsonOptions();
        e.PackageJsonOptions.GitHead = JToken.ReadFrom(reader);
      }), (Func<INpmMetadataEntryWriteable, bool>) (e => e.GitHead == null || NpmMetadataDocumentSerializerFactory.SkipOnPermDeleted(e))));
      dictionary.Add("Gyp", new PropertyDefinition<INpmMetadataEntryWriteable>((Expression<Func<INpmMetadataEntryWriteable, object>>) (e => (object) e.HasGypFileAtRoot), (Action<JsonWriter, ICache<string, object>, INpmMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(e.HasGypFileAtRoot)), (Action<INpmMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) =>
      {
        e.PackageJsonOptions = e.PackageJsonOptions?.Clone() ?? new PackageJsonOptions();
        e.PackageJsonOptions.ContainsBindingGypFileAtRoot = (bool) val;
      }), skipSerializingCondition: (Func<INpmMetadataEntryWriteable, bool>) (e => !e.HasGypFileAtRoot || NpmMetadataDocumentSerializerFactory.SkipOnPermDeleted(e))));
      dictionary.Add("ServerJs", new PropertyDefinition<INpmMetadataEntryWriteable>((Expression<Func<INpmMetadataEntryWriteable, object>>) (e => (object) e.HasServerJsAtRoot), (Action<JsonWriter, ICache<string, object>, INpmMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(e.HasServerJsAtRoot)), (Action<INpmMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) =>
      {
        e.PackageJsonOptions = e.PackageJsonOptions?.Clone() ?? new PackageJsonOptions();
        e.PackageJsonOptions.ContainsServerJsFileAtRoot = (bool) val;
      }), skipSerializingCondition: (Func<INpmMetadataEntryWriteable, bool>) (e => !e.HasServerJsAtRoot || NpmMetadataDocumentSerializerFactory.SkipOnPermDeleted(e))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      dictionary.Add("MD", new PropertyDefinition<INpmMetadataEntryWriteable>((Expression<Func<INpmMetadataEntryWriteable, object>>) (e => e.PackageJsonContentBytes), (Action<JsonWriter, ICache<string, object>, INpmMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => this.WriteJsonBytes(w, tel, e)), (Action<INpmMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => this.ReadJsonBytes(e, tel, (string) val)), skipSerializingCondition: NpmMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C0\u003E__SkipOnPermDeleted ?? (NpmMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C0\u003E__SkipOnPermDeleted = new Func<INpmMetadataEntryWriteable, bool>(NpmMetadataDocumentSerializerFactory.SkipOnPermDeleted))));
      return dictionary;
    }

    private static bool SkipOnPermDeleted(INpmMetadataEntryWriteable entry) => ((IMetadataEntryWritable) entry).PermanentDeletedDate.HasValue;

    public ISerializer<MetadataDocument<INpmMetadataEntry>> Get(
      PackageNameQuery<INpmMetadataEntry> queryRequest)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      StreamingPackageMetadataDocumentJsonConverter<SemanticVersion, INpmMetadataEntryWriteable, INpmMetadataEntry> documentJsonConverter = new StreamingPackageMetadataDocumentJsonConverter<SemanticVersion, INpmMetadataEntryWriteable, INpmMetadataEntry>(queryRequest.RequestData, (IConverter<FeedRequest<INpmMetadataEntryWriteable>, INpmMetadataEntryWriteable>) new LocalImplicitViewMetadataConverter<INpmMetadataEntryWriteable>(), queryRequest.Options, (Func<IPackageName, SemanticVersion, INpmMetadataEntryWriteable>) ((packageName, packageVersion) => (INpmMetadataEntryWriteable) new NpmMetadataEntry(new NpmPackageIdentity((NpmPackageName) packageName, packageVersion))), (Func<string, IPackageName>) (nameString => (IPackageName) new NpmPackageName(nameString)), NpmMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C1\u003E__ParseNpmPackageVersion ?? (NpmMetadataDocumentSerializerFactory.\u003C\u003EO.\u003C1\u003E__ParseNpmPackageVersion = new Func<string, SemanticVersion>(NpmVersionUtils.ParseNpmPackageVersion)), (IEnumerable<KeyValuePair<string, PropertyDefinition<INpmMetadataEntryWriteable>>>) this.NpmProperties, this.requestItemsAsCacheFacade, this.tracerService, this.NpmNameMetadataProperties);
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.Converters.Add((JsonConverter) documentJsonConverter);
      return (ISerializer<MetadataDocument<INpmMetadataEntry>>) new JsonSerializer<MetadataDocument<INpmMetadataEntry>>(settings);
    }

    private PropertyDefinition<IMetadataDocumentPropertiesWriteable> NpmNameMetadataProperties => new PropertyDefinition<IMetadataDocumentPropertiesWriteable>((Expression<Func<IMetadataDocumentPropertiesWriteable, object>>) (w => w.NameMetadata), (Action<JsonWriter, ICache<string, object>, IMetadataDocumentPropertiesWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => this.WriteNpmSharedPackageMetadata(w, tel, e)), (Action<IMetadataDocumentPropertiesWriteable, MetadataDocumentProperties, ICache<string, object>, object>) null, new Action<IMetadataDocumentPropertiesWriteable, ICache<string, object>, JsonReader>(this.ReadNpmSharedPackageMetadata), (Func<IMetadataDocumentPropertiesWriteable, bool>) (dp => !(dp?.NameMetadata is NpmLocalAndUpstreamSharedPackageMetadata)));

    private void WriteNpmSharedPackageMetadata(
      JsonWriter jsonWriter,
      ICache<string, object> tel,
      IMetadataDocumentPropertiesWriteable docProps)
    {
      NpmLocalAndUpstreamSharedPackageMetadata nameMetadata = (NpmLocalAndUpstreamSharedPackageMetadata) docProps.NameMetadata;
      jsonWriter.WriteStartObject();
      jsonWriter.WritePropertyName("tags");
      this.WriteDistTags(jsonWriter, nameMetadata.LocalDistributionTags);
      jsonWriter.WritePropertyName("upstreamTags");
      this.WriteDistTags(jsonWriter, nameMetadata.UpstreamDistributionTags);
      jsonWriter.WritePropertyName("rev");
      jsonWriter.WriteValue(nameMetadata.Revision);
      jsonWriter.WriteEndObject();
    }

    private void WriteDistTags(JsonWriter jsonWriter, IDictionary<string, string> distTagMap)
    {
      jsonWriter.WriteStartObject();
      if (distTagMap != null)
      {
        foreach (KeyValuePair<string, string> distTag in (IEnumerable<KeyValuePair<string, string>>) distTagMap)
        {
          jsonWriter.WritePropertyName(distTag.Key);
          jsonWriter.WriteValue(distTag.Value);
        }
      }
      jsonWriter.WriteEndObject();
    }

    private void ReadNpmSharedPackageMetadata(
      IMetadataDocumentPropertiesWriteable documentProperties,
      ICache<string, object> telCache,
      JsonReader reader)
    {
      NpmLocalAndUpstreamSharedPackageMetadata sharedPackageMetadata = new NpmLocalAndUpstreamSharedPackageMetadata();
      reader.Read();
      reader.ReadExpectedPropertyNameOnly("tags");
      sharedPackageMetadata.LocalDistributionTags = (IDictionary<string, string>) this.ReadDistTags(reader);
      reader.ReadExpectedPropertyNameOnly("upstreamTags");
      sharedPackageMetadata.UpstreamDistributionTags = (IDictionary<string, string>) this.ReadDistTags(reader);
      reader.ReadExpectedPropertyNameOnly("rev");
      sharedPackageMetadata.Revision = reader.ReadAsString();
      reader.Read();
      documentProperties.NameMetadata = (object) sharedPackageMetadata;
    }

    private Dictionary<string, string> ReadDistTags(JsonReader reader)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      reader.Read();
      while (true)
      {
        string key = reader.ReadPropertyNameOnly();
        if (key != null)
        {
          string str = reader.ReadAsString();
          dictionary[key] = str;
        }
        else
          break;
      }
      return dictionary;
    }
  }
}
