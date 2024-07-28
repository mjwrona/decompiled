// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordFeedLinesWrapper
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TimelineRecordFeedLinesWrapper
  {
    public TimelineRecordFeedLinesWrapper()
    {
    }

    public TimelineRecordFeedLinesWrapper(Guid stepId) => this.StepId = stepId;

    public TimelineRecordFeedLinesWrapper(Guid stepId, IList<string> lines)
    {
      this.StepId = stepId;
      this.Value = lines.ToList<string>();
      this.Count = lines.Count;
    }

    public TimelineRecordFeedLinesWrapper(Guid stepId, IList<string> lines, long startLine)
      : this(stepId, lines)
    {
      this.StartLine = new long?(startLine);
      long? startLine1 = this.StartLine;
      long count = (long) this.Count;
      long? nullable = startLine1.HasValue ? new long?(startLine1.GetValueOrDefault() + count) : new long?();
      long num = 1;
      this.EndLine = nullable.HasValue ? new long?(nullable.GetValueOrDefault() - num) : new long?();
    }

    public TimelineRecordFeedLinesWrapper(
      Guid stepId,
      IList<string> lines,
      long startLine,
      long endLine)
      : this(stepId, lines)
    {
      this.StartLine = new long?(startLine);
      this.EndLine = new long?(endLine);
    }

    [DataMember(Order = 0)]
    public int Count { get; private set; }

    [DataMember]
    public long? StartLine { get; private set; }

    [DataMember]
    public long? EndLine { get; private set; }

    [DataMember]
    public List<string> Value { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid StepId { get; set; }
  }
}
