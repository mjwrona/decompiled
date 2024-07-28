// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementObjectHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementObjectHelper : ITestManagementObjectHelper
  {
    public virtual TeamProjectReference GetProjectReference(
      string projectIdentifier,
      IVssRequestContext requestContext)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName("GetProjectReferenece", "Project")))
        return this.ExecuteAction<TeamProjectReference>(requestContext, "TestManagementObjectHelper.GetProjectReference", (Func<TeamProjectReference>) (() =>
        {
          IProjectService service = requestContext.GetService<IProjectService>();
          Guid result;
          if (Guid.TryParse(projectIdentifier, out result))
          {
            ProjectInfo project = service.GetProject(requestContext, result);
            return project == null ? (TeamProjectReference) null : this.ToTeamProjectReference(requestContext, project);
          }
          ProjectInfo project1 = service.GetProject(requestContext, projectIdentifier);
          return project1 == null ? (TeamProjectReference) null : this.ToTeamProjectReference(requestContext, project1);
        }), 1015080, "TestManagement", "BusinessLayer");
    }

    public virtual TestRun CreateTestRun(
      TestRun run,
      TestManagementRequestContext requestContext,
      TestSettings settings,
      TestCaseResult[] results,
      bool populateDataRowCount,
      string teamProjectName)
    {
      return this.ExecuteAction<TestRun>(requestContext.RequestContext, "TestManagementObjectHelper.CreateTestRun", (Func<TestRun>) (() => run.Create(requestContext, settings, results, populateDataRowCount, teamProjectName, true)), 1015080, "TestManagement", "BusinessLayer");
    }

    public virtual Microsoft.TeamFoundation.Build.WebApi.Build GetBuildDetailFromUri(
      string buildUri,
      IVssRequestContext requestContext,
      Guid projectId,
      IBuildServiceHelper buildServiceHelper)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (GetBuildDetailFromUri), "Build")))
        return this.ExecuteAction<Microsoft.TeamFoundation.Build.WebApi.Build>(requestContext, "TestManagementObjectHelper.GetBuildDetailFromUri", (Func<Microsoft.TeamFoundation.Build.WebApi.Build>) (() => string.IsNullOrEmpty(buildUri) ? (Microsoft.TeamFoundation.Build.WebApi.Build) null : buildServiceHelper.QueryBuildByUri(requestContext, projectId, buildUri, true)), 1015080, "TestManagement", "BusinessLayer");
    }

    public virtual void CheckForViewTestResultPermission(
      string projectName,
      TestManagementRequestContext requestContext)
    {
      string projectUriFromName = Validator.CheckAndGetProjectUriFromName(requestContext, projectName);
      if (!requestContext.SecurityManager.HasViewTestResultsPermission(requestContext, projectUriFromName))
        throw new Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
    }

    public virtual TestSettings GetTestSettingdById(
      TestManagementRequestContext requestContext,
      int testSettingId,
      string projectName)
    {
      return this.ExecuteAction<TestSettings>(requestContext.RequestContext, "TestManagementObjectHelper.GetTestSettingdById", (Func<TestSettings>) (() => TestSettings.QueryById(requestContext, testSettingId, projectName)), 1015080, "TestManagement", "BusinessLayer");
    }

    public virtual List<TestRunStatistic> QueryTestRunStatistics(
      TestManagementRequestContext context,
      string projectName,
      int testRunId)
    {
      return this.ExecuteAction<List<TestRunStatistic>>(context.RequestContext, "TestManagementObjectHelper.QueryTestRunStatistics", (Func<List<TestRunStatistic>>) (() => TestRunStatistic.Query(context, projectName, testRunId)), 1015080, "TestManagement", "BusinessLayer");
    }

    protected T ExecuteAction<T>(
      IVssRequestContext requestContext,
      string methodName,
      Func<T> action,
      int tracePoint,
      string traceArea,
      string traceLayer = "RestLayer")
    {
      try
      {
        requestContext.TraceEnter(tracePoint, traceArea, traceLayer, methodName);
        return action();
      }
      finally
      {
        requestContext.TraceLeave(tracePoint, traceArea, traceLayer, methodName);
      }
    }

    protected TeamProjectReference ToTeamProjectReference(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      return new TeamProjectReference()
      {
        Id = projectInfo.Id,
        Abbreviation = projectInfo.Abbreviation,
        Name = projectInfo.Name,
        State = projectInfo.State,
        Description = projectInfo.Description,
        Revision = projectInfo.Revision,
        Visibility = projectInfo.Visibility
      };
    }
  }
}
