// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Charting.WitUnifiedTransformPipeline`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Charting
{
  internal class WitUnifiedTransformPipeline<RecordType> : DataTransformPipelineBase<RecordType>
  {
    public override IEnumerable<TransformResult> GetResults(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformOptions> options)
    {
      this.RequestContext.TraceEnter(901610, "WITCharting", "WitQueryProvider", nameof (GetResults));
      List<ITabulator<RecordType>> tabulatorList = (List<ITabulator<RecordType>>) null;
      try
      {
        WitTrendPolicyHelper.EnforceTrendSettings(options, false);
        IEnumerable<FixedIntervalDateRange> ranges = WitDataServiceCapabilityProvider.GetRangeOptions();
        TimeZoneInfo accountTimeZone = WitTrendPolicyHelper.GetCollectionTimeZoneInfo(requestContext);
        DateTime anchorTime = WitTrendPolicyHelper.SampleTime(this.RequestContext).FromUtc(accountTimeZone);
        IEnumerable<TransformInstructions<RecordType>> options1 = options.Select<TransformOptions, TransformInstructions<RecordType>>((Func<TransformOptions, TransformInstructions<RecordType>>) (option => new TransformInstructions<RecordType>()
        {
          Options = option,
          ChartDimensionality = ChartDimensionality.FromTransformOptions(option, true),
          HistorySamplePoints = WitTrendPolicyHelper.DatesToUtc(WitTrendPolicyHelper.InstantiateRange(ranges, option.HistoryRange, anchorTime), accountTimeZone),
          AggregationStrategy = AggregationMediator.GetStrategy<RecordType>(requestContext, (IInterpretRecord<RecordType>) this.DataInterpreter, option.Measure),
          LocalTimeZone = accountTimeZone
        }));
        IEnumerable<RecordType> filteredData = this.DataProvider.GetFilteredData(requestContext, transformSecurityInformation, options1);
        tabulatorList = new List<ITabulator<RecordType>>();
        foreach (TransformInstructions<RecordType> instructions in options1)
          tabulatorList.Add(this.InstantiateTabulator(requestContext, instructions));
        this.ProcessResults(requestContext, (IEnumerable<ITabulator<RecordType>>) tabulatorList, filteredData);
        return tabulatorList.Select<ITabulator<RecordType>, TransformResult>((Func<ITabulator<RecordType>, TransformResult>) (o => o.PackResultsForLocalZoneTime(accountTimeZone, requestContext.CancellationToken)));
      }
      finally
      {
        this.RecordTabulationCost(FeatureProviderScopes.WorkItemQueries, tabulatorList);
        this.RequestContext.TraceLeave(901611, "WITCharting", "WitQueryProvider", nameof (GetResults));
      }
    }
  }
}
