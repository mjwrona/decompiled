// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.TenantDetail
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
  [ExcludeFromCodeCoverage]
  [Entity("tenantDetails", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.TenantDetail", "Microsoft.DirectoryServices.TenantDetail"})]
  public class TenantDetail : DirectoryObject
  {
    private ChangeTrackingCollection<string> _authorizedServiceInstance;
    private bool _authorizedServiceInstanceInitialized;
    private ChangeTrackingCollection<string> _companyTags;
    private bool _companyTagsInitialized;
    private bool? _compassEnabled;
    private string _releaseTrack;
    private string _replicationScope;
    private SelfServePasswordResetPolicy _selfServePasswordResetPolicy;
    private bool _selfServePasswordResetPolicyInitialized;
    private string _tenantType;
    private ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.ServiceInfo> _serviceInfo;
    private bool _serviceInfoInitialized;
    private ChangeTrackingCollection<AssignedPlan> _assignedPlans;
    private bool _assignedPlansInitialized;
    private string _city;
    private DateTime? _companyLastDirSyncTime;
    private string _country;
    private string _countryLetterCode;
    private bool? _dirSyncEnabled;
    private string _displayName;
    private ChangeTrackingCollection<string> _marketingNotificationEmails;
    private bool _marketingNotificationEmailsInitialized;
    private string _postalCode;
    private string _preferredLanguage;
    private ChangeTrackingCollection<ProvisionedPlan> _provisionedPlans;
    private bool _provisionedPlansInitialized;
    private ChangeTrackingCollection<ProvisioningError> _provisioningErrors;
    private bool _provisioningErrorsInitialized;
    private ChangeTrackingCollection<string> _securityComplianceNotificationMails;
    private bool _securityComplianceNotificationMailsInitialized;
    private ChangeTrackingCollection<string> _securityComplianceNotificationPhones;
    private bool _securityComplianceNotificationPhonesInitialized;
    private string _state;
    private string _street;
    private ChangeTrackingCollection<string> _technicalNotificationMails;
    private bool _technicalNotificationMailsInitialized;
    private string _telephoneNumber;
    private ChangeTrackingCollection<VerifiedDomain> _verifiedDomains;
    private bool _verifiedDomainsInitialized;

    [JsonProperty("authorizedServiceInstance")]
    public ChangeTrackingCollection<string> AuthorizedServiceInstance
    {
      get
      {
        if (this._authorizedServiceInstance == null)
          this._authorizedServiceInstance = new ChangeTrackingCollection<string>();
        if (!this._authorizedServiceInstanceInitialized)
        {
          this._authorizedServiceInstance.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (AuthorizedServiceInstance)));
          this._authorizedServiceInstanceInitialized = true;
        }
        return this._authorizedServiceInstance;
      }
      set
      {
        this._authorizedServiceInstance = value;
        this.ChangedProperties.Add(nameof (AuthorizedServiceInstance));
      }
    }

    [JsonProperty("companyTags")]
    public ChangeTrackingCollection<string> CompanyTags
    {
      get
      {
        if (this._companyTags == null)
          this._companyTags = new ChangeTrackingCollection<string>();
        if (!this._companyTagsInitialized)
        {
          this._companyTags.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (CompanyTags)));
          this._companyTagsInitialized = true;
        }
        return this._companyTags;
      }
      set
      {
        this._companyTags = value;
        this.ChangedProperties.Add(nameof (CompanyTags));
      }
    }

    [JsonProperty("compassEnabled")]
    public bool? CompassEnabled
    {
      get => this._compassEnabled;
      set
      {
        this._compassEnabled = value;
        this.ChangedProperties.Add(nameof (CompassEnabled));
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

    [JsonProperty("replicationScope")]
    public string ReplicationScope
    {
      get => this._replicationScope;
      set
      {
        this._replicationScope = value;
        this.ChangedProperties.Add(nameof (ReplicationScope));
      }
    }

    [JsonProperty("selfServePasswordResetPolicy")]
    public SelfServePasswordResetPolicy SelfServePasswordResetPolicy
    {
      get
      {
        if (this._selfServePasswordResetPolicy != null && !this._selfServePasswordResetPolicyInitialized)
        {
          this._selfServePasswordResetPolicy.ItemChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (SelfServePasswordResetPolicy)));
          this._selfServePasswordResetPolicyInitialized = true;
        }
        return this._selfServePasswordResetPolicy;
      }
      set
      {
        this._selfServePasswordResetPolicy = value;
        this.ChangedProperties.Add(nameof (SelfServePasswordResetPolicy));
      }
    }

    [JsonProperty("tenantType")]
    public string TenantType
    {
      get => this._tenantType;
      set
      {
        this._tenantType = value;
        this.ChangedProperties.Add(nameof (TenantType));
      }
    }

    [JsonProperty("serviceInfo")]
    [JsonConverter(typeof (AadJsonConverter))]
    [Link("serviceInfo", false)]
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

    public TenantDetail() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.TenantDetail";

    public TenantDetail(string objectId)
      : this()
    {
      this.ObjectId = objectId;
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

    [JsonProperty("companyLastDirSyncTime")]
    public DateTime? CompanyLastDirSyncTime
    {
      get => this._companyLastDirSyncTime;
      set
      {
        this._companyLastDirSyncTime = value;
        this.ChangedProperties.Add(nameof (CompanyLastDirSyncTime));
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

    [JsonProperty("countryLetterCode")]
    public string CountryLetterCode
    {
      get => this._countryLetterCode;
      set
      {
        this._countryLetterCode = value;
        this.ChangedProperties.Add(nameof (CountryLetterCode));
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

    [JsonProperty("marketingNotificationEmails")]
    public ChangeTrackingCollection<string> MarketingNotificationEmails
    {
      get
      {
        if (this._marketingNotificationEmails == null)
          this._marketingNotificationEmails = new ChangeTrackingCollection<string>();
        if (!this._marketingNotificationEmailsInitialized)
        {
          this._marketingNotificationEmails.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (MarketingNotificationEmails)));
          this._marketingNotificationEmailsInitialized = true;
        }
        return this._marketingNotificationEmails;
      }
      set
      {
        this._marketingNotificationEmails = value;
        this.ChangedProperties.Add(nameof (MarketingNotificationEmails));
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

    [JsonProperty("securityComplianceNotificationMails")]
    public ChangeTrackingCollection<string> SecurityComplianceNotificationMails
    {
      get
      {
        if (this._securityComplianceNotificationMails == null)
          this._securityComplianceNotificationMails = new ChangeTrackingCollection<string>();
        if (!this._securityComplianceNotificationMailsInitialized)
        {
          this._securityComplianceNotificationMails.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (SecurityComplianceNotificationMails)));
          this._securityComplianceNotificationMailsInitialized = true;
        }
        return this._securityComplianceNotificationMails;
      }
      set
      {
        this._securityComplianceNotificationMails = value;
        this.ChangedProperties.Add(nameof (SecurityComplianceNotificationMails));
      }
    }

    [JsonProperty("securityComplianceNotificationPhones")]
    public ChangeTrackingCollection<string> SecurityComplianceNotificationPhones
    {
      get
      {
        if (this._securityComplianceNotificationPhones == null)
          this._securityComplianceNotificationPhones = new ChangeTrackingCollection<string>();
        if (!this._securityComplianceNotificationPhonesInitialized)
        {
          this._securityComplianceNotificationPhones.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (SecurityComplianceNotificationPhones)));
          this._securityComplianceNotificationPhonesInitialized = true;
        }
        return this._securityComplianceNotificationPhones;
      }
      set
      {
        this._securityComplianceNotificationPhones = value;
        this.ChangedProperties.Add(nameof (SecurityComplianceNotificationPhones));
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

    [JsonProperty("street")]
    public string Street
    {
      get => this._street;
      set
      {
        this._street = value;
        this.ChangedProperties.Add(nameof (Street));
      }
    }

    [JsonProperty("technicalNotificationMails")]
    public ChangeTrackingCollection<string> TechnicalNotificationMails
    {
      get
      {
        if (this._technicalNotificationMails == null)
          this._technicalNotificationMails = new ChangeTrackingCollection<string>();
        if (!this._technicalNotificationMailsInitialized)
        {
          this._technicalNotificationMails.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (TechnicalNotificationMails)));
          this._technicalNotificationMailsInitialized = true;
        }
        return this._technicalNotificationMails;
      }
      set
      {
        this._technicalNotificationMails = value;
        this.ChangedProperties.Add(nameof (TechnicalNotificationMails));
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

    [JsonProperty("verifiedDomains")]
    public ChangeTrackingCollection<VerifiedDomain> VerifiedDomains
    {
      get
      {
        if (this._verifiedDomains == null)
          this._verifiedDomains = new ChangeTrackingCollection<VerifiedDomain>();
        if (!this._verifiedDomainsInitialized)
        {
          this._verifiedDomains.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (VerifiedDomains)));
          this._verifiedDomains.ToList<VerifiedDomain>().ForEach((Action<VerifiedDomain>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (VerifiedDomains)))));
          this._verifiedDomainsInitialized = true;
        }
        return this._verifiedDomains;
      }
      set
      {
        this._verifiedDomains = value;
        this.ChangedProperties.Add(nameof (VerifiedDomains));
      }
    }
  }
}
