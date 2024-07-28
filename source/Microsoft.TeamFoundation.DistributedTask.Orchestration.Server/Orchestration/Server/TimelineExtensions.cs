// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TimelineExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class TimelineExtensions
  {
    public static TimelineReference AsReference(this Timeline timeline) => new TimelineReference()
    {
      ChangeId = timeline.ChangeId,
      Id = timeline.Id
    };

    public static void UpdateLocations(
      this Timeline timeline,
      IVssRequestContext requestContext,
      Guid planId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      timeline.Location = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.Timelines_Compat, (object) new
      {
        planId = planId,
        timelineId = timeline.Id
      });
      if (timeline.Records == null)
        return;
      foreach (TimelineRecord record in timeline.Records)
        record.UpdateLocations(requestContext, planId, timeline.Id);
    }
  }
}
