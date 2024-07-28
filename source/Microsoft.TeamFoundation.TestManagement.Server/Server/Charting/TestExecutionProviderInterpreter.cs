// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionProviderInterpreter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestExecutionProviderInterpreter : 
    IInterpretTimedData<DatedTestFieldData>,
    IInterpretRecord<DatedTestFieldData>,
    IProvideFilteredData<DatedTestFieldData>
  {
    private readonly Guid pointsResultsMigrationJobId = new Guid("758F2D56-211D-4D36-8F93-E9A01B3EE462");

    public IEnumerable<DatedTestFieldData> GetFilteredData(
      IVssRequestContext requestContext,
      TransformSecurityInformation transformSecurityInformation,
      IEnumerable<TransformInstructions<DatedTestFieldData>> instructions)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<TransformInstructions<DatedTestFieldData>>>(instructions, nameof (instructions));
      if (instructions.Count<TransformInstructions<DatedTestFieldData>>() != 1)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) nameof (instructions)));
      TestExecutionReportData executionReportData = new TestExecutionReportData();
      List<KeyValuePair<int, string>> dimensionList = new List<KeyValuePair<int, string>>();
      int result1 = 0;
      int result2 = 0;
      Guid projectGuid = Guid.Empty;
      TransformOptions options = instructions.FirstOrDefault<TransformInstructions<DatedTestFieldData>>().Options;
      Dictionary<string, string> dictionary1 = !string.IsNullOrEmpty(options.Filter) ? TestReportsHelper.GetTransformFilterMap(options.Filter) : throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "Filter"));
      string s1;
      if (!dictionary1.TryGetValue("suiteId", out s1) || string.IsNullOrEmpty(s1) || !int.TryParse(s1, out result1))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "Filter[suiteId]"));
      string s2;
      if (!dictionary1.TryGetValue("planId", out s2) || string.IsNullOrEmpty(s2) || !int.TryParse(s2, out result2))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "Filter[planId]"));
      if (options.FilterContext == null || options.FilterContext.Count < 1 || !options.FilterContext.Contains((object) "projectId"))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "FilterContext"));
      string input = options.FilterContext[(object) "projectId"].ToString();
      if (string.IsNullOrEmpty(input) || !Guid.TryParse(input, out projectGuid) || !projectGuid.Equals((object) transformSecurityInformation.ProjectId))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "FilterContext[projectId]"));
      string nameFromProjectGuid = Microsoft.TeamFoundation.TestManagement.Server.Validator.GetProjectNameFromProjectGuid(requestContext, projectGuid);
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      List<Microsoft.TeamFoundation.TestManagement.Server.TestPlan> source1 = TestPlanWorkItem.FetchPlans((TestManagementRequestContext) managementRequestContext, nameFromProjectGuid, ((IEnumerable<int>) new int[1]
      {
        result2
      }).ToList<int>());
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan = source1 != null && source1.Count == 1 ? source1.FirstOrDefault<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>() : throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "TestPlan"));
      if (testPlan == null || testPlan.Name == null)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidChartTransformInstruction, (object) "TestPlan"));
      if (string.IsNullOrEmpty(options.GroupBy))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidDimensionInChartTransform));
      dimensionList.Add(new KeyValuePair<int, string>(0, options.GroupBy));
      if (!string.IsNullOrEmpty(options.Series))
        dimensionList.Add(new KeyValuePair<int, string>(1, options.Series));
      ChartingHelper.ValidateDimensions(dimensionList.Select<KeyValuePair<int, string>, string>((Func<KeyValuePair<int, string>, string>) (m => m.Value)).ToList<string>(), "execution");
      if (!managementRequestContext.PlannedTestingTCMServiceHelper.IsTCMEnabledForPlannedTestResults((TestManagementRequestContext) managementRequestContext, result2))
      {
        List<TestFieldData> source2 = new List<TestFieldData>();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(requestContext))
          source2 = planningDatabase.GetTestExecutionReport(testPlan.Name, result1, dimensionList);
        if (source2 != null && source2.Any<TestFieldData>())
        {
          foreach (TestFieldData testFieldData in source2)
          {
            Dictionary<string, object> dimensions = testFieldData.Dimensions;
            foreach (string str in dimensions.Keys.ToList<string>())
            {
              object dimensionValue = dimensions[str];
              object obj = (object) TestExecutionReportData.MapDimensionValue(requestContext, str, dimensionValue);
              dimensions[str] = obj;
            }
            executionReportData.AddReportDatarow(dimensions, testFieldData.Measure);
          }
        }
      }
      else
      {
        if (requestContext.IsFeatureEnabled("TestManagement.Server.EnableChartingMigration"))
          this.TryQueueJobForDataMigration(requestContext);
        List<TestAuthoringDetails> authoringDetailsList = new List<TestAuthoringDetails>();
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> webApiModels = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails>();
        Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
        Dictionary<int, string> dictionary3 = new Dictionary<int, string>();
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(requestContext))
          authoringDetailsList = planningDatabase.GetTestAuthoringChartDetails(testPlan.Name, result1);
        foreach (TestAuthoringDetails authoringDetails in authoringDetailsList)
        {
          dictionary2[authoringDetails.ConfigurationId] = authoringDetails.ConfigurationName;
          dictionary3[authoringDetails.SuiteId] = authoringDetails.SuiteName;
          webApiModels.Add(authoringDetails.ToWebApiModel());
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData executionSummaryReport = (Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData) null;
        bool flag = true;
        if (!managementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        {
          flag = managementRequestContext.TcmServiceHelper.TryGetTestExecutionSummaryReport(requestContext, projectGuid, testPlan.PlanId, webApiModels, dimensionList.Select<KeyValuePair<int, string>, string>((Func<KeyValuePair<int, string>, string>) (d => d.Value)).ToList<string>(), out executionSummaryReport);
        }
        else
        {
          TcmHttpClient tcmHttpClient = managementRequestContext.RequestContext.GetClient<TcmHttpClient>(requestContext.GetHttpClientOptionsForEventualReadConsistencyLevel("TestManagement.Server.GetTestExecutionSummaryReport.EnableSqlReadReplica"));
          executionSummaryReport = TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData>) (() => tcmHttpClient.GetTestExecutionSummaryReportAsync((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails>) webApiModels, projectGuid, testPlan.PlanId, (IEnumerable<string>) dimensionList.Select<KeyValuePair<int, string>, string>((Func<KeyValuePair<int, string>, string>) (d => d.Value)).ToList<string>())?.Result));
        }
        if (flag)
        {
          foreach (Microsoft.TeamFoundation.TestManagement.WebApi.DatedTestFieldData datedTestFieldData in executionSummaryReport.ReportData)
          {
            Dictionary<string, object> dimensions = datedTestFieldData.Value.Dimensions;
            foreach (string str in dimensions.Keys.ToList<string>())
            {
              object dimensionValue = dimensions[str];
              if (dimensionValue != null && string.Equals(str, "Configuration", StringComparison.OrdinalIgnoreCase))
              {
                int result3;
                if (int.TryParse(dimensionValue.ToString(), out result3))
                  dimensionValue = (object) dictionary2[result3];
              }
              else
              {
                int result4;
                if (dimensionValue != null && string.Equals(str, "Suite", StringComparison.OrdinalIgnoreCase) && int.TryParse(dimensionValue.ToString(), out result4))
                  dimensionValue = (object) dictionary3[result4];
              }
              object obj = (object) TestExecutionReportData.MapDimensionValue(requestContext, str, dimensionValue);
              dimensions[str] = obj;
            }
            executionReportData.AddReportDatarow(dimensions, datedTestFieldData.Value.Measure);
          }
        }
      }
      return executionReportData.GetReportTable();
    }

    public bool SamplePoint(
      DatedTestFieldData record,
      TransformInstructions<DatedTestFieldData> instructions,
      out AggregatedRecord sampledRecord)
    {
      ArgumentUtility.CheckForNull<TransformInstructions<DatedTestFieldData>>(instructions, nameof (instructions));
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
      else if (instructions.Options.Series.Equals(instructions.Options.GroupBy) && record.Value.Dimensions.Count == 1)
      {
        sampledRecord = new AggregatedRecord()
        {
          Measure = (float) record.Value.Measure,
          Group = record.Value.Dimensions.ElementAt<KeyValuePair<string, object>>(0).Value,
          Series = record.Value.Dimensions.ElementAt<KeyValuePair<string, object>>(0).Value
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

    private void TryQueueJobForDataMigration(IVssRequestContext requestContext)
    {
      ISqlRegistryService service = requestContext.GetService<ISqlRegistryService>();
      switch ((TCMServiceDataMigrationStatus) service.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/PointResultsMigrationStatus", 0))
      {
        case TCMServiceDataMigrationStatus.NotStarted:
          if (this.ShouldQueueJob(requestContext))
          {
            requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              this.pointsResultsMigrationJobId
            });
            break;
          }
          break;
        case TCMServiceDataMigrationStatus.Completed:
          return;
      }
      if (service.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/PointResultsMigrationStatus", 0) != 2)
        throw new TCMChartingInProgressException(Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TCMChartingInProgressError);
    }

    private bool ShouldQueueJob(IVssRequestContext requestContext)
    {
      List<TeamFoundationJobQueueEntry> source = requestContext.GetService<ITeamFoundationJobService>().QueryRunningJobs(requestContext, true);
      return source == null || !source.Where<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (job => job.JobSource == requestContext.ServiceHost.InstanceId && job.JobId == this.pointsResultsMigrationJobId)).Any<TeamFoundationJobQueueEntry>();
    }

    public int GetWorkIemId(DatedTestFieldData record) => 0;
  }
}
