// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.JsonDataFormatter
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using System;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class JsonDataFormatter
  {
    private const string m_nullSeriesOrGroup = "[NULL]";

    private string FormatDate(DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");

    public string FormatLabel(object rawData) => !(rawData is DateTime dateTime) ? (rawData ?? (object) "[NULL]").ToString() : this.FormatDate(dateTime);

    public float FormatValue(float value) => Math.Max(value, 0.0f);

    public FormattedRecord FormatRecord(AggregatedRecord o) => new FormattedRecord()
    {
      Group = this.FormatLabel(o.Group),
      Series = this.FormatLabel(o.Series),
      Measure = this.FormatValue(o.Measure)
    };
  }
}
