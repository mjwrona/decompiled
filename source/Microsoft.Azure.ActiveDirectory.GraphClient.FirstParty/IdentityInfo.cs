// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.IdentityInfo
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
  public class IdentityInfo
  {
    private Guid? _objectId;
    private string _displayName;
    private string _userPrincipalName;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("objectId")]
    public Guid? ObjectId
    {
      get => this._objectId;
      set
      {
        this._objectId = value;
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

    [JsonProperty("userPrincipalName")]
    [Key(false)]
    public string UserPrincipalName
    {
      get => this._userPrincipalName;
      set
      {
        this._userPrincipalName = value;
        this.OnItemChanged();
      }
    }
  }
}
