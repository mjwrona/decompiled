// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.AlternativeSecurityId
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
  public class AlternativeSecurityId
  {
    private int? _type;
    private string _identityProvider;
    private byte[] _key;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("type")]
    public int? Type
    {
      get => this._type;
      set
      {
        this._type = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("identityProvider")]
    public string IdentityProvider
    {
      get => this._identityProvider;
      set
      {
        this._identityProvider = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("key")]
    public byte[] Key
    {
      get => this._key;
      set
      {
        this._key = value;
        this.OnItemChanged();
      }
    }
  }
}
