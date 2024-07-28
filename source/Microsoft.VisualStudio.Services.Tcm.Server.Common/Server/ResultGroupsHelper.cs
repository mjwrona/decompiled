// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultGroupsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ResultGroupsHelper : RestApiHelper
  {
    internal ResultGroupsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public TestResultsGroupsForRelease GetTestResultsGroupsByRelease(
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvironmentId,
      string publishContext,
      string fields)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultGroupsHelper.GetTestResultsGroupsByRelease", 50, true))
        return this.ExecuteAction<TestResultsGroupsForRelease>("ResultGroupsHelper.GetTestResultsGroupsByRelease", (Func<TestResultsGroupsForRelease>) (() =>
        {
          TestResultsGroupsForRelease resultsGroupsForRelease = (TestResultsGroupsForRelease) null;
          ITeamFoundationTestManagementResultService testManagementResultService = this.RequestContext.GetService<ITeamFoundationTestManagementResultService>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          if (!flag1 && !flag2)
          {
            TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => resultsGroupsForRelease = testManagementResultService.GetTestResultGroupsByRelease(this.TestManagementRequestContext, projectInfo, releaseId, releaseEnvironmentId, fields, publishContext)), this.RequestContext);
            TestResultsGroupsForRelease resultsGroupsForRelease1;
            if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultGroupsByRelease(this.RequestContext, projectInfo, releaseId, releaseEnvironmentId, fields, publishContext, out resultsGroupsForRelease1))
              resultsGroupsForRelease = this.TestManagementRequestContext.MergeDataHelper.MergeTestResultsGroupsForRelease(resultsGroupsForRelease, resultsGroupsForRelease1);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            resultsGroupsForRelease = testManagementResultService.GetTestResultGroupsByRelease(this.TestManagementRequestContext, projectInfo, releaseId, releaseEnvironmentId, fields, publishContext);
          }
          TestResultsGroupsForRelease groupsForRelease = resultsGroupsForRelease;
          if (groupsForRelease != null)
            groupsForRelease.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
            {
              Project = new ShallowReference()
              {
                Id = projectInfo.Id.ToString()
              }
            });
          return resultsGroupsForRelease;
        }), 1015076, "TestManagement");
    }

    public TestResultsGroupsForBuild GetTestResultsGroupsByBuild(
      ProjectInfo projectInfo,
      int buildId,
      string publishContext,
      string fields)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultGroupsHelper.GetTestResultsGroupsByBuild", 50, true))
        return this.ExecuteAction<TestResultsGroupsForBuild>("ResultGroupsHelper.GetTestResultsGroupsByBuild", (Func<TestResultsGroupsForBuild>) (() =>
        {
          TestResultsGroupsForBuild resultsGroupsForBuild = (TestResultsGroupsForBuild) null;
          ITeamFoundationTestManagementResultService testManagementResultService = this.RequestContext.GetService<ITeamFoundationTestManagementResultService>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          if (!flag1 && !flag2)
          {
            TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => resultsGroupsForBuild = testManagementResultService.GetTestResultGroupsByBuild(this.TestManagementRequestContext, projectInfo, buildId, fields, publishContext)), this.RequestContext);
            TestResultsGroupsForBuild resultsGroupsForBuild1;
            if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultGroupsByBuild(this.RequestContext, projectInfo, buildId, fields, publishContext, out resultsGroupsForBuild1))
              resultsGroupsForBuild = this.TestManagementRequestContext.MergeDataHelper.MergeTestResultsGroupsForBuild(resultsGroupsForBuild, resultsGroupsForBuild1);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            resultsGroupsForBuild = testManagementResultService.GetTestResultGroupsByBuild(this.TestManagementRequestContext, projectInfo, buildId, fields, publishContext);
          }
          TestResultsGroupsForBuild resultsGroupsForBuild2 = resultsGroupsForBuild;
          if (resultsGroupsForBuild2 != null)
            resultsGroupsForBuild2.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
            {
              Project = new ShallowReference()
              {
                Id = projectInfo.Id.ToString()
              }
            });
          return resultsGroupsForBuild;
        }), 1015075, "TestManagement");
    }

    public virtual IPagedList<FieldDetailsForTestResults> GetTestResultsGroupsByRelease(
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvironmentId,
      string publishContext,
      string fields,
      string continuationToken)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultGroupsHelper.GetTestResultsGroupsByRelease", 50, true))
        return this.ExecuteAction<IPagedList<FieldDetailsForTestResults>>("ResultGroupsHelper.GetTestResultsGroupsByRelease", (Func<IPagedList<FieldDetailsForTestResults>>) (() =>
        {
          IPagedList<FieldDetailsForTestResults> resultsGroupsForRelease = (IPagedList<FieldDetailsForTestResults>) new PagedList<FieldDetailsForTestResults>((IEnumerable<FieldDetailsForTestResults>) new List<FieldDetailsForTestResults>(), (string) null);
          int continuationTokenRunId;
          int continuationTokenResultId;
          ParsingHelper.ParseContinuationTokenResultId(continuationToken, out continuationTokenRunId, out continuationTokenResultId);
          ITeamFoundationTestManagementResultService testManagementResultService = this.RequestContext.GetService<ITeamFoundationTestManagementResultService>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          if (!flag1 && !flag2)
          {
            TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => resultsGroupsForRelease = testManagementResultService.GetTestResultGroupsByRelease(this.TestManagementRequestContext, projectInfo, releaseId, releaseEnvironmentId, fields, publishContext, continuationTokenRunId, continuationTokenResultId)), this.RequestContext);
            IPagedList<FieldDetailsForTestResults> resultsGroupsForRelease1;
            if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultGroupsByReleaseWithWatermark(this.RequestContext, projectInfo, releaseId, releaseEnvironmentId, fields, publishContext, continuationToken, out resultsGroupsForRelease1))
              resultsGroupsForRelease = this.TestManagementRequestContext.MergeDataHelper.MergeTestResultsGroups(resultsGroupsForRelease, resultsGroupsForRelease1);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            resultsGroupsForRelease = testManagementResultService.GetTestResultGroupsByRelease(this.TestManagementRequestContext, projectInfo, releaseId, releaseEnvironmentId, fields, publishContext, continuationTokenRunId, continuationTokenResultId);
          }
          this.SecureTestResultsGroups(resultsGroupsForRelease, projectInfo.Id);
          return resultsGroupsForRelease;
        }), 1015076, "TestManagement");
    }

    public virtual IPagedList<FieldDetailsForTestResults> GetTestResultsGroupsByBuild(
      ProjectInfo projectInfo,
      int buildId,
      string publishContext,
      string fields,
      string continuationToken)
    {
      using (PerfManager.Measure(this.RequestContext, "RestLayer", "ResultGroupsHelper.GetTestResultsGroupsByBuild", 50, true))
        return this.ExecuteAction<IPagedList<FieldDetailsForTestResults>>("ResultGroupsHelper.GetTestResultsGroupsByBuild", (Func<IPagedList<FieldDetailsForTestResults>>) (() =>
        {
          IPagedList<FieldDetailsForTestResults> resultsGroupsForBuild = (IPagedList<FieldDetailsForTestResults>) new PagedList<FieldDetailsForTestResults>((IEnumerable<FieldDetailsForTestResults>) new List<FieldDetailsForTestResults>(), (string) null);
          int continuationTokenRunId;
          int continuationTokenResultId;
          ParsingHelper.ParseContinuationTokenResultId(continuationToken, out continuationTokenRunId, out continuationTokenResultId);
          ITeamFoundationTestManagementResultService testManagementResultService = this.RequestContext.GetService<ITeamFoundationTestManagementResultService>();
          bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
          bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
          if (!flag1 && !flag2)
          {
            TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() => resultsGroupsForBuild = testManagementResultService.GetTestResultGroupsByBuild(this.TestManagementRequestContext, projectInfo, buildId, fields, publishContext, continuationTokenRunId, continuationTokenResultId)), this.RequestContext);
            IPagedList<FieldDetailsForTestResults> resultsGroupsForBuild1;
            if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultGroupsByBuildWithWatermark(this.RequestContext, projectInfo, buildId, fields, publishContext, continuationToken, out resultsGroupsForBuild1))
              resultsGroupsForBuild = this.TestManagementRequestContext.MergeDataHelper.MergeTestResultsGroups(resultsGroupsForBuild, resultsGroupsForBuild1);
          }
          else
          {
            if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
            resultsGroupsForBuild = testManagementResultService.GetTestResultGroupsByBuild(this.TestManagementRequestContext, projectInfo, buildId, fields, publishContext, continuationTokenRunId, continuationTokenResultId);
          }
          this.SecureTestResultsGroups(resultsGroupsForBuild, projectInfo.Id);
          return resultsGroupsForBuild;
        }), 1015075, "TestManagement");
    }

    public void SecureTestResultsGroups(
      IPagedList<FieldDetailsForTestResults> testResultsGroups,
      Guid projectId)
    {
      ISecuredObject securedObject = (ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      };
      foreach (TestManagementBaseSecuredObject testResultsGroup in (IEnumerable<FieldDetailsForTestResults>) testResultsGroups)
        testResultsGroup.InitializeSecureObject(securedObject);
    }
  }
}
