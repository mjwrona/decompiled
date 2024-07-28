// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.MetadataDBStamps
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class MetadataDBStamps : ReadOnlyDictionary<MetadataTable, long>
  {
    private static readonly System.Collections.Generic.Dictionary<MetadataTable, long> sm_emptyDict = new System.Collections.Generic.Dictionary<MetadataTable, long>();
    public static readonly MetadataDBStamps Empty = new MetadataDBStamps();
    public static readonly MetadataDBStamps Zero = new MetadataDBStamps((IDictionary<MetadataTable, long>) Enum.GetValues(typeof (MetadataTable)).Cast<MetadataTable>().ToDictionary<MetadataTable, MetadataTable, long>((Func<MetadataTable, MetadataTable>) (table => table), (Func<MetadataTable, long>) (table => 0L)));

    public MetadataDBStamps(IDictionary<MetadataTable, long> stamps)
      : base(stamps)
    {
    }

    public MetadataDBStamps()
      : base((IDictionary<MetadataTable, long>) MetadataDBStamps.sm_emptyDict)
    {
    }

    public long GetMax() => this.Max<KeyValuePair<MetadataTable, long>>((Func<KeyValuePair<MetadataTable, long>, long>) (kvp => kvp.Value));
  }
}
