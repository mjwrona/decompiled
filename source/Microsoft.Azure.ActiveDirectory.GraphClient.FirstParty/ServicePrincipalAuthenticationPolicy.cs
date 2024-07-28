// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ServicePrincipalAuthenticationPolicy
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
  public class ServicePrincipalAuthenticationPolicy
  {
    private string _defaultPolicy;
    private ChangeTrackingCollection<string> _allowedPolicies;
    private bool _allowedPoliciesInitialized;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("defaultPolicy")]
    public string DefaultPolicy
    {
      get => this._defaultPolicy;
      set
      {
        this._defaultPolicy = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("allowedPolicies")]
    public ChangeTrackingCollection<string> AllowedPolicies
    {
      get
      {
        if (this._allowedPolicies == null)
          this._allowedPolicies = new ChangeTrackingCollection<string>();
        if (this._allowedPolicies != null && !this._allowedPoliciesInitialized)
        {
          this._allowedPolicies.CollectionChanged += (EventHandler) ((sender, s) => this.OnItemChanged());
          this._allowedPoliciesInitialized = true;
        }
        return this._allowedPolicies;
      }
      set
      {
        this._allowedPolicies = value;
        this.OnItemChanged();
      }
    }
  }
}
