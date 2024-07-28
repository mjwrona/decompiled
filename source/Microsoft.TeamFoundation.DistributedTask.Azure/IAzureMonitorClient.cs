// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.IAzureMonitorClient
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Azure
{
  public interface IAzureMonitorClient : ICanExpire
  {
    Task DeleteAutoscaleSetting(
      Guid activityId,
      AutoscaleSettingResource autoscaleSettingResource,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<AutoscaleSettingResource>> ListAutoscaleSettings(
      Guid activityId,
      Guid subscriptionId,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
