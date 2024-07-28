// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.AssignedPlan
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
  public class AssignedPlan
  {
    private DateTime? _assignedTimestamp;
    private string _capabilityStatus;
    private string _service;
    private Guid? _servicePlanId;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("assignedTimestamp")]
    public DateTime? AssignedTimestamp
    {
      get => this._assignedTimestamp;
      set
      {
        this._assignedTimestamp = value;
        this.OnItemChanged();
      }
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

    [JsonProperty("servicePlanId")]
    public Guid? ServicePlanId
    {
      get => this._servicePlanId;
      set
      {
        this._servicePlanId = value;
        this.OnItemChanged();
      }
    }
  }
}
