// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.RecentActivityDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D4A0500-806F-44D4-BA97-D409A2311716
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache
{
  public class RecentActivityDetails
  {
    public RecentActivityDetails()
    {
      this.LastSearchedTime = DateTime.UtcNow;
      this.Details = new Dictionary<int, ItemDetails>();
    }

    public DateTime LastSearchedTime { get; set; }

    public Dictionary<int, ItemDetails> Details { get; set; }

    public void Evict()
    {
      List<KeyValuePair<int, ItemDetails>> list = this.Details.ToList<KeyValuePair<int, ItemDetails>>();
      list.Sort((Comparison<KeyValuePair<int, ItemDetails>>) ((pair1, pair2) => pair1.Value.CompareTo(pair2.Value)));
      this.Details.Remove(list[0].Key);
    }

    public override string ToString() => "Last Searched Time: " + this.LastSearchedTime.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ", Details: {" + string.Join(",", this.Details.Select<KeyValuePair<int, ItemDetails>, string>((Func<KeyValuePair<int, ItemDetails>, string>) (kv => kv.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "=" + kv.Value.ToString())).ToArray<string>()) + "}";
  }
}
