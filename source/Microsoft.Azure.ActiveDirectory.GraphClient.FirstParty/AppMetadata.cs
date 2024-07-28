// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.AppMetadata
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
  public class AppMetadata
  {
    private int? _version;
    private ChangeTrackingCollection<AppMetadataEntry> _data;
    private bool _dataInitialized;

    internal event EventHandler ItemChanged;

    protected virtual void OnItemChanged()
    {
      if (this.ItemChanged == null)
        return;
      this.ItemChanged((object) this, EventArgs.Empty);
    }

    [JsonProperty("version")]
    public int? Version
    {
      get => this._version;
      set
      {
        this._version = value;
        this.OnItemChanged();
      }
    }

    [JsonProperty("data")]
    public ChangeTrackingCollection<AppMetadataEntry> Data
    {
      get
      {
        if (this._data != null && !this._dataInitialized)
        {
          this._data.CollectionChanged += (EventHandler) ((sender, s) => this.OnItemChanged());
          this._data.ToList<AppMetadataEntry>().ForEach((Action<AppMetadataEntry>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.OnItemChanged())));
          this._dataInitialized = true;
        }
        return this._data;
      }
      set
      {
        this._data = value;
        this.OnItemChanged();
      }
    }
  }
}
