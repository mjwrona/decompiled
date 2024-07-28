// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.SelfServePasswordResetPolicy
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
  public class SelfServePasswordResetPolicy
  {
    private string _enforcedRegistrationEnablement;
    private int? _enforcedRegistrationIntervalInDays;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("enforcedRegistrationEnablement")]
    public string EnforcedRegistrationEnablement
    {
      get => this._enforcedRegistrationEnablement;
      set
      {
        this._enforcedRegistrationEnablement = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("enforcedRegistrationIntervalInDays")]
    public int? EnforcedRegistrationIntervalInDays
    {
      get => this._enforcedRegistrationIntervalInDays;
      set
      {
        this._enforcedRegistrationIntervalInDays = value;
        this.OnItemChanged();
      }
    }
  }
}
