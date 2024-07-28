// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.MetricsJobUtils
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public static class MetricsJobUtils
  {
    public static void QueueMetricsAggregationJob(IVssRequestContext requestContext, Guid jobId)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueJobsNow(requestContext.To(TeamFoundationHostType.Deployment), (IEnumerable<Guid>) new List<Guid>()
        {
          jobId
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019117, "Feed", "FeedMetrics", ex);
      }
    }
  }
}
