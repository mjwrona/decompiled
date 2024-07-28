// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.GuestTenantDetail
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [JsonObject(MemberSerialization.OptIn)]
  [ExcludeFromCodeCoverage]
  public class GuestTenantDetail
  {
    private string _tenantId;
    private string _country;
    private string _countryCode;
    private string _displayName;
    private ChangeTrackingCollection<string> _domains;
    private bool _domainsInitialized;
    private bool? _isHomeTenant;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("tenantId")]
    public string TenantId
    {
      get => this._tenantId;
      set
      {
        this._tenantId = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("country")]
    public string Country
    {
      get => this._country;
      set
      {
        this._country = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("countryCode")]
    public string CountryCode
    {
      get => this._countryCode;
      set
      {
        this._countryCode = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("displayName")]
    public string DisplayName
    {
      get => this._displayName;
      set
      {
        this._displayName = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("domains")]
    public ChangeTrackingCollection<string> Domains
    {
      get
      {
        if (this._domains == null)
          this._domains = new ChangeTrackingCollection<string>();
        if (this._domains != null && !this._domainsInitialized)
        {
          this._domains.CollectionChanged += (EventHandler) ((sender, s) => this.OnItemChanged());
          this._domainsInitialized = true;
        }
        return this._domains;
      }
      set
      {
        this._domains = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("isHomeTenant")]
    public bool? IsHomeTenant
    {
      get => this._isHomeTenant;
      set
      {
        this._isHomeTenant = value;
        this.OnItemChanged();
      }
    }
  }
}
