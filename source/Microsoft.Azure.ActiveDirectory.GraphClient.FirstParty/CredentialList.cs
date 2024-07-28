// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.CredentialList
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
  public class CredentialList
  {
    private ChangeTrackingCollection<string> _passwords;
    private bool _passwordsInitialized;
    private string _username;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("passwords")]
    public ChangeTrackingCollection<string> Passwords
    {
      get
      {
        if (this._passwords == null)
          this._passwords = new ChangeTrackingCollection<string>();
        if (this._passwords != null && !this._passwordsInitialized)
        {
          this._passwords.CollectionChanged += (EventHandler) ((sender, s) => this.OnItemChanged());
          this._passwordsInitialized = true;
        }
        return this._passwords;
      }
      set
      {
        this._passwords = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("username")]
    public string Username
    {
      get => this._username;
      set
      {
        this._username = value;
        this.OnItemChanged();
      }
    }
  }
}
