// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.LazyTimelineRecordContext
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class LazyTimelineRecordContext : ITimelineRecordContext, IDisposable
  {
    private TaskHub m_hub;
    private Guid m_projectId;
    private Guid m_planId;
    private Guid m_timelineId;
    private Guid m_jobRecordId;
    private TimelineRecord m_timelineRecord;
    private ITimelineRecordContext m_recordContext;
    private IVssRequestContext m_requestContext;
    private ITimelineRecordContext m_parentContext;

    public LazyTimelineRecordContext(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      Guid projectId,
      Guid planId,
      Guid timelineId,
      Guid jobRecordId,
      TimelineRecord timelineRecord,
      ITimelineRecordContext parentContext)
    {
      this.m_requestContext = requestContext;
      this.m_hub = taskHub;
      this.m_projectId = projectId;
      this.m_planId = planId;
      this.m_timelineId = timelineId;
      this.m_jobRecordId = jobRecordId;
      this.m_timelineRecord = timelineRecord;
      this.m_parentContext = parentContext;
    }

    public Guid Id => this.GetRecordContext().Id;

    public void Dispose()
    {
      if (this.m_requestContext == null || this.m_recordContext == null)
        return;
      this.m_recordContext.Dispose();
      this.m_recordContext = (ITimelineRecordContext) null;
      this.m_requestContext = (IVssRequestContext) null;
    }

    public void PropagateError() => this.GetRecordContext().PropagateError();

    public void WriteError(string format, params object[] arguments) => this.GetRecordContext().WriteError(format, arguments);

    public void WriteLine(string format, params object[] arguments) => this.GetRecordContext().WriteLine(format, arguments);

    public void WriteWarning(string format, params object[] arguments) => this.GetRecordContext().WriteWarning(format, arguments);

    private ITimelineRecordContext GetRecordContext()
    {
      if (this.m_recordContext == null)
      {
        this.m_hub.UpdateTimeline(this.m_requestContext, this.m_projectId, this.m_planId, this.m_timelineId, (IList<TimelineRecord>) new TimelineRecord[1]
        {
          this.m_timelineRecord
        });
        this.m_recordContext = (ITimelineRecordContext) new TimelineRecordContext(this.m_requestContext, this.m_hub, this.m_projectId, this.m_planId, this.m_timelineId, this.m_jobRecordId, this.m_timelineRecord, this.m_parentContext);
      }
      return this.m_recordContext;
    }
  }
}
