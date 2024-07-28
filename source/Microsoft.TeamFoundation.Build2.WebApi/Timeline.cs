// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Timeline
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public sealed class Timeline : TimelineReference
  {
    private List<TimelineRecord> m_records;
    [DataMember(Name = "Records", EmitDefaultValue = false)]
    private List<TimelineRecord> m_serializedRecords;

    internal Timeline()
    {
    }

    internal Timeline(ISecuredObject securedObject)
      : base(securedObject)
    {
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

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (this.m_serializedRecords == null || this.m_serializedRecords.Count <= 0)
        return;
      this.m_records = new List<TimelineRecord>((IEnumerable<TimelineRecord>) this.m_serializedRecords);
      this.m_serializedRecords = (List<TimelineRecord>) null;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      if (this.m_records == null || this.m_records.Count <= 0)
        return;
      this.m_serializedRecords = new List<TimelineRecord>((IEnumerable<TimelineRecord>) this.m_records);
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedRecords = (List<TimelineRecord>) null;
  }
}
