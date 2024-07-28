// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.LazyTimelineRecordContext
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public sealed class LazyTimelineRecordContext : ITimelineRecordContext, IDisposable
  {
    private readonly TaskHub hub;
    private readonly Guid projectId;
    private readonly Guid planId;
    private readonly Guid timelineId;
    private readonly Guid jobRecordId;
    private readonly TimelineRecord timelineRecord;
    private ITimelineRecordContext recordContext;
    private IVssRequestContext requestContext;

    public LazyTimelineRecordContext(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      Guid projectId,
      Guid planId,
      Guid timelineId,
      Guid jobRecordId,
      TimelineRecord timelineRecord)
    {
      this.requestContext = requestContext;
      this.hub = taskHub;
      this.projectId = projectId;
      this.planId = planId;
      this.timelineId = timelineId;
      this.jobRecordId = jobRecordId;
      this.timelineRecord = timelineRecord;
    }

    public Guid Id => this.GetRecordContext().Id;

    public void Dispose()
    {
      if (this.requestContext == null || this.recordContext == null)
        return;
      this.recordContext.Dispose();
      this.recordContext = (ITimelineRecordContext) null;
      this.requestContext = (IVssRequestContext) null;
    }

    public void WriteError(string format, params object[] arguments) => this.GetRecordContext().WriteError(format, arguments);

    public void WriteLine(string format, params object[] arguments) => this.GetRecordContext().WriteLine(format, arguments);

    public void WriteWarning(string format, params object[] arguments) => this.GetRecordContext().WriteWarning(format, arguments);

    private ITimelineRecordContext GetRecordContext()
    {
      if (this.recordContext == null)
      {
        this.hub.UpdateTimeline(this.requestContext, this.projectId, this.planId, this.timelineId, (IList<TimelineRecord>) new TimelineRecord[1]
        {
          this.timelineRecord
        });
        this.recordContext = (ITimelineRecordContext) new TimelineRecordContext(this.requestContext, this.hub, this.projectId, this.planId, this.timelineId, this.jobRecordId, this.timelineRecord);
      }
      return this.recordContext;
    }
  }
}
