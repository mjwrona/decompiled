// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ProvisionedPlan
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [ExcludeFromCodeCoverage]
  [JsonObject(MemberSerialization.OptIn)]
  public class ProvisionedPlan
  {
    private string _capabilityStatus;
    private string _provisioningStatus;
    private string _service;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("capabilityStatus")]
    public string CapabilityStatus
    {
      get => this._capabilityStatus;
      set
      {
        this._capabilityStatus = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("provisioningStatus")]
    public string ProvisioningStatus
    {
      get => this._provisioningStatus;
      set
      {
        this._provisioningStatus = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("service")]
    public string Service
    {
      get => this._service;
      set
      {
        this._service = value;
        this.OnItemChanged();
      }
    }
  }
}
