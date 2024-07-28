// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HealthStatusMonitoringRecord
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class HealthStatusMonitoringRecord
  {
    public HealthStatusMonitoringRecord(Guid collectionId, Guid jobId)
      : this(collectionId, jobId, new ActionRunData())
    {
    }

    public HealthStatusMonitoringRecord(Guid hostId, Guid jobId, ActionRunData data)
    {
      this.HostId = hostId;
      this.JobId = jobId;
      this.ActionRunData = data;
    }

    public int Id { get; set; }

    public Guid HostId { get; }

    public Guid JobId { get; }

    public ActionRunData ActionRunData { get; set; }

    public string ActionName { get; set; }

    public JobStatus Status { get; set; }

    public DateTime LastUpdatedTimeStamp { get; set; }

    public DateTime CreatedTimeStamp { get; set; }
  }
}
