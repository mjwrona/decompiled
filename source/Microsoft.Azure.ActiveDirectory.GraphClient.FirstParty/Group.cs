// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.Group
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [ExcludeFromCodeCoverage]
  [Entity("groups", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.Group", "Microsoft.DirectoryServices.Group"})]
  [JsonObject(MemberSerialization.OptIn)]
  public class Group : DirectoryObject
  {
    private AppMetadata _appMetadata;
    private bool _appMetadataInitialized;
    private string _cloudSecurityIdentifier;
    private ChangeTrackingCollection<string> _exchangeResources;
    private bool _exchangeResourcesInitialized;
    private string _groupType;
    private bool? _isPublic;
    private ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.LicenseAssignment> _licenseAssignment;
    private bool _licenseAssignmentInitialized;
    private ChangeTrackingCollection<string> _sharepointResources;
    private bool _sharepointResourcesInitialized;
    private string _targetAddress;
    private string _wellKnownObject;
    private ChangeTrackingCollection<GraphObject> _allowAccessTo;
    private bool _allowAccessToInitialized;
    private ChangeTrackingCollection<GraphObject> _hasAccessTo;
    private bool _hasAccessToInitialized;
    private ChangeTrackingCollection<GraphObject> _pendingMembers;
    private bool _pendingMembersInitialized;
    private string _description;
    private bool? _dirSyncEnabled;
    private string _displayName;
    private DateTime? _lastDirSyncTime;
    private string _mail;
    private string _mailNickname;
    private bool? _mailEnabled;
    private string _onPremisesSecurityIdentifier;
    private ChangeTrackingCollection<ProvisioningError> _provisioningErrors;
    private bool _provisioningErrorsInitialized;
    private ChangeTrackingCollection<string> _proxyAddresses;
    private bool _proxyAddressesInitialized;
    private bool? _securityEnabled;
    private ChangeTrackingCollection<AppRoleAssignment> _appRoleAssignments;
    private bool _appRoleAssignmentsInitialized;

    [JsonProperty("appMetadata")]
    public AppMetadata AppMetadata
    {
      get
      {
        if (this._appMetadata != null && !this._appMetadataInitialized)
        {
          this._appMetadata.ItemChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AppMetadata)));
          this._appMetadataInitialized = true;
        }
        return this._appMetadata;
      }
      set
      {
        this._appMetadata = value;
        this.ChangedProperties.Add(nameof (AppMetadata));
      }
    }

    [JsonProperty("cloudSecurityIdentifier")]
    public string CloudSecurityIdentifier
    {
      get => this._cloudSecurityIdentifier;
      set
      {
        this._cloudSecurityIdentifier = value;
        this.ChangedProperties.Add(nameof (CloudSecurityIdentifier));
      }
    }

    [JsonProperty("exchangeResources")]
    public ChangeTrackingCollection<string> ExchangeResources
    {
      get
      {
        if (this._exchangeResources == null)
          this._exchangeResources = new ChangeTrackingCollection<string>();
        if (!this._exchangeResourcesInitialized)
        {
          this._exchangeResources.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ExchangeResources)));
          this._exchangeResourcesInitialized = true;
        }
        return this._exchangeResources;
      }
      set
      {
        this._exchangeResources = value;
        this.ChangedProperties.Add(nameof (ExchangeResources));
      }
    }

    [JsonProperty("groupType")]
    public string GroupType
    {
      get => this._groupType;
      set
      {
        this._groupType = value;
        this.ChangedProperties.Add(nameof (GroupType));
      }
    }

    [JsonProperty("isPublic")]
    public bool? IsPublic
    {
      get => this._isPublic;
      set
      {
        this._isPublic = value;
        this.ChangedProperties.Add(nameof (IsPublic));
      }
    }

    [JsonProperty("licenseAssignment")]
    public ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.LicenseAssignment> LicenseAssignment
    {
      get
      {
        if (this._licenseAssignment == null)
          this._licenseAssignment = new ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.LicenseAssignment>();
        if (!this._licenseAssignmentInitialized)
        {
          this._licenseAssignment.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (LicenseAssignment)));
          this._licenseAssignment.ToList<Microsoft.Azure.ActiveDirectory.GraphClient.LicenseAssignment>().ForEach((Action<Microsoft.Azure.ActiveDirectory.GraphClient.LicenseAssignment>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (LicenseAssignment)))));
          this._licenseAssignmentInitialized = true;
        }
        return this._licenseAssignment;
      }
      set
      {
        this._licenseAssignment = value;
        this.ChangedProperties.Add(nameof (LicenseAssignment));
      }
    }

    [JsonProperty("sharepointResources")]
    public ChangeTrackingCollection<string> SharepointResources
    {
      get
      {
        if (this._sharepointResources == null)
          this._sharepointResources = new ChangeTrackingCollection<string>();
        if (!this._sharepointResourcesInitialized)
        {
          this._sharepointResources.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (SharepointResources)));
          this._sharepointResourcesInitialized = true;
        }
        return this._sharepointResources;
      }
      set
      {
        this._sharepointResources = value;
        this.ChangedProperties.Add(nameof (SharepointResources));
      }
    }

    [JsonProperty("targetAddress")]
    public string TargetAddress
    {
      get => this._targetAddress;
      set
      {
        this._targetAddress = value;
        this.ChangedProperties.Add(nameof (TargetAddress));
      }
    }

    [JsonProperty("wellKnownObject")]
    public string WellKnownObject
    {
      get => this._wellKnownObject;
      set
      {
        this._wellKnownObject = value;
        this.ChangedProperties.Add(nameof (WellKnownObject));
      }
    }

    [Link("allowAccessTo", false)]
    [JsonConverter(typeof (AadJsonConverter))]
    [JsonProperty("allowAccessTo")]
    public ChangeTrackingCollection<GraphObject> AllowAccessTo
    {
      get
      {
        if (this._allowAccessTo == null)
          this._allowAccessTo = new ChangeTrackingCollection<GraphObject>();
        if (!this._allowAccessToInitialized)
        {
          this._allowAccessTo.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AllowAccessTo)));
          this._allowAccessToInitialized = true;
        }
        return this._allowAccessTo;
      }
      set
      {
        this._allowAccessTo = value;
        this.ChangedProperties.Add(nameof (AllowAccessTo));
      }
    }

    [JsonProperty("hasAccessTo")]
    [JsonConverter(typeof (AadJsonConverter))]
    [Link("hasAccessTo", false)]
    public ChangeTrackingCollection<GraphObject> HasAccessTo
    {
      get
      {
        if (this._hasAccessTo == null)
          this._hasAccessTo = new ChangeTrackingCollection<GraphObject>();
        if (!this._hasAccessToInitialized)
        {
          this._hasAccessTo.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (HasAccessTo)));
          this._hasAccessToInitialized = true;
        }
        return this._hasAccessTo;
      }
      set
      {
        this._hasAccessTo = value;
        this.ChangedProperties.Add(nameof (HasAccessTo));
      }
    }

    [Link("pendingMembers", false)]
    [JsonProperty("pendingMembers")]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<GraphObject> PendingMembers
    {
      get
      {
        if (this._pendingMembers == null)
          this._pendingMembers = new ChangeTrackingCollection<GraphObject>();
        if (!this._pendingMembersInitialized)
        {
          this._pendingMembers.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (PendingMembers)));
          this._pendingMembersInitialized = true;
        }
        return this._pendingMembers;
      }
      set
      {
        this._pendingMembers = value;
        this.ChangedProperties.Add(nameof (PendingMembers));
      }
    }

    public Group() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.Group";

    public Group(string objectId)
      : this()
    {
      this.ObjectId = objectId;
    }

    [JsonProperty("description")]
    public string Description
    {
      get => this._description;
      set
      {
        this._description = value;
        this.ChangedProperties.Add(nameof (Description));
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

    [JsonProperty("mail")]
    public string Mail
    {
      get => this._mail;
      set
      {
        this._mail = value;
        this.ChangedProperties.Add(nameof (Mail));
      }
    }

    [JsonProperty("mailNickname")]
    public string MailNickname
    {
      get => this._mailNickname;
      set
      {
        this._mailNickname = value;
        this.ChangedProperties.Add(nameof (MailNickname));
      }
    }

    [JsonProperty("mailEnabled")]
    public bool? MailEnabled
    {
      get => this._mailEnabled;
      set
      {
        this._mailEnabled = value;
        this.ChangedProperties.Add(nameof (MailEnabled));
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

    [JsonProperty("provisioningErrors")]
    public ChangeTrackingCollection<ProvisioningError> ProvisioningErrors
    {
      get
      {
        if (this._provisioningErrors == null)
          this._provisioningErrors = new ChangeTrackingCollection<ProvisioningError>();
        if (!this._provisioningErrorsInitialized)
        {
          this._provisioningErrors.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ProvisioningErrors)));
          this._provisioningErrors.ToList<ProvisioningError>().ForEach((Action<ProvisioningError>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (ProvisioningErrors)))));
          this._provisioningErrorsInitialized = true;
        }
        return this._provisioningErrors;
      }
      set
      {
        this._provisioningErrors = value;
        this.ChangedProperties.Add(nameof (ProvisioningErrors));
      }
    }

    [JsonProperty("proxyAddresses")]
    public ChangeTrackingCollection<string> ProxyAddresses
    {
      get
      {
        if (this._proxyAddresses == null)
          this._proxyAddresses = new ChangeTrackingCollection<string>();
        if (!this._proxyAddressesInitialized)
        {
          this._proxyAddresses.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ProxyAddresses)));
          this._proxyAddressesInitialized = true;
        }
        return this._proxyAddresses;
      }
      set
      {
        this._proxyAddresses = value;
        this.ChangedProperties.Add(nameof (ProxyAddresses));
      }
    }

    [JsonProperty("securityEnabled")]
    public bool? SecurityEnabled
    {
      get => this._securityEnabled;
      set
      {
        this._securityEnabled = value;
        this.ChangedProperties.Add(nameof (SecurityEnabled));
      }
    }

    [JsonProperty("appRoleAssignments")]
    [JsonConverter(typeof (AadJsonConverter))]
    [Link("appRoleAssignments", false)]
    public ChangeTrackingCollection<AppRoleAssignment> AppRoleAssignments
    {
      get
      {
        if (this._appRoleAssignments == null)
          this._appRoleAssignments = new ChangeTrackingCollection<AppRoleAssignment>();
        if (!this._appRoleAssignmentsInitialized)
        {
          this._appRoleAssignments.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AppRoleAssignments)));
          this._appRoleAssignmentsInitialized = true;
        }
        return this._appRoleAssignments;
      }
      set
      {
        this._appRoleAssignments = value;
        this.ChangedProperties.Add(nameof (AppRoleAssignments));
      }
    }

    public override void ValidateProperties(bool isCreate)
    {
      base.ValidateProperties(isCreate);
      if (this.MailEnabled.HasValue && this.MailEnabled.Value)
        throw new PropertyValidationException("MailEnabled groups cannot be created through Graph Api.");
    }
  }
}
