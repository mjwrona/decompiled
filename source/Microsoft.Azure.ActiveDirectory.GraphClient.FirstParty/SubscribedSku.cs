// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.SubscribedSku
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
  [Entity("subscribedSkus", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.SubscribedSku", "Microsoft.DirectoryServices.SubscribedSku"})]
  public class SubscribedSku : GraphObject
  {
    private string _capabilityStatus;
    private int? _consumedUnits;
    private string _objectId;
    private LicenseUnitsDetail _prepaidUnits;
    private bool _prepaidUnitsInitialized;
    private ChangeTrackingCollection<ServicePlanInfo> _servicePlans;
    private bool _servicePlansInitialized;
    private Guid? _skuId;
    private string _skuPartNumber;

    [JsonProperty("capabilityStatus")]
    public string CapabilityStatus
    {
      get => this._capabilityStatus;
      set
      {
        this._capabilityStatus = value;
        this.ChangedProperties.Add(nameof (CapabilityStatus));
      }
    }

    [JsonProperty("consumedUnits")]
    public int? ConsumedUnits
    {
      get => this._consumedUnits;
      set
      {
        this._consumedUnits = value;
        this.ChangedProperties.Add(nameof (ConsumedUnits));
      }
    }

    [JsonProperty("objectId")]
    [Key(true)]
    public string ObjectId
    {
      get => this._objectId;
      set
      {
        this._objectId = value;
        this.ChangedProperties.Add(nameof (ObjectId));
      }
    }

    [JsonProperty("prepaidUnits")]
    public LicenseUnitsDetail PrepaidUnits
    {
      get
      {
        if (this._prepaidUnits != null && !this._prepaidUnitsInitialized)
        {
          this._prepaidUnits.ItemChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (PrepaidUnits)));
          this._prepaidUnitsInitialized = true;
        }
        return this._prepaidUnits;
      }
      set
      {
        this._prepaidUnits = value;
        this.ChangedProperties.Add(nameof (PrepaidUnits));
      }
    }

    [JsonProperty("servicePlans")]
    public ChangeTrackingCollection<ServicePlanInfo> ServicePlans
    {
      get
      {
        if (this._servicePlans == null)
          this._servicePlans = new ChangeTrackingCollection<ServicePlanInfo>();
        if (!this._servicePlansInitialized)
        {
          this._servicePlans.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ServicePlans)));
          this._servicePlans.ToList<ServicePlanInfo>().ForEach((Action<ServicePlanInfo>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (ServicePlans)))));
          this._servicePlansInitialized = true;
        }
        return this._servicePlans;
      }
      set
      {
        this._servicePlans = value;
        this.ChangedProperties.Add(nameof (ServicePlans));
      }
    }

    [JsonProperty("skuId")]
    public Guid? SkuId
    {
      get => this._skuId;
      set
      {
        this._skuId = value;
        this.ChangedProperties.Add(nameof (SkuId));
      }
    }

    [JsonProperty("skuPartNumber")]
    public string SkuPartNumber
    {
      get => this._skuPartNumber;
      set
      {
        this._skuPartNumber = value;
        this.ChangedProperties.Add(nameof (SkuPartNumber));
      }
    }
  }
}
