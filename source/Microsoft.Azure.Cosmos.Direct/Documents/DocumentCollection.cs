// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DocumentCollection
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
  internal class DocumentCollection : Resource
  {
    private IndexingPolicy indexingPolicy;
    private GeospatialConfig geospatialConfig;
    private PartitionKeyDefinition partitionKey;
    private SchemaDiscoveryPolicy schemaDiscoveryPolicy;
    private UniqueKeyPolicy uniqueKeyPolicy;
    private UniqueIndexReIndexContext uniqueIndexReIndexContext;
    private ConflictResolutionPolicy conflictResolutionPolicy;
    private ChangeFeedPolicy changeFeedPolicy;
    private CollectionBackupPolicy collectionBackupPolicy;
    private MaterializedViewDefinition materializedViewDefinition;
    private ByokConfig byokConfig;
    private ClientEncryptionPolicy clientEncryptionPolicy;
    private Collection<Microsoft.Azure.Documents.MaterializedViews> materializedViews;

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

    [JsonProperty(PropertyName = "materializedViewDefinition")]
    internal MaterializedViewDefinition MaterializedViewDefinition
    {
      get
      {
        if (this.materializedViewDefinition == null)
          this.materializedViewDefinition = this.GetObject<MaterializedViewDefinition>("materializedViewDefinition") ?? new MaterializedViewDefinition();
        return this.materializedViewDefinition;
      }
      set
      {
        this.materializedViewDefinition = value;
        this.SetObject<MaterializedViewDefinition>("materializedViewDefinition", value);
      }
    }

    [JsonProperty(PropertyName = "byokConfig")]
    internal ByokConfig ByokConfig
    {
      get
      {
        if (this.byokConfig == null)
          this.byokConfig = this.GetObject<ByokConfig>("byokConfig") ?? new ByokConfig();
        return this.byokConfig;
      }
      set
      {
        this.byokConfig = value;
        this.SetObject<ByokConfig>("byokConfig", value);
      }
    }

    [JsonProperty(PropertyName = "uniqueIndexNameEncodingMode")]
    internal byte UniqueIndexNameEncodingMode
    {
      get => this.GetValue<byte>("uniqueIndexNameEncodingMode");
      set => this.SetValue("uniqueIndexNameEncodingMode", (object) value);
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

    [JsonProperty(PropertyName = "uniqueIndexReIndexContext")]
    internal UniqueIndexReIndexContext UniqueIndexReIndexContext
    {
      get
      {
        if (this.uniqueIndexReIndexContext == null)
          this.uniqueIndexReIndexContext = this.GetObject<UniqueIndexReIndexContext>("uniqueIndexReIndexContext") ?? new UniqueIndexReIndexContext();
        return this.uniqueIndexReIndexContext;
      }
      set
      {
        this.uniqueIndexReIndexContext = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (UniqueIndexReIndexContext)));
        this.SetObject<UniqueIndexReIndexContext>("uniqueIndexReIndexContext", value);
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

    [Obsolete("PartitionKeyDeleteThroughputFraction is deprecated.")]
    [JsonProperty(PropertyName = "partitionKeyDeleteThroughputFraction")]
    public double PartitionKeyDeleteThroughputFraction
    {
      get => this.GetValue<double>("partitionKeyDeleteThroughputFraction");
      set => this.SetValue("partitionKeyDeleteThroughputFraction", (object) value);
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

    [JsonProperty(PropertyName = "changeFeedPolicy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
    internal ChangeFeedPolicy ChangeFeedPolicy
    {
      get
      {
        if (this.changeFeedPolicy == null)
          this.changeFeedPolicy = this.GetObject<ChangeFeedPolicy>("changeFeedPolicy");
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

    [JsonProperty(PropertyName = "allowMaterializedViews", NullValueHandling = NullValueHandling.Ignore)]
    internal bool? AllowMaterializedViews
    {
      get => this.GetValue<bool?>("allowMaterializedViews");
      set => this.SetValue("allowMaterializedViews", (object) value);
    }

    [JsonProperty(PropertyName = "backupPolicy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
    internal CollectionBackupPolicy CollectionBackupPolicy
    {
      get
      {
        if (this.collectionBackupPolicy == null)
          this.collectionBackupPolicy = this.GetObject<CollectionBackupPolicy>("backupPolicy");
        return this.collectionBackupPolicy;
      }
      set
      {
        this.collectionBackupPolicy = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (CollectionBackupPolicy)));
        this.SetObject<CollectionBackupPolicy>("backupPolicy", value);
      }
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

    internal bool IsMaterializedView() => this.MaterializedViewDefinition.SourceCollectionRid != null;

    [JsonProperty(PropertyName = "clientEncryptionPolicy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
    internal ClientEncryptionPolicy ClientEncryptionPolicy
    {
      get
      {
        if (this.clientEncryptionPolicy == null)
          this.clientEncryptionPolicy = this.GetObject<ClientEncryptionPolicy>("clientEncryptionPolicy");
        return this.clientEncryptionPolicy;
      }
      set
      {
        this.clientEncryptionPolicy = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (ClientEncryptionPolicy)));
        this.SetObject<ClientEncryptionPolicy>("clientEncryptionPolicy", value);
      }
    }

    [JsonProperty(PropertyName = "materializedViews", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    internal Collection<Microsoft.Azure.Documents.MaterializedViews> MaterializedViews
    {
      get
      {
        if (this.materializedViews == null)
          this.materializedViews = this.GetValueCollection<Microsoft.Azure.Documents.MaterializedViews>("materializedViews") ?? new Collection<Microsoft.Azure.Documents.MaterializedViews>();
        return this.materializedViews;
      }
      set
      {
        this.materializedViews = value;
        this.SetValueCollection<Microsoft.Azure.Documents.MaterializedViews>("materializedViews", value);
      }
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
      this.UniqueIndexReIndexContext.Validate();
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
      if (this.collectionBackupPolicy != null)
        this.SetObject<CollectionBackupPolicy>("backupPolicy", this.collectionBackupPolicy);
      if (this.geospatialConfig != null)
        this.SetObject<GeospatialConfig>("geospatialConfig", this.geospatialConfig);
      if (this.byokConfig != null)
        this.SetObject<ByokConfig>("byokConfig", this.byokConfig);
      if (this.uniqueIndexReIndexContext != null)
      {
        this.uniqueIndexReIndexContext.OnSave();
        this.SetObject<UniqueIndexReIndexContext>("uniqueIndexReIndexContext", this.uniqueIndexReIndexContext);
      }
      if (this.clientEncryptionPolicy != null)
        this.SetObject<ClientEncryptionPolicy>("clientEncryptionPolicy", this.clientEncryptionPolicy);
      if (this.materializedViews == null)
        return;
      this.SetValueCollection<Microsoft.Azure.Documents.MaterializedViews>("materializedViews", this.materializedViews);
    }
  }
}
