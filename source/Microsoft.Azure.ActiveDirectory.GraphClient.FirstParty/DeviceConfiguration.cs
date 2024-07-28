// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.DeviceConfiguration
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [Entity("deviceConfigurations", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.DeviceConfiguration", "Microsoft.DirectoryServices.DeviceConfiguration"})]
  [ExcludeFromCodeCoverage]
  [JsonObject(MemberSerialization.OptIn)]
  public class DeviceConfiguration : DirectoryObject
  {
    private ChangeTrackingCollection<byte[]> _publicIssuerCertificates;
    private bool _publicIssuerCertificatesInitialized;
    private ChangeTrackingCollection<byte[]> _cloudPublicIssuerCertificates;
    private bool _cloudPublicIssuerCertificatesInitialized;
    private int? _registrationQuota;
    private int? _maximumRegistrationInactivityPeriod;

    public DeviceConfiguration() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.DeviceConfiguration";

    public DeviceConfiguration(string objectId)
      : this()
    {
      this.ObjectId = objectId;
    }

    [JsonProperty("publicIssuerCertificates")]
    public ChangeTrackingCollection<byte[]> PublicIssuerCertificates
    {
      get
      {
        if (this._publicIssuerCertificates == null)
          this._publicIssuerCertificates = new ChangeTrackingCollection<byte[]>();
        if (!this._publicIssuerCertificatesInitialized)
        {
          this._publicIssuerCertificates.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (PublicIssuerCertificates)));
          this._publicIssuerCertificatesInitialized = true;
        }
        return this._publicIssuerCertificates;
      }
      set
      {
        this._publicIssuerCertificates = value;
        this.ChangedProperties.Add(nameof (PublicIssuerCertificates));
      }
    }

    [JsonProperty("cloudPublicIssuerCertificates")]
    public ChangeTrackingCollection<byte[]> CloudPublicIssuerCertificates
    {
      get
      {
        if (this._cloudPublicIssuerCertificates == null)
          this._cloudPublicIssuerCertificates = new ChangeTrackingCollection<byte[]>();
        if (!this._cloudPublicIssuerCertificatesInitialized)
        {
          this._cloudPublicIssuerCertificates.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (CloudPublicIssuerCertificates)));
          this._cloudPublicIssuerCertificatesInitialized = true;
        }
        return this._cloudPublicIssuerCertificates;
      }
      set
      {
        this._cloudPublicIssuerCertificates = value;
        this.ChangedProperties.Add(nameof (CloudPublicIssuerCertificates));
      }
    }

    [JsonProperty("registrationQuota")]
    public int? RegistrationQuota
    {
      get => this._registrationQuota;
      set
      {
        this._registrationQuota = value;
        this.ChangedProperties.Add(nameof (RegistrationQuota));
      }
    }

    [JsonProperty("maximumRegistrationInactivityPeriod")]
    public int? MaximumRegistrationInactivityPeriod
    {
      get => this._maximumRegistrationInactivityPeriod;
      set
      {
        this._maximumRegistrationInactivityPeriod = value;
        this.ChangedProperties.Add(nameof (MaximumRegistrationInactivityPeriod));
      }
    }
  }
}
