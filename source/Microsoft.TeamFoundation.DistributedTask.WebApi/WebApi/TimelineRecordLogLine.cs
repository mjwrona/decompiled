// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordLogLine
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ClientIgnore]
  [DataContract]
  public class TimelineRecordLogLine
  {
    public TimelineRecordLogLine(string line, long lineNumber)
    {
      this.Line = line;
      this.LineNumber = lineNumber;
    }

    [DataMember]
    public string Line { get; set; }

    [DataMember]
    public long LineNumber { get; set; }

    public override bool Equals(object obj) => obj is TimelineRecordLogLine timelineRecordLogLine && string.Equals(this.Line, timelineRecordLogLine.Line) && this.LineNumber.Equals(timelineRecordLogLine.LineNumber);

    public override int GetHashCode()
    {
      int num = 13 * 7;
      string line = this.Line;
      int hashCode = line != null ? line.GetHashCode() : 0;
      return (num + hashCode) * 7 + this.LineNumber.GetHashCode();
    }
  }
}
