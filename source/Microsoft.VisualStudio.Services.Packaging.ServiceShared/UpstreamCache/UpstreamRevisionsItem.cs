// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.UpstreamRevisionsItem
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache
{
  public class UpstreamRevisionsItem : StoredItem
  {
    private const string CurrentRevisionsTableKey = "Revisions";
    private const string StaleRevisionListKey = "StaleRevisions";
    private const string AliasListItemType = "AliasList";
    private const string MetadataFormatVersionKey = "MetadataFormatVersion";

    public UpstreamRevisionsItem()
      : base("AliasList")
    {
      this.MetadataFormatVersion = 1;
    }

    public UpstreamRevisionsItem(IItemData data)
      : base(data, "AliasList")
    {
      this.MetadataFormatVersion = 1;
    }

    public List<UpstreamRevision> StaleRevisionList
    {
      get
      {
        string str = this.Data["StaleRevisions"];
        return str == null ? new List<UpstreamRevision>() : new List<UpstreamRevision>(JsonConvert.DeserializeObject<IEnumerable<string>>(str).Select<string, UpstreamRevision>((Func<string, UpstreamRevision>) (x => JsonConvert.DeserializeObject<UpstreamRevision>(x))));
      }
      set
      {
        ArgumentUtility.CheckForNull<List<UpstreamRevision>>(value, "CurrentRevisionTable");
        this.Data["StaleRevisions"] = JsonConvert.SerializeObject((object) value.Select<UpstreamRevision, string>((Func<UpstreamRevision, string>) (x => JsonConvert.SerializeObject((object) x))));
      }
    }

    public Dictionary<string, uint> CurrentRevisionTable
    {
      get
      {
        string str = this.Data["Revisions"];
        return str == null ? new Dictionary<string, uint>() : JsonConvert.DeserializeObject<Dictionary<string, uint>>(str);
      }
      set
      {
        ArgumentUtility.CheckForNull<Dictionary<string, uint>>(value, nameof (CurrentRevisionTable));
        this.Data["Revisions"] = JsonConvert.SerializeObject((object) value);
      }
    }

    private int MetadataFormatVersion
    {
      get => int.Parse(this.Data[nameof (MetadataFormatVersion)]);
      set => this.Data[nameof (MetadataFormatVersion)] = value.ToString();
    }
  }
}
