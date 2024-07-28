// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.AccountProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Cosmos
{
  public class AccountProperties
  {
    private Collection<AccountRegion> readRegions;
    private Collection<AccountRegion> writeRegions;
    internal readonly Lazy<IDictionary<string, object>> QueryEngineConfigurationInternal;
    [JsonIgnore]
    private string id;

    internal AccountProperties() => this.QueryEngineConfigurationInternal = new Lazy<IDictionary<string, object>>((Func<IDictionary<string, object>>) (() => this.QueryStringToDictConverter()));

    [JsonIgnore]
    public IEnumerable<AccountRegion> WritableRegions => (IEnumerable<AccountRegion>) this.WriteLocationsInternal;

    [JsonIgnore]
    public IEnumerable<AccountRegion> ReadableRegions => (IEnumerable<AccountRegion>) this.ReadLocationsInternal;

    [JsonProperty(PropertyName = "id")]
    public string Id
    {
      get => this.id;
      internal set => this.id = value;
    }

    [JsonProperty(PropertyName = "_etag", NullValueHandling = NullValueHandling.Ignore)]
    public string ETag { get; internal set; }

    [JsonProperty(PropertyName = "_rid", NullValueHandling = NullValueHandling.Ignore)]
    internal string ResourceId { get; set; }

    [JsonProperty(PropertyName = "writableLocations")]
    internal Collection<AccountRegion> WriteLocationsInternal
    {
      get
      {
        if (this.writeRegions == null)
          this.writeRegions = new Collection<AccountRegion>();
        return this.writeRegions;
      }
      set => this.writeRegions = value;
    }

    [JsonProperty(PropertyName = "readableLocations")]
    internal Collection<AccountRegion> ReadLocationsInternal
    {
      get
      {
        if (this.readRegions == null)
          this.readRegions = new Collection<AccountRegion>();
        return this.readRegions;
      }
      set => this.readRegions = value;
    }

    internal long MaxMediaStorageUsageInMB { get; set; }

    internal long MediaStorageUsageInMB { get; set; }

    internal long ConsumedDocumentStorageInMB { get; set; }

    internal long ReservedDocumentStorageInMB { get; set; }

    internal long ProvisionedDocumentStorageInMB { get; set; }

    [JsonProperty(PropertyName = "userConsistencyPolicy")]
    public AccountConsistency Consistency { get; internal set; }

    [JsonProperty(PropertyName = "addresses")]
    internal string AddressesLink { get; set; }

    [JsonProperty(PropertyName = "userReplicationPolicy")]
    internal ReplicationPolicy ReplicationPolicy { get; set; }

    [JsonProperty(PropertyName = "systemReplicationPolicy")]
    internal ReplicationPolicy SystemReplicationPolicy { get; set; }

    [JsonProperty(PropertyName = "readPolicy")]
    internal ReadPolicy ReadPolicy { get; set; }

    internal IDictionary<string, object> QueryEngineConfiguration => this.QueryEngineConfigurationInternal.Value;

    [JsonProperty(PropertyName = "queryEngineConfiguration")]
    internal string QueryEngineConfigurationString { get; set; }

    [JsonProperty(PropertyName = "enableMultipleWriteLocations")]
    internal bool EnableMultipleWriteLocations { get; set; }

    private IDictionary<string, object> QueryStringToDictConverter() => !string.IsNullOrEmpty(this.QueryEngineConfigurationString) ? (IDictionary<string, object>) JsonConvert.DeserializeObject<Dictionary<string, object>>(this.QueryEngineConfigurationString) : (IDictionary<string, object>) new Dictionary<string, object>();

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}
