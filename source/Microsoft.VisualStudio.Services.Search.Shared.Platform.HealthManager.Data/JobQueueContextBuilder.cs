// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.JobQueueContextBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class JobQueueContextBuilder : IProviderContextBuilder
  {
    public ProviderContext BuildContext(
      IVssRequestContext requestContext,
      Scenario scenario,
      HealthStatusJobData healthInputData,
      CollectionIndexingProperties indexProperties)
    {
      JobQueueContext jobQueueContext = new JobQueueContext();
      jobQueueContext.RequestContext = requestContext;
      jobQueueContext.JobIds = new Dictionary<Guid, IEntityType>();
      if (healthInputData.EntityType.Name == "Code")
        jobQueueContext.JobIds[new Guid("02F271F3-0D40-4FA0-9328-C77EBCA59B6F")] = (IEntityType) CodeEntityType.GetInstance();
      return (ProviderContext) jobQueueContext;
    }
  }
}
