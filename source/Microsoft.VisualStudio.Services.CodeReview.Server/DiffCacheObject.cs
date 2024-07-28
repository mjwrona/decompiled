// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.DiffCacheObject
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [DataContract]
  public class DiffCacheObject
  {
    [DataMember]
    public Dictionary<string, List<DiffCacheEntry>> DiffCacheEntries;

    public DiffCacheObject() => this.DiffCacheEntries = new Dictionary<string, List<DiffCacheEntry>>();

    public void AddEntry(string key, DiffCacheEntry cacheEntry)
    {
      if (!this.DiffCacheEntries.ContainsKey(key))
        this.DiffCacheEntries[key] = new List<DiffCacheEntry>();
      this.DiffCacheEntries[key].Add(cacheEntry);
    }

    public void CombineWith(DiffCacheObject otherCache)
    {
      if (otherCache == null)
        return;
      foreach (string key in otherCache.DiffCacheEntries.Keys)
        this.DiffCacheEntries[key] = otherCache.DiffCacheEntries[key];
    }

    public bool IsEmpty() => this.DiffCacheEntries.Count == 0;

    public bool IsSubsetOf(DiffCacheObject otherCache)
    {
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) this.DiffCacheEntries.Keys);
      return otherCache != null && stringSet.IsSubsetOf((IEnumerable<string>) new HashSet<string>((IEnumerable<string>) otherCache.DiffCacheEntries.Keys));
    }

    public int NumDiffFiles() => this.DiffCacheEntries.Count;

    public int NumCacheEntries()
    {
      int num = 0;
      foreach (List<DiffCacheEntry> diffCacheEntryList in this.DiffCacheEntries.Values)
        num += diffCacheEntryList.Count;
      return num;
    }
  }
}
