// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SnapshotContent
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class SnapshotContent : JsonSerializable
  {
    private Database databaseResource;
    private DocumentCollection collectionResource;
    private IList<string> partitionKeyRanges;
    private IList<PartitionKeyRange> partitionKeyRangeList;
    private SerializableNameValueCollection geoLinkIdToPKRangeRid;
    private IList<string> partitionKeyRangeResourceIds;
    private IList<string> dataDirectories;
    private IList<string> serializedClientEncryptionKeys;
    private IList<ClientEncryptionKey> clientEncryptionKeysList;

    [JsonConstructor]
    public SnapshotContent()
    {
    }

    internal SnapshotContent(
      OperationType operationType,
      string serializedDatabase,
      string serializedCollection,
      string serializedOffer,
      IList<string> serializedPkranges)
    {
      if (operationType == OperationType.Invalid)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (operationType)));
      this.ArgumentStringNotNullOrWhiteSpace(serializedDatabase, nameof (serializedDatabase));
      this.ArgumentStringNotNullOrWhiteSpace(serializedCollection, nameof (serializedCollection));
      this.ArgumentStringNotNullOrWhiteSpace(serializedOffer, nameof (serializedOffer));
      if (serializedPkranges == null || serializedPkranges.Count == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (serializedPkranges)));
      this.OperationType = operationType;
      this.SerializedDatabase = serializedDatabase;
      this.SerializedCollection = serializedCollection;
      this.SerializedOffer = serializedOffer;
      this.SerializedPartitionKeyRanges = serializedPkranges;
    }

    internal SnapshotContent(OperationType operationType, string serializedDatabase)
    {
      if (operationType == OperationType.Invalid)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (operationType)));
      this.ArgumentStringNotNullOrWhiteSpace(serializedDatabase, nameof (serializedDatabase));
      this.OperationType = operationType;
      this.SerializedDatabase = serializedDatabase;
    }

    internal SnapshotContent(
      OperationType operationType,
      string serializedDatabase,
      string serializedOffer,
      SerializableNameValueCollection geoLinkIdToPKRangeRid)
    {
      if (operationType == OperationType.Invalid)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (operationType)));
      this.ArgumentStringNotNullOrWhiteSpace(serializedDatabase, nameof (serializedDatabase));
      this.ArgumentStringNotNullOrWhiteSpace(serializedOffer, nameof (serializedOffer));
      if (geoLinkIdToPKRangeRid == null || geoLinkIdToPKRangeRid.Collection.Count == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (geoLinkIdToPKRangeRid)));
      this.OperationType = operationType;
      this.SerializedDatabase = serializedDatabase;
      this.SerializedOffer = serializedOffer;
      this.GeoLinkIdToPKRangeRid = geoLinkIdToPKRangeRid;
    }

    internal SnapshotContent(
      OperationType operationType,
      string serializedDatabase,
      string serializedCollection,
      IList<string> serializedPkranges,
      SerializableNameValueCollection geoLinkIdToPKRangeRid)
    {
      if (operationType == OperationType.Invalid)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (operationType)));
      this.ArgumentStringNotNullOrWhiteSpace(serializedDatabase, nameof (serializedDatabase));
      this.ArgumentStringNotNullOrWhiteSpace(serializedCollection, nameof (serializedCollection));
      if (serializedPkranges == null || serializedPkranges.Count == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (serializedPkranges)));
      if (geoLinkIdToPKRangeRid == null || geoLinkIdToPKRangeRid.Collection.Count == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (geoLinkIdToPKRangeRid)));
      this.OperationType = operationType;
      this.SerializedDatabase = serializedDatabase;
      this.SerializedCollection = serializedCollection;
      this.SerializedPartitionKeyRanges = serializedPkranges;
      this.GeoLinkIdToPKRangeRid = geoLinkIdToPKRangeRid;
    }

    [JsonProperty(PropertyName = "operationType")]
    public OperationType OperationType
    {
      get
      {
        string str = this.GetValue<string>("operationType");
        return string.IsNullOrEmpty(str) ? OperationType.Invalid : (OperationType) Enum.Parse(typeof (OperationType), str, true);
      }
      internal set => this.SetValue("operationType", (object) (int) value);
    }

    [JsonIgnore]
    public Database Database
    {
      get
      {
        if (this.databaseResource == null && this.SerializedDatabase != null)
          this.databaseResource = this.GetResourceIfDeserialized<Database>(this.SerializedDatabase);
        return this.databaseResource;
      }
    }

    [JsonIgnore]
    public DocumentCollection DocumentCollection
    {
      get
      {
        if (this.collectionResource == null && this.SerializedCollection != null)
          this.collectionResource = this.GetResourceIfDeserialized<DocumentCollection>(this.SerializedCollection);
        return this.collectionResource;
      }
    }

    [JsonIgnore]
    public IList<PartitionKeyRange> PartitionKeyRangeList
    {
      get
      {
        if (this.partitionKeyRangeList == null && this.SerializedPartitionKeyRanges != null)
        {
          this.partitionKeyRangeList = (IList<PartitionKeyRange>) new List<PartitionKeyRange>();
          foreach (string partitionKeyRange in (IEnumerable<string>) this.SerializedPartitionKeyRanges)
            this.partitionKeyRangeList.Add(this.GetResourceIfDeserialized<PartitionKeyRange>(partitionKeyRange));
        }
        return this.partitionKeyRangeList;
      }
    }

    [JsonProperty(PropertyName = "geoLinkIdToPKRangeRid")]
    public SerializableNameValueCollection GeoLinkIdToPKRangeRid
    {
      get
      {
        if (this.geoLinkIdToPKRangeRid == null)
        {
          this.geoLinkIdToPKRangeRid = this.GetObject<SerializableNameValueCollection>("geoLinkIdToPKRangeRid");
          if (this.geoLinkIdToPKRangeRid == null)
            this.geoLinkIdToPKRangeRid = new SerializableNameValueCollection();
        }
        return this.geoLinkIdToPKRangeRid;
      }
      internal set
      {
        this.geoLinkIdToPKRangeRid = value;
        this.SetObject<SerializableNameValueCollection>("geoLinkIdToPKRangeRid", value);
      }
    }

    [JsonProperty(PropertyName = "partitionKeyRangeResourceIds")]
    public IList<string> PartitionKeyRangeResourceIds
    {
      get
      {
        if (this.partitionKeyRangeResourceIds == null)
          this.partitionKeyRangeResourceIds = this.GetValue<IList<string>>("partitionKeyRangeResourceIds");
        return this.partitionKeyRangeResourceIds;
      }
      internal set
      {
        this.partitionKeyRangeResourceIds = value;
        this.SetValue("partitionKeyRangeResourceIds", (object) value);
      }
    }

    [JsonProperty(PropertyName = "dataDirectories")]
    public IList<string> DataDirectories
    {
      get
      {
        if (this.dataDirectories == null)
          this.dataDirectories = this.GetValue<IList<string>>("dataDirectories");
        return this.dataDirectories;
      }
      internal set
      {
        this.dataDirectories = value;
        this.SetValue("dataDirectories", (object) value);
      }
    }

    [JsonProperty(PropertyName = "metadataDirectory")]
    public string MetadataDirectory
    {
      get => this.GetValue<string>("metadataDirectory");
      internal set => this.SetValue("metadataDirectory", (object) value);
    }

    [JsonProperty(PropertyName = "databaseContent")]
    public string SerializedDatabase
    {
      get => this.GetValue<string>("databaseContent");
      internal set => this.SetValue("databaseContent", (object) value);
    }

    [JsonProperty(PropertyName = "offerContent")]
    public string SerializedOffer
    {
      get => this.GetValue<string>("offerContent");
      internal set => this.SetValue("offerContent", (object) value);
    }

    [JsonProperty(PropertyName = "collectionContent")]
    public string SerializedCollection
    {
      get => this.GetValue<string>("collectionContent");
      internal set => this.SetValue("collectionContent", (object) value);
    }

    [JsonProperty(PropertyName = "partitionKeyRanges")]
    public IList<string> SerializedPartitionKeyRanges
    {
      get
      {
        if (this.partitionKeyRanges == null)
          this.partitionKeyRanges = this.GetValue<IList<string>>("partitionKeyRanges");
        return this.partitionKeyRanges;
      }
      internal set
      {
        this.partitionKeyRanges = value;
        this.SetValue("partitionKeyRanges", (object) value);
      }
    }

    [JsonProperty(PropertyName = "clientEncryptionKeyResources")]
    public IList<string> SerializedClientEncryptionKeyResources
    {
      get
      {
        if (this.serializedClientEncryptionKeys == null)
          this.serializedClientEncryptionKeys = this.GetValue<IList<string>>("clientEncryptionKeyResources");
        return this.serializedClientEncryptionKeys;
      }
      internal set
      {
        this.serializedClientEncryptionKeys = value;
        this.SetValue("clientEncryptionKeyResources", (object) value);
      }
    }

    [JsonIgnore]
    public IList<ClientEncryptionKey> ClientEncryptionKeysList
    {
      get
      {
        if (this.clientEncryptionKeysList == null && this.SerializedClientEncryptionKeyResources != null)
        {
          this.clientEncryptionKeysList = (IList<ClientEncryptionKey>) new List<ClientEncryptionKey>();
          foreach (string encryptionKeyResource in (IEnumerable<string>) this.SerializedClientEncryptionKeyResources)
            this.clientEncryptionKeysList.Add(this.GetResourceIfDeserialized<ClientEncryptionKey>(encryptionKeyResource));
        }
        return this.clientEncryptionKeysList;
      }
    }

    internal override void OnSave()
    {
      base.OnSave();
      if (this.partitionKeyRangeResourceIds != null)
        this.SetValue("partitionKeyRangeResourceIds", (object) this.partitionKeyRangeResourceIds);
      if (this.dataDirectories != null)
        this.SetValue("dataDirectories", (object) this.dataDirectories);
      if (this.geoLinkIdToPKRangeRid == null)
        return;
      this.geoLinkIdToPKRangeRid.OnSave();
      this.SetObject<SerializableNameValueCollection>("geoLinkIdToPKRangeRid", this.geoLinkIdToPKRangeRid);
    }

    private T GetResourceIfDeserialized<T>(string body) where T : Resource, new()
    {
      try
      {
        using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
        {
          ITypeResolver<T> typeResolver = JsonSerializable.GetTypeResolver<T>();
          return JsonSerializable.LoadFrom<T>((Stream) memoryStream, typeResolver);
        }
      }
      catch (JsonException ex)
      {
        return default (T);
      }
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<IList<string>>("partitionKeyRangeResourceIds");
      this.GetValue<IList<string>>("dataDirectories");
    }

    private void ArgumentStringNotNullOrWhiteSpace(string parameter, string parameterName)
    {
      if (string.IsNullOrWhiteSpace(parameterName))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (parameterName)));
      if (string.IsNullOrWhiteSpace(parameter))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, parameterName));
    }
  }
}
