// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.StatisticsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class StatisticsHelper : RestApiHelper
  {
    public StatisticsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunStatisticsForRunId(
      string projectId,
      int runId)
    {
      this.RequestContext.TraceInfo("RestLayer", "StatisticsHelper.GetTestRunStatisticsForRunId projectId = {0}, runId = {1}", (object) projectId, (object) runId);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>("StatisticsHelper.GetTestRunStatisticsForRunId", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string name = projectReference.Name;
        this.CheckForViewTestResultPermission(name);
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic testRunStatistic;
        if (!flag1 && !flag2 && this.TestManagementRequestContext.TcmServiceHelper.TryGetTestRunStatistics(this.RequestContext, projectReference.Id, runId, out testRunStatistic))
          return testRunStatistic;
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        TestRun testRun;
        if (!this.TryGetTestRunFromRunId(name, runId, out testRun))
          throw new TestObjectNotFoundException(this.RequestContext, runId, ObjectTypes.TestRun);
        return this.GetTestRunStatisticsForRun(projectReference, testRun);
      }), 1015063, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic GetTestRunStatisticsForRun(
      TeamProjectReference projectReference,
      TestRun run)
    {
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>("StatisticsHelper.GetTestRunStatisticsForRun", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic>) (() =>
      {
        DataContractConverter dataContractConverter = new DataContractConverter(this.TestManagementRequestContext);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic statisticsForRun = new Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic();
        statisticsForRun.Run = this.GetRunRepresentation(projectReference.Name, run);
        statisticsForRun.RunStatistics = new List<RunStatistic>();
        List<TestRunStatistic> source = this.ObjectFactory.QueryTestRunStatistics(this.TestManagementRequestContext, projectReference.Name, run.TestRunId);
        if (source != null && source.Any<TestRunStatistic>())
          statisticsForRun.RunStatistics.AddRange(source.Select<TestRunStatistic, RunStatistic>((Func<TestRunStatistic, RunStatistic>) (testRunStatistic => dataContractConverter.ConvertTestStatisticsToDataContract(testRunStatistic))));
        return statisticsForRun;
      }), 1015063, "TestManagement");
    }
  }
}
