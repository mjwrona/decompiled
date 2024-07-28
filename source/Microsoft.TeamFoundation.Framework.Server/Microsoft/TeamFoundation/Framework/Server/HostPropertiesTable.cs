// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostPropertiesTable
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostPropertiesTable
  {
    private ConcurrentDictionary<Guid, HostProperties> m_serviceHostProperties;
    private ConcurrentDictionary<Guid, List<Guid>> m_childrenHostsIndex;

    public HostPropertiesTable()
    {
      this.m_serviceHostProperties = new ConcurrentDictionary<Guid, HostProperties>();
      this.m_childrenHostsIndex = new ConcurrentDictionary<Guid, List<Guid>>();
    }

    public bool ContainsKey(Guid hostId) => this.m_serviceHostProperties.ContainsKey(hostId);

    public void UpdateProperties(Action<HostProperties> updater)
    {
      foreach (HostProperties hostProperties in (IEnumerable<HostProperties>) this.m_serviceHostProperties.Values)
        updater(hostProperties);
    }

    public HostProperties this[Guid hostId] => this.m_serviceHostProperties[hostId];

    public void Add(HostProperties properties)
    {
      List<Guid> addValue = new List<Guid>();
      addValue.Add(properties.Id);
      this.m_serviceHostProperties.TryAdd(properties.Id, properties);
      if (properties.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      this.m_childrenHostsIndex.AddOrUpdate(properties.ParentId, addValue, (Func<Guid, List<Guid>, List<Guid>>) ((g, children) => new List<Guid>((IEnumerable<Guid>) children)
      {
        properties.Id
      }));
    }

    public bool TryGetValue(Guid hostId, out HostProperties properties) => this.m_serviceHostProperties.TryGetValue(hostId, out properties);

    public bool Remove(Guid hostId)
    {
      HostProperties hostProperties;
      if (!this.m_serviceHostProperties.TryRemove(hostId, out hostProperties))
        return false;
      if (hostProperties.HostType == TeamFoundationHostType.ProjectCollection)
      {
        bool flag;
        do
        {
          List<Guid> guidList;
          if (this.m_childrenHostsIndex.TryGetValue(hostProperties.ParentId, out guidList) && guidList.Contains(hostId))
          {
            List<Guid> newValue = new List<Guid>((IEnumerable<Guid>) guidList);
            newValue.Remove(hostId);
            flag = this.m_childrenHostsIndex.TryUpdate(hostProperties.ParentId, newValue, guidList);
          }
          else
            flag = true;
        }
        while (!flag);
      }
      return true;
    }

    public List<HostProperties> GetServiceHostChildrenProperties(Guid hostId)
    {
      HostProperties hostProperties;
      if (!this.m_serviceHostProperties.TryGetValue(hostId, out hostProperties))
        return (List<HostProperties>) null;
      List<HostProperties> childrenProperties = new List<HostProperties>();
      if (hostProperties.HostType == TeamFoundationHostType.Application)
      {
        List<Guid> guidList = (List<Guid>) null;
        this.m_childrenHostsIndex.TryGetValue(hostId, out guidList);
        if (guidList != null && guidList.Count > 0)
        {
          foreach (Guid key in guidList)
            childrenProperties.Add(new HostProperties(this.m_serviceHostProperties[key]));
        }
      }
      else if ((hostProperties.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
      {
        foreach (HostProperties other in (IEnumerable<HostProperties>) this.m_serviceHostProperties.Values)
        {
          if (other.ParentId == hostId)
            childrenProperties.Add(new HostProperties(other));
        }
      }
      return childrenProperties;
    }

    public void Clear()
    {
      this.m_serviceHostProperties.Clear();
      this.m_childrenHostsIndex.Clear();
    }

    public List<Guid> GetChildrenHosts(Guid hostId)
    {
      List<Guid> childrenHosts;
      this.m_childrenHostsIndex.TryGetValue(hostId, out childrenHosts);
      return childrenHosts;
    }

    public string DumpHostPropertiesTable()
    {
      StringBuilder stringBuilder = new StringBuilder(1024);
      List<HostProperties> list = this.m_serviceHostProperties.Values.ToList<HostProperties>();
      list.Sort((Comparison<HostProperties>) ((h1, h2) => h1.Id.CompareTo(h2.Id)));
      stringBuilder.AppendLine("[");
      foreach (HostProperties hostProperties in list)
      {
        List<HostProperties> childrenProperties = this.GetServiceHostChildrenProperties(hostProperties.Id);
        stringBuilder.AppendFormat("{{ \"Id\":\"{0}\", \"Name\":\"{1}\", \"Children\":\"{2}\" }},\r\n", (object) hostProperties.Id, (object) hostProperties.Name, childrenProperties == null ? (object) string.Empty : (object) string.Join(",", (IEnumerable<string>) childrenProperties.Select<HostProperties, string>((Func<HostProperties, string>) (x => x.Name)).OrderBy<string, string>((Func<string, string>) (x => x), (IComparer<string>) StringComparer.Ordinal)));
      }
      stringBuilder.AppendLine("]");
      return stringBuilder.ToString();
    }
  }
}
