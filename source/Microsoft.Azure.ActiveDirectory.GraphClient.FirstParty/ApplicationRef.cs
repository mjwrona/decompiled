// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ApplicationRef
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
  [Entity("applicationRefs", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.ApplicationRef", "Microsoft.DirectoryServices.ApplicationRef"})]
  [ExcludeFromCodeCoverage]
  public class ApplicationRef : GraphObject
  {
    private Guid? _appContextId;
    private string _appId;
    private ChangeTrackingCollection<AppRole> _appRoles;
    private bool _appRolesInitialized;
    private bool? _availableToOtherTenants;
    private string _displayName;
    private string _errorUrl;
    private string _homepage;
    private ChangeTrackingCollection<string> _identifierUris;
    private bool _identifierUrisInitialized;
    private ChangeTrackingCollection<Guid> _knownClientApplications;
    private bool _knownClientApplicationsInitialized;
    private string _logoutUrl;
    private Stream _mainLogo;
    private ChangeTrackingCollection<OAuth2Permission> _oauth2Permissions;
    private bool _oauth2PermissionsInitialized;
    private string _publisherDomain;
    private string _publisherName;
    private bool? _publicClient;
    private ChangeTrackingCollection<string> _replyUrls;
    private bool _replyUrlsInitialized;
    private ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.RequiredResourceAccess> _requiredResourceAccess;
    private bool _requiredResourceAccessInitialized;
    private string _samlMetadataUrl;

    [JsonProperty("appContextId")]
    public Guid? AppContextId
    {
      get => this._appContextId;
      set
      {
        this._appContextId = value;
        this.ChangedProperties.Add(nameof (AppContextId));
      }
    }

    [JsonProperty("appId")]
    [Key(true)]
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

    [JsonProperty("publisherDomain")]
    public string PublisherDomain
    {
      get => this._publisherDomain;
      set
      {
        this._publisherDomain = value;
        this.ChangedProperties.Add(nameof (PublisherDomain));
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
  }
}
