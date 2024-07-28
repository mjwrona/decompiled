// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostDictionaryWrapper`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HostDictionaryWrapper<T>
  {
    private HostDictionary<T> m_collections;
    private HostDictionary<T> m_organizations;

    public int Count => this.m_collections.Count + this.m_organizations.Count;

    public HostDictionaryWrapper(int capacity)
    {
      this.m_collections = new HostDictionary<T>(capacity);
      this.m_organizations = new HostDictionary<T>(capacity);
    }

    public void Add(HostProperties hostProperties, T host)
    {
      switch (hostProperties.HostType)
      {
        case TeamFoundationHostType.Application:
          this.m_organizations.Add(hostProperties, host);
          break;
        case TeamFoundationHostType.ProjectCollection:
          this.m_collections.Add(hostProperties, host);
          break;
        default:
          throw new ArgumentOutOfRangeException("HostType");
      }
    }

    public bool Remove(HostProperties hostProperties)
    {
      switch (hostProperties.HostType)
      {
        case TeamFoundationHostType.Application:
          return this.m_organizations.Remove(hostProperties.Id);
        case TeamFoundationHostType.ProjectCollection:
          return this.m_collections.Remove(hostProperties.Id);
        default:
          throw new ArgumentOutOfRangeException("HostType");
      }
    }

    public bool TryGetValue(HostProperties hostProperties, out T host)
    {
      switch (hostProperties.HostType)
      {
        case TeamFoundationHostType.Application:
          return this.m_organizations.TryGetValue(hostProperties.Id, out host);
        case TeamFoundationHostType.ProjectCollection:
          return this.m_collections.TryGetValue(hostProperties.Id, out host);
        default:
          throw new ArgumentOutOfRangeException("HostType");
      }
    }

    public void SetLastAccessTime(HostProperties hostProperties)
    {
      switch (hostProperties.HostType)
      {
        case TeamFoundationHostType.Application:
          this.m_organizations.SetLastAccessTime(hostProperties.Id);
          break;
        case TeamFoundationHostType.ProjectCollection:
          this.m_collections.SetLastAccessTime(hostProperties.Id);
          break;
        default:
          throw new ArgumentOutOfRangeException("HostType");
      }
    }

    public void ForceSetLastAccessTime(HostProperties hostProperties, DateTime lastAccessTime)
    {
      switch (hostProperties.HostType)
      {
        case TeamFoundationHostType.Application:
          this.m_organizations.ForceSetLastAccessTime(hostProperties.Id, lastAccessTime);
          break;
        case TeamFoundationHostType.ProjectCollection:
          this.m_collections.ForceSetLastAccessTime(hostProperties.Id, lastAccessTime);
          break;
        default:
          throw new ArgumentOutOfRangeException("HostType");
      }
    }

    public List<HostProperties> GetLeastRecentlyUsed(DateTime threshold)
    {
      List<HostProperties> leastRecentlyUsed1 = this.m_collections.GetLeastRecentlyUsed(threshold);
      List<HostProperties> leastRecentlyUsed2 = this.m_organizations.GetLeastRecentlyUsed(threshold);
      List<HostProperties> leastRecentlyUsed3;
      if (leastRecentlyUsed1 != null && leastRecentlyUsed1.Count > 0 && leastRecentlyUsed2 != null && leastRecentlyUsed2.Count > 0)
      {
        leastRecentlyUsed3 = leastRecentlyUsed1;
        HashSet<Guid> guidSet = new HashSet<Guid>(leastRecentlyUsed1.Count);
        for (int index = 0; index < leastRecentlyUsed1.Count; ++index)
        {
          if (!guidSet.Contains(leastRecentlyUsed1[index].ParentId))
            guidSet.Add(leastRecentlyUsed1[index].ParentId);
        }
        for (int index = 0; index < leastRecentlyUsed2.Count; ++index)
        {
          if (!guidSet.Contains(leastRecentlyUsed2[index].Id))
            leastRecentlyUsed3.Add(leastRecentlyUsed2[index]);
        }
      }
      else
        leastRecentlyUsed3 = leastRecentlyUsed1 == null || leastRecentlyUsed1.Count <= 0 ? leastRecentlyUsed2 : leastRecentlyUsed1;
      return leastRecentlyUsed3;
    }

    public List<HostProperties> GetLeastRecentlyUsed(int count)
    {
      List<HostProperties> leastRecentlyUsed = this.m_collections.GetLeastRecentlyUsed(count);
      if (leastRecentlyUsed == null || leastRecentlyUsed.Count == 0)
        leastRecentlyUsed = this.m_organizations.GetLeastRecentlyUsed(count);
      return leastRecentlyUsed;
    }

    public List<Guid> GetHostIds()
    {
      if (this.m_collections.Count > 0 && this.m_organizations.Count > 0)
      {
        List<Guid> hostIds = new List<Guid>(this.m_collections.Count + this.m_organizations.Count);
        hostIds.AddRange((IEnumerable<Guid>) this.m_collections.GetHostIds());
        hostIds.AddRange((IEnumerable<Guid>) this.m_organizations.GetHostIds());
        return hostIds;
      }
      if (this.m_collections.Count > 0)
        return this.m_collections.GetHostIds();
      return this.m_organizations.Count > 0 ? this.m_organizations.GetHostIds() : (List<Guid>) null;
    }

    public List<HostProperties> GetHostProperties()
    {
      if (this.m_collections.Count > 0 && this.m_organizations.Count > 0)
      {
        List<HostProperties> hostProperties = new List<HostProperties>(this.m_collections.Count + this.m_organizations.Count);
        hostProperties.AddRange((IEnumerable<HostProperties>) this.m_collections.GetHostProperties());
        hostProperties.AddRange((IEnumerable<HostProperties>) this.m_organizations.GetHostProperties());
        return hostProperties;
      }
      if (this.m_collections.Count > 0)
        return this.m_collections.GetHostProperties();
      return this.m_organizations.Count > 0 ? this.m_organizations.GetHostProperties() : (List<HostProperties>) null;
    }
  }
}
