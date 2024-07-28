// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.AppRole
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
  public class AppRole
  {
    private string _origin;
    private ChangeTrackingCollection<string> _allowedMemberTypes;
    private bool _allowedMemberTypesInitialized;
    private string _description;
    private string _displayName;
    private Guid? _id;
    private bool? _isEnabled;
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

    [JsonProperty("allowedMemberTypes")]
    public ChangeTrackingCollection<string> AllowedMemberTypes
    {
      get
      {
        if (this._allowedMemberTypes == null)
          this._allowedMemberTypes = new ChangeTrackingCollection<string>();
        if (this._allowedMemberTypes != null && !this._allowedMemberTypesInitialized)
        {
          this._allowedMemberTypes.CollectionChanged += (EventHandler) ((sender, s) => this.OnItemChanged());
          this._allowedMemberTypesInitialized = true;
        }
        return this._allowedMemberTypes;
      }
      set
      {
        this._allowedMemberTypes = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("description")]
    public string Description
    {
      get => this._description;
      set
      {
        this._description = value;
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
