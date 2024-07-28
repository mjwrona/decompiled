// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestRunProviderInterpreter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestRunProviderInterpreter : 
    IInterpretTimedData<DatedTestFieldData>,
    IInterpretRecord<DatedTestFieldData>,
    IProvideFilteredData<DatedTestFieldData>
  {
    public IEnumerable<DatedTestFieldData> GetFilteredData(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformInstructions<DatedTestFieldData>> instructions)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<TransformInstructions<DatedTestFieldData>>>(instructions, nameof (instructions), "Test Results");
      TfsTestManagementRequestContext context = new TfsTestManagementRequestContext(requestContext);
      try
      {
        context.TraceEnter("BusinessLayer", "TestRunProviderInterpreter.GetFilteredData()");
        if (instructions.Count<TransformInstructions<DatedTestFieldData>>() != 1)
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) nameof (instructions)));
        TestExecutionReportData executionReportData = (TestExecutionReportData) null;
        List<string> dimensionList = new List<string>();
        int runId = 0;
        Guid projectGuid = Guid.Empty;
        TransformOptions options = instructions.FirstOrDefault<TransformInstructions<DatedTestFieldData>>().Options;
        context.TraceVerbose("BusinessLayer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Filter: {0}", (object) options.Filter));
        if (string.IsNullOrEmpty(options.Filter))
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "Filter"));
        string input;
        if (!TestReportsHelper.GetTransformFilterMap(options.Filter).TryGetValue("projectId", out input) || string.IsNullOrEmpty(input) || !Guid.TryParse(input, out projectGuid) || !projectGuid.Equals((object) transformSecurityInformation.ProjectId))
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "Filter[projectId]"));
        if (options.FilterContext == null || options.FilterContext.Count < 1 || !options.FilterContext.Contains((object) "runId"))
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "FilterContext"));
        context.TraceVerbose("BusinessLayer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FilterContext: {0}", (object) options.FilterContext.ToString()));
        runId = Convert.ToInt32(options.FilterContext[(object) "runId"].ToString());
        if (string.IsNullOrEmpty(options.GroupBy))
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidDimensionInChartTransform));
        dimensionList.Add(options.GroupBy);
        if (!string.IsNullOrEmpty(options.Series))
          dimensionList.Add(options.Series);
        context.TraceVerbose("BusinessLayer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Dimensions: {0}", (object) string.Join(",", (IEnumerable<string>) dimensionList)));
        ChartingHelper.ValidateDimensions(dimensionList, "runsummary");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData runSummaryReport = (Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData) null;
        bool flag = true;
        if (!requestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        {
          flag = context.TcmServiceHelper.TryGetTestRunSummaryReport(requestContext, projectGuid, runId, dimensionList, out runSummaryReport);
          if (!flag)
          {
            using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(requestContext))
              executionReportData = planningDatabase.GetTestRunSummaryReport(projectGuid, runId, dimensionList, runId == 0);
          }
        }
        else
        {
          TcmHttpClient tcmHttpClient = requestContext.GetClient<TcmHttpClient>();
          runSummaryReport = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData>) (() => tcmHttpClient.GetTestRunSummaryReportAsync(projectGuid, runId, (IEnumerable<string>) dimensionList)?.Result));
        }
        if (flag)
        {
          executionReportData = TestExecutionReportData.FromWebApiModel(runSummaryReport);
          if (string.Equals("Configuration", options.GroupBy, StringComparison.OrdinalIgnoreCase))
          {
            List<int> intList = new List<int>();
            foreach (DatedTestFieldData datedTestFieldData in executionReportData.GetReportTable())
            {
              int result;
              if (int.TryParse(datedTestFieldData.Value.Dimensions["Configuration"] as string, out result))
                intList.Add(result);
            }
            if (intList.Any<int>())
            {
              Dictionary<int, string> dictionary = new Dictionary<int, string>();
              using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
                dictionary = managementDatabase.QueryShallowTestConfigurations(projectGuid, intList);
              foreach (DatedTestFieldData datedTestFieldData in executionReportData.GetReportTable())
              {
                int result;
                if (int.TryParse(datedTestFieldData.Value.Dimensions["Configuration"] as string, out result) && dictionary.ContainsKey(result))
                  datedTestFieldData.Value.Dimensions["Configuration"] = (object) dictionary[result];
              }
            }
          }
        }
        return executionReportData?.GetReportTable();
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestRunProviderInterpreter.GetFilteredData()");
      }
    }

    public bool SamplePoint(
      DatedTestFieldData record,
      TransformInstructions<DatedTestFieldData> instructions,
      out AggregatedRecord sampledRecord)
    {
      ArgumentUtility.CheckForNull<TransformInstructions<DatedTestFieldData>>(instructions, nameof (instructions), "Test Results");
      if (string.IsNullOrEmpty(instructions.Options.Series))
      {
        if (record.Value.Dimensions.Count != 1)
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidDimensionInChartTransform));
        sampledRecord = new AggregatedRecord()
        {
          Measure = (float) record.Value.Measure,
          Group = record.Value.Dimensions.ElementAt<KeyValuePair<string, object>>(0).Value
        };
      }
      else
      {
        if (record.Value.Dimensions.Count != 2)
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidDimensionInChartTransform));
        sampledRecord = new AggregatedRecord()
        {
          Measure = (float) record.Value.Measure,
          Group = record.Value.Dimensions.ElementAt<KeyValuePair<string, object>>(0).Value,
          Series = record.Value.Dimensions.ElementAt<KeyValuePair<string, object>>(1).Value
        };
      }
      return true;
    }

    public object GetRecordValue(string recordPropertyName, DatedTestFieldData record) => (object) record.Value.Measure;

    public int GetWorkIemId(DatedTestFieldData record) => 0;
  }
}
