// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.PasswordProfile
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
  public class PasswordProfile
  {
    private string _password;
    private bool? _forceChangePasswordNextLogin;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("password")]
    public string Password
    {
      get => this._password;
      set
      {
        this._password = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("forceChangePasswordNextLogin")]
    public bool? ForceChangePasswordNextLogin
    {
      get => this._forceChangePasswordNextLogin;
      set
      {
        this._forceChangePasswordNextLogin = value;
        this.OnItemChanged();
      }
    }
  }
}
