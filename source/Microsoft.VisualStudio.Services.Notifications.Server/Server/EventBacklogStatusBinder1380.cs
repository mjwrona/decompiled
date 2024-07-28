// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventBacklogStatusBinder1380
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventBacklogStatusBinder1380 : EventBacklogStatusBinder1200
  {
    protected SqlColumnBinder OldestCreatedTimeColumn = new SqlColumnBinder("OldestCreatedTime");
    protected SqlColumnBinder CaptureTimeColumn = new SqlColumnBinder("CaptureTime");

    protected override EventBacklogStatus Bind()
    {
      EventBacklogStatus eventBacklogStatus = new EventBacklogStatus()
      {
        UnprocessedEvents = this.UnprocessedEventsColumn.GetInt32((IDataReader) this.Reader),
        OldestPendingEventTime = this.OldestCreatedTimeColumn.GetDateTime((IDataReader) this.Reader),
        Publisher = this.PublisherColumn.GetString((IDataReader) this.Reader, true),
        CaptureTime = this.CaptureTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue)
      };
      if (eventBacklogStatus.CaptureTime <= DateTime.MinValue)
        eventBacklogStatus.CaptureTime = DateTime.UtcNow;
      return eventBacklogStatus;
    }
  }
}
