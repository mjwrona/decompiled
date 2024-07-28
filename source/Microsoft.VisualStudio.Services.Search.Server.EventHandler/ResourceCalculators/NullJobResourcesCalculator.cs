// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.ResourceCalculators.NullJobResourcesCalculator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler.ResourceCalculators
{
  public class NullJobResourcesCalculator : BaseJobResourcesCalculator
  {
    internal override JobResourcesResponse GetResourcesToQueueJobs(
      ExecutionContext executionContext,
      JobResourcesRequest jobResourcesRequest)
    {
      TimeSpan queueDelayFactor = new JobQueueController(executionContext.RequestContext).GetQueueDelayFactor();
      return new JobResourcesResponse()
      {
        AllottedResources = jobResourcesRequest.RequestedResources,
        DelayToQueueCallback = TimeSpan.FromMinutes(-1.0),
        DelayToQueueJobs = queueDelayFactor
      };
    }

    internal override void RefreshCurrentResourceConsumptionData(IVssRequestContext requestContext)
    {
    }
  }
}
