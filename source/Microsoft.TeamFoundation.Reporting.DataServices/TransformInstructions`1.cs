// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.TransformInstructions`1
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class TransformInstructions<RecordType>
  {
    public TransformOptions Options;

    public IEnumerable<DateTime> HistorySamplePoints { get; set; }

    public IAggregationStrategy<RecordType> AggregationStrategy { get; set; }

    public ChartDimensionality ChartDimensionality { get; set; }

    public TimeZoneInfo LocalTimeZone { get; set; }
  }
}
