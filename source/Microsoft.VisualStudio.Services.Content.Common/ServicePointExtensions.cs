// Decompiled with JetBrains decompiler
// Type: ServicePointExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;

public static class ServicePointExtensions
{
  private static ConcurrentDictionary<string, ServicePointExtensions.ServicePointConfig> cache = new ConcurrentDictionary<string, ServicePointExtensions.ServicePointConfig>();
  private static object Lock = new object();

  public static IEnumerable<string> GetConfigCacheKeys()
  {
    lock (ServicePointExtensions.Lock)
      return (IEnumerable<string>) ServicePointExtensions.cache.Keys.ToArray<string>();
  }

  public static void UpdateServicePointSettings(
    this ServicePoint servicePointToModify,
    ServicePointExtensions.ServicePointConfig config)
  {
    string key = ServicePointExtensions.MakeQueryString(servicePointToModify.Address);
    ServicePointExtensions.ServicePointConfig a = new ServicePointExtensions.ServicePointConfig();
    ServicePointExtensions.cache.TryGetValue(key, out a);
    ServicePointExtensions.ServicePointConfig updatedConfig = ServicePointExtensions.ServicePointConfig.Update(a, config);
    if (a.Equals((object) updatedConfig))
      return;
    lock (ServicePointExtensions.Lock)
    {
      ServicePointExtensions.cache.TryGetValue(key, out a);
      updatedConfig = ServicePointExtensions.ServicePointConfig.Update(a, config);
      if (a.Equals((object) updatedConfig))
        return;
      int? connectionsPerProcessor1 = a.MaxConnectionsPerProcessor;
      int? connectionsPerProcessor2 = updatedConfig.MaxConnectionsPerProcessor;
      if (!(connectionsPerProcessor1.GetValueOrDefault() == connectionsPerProcessor2.GetValueOrDefault() & connectionsPerProcessor1.HasValue == connectionsPerProcessor2.HasValue))
      {
        ServicePointExtensions.cache.AddOrUpdate(key, updatedConfig, (Func<string, ServicePointExtensions.ServicePointConfig, ServicePointExtensions.ServicePointConfig>) ((k, v) =>
        {
          v.MaxConnectionsPerProcessor = updatedConfig.MaxConnectionsPerProcessor;
          return v;
        }));
        servicePointToModify.ConnectionLimit = updatedConfig.MaxConnectionsPerProcessor.Value * Environment.ProcessorCount;
      }
      TimeSpan? nullable1 = a.ConnectionLeaseTimeout;
      TimeSpan? nullable2 = updatedConfig.ConnectionLeaseTimeout;
      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      {
        ServicePointExtensions.cache.AddOrUpdate(key, updatedConfig, (Func<string, ServicePointExtensions.ServicePointConfig, ServicePointExtensions.ServicePointConfig>) ((k, v) =>
        {
          v.ConnectionLeaseTimeout = updatedConfig.ConnectionLeaseTimeout;
          return v;
        }));
        servicePointToModify.ConnectionLeaseTimeout = (int) updatedConfig.ConnectionLeaseTimeout.Value.TotalMilliseconds;
      }
      bool? nullable3 = a.UseNagleAlgorithm;
      bool? useNagleAlgorithm = updatedConfig.UseNagleAlgorithm;
      if (!(nullable3.GetValueOrDefault() == useNagleAlgorithm.GetValueOrDefault() & nullable3.HasValue == useNagleAlgorithm.HasValue))
      {
        ServicePointExtensions.cache.AddOrUpdate(key, updatedConfig, (Func<string, ServicePointExtensions.ServicePointConfig, ServicePointExtensions.ServicePointConfig>) ((k, v) =>
        {
          v.UseNagleAlgorithm = updatedConfig.UseNagleAlgorithm;
          return v;
        }));
        servicePointToModify.UseNagleAlgorithm = updatedConfig.UseNagleAlgorithm.Value;
      }
      bool? expect100Continue = a.Expect100Continue;
      nullable3 = updatedConfig.Expect100Continue;
      if (!(expect100Continue.GetValueOrDefault() == nullable3.GetValueOrDefault() & expect100Continue.HasValue == nullable3.HasValue))
      {
        ServicePointExtensions.cache.AddOrUpdate(key, updatedConfig, (Func<string, ServicePointExtensions.ServicePointConfig, ServicePointExtensions.ServicePointConfig>) ((k, v) =>
        {
          v.Expect100Continue = updatedConfig.Expect100Continue;
          return v;
        }));
        servicePointToModify.Expect100Continue = updatedConfig.Expect100Continue.Value;
      }
      if (!a.TcpKeepAlive.Equals((object) updatedConfig.TcpKeepAlive))
      {
        ServicePointExtensions.cache.AddOrUpdate(key, updatedConfig, (Func<string, ServicePointExtensions.ServicePointConfig, ServicePointExtensions.ServicePointConfig>) ((k, v) =>
        {
          v.TcpKeepAlive = updatedConfig.TcpKeepAlive;
          return v;
        }));
        servicePointToModify.SetTcpKeepAlive(true, (int) updatedConfig.TcpKeepAlive.Value.KeepAliveTime.TotalMilliseconds, (int) updatedConfig.TcpKeepAlive.Value.KeepAliveInterval.TotalMilliseconds);
      }
      nullable2 = a.MaxIdleTime;
      nullable1 = updatedConfig.MaxIdleTime;
      if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != nullable1.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
        return;
      ServicePointExtensions.cache.AddOrUpdate(key, updatedConfig, (Func<string, ServicePointExtensions.ServicePointConfig, ServicePointExtensions.ServicePointConfig>) ((k, v) =>
      {
        v.MaxIdleTime = updatedConfig.MaxIdleTime;
        return v;
      }));
      servicePointToModify.MaxIdleTime = (int) updatedConfig.MaxIdleTime.Value.TotalMilliseconds;
    }
  }

