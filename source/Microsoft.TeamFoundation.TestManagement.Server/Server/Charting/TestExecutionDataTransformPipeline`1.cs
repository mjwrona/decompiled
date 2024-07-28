// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionDataTransformPipeline`1
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestExecutionDataTransformPipeline<RecordType> : 
    DataTransformPipelineBase<RecordType>
  {
    public override IEnumerable<TransformResult> GetResults(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformOptions> options)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<TransformOptions>>(options, nameof (options), requestContext.ServiceName);
      if (options.Count<TransformOptions>() < 1)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) nameof (options)));
      List<ITabulator<RecordType>> tabulatorList = new List<ITabulator<RecordType>>();
      try
      {
        foreach (TransformOptions option in options)
        {
          List<TransformInstructions<RecordType>> options1 = new List<TransformInstructions<RecordType>>();
          TransformInstructions<RecordType> instructions = new TransformInstructions<RecordType>()
          {
            Options = option,
            ChartDimensionality = ChartDimensionality.FromTransformOptions(option, true),
            AggregationStrategy = AggregationMediator.GetStrategy<RecordType>(requestContext, (IInterpretRecord<RecordType>) this.DataInterpreter, option.Measure)
          };
          options1.Add(instructions);
          ITabulator<RecordType> tabulator = this.InstantiateTabulator(requestContext, instructions);
          IEnumerable<RecordType> filteredData = this.DataProvider.GetFilteredData(requestContext, transformSecurityInformation, (IEnumerable<TransformInstructions<RecordType>>) options1);
          TestReportsHelper.ProcessResultsForCharts<RecordType>(requestContext, tabulator, filteredData);
          tabulatorList.Add(tabulator);
        }
        return tabulatorList.Select<ITabulator<RecordType>, TransformResult>((Func<ITabulator<RecordType>, TransformResult>) (o => o.PackResultsForLocalZoneTime(TimeZoneInfo.Local)));
      }
      finally
      {
        this.RecordTabulationCost(FeatureProviderScopes.TestReports, tabulatorList);
      }
    }
  }
}
