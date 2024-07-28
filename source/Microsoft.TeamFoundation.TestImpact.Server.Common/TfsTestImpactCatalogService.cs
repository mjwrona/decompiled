// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TfsTestImpactCatalogService
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestImpact.Server.Common.Properties;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class TfsTestImpactCatalogService : ITfsTestImpactCatalogService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public BuildType QueryTIAEnabledRun(
      TestImpactRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      Microsoft.TeamFoundation.Build.WebApi.Build buildDetail = TestImpactServer.GetBuildDetail(requestContext, projectId, buildId);
      using (DefinitionRunDatabase component = requestContext1.CreateComponent<DefinitionRunDatabase>())
        return component.QueryIfRebaseRun(projectId, 0, buildDetail.Definition.Id, buildDetail.Id);
    }

    public ImpactedTests QueryImpactedTests(
      TestImpactRequestContext requestContext,
      Guid projectId,
      DefinitionRunInfo definitionRunInfo,
      int currentTestRunId,
      TestInclusionOptions typesToInclude)
    {
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      ImpactedTests impactedTests1 = new ImpactedTests();
      BuildType buildType = BuildType.TestImpactOff;
      requestContext1.Trace(15113061, TraceLevel.Info, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, "QueryImpactedTests: ProjectId: {0} Buildid: {0}", (object) projectId, (object) definitionRunInfo.DefinitionRunId);
      try
      {
        int num = 0;
        string projectName = string.Empty;
        ImpactedTests impactedTests2 = new ImpactedTests();
        if (definitionRunInfo.DefinitionType == Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Build)
        {
          num = TestImpactServer.GetBuildDetail(requestContext, projectId, definitionRunInfo.DefinitionRunId).Definition.Id;
          if (!requestContext.RequestContext.GetService<IProjectService>().TryGetProjectName(requestContext.RequestContext, projectId, out projectName))
            projectName = string.Empty;
        }
        else if (definitionRunInfo.DefinitionType == Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Release)
        {
          num = TestImpactServer.GetReleaseDetail(requestContext, definitionRunInfo.DefinitionRunId, projectId).ReleaseDefinitionReference.Id;
          projectName = TestImpactServer.GetProjectName(requestContext, projectId);
        }
        Dictionary<string, object> dictionary = new Dictionary<string, object>()
        {
          {
            TestImpactServiceCIProperty.DefinitionRunId,
            (object) definitionRunInfo.DefinitionRunId
          },
          {
            TestImpactServiceCIProperty.DefinitionType,
            (object) definitionRunInfo.DefinitionType
          },
          {
            TestImpactServiceCIProperty.DefinitionId,
            (object) num
          },
          {
            TestImpactServiceCIProperty.TcmRunId,
            (object) currentTestRunId
          }
        };
        using (DefinitionRunDatabase component = requestContext1.CreateComponent<DefinitionRunDatabase>())
          buildType = component.QueryIfRebaseRun(projectId, (int) definitionRunInfo.DefinitionType, num, definitionRunInfo.DefinitionRunId);
        if (buildType == BuildType.TestImpactOn)
        {
          ImpactedTests impactedTests3 = this.QueryImpactedTestsDev15(requestContext1, projectId, num, definitionRunInfo.DefinitionRunId, definitionRunInfo.DefinitionType);
          impactedTests1 = this.ModifyImpactedSetBasedForNewAndFailedTests(requestContext, projectId, projectName, definitionRunInfo.BaseLineDefinitionRunId, num, definitionRunInfo.DefinitionType, currentTestRunId, typesToInclude, impactedTests3, dictionary);
        }
        else
          impactedTests1.AreAllTestsImpacted = true;
        dictionary.Add(TestImpactServiceCIProperty.IsRebase, (object) impactedTests1.AreAllTestsImpacted);
        CILogger.Instance.PublishCI(requestContext1, TestImpactServiceCIFeature.QueryImpactedTests, dictionary);
      }
      catch (Exception ex)
      {
        requestContext1.TraceException(15113071, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, ex);
        impactedTests1.AreAllTestsImpacted = true;
      }
      requestContext1.Trace(15113080, TraceLevel.Info, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, "QueryImpactedTests: Total Impacted tests: {0}", (object) impactedTests1.Tests.Count);
      return impactedTests1;
    }

    public void PublishCodeSignatures(
      TestImpactRequestContext requestContext,
      Guid projectId,
      TestResultSignaturesInfo results)
    {
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      requestContext1.Trace(15113091, TraceLevel.Info, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, "PublishCodeSignatures: ProjectId: {0} Runid: {0}", (object) projectId, (object) results.TestRunId);
      this.ValidatePublishCodeSignaturesPageSizeLimit(results.TestResultSignatures.ToArray().Length);
      try
      {
        if (results.DefinitionType == Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Build)
        {
          Microsoft.TeamFoundation.Build.WebApi.Build buildDetail = TestImpactServer.GetBuildDetail(requestContext, projectId, results.DefinitionRunId);
          TestImpactServer.PublishImpactData(requestContext, projectId, buildDetail.Definition.Id, results);
        }
        else
        {
          if (results.DefinitionType != Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Release)
            return;
          Release releaseDetail = TestImpactServer.GetReleaseDetail(requestContext, results.DefinitionRunId, projectId);
          TestImpactServer.PublishImpactData(requestContext, projectId, releaseDetail.ReleaseDefinitionReference.Id, results);
        }
      }
      catch (Exception ex)
      {
        requestContext1.TraceException(15113095, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, ex);
        throw;
      }
    }

    private void ValidatePublishCodeSignaturesPageSizeLimit(int publishCodeSignaturesLength)
    {
      if (publishCodeSignaturesLength > 500)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MaxCodeSignaturesLimitCrossedError, (object) 500));
    }

    private ImpactedTests ModifyImpactedSetBasedForNewAndFailedTests(
      TestImpactRequestContext requestContext,
      Guid projectId,
      string projectName,
      int baseLineBuildId,
      int definitionId,
      Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType definitionType,
      int currentRunId,
      TestInclusionOptions typesToInclude,
      ImpactedTests impactedTests,
      Dictionary<string, object> ciData)
    {
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      ImpactedTests testsForAgivenRun = TestImpactServer.GetAllTestsForAGivenRun(requestContext, currentRunId, projectId);
      IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test> tests1 = impactedTests.Tests.Intersect<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>((IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>) testsForAgivenRun.Tests, (IEqualityComparer<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>) new ImpactedTestsEqualityComparer());
      stopwatch1.Stop();
      ciData.Add(TestImpactServiceCIProperty.TimeTakenToFindDiscoveredTests, (object) stopwatch1.ElapsedMilliseconds);
      ciData.Add(TestImpactServiceCIProperty.DiscoveredTestCases, (object) testsForAgivenRun.Tests.Count);
      Stopwatch stopwatch2 = Stopwatch.StartNew();
      ImpactedTests impactedTests1 = this.QueryAllTestsInDb(requestContext, projectId, currentRunId, definitionId, definitionType);
      if (impactedTests1.Tests != null && impactedTests1.Tests.Count > 0)
      {
        IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test> tests2 = testsForAgivenRun.Tests.Except<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>((IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>) impactedTests1.Tests, (IEqualityComparer<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>) new ImpactedTestsEqualityComparer());
        stopwatch2.Stop();
        ciData.Add(TestImpactServiceCIProperty.TimeTakenToFindNewTests, (object) stopwatch2.ElapsedMilliseconds);
        Stopwatch stopwatch3 = Stopwatch.StartNew();
        List<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test> first = new List<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>();
        if (typesToInclude == TestInclusionOptions.Failed)
          first = definitionType != Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Build ? TestImpactServer.GetFailedTests(requestContext, Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Release, currentRunId, baseLineBuildId, projectId) : TestImpactServer.GetFailedTests(requestContext, Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Build, currentRunId, baseLineBuildId, projectId);
        IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test> tests3 = first.Intersect<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>((IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>) testsForAgivenRun.Tests, (IEqualityComparer<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>) new ImpactedTestsEqualityComparer());
        IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test> second = tests3.Except<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>(tests1, (IEqualityComparer<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>) new ImpactedTestsEqualityComparer());
        stopwatch3.Stop();
        ciData.Add(TestImpactServiceCIProperty.TimeTakenToFindFailedTests, (object) stopwatch3.ElapsedMilliseconds);
        IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test> collection = tests1.Concat<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>(tests2).Concat<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>(second);
        ImpactedTests impactedTests2 = new ImpactedTests();
        impactedTests2.Tests = new List<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>(collection);
        ciData.Add(TestImpactServiceCIProperty.NewTestCases, (object) tests2.Count<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>());
        ciData.Add(TestImpactServiceCIProperty.FailedTestCases, (object) tests3.Count<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>());
        ciData.Add(TestImpactServiceCIProperty.UnimpactedTestCases, (object) (testsForAgivenRun.Tests.Count - impactedTests2.Tests.Count));
        return impactedTests2;
      }
      impactedTests = testsForAgivenRun;
      return impactedTests;
    }

    private ImpactedTests QueryImpactedTestsDev15(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      int defintionId,
      int buildId,
      Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType definitionType)
    {
      tfsRequestContext.Trace(15113061, TraceLevel.Info, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, "QueryImpactedTests: Using new test impact flow");
      ImpactedTests impactedTests = new ImpactedTests();
      using (ImpactDatabase component = tfsRequestContext.CreateComponent<ImpactDatabase>())
        return component.QueryImpactedTests(projectId, (int) definitionType, defintionId, buildId);
    }

    private ImpactedTests QueryAllTestsInDb(
      TestImpactRequestContext requestContext,
      Guid project,
      int runId,
      int definitionId,
      Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType definitionType)
    {
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      ImpactedTests impactedTests = new ImpactedTests();
      using (ImpactDatabase component = requestContext1.CreateComponent<ImpactDatabase>())
        return component.QueryAllTests(project, (int) definitionType, definitionId, runId);
    }
  }
}
