// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.Contact
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
  [ExcludeFromCodeCoverage]
  [Entity("contacts", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.Contact", "Microsoft.DirectoryServices.Contact"})]
  [JsonObject(MemberSerialization.OptIn)]
  public class Contact : DirectoryObject
  {
    private string _city;
    private string _country;
    private string _department;
    private bool? _dirSyncEnabled;
    private string _displayName;
    private string _facsimileTelephoneNumber;
    private string _givenName;
    private string _jobTitle;
    private DateTime? _lastDirSyncTime;
    private string _mail;
    private string _mailNickname;
    private string _mobile;
    private string _physicalDeliveryOfficeName;
    private string _postalCode;
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

    public Contact() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.Contact";

    public Contact(string objectId)
      : this()
    {
      this.ObjectId = objectId;
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
  }
}
