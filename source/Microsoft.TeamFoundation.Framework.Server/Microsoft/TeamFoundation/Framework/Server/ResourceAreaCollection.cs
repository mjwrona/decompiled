// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceAreaCollection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ResourceAreaCollection : 
    ICollection<ResourceArea>,
    IEnumerable<ResourceArea>,
    IEnumerable
  {
    private Dictionary<Guid, ResourceArea> m_resourceAreas = new Dictionary<Guid, ResourceArea>();

    public int Count => this.m_resourceAreas.Count;

    public bool IsReadOnly => false;

    public ResourceArea this[Guid areaId]
    {
      get
      {
        ResourceArea resourceArea = (ResourceArea) null;
        this.m_resourceAreas.TryGetValue(areaId, out resourceArea);
        return resourceArea;
      }
    }

    public virtual void RegisterArea(string areaName, Guid areaId) => this.Add(new ResourceArea(areaName, areaId));

    public virtual void RegisterArea(string areaName, string areaGuid)
    {
      Guid areaId = Guid.Parse(areaGuid);
      this.Add(new ResourceArea(areaName, areaId));
    }

    public void Add(ResourceArea item) => this.m_resourceAreas[item.AreaId] = item;

    public void Clear() => throw new InvalidOperationException();

    public bool Contains(ResourceArea item) => this.m_resourceAreas.ContainsKey(item.AreaId);

    public void CopyTo(ResourceArea[] array, int arrayIndex) => this.m_resourceAreas.Values.CopyTo(array, arrayIndex);

    public bool Remove(ResourceArea item) => throw new InvalidOperationException();

    public IEnumerator<ResourceArea> GetEnumerator() => (IEnumerator<ResourceArea>) this.m_resourceAreas.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_resourceAreas.Values.GetEnumerator();
  }
}
