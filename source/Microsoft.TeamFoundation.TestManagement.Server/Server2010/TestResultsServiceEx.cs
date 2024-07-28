// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestResultsServiceEx
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2007/02/TCM/TestResults/01", Description = "Test Management Results Service", Name = "TestResultsServiceEx")]
  [ClientService(ComponentName = "TestManagement", RegistrationName = "TestManagement", ServiceName = "TestResultsServiceEx", CollectionServiceIdentifier = "7826fd15-ef51-42a4-83c5-f7c659d5a835")]
  public class TestResultsServiceEx : TeamFoundationWebService
  {
    private TfsTestManagementRequestContext m_tmRequestContext;
    private Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper m_resultsHelper;

    public TestResultsServiceEx()
    {
      this.RequestContext.ServiceName = "Test Management Ex";
      this.m_tmRequestContext = new TfsTestManagementRequestContext(this.RequestContext);
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPoint))]
    public List<TestPoint> QueryTestPointsAndStatistics(
      int planId,
      ResultsStoreQuery query,
      int pageSize,
      [XmlArray, XmlArrayItem(typeof (TestPointStatistic))] out List<TestPointStatistic> stats)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointsAndStatistics), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        List<Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic> stats1 = new List<Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic>();
        List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPoints = Microsoft.TeamFoundation.TestManagement.Server.TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, pageSize, Compat2010Helper.Convert(query), out stats1, true, (string[]) null);
        List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPointList = TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints);
        stats = Compat2010Helper.Convert(TestPointUpdate.GroupPointsByStatistics(testPointList));
        return Compat2010Helper.Convert(testPointList);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TestRun CreateTestRun2(
      TestRun testRun,
      TestSettings settings,
      TestCaseResult[] results,
      string teamProjectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        string str = string.Empty;
        if (testRun != null)
          str = testRun.TestPlanId != 0 ? (!testRun.IsAutomated ? (testRun.Type == (byte) 8 ? "_M1" : "_M") : "_A2") : "_A1";
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestRun2) + str, MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (testRun), (object) testRun);
        methodInformation.AddParameter(nameof (settings), (object) settings);
        methodInformation.AddArrayParameter<TestCaseResult>(nameof (results), (IList<TestCaseResult>) results);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestRun>(testRun, nameof (testRun), "Test Results");
        GuidAndString projectFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        if (results != null && results.Length != 0)
          this.m_tmRequestContext.WorkItemFieldDataHelper.ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired((TestManagementRequestContext) this.m_tmRequestContext, projectFromName, Compat2010Helper.Convert(results), false);
        LegacyTestSettings testSettings = TestSettingsContractConverter.Convert(Compat2010Helper.Convert(settings));
        IEnumerable<LegacyTestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) Compat2010Helper.Convert(results));
        LegacyTestCaseResult[] array = source != null ? source.ToArray<LegacyTestCaseResult>() : (LegacyTestCaseResult[]) null;
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun1 = TestRunContractConverter.Convert(Compat2010Helper.Convert(testRun));
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun2 = this.ResultsHelper.CreateTestRun((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, testRun1, array, testSettings);
        if (this.m_tmRequestContext.TestPointOutcomeHelper.IsDualWriteEnabled(this.m_tmRequestContext.RequestContext))
        {
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResultsByRunId = this.ResultsHelper.GetTestResultsByRunId((TestManagementRequestContext) this.m_tmRequestContext, projectFromName.GuidId, testRun2.TestRunId);
          this.m_tmRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(this.m_tmRequestContext.RequestContext, teamProjectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testResultsByRunId);
        }
        return Compat2010Helper.Convert(TestRunContractConverter.Convert(testRun2));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    internal Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper ResultsHelper
    {
      get
      {
        if (this.m_resultsHelper == null)
          this.m_resultsHelper = new Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper((TestManagementRequestContext) this.m_tmRequestContext);
        return this.m_resultsHelper;
      }
    }
  }
}
