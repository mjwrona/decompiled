// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.StreamingPackageNamesDocumentJsonConverter`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class StreamingPackageNamesDocumentJsonConverter<TPackageName> : JsonConverter where TPackageName : IPackageName
  {
    private readonly Func<string, TPackageName> nameParsingFunc;
    private IDictionary<string, PropertyDefinition<PackageNameEntry<TPackageName>>> propertyInfoMapBacking;

    public StreamingPackageNamesDocumentJsonConverter(Func<string, TPackageName> nameParsingFunc) => this.nameParsingFunc = nameParsingFunc;

    private IDictionary<string, PropertyDefinition<PackageNameEntry<TPackageName>>> PropertyInfoMap
    {
      get
      {
        if (this.propertyInfoMapBacking != null)
          return this.propertyInfoMapBacking;
        Dictionary<string, PropertyDefinition<PackageNameEntry<TPackageName>>> dictionary = new Dictionary<string, PropertyDefinition<PackageNameEntry<TPackageName>>>();
        dictionary.Add("N", new PropertyDefinition<PackageNameEntry<TPackageName>>((Expression<Func<PackageNameEntry<TPackageName>, object>>) (e => (object) e.Name), (Action<JsonWriter, ICache<string, object>, PackageNameEntry<TPackageName>, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(e.Name.DisplayName)), (Action<PackageNameEntry<TPackageName>, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.Name = this.nameParsingFunc((string) val))));
        dictionary.Add("ModD", new PropertyDefinition<PackageNameEntry<TPackageName>>((Expression<Func<PackageNameEntry<TPackageName>, object>>) (e => (object) e.LastUpdatedDateTime), (Action<JsonWriter, ICache<string, object>, PackageNameEntry<TPackageName>, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(e.LastUpdatedDateTime.ToString("u"))), (Action<PackageNameEntry<TPackageName>, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.LastUpdatedDateTime = SerializationUtils.ReadDate((string) val))));
        this.propertyInfoMapBacking = (IDictionary<string, PropertyDefinition<PackageNameEntry<TPackageName>>>) dictionary;
        return this.propertyInfoMapBacking;
      }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      IReadOnlyList<PackageNameEntry<TPackageName>> packageNameEntryList = (IReadOnlyList<PackageNameEntry<TPackageName>>) value;
      writer.WriteStartArray();
      foreach (PackageNameEntry<TPackageName> packageNameEntry in (IEnumerable<PackageNameEntry<TPackageName>>) packageNameEntryList)
      {
        writer.WriteStartObject();
        foreach (KeyValuePair<string, PropertyDefinition<PackageNameEntry<TPackageName>>> propertyInfo in (IEnumerable<KeyValuePair<string, PropertyDefinition<PackageNameEntry<TPackageName>>>>) this.PropertyInfoMap)
        {
          string key = propertyInfo.Key;
          PropertyDefinition<PackageNameEntry<TPackageName>> propertyDefinition = propertyInfo.Value;
          if (propertyDefinition.SkipSerializingCondition == null || !propertyDefinition.SkipSerializingCondition(packageNameEntry))
          {
            writer.WritePropertyName(key);
            propertyDefinition.WriteFunc(writer, (ICache<string, object>) null, packageNameEntry, (IInternalMetadataDocumentProperties) null);
          }
        }
        writer.WriteEndObject();
      }
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      List<PackageNameEntry<TPackageName>> packageNameEntryList = new List<PackageNameEntry<TPackageName>>();
      if (reader.TokenType != JsonToken.StartArray)
        throw new InvalidDataException(Resources.Error_ExpectedStartOfArray());
      bool continueReading = true;
      while (continueReading)
      {
        PackageNameEntry<TPackageName> packageNameEntry = this.ReadEntry(reader, out continueReading);
        if (packageNameEntry != null)
          packageNameEntryList.Add(packageNameEntry);
      }
      return (object) packageNameEntryList;
    }

    private PackageNameEntry<TPackageName> ReadEntry(JsonReader reader, out bool continueReading)
    {
      continueReading = true;
      reader.Read();
      if (reader.TokenType == JsonToken.EndArray)
      {
        continueReading = false;
        return (PackageNameEntry<TPackageName>) null;
      }
      int num = reader.TokenType == JsonToken.StartObject ? reader.Depth : throw new InvalidDataException(Resources.Error_ExpectedStartOfObject());
      PackageNameEntry<TPackageName> entry = new PackageNameEntry<TPackageName>();
      while (reader.Read() && reader.Depth > num)
      {
        string serializedPropertyName = reader.TokenType == JsonToken.PropertyName ? reader.Value.ToString() : throw new InvalidDataException(Resources.Error_PropertiesExpectedInsideMetadata());
        if (!reader.Read())
          throw new ArgumentException(Resources.Error_ValueNotFoundForProperty((object) serializedPropertyName));
        this.ApplyChange(serializedPropertyName, entry, reader);
      }
      if (reader.TokenType != JsonToken.EndObject)
        throw new InvalidDataException(Resources.Error_ExpectedLastTokenEndOfObject());
      return entry;
    }

    private void ApplyChange(
      string serializedPropertyName,
      PackageNameEntry<TPackageName> entry,
      JsonReader reader)
    {
      PropertyDefinition<PackageNameEntry<TPackageName>> propertyInfo = this.PropertyInfoMap[serializedPropertyName];
      if (propertyInfo.ApplyUsingReaderFunc != null)
        propertyInfo.ApplyUsingReaderFunc(entry, (ICache<string, object>) null, reader);
      else
        propertyInfo.ApplyFunc(entry, (MetadataDocumentProperties) null, (ICache<string, object>) null, reader.Value);
    }

    public override bool CanConvert(Type objectType) => typeof (IReadOnlyList<PackageNameEntry<TPackageName>>).IsAssignableFrom(objectType);
  }
}
