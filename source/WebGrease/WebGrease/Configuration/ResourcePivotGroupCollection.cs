// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.ResourcePivotGroupCollection
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebGrease.Configuration
{
  public class ResourcePivotGroupCollection : IEnumerable<ResourcePivotGroup>, IEnumerable
  {
    private readonly IDictionary<string, ResourcePivotGroup> resourcePivots = (IDictionary<string, ResourcePivotGroup>) new Dictionary<string, ResourcePivotGroup>();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public ResourcePivotGroup this[string groupKey]
    {
      get
      {
        ResourcePivotGroup resourcePivotGroup;
        return this.resourcePivots.TryGetValue(groupKey, out resourcePivotGroup) ? resourcePivotGroup : (ResourcePivotGroup) null;
      }
    }

    public IEnumerator<ResourcePivotGroup> GetEnumerator() => this.resourcePivots.Values.GetEnumerator();

    internal void Clear(string groupKey) => this[groupKey]?.Keys.Clear();

    internal void Set(string groupKey, ResourcePivotApplyMode? applyMode, IEnumerable<string> keys)
    {
      ResourcePivotGroup resourcePivotGroup1 = this[groupKey];
      ResourcePivotGroup resourcePivotGroup2 = resourcePivotGroup1 == null ? new ResourcePivotGroup(groupKey, (ResourcePivotApplyMode) ((int) applyMode ?? 0), keys) : new ResourcePivotGroup(groupKey, (ResourcePivotApplyMode) ((int) applyMode ?? (int) resourcePivotGroup1.ApplyMode), resourcePivotGroup1.Keys.Concat<string>(keys));
      this.resourcePivots[groupKey] = resourcePivotGroup2;
    }
  }
}
