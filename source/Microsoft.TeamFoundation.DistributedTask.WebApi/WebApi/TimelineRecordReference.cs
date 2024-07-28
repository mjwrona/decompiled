// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TimelineRecordReference : ITimelineRecordReference
  {
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public string Identifier { get; set; }

    [DataMember(Order = 3)]
    public TimelineRecordState? State { get; set; }

    public string ToStringExtended()
    {
      try
      {
        return JsonUtility.ToString((object) new
        {
          RecordId = this.Id,
          State = this.State
        });
      }
      catch (Exception ex)
      {
        return string.Format("{0}() encountered {1}", (object) "ToString", (object) ex.GetType());
      }
    }
  }
}
