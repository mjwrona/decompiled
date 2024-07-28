// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementWebService3
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/07/TCM/TestManagement/01", Description = "Test Management Service", Name = "TestManagementWebService3")]
  [ClientService(ComponentName = "TestManagement", RegistrationName = "TestManagement", ServiceName = "TestManagementWebService3", CollectionServiceIdentifier = "3CAED749-9E99-4B38-B44C-3458D22A1059")]
  public class TestManagementWebService3 : BaseTestManagementWebService
  {
    private TfsTestManagementRequestContext m_tmRequestContext;
    private Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper m_resultsHelper;

    [WebMethod]
    public int CreateTestFailureType(TestFailureType failureType, string teamProjectName) => throw this.HandleException(new Exception(ServerResources.TestFailureTypeDeprecationMessage));

    [WebMethod]
    public void UpdateTestFailureType(TestFailureType failureType, string projectName) => throw this.HandleException(new Exception(ServerResources.TestFailureTypeDeprecationMessage));

    [WebMethod]
    public void DeleteTestFailureType(int failureTypeId, string projectName) => throw this.HandleException(new Exception(ServerResources.TestFailureTypeDeprecationMessage));

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestFailureType))]
    public List<TestFailureType> QueryTestFailureTypeById(int testFailureTypeId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestFailureTypeById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testFailureTypeId), (object) testFailureTypeId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return TestFailureType.Query((TestManagementRequestContext) this.m_tmRequestContext, testFailureTypeId, projectName);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestFailureType))]
    public List<TestFailureType> QueryTestFailureTypes(string teamProject)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestFailureTypes), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        this.EnterMethod(methodInformation);
        return TestFailureType.Query((TestManagementRequestContext) this.m_tmRequestContext, -1, teamProject);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestFailureType))]
    public List<TestFailureType> ImportFailureTypes(
      TestFailureType[] failureTypes,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ImportFailureTypes), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (failureTypes), (object) failureTypes);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestFailureType[]>(failureTypes, nameof (failureTypes), this.m_tmRequestContext.RequestContext.ServiceName);
        return TestFailureType.ImportFailureTypes((TestManagementRequestContext) this.m_tmRequestContext, failureTypes, projectName);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestResolutionState))]
    public List<TestResolutionState> ImportResolutionStates(
      TestResolutionState[] resolutionStates,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ImportResolutionStates), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (resolutionStates), (object) resolutionStates);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestResolutionState[]>(resolutionStates, nameof (resolutionStates), this.m_tmRequestContext.RequestContext.ServiceName);
        return TestResolutionState.ImportResolutionStates((TestManagementRequestContext) this.m_tmRequestContext, resolutionStates, projectName);
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

    public TestManagementWebService3()
    {
      this.RequestContext.ServiceName = "Test Management 3";
      this.m_tmRequestContext = new TfsTestManagementRequestContext(this.RequestContext);
    }

    [WebMethod]
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPointStatistic))]
    public List<TestPointStatistic> QueryTestPointStatistics(int planId, ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointStatistics), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        List<TestPoint> testPoints = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, int.MaxValue, query, out List<TestPointStatistic> _, false, (string[]) null);
        return TestPointUpdate.GetTestPointStatistics(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPointStatisticsByPivotType))]
    public List<TestPointStatisticsByPivotType> QueryTestPointStatisticsByPivots(
      int planId,
      ResultsStoreQuery query,
      List<TestPointStatisticsQueryPivotType> pivotList)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointStatisticsByPivots), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddArrayParameter<TestPointStatisticsQueryPivotType>(nameof (pivotList), (IList<TestPointStatisticsQueryPivotType>) pivotList);
        List<TestPoint> testPoints1 = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, int.MaxValue, query, out List<TestPointStatistic> _, false, (string[]) null);
        List<TestPoint> testPoints2 = TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints1);
        return TestPointUpdate.GetTestPointStatisticsByPivotType(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints2, pivotList);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPointStatisticsByPivotType))]
    public List<TestPointStatisticsByPivotType> QueryTestPointStatisticsByPivots2(
      int planId,
      ResultsStoreQuery query,
      List<TestPointStatisticsQueryPivotType> pivotList)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointStatisticsByPivots2), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddArrayParameter<TestPointStatisticsQueryPivotType>(nameof (pivotList), (IList<TestPointStatisticsQueryPivotType>) pivotList);
        List<TestPoint> testPoints1 = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, int.MaxValue, query, out List<TestPointStatistic> _, false, (string[]) null);
        List<TestPoint> testPoints2 = TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints1);
        return TestPointUpdate.GetTestPointStatisticsByPivotType(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints2, pivotList, true);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPoint))]
    public List<TestPoint> FetchTestPoints(
      string projectName,
      int planId,
      IdAndRev[] idsToFetch,
      string[] testCaseProperties,
      out List<int> deletedIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestPoints), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddArrayParameter<IdAndRev>(nameof (idsToFetch), (IList<IdAndRev>) idsToFetch);
        this.EnterMethod(methodInformation);
        deletedIds = new List<int>();
        List<TestPoint> testPoints = TestPoint.Fetch((TestManagementRequestContext) this.m_tmRequestContext, projectName, planId, idsToFetch, testCaseProperties, deletedIds);
        return TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, projectName, planId, testPoints);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPoint))]
    public List<TestPoint> QueryTestPointHistory(int testPointId, int planId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointHistory), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testPointId), (object) testPointId);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        List<TestPoint> testPoints = TestPoint.QueryTestPointHistory((TestManagementRequestContext) this.m_tmRequestContext, testPointId, planId, projectName);
        return TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, projectName, planId, testPoints);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPoint))]
    public List<TestPoint> QueryTestPoints(
      int planId,
      ResultsStoreQuery query,
      int pageSize,
      string[] testCaseProperties)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPoints), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddArrayParameter<string>(nameof (testCaseProperties), (IList<string>) testCaseProperties);
        this.EnterMethod(methodInformation);
        List<TestPoint> testPoints = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, pageSize, query, out List<TestPointStatistic> _, false, testCaseProperties);
        return TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestPoint))]
    public List<TestPoint> QueryTestPointsAndStatistics(
      int planId,
      ResultsStoreQuery query,
      int pageSize,
      string[] testCaseProperties,
      [XmlArray, XmlArrayItem(typeof (TestPointStatistic))] out List<TestPointStatistic> stats)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPointsAndStatistics), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddArrayParameter<string>(nameof (testCaseProperties), (IList<string>) testCaseProperties);
        this.EnterMethod(methodInformation);
        List<TestPoint> testPoints1 = TestPoint.Query((TestManagementRequestContext) this.m_tmRequestContext, planId, pageSize, query, out stats, false, testCaseProperties);
        List<TestPoint> testPoints2 = TestPointUpdate.UpdatePointsWithLatestResults(this.m_tmRequestContext, query.TeamProjectName, planId, testPoints1);
        stats = TestPointUpdate.GroupPointsByStatistics(testPoints2);
        return testPoints2;
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
    public int BeginCloneOperation(
      int sourcePlanId,
      TestPlan destinationPlan,
      List<int> sourceSuiteIds,
      string projectName,
      CloneOptions options,
      string targetAreaPath)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (BeginCloneOperation), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (sourcePlanId), (object) sourcePlanId);
        methodInformation.AddParameter(nameof (destinationPlan), (object) destinationPlan);
        methodInformation.AddArrayParameter<int>(nameof (sourceSuiteIds), (IList<int>) sourceSuiteIds);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (options), (object) options);
        methodInformation.AddParameter(nameof (targetAreaPath), (object) targetAreaPath);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestPlan>(destinationPlan, nameof (destinationPlan), this.m_tmRequestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<CloneOptions>(options, nameof (options), this.m_tmRequestContext.RequestContext.ServiceName);
        this.m_tmRequestContext.TraceVerbose("WebService", "BeginCloneOperation: {0}, {1}", (object) sourcePlanId, (object) destinationPlan.PlanId);
        return TestPlan.BeginCloneOperation((TestManagementRequestContext) this.m_tmRequestContext, projectName, sourcePlanId, destinationPlan, sourceSuiteIds, options, targetAreaPath);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (CloneOperationInformation))]
    public List<CloneOperationInformation> FetchCloneInformationForTestPlans(
      string projectName,
      int planId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchCloneInformationForTestPlans), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        this.EnterMethod(methodInformation);
        return TestPlan.FetchCloneInformationForTestPlans((TestManagementRequestContext) this.m_tmRequestContext, projectName, planId);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (int))]
    public List<int> FetchPlanIdsContainingCloneHistory(
      string projectName,
      List<int> planIds,
      bool fetchAllPlans)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchPlanIdsContainingCloneHistory), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddArrayParameter<int>(nameof (planIds), (IList<int>) planIds);
        methodInformation.AddParameter(nameof (fetchAllPlans), (object) fetchAllPlans);
        this.EnterMethod(methodInformation);
        return TestPlan.FetchPlanIdsContainingCloneHistory((TestManagementRequestContext) this.m_tmRequestContext, projectName, planIds, fetchAllPlans);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResults(
      ResultsStoreQuery query,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> excessIds1;
        List<LegacyTestCaseResult> testResultsByQuery = this.ResultsHelper.GetTestResultsByQuery((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(query), pageSize, out excessIds1);
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source1 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> list = source1 != null ? source1.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local = list;
        IEnumerable<TestCaseResult> source2 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) testResultsByQuery);
        return source2 != null ? source2.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> FetchTestResults(
      TestCaseResultIdAndRev[] idsToFetch,
      string projectName,
      bool includeActionResults,
      out List<TestCaseResultIdentifier> deletedIds,
      [XmlArray, XmlArrayItem(typeof (TestActionResult))] out List<TestActionResult> actionResults,
      [XmlArray, XmlArrayItem(typeof (TestResultParameter))] out List<TestResultParameter> parameters,
      [XmlArray, XmlArrayItem(typeof (TestResultAttachment))] out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation("FetchTestResultsIds", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestCaseResultIdAndRev>(nameof (idsToFetch), (IList<TestCaseResultIdAndRev>) idsToFetch);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (includeActionResults), (object) includeActionResults);
        this.EnterMethod(methodInformation);
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> source1 = TestCaseResultIdAndRevConverter.Convert((IEnumerable<TestCaseResultIdAndRev>) idsToFetch);
        List<LegacyTestCaseResultIdentifier> webApiDeletedIds;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments;
        List<LegacyTestCaseResult> testCaseResults = this.ResultsHelper.Fetch((TestManagementRequestContext) this.m_tmRequestContext, source1 != null ? source1.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>) null, projectName, includeActionResults, out webApiDeletedIds, out webApiActionResults, out webApiParams, out webApiAttachments);
        ref List<TestActionResult> local1 = ref actionResults;
        IEnumerable<TestActionResult> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) webApiActionResults);
        List<TestActionResult> list1 = source2 != null ? source2.ToList<TestActionResult>() : (List<TestActionResult>) null;
        local1 = list1;
        ref List<TestResultParameter> local2 = ref parameters;
        IEnumerable<TestResultParameter> source3 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) webApiParams);
        List<TestResultParameter> list2 = source3 != null ? source3.ToList<TestResultParameter>() : (List<TestResultParameter>) null;
        local2 = list2;
        ref List<TestResultAttachment> local3 = ref attachments;
        IEnumerable<TestResultAttachment> source4 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) webApiAttachments);
        List<TestResultAttachment> list3 = source4 != null ? source4.ToList<TestResultAttachment>() : (List<TestResultAttachment>) null;
        local3 = list3;
        ref List<TestCaseResultIdentifier> local4 = ref deletedIds;
        IEnumerable<TestCaseResultIdentifier> source5 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) webApiDeletedIds);
        List<TestCaseResultIdentifier> list4 = source5 != null ? source5.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local4 = list4;
        IEnumerable<TestCaseResult> source6 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) testCaseResults);
        return source6 != null ? source6.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByRun(
      int testRunId,
      int pageSize,
      string projectName,
      bool includeActionResults,
      out List<TestCaseResultIdentifier> excessIds,
      [XmlArray, XmlArrayItem(typeof (TestActionResult))] out List<TestActionResult> actionResults,
      [XmlArray, XmlArrayItem(typeof (TestResultParameter))] out List<TestResultParameter> parameters,
      [XmlArray, XmlArrayItem(typeof (TestResultAttachment))] out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByRun), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (includeActionResults), (object) includeActionResults);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> webApiExcessIds;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments;
        IEnumerable<TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRun((TestManagementRequestContext) this.m_tmRequestContext, testRunId, pageSize, out webApiExcessIds, projectName, includeActionResults, out webApiActionResults, out webApiParams, out webApiAttachments));
        List<TestCaseResult> list1 = source1 != null ? source1.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
        ref List<TestActionResult> local1 = ref actionResults;
        IEnumerable<TestActionResult> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) webApiActionResults);
        List<TestActionResult> list2 = source2 != null ? source2.ToList<TestActionResult>() : (List<TestActionResult>) null;
        local1 = list2;
        ref List<TestResultParameter> local2 = ref parameters;
        IEnumerable<TestResultParameter> source3 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) webApiParams);
        List<TestResultParameter> list3 = source3 != null ? source3.ToList<TestResultParameter>() : (List<TestResultParameter>) null;
        local2 = list3;
        ref List<TestResultAttachment> local3 = ref attachments;
        IEnumerable<TestResultAttachment> source4 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) webApiAttachments);
        List<TestResultAttachment> list4 = source4 != null ? source4.ToList<TestResultAttachment>() : (List<TestResultAttachment>) null;
        local3 = list4;
        ref List<TestCaseResultIdentifier> local4 = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source5 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) webApiExcessIds);
        List<TestCaseResultIdentifier> list5 = source5 != null ? source5.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local4 = list5;
        return list1;
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByPoint(
      string projectName,
      int planId,
      int pointId)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByPoint), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (pointId), (object) pointId);
        this.EnterMethod(methodInformation);
        IEnumerable<TestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByPoint((TestManagementRequestContext) this.m_tmRequestContext, projectName, planId, pointId));
        return source != null ? source.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByRunAndOwner(
      int testRunId,
      Guid owner,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByRunAndOwner), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (owner), (object) owner);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> excessIds1;
        IEnumerable<TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRunAndOwner((TestManagementRequestContext) this.m_tmRequestContext, testRunId, owner, pageSize, out excessIds1, projectName));
        List<TestCaseResult> list1 = source1 != null ? source1.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> list2 = source2 != null ? source2.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local = list2;
        return list1;
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByRunAndState(
      int testRunId,
      byte state,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByRunAndState), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (state), (object) state);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> excessIds1;
        IEnumerable<TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRunAndState((TestManagementRequestContext) this.m_tmRequestContext, testRunId, state, pageSize, out excessIds1, projectName));
        List<TestCaseResult> list1 = source1 != null ? source1.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> list2 = source2 != null ? source2.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local = list2;
        return list1;
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestCaseResult))]
    public List<TestCaseResult> QueryTestResultsByRunAndOutcome(
      int testRunId,
      byte outcome,
      int pageSize,
      out List<TestCaseResultIdentifier> excessIds,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestResultsByRunAndOutcome), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (outcome), (object) outcome);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        List<LegacyTestCaseResultIdentifier> excessIds1;
        IEnumerable<TestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) this.ResultsHelper.QueryByRunAndOutcome((TestManagementRequestContext) this.m_tmRequestContext, testRunId, outcome, pageSize, out excessIds1, projectName));
        List<TestCaseResult> list1 = source1 != null ? source1.ToList<TestCaseResult>() : (List<TestCaseResult>) null;
        ref List<TestCaseResultIdentifier> local = ref excessIds;
        IEnumerable<TestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) excessIds1);
        List<TestCaseResultIdentifier> list2 = source2 != null ? source2.ToList<TestCaseResultIdentifier>() : (List<TestCaseResultIdentifier>) null;
        local = list2;
        return list1;
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
    public TestCaseResult FindTestResultInMultipleProjects(
      int testRunId,
      int testResultId,
      out string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (FindTestResultInMultipleProjects), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (testResultId), (object) testResultId);
        this.EnterMethod(methodInformation);
        return TestCaseResultContractConverter.Convert(this.ResultsHelper.GetTestResultInMultipleProjects((TestManagementRequestContext) this.m_tmRequestContext, testRunId, testResultId, out projectName));
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
    public TestCaseResult[] ResetTestResults(
      TestCaseResultIdentifier[] identifiers,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (ResetTestResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<TestCaseResultIdentifier>(nameof (identifiers), (IList<TestCaseResultIdentifier>) identifiers);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper resultsHelper = this.ResultsHelper;
        TfsTestManagementRequestContext tmRequestContext = this.m_tmRequestContext;
        IEnumerable<LegacyTestCaseResultIdentifier> source1 = TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) identifiers);
        LegacyTestCaseResultIdentifier[] array = source1 != null ? source1.ToArray<LegacyTestCaseResultIdentifier>() : (LegacyTestCaseResultIdentifier[]) null;
        string projectName1 = projectName;
        IEnumerable<TestCaseResult> source2 = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) resultsHelper.ResetTestResults((TestManagementRequestContext) tmRequestContext, array, projectName1));
        return source2 != null ? source2.ToArray<TestCaseResult>() : (TestCaseResult[]) null;
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestActionResult))]
    public List<TestActionResult> QueryTestActionResults(
      TestCaseResultIdentifier identifier,
      [XmlArray, XmlArrayItem(typeof (TestResultParameter))] out List<TestResultParameter> parameters,
      [XmlArray, XmlArrayItem(typeof (TestResultAttachment))] out List<TestResultAttachment> attachments,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestActionResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (identifier), (object) identifier);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        QueryTestActionResultResponse actionResultResponse = this.ResultsHelper.QueryTestActionResults((TestManagementRequestContext) this.m_tmRequestContext, projectName, identifier);
        IEnumerable<TestActionResult> source1 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) actionResultResponse.TestActionResults);
        List<TestActionResult> list1 = source1 != null ? source1.ToList<TestActionResult>() : (List<TestActionResult>) null;
        ref List<TestResultParameter> local1 = ref parameters;
        IEnumerable<TestResultParameter> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) actionResultResponse.TestResultParameters);
        List<TestResultParameter> list2 = source2 != null ? source2.ToList<TestResultParameter>() : (List<TestResultParameter>) null;
        local1 = list2;
        ref List<TestResultAttachment> local2 = ref attachments;
        IEnumerable<TestResultAttachment> source3 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) actionResultResponse.TestAttachments);
        List<TestResultAttachment> list3 = source3 != null ? source3.ToList<TestResultAttachment>() : (List<TestResultAttachment>) null;
        local2 = list3;
        return list1;
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
    public List<TestRunStatistic> QueryTestRunStatistics(string projectName, int testRunId)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunStatistics), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        IEnumerable<TestRunStatistic> source = TestRunStatisticConverter.Convert((IEnumerable<LegacyTestRunStatistic>) this.ResultsHelper.QueryTestRunStats((TestManagementRequestContext) this.m_tmRequestContext, projectName, testRunId));
        return source != null ? source.ToList<TestRunStatistic>() : (List<TestRunStatistic>) null;
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
    public ResultUpdateResponse[] UpdateTestResults(
      ResultUpdateRequest[] requests,
      string projectName)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestResults), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<ResultUpdateRequest>(nameof (requests), (IList<ResultUpdateRequest>) requests);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest> source1 = ResultUpdateRequestConverter.Convert((IEnumerable<ResultUpdateRequest>) requests);
        IEnumerable<ResultUpdateResponse> source2 = ResultUpdateResponseConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse>) this.ResultsHelper.Update((TestManagementRequestContext) this.m_tmRequestContext, source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[]) null, projectName));
        return source2 != null ? source2.ToArray<ResultUpdateResponse>() : (ResultUpdateResponse[]) null;
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
    public TestRun CreateTestRun(
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
        MethodInformation methodInformation = new MethodInformation("CreateTestRun2" + str, MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (testRun), (object) testRun);
        methodInformation.AddParameter(nameof (settings), (object) settings);
        methodInformation.AddArrayParameter<TestCaseResult>(nameof (results), (IList<TestCaseResult>) results);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestRun>(testRun, nameof (testRun), "Test Results");
        testRun.ThrowInvalidOperationIfRunHasDtlEnvironment();
        testRun.SourceWorkflow = Compat2011QU1Helper.GetSourceWorkflowForTestRun(testRun);
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName);
        if (results != null && results.Length != 0)
          this.m_tmRequestContext.WorkItemFieldDataHelper.ValidateParamsAndUpdateResultsWithTestCasePropertiesIfRequired((TestManagementRequestContext) this.m_tmRequestContext, projectFromName, results, false);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun1 = TestRunContractConverter.Convert(testRun);
        IEnumerable<LegacyTestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) results);
        LegacyTestCaseResult[] array = source != null ? source.ToArray<LegacyTestCaseResult>() : (LegacyTestCaseResult[]) null;
        LegacyTestSettings testSettings = TestSettingsContractConverter.Convert(settings);
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun2 = this.ResultsHelper.CreateTestRun((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, testRun1, array, testSettings);
        if (this.m_tmRequestContext.TestPointOutcomeHelper.IsDualWriteEnabled(this.m_tmRequestContext.RequestContext))
        {
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResultsByRunId = this.ResultsHelper.GetTestResultsByRunId((TestManagementRequestContext) this.m_tmRequestContext, projectFromName.GuidId, testRun2.TestRunId);
          this.m_tmRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(this.m_tmRequestContext.RequestContext, teamProjectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testResultsByRunId);
        }
        return TestRunContractConverter.Convert(testRun2);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRuns(ResultsStoreQuery query, bool includeStatistics)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRuns), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        IEnumerable<TestRun> source = TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(query), includeStatistics));
        return TestRun.FilterNotOfType(source != null ? (IEnumerable<TestRun>) source.ToList<TestRun>() : (IEnumerable<TestRun>) null, Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRunsInMultipleProjects(ResultsStoreQuery query)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunsInMultipleProjects), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return TestRun.FilterNotOfType(TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.QueryTestRunsInMultipleProjects((TestManagementRequestContext) this.m_tmRequestContext, ResultsStoreQueryContractConverter.Convert(query))), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRunsById(string teamProjectName, int testRunId)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunsById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testRunId), (object) testRunId);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        this.EnterMethod(methodInformation);
        return TestRun.FilterNotOfType(TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, testRunId, Guid.Empty, (string) null, teamProjectName)), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRunsByBuild(string teamProjectName, string buildUri)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunsByBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        return TestRun.FilterNotOfType(TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, 0, Guid.Empty, buildUri, teamProjectName)), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
    [return: XmlArray]
    [return: XmlArrayItem(typeof (TestRun))]
    public List<TestRun> QueryTestRunsByOwner(string teamProjectName, Guid owner)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunsByOwner), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (owner), (object) owner);
        this.EnterMethod(methodInformation);
        return TestRun.FilterNotOfType(TestRunContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) this.ResultsHelper.Query((TestManagementRequestContext) this.m_tmRequestContext, 0, owner, (string) null, teamProjectName)), Microsoft.TeamFoundation.TestManagement.Client.TestRunType.RunWithDtlEnv);
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
    public TestRun QueryTestRunByTmiRunId(Guid tmiRunId)
    {
      try
      {
        this.RequestContext.ServiceName = "Test Results";
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestRunByTmiRunId), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (tmiRunId), (object) tmiRunId);
        this.EnterMethod(methodInformation);
        TestRun testRun = TestRunContractConverter.Convert(this.ResultsHelper.QueryTestRunByTmiRunId((TestManagementRequestContext) this.m_tmRequestContext, tmiRunId));
        return testRun != null && !testRun.RunHasDtlEnvironment ? testRun : (TestRun) null;
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
