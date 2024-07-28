// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DocumentCollection
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  public class DocumentCollection : Resource
  {
    private IndexingPolicy indexingPolicy;
    private GeospatialConfig geospatialConfig;
    private PartitionKeyDefinition partitionKey;
    private SchemaDiscoveryPolicy schemaDiscoveryPolicy;
    private UniqueKeyPolicy uniqueKeyPolicy;
    private ConflictResolutionPolicy conflictResolutionPolicy;
    private ChangeFeedPolicy changeFeedPolicy;

    public IndexingPolicy IndexingPolicy
    {
      get
      {
        if (this.indexingPolicy == null)
          this.indexingPolicy = this.GetObject<IndexingPolicy>("indexingPolicy") ?? new IndexingPolicy();
        return this.indexingPolicy;
      }
      set
      {
        this.indexingPolicy = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (IndexingPolicy)));
        this.SetObject<IndexingPolicy>("indexingPolicy", value);
      }
    }

    [JsonProperty(PropertyName = "geospatialConfig")]
    public GeospatialConfig GeospatialConfig
    {
      get
      {
        if (this.geospatialConfig == null)
          this.geospatialConfig = this.GetObject<GeospatialConfig>("geospatialConfig") ?? new GeospatialConfig();
        return this.geospatialConfig;
      }
      set
      {
        this.geospatialConfig = value;
        this.SetObject<GeospatialConfig>("geospatialConfig", value);
      }
    }

    [JsonProperty(PropertyName = "_docs")]
    public string DocumentsLink => this.SelfLink.TrimEnd('/') + "/" + this.GetValue<string>("_docs");

    [JsonProperty(PropertyName = "_sprocs")]
    public string StoredProceduresLink => this.SelfLink.TrimEnd('/') + "/" + this.GetValue<string>("_sprocs");

    public string TriggersLink => this.SelfLink.TrimEnd('/') + "/" + this.GetValue<string>("_triggers");

    public string UserDefinedFunctionsLink => this.SelfLink.TrimEnd('/') + "/" + this.GetValue<string>("_udfs");

    public string ConflictsLink => this.SelfLink.TrimEnd('/') + "/" + this.GetValue<string>("_conflicts");

    [JsonProperty(PropertyName = "partitionKey")]
    public PartitionKeyDefinition PartitionKey
    {
      get
      {
        if (this.partitionKey == null)
          this.partitionKey = this.GetValue<PartitionKeyDefinition>("partitionKey") ?? new PartitionKeyDefinition();
        return this.partitionKey;
      }
      set
      {
        this.partitionKey = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (PartitionKey)));
        this.SetValue("partitionKey", (object) this.partitionKey);
      }
    }

    [JsonProperty(PropertyName = "defaultTtl", NullValueHandling = NullValueHandling.Ignore)]
    public int? DefaultTimeToLive
    {
      get => this.GetValue<int?>("defaultTtl");
      set => this.SetValue("defaultTtl", (object) value);
    }

    [JsonProperty(PropertyName = "ttlPropertyPath", NullValueHandling = NullValueHandling.Ignore)]
    public string TimeToLivePropertyPath
    {
      get => this.GetValue<string>("ttlPropertyPath");
      set => this.SetValue("ttlPropertyPath", (object) value);
    }

    internal SchemaDiscoveryPolicy SchemaDiscoveryPolicy
    {
      get
      {
        if (this.schemaDiscoveryPolicy == null)
          this.schemaDiscoveryPolicy = this.GetObject<SchemaDiscoveryPolicy>("schemaDiscoveryPolicy") ?? new SchemaDiscoveryPolicy();
        return this.schemaDiscoveryPolicy;
      }
      set
      {
        this.schemaDiscoveryPolicy = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (SchemaDiscoveryPolicy)));
        this.SetObject<SchemaDiscoveryPolicy>("schemaDiscoveryPolicy", value);
      }
    }

    [JsonProperty(PropertyName = "uniqueKeyPolicy")]
    public UniqueKeyPolicy UniqueKeyPolicy
    {
      get
      {
        if (this.uniqueKeyPolicy == null)
          this.uniqueKeyPolicy = this.GetObject<UniqueKeyPolicy>("uniqueKeyPolicy") ?? new UniqueKeyPolicy();
        return this.uniqueKeyPolicy;
      }
      set
      {
        this.uniqueKeyPolicy = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (UniqueKeyPolicy)));
        this.SetObject<UniqueKeyPolicy>("uniqueKeyPolicy", value);
      }
    }

    [JsonProperty(PropertyName = "conflictResolutionPolicy")]
    public ConflictResolutionPolicy ConflictResolutionPolicy
    {
      get
      {
        if (this.conflictResolutionPolicy == null)
          this.conflictResolutionPolicy = this.GetObject<ConflictResolutionPolicy>("conflictResolutionPolicy") ?? new ConflictResolutionPolicy();
        return this.conflictResolutionPolicy;
      }
      set
      {
        this.conflictResolutionPolicy = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (ConflictResolutionPolicy)));
        this.SetObject<ConflictResolutionPolicy>("conflictResolutionPolicy", value);
      }
    }

    [JsonIgnore]
    public IReadOnlyList<Microsoft.Azure.Documents.PartitionKeyRangeStatistics> PartitionKeyRangeStatistics => (IReadOnlyList<Microsoft.Azure.Documents.PartitionKeyRangeStatistics>) new JsonSerializableList<Microsoft.Azure.Documents.PartitionKeyRangeStatistics>((IEnumerable<Microsoft.Azure.Documents.PartitionKeyRangeStatistics>) this.StatisticsJRaw.Where<JRaw>((Func<JRaw, bool>) (jraw => jraw != null)).Select<JRaw, Microsoft.Azure.Documents.PartitionKeyRangeStatistics>((Func<JRaw, Microsoft.Azure.Documents.PartitionKeyRangeStatistics>) (jraw => JsonConvert.DeserializeObject<Microsoft.Azure.Documents.PartitionKeyRangeStatistics>((string) jraw.Value))).ToList<Microsoft.Azure.Documents.PartitionKeyRangeStatistics>());

    [JsonProperty(PropertyName = "statistics")]
    internal IReadOnlyList<JRaw> StatisticsJRaw
    {
      get => this.GetValue<IReadOnlyList<JRaw>>("statistics") ?? (IReadOnlyList<JRaw>) new Collection<JRaw>();
      set => this.SetValue("statistics", (object) value);
    }

    internal bool HasPartitionKey => this.partitionKey != null || this.GetValue<object>("partitionKey") != null;

    internal PartitionKeyInternal NonePartitionKeyValue => this.PartitionKey.Paths.Count == 0 || this.PartitionKey.IsSystemKey.GetValueOrDefault(false) ? PartitionKeyInternal.Empty : PartitionKeyInternal.Undefined;

    [JsonProperty(PropertyName = "changeFeedPolicy")]
    internal ChangeFeedPolicy ChangeFeedPolicy
    {
      get
      {
        if (this.changeFeedPolicy == null)
          this.changeFeedPolicy = this.GetObject<ChangeFeedPolicy>("changeFeedPolicy") ?? new ChangeFeedPolicy();
        return this.changeFeedPolicy;
      }
      set
      {
        this.changeFeedPolicy = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (ChangeFeedPolicy)));
        this.SetObject<ChangeFeedPolicy>("changeFeedPolicy", value);
      }
    }

    [JsonProperty(PropertyName = "analyticalStorageTtl", NullValueHandling = NullValueHandling.Ignore)]
    internal int? AnalyticalStorageTimeToLive
    {
      get => this.GetValue<int?>("analyticalStorageTtl");
      set => this.SetValue("analyticalStorageTtl", (object) value);
    }

    [JsonProperty(PropertyName = "schemaPolicy", NullValueHandling = NullValueHandling.Ignore)]
    internal JRaw SchemaPolicy
    {
      get => this.GetValue<JRaw>("schemaPolicy");
      set => this.SetValue("schemaPolicy", (object) value);
    }

    [JsonProperty(PropertyName = "internalSchemaProperties", NullValueHandling = NullValueHandling.Ignore)]
    internal InternalSchemaProperties InternalSchemaProperties
    {
      get => this.GetValue<InternalSchemaProperties>("internalSchemaProperties");
      set => this.SetValue("internalSchemaProperties", (object) value);
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<int?>("defaultTtl");
      this.GetValue<string>("ttlPropertyPath");
      this.IndexingPolicy.Validate();
      this.PartitionKey.Validate();
      this.UniqueKeyPolicy.Validate();
      this.ConflictResolutionPolicy.Validate();
    }

    internal override void OnSave()
    {
      if (this.indexingPolicy != null)
      {
        this.indexingPolicy.OnSave();
        this.SetObject<IndexingPolicy>("indexingPolicy", this.indexingPolicy);
      }
      if (this.partitionKey != null)
        this.SetValue("partitionKey", (object) this.partitionKey);
      if (this.uniqueKeyPolicy != null)
      {
        this.uniqueKeyPolicy.OnSave();
        this.SetObject<UniqueKeyPolicy>("uniqueKeyPolicy", this.uniqueKeyPolicy);
      }
      if (this.conflictResolutionPolicy != null)
      {
        this.conflictResolutionPolicy.OnSave();
        this.SetObject<ConflictResolutionPolicy>("conflictResolutionPolicy", this.conflictResolutionPolicy);
      }
      if (this.schemaDiscoveryPolicy != null)
        this.SetObject<SchemaDiscoveryPolicy>("schemaDiscoveryPolicy", this.schemaDiscoveryPolicy);
      if (this.changeFeedPolicy != null)
        this.SetObject<ChangeFeedPolicy>("changeFeedPolicy", this.changeFeedPolicy);
      if (this.geospatialConfig == null)
        return;
      this.SetObject<GeospatialConfig>("geospatialConfig", this.geospatialConfig);
    }
  }
}
