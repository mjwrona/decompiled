// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.RequiredResourceAccess
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [JsonObject(MemberSerialization.OptIn)]
  [ExcludeFromCodeCoverage]
  public class RequiredResourceAccess
  {
    private string _resourceAppId;
    private ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.ResourceAccess> _resourceAccess;
    private bool _resourceAccessInitialized;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("resourceAppId")]
    public string ResourceAppId
    {
      get => this._resourceAppId;
      set
      {
        this._resourceAppId = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("resourceAccess")]
    public ChangeTrackingCollection<Microsoft.Azure.ActiveDirectory.GraphClient.ResourceAccess> ResourceAccess
    {
      get
      {
        if (this._resourceAccess != null && !this._resourceAccessInitialized)
        {
          this._resourceAccess.CollectionChanged += (EventHandler) ((sender, s) => this.OnItemChanged());
          this._resourceAccess.ToList<Microsoft.Azure.ActiveDirectory.GraphClient.ResourceAccess>().ForEach((Action<Microsoft.Azure.ActiveDirectory.GraphClient.ResourceAccess>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.OnItemChanged())));
          this._resourceAccessInitialized = true;
        }
        return this._resourceAccess;
      }
      set
      {
        this._resourceAccess = value;
        this.OnItemChanged();
      }
    }
  }
}
