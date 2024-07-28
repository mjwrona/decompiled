// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.VerifiedDomain
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
  public class VerifiedDomain
  {
    private string _capabilities;
    private bool? _default;
    private string _id;
    private bool? _initial;
    private string _name;
    private string _type;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("capabilities")]
    public string Capabilities
    {
      get => this._capabilities;
      set
      {
        this._capabilities = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("default")]
    public bool? Default
    {
      get => this._default;
      set
      {
        this._default = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("id")]
    public string Id
    {
      get => this._id;
      set
      {
        this._id = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("initial")]
    public bool? Initial
    {
      get => this._initial;
      set
      {
        this._initial = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("name")]
    public string Name
    {
      get => this._name;
      set
      {
        this._name = value;
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
  }
}
