// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ContainerProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Cosmos
{
  public class ContainerProperties
  {
    private static readonly char[] partitionKeyTokenDelimeter = new char[1]
    {
      '/'
    };
    [JsonProperty(PropertyName = "changeFeedPolicy", NullValueHandling = NullValueHandling.Ignore)]
    private ChangeFeedPolicy changeFeedPolicyInternal;
    [JsonProperty(PropertyName = "indexingPolicy", NullValueHandling = NullValueHandling.Ignore)]
    private IndexingPolicy indexingPolicyInternal;
    [JsonProperty(PropertyName = "geospatialConfig", NullValueHandling = NullValueHandling.Ignore)]
    private GeospatialConfig geospatialConfigInternal;
    [JsonProperty(PropertyName = "uniqueKeyPolicy", NullValueHandling = NullValueHandling.Ignore)]
    private UniqueKeyPolicy uniqueKeyPolicyInternal;
    [JsonProperty(PropertyName = "conflictResolutionPolicy", NullValueHandling = NullValueHandling.Ignore)]
    private ConflictResolutionPolicy conflictResolutionInternal;
    [JsonProperty(PropertyName = "clientEncryptionPolicy", NullValueHandling = NullValueHandling.Ignore)]
    private ClientEncryptionPolicy clientEncryptionPolicyInternal;
    private IReadOnlyList<IReadOnlyList<string>> partitionKeyPathTokens;
    private string id;

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }

    public ContainerProperties()
    {
    }

    public ContainerProperties(string id, string partitionKeyPath)
    {
      this.Id = id;
      this.PartitionKeyPath = partitionKeyPath;
      this.ValidateRequiredProperties();
    }

    internal ContainerProperties(string id, IReadOnlyList<string> partitionKeyPaths)
    {
      this.Id = id;
      this.PartitionKeyPaths = partitionKeyPaths;
      this.ValidateRequiredProperties();
    }

    [JsonIgnore]
    public Microsoft.Azure.Cosmos.PartitionKeyDefinitionVersion? PartitionKeyDefinitionVersion
    {
      get
      {
        Microsoft.Azure.Documents.PartitionKeyDefinitionVersion? version = (Microsoft.Azure.Documents.PartitionKeyDefinitionVersion?) this.PartitionKey?.Version;
        return !version.HasValue ? new Microsoft.Azure.Cosmos.PartitionKeyDefinitionVersion?() : new Microsoft.Azure.Cosmos.PartitionKeyDefinitionVersion?((Microsoft.Azure.Cosmos.PartitionKeyDefinitionVersion) version.GetValueOrDefault());
      }
      set
      {
        if (this.PartitionKey == null)
          throw new ArgumentOutOfRangeException("PartitionKey is not defined for container");
        this.PartitionKey.Version = new Microsoft.Azure.Documents.PartitionKeyDefinitionVersion?((Microsoft.Azure.Documents.PartitionKeyDefinitionVersion) value.Value);
      }
    }

    [JsonIgnore]
    public ConflictResolutionPolicy ConflictResolutionPolicy
    {
      get
      {
        if (this.conflictResolutionInternal == null)
          this.conflictResolutionInternal = new ConflictResolutionPolicy();
        return this.conflictResolutionInternal;
      }
      set => this.conflictResolutionInternal = value ?? throw new ArgumentNullException(nameof (value));
    }

    [JsonProperty(PropertyName = "id")]
    public string Id
    {
      get => this.id;
      set => this.id = value ?? throw new ArgumentNullException(nameof (Id));
    }

    [JsonIgnore]
    public UniqueKeyPolicy UniqueKeyPolicy
    {
      get
      {
        if (this.uniqueKeyPolicyInternal == null)
          this.uniqueKeyPolicyInternal = new UniqueKeyPolicy();
        return this.uniqueKeyPolicyInternal;
      }
      set => this.uniqueKeyPolicyInternal = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    [JsonProperty(PropertyName = "_etag", NullValueHandling = NullValueHandling.Ignore)]
    public string ETag { get; private set; }

    [JsonProperty(PropertyName = "_ts", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof (UnixDateTimeConverter))]
    public DateTime? LastModified { get; private set; }

    [JsonIgnore]
    public ClientEncryptionPolicy ClientEncryptionPolicy
    {
      get => this.clientEncryptionPolicyInternal;
      set => this.clientEncryptionPolicyInternal = value;
    }

    [JsonIgnore]
    public IndexingPolicy IndexingPolicy
    {
      get
      {
        if (this.indexingPolicyInternal == null)
          this.indexingPolicyInternal = new IndexingPolicy();
        return this.indexingPolicyInternal;
      }
      set => this.indexingPolicyInternal = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    [JsonIgnore]
    internal ChangeFeedPolicy ChangeFeedPolicy
    {
      get
      {
        if (this.changeFeedPolicyInternal == null)
          this.changeFeedPolicyInternal = new ChangeFeedPolicy();
        return this.changeFeedPolicyInternal;
      }
      set => this.changeFeedPolicyInternal = value;
    }

    [JsonIgnore]
    public GeospatialConfig GeospatialConfig
    {
      get
      {
        if (this.geospatialConfigInternal == null)
          this.geospatialConfigInternal = new GeospatialConfig();
        return this.geospatialConfigInternal;
      }
      set => this.geospatialConfigInternal = value;
    }

    [JsonIgnore]
    public string PartitionKeyPath
    {
      get
      {
        if (this.PartitionKey?.Paths == null || this.PartitionKey.Paths.Count <= 0)
          return (string) null;
        return this.PartitionKey?.Paths[0];
      }
      set => this.PartitionKey = !string.IsNullOrEmpty(value) ? new PartitionKeyDefinition()
      {
        Paths = new Collection<string>() { value }
      } : throw new ArgumentNullException(nameof (PartitionKeyPath));
    }

    [JsonIgnore]
    internal IReadOnlyList<string> PartitionKeyPaths
    {
      get
      {
        PartitionKeyDefinition partitionKey = this.PartitionKey;
        return partitionKey == null ? (IReadOnlyList<string>) null : (IReadOnlyList<string>) partitionKey.Paths;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (PartitionKeyPaths));
        Collection<string> collection = new Collection<string>();
        foreach (string str in (IEnumerable<string>) value)
          collection.Add(str);
        this.PartitionKey = new PartitionKeyDefinition()
        {
          Paths = collection,
          Kind = PartitionKind.MultiHash,
          Version = new Microsoft.Azure.Documents.PartitionKeyDefinitionVersion?(Microsoft.Azure.Documents.PartitionKeyDefinitionVersion.V2)
        };
      }
    }

    [Obsolete]
    [JsonProperty(PropertyName = "ttlPropertyPath", NullValueHandling = NullValueHandling.Ignore)]
    public string TimeToLivePropertyPath { get; set; }

    [JsonProperty(PropertyName = "defaultTtl", NullValueHandling = NullValueHandling.Ignore)]
    public int? DefaultTimeToLive { get; set; }

    [JsonProperty(PropertyName = "analyticalStorageTtl", NullValueHandling = NullValueHandling.Ignore)]
    public int? AnalyticalStoreTimeToLiveInSeconds { get; set; }

    [JsonProperty(PropertyName = "_self", NullValueHandling = NullValueHandling.Ignore)]
    public string SelfLink { get; private set; }

    internal PartitionKeyInternal GetNoneValue()
    {
      if (this.PartitionKey == null)
        throw new ArgumentNullException("PartitionKey");
      if (this.PartitionKey.Paths.Count != 0)
      {
        bool? isSystemKey = this.PartitionKey.IsSystemKey;
        bool flag = true;
        if (!(isSystemKey.GetValueOrDefault() == flag & isSystemKey.HasValue))
          return PartitionKeyInternal.Undefined;
      }
      return PartitionKeyInternal.Empty;
    }

    internal static ContainerProperties CreateWithResourceId(string resourceId) => !string.IsNullOrEmpty(resourceId) ? new ContainerProperties()
    {
      ResourceId = resourceId
    } : throw new ArgumentNullException(nameof (resourceId));

    internal ContainerProperties(string id, PartitionKeyDefinition partitionKeyDefinition)
    {
      this.Id = id;
      this.PartitionKey = partitionKeyDefinition;
      this.ValidateRequiredProperties();
    }

    [JsonProperty(PropertyName = "partitionKey", NullValueHandling = NullValueHandling.Ignore)]
    internal PartitionKeyDefinition PartitionKey { get; set; } = new PartitionKeyDefinition();

    [JsonProperty(PropertyName = "_rid", NullValueHandling = NullValueHandling.Ignore)]
    internal string ResourceId { get; private set; }

    internal bool HasPartitionKey => this.PartitionKey != null;

    internal IReadOnlyList<IReadOnlyList<string>> PartitionKeyPathTokens
    {
      get
      {
        if (this.partitionKeyPathTokens != null)
          return this.partitionKeyPathTokens;
        if (this.PartitionKey == null)
          throw new ArgumentNullException("PartitionKey");
        if (this.PartitionKey.Paths.Count > 1 && this.PartitionKey.Kind != PartitionKind.MultiHash)
          throw new NotImplementedException("PartitionKey extraction with composite partition keys not supported.");
        if (this.PartitionKey.Kind != PartitionKind.MultiHash && this.PartitionKeyPath == null)
          throw new ArgumentOutOfRangeException("Container " + this.Id + " is not partitioned");
        List<IReadOnlyList<string>> stringListList = new List<IReadOnlyList<string>>();
        foreach (string path in this.PartitionKey?.Paths)
        {
          string[] collection = path.Split(ContainerProperties.partitionKeyTokenDelimeter, StringSplitOptions.RemoveEmptyEntries);
          stringListList.Add((IReadOnlyList<string>) new List<string>((IEnumerable<string>) collection));
        }
        this.partitionKeyPathTokens = (IReadOnlyList<IReadOnlyList<string>>) stringListList;
        return this.partitionKeyPathTokens;
      }
    }

    internal void ValidateRequiredProperties()
    {
      if (this.Id == null)
        throw new ArgumentNullException("Id");
      if (this.PartitionKey == null || this.PartitionKey.Paths.Count == 0)
        throw new ArgumentNullException("PartitionKey");
      if (this.indexingPolicyInternal != null && this.indexingPolicyInternal.IndexingMode != IndexingMode.None && this.indexingPolicyInternal.IncludedPaths.Count == 0 && this.indexingPolicyInternal.ExcludedPaths.Count == 0)
        this.indexingPolicyInternal.IncludedPaths.Add(new IncludedPath()
        {
          Path = "/*"
        });
      if (this.ClientEncryptionPolicy == null)
        return;
      this.ClientEncryptionPolicy.ValidatePartitionKeyPathsIfEncrypted(this.PartitionKeyPathTokens);
    }
  }
}
