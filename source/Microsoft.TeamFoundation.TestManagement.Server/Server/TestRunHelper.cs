// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestRunHelper
  {
    private static IBuildServiceHelper m_buildServiceHelper;

    internal static TestRun CreateTestRun(
      string runTitle,
      TestPlan testPlan,
      TestCaseResult[] testResults,
      TestOutcome outcome,
      DateTime dateTime,
      Guid runOwner,
      TestManagementRequestContext testManagementRequestContext,
      string projectName,
      TestRunType runType)
    {
      TestRun testRun = new TestRun();
      testRun.Owner = runOwner;
      testRun.Iteration = testPlan.Iteration;
      testRun.BuildUri = testPlan.BuildUri;
      testRun.State = (byte) 2;
      testRun.IsAutomated = false;
      testRun.TestPlanId = testPlan.PlanId;
      testRun.StartDate = dateTime;
      testRun.CompleteDate = dateTime;
      testRun.Title = runTitle;
      TestRun run = testRun;
      TestRunHelper.PopulateBuildDetails(testManagementRequestContext, run, run.BuildUri, projectName, testPlan.BuildDefinitionId);
      run.Type = outcome != TestOutcome.Blocked ? (byte) runType : (byte) 2;
      return run.Create(testManagementRequestContext, (TestSettings) null, testResults, false, projectName);
    }

    internal static void PopulateBuildDetails(
      TestManagementRequestContext testManagementRequestContext,
      TestRun run,
      string buildUri,
      string projectName = null,
      int buildDefinitionId = 0)
    {
      using (PerfManager.Measure(testManagementRequestContext.RequestContext, "BusinessLayer", "TestRunHelper.PopulateBuildDetails"))
      {
        if (!string.IsNullOrEmpty(buildUri))
        {
          BuildDetail buildDetail = TestRunHelper.QueryBuildByUri(testManagementRequestContext.RequestContext, buildUri);
          if (buildDetail == null)
            return;
          run.BuildNumber = buildDetail.BuildNumber;
          run.DropLocation = buildDetail.DropLocation;
        }
        else
        {
          if (string.IsNullOrEmpty(projectName) || buildDefinitionId <= 0)
            return;
          BuildConfiguration buildConfiguration = TestRunHelper.BuildServiceHelper.QueryLastSuccessfulBuild(testManagementRequestContext.RequestContext, Validator.CheckAndGetProjectFromName(testManagementRequestContext, projectName).GuidId, new BuildConfiguration()
          {
            BuildDefinitionId = buildDefinitionId
          }, DateTime.MaxValue);
          if (buildConfiguration == null)
            return;
          run.BuildNumber = buildConfiguration.BuildNumber;
        }
      }
    }

    private static BuildDetail QueryBuildByUri(IVssRequestContext context, string uri)
    {
      context.TraceInfo("BusinessLayer", "TestRunHelper.QueryBuildByUri started - {0}", (object) uri);
      try
      {
        ITeamFoundationBuildService service = context.GetService<ITeamFoundationBuildService>();
        IVssRequestContext requestContext = context;
        List<string> uris = new List<string>();
        uris.Add(uri);
        Guid projectId = new Guid();
        using (TeamFoundationDataReader foundationDataReader = service.QueryBuildsByUri(requestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.None, QueryDeletedOption.ExcludeDeleted, projectId))
        {
          BuildQueryResult buildQueryResult = foundationDataReader.Current<BuildQueryResult>();
          if (buildQueryResult != null)
          {
            if (buildQueryResult.Builds.MoveNext())
            {
              context.TraceInfo("BusinessLayer", "TestRunHelper.QueryBuildByUri ended");
              return buildQueryResult.Builds.Current;
            }
          }
        }
      }
      catch (Exception ex)
      {
        context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (BuildDetail) null;
    }

    internal static IBuildServiceHelper BuildServiceHelper
    {
      get => TestRunHelper.m_buildServiceHelper ?? (IBuildServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
      set => TestRunHelper.m_buildServiceHelper = value;
    }
  }
}
