// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostDictionary`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HostDictionary<T>
  {
    private readonly Dictionary<Guid, LinkedListNode<HostDictionary<T>.HostInformation>> m_hosts;
    private readonly LinkedList<HostDictionary<T>.HostInformation> m_lastAccessTime;

    public HostDictionary(int capacity)
    {
      this.m_hosts = new Dictionary<Guid, LinkedListNode<HostDictionary<T>.HostInformation>>(capacity);
      this.m_lastAccessTime = new LinkedList<HostDictionary<T>.HostInformation>();
    }

    public int Count => this.m_hosts.Count;

    public void Add(HostProperties hostProperties, T host)
    {
      HostDictionary<T>.HostInformation hostInformation = new HostDictionary<T>.HostInformation()
      {
        LastAccessTime = DateTime.UtcNow,
        Host = host,
        HostProperties = hostProperties
      };
      this.m_hosts.Add(hostProperties.Id, this.m_lastAccessTime.AddFirst(hostInformation));
    }

    public bool Remove(Guid hostId)
    {
      LinkedListNode<HostDictionary<T>.HostInformation> host = this.m_hosts[hostId];
      if (host != null)
        this.m_lastAccessTime.Remove(host);
      return this.m_hosts.Remove(hostId);
    }

    public bool TryGetValue(Guid hostId, out T host)
    {
      LinkedListNode<HostDictionary<T>.HostInformation> linkedListNode;
      if (this.m_hosts.TryGetValue(hostId, out linkedListNode))
      {
        host = linkedListNode.Value.Host;
        return true;
      }
      host = default (T);
      return false;
    }

    public void ForceSetLastAccessTime(Guid hostId, DateTime lastAccessTime)
    {
      LinkedListNode<HostDictionary<T>.HostInformation> host = this.m_hosts[hostId];
      if (host == null)
        return;
      host.Value.LastAccessTime = lastAccessTime;
      this.m_lastAccessTime.Remove(host);
      for (LinkedListNode<HostDictionary<T>.HostInformation> node = this.m_lastAccessTime.First; node != null; node = node.Next)
      {
        if (node.Value.LastAccessTime < lastAccessTime)
        {
          this.m_lastAccessTime.AddBefore(node, host);
          return;
        }
      }
      this.m_lastAccessTime.AddLast(host);
    }

    public void SetLastAccessTime(Guid hostId)
    {
      LinkedListNode<HostDictionary<T>.HostInformation> host = this.m_hosts[hostId];
      if (this.m_lastAccessTime.First == host)
        return;
      this.m_lastAccessTime.Remove(host);
      this.m_lastAccessTime.AddFirst(host);
    }

    public List<HostProperties> GetLeastRecentlyUsed(DateTime threshold)
    {
      List<HostProperties> leastRecentlyUsed = (List<HostProperties>) null;
      int capacity = 0;
      for (LinkedListNode<HostDictionary<T>.HostInformation> linkedListNode = this.m_lastAccessTime.Last; linkedListNode != null && linkedListNode.Value.LastAccessTime <= threshold; linkedListNode = linkedListNode.Previous)
        ++capacity;
      if (capacity > 0)
      {
        leastRecentlyUsed = new List<HostProperties>(capacity);
        for (LinkedListNode<HostDictionary<T>.HostInformation> linkedListNode = this.m_lastAccessTime.Last; linkedListNode != null && linkedListNode.Value.LastAccessTime <= threshold; linkedListNode = linkedListNode.Previous)
          leastRecentlyUsed.Add(linkedListNode.Value.HostProperties);
      }
      return leastRecentlyUsed;
    }

    public List<HostProperties> GetLeastRecentlyUsed(int numberOfHosts)
    {
      List<HostProperties> leastRecentlyUsed = new List<HostProperties>(Math.Min(numberOfHosts, this.m_hosts.Count));
      int num = 0;
      for (LinkedListNode<HostDictionary<T>.HostInformation> linkedListNode = this.m_lastAccessTime.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
      {
        leastRecentlyUsed.Add(linkedListNode.Value.HostProperties);
        if (++num == numberOfHosts)
          break;
      }
      return leastRecentlyUsed;
    }

    public List<Guid> GetHostIds() => this.m_hosts.Keys.ToList<Guid>();

    public List<HostProperties> GetHostProperties() => this.m_hosts.Values.Select<LinkedListNode<HostDictionary<T>.HostInformation>, HostProperties>((Func<LinkedListNode<HostDictionary<T>.HostInformation>, HostProperties>) (node => node.Value.HostProperties)).ToList<HostProperties>();

    private class HostInformation
    {
      public DateTime LastAccessTime { get; set; }

      public T Host { get; set; }

      public HostProperties HostProperties { get; set; }
    }
  }
}
