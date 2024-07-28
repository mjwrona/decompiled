// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.IAzureMonitorResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  [DefaultServiceImplementation(typeof (AzureMonitorResourceService))]
  internal interface IAzureMonitorResourceService : IVssFrameworkService
  {
    Task DeleteAutoscaleSettingAsync(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope,
      AutoscaleSettingResource autoscaleSetting);

    Task<IEnumerable<AutoscaleSettingResource>> ListAutoscaleSettingsInSubscriptionAsync(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope);
  }
}
