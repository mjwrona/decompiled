// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.Device
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [JsonObject(MemberSerialization.OptIn)]
  [Entity("devices", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.Device", "Microsoft.DirectoryServices.Device"})]
  [ExcludeFromCodeCoverage]
  public class Device : DirectoryObject
  {
    private ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.DeviceKey> _deviceKey;
    private bool _deviceKeyInitialized;
    private ChangeTrackingCollection<string> _exchangeActiveSyncId;
    private bool _exchangeActiveSyncIdInitialized;
    private bool? _isCompliant;
    private bool? _isManaged;
    private ChangeTrackingCollection<KeyCredential> _keyCredentials;
    private bool _keyCredentialsInitialized;
    private string _onPremisesSecurityIdentifier;
    private bool? _accountEnabled;
    private ChangeTrackingCollection<AlternativeSecurityId> _alternativeSecurityIds;
    private bool _alternativeSecurityIdsInitialized;
    private DateTime? _approximateLastLogonTimestamp;
    private Guid? _deviceId;
    private string _deviceMetadata;
    private int? _deviceObjectVersion;
    private string _deviceOSType;
    private string _deviceOSVersion;
    private ChangeTrackingCollection<string> _devicePhysicalIds;
    private bool _devicePhysicalIdsInitialized;
    private string _deviceTrustType;
    private bool? _dirSyncEnabled;
    private string _displayName;
    private DateTime? _lastDirSyncTime;
    private ChangeTrackingCollection<GraphObject> _registeredOwners;
    private bool _registeredOwnersInitialized;
    private ChangeTrackingCollection<GraphObject> _registeredUsers;
    private bool _registeredUsersInitialized;

    [JsonProperty("deviceKey")]
    public ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.DeviceKey> DeviceKey
    {
      get
      {
        if (this._deviceKey == null)
          this._deviceKey = new ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.DeviceKey>();
        if (!this._deviceKeyInitialized)
        {
          this._deviceKey.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (DeviceKey)));
          this._deviceKey.ToList<Microsoft.Azure.ActiveDirectory.GraphClient.DeviceKey>().ForEach((Action<Microsoft.Azure.ActiveDirectory.GraphClient.DeviceKey>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (DeviceKey)))));
          this._deviceKeyInitialized = true;
        }
        return this._deviceKey;
      }
      set
      {
        this._deviceKey = value;
        this.ChangedProperties.Add(nameof (DeviceKey));
      }
    }

    [JsonProperty("exchangeActiveSyncId")]
    public ChangeTrackingCollection<string> ExchangeActiveSyncId
    {
      get
      {
        if (this._exchangeActiveSyncId == null)
          this._exchangeActiveSyncId = new ChangeTrackingCollection<string>();
        if (!this._exchangeActiveSyncIdInitialized)
        {
          this._exchangeActiveSyncId.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ExchangeActiveSyncId)));
          this._exchangeActiveSyncIdInitialized = true;
        }
        return this._exchangeActiveSyncId;
      }
      set
      {
        this._exchangeActiveSyncId = value;
        this.ChangedProperties.Add(nameof (ExchangeActiveSyncId));
      }
    }

    [JsonProperty("isCompliant")]
    public bool? IsCompliant
    {
      get => this._isCompliant;
      set
      {
        this._isCompliant = value;
        this.ChangedProperties.Add(nameof (IsCompliant));
      }
    }

    [JsonProperty("isManaged")]
    public bool? IsManaged
    {
      get => this._isManaged;
      set
      {
        this._isManaged = value;
        this.ChangedProperties.Add(nameof (IsManaged));
      }
    }

    [JsonProperty("keyCredentials")]
    public ChangeTrackingCollection<KeyCredential> KeyCredentials
    {
      get
      {
        if (this._keyCredentials == null)
          this._keyCredentials = new ChangeTrackingCollection<KeyCredential>();
        if (!this._keyCredentialsInitialized)
        {
          this._keyCredentials.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (KeyCredentials)));
          this._keyCredentials.ToList<KeyCredential>().ForEach((Action<KeyCredential>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (KeyCredentials)))));
          this._keyCredentialsInitialized = true;
        }
        return this._keyCredentials;
      }
      set
      {
        this._keyCredentials = value;
        this.ChangedProperties.Add(nameof (KeyCredentials));
      }
    }

    [JsonProperty("onPremisesSecurityIdentifier")]
    public string OnPremisesSecurityIdentifier
    {
      get => this._onPremisesSecurityIdentifier;
      set
      {
        this._onPremisesSecurityIdentifier = value;
        this.ChangedProperties.Add(nameof (OnPremisesSecurityIdentifier));
      }
    }

    public Device() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.Device";

    public Device(string objectId)
      : this()
    {
      this.ObjectId = objectId;
    }

    [JsonProperty("accountEnabled")]
    public bool? AccountEnabled
    {
      get => this._accountEnabled;
      set
      {
        this._accountEnabled = value;
        this.ChangedProperties.Add(nameof (AccountEnabled));
      }
    }

    [JsonProperty("alternativeSecurityIds")]
    public ChangeTrackingCollection<AlternativeSecurityId> AlternativeSecurityIds
    {
      get
      {
        if (this._alternativeSecurityIds == null)
          this._alternativeSecurityIds = new ChangeTrackingCollection<AlternativeSecurityId>();
        if (!this._alternativeSecurityIdsInitialized)
        {
          this._alternativeSecurityIds.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AlternativeSecurityIds)));
          this._alternativeSecurityIds.ToList<AlternativeSecurityId>().ForEach((Action<AlternativeSecurityId>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (AlternativeSecurityIds)))));
          this._alternativeSecurityIdsInitialized = true;
        }
        return this._alternativeSecurityIds;
      }
      set
      {
        this._alternativeSecurityIds = value;
        this.ChangedProperties.Add(nameof (AlternativeSecurityIds));
      }
    }

    [JsonProperty("approximateLastLogonTimestamp")]
    public DateTime? ApproximateLastLogonTimestamp
    {
      get => this._approximateLastLogonTimestamp;
      set
      {
        this._approximateLastLogonTimestamp = value;
        this.ChangedProperties.Add(nameof (ApproximateLastLogonTimestamp));
      }
    }

    [JsonProperty("deviceId")]
    public Guid? DeviceId
    {
      get => this._deviceId;
      set
      {
        this._deviceId = value;
        this.ChangedProperties.Add(nameof (DeviceId));
      }
    }

    [JsonProperty("deviceMetadata")]
    public string DeviceMetadata
    {
      get => this._deviceMetadata;
      set
      {
        this._deviceMetadata = value;
        this.ChangedProperties.Add(nameof (DeviceMetadata));
      }
    }

    [JsonProperty("deviceObjectVersion")]
    public int? DeviceObjectVersion
    {
      get => this._deviceObjectVersion;
      set
      {
        this._deviceObjectVersion = value;
        this.ChangedProperties.Add(nameof (DeviceObjectVersion));
      }
    }

    [JsonProperty("deviceOSType")]
    public string DeviceOSType
    {
      get => this._deviceOSType;
      set
      {
        this._deviceOSType = value;
        this.ChangedProperties.Add(nameof (DeviceOSType));
      }
    }

    [JsonProperty("deviceOSVersion")]
    public string DeviceOSVersion
    {
      get => this._deviceOSVersion;
      set
      {
        this._deviceOSVersion = value;
        this.ChangedProperties.Add(nameof (DeviceOSVersion));
      }
    }

    [JsonProperty("devicePhysicalIds")]
    public ChangeTrackingCollection<string> DevicePhysicalIds
    {
      get
      {
        if (this._devicePhysicalIds == null)
          this._devicePhysicalIds = new ChangeTrackingCollection<string>();
        if (!this._devicePhysicalIdsInitialized)
        {
          this._devicePhysicalIds.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (DevicePhysicalIds)));
          this._devicePhysicalIdsInitialized = true;
        }
        return this._devicePhysicalIds;
      }
      set
      {
        this._devicePhysicalIds = value;
        this.ChangedProperties.Add(nameof (DevicePhysicalIds));
      }
    }

    [JsonProperty("deviceTrustType")]
    public string DeviceTrustType
    {
      get => this._deviceTrustType;
      set
      {
        this._deviceTrustType = value;
        this.ChangedProperties.Add(nameof (DeviceTrustType));
      }
    }

    [JsonProperty("dirSyncEnabled")]
    public bool? DirSyncEnabled
    {
      get => this._dirSyncEnabled;
      set
      {
        this._dirSyncEnabled = value;
        this.ChangedProperties.Add(nameof (DirSyncEnabled));
      }
    }

    [JsonProperty("displayName")]
    public string DisplayName
    {
      get => this._displayName;
      set
      {
        this._displayName = value;
        this.ChangedProperties.Add(nameof (DisplayName));
      }
    }

    [JsonProperty("lastDirSyncTime")]
    public DateTime? LastDirSyncTime
    {
      get => this._lastDirSyncTime;
      set
      {
        this._lastDirSyncTime = value;
        this.ChangedProperties.Add(nameof (LastDirSyncTime));
      }
    }

    [Link("registeredOwners", false)]
    [JsonProperty("registeredOwners")]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<GraphObject> RegisteredOwners
    {
      get
      {
        if (this._registeredOwners == null)
          this._registeredOwners = new ChangeTrackingCollection<GraphObject>();
        if (!this._registeredOwnersInitialized)
        {
          this._registeredOwners.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (RegisteredOwners)));
          this._registeredOwnersInitialized = true;
        }
        return this._registeredOwners;
      }
      set
      {
        this._registeredOwners = value;
        this.ChangedProperties.Add(nameof (RegisteredOwners));
      }
    }

    [JsonProperty("registeredUsers")]
    [JsonConverter(typeof (AadJsonConverter))]
    [Link("registeredUsers", false)]
    public ChangeTrackingCollection<GraphObject> RegisteredUsers
    {
      get
      {
        if (this._registeredUsers == null)
          this._registeredUsers = new ChangeTrackingCollection<GraphObject>();
        if (!this._registeredUsersInitialized)
        {
          this._registeredUsers.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (RegisteredUsers)));
          this._registeredUsersInitialized = true;
        }
        return this._registeredUsers;
      }
      set
      {
        this._registeredUsers = value;
        this.ChangedProperties.Add(nameof (RegisteredUsers));
      }
    }
  }
}
