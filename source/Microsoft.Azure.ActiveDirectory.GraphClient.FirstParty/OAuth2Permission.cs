// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.OAuth2Permission
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
  public class OAuth2Permission
  {
    private string _origin;
    private string _adminConsentDescription;
    private string _adminConsentDisplayName;
    private Guid? _id;
    private bool? _isEnabled;
    private string _type;
    private string _userConsentDescription;
    private string _userConsentDisplayName;
    private string _value;

    [JsonProperty("origin")]
    public string Origin
    {
      get => this._origin;
      set
      {
        this._origin = value;
        this.OnItemChanged();
      }
    }

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("adminConsentDescription")]
    public string AdminConsentDescription
    {
      get => this._adminConsentDescription;
      set
      {
        this._adminConsentDescription = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("adminConsentDisplayName")]
    public string AdminConsentDisplayName
    {
      get => this._adminConsentDisplayName;
      set
      {
        this._adminConsentDisplayName = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("id")]
    public Guid? Id
    {
      get => this._id;
      set
      {
        this._id = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("isEnabled")]
    public bool? IsEnabled
    {
      get => this._isEnabled;
      set
      {
        this._isEnabled = value;
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

    [JsonProperty("userConsentDescription")]
    public string UserConsentDescription
    {
      get => this._userConsentDescription;
      set
      {
        this._userConsentDescription = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("userConsentDisplayName")]
    public string UserConsentDisplayName
    {
      get => this._userConsentDisplayName;
      set
      {
        this._userConsentDisplayName = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("value")]
    public string Value
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
