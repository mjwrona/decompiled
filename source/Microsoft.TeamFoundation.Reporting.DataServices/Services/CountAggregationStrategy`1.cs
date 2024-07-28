// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.CountAggregationStrategy`1
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class CountAggregationStrategy<RecordType> : IAggregationStrategy<RecordType>
  {
    private IInterpretRecord<RecordType> m_recordInterpreter;

    public void Initialize(IInterpretRecord<RecordType> recordInterpreter, Measure measureOptions) => this.m_recordInterpreter = recordInterpreter;

    public float GetValue(RecordType record) => 1f;

    public void UpdateAggregateRecord(
      float recordMeasureValue,
      AggregatedRecord aggregateRecord,
      RecordType record)
    {
      aggregateRecord.WorkItemIds.Add(this.m_recordInterpreter.GetWorkIemId(record));
      aggregateRecord.Measure += recordMeasureValue;
    }
  }
}
