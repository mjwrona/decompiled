// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DatabaseAccount
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents
{
  public class DatabaseAccount : Resource
  {
    private ReplicationPolicy replicationPolicy;
    private ConsistencyPolicy consistencyPolicy;
    private ReplicationPolicy systemReplicationPolicy;
    private ReadPolicy readPolicy;
    private Dictionary<string, object> queryEngineConfiguration;
    private Collection<DatabaseAccountLocation> readLocations;
    private Collection<DatabaseAccountLocation> writeLocations;

    internal DatabaseAccount() => this.SelfLink = string.Empty;

    [JsonProperty(PropertyName = "_dbs")]
    public string DatabasesLink
    {
      get => this.GetValue<string>("_dbs");
      internal set => this.SetValue("_dbs", (object) value);
    }

    [JsonProperty(PropertyName = "media")]
    public string MediaLink
    {
      get => this.GetValue<string>("media");
      internal set => this.SetValue("media", (object) value);
    }

    [JsonIgnore]
    public IEnumerable<DatabaseAccountLocation> WritableLocations => (IEnumerable<DatabaseAccountLocation>) this.WriteLocationsInternal;

    [JsonProperty(PropertyName = "writableLocations")]
    internal Collection<DatabaseAccountLocation> WriteLocationsInternal
    {
      get
      {
        if (this.writeLocations == null)
        {
          this.writeLocations = this.GetObjectCollection<DatabaseAccountLocation>("writableLocations");
          if (this.writeLocations == null)
          {
            this.writeLocations = new Collection<DatabaseAccountLocation>();
            this.SetObjectCollection<DatabaseAccountLocation>("writableLocations", this.writeLocations);
          }
        }
        return this.writeLocations;
      }
      set
      {
        this.writeLocations = value;
        this.SetObjectCollection<DatabaseAccountLocation>("writableLocations", value);
      }
    }

    [JsonIgnore]
    public IEnumerable<DatabaseAccountLocation> ReadableLocations => (IEnumerable<DatabaseAccountLocation>) this.ReadLocationsInternal;

    [JsonProperty(PropertyName = "readableLocations")]
    internal Collection<DatabaseAccountLocation> ReadLocationsInternal
    {
      get
      {
        if (this.readLocations == null)
        {
          this.readLocations = this.GetObjectCollection<DatabaseAccountLocation>("readableLocations");
          if (this.readLocations == null)
          {
            this.readLocations = new Collection<DatabaseAccountLocation>();
            this.SetObjectCollection<DatabaseAccountLocation>("readableLocations", this.readLocations);
          }
        }
        return this.readLocations;
      }
      set
      {
        this.readLocations = value;
        this.SetObjectCollection<DatabaseAccountLocation>("readableLocations", value);
      }
    }

    public long MaxMediaStorageUsageInMB { get; internal set; }

    public long MediaStorageUsageInMB { get; internal set; }

    internal long ConsumedDocumentStorageInMB { get; set; }

    internal long ReservedDocumentStorageInMB { get; set; }

    internal long ProvisionedDocumentStorageInMB { get; set; }

    public ConsistencyPolicy ConsistencyPolicy
    {
      get
      {
        if (this.consistencyPolicy == null)
        {
          this.consistencyPolicy = this.GetObject<ConsistencyPolicy>("userConsistencyPolicy");
          if (this.consistencyPolicy == null)
            this.consistencyPolicy = new ConsistencyPolicy();
        }
        return this.consistencyPolicy;
      }
    }

    [JsonProperty(PropertyName = "addresses")]
    internal string AddressesLink
    {
      get => this.GetValue<string>("addresses");
      set => this.SetValue("addresses", (object) value.ToString());
    }

    internal ReplicationPolicy ReplicationPolicy
    {
      get
      {
        if (this.replicationPolicy == null)
        {
          this.replicationPolicy = this.GetObject<ReplicationPolicy>("userReplicationPolicy");
          if (this.replicationPolicy == null)
            this.replicationPolicy = new ReplicationPolicy();
        }
        return this.replicationPolicy;
      }
    }

    internal ReplicationPolicy SystemReplicationPolicy
    {
      get
      {
        if (this.systemReplicationPolicy == null)
        {
          this.systemReplicationPolicy = this.GetObject<ReplicationPolicy>("systemReplicationPolicy");
          if (this.systemReplicationPolicy == null)
            this.systemReplicationPolicy = new ReplicationPolicy();
        }
        return this.systemReplicationPolicy;
      }
    }

    internal ReadPolicy ReadPolicy
    {
      get
      {
        if (this.readPolicy == null)
        {
          this.readPolicy = this.GetObject<ReadPolicy>("readPolicy");
          if (this.readPolicy == null)
            this.readPolicy = new ReadPolicy();
        }
        return this.readPolicy;
      }
    }

    internal IDictionary<string, object> QueryEngineConfiuration
    {
      get
      {
        if (this.queryEngineConfiguration == null)
        {
          string str = this.GetValue<string>("queryEngineConfiguration");
          if (!string.IsNullOrEmpty(str))
            this.queryEngineConfiguration = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);
          if (this.queryEngineConfiguration == null)
            this.queryEngineConfiguration = new Dictionary<string, object>();
        }
        return (IDictionary<string, object>) this.queryEngineConfiguration;
      }
    }

    internal bool EnableMultipleWriteLocations
    {
      get => this.GetValue<bool>("enableMultipleWriteLocations");
      set => this.SetValue("enableMultipleWriteLocations", (object) value);
    }

    internal override void OnSave()
    {
      if (this.replicationPolicy != null)
      {
        this.replicationPolicy.OnSave();
        this.SetObject<ReplicationPolicy>("userReplicationPolicy", this.replicationPolicy);
      }
      if (this.consistencyPolicy != null)
      {
        this.consistencyPolicy.OnSave();
        this.SetObject<ConsistencyPolicy>("userConsistencyPolicy", this.consistencyPolicy);
      }
      if (this.systemReplicationPolicy != null)
      {
        this.systemReplicationPolicy.OnSave();
        this.SetObject<ReplicationPolicy>("systemReplicationPolicy", this.systemReplicationPolicy);
      }
      if (this.readPolicy != null)
      {
        this.readPolicy.OnSave();
        this.SetObject<ReadPolicy>("readPolicy", this.readPolicy);
      }
      if (this.readLocations != null)
        this.SetObjectCollection<DatabaseAccountLocation>("readableLocations", this.readLocations);
      if (this.writeLocations != null)
        this.SetObjectCollection<DatabaseAccountLocation>("writableLocations", this.writeLocations);
      if (this.queryEngineConfiguration == null)
        return;
      this.SetValue("queryEngineConfiguration", (object) JsonConvert.SerializeObject((object) this.queryEngineConfiguration));
    }

    internal static DatabaseAccount CreateNewInstance() => new DatabaseAccount();
  }
}
