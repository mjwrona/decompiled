// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.User
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [JsonObject(MemberSerialization.OptIn)]
  [ExcludeFromCodeCoverage]
  [Entity("users", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.User", "Microsoft.DirectoryServices.User"})]
  public class User : DirectoryObject
  {
    private string _acceptedAs;
    private DateTime? _acceptedOn;
    private ChangeTrackingCollection<AlternativeSecurityId> _alternativeSecurityIds;
    private bool _alternativeSecurityIdsInitialized;
    private ChangeTrackingCollection<string> _alternativeSignInNames;
    private bool _alternativeSignInNamesInitialized;
    private AppMetadata _appMetadata;
    private bool _appMetadataInitialized;
    private string _cloudSecurityIdentifier;
    private string _creationType;
    private string _extensionAttribute1;
    private string _extensionAttribute2;
    private string _extensionAttribute3;
    private string _extensionAttribute4;
    private string _extensionAttribute5;
    private string _extensionAttribute6;
    private string _extensionAttribute7;
    private string _extensionAttribute8;
    private string _extensionAttribute9;
    private string _extensionAttribute10;
    private string _extensionAttribute11;
    private string _extensionAttribute12;
    private string _extensionAttribute13;
    private string _extensionAttribute14;
    private string _extensionAttribute15;
    private DateTime? _invitedOn;
    private ChangeTrackingCollection<KeyValue> _inviteReplyUrl;
    private bool _inviteReplyUrlInitialized;
    private ChangeTrackingCollection<string> _inviteResources;
    private bool _inviteResourcesInitialized;
    private ChangeTrackingCollection<InvitationTicket> _inviteTicket;
    private bool _inviteTicketInitialized;
    private bool? _isCompromised;
    private string _jrnlProxyAddress;
    private ChangeTrackingCollection<LogonIdentifier> _logonIdentifiers;
    private bool _logonIdentifiersInitialized;
    private string _netId;
    private string _primarySMTPAddress;
    private string _releaseTrack;
    private ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.SearchableDeviceKey> _searchableDeviceKey;
    private bool _searchableDeviceKeyInitialized;
    private SelfServePasswordResetData _selfServePasswordResetData;
    private bool _selfServePasswordResetDataInitialized;
    private ChangeTrackingCollection<string> _smtpAddresses;
    private bool _smtpAddressesInitialized;
    private string _userState;
    private DateTime? _userStateChangedOn;
    private ChangeTrackingCollection<GraphObject> _invitedBy;
    private bool _invitedByInitialized;
    private ChangeTrackingCollection<GraphObject> _invitedUsers;
    private bool _invitedUsersInitialized;
    private ChangeTrackingCollection<GraphObject> _pendingMemberOf;
    private bool _pendingMemberOfInitialized;
    private ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.ServiceInfo> _serviceInfo;
    private bool _serviceInfoInitialized;
    private ChangeTrackingCollection<ScopedRoleMembership> _scopedMemberOf;
    private bool _scopedMemberOfInitialized;
    private bool? _accountEnabled;
    private ChangeTrackingCollection<AssignedLicense> _assignedLicenses;
    private bool _assignedLicensesInitialized;
    private ChangeTrackingCollection<AssignedPlan> _assignedPlans;
    private bool _assignedPlansInitialized;
    private string _city;
    private string _country;
    private string _department;
    private bool? _dirSyncEnabled;
    private string _displayName;
    private string _facsimileTelephoneNumber;
    private string _givenName;
    private string _immutableId;
    private string _jobTitle;
    private DateTime? _lastDirSyncTime;
    private string _mail;
    private string _mailNickname;
    private string _mobile;
    private string _onPremisesSecurityIdentifier;
    private ChangeTrackingCollection<string> _otherMails;
    private bool _otherMailsInitialized;
    private string _passwordPolicies;
    private PasswordProfile _passwordProfile;
    private bool _passwordProfileInitialized;
    private string _physicalDeliveryOfficeName;
    private string _postalCode;
    private string _preferredLanguage;
    private ChangeTrackingCollection<ProvisionedPlan> _provisionedPlans;
    private bool _provisionedPlansInitialized;
    private ChangeTrackingCollection<ProvisioningError> _provisioningErrors;
    private bool _provisioningErrorsInitialized;
    private ChangeTrackingCollection<string> _proxyAddresses;
    private bool _proxyAddressesInitialized;
    private string _sipProxyAddress;
    private string _state;
    private string _streetAddress;
    private string _surname;
    private string _telephoneNumber;
    private Stream _thumbnailPhoto;
    private string _usageLocation;
    private string _userPrincipalName;
    private string _userType;
    private ChangeTrackingCollection<AppRoleAssignment> _appRoleAssignments;
    private bool _appRoleAssignmentsInitialized;
    private ChangeTrackingCollection<OAuth2PermissionGrant> _oauth2PermissionGrants;
    private bool _oauth2PermissionGrantsInitialized;
    private ChangeTrackingCollection<GraphObject> _ownedDevices;
    private bool _ownedDevicesInitialized;
    private ChangeTrackingCollection<GraphObject> _registeredDevices;
    private bool _registeredDevicesInitialized;

    [JsonProperty("acceptedAs")]
    public string AcceptedAs
    {
      get => this._acceptedAs;
      set
      {
        this._acceptedAs = value;
        this.ChangedProperties.Add(nameof (AcceptedAs));
      }
    }

    [JsonProperty("acceptedOn")]
    public DateTime? AcceptedOn
    {
      get => this._acceptedOn;
      set
      {
        this._acceptedOn = value;
        this.ChangedProperties.Add(nameof (AcceptedOn));
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

    [JsonProperty("alternativeSignInNames")]
    public ChangeTrackingCollection<string> AlternativeSignInNames
    {
      get
      {
        if (this._alternativeSignInNames == null)
          this._alternativeSignInNames = new ChangeTrackingCollection<string>();
        if (!this._alternativeSignInNamesInitialized)
        {
          this._alternativeSignInNames.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AlternativeSignInNames)));
          this._alternativeSignInNamesInitialized = true;
        }
        return this._alternativeSignInNames;
      }
      set
      {
        this._alternativeSignInNames = value;
        this.ChangedProperties.Add(nameof (AlternativeSignInNames));
      }
    }

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

    [JsonProperty("creationType")]
    public string CreationType
    {
      get => this._creationType;
      set
      {
        this._creationType = value;
        this.ChangedProperties.Add(nameof (CreationType));
      }
    }

    [JsonProperty("extensionAttribute1")]
    public string ExtensionAttribute1
    {
      get => this._extensionAttribute1;
      set
      {
        this._extensionAttribute1 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute1));
      }
    }

    [JsonProperty("extensionAttribute2")]
    public string ExtensionAttribute2
    {
      get => this._extensionAttribute2;
      set
      {
        this._extensionAttribute2 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute2));
      }
    }

    [JsonProperty("extensionAttribute3")]
    public string ExtensionAttribute3
    {
      get => this._extensionAttribute3;
      set
      {
        this._extensionAttribute3 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute3));
      }
    }

    [JsonProperty("extensionAttribute4")]
    public string ExtensionAttribute4
    {
      get => this._extensionAttribute4;
      set
      {
        this._extensionAttribute4 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute4));
      }
    }

    [JsonProperty("extensionAttribute5")]
    public string ExtensionAttribute5
    {
      get => this._extensionAttribute5;
      set
      {
        this._extensionAttribute5 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute5));
      }
    }

    [JsonProperty("extensionAttribute6")]
    public string ExtensionAttribute6
    {
      get => this._extensionAttribute6;
      set
      {
        this._extensionAttribute6 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute6));
      }
    }

    [JsonProperty("extensionAttribute7")]
    public string ExtensionAttribute7
    {
      get => this._extensionAttribute7;
      set
      {
        this._extensionAttribute7 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute7));
      }
    }

    [JsonProperty("extensionAttribute8")]
    public string ExtensionAttribute8
    {
      get => this._extensionAttribute8;
      set
      {
        this._extensionAttribute8 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute8));
      }
    }

    [JsonProperty("extensionAttribute9")]
    public string ExtensionAttribute9
    {
      get => this._extensionAttribute9;
      set
      {
        this._extensionAttribute9 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute9));
      }
    }

    [JsonProperty("extensionAttribute10")]
    public string ExtensionAttribute10
    {
      get => this._extensionAttribute10;
      set
      {
        this._extensionAttribute10 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute10));
      }
    }

    [JsonProperty("extensionAttribute11")]
    public string ExtensionAttribute11
    {
      get => this._extensionAttribute11;
      set
      {
        this._extensionAttribute11 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute11));
      }
    }

    [JsonProperty("extensionAttribute12")]
    public string ExtensionAttribute12
    {
      get => this._extensionAttribute12;
      set
      {
        this._extensionAttribute12 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute12));
      }
    }

    [JsonProperty("extensionAttribute13")]
    public string ExtensionAttribute13
    {
      get => this._extensionAttribute13;
      set
      {
        this._extensionAttribute13 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute13));
      }
    }

    [JsonProperty("extensionAttribute14")]
    public string ExtensionAttribute14
    {
      get => this._extensionAttribute14;
      set
      {
        this._extensionAttribute14 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute14));
      }
    }

    [JsonProperty("extensionAttribute15")]
    public string ExtensionAttribute15
    {
      get => this._extensionAttribute15;
      set
      {
        this._extensionAttribute15 = value;
        this.ChangedProperties.Add(nameof (ExtensionAttribute15));
      }
    }

    [JsonProperty("invitedOn")]
    public DateTime? InvitedOn
    {
      get => this._invitedOn;
      set
      {
        this._invitedOn = value;
        this.ChangedProperties.Add(nameof (InvitedOn));
      }
    }

    [JsonProperty("inviteReplyUrl")]
    public ChangeTrackingCollection<KeyValue> InviteReplyUrl
    {
      get
      {
        if (this._inviteReplyUrl == null)
          this._inviteReplyUrl = new ChangeTrackingCollection<KeyValue>();
        if (!this._inviteReplyUrlInitialized)
        {
          this._inviteReplyUrl.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (InviteReplyUrl)));
          this._inviteReplyUrl.ToList<KeyValue>().ForEach((Action<KeyValue>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (InviteReplyUrl)))));
          this._inviteReplyUrlInitialized = true;
        }
        return this._inviteReplyUrl;
      }
      set
      {
        this._inviteReplyUrl = value;
        this.ChangedProperties.Add(nameof (InviteReplyUrl));
      }
    }

    [JsonProperty("inviteResources")]
    public ChangeTrackingCollection<string> InviteResources
    {
      get
      {
        if (this._inviteResources == null)
          this._inviteResources = new ChangeTrackingCollection<string>();
        if (!this._inviteResourcesInitialized)
        {
          this._inviteResources.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (InviteResources)));
          this._inviteResourcesInitialized = true;
        }
        return this._inviteResources;
      }
      set
      {
        this._inviteResources = value;
        this.ChangedProperties.Add(nameof (InviteResources));
      }
    }

    [JsonProperty("inviteTicket")]
    public ChangeTrackingCollection<InvitationTicket> InviteTicket
    {
      get
      {
        if (this._inviteTicket == null)
          this._inviteTicket = new ChangeTrackingCollection<InvitationTicket>();
        if (!this._inviteTicketInitialized)
        {
          this._inviteTicket.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (InviteTicket)));
          this._inviteTicket.ToList<InvitationTicket>().ForEach((Action<InvitationTicket>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (InviteTicket)))));
          this._inviteTicketInitialized = true;
        }
        return this._inviteTicket;
      }
      set
      {
        this._inviteTicket = value;
        this.ChangedProperties.Add(nameof (InviteTicket));
      }
    }

    [JsonProperty("isCompromised")]
    public bool? IsCompromised
    {
      get => this._isCompromised;
      set
      {
        this._isCompromised = value;
        this.ChangedProperties.Add(nameof (IsCompromised));
      }
    }

    [JsonProperty("jrnlProxyAddress")]
    public string JrnlProxyAddress
    {
      get => this._jrnlProxyAddress;
      set
      {
        this._jrnlProxyAddress = value;
        this.ChangedProperties.Add(nameof (JrnlProxyAddress));
      }
    }

    [JsonProperty("logonIdentifiers")]
    public ChangeTrackingCollection<LogonIdentifier> LogonIdentifiers
    {
      get
      {
        if (this._logonIdentifiers == null)
          this._logonIdentifiers = new ChangeTrackingCollection<LogonIdentifier>();
        if (!this._logonIdentifiersInitialized)
        {
          this._logonIdentifiers.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (LogonIdentifiers)));
          this._logonIdentifiers.ToList<LogonIdentifier>().ForEach((Action<LogonIdentifier>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (LogonIdentifiers)))));
          this._logonIdentifiersInitialized = true;
        }
        return this._logonIdentifiers;
      }
      set
      {
        this._logonIdentifiers = value;
        this.ChangedProperties.Add(nameof (LogonIdentifiers));
      }
    }

    [JsonProperty("netId")]
    public string NetId
    {
      get => this._netId;
      set
      {
        this._netId = value;
        this.ChangedProperties.Add(nameof (NetId));
      }
    }

    [JsonProperty("primarySMTPAddress")]
    public string PrimarySMTPAddress
    {
      get => this._primarySMTPAddress;
      set
      {
        this._primarySMTPAddress = value;
        this.ChangedProperties.Add(nameof (PrimarySMTPAddress));
      }
    }

    [JsonProperty("releaseTrack")]
    public string ReleaseTrack
    {
      get => this._releaseTrack;
      set
      {
        this._releaseTrack = value;
        this.ChangedProperties.Add(nameof (ReleaseTrack));
      }
    }

    [JsonProperty("searchableDeviceKey")]
    public ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.SearchableDeviceKey> SearchableDeviceKey
    {
      get
      {
        if (this._searchableDeviceKey == null)
          this._searchableDeviceKey = new ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.SearchableDeviceKey>();
        if (!this._searchableDeviceKeyInitialized)
        {
          this._searchableDeviceKey.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (SearchableDeviceKey)));
          this._searchableDeviceKey.ToList<Microsoft.Azure.ActiveDirectory.GraphClient.SearchableDeviceKey>().ForEach((Action<Microsoft.Azure.ActiveDirectory.GraphClient.SearchableDeviceKey>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (SearchableDeviceKey)))));
          this._searchableDeviceKeyInitialized = true;
        }
        return this._searchableDeviceKey;
      }
      set
      {
        this._searchableDeviceKey = value;
        this.ChangedProperties.Add(nameof (SearchableDeviceKey));
      }
    }

    [JsonProperty("selfServePasswordResetData")]
    public SelfServePasswordResetData SelfServePasswordResetData
    {
      get
      {
        if (this._selfServePasswordResetData != null && !this._selfServePasswordResetDataInitialized)
        {
          this._selfServePasswordResetData.ItemChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (SelfServePasswordResetData)));
          this._selfServePasswordResetDataInitialized = true;
        }
        return this._selfServePasswordResetData;
      }
      set
      {
        this._selfServePasswordResetData = value;
        this.ChangedProperties.Add(nameof (SelfServePasswordResetData));
      }
    }

    [JsonProperty("smtpAddresses")]
    public ChangeTrackingCollection<string> SmtpAddresses
    {
      get
      {
        if (this._smtpAddresses == null)
          this._smtpAddresses = new ChangeTrackingCollection<string>();
        if (!this._smtpAddressesInitialized)
        {
          this._smtpAddresses.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (SmtpAddresses)));
          this._smtpAddressesInitialized = true;
        }
        return this._smtpAddresses;
      }
      set
      {
        this._smtpAddresses = value;
        this.ChangedProperties.Add(nameof (SmtpAddresses));
      }
    }

    [JsonProperty("userState")]
    public string UserState
    {
      get => this._userState;
      set
      {
        this._userState = value;
        this.ChangedProperties.Add(nameof (UserState));
      }
    }

    [JsonProperty("userStateChangedOn")]
    public DateTime? UserStateChangedOn
    {
      get => this._userStateChangedOn;
      set
      {
        this._userStateChangedOn = value;
        this.ChangedProperties.Add(nameof (UserStateChangedOn));
      }
    }

    [Link("invitedBy", false)]
    [JsonConverter(typeof (AadJsonConverter))]
    [JsonProperty("invitedBy")]
    public ChangeTrackingCollection<GraphObject> InvitedBy
    {
      get
      {
        if (this._invitedBy == null)
          this._invitedBy = new ChangeTrackingCollection<GraphObject>();
        if (!this._invitedByInitialized)
        {
          this._invitedBy.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (InvitedBy)));
          this._invitedByInitialized = true;
        }
        return this._invitedBy;
      }
      set
      {
        this._invitedBy = value;
        this.ChangedProperties.Add(nameof (InvitedBy));
      }
    }

    [Link("invitedUsers", false)]
    [JsonProperty("invitedUsers")]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<GraphObject> InvitedUsers
    {
      get
      {
        if (this._invitedUsers == null)
          this._invitedUsers = new ChangeTrackingCollection<GraphObject>();
        if (!this._invitedUsersInitialized)
        {
          this._invitedUsers.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (InvitedUsers)));
          this._invitedUsersInitialized = true;
        }
        return this._invitedUsers;
      }
      set
      {
        this._invitedUsers = value;
        this.ChangedProperties.Add(nameof (InvitedUsers));
      }
    }

    [Link("pendingMemberOf", false)]
    [JsonConverter(typeof (AadJsonConverter))]
    [JsonProperty("pendingMemberOf")]
    public ChangeTrackingCollection<GraphObject> PendingMemberOf
    {
      get
      {
        if (this._pendingMemberOf == null)
          this._pendingMemberOf = new ChangeTrackingCollection<GraphObject>();
        if (!this._pendingMemberOfInitialized)
        {
          this._pendingMemberOf.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (PendingMemberOf)));
          this._pendingMemberOfInitialized = true;
        }
        return this._pendingMemberOf;
      }
      set
      {
        this._pendingMemberOf = value;
        this.ChangedProperties.Add(nameof (PendingMemberOf));
      }
    }

    [Link("serviceInfo", false)]
    [JsonProperty("serviceInfo")]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.ServiceInfo> ServiceInfo
    {
      get
      {
        if (this._serviceInfo == null)
          this._serviceInfo = new ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.ServiceInfo>();
        if (!this._serviceInfoInitialized)
        {
          this._serviceInfo.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ServiceInfo)));
          this._serviceInfoInitialized = true;
        }
        return this._serviceInfo;
      }
      set
      {
        this._serviceInfo = value;
        this.ChangedProperties.Add(nameof (ServiceInfo));
      }
    }

    [Link("scopedMemberOf", false)]
    [JsonProperty("scopedMemberOf")]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<ScopedRoleMembership> ScopedMemberOf
    {
      get
      {
        if (this._scopedMemberOf == null)
          this._scopedMemberOf = new ChangeTrackingCollection<ScopedRoleMembership>();
        if (!this._scopedMemberOfInitialized)
        {
          this._scopedMemberOf.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ScopedMemberOf)));
          this._scopedMemberOfInitialized = true;
        }
        return this._scopedMemberOf;
      }
      set
      {
        this._scopedMemberOf = value;
        this.ChangedProperties.Add(nameof (ScopedMemberOf));
      }
    }

    public User() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.User";

    public User(string objectId)
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

    [JsonProperty("assignedLicenses")]
    public ChangeTrackingCollection<AssignedLicense> AssignedLicenses
    {
      get
      {
        if (this._assignedLicenses == null)
          this._assignedLicenses = new ChangeTrackingCollection<AssignedLicense>();
        if (!this._assignedLicensesInitialized)
        {
          this._assignedLicenses.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AssignedLicenses)));
          this._assignedLicenses.ToList<AssignedLicense>().ForEach((Action<AssignedLicense>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (AssignedLicenses)))));
          this._assignedLicensesInitialized = true;
        }
        return this._assignedLicenses;
      }
      set
      {
        this._assignedLicenses = value;
        this.ChangedProperties.Add(nameof (AssignedLicenses));
      }
    }

    [JsonProperty("assignedPlans")]
    public ChangeTrackingCollection<AssignedPlan> AssignedPlans
    {
      get
      {
        if (this._assignedPlans == null)
          this._assignedPlans = new ChangeTrackingCollection<AssignedPlan>();
        if (!this._assignedPlansInitialized)
        {
          this._assignedPlans.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AssignedPlans)));
          this._assignedPlans.ToList<AssignedPlan>().ForEach((Action<AssignedPlan>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (AssignedPlans)))));
          this._assignedPlansInitialized = true;
        }
        return this._assignedPlans;
      }
      set
      {
        this._assignedPlans = value;
        this.ChangedProperties.Add(nameof (AssignedPlans));
      }
    }

    [JsonProperty("city")]
    public string City
    {
      get => this._city;
      set
      {
        this._city = value;
        this.ChangedProperties.Add(nameof (City));
      }
    }

    [JsonProperty("country")]
    public string Country
    {
      get => this._country;
      set
      {
        this._country = value;
        this.ChangedProperties.Add(nameof (Country));
      }
    }

    [JsonProperty("department")]
    public string Department
    {
      get => this._department;
      set
      {
        this._department = value;
        this.ChangedProperties.Add(nameof (Department));
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

    [JsonProperty("facsimileTelephoneNumber")]
    public string FacsimileTelephoneNumber
    {
      get => this._facsimileTelephoneNumber;
      set
      {
        this._facsimileTelephoneNumber = value;
        this.ChangedProperties.Add(nameof (FacsimileTelephoneNumber));
      }
    }

    [JsonProperty("givenName")]
    public string GivenName
    {
      get => this._givenName;
      set
      {
        this._givenName = value;
        this.ChangedProperties.Add(nameof (GivenName));
      }
    }

    [JsonProperty("immutableId")]
    public string ImmutableId
    {
      get => this._immutableId;
      set
      {
        this._immutableId = value;
        this.ChangedProperties.Add(nameof (ImmutableId));
      }
    }

    [JsonProperty("jobTitle")]
    public string JobTitle
    {
      get => this._jobTitle;
      set
      {
        this._jobTitle = value;
        this.ChangedProperties.Add(nameof (JobTitle));
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

    [JsonProperty("mobile")]
    public string Mobile
    {
      get => this._mobile;
      set
      {
        this._mobile = value;
        this.ChangedProperties.Add(nameof (Mobile));
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

    [JsonProperty("otherMails")]
    public ChangeTrackingCollection<string> OtherMails
    {
      get
      {
        if (this._otherMails == null)
          this._otherMails = new ChangeTrackingCollection<string>();
        if (!this._otherMailsInitialized)
        {
          this._otherMails.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (OtherMails)));
          this._otherMailsInitialized = true;
        }
        return this._otherMails;
      }
      set
      {
        this._otherMails = value;
        this.ChangedProperties.Add(nameof (OtherMails));
      }
    }

    [JsonProperty("passwordPolicies")]
    public string PasswordPolicies
    {
      get => this._passwordPolicies;
      set
      {
        this._passwordPolicies = value;
        this.ChangedProperties.Add(nameof (PasswordPolicies));
      }
    }

    [JsonProperty("passwordProfile")]
    public PasswordProfile PasswordProfile
    {
      get
      {
        if (this._passwordProfile != null && !this._passwordProfileInitialized)
        {
          this._passwordProfile.ItemChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (PasswordProfile)));
          this._passwordProfileInitialized = true;
        }
        return this._passwordProfile;
      }
      set
      {
        this._passwordProfile = value;
        this.ChangedProperties.Add(nameof (PasswordProfile));
      }
    }

    [JsonProperty("physicalDeliveryOfficeName")]
    public string PhysicalDeliveryOfficeName
    {
      get => this._physicalDeliveryOfficeName;
      set
      {
        this._physicalDeliveryOfficeName = value;
        this.ChangedProperties.Add(nameof (PhysicalDeliveryOfficeName));
      }
    }

    [JsonProperty("postalCode")]
    public string PostalCode
    {
      get => this._postalCode;
      set
      {
        this._postalCode = value;
        this.ChangedProperties.Add(nameof (PostalCode));
      }
    }

    [JsonProperty("preferredLanguage")]
    public string PreferredLanguage
    {
      get => this._preferredLanguage;
      set
      {
        this._preferredLanguage = value;
        this.ChangedProperties.Add(nameof (PreferredLanguage));
      }
    }

    [JsonProperty("provisionedPlans")]
    public ChangeTrackingCollection<ProvisionedPlan> ProvisionedPlans
    {
      get
      {
        if (this._provisionedPlans == null)
          this._provisionedPlans = new ChangeTrackingCollection<ProvisionedPlan>();
        if (!this._provisionedPlansInitialized)
        {
          this._provisionedPlans.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ProvisionedPlans)));
          this._provisionedPlans.ToList<ProvisionedPlan>().ForEach((Action<ProvisionedPlan>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (ProvisionedPlans)))));
          this._provisionedPlansInitialized = true;
        }
        return this._provisionedPlans;
      }
      set
      {
        this._provisionedPlans = value;
        this.ChangedProperties.Add(nameof (ProvisionedPlans));
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

    [JsonProperty("sipProxyAddress")]
    public string SipProxyAddress
    {
      get => this._sipProxyAddress;
      set
      {
        this._sipProxyAddress = value;
        this.ChangedProperties.Add(nameof (SipProxyAddress));
      }
    }

    [JsonProperty("state")]
    public string State
    {
      get => this._state;
      set
      {
        this._state = value;
        this.ChangedProperties.Add(nameof (State));
      }
    }

    [JsonProperty("streetAddress")]
    public string StreetAddress
    {
      get => this._streetAddress;
      set
      {
        this._streetAddress = value;
        this.ChangedProperties.Add(nameof (StreetAddress));
      }
    }

    [JsonProperty("surname")]
    public string Surname
    {
      get => this._surname;
      set
      {
        this._surname = value;
        this.ChangedProperties.Add(nameof (Surname));
      }
    }

    [JsonProperty("telephoneNumber")]
    public string TelephoneNumber
    {
      get => this._telephoneNumber;
      set
      {
        this._telephoneNumber = value;
        this.ChangedProperties.Add(nameof (TelephoneNumber));
      }
    }

    [JsonProperty("thumbnailPhoto")]
    public Stream ThumbnailPhoto
    {
      get => this._thumbnailPhoto;
      set
      {
        this._thumbnailPhoto = value;
        this.ChangedProperties.Add(nameof (ThumbnailPhoto));
      }
    }

    [JsonProperty("usageLocation")]
    public string UsageLocation
    {
      get => this._usageLocation;
      set
      {
        this._usageLocation = value;
        this.ChangedProperties.Add(nameof (UsageLocation));
      }
    }

    [JsonProperty("userPrincipalName")]
    [Key(false)]
    public string UserPrincipalName
    {
      get => this._userPrincipalName;
      set
      {
        this._userPrincipalName = value;
        this.ChangedProperties.Add(nameof (UserPrincipalName));
      }
    }

    [JsonProperty("userType")]
    public string UserType
    {
      get => this._userType;
      set
      {
        this._userType = value;
        this.ChangedProperties.Add(nameof (UserType));
      }
    }

    [JsonProperty("appRoleAssignments")]
    [Link("appRoleAssignments", false)]
    [JsonConverter(typeof (AadJsonConverter))]
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

    [Link("oauth2PermissionGrants", false)]
    [JsonProperty("oauth2PermissionGrants")]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<OAuth2PermissionGrant> Oauth2PermissionGrants
    {
      get
      {
        if (this._oauth2PermissionGrants == null)
          this._oauth2PermissionGrants = new ChangeTrackingCollection<OAuth2PermissionGrant>();
        if (!this._oauth2PermissionGrantsInitialized)
        {
          this._oauth2PermissionGrants.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (Oauth2PermissionGrants)));
          this._oauth2PermissionGrantsInitialized = true;
        }
        return this._oauth2PermissionGrants;
      }
      set
      {
        this._oauth2PermissionGrants = value;
        this.ChangedProperties.Add(nameof (Oauth2PermissionGrants));
      }
    }

    [JsonProperty("ownedDevices")]
    [Link("ownedDevices", false)]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<GraphObject> OwnedDevices
    {
      get
      {
        if (this._ownedDevices == null)
          this._ownedDevices = new ChangeTrackingCollection<GraphObject>();
        if (!this._ownedDevicesInitialized)
        {
          this._ownedDevices.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (OwnedDevices)));
          this._ownedDevicesInitialized = true;
        }
        return this._ownedDevices;
      }
      set
      {
        this._ownedDevices = value;
        this.ChangedProperties.Add(nameof (OwnedDevices));
      }
    }

    [Link("registeredDevices", false)]
    [JsonProperty("registeredDevices")]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<GraphObject> RegisteredDevices
    {
      get
      {
        if (this._registeredDevices == null)
          this._registeredDevices = new ChangeTrackingCollection<GraphObject>();
        if (!this._registeredDevicesInitialized)
        {
          this._registeredDevices.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (RegisteredDevices)));
          this._registeredDevicesInitialized = true;
        }
        return this._registeredDevices;
      }
      set
      {
        this._registeredDevices = value;
        this.ChangedProperties.Add(nameof (RegisteredDevices));
      }
    }

    public override void ValidateProperties(bool isCreate)
    {
      base.ValidateProperties(isCreate);
      if (!isCreate)
        return;
      if (!this.ChangedProperties.Contains("AccountEnabled"))
        throw new PropertyValidationException("AccountEnabled should be specified.");
      if (this.ChangedProperties.Contains("AssignedLicenses"))
        throw new PropertyValidationException("AssignedLicense should not specified.");
    }
  }
}
