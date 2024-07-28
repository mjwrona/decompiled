// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HealthStatusRecord
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class HealthStatusRecord
  {
    public HealthStatusRecord(Guid collectionId, string jobName)
      : this(collectionId, jobName, new HealthStatusJobData())
    {
    }

    public HealthStatusRecord(Guid collectionId, string jobName, HealthStatusJobData data)
    {
      this.CollectionId = collectionId;
      this.JobName = jobName;
      this.Data = data;
    }

    public int Id { get; set; }

    public Guid CollectionId { get; }

    public string JobName { get; }

    public HealthStatusJobData Data { get; set; }

    public JobMode Mode { get; set; }

    public DateTime LastUpdatedTimeStamp { get; set; }

    public JobStatus Status { get; set; }

    public DateTime CreationTime { get; set; }
  }
}
