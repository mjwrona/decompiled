// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.Traditional.Binders.ChartConfigurationBinder2
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.Traditional.Binders
{
  internal class ChartConfigurationBinder2 : ObjectBinder<ChartConfiguration>
  {
    internal SqlColumnBinder chartId = new SqlColumnBinder("ChartId");
    internal SqlColumnBinder groupKey = new SqlColumnBinder("GroupKey");
    internal SqlColumnBinder scope = new SqlColumnBinder("Scope");
    internal SqlColumnBinder chartType = new SqlColumnBinder("ChartType");
    internal SqlColumnBinder title = new SqlColumnBinder("Title");
    internal SqlColumnBinder transformId = new SqlColumnBinder("TransformId");
    internal SqlColumnBinder filter = new SqlColumnBinder("Filter");
    internal SqlColumnBinder historyRange = new SqlColumnBinder("HistoryRange");
    internal SqlColumnBinder groupBy = new SqlColumnBinder("GroupBy");
    internal SqlColumnBinder orderPropertyName = new SqlColumnBinder("OrderBy");
    internal SqlColumnBinder orderDirection = new SqlColumnBinder("OrderDirection");
    internal SqlColumnBinder measureAggregation = new SqlColumnBinder("MeasureAggregation");
    internal SqlColumnBinder series = new SqlColumnBinder("Series");
    internal SqlColumnBinder measurePropertyName = new SqlColumnBinder("MeasureProperty");

    protected override ChartConfiguration Bind() => new ChartConfiguration()
    {
      ChartId = new Guid?(this.chartId.GetGuid((IDataReader) this.Reader)),
      GroupKey = this.groupKey.GetString((IDataReader) this.Reader, false),
      Scope = this.scope.GetString((IDataReader) this.Reader, false),
      ChartType = this.chartType.GetString((IDataReader) this.Reader, false),
      Title = this.title.GetString((IDataReader) this.Reader, false),
      TransformOptions = new TransformOptions()
      {
        TransformId = new Guid?(this.transformId.GetGuid((IDataReader) this.Reader)),
        Filter = this.filter.GetString((IDataReader) this.Reader, false),
        HistoryRange = this.historyRange.GetString((IDataReader) this.Reader, false),
        GroupBy = this.groupBy.GetString((IDataReader) this.Reader, false),
        OrderBy = new OrderBy()
        {
          PropertyName = this.orderPropertyName.GetString((IDataReader) this.Reader, false),
          Direction = this.orderDirection.GetString((IDataReader) this.Reader, false)
        },
        Series = this.series.GetString((IDataReader) this.Reader, false),
        Measure = new Measure()
        {
          Aggregation = this.measureAggregation.GetString((IDataReader) this.Reader, false),
          PropertyName = this.measurePropertyName.GetString((IDataReader) this.Reader, false)
        }
      }
    };
  }
}
