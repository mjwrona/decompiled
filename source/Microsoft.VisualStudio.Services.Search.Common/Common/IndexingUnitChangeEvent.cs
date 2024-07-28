// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class IndexingUnitChangeEvent
  {
    public IndexingUnitChangeEvent()
    {
    }

    public IndexingUnitChangeEvent(string leaseId) => this.LeaseId = leaseId;

    public long Id { get; set; }

    public int IndexingUnitId { get; set; }

    public string ChangeType { get; set; }

    public ChangeEventData ChangeData { get; set; }

    public DateTime CreatedTimeUtc { get; set; }

    public string CurrentStage { get; set; }

    public IndexingUnitChangeEventState State { get; set; }

    public byte AttemptCount { get; set; }

    public DateTime LastModificationTimeUtc { get; set; }

    public IndexingUnitChangeEventPrerequisites Prerequisites { get; set; }

    public string LeaseId { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(Id: ");
      stringBuilder.Append(this.Id);
      stringBuilder.Append(", IndexingUnitId: ");
      stringBuilder.Append(this.IndexingUnitId);
      stringBuilder.Append(", ChangeType: ");
      stringBuilder.Append(this.ChangeType);
      stringBuilder.Append(", CurrentStage: ");
      stringBuilder.Append(this.CurrentStage);
      stringBuilder.Append(", State: ");
      stringBuilder.Append((object) this.State);
      stringBuilder.Append(", AttemptCount: ");
      stringBuilder.Append(this.AttemptCount);
      stringBuilder.Append(", CreatedTimeUtc: ");
      stringBuilder.Append((object) this.CreatedTimeUtc);
      stringBuilder.Append(", LastModificationTimeUtc: ");
      stringBuilder.Append((object) this.LastModificationTimeUtc);
      stringBuilder.Append(", LeaseId: ");
      stringBuilder.Append(this.LeaseId);
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }
  }
}
