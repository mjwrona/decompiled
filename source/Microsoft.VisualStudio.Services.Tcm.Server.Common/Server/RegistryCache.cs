// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RegistryCache
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class RegistryCache
  {
    private static ConcurrentDictionary<Guid, long> maxAttachmentSize = new ConcurrentDictionary<Guid, long>();
    private static ConcurrentDictionary<Guid, DateTime> lastUpdated = new ConcurrentDictionary<Guid, DateTime>();
    private const string maxAttachmentSizeForProxyPath = "/Service/TestManagement/Configuration/MaxAttachmentSizeForProxy";

    public static long GetMaxAttachmentSizeForProxy(IVssRequestContext context)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetMaxAttachmentSizeForProxy), "Registry")))
      {
        if (RegistryCache.maxAttachmentSize.ContainsKey(context.ServiceHost.InstanceId) && RegistryCache.lastUpdated.ContainsKey(context.ServiceHost.InstanceId) && (DateTime.UtcNow - RegistryCache.lastUpdated[context.ServiceHost.InstanceId]).TotalMinutes < 5.0)
          return RegistryCache.maxAttachmentSize[context.ServiceHost.InstanceId];
        long result;
        if (long.TryParse(context.GetService<IVssRegistryService>().GetValue(context, (RegistryQuery) "/Service/TestManagement/Configuration/MaxAttachmentSizeForProxy", false, (string) null), out result))
        {
          RegistryCache.maxAttachmentSize[context.ServiceHost.InstanceId] = result;
          RegistryCache.lastUpdated[context.ServiceHost.InstanceId] = DateTime.UtcNow;
          return result;
        }
        if (long.TryParse(context.To(TeamFoundationHostType.Application).GetService<IVssRegistryService>().GetValue(context.To(TeamFoundationHostType.Application), (RegistryQuery) "/Service/TestManagement/Configuration/MaxAttachmentSizeForProxy", false, (string) null), out result))
        {
          RegistryCache.maxAttachmentSize[context.ServiceHost.InstanceId] = result;
          RegistryCache.lastUpdated[context.ServiceHost.InstanceId] = DateTime.UtcNow;
          return result;
        }
        long maxValue = long.MaxValue;
        RegistryCache.maxAttachmentSize[context.ServiceHost.InstanceId] = maxValue;
        RegistryCache.lastUpdated[context.ServiceHost.InstanceId] = DateTime.UtcNow;
        return maxValue;
      }
    }
  }
}