  public static void UpdateServicePointSettings(
    this ServicePoint servicePointToModify,
    int maxConnectionsPerProcessor,
    TimeSpan? connectionLeaseTimeout = null)
  {
    ServicePointExtensions.ServicePointConfig config = new ServicePointExtensions.ServicePointConfig()
    {
      MaxConnectionsPerProcessor = new int?(maxConnectionsPerProcessor),
      ConnectionLeaseTimeout = connectionLeaseTimeout,
      Expect100Continue = new bool?(false),
      UseNagleAlgorithm = new bool?(false),
      TcpKeepAlive = new ServicePointExtensions.ServicePointConfigKeepAlive?(new ServicePointExtensions.ServicePointConfigKeepAlive()
      {
        KeepAliveTime = TimeSpan.FromSeconds(30.0),
        KeepAliveInterval = TimeSpan.FromSeconds(5.0)
      })
    };
    servicePointToModify.UpdateServicePointSettings(config);
  }

  private static string MakeQueryString(Uri address)
  {
    if (address.IsDefaultPort)
      return address.Scheme + "://" + address.DnsSafeHost;
    return address.Scheme + "://" + address.DnsSafeHost + ":" + address.Port.ToString();
  }

  public struct ServicePointConfigKeepAlive
  {
    public TimeSpan KeepAliveTime;
    public TimeSpan KeepAliveInterval;
  }

  public struct ServicePointConfig
  {
    public int? MaxConnectionsPerProcessor;
    public TimeSpan? ConnectionLeaseTimeout;
    public bool? UseNagleAlgorithm;
    public bool? Expect100Continue;
    public ServicePointExtensions.ServicePointConfigKeepAlive? TcpKeepAlive;
    public TimeSpan? MaxIdleTime;

    public static ServicePointExtensions.ServicePointConfig Update(
      ServicePointExtensions.ServicePointConfig a,
      ServicePointExtensions.ServicePointConfig b)
    {
      return new ServicePointExtensions.ServicePointConfig()
      {
        MaxConnectionsPerProcessor = b.MaxConnectionsPerProcessor.HasValue ? b.MaxConnectionsPerProcessor : a.MaxConnectionsPerProcessor,
        ConnectionLeaseTimeout = b.ConnectionLeaseTimeout.HasValue ? b.ConnectionLeaseTimeout : a.ConnectionLeaseTimeout,
        UseNagleAlgorithm = b.UseNagleAlgorithm.HasValue ? b.UseNagleAlgorithm : a.UseNagleAlgorithm,
        Expect100Continue = b.Expect100Continue.HasValue ? b.Expect100Continue : a.Expect100Continue,
        TcpKeepAlive = b.TcpKeepAlive.HasValue ? b.TcpKeepAlive : a.TcpKeepAlive,
        MaxIdleTime = b.MaxIdleTime.HasValue ? b.MaxIdleTime : a.MaxIdleTime
      };
    }
  }
}
