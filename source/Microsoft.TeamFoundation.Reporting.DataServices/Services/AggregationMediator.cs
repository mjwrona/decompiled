// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.AggregationMediator
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class AggregationMediator
  {
    public static IAggregationStrategy<T> GetStrategy<T>(
      IVssRequestContext requestContext,
      IInterpretRecord<T> recordInterpreter,
      Measure measureOptions)
    {
      IAggregationStrategy<T> strategy = (IAggregationStrategy<T>) null;
      if (measureOptions.Aggregation == "count")
        strategy = (IAggregationStrategy<T>) new CountAggregationStrategy<T>();
      else if (measureOptions.Aggregation == "sum")
        strategy = (IAggregationStrategy<T>) new SumAggregationStrategy<T>();
      if (strategy == null)
        throw new InvalidChartConfigurationException(ReportingResources.InvalidMeasureAggregation((object) measureOptions.Aggregation));
      strategy.Initialize(recordInterpreter, measureOptions);
      return strategy;
    }

    public static IEnumerable<NameLabelPair> GetNumericalAggregationFunctions(
      IVssRequestContext requestContext)
    {
      return (IEnumerable<NameLabelPair>) new List<NameLabelPair>()
      {
        new NameLabelPair()
        {
          Name = "sum",
          LabelText = ReportingResources.AggregationFunctionLabel_Sum()
        }
      };
    }

    public static bool IsSupportedMeasure(IVssRequestContext requestContext, Measure measure)
    {
      if (!AggregationMediator.CheckValidAggregationFunction(requestContext, measure.Aggregation))
        return false;
      return !measure.IsPropertyNeeded() || !string.IsNullOrEmpty(measure.PropertyName);
    }

    public static void CheckMeasureParameters(IVssRequestContext requestContext, Measure measure)
    {
      ArgumentUtility.CheckForNull<Measure>(measure, "Measure");
      if (!AggregationMediator.CheckValidAggregationFunction(requestContext, measure.Aggregation))
        throw new InvalidChartConfigurationException(ReportingResources.InvalidMeasureAggregation((object) measure.Aggregation));
      if (!measure.IsPropertyNeeded())
        return;
      ArgumentUtility.CheckStringForNullOrEmpty(measure.PropertyName, "Measure.PropertyName");
    }

    private static bool CheckValidAggregationFunction(
      IVssRequestContext requestContext,
      string aggregationFunction)
    {
      return aggregationFunction == "sum" || aggregationFunction == "count";
    }
  }
}
