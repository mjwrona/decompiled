// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegionCollection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract]
  public class RegionCollection
  {
    [DataMember]
    private PropertiesCollection m_regionWeights;

    public RegionCollection() => this.m_regionWeights = new PropertiesCollection();

    public RegionCollection(RegionCollection regionCollection) => this.m_regionWeights = new PropertiesCollection((IDictionary<string, object>) regionCollection.m_regionWeights);

    public RegionCollection(IDictionary<string, int> regionWeights)
    {
      this.m_regionWeights = new PropertiesCollection();
      foreach (KeyValuePair<string, int> regionWeight in (IEnumerable<KeyValuePair<string, int>>) regionWeights)
        this.m_regionWeights.Add(regionWeight.Key, (object) regionWeight.Value);
    }

    public int Count => this.m_regionWeights.Count;

    public void Clear() => this.m_regionWeights.Clear();

    public string[] GetSupportedRegions() => this.m_regionWeights.Keys.ToArray<string>();

    public bool IsRegionSupported(string region) => this.m_regionWeights.ContainsKey(region);

    public int GetRegionWeight(string region)
    {
      if (region == null)
        return this.m_regionWeights.Sum<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, int>) (regionWeight => (int) regionWeight.Value));
      int num;
      return this.m_regionWeights.TryGetValue<int>(region, out num) ? num : 0;
    }

    internal KeyValuePair<string, int>[] GetRegionWeights() => this.m_regionWeights.Select<KeyValuePair<string, object>, KeyValuePair<string, int>>((Func<KeyValuePair<string, object>, KeyValuePair<string, int>>) (kvp => new KeyValuePair<string, int>(kvp.Key, (int) kvp.Value))).ToArray<KeyValuePair<string, int>>();

    public void SetRegionWeight(string region, int weight)
    {
      ArgumentUtility.CheckForNull<string>(region, nameof (region));
      this.m_regionWeights[region] = (object) weight;
    }

    public bool CollectionEquals(RegionCollection other)
    {
      if (other == null || other.Count != this.Count)
        return false;
      foreach (KeyValuePair<string, int> regionWeight in other.GetRegionWeights())
      {
        if (!this.IsRegionSupported(regionWeight.Key) || regionWeight.Value != this.GetRegionWeight(regionWeight.Key))
          return false;
      }
      return true;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, object> regionWeight in (IEnumerable<KeyValuePair<string, object>>) this.m_regionWeights)
      {
        stringBuilder.Append(regionWeight.Key);
        stringBuilder.Append('=');
        stringBuilder.Append(regionWeight.Value);
        stringBuilder.Append(',');
      }
      --stringBuilder.Length;
      return stringBuilder.ToString();
    }
  }
}
