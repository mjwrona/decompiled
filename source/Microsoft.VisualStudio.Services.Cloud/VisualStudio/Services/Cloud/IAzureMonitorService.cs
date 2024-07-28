// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IAzureMonitorService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.ResourceManager.Monitor.Models;
using Azure.ResourceManager.Redis;
using Azure.ResourceManager.Resources;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DefaultServiceImplementation(typeof (AzureMonitorService))]
  public interface IAzureMonitorService : IVssFrameworkService
  {
    HashSet<GenericResource> GetAzureResources(
      IVssRequestContext requestContext,
      string resourceType);

    MonitorMetric[] GetAzureResourceMetricData(
      IVssRequestContext requestContext,
      string resourceId,
      string metricNames,
      string aggregation,
      DateTime startTime,
      DateTime endTime,
      TimeSpan interval);

    RedisResource[] GetRedisResources(IVssRequestContext requestContext, string resourceGroupName);
  }
}
