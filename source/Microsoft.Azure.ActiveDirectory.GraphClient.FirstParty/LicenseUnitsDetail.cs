// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.LicenseUnitsDetail
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
  public class LicenseUnitsDetail
  {
    private int? _enabled;
    private int? _suspended;
    private int? _warning;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("enabled")]
    public int? Enabled
    {
      get => this._enabled;
      set
      {
        this._enabled = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("suspended")]
    public int? Suspended
    {
      get => this._suspended;
      set
      {
        this._suspended = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("warning")]
    public int? Warning
    {
      get => this._warning;
      set
      {
        this._warning = value;
        this.OnItemChanged();
      }
    }
  }
}
