// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.CoreJobResourceUtilizationController
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  public sealed class CoreJobResourceUtilizationController : IJobResourceUtilizationController
  {
    public IEntityType[] SupportedEntityTypes => new IEntityType[1]
    {
      (IEntityType) AllEntityType.GetInstance()
    };

    public void Initialize(IVssRequestContext requestContext)
    {
    }

    public JobResourcesResponse GetResourcesToQueueJobs(
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
  }
}
