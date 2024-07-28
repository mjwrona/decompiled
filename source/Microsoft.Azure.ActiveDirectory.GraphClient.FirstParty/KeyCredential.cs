// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.KeyCredential
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
  public class KeyCredential
  {
    private byte[] _customKeyIdentifier;
    private DateTime? _endDate;
    private Guid? _keyId;
    private DateTime? _startDate;
    private string _type;
    private string _usage;
    private byte[] _value;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("customKeyIdentifier")]
    public byte[] CustomKeyIdentifier
    {
      get => this._customKeyIdentifier;
      set
      {
        this._customKeyIdentifier = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("endDate")]
    public DateTime? EndDate
    {
      get => this._endDate;
      set
      {
        this._endDate = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("keyId")]
    public Guid? KeyId
    {
      get => this._keyId;
      set
      {
        this._keyId = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("startDate")]
    public DateTime? StartDate
    {
      get => this._startDate;
      set
      {
        this._startDate = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("type")]
    public string Type
    {
      get => this._type;
      set
      {
        this._type = value;
        this.OnItemChanged();
      }
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

    [JsonProperty("value")]
    public byte[] Value
    {
      get => this._value;
      set
      {
        this._value = value;
        this.OnItemChanged();
      }
    }
  }
}
