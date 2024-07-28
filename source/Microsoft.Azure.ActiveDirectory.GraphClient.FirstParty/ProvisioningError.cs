// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ProvisioningError
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
  public class ProvisioningError
  {
    private string _errorDetail;
    private bool? _resolved;
    private string _service;
    private DateTime? _timestamp;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("errorDetail")]
    public string ErrorDetail
    {
      get => this._errorDetail;
      set
      {
        this._errorDetail = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("resolved")]
    public bool? Resolved
    {
      get => this._resolved;
      set
      {
        this._resolved = value;
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

    [JsonProperty("timestamp")]
    public DateTime? Timestamp
    {
      get => this._timestamp;
      set
      {
        this._timestamp = value;
        this.OnItemChanged();
      }
    }
  }
}
