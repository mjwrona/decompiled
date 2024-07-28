// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Charting.WebApi.AggregatedRecord
// Assembly: Microsoft.TeamFoundation.Charting.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D43D663A-A882-4856-B0B7-D0E666C5CC4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Charting.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Charting.WebApi
{
  [DataContract]
  public class AggregatedRecord : SecuredChartObject, IMeasureAggregation
  {
    [DataMember]
    public float Measure { get; set; }

    [DataMember]
    public object Group { get; set; }

    [DataMember]
    public object Series { get; set; }

    [DataMember]
    public List<int> WorkItemIds { get; set; }

    public AggregatedRecord() => this.WorkItemIds = new List<int>();

    public AggregatedRecord(object series, object group)
    {
      this.Series = series;
      this.Group = group;
      this.WorkItemIds = new List<int>();
    }

    public AggregatedRecord.AggregationRecordKey CreateKey() => new AggregatedRecord.AggregationRecordKey(this.Series, this.Group);

    public static AggregatedRecord.AggregationRecordKey CreateKey(object series, object group) => new AggregatedRecord.AggregationRecordKey(series, group);

    public bool IsSemanticEqual(AggregatedRecord target) => (double) target.Measure == (double) this.Measure && target.Group == this.Group && target.Series == this.Series;

    public struct AggregationRecordKey
    {
      private string Series;
      private string Group;

      internal AggregationRecordKey(object series, object group)
        : this()
      {
        if (series != null)
          this.Series = series.ToString().ToLower();
        if (group == null)
          return;
        this.Group = group.ToString().ToLower();
      }
    }
  }
}
