// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipal
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
  [Entity("servicePrincipals", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.ServicePrincipal", "Microsoft.DirectoryServices.ServicePrincipal"})]
  [ExcludeFromCodeCoverage]
  public class ServicePrincipal : DirectoryObject
  {
    private string _appCategory;
    private string _appData;
    private AppMetadata _appMetadata;
    private bool _appMetadataInitialized;
    private ServicePrincipalAuthenticationPolicy _authenticationPolicy;
    private bool _authenticationPolicyInitialized;
    private bool? _microsoftFirstParty;
    private bool? _useCustomTokenSigningKey;
    private ChangeTrackingCollection<GraphObject> _defaultPolicy;
    private bool _defaultPolicyInitialized;
    private bool? _accountEnabled;
    private string _appDisplayName;
    private string _appId;
    private Guid? _appOwnerTenantId;
    private bool? _appRoleAssignmentRequired;
    private ChangeTrackingCollection<AppRole> _appRoles;
    private bool _appRolesInitialized;
    private string _displayName;
    private string _errorUrl;
    private string _homepage;
    private ChangeTrackingCollection<KeyCredential> _keyCredentials;
    private bool _keyCredentialsInitialized;
    private string _logoutUrl;
    private ChangeTrackingCollection<OAuth2Permission> _oauth2Permissions;
    private bool _oauth2PermissionsInitialized;
    private ChangeTrackingCollection<PasswordCredential> _passwordCredentials;
    private bool _passwordCredentialsInitialized;
    private string _preferredTokenSigningKeyThumbprint;
    private string _publisherName;
    private ChangeTrackingCollection<string> _replyUrls;
    private bool _replyUrlsInitialized;
    private string _samlMetadataUrl;
    private ChangeTrackingCollection<string> _servicePrincipalNames;
    private bool _servicePrincipalNamesInitialized;
    private ChangeTrackingCollection<string> _tags;
    private bool _tagsInitialized;
    private ChangeTrackingCollection<AppRoleAssignment> _appRoleAssignedTo;
    private bool _appRoleAssignedToInitialized;
    private ChangeTrackingCollection<AppRoleAssignment> _appRoleAssignments;
    private bool _appRoleAssignmentsInitialized;
    private ChangeTrackingCollection<OAuth2PermissionGrant> _oauth2PermissionGrants;
    private bool _oauth2PermissionGrantsInitialized;

    [JsonProperty("appCategory")]
    public string AppCategory
    {
      get => this._appCategory;
      set
      {
        this._appCategory = value;
        this.ChangedProperties.Add(nameof (AppCategory));
      }
    }

    [JsonProperty("appData")]
    public string AppData
    {
      get => this._appData;
      set
      {
        this._appData = value;
        this.ChangedProperties.Add(nameof (AppData));
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

    [JsonProperty("authenticationPolicy")]
    public ServicePrincipalAuthenticationPolicy AuthenticationPolicy
    {
      get
      {
        if (this._authenticationPolicy != null && !this._authenticationPolicyInitialized)
        {
          this._authenticationPolicy.ItemChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AuthenticationPolicy)));
          this._authenticationPolicyInitialized = true;
        }
        return this._authenticationPolicy;
      }
      set
      {
        this._authenticationPolicy = value;
        this.ChangedProperties.Add(nameof (AuthenticationPolicy));
      }
    }

    [JsonProperty("microsoftFirstParty")]
    public bool? MicrosoftFirstParty
    {
      get => this._microsoftFirstParty;
      set
      {
        this._microsoftFirstParty = value;
        this.ChangedProperties.Add(nameof (MicrosoftFirstParty));
      }
    }

    [JsonProperty("useCustomTokenSigningKey")]
    public bool? UseCustomTokenSigningKey
    {
      get => this._useCustomTokenSigningKey;
      set
      {
        this._useCustomTokenSigningKey = value;
        this.ChangedProperties.Add(nameof (UseCustomTokenSigningKey));
      }
    }

    [Link("defaultPolicy", false)]
    [JsonConverter(typeof (AadJsonConverter))]
    [JsonProperty("defaultPolicy")]
    public ChangeTrackingCollection<GraphObject> DefaultPolicy
    {
      get
      {
        if (this._defaultPolicy == null)
          this._defaultPolicy = new ChangeTrackingCollection<GraphObject>();
        if (!this._defaultPolicyInitialized)
        {
          this._defaultPolicy.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (DefaultPolicy)));
          this._defaultPolicyInitialized = true;
        }
        return this._defaultPolicy;
      }
      set
      {
        this._defaultPolicy = value;
        this.ChangedProperties.Add(nameof (DefaultPolicy));
      }
    }

    public ServicePrincipal() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.ServicePrincipal";

    public ServicePrincipal(string objectId)
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

    [JsonProperty("appDisplayName")]
    public string AppDisplayName
    {
      get => this._appDisplayName;
      set
      {
        this._appDisplayName = value;
        this.ChangedProperties.Add(nameof (AppDisplayName));
      }
    }

    [JsonProperty("appId")]
    public string AppId
    {
      get => this._appId;
      set
      {
        this._appId = value;
        this.ChangedProperties.Add(nameof (AppId));
      }
    }

    [JsonProperty("appOwnerTenantId")]
    public Guid? AppOwnerTenantId
    {
      get => this._appOwnerTenantId;
      set
      {
        this._appOwnerTenantId = value;
        this.ChangedProperties.Add(nameof (AppOwnerTenantId));
      }
    }

    [JsonProperty("appRoleAssignmentRequired")]
    public bool? AppRoleAssignmentRequired
    {
      get => this._appRoleAssignmentRequired;
      set
      {
        this._appRoleAssignmentRequired = value;
        this.ChangedProperties.Add(nameof (AppRoleAssignmentRequired));
      }
    }

    [JsonProperty("appRoles")]
    public ChangeTrackingCollection<AppRole> AppRoles
    {
      get
      {
        if (this._appRoles == null)
          this._appRoles = new ChangeTrackingCollection<AppRole>();
        if (!this._appRolesInitialized)
        {
          this._appRoles.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AppRoles)));
          this._appRoles.ToList<AppRole>().ForEach((Action<AppRole>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (AppRoles)))));
          this._appRolesInitialized = true;
        }
        return this._appRoles;
      }
      set
      {
        this._appRoles = value;
        this.ChangedProperties.Add(nameof (AppRoles));
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

    [JsonProperty("errorUrl")]
    public string ErrorUrl
    {
      get => this._errorUrl;
      set
      {
        this._errorUrl = value;
        this.ChangedProperties.Add(nameof (ErrorUrl));
      }
    }

    [JsonProperty("homepage")]
    public string Homepage
    {
      get => this._homepage;
      set
      {
        this._homepage = value;
        this.ChangedProperties.Add(nameof (Homepage));
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

    [JsonProperty("logoutUrl")]
    public string LogoutUrl
    {
      get => this._logoutUrl;
      set
      {
        this._logoutUrl = value;
        this.ChangedProperties.Add(nameof (LogoutUrl));
      }
    }

    [JsonProperty("oauth2Permissions")]
    public ChangeTrackingCollection<OAuth2Permission> Oauth2Permissions
    {
      get
      {
        if (this._oauth2Permissions == null)
          this._oauth2Permissions = new ChangeTrackingCollection<OAuth2Permission>();
        if (!this._oauth2PermissionsInitialized)
        {
          this._oauth2Permissions.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (Oauth2Permissions)));
          this._oauth2Permissions.ToList<OAuth2Permission>().ForEach((Action<OAuth2Permission>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (Oauth2Permissions)))));
          this._oauth2PermissionsInitialized = true;
        }
        return this._oauth2Permissions;
      }
      set
      {
        this._oauth2Permissions = value;
        this.ChangedProperties.Add(nameof (Oauth2Permissions));
      }
    }

    [JsonProperty("passwordCredentials")]
    public ChangeTrackingCollection<PasswordCredential> PasswordCredentials
    {
      get
      {
        if (this._passwordCredentials == null)
          this._passwordCredentials = new ChangeTrackingCollection<PasswordCredential>();
        if (!this._passwordCredentialsInitialized)
        {
          this._passwordCredentials.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (PasswordCredentials)));
          this._passwordCredentials.ToList<PasswordCredential>().ForEach((Action<PasswordCredential>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (PasswordCredentials)))));
          this._passwordCredentialsInitialized = true;
        }
        return this._passwordCredentials;
      }
      set
      {
        this._passwordCredentials = value;
        this.ChangedProperties.Add(nameof (PasswordCredentials));
      }
    }

    [JsonProperty("preferredTokenSigningKeyThumbprint")]
    public string PreferredTokenSigningKeyThumbprint
    {
      get => this._preferredTokenSigningKeyThumbprint;
      set
      {
        this._preferredTokenSigningKeyThumbprint = value;
        this.ChangedProperties.Add(nameof (PreferredTokenSigningKeyThumbprint));
      }
    }

    [JsonProperty("publisherName")]
    public string PublisherName
    {
      get => this._publisherName;
      set
      {
        this._publisherName = value;
        this.ChangedProperties.Add(nameof (PublisherName));
      }
    }

    [JsonProperty("replyUrls")]
    public ChangeTrackingCollection<string> ReplyUrls
    {
      get
      {
        if (this._replyUrls == null)
          this._replyUrls = new ChangeTrackingCollection<string>();
        if (!this._replyUrlsInitialized)
        {
          this._replyUrls.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ReplyUrls)));
          this._replyUrlsInitialized = true;
        }
        return this._replyUrls;
      }
      set
      {
        this._replyUrls = value;
        this.ChangedProperties.Add(nameof (ReplyUrls));
      }
    }

    [JsonProperty("samlMetadataUrl")]
    public string SamlMetadataUrl
    {
      get => this._samlMetadataUrl;
      set
      {
        this._samlMetadataUrl = value;
        this.ChangedProperties.Add(nameof (SamlMetadataUrl));
      }
    }

    [JsonProperty("servicePrincipalNames")]
    public ChangeTrackingCollection<string> ServicePrincipalNames
    {
      get
      {
        if (this._servicePrincipalNames == null)
          this._servicePrincipalNames = new ChangeTrackingCollection<string>();
        if (!this._servicePrincipalNamesInitialized)
        {
          this._servicePrincipalNames.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ServicePrincipalNames)));
          this._servicePrincipalNamesInitialized = true;
        }
        return this._servicePrincipalNames;
      }
      set
      {
        this._servicePrincipalNames = value;
        this.ChangedProperties.Add(nameof (ServicePrincipalNames));
      }
    }

    [JsonProperty("tags")]
    public ChangeTrackingCollection<string> Tags
    {
      get
      {
        if (this._tags == null)
          this._tags = new ChangeTrackingCollection<string>();
        if (!this._tagsInitialized)
        {
          this._tags.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (Tags)));
          this._tagsInitialized = true;
        }
        return this._tags;
      }
      set
      {
        this._tags = value;
        this.ChangedProperties.Add(nameof (Tags));
      }
    }

    [Link("appRoleAssignedTo", false)]
    [JsonConverter(typeof (AadJsonConverter))]
    [JsonProperty("appRoleAssignedTo")]
    public ChangeTrackingCollection<AppRoleAssignment> AppRoleAssignedTo
    {
      get
      {
        if (this._appRoleAssignedTo == null)
          this._appRoleAssignedTo = new ChangeTrackingCollection<AppRoleAssignment>();
        if (!this._appRoleAssignedToInitialized)
        {
          this._appRoleAssignedTo.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AppRoleAssignedTo)));
          this._appRoleAssignedToInitialized = true;
        }
        return this._appRoleAssignedTo;
      }
      set
      {
        this._appRoleAssignedTo = value;
        this.ChangedProperties.Add(nameof (AppRoleAssignedTo));
      }
    }

    [JsonConverter(typeof (AadJsonConverter))]
    [JsonProperty("appRoleAssignments")]
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

    [JsonProperty("oauth2PermissionGrants")]
    [JsonConverter(typeof (AadJsonConverter))]
    [Link("oauth2PermissionGrants", false)]
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
  }
}
