// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.Application
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
  [Entity("applications", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.Application", "Microsoft.DirectoryServices.Application"})]
  [JsonObject(MemberSerialization.OptIn)]
  [ExcludeFromCodeCoverage]
  public class Application : DirectoryObject
  {
    private bool? _allowActAsForAllClients;
    private string _appCategory;
    private string _appData;
    private AppMetadata _appMetadata;
    private bool _appMetadataInitialized;
    private ChangeTrackingCollection<GraphObject> _defaultPolicy;
    private bool _defaultPolicyInitialized;
    private string _appId;
    private ChangeTrackingCollection<AppRole> _appRoles;
    private bool _appRolesInitialized;
    private bool? _availableToOtherTenants;
    private string _displayName;
    private string _errorUrl;
    private string _groupMembershipClaims;
    private string _homepage;
    private ChangeTrackingCollection<string> _identifierUris;
    private bool _identifierUrisInitialized;
    private ChangeTrackingCollection<KeyCredential> _keyCredentials;
    private bool _keyCredentialsInitialized;
    private ChangeTrackingCollection<Guid> _knownClientApplications;
    private bool _knownClientApplicationsInitialized;
    private Stream _mainLogo;
    private string _logoutUrl;
    private bool? _oauth2AllowImplicitFlow;
    private bool? _oauth2AllowUrlPathMatching;
    private ChangeTrackingCollection<OAuth2Permission> _oauth2Permissions;
    private bool _oauth2PermissionsInitialized;
    private bool? _oauth2RequirePostResponse;
    private ChangeTrackingCollection<PasswordCredential> _passwordCredentials;
    private bool _passwordCredentialsInitialized;
    private bool? _publicClient;
    private ChangeTrackingCollection<string> _replyUrls;
    private bool _replyUrlsInitialized;
    private ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.RequiredResourceAccess> _requiredResourceAccess;
    private bool _requiredResourceAccessInitialized;
    private string _samlMetadataUrl;
    private ChangeTrackingCollection<ExtensionProperty> _extensionProperties;
    private bool _extensionPropertiesInitialized;

    [JsonProperty("allowActAsForAllClients")]
    public bool? AllowActAsForAllClients
    {
      get => this._allowActAsForAllClients;
      set
      {
        this._allowActAsForAllClients = value;
        this.ChangedProperties.Add(nameof (AllowActAsForAllClients));
      }
    }

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

    public Application() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.Application";

    public Application(string objectId)
      : this()
    {
      this.ObjectId = objectId;
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

    [JsonProperty("availableToOtherTenants")]
    public bool? AvailableToOtherTenants
    {
      get => this._availableToOtherTenants;
      set
      {
        this._availableToOtherTenants = value;
        this.ChangedProperties.Add(nameof (AvailableToOtherTenants));
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

    [JsonProperty("groupMembershipClaims")]
    public string GroupMembershipClaims
    {
      get => this._groupMembershipClaims;
      set
      {
        this._groupMembershipClaims = value;
        this.ChangedProperties.Add(nameof (GroupMembershipClaims));
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

    [JsonProperty("identifierUris")]
    public ChangeTrackingCollection<string> IdentifierUris
    {
      get
      {
        if (this._identifierUris == null)
          this._identifierUris = new ChangeTrackingCollection<string>();
        if (!this._identifierUrisInitialized)
        {
          this._identifierUris.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (IdentifierUris)));
          this._identifierUrisInitialized = true;
        }
        return this._identifierUris;
      }
      set
      {
        this._identifierUris = value;
        this.ChangedProperties.Add(nameof (IdentifierUris));
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

    [JsonProperty("knownClientApplications")]
    public ChangeTrackingCollection<Guid> KnownClientApplications
    {
      get
      {
        if (this._knownClientApplications == null)
          this._knownClientApplications = new ChangeTrackingCollection<Guid>();
        if (!this._knownClientApplicationsInitialized)
        {
          this._knownClientApplications.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (KnownClientApplications)));
          this._knownClientApplicationsInitialized = true;
        }
        return this._knownClientApplications;
      }
      set
      {
        this._knownClientApplications = value;
        this.ChangedProperties.Add(nameof (KnownClientApplications));
      }
    }

    [JsonProperty("mainLogo")]
    public Stream MainLogo
    {
      get => this._mainLogo;
      set
      {
        this._mainLogo = value;
        this.ChangedProperties.Add(nameof (MainLogo));
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

    [JsonProperty("oauth2AllowImplicitFlow")]
    public bool? Oauth2AllowImplicitFlow
    {
      get => this._oauth2AllowImplicitFlow;
      set
      {
        this._oauth2AllowImplicitFlow = value;
        this.ChangedProperties.Add(nameof (Oauth2AllowImplicitFlow));
      }
    }

    [JsonProperty("oauth2AllowUrlPathMatching")]
    public bool? Oauth2AllowUrlPathMatching
    {
      get => this._oauth2AllowUrlPathMatching;
      set
      {
        this._oauth2AllowUrlPathMatching = value;
        this.ChangedProperties.Add(nameof (Oauth2AllowUrlPathMatching));
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

    [JsonProperty("oauth2RequirePostResponse")]
    public bool? Oauth2RequirePostResponse
    {
      get => this._oauth2RequirePostResponse;
      set
      {
        this._oauth2RequirePostResponse = value;
        this.ChangedProperties.Add(nameof (Oauth2RequirePostResponse));
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

    [JsonProperty("publicClient")]
    public bool? PublicClient
    {
      get => this._publicClient;
      set
      {
        this._publicClient = value;
        this.ChangedProperties.Add(nameof (PublicClient));
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

    [JsonProperty("requiredResourceAccess")]
    public ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.RequiredResourceAccess> RequiredResourceAccess
    {
      get
      {
        if (this._requiredResourceAccess == null)
          this._requiredResourceAccess = new ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.RequiredResourceAccess>();
        if (!this._requiredResourceAccessInitialized)
        {
          this._requiredResourceAccess.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (RequiredResourceAccess)));
          this._requiredResourceAccess.ToList<Microsoft.Azure.ActiveDirectory.GraphClient.RequiredResourceAccess>().ForEach((Action<Microsoft.Azure.ActiveDirectory.GraphClient.RequiredResourceAccess>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (RequiredResourceAccess)))));
          this._requiredResourceAccessInitialized = true;
        }
        return this._requiredResourceAccess;
      }
      set
      {
        this._requiredResourceAccess = value;
        this.ChangedProperties.Add(nameof (RequiredResourceAccess));
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

    [JsonConverter(typeof (AadJsonConverter))]
    [Link("extensionProperties", false)]
    [JsonProperty("extensionProperties")]
    public ChangeTrackingCollection<ExtensionProperty> ExtensionProperties
    {
      get
      {
        if (this._extensionProperties == null)
          this._extensionProperties = new ChangeTrackingCollection<ExtensionProperty>();
        if (!this._extensionPropertiesInitialized)
        {
          this._extensionProperties.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ExtensionProperties)));
          this._extensionPropertiesInitialized = true;
        }
        return this._extensionProperties;
      }
      set
      {
        this._extensionProperties = value;
        this.ChangedProperties.Add(nameof (ExtensionProperties));
      }
    }
  }
}
