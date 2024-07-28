// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Framework.Server.StartedChangeRecordInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Framework.Server
{
  public class StartedChangeRecordInfo
  {
    public string RecordId { get; private set; }

    public DateTimeOffset StartTime { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public byte Priority { get; private set; }

    public StartedChangeRecordInfo(
      string recordId,
      string title,
      string description,
      DateTimeOffset startTime,
      byte priority)
    {
      this.RecordId = recordId;
      this.Title = title;
      this.Description = description;
      this.StartTime = startTime;
      this.Priority = priority;
    }
  }
}
