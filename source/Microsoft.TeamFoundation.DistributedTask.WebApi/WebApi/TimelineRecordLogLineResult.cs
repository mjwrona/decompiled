// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordLogLineResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ClientIgnore]
  [DataContract]
  public class TimelineRecordLogLineResult
  {
    public TimelineRecordLogLineResult(IList<TimelineRecordLogLine> lines, string continuationToken)
    {
      this.Lines = lines;
      this.ContinuationToken = continuationToken;
    }

    [DataMember]
    public IList<TimelineRecordLogLine> Lines { get; set; }

    [DataMember]
    public string ContinuationToken { get; set; }
  }
}
