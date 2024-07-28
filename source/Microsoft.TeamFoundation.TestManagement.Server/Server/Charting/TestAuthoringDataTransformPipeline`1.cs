// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestAuthoringDataTransformPipeline`1
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Charting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestAuthoringDataTransformPipeline<RecordType> : 
    DataTransformPipelineBase<RecordType>
  {
    public override IEnumerable<TransformResult> GetResults(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformOptions> options)
    {
      List<ITabulator<RecordType>> tabulatorList = (List<ITabulator<RecordType>>) null;
      try
      {
        IEnumerable<FixedIntervalDateRange> ranges = WitDataServiceCapabilityProvider.GetRangeOptions();
        TimeZoneInfo accountTimeZone = TrendDateTimeHelper.GetCollectionTimeZoneInfo(requestContext);
        DateTime anchorTime = TrendDateTimeHelper.SampleTime(this.RequestContext).FromUtc(accountTimeZone);
        IEnumerable<TransformInstructions<RecordType>> options1 = options.Select<TransformOptions, TransformInstructions<RecordType>>((Func<TransformOptions, TransformInstructions<RecordType>>) (option => new TransformInstructions<RecordType>()
        {
          Options = option,
          ChartDimensionality = ChartDimensionality.FromTransformOptions(option, true),
          HistorySamplePoints = TrendDateTimeHelper.DatesToUtc(TrendDateTimeHelper.InstantiateRange(ranges, option.HistoryRange, anchorTime), accountTimeZone),
          AggregationStrategy = AggregationMediator.GetStrategy<RecordType>(requestContext, (IInterpretRecord<RecordType>) this.DataInterpreter, option.Measure)
        }));
        IEnumerable<RecordType> filteredData = this.DataProvider.GetFilteredData(requestContext, transformSecurityInformation, options1);
        tabulatorList = new List<ITabulator<RecordType>>();
        foreach (TransformInstructions<RecordType> instructions in options1)
          tabulatorList.Add(this.InstantiateTabulator(requestContext, instructions));
        this.ProcessResults(requestContext, (IEnumerable<ITabulator<RecordType>>) tabulatorList, filteredData);
        return tabulatorList.Select<ITabulator<RecordType>, TransformResult>((Func<ITabulator<RecordType>, TransformResult>) (o => o.PackResultsForLocalZoneTime(TimeZoneInfo.Local)));
      }
      finally
      {
        this.RecordTabulationCost(FeatureProviderScopes.TestAuthoringMetadata, tabulatorList);
      }
    }
  }
}
