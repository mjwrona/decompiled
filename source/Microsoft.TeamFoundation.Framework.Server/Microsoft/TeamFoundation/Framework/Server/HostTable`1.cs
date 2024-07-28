// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostTable`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostTable<T> where T : class
  {
    private T m_deploymentServiceHost;
    private HostProperties m_deploymentHostProperties;
    private readonly HostDictionaryWrapper<T>[] m_serviceHosts;
    private readonly ILockName[] m_hostTableLock;
    private LockManager m_lockManager;
    private static readonly string s_Area = "HostManagement";
    private static readonly string s_Layer = nameof (HostTable<T>);
    private int m_count;
    private readonly int m_numberOfPartitions;

    public HostTable(LockManager lockManager, int numberOfPartitions)
    {
      this.m_lockManager = lockManager;
      this.m_numberOfPartitions = numberOfPartitions;
      this.m_serviceHosts = new HostDictionaryWrapper<T>[this.m_numberOfPartitions];
      this.m_hostTableLock = (ILockName[]) new LockName<string, int>[this.m_numberOfPartitions];
      this.m_count = 0;
      for (int nameValue2 = 0; nameValue2 < this.m_numberOfPartitions; ++nameValue2)
      {
        this.m_hostTableLock[nameValue2] = (ILockName) new LockName<string, int>("Host Table", nameValue2, LockLevel.Last);
        this.m_serviceHosts[nameValue2] = new HostDictionaryWrapper<T>(100);
      }
    }

    public int NumberOfPartitions => this.m_numberOfPartitions;

    public int Count => this.m_count;

    public void SetLastAccessTime(HostProperties hostProperties, long requestId)
    {
      if ((hostProperties.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        return;
      using (this.GetHostTableLock(hostProperties, LockManager.LockType.MapLockExclusive, requestId))
        this.GetServiceHosts(hostProperties).SetLastAccessTime(hostProperties);
    }

    public void ForceSetLastAccessTime(
      HostProperties hostProperties,
      long requestId,
      DateTime lastAccessTime)
    {
      if ((hostProperties.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        return;
      using (this.GetHostTableLock(hostProperties, LockManager.LockType.MapLockExclusive, requestId))
        this.GetServiceHosts(hostProperties).ForceSetLastAccessTime(hostProperties, lastAccessTime);
    }

    public bool RemoveHost(HostProperties hostProperties, long requestId)
    {
      this.TraceRaw(58176, TraceLevel.Info, hostProperties.Id, "Removing host {0} from the host table", (object) hostProperties.Name);
      if ((hostProperties.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
      {
        T deploymentServiceHost = this.m_deploymentServiceHost;
        this.m_deploymentServiceHost = default (T);
        this.m_deploymentHostProperties = (HostProperties) null;
        return (object) deploymentServiceHost != null;
      }
      using (this.GetHostTableLock(hostProperties, LockManager.LockType.MapLockExclusive, requestId))
      {
        int num = this.GetServiceHosts(hostProperties).Remove(hostProperties) ? 1 : 0;
        if (num != 0)
          Interlocked.Decrement(ref this.m_count);
        return num != 0;
      }
    }

    public int GetPartitionSize(long requestId, HostProperties hostProperties)
    {
      using (this.GetHostTableLock(hostProperties, LockManager.LockType.MapLockShared, requestId))
        return this.GetServiceHosts(hostProperties).Count;
    }

    public void AddHost(HostProperties properties, T host, long requestId)
    {
      if ((properties.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
      {
        this.m_deploymentServiceHost = (object) this.m_deploymentServiceHost == null ? host : throw new InvalidOperationException("We do not support adding another deployment host - something is very wrong");
        this.m_deploymentHostProperties = properties;
        Interlocked.Increment(ref this.m_count);
      }
      else
      {
        using (this.GetHostTableLock(properties, LockManager.LockType.MapLockExclusive, requestId))
        {
          this.GetServiceHosts(properties).Add(properties, host);
          Interlocked.Increment(ref this.m_count);
        }
      }
    }

    public bool TryGetHost(HostProperties properties, long requestId, out T host)
    {
      if ((properties.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
      {
        host = this.m_deploymentServiceHost;
        return (object) this.m_deploymentServiceHost != null;
      }
      using (this.GetHostTableLock(properties, LockManager.LockType.MapLockShared, requestId))
        return this.GetServiceHosts(properties).TryGetValue(properties, out host);
    }

    private NamedLockFrame GetHostTableLock(
      HostProperties properties,
      LockManager.LockType lockType,
      long requestId)
    {
      return this.m_lockManager.Lock(this.m_hostTableLock[this.GetPartitionId(properties)], lockType, requestId);
    }

    public int GetPartitionId(HostProperties properties) => ((properties.HostType != TeamFoundationHostType.ProjectCollection ? properties.Id : properties.ParentId).GetHashCode() & int.MaxValue) % this.m_numberOfPartitions;

    private HostDictionaryWrapper<T> GetServiceHosts(HostProperties properties) => this.m_serviceHosts[this.GetPartitionId(properties)];

    private void TraceRaw(
      int tracepoint,
      TraceLevel level,
      Guid hostId,
      string message,
      params object[] args)
    {
      TraceEvent trace = new TraceEvent(message, args);
      TeamFoundationTracingService.GetTraceEvent(ref trace, tracepoint, level, HostTable<T>.s_Area, HostTable<T>.s_Layer, (string[]) null, (string) null);
      trace.ServiceHost = hostId;
      TeamFoundationTracingService.TraceRaw(ref trace);
    }

    public List<Guid> GetAllHostIds(long requestId)
    {
      List<Guid> allHostIds = new List<Guid>(this.m_count);
      for (int index = 0; index < this.m_numberOfPartitions; ++index)
      {
        using (this.m_lockManager.Lock(this.m_hostTableLock[index], LockManager.LockType.MapLockShared, requestId))
        {
          List<Guid> hostIds = this.m_serviceHosts[index].GetHostIds();
          if (hostIds != null)
          {
            if (hostIds.Count > 0)
              allHostIds.AddRange((IEnumerable<Guid>) hostIds);
          }
        }
      }
      allHostIds.Add(this.m_deploymentHostProperties.Id);
      return allHostIds;
    }

    public List<HostProperties> GetAllHostProperties(long requestId)
    {
      List<HostProperties> allHostProperties = new List<HostProperties>(this.m_count);
      for (int index = 0; index < this.m_numberOfPartitions; ++index)
      {
        using (this.m_lockManager.Lock(this.m_hostTableLock[index], LockManager.LockType.MapLockShared, requestId))
        {
          List<HostProperties> hostProperties = this.m_serviceHosts[index].GetHostProperties();
          if (hostProperties != null)
          {
            if (hostProperties.Count > 0)
              allHostProperties.AddRange((IEnumerable<HostProperties>) hostProperties);
          }
        }
      }
      if (this.m_deploymentHostProperties != null)
        allHostProperties.Add(this.m_deploymentHostProperties);
      return allHostProperties;
    }

    public List<HostProperties> GetLeastRecentlyUsed(int partitionId, long requestId, int count)
    {
      ArgumentUtility.CheckForOutOfRange(partitionId, nameof (partitionId), 0, this.m_numberOfPartitions);
      using (this.m_lockManager.Lock(this.m_hostTableLock[partitionId], LockManager.LockType.MapLockShared, requestId))
        return this.m_serviceHosts[partitionId].GetLeastRecentlyUsed(count);
    }

    public List<HostProperties> CheckForDormancyCandidates(
      int partitionId,
      long requestId,
      DateTime utcThreshold)
    {
      ArgumentUtility.CheckForOutOfRange(partitionId, nameof (partitionId), 0, this.m_numberOfPartitions);
      using (this.m_lockManager.Lock(this.m_hostTableLock[partitionId], LockManager.LockType.MapLockShared, requestId))
        return this.m_serviceHosts[partitionId].GetLeastRecentlyUsed(utcThreshold);
    }
  }
}
