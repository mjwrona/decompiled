// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.SearchableDeviceKey
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
  public class SearchableDeviceKey
  {
    private string _usage;
    private Guid? _keyIdentifier;
    private byte[] _keyMaterial;
    private DateTime? _creationTime;
    private Guid? _deviceId;
    private byte[] _customKeyInformation;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("usage")]
    public string Usage
    {
      get => this._usage;
      set
      {
        this._usage = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("keyIdentifier")]
    public Guid? KeyIdentifier
    {
      get => this._keyIdentifier;
      set
      {
        this._keyIdentifier = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("keyMaterial")]
    public byte[] KeyMaterial
    {
      get => this._keyMaterial;
      set
      {
        this._keyMaterial = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("creationTime")]
    public DateTime? CreationTime
    {
      get => this._creationTime;
      set
      {
        this._creationTime = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("deviceId")]
    public Guid? DeviceId
    {
      get => this._deviceId;
      set
      {
        this._deviceId = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("customKeyInformation")]
    public byte[] CustomKeyInformation
    {
      get => this._customKeyInformation;
      set
      {
        this._customKeyInformation = value;
        this.OnItemChanged();
      }
    }
  }
}
