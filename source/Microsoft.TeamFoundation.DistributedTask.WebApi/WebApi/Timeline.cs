// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline
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
  public sealed class Timeline : TimelineReference
  {
    [DataMember(Name = "Records", EmitDefaultValue = false, Order = 4)]
    private List<TimelineRecord> m_records;

    public Timeline()
    {
    }

    public Timeline(Guid timelineId) => this.Id = timelineId;

    private Timeline(Timeline timelineToBeCloned)
    {
      this.ChangeId = timelineToBeCloned.ChangeId;
      this.Id = timelineToBeCloned.Id;
      this.LastChangedBy = timelineToBeCloned.LastChangedBy;
      this.LastChangedOn = timelineToBeCloned.LastChangedOn;
      this.Location = timelineToBeCloned.Location;
      if (timelineToBeCloned.m_records == null)
        return;
      this.m_records = timelineToBeCloned.m_records.Select<TimelineRecord, TimelineRecord>((Func<TimelineRecord, TimelineRecord>) (x => x.Clone())).ToList<TimelineRecord>();
    }

    [DataMember]
    public Guid LastChangedBy { get; internal set; }

    [DataMember]
    public DateTime LastChangedOn { get; internal set; }

    public List<TimelineRecord> Records
    {
      get
      {
        if (this.m_records == null)
          this.m_records = new List<TimelineRecord>();
        return this.m_records;
      }
    }

    public Timeline Clone() => new Timeline(this);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<TimelineRecord> records = this.m_records;
      // ISSUE: explicit non-virtual call
      if ((records != null ? (__nonvirtual (records.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_records = (List<TimelineRecord>) null;
    }
  }
}
