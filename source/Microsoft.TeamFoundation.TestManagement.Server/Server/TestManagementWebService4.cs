// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementWebService4
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/07/TCM/TestManagement/01", Description = "Test Management Service", Name = "TestManagementWebService4")]
  [ClientService(ComponentName = "TestManagement", RegistrationName = "TestManagement", ServiceName = "TestManagementWebService4", CollectionServiceIdentifier = "2A0B8D72-EE57-4E41-99B3-924B38C2198D")]
  public class TestManagementWebService4 : BaseTestManagementWebService
  {
    private TfsTestManagementRequestContext m_tmRequestContext;

    public TestManagementWebService4()
    {
      this.RequestContext.ServiceName = "Test Management 4";
      this.m_tmRequestContext = new TfsTestManagementRequestContext(this.RequestContext);
    }

    [WebMethod]
    public TestPlan CreateTestPlanFromExistingWorkItem(string teamProjectName, int workItemId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestPlanFromExistingWorkItem), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (workItemId), (object) workItemId);
        this.EnterMethod(methodInformation);
        return new TestPlan().CreateTestPlanFromExistingWorkItem((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, workItemId, TestPlanSource.Mtm);
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
    [return: XmlArrayItem(typeof (SkinnyPlan))]
    public List<SkinnyPlan> QueryTestPlans2(
      ResultsStoreQuery query,
      bool excludePlansWithNoRootSuite)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPlans2), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (excludePlansWithNoRootSuite), (object) excludePlansWithNoRootSuite);
        this.EnterMethod(methodInformation);
        return TestPlan.Query((TestManagementRequestContext) this.m_tmRequestContext, query, false, excludePlansWithNoRootSuite);
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
    [return: XmlArrayItem(typeof (TestPlan))]
    public List<TestPlan> FetchTestPlans2(
      IdAndRev[] idsToFetch,
      out List<int> deletedIds,
      string projectName,
      bool excludePlansWithNoRootSuite)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestPlans2), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<IdAndRev>(nameof (idsToFetch), (IList<IdAndRev>) idsToFetch);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (excludePlansWithNoRootSuite), (object) excludePlansWithNoRootSuite);
        this.EnterMethod(methodInformation);
        deletedIds = new List<int>();
        return TestPlan.Fetch((TestManagementRequestContext) this.m_tmRequestContext, idsToFetch, deletedIds, projectName, excludePlansWithNoRootSuite);
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
    public TestPlan UpdateTestPlan(
      TestPlan testPlan,
      string projectName,
      TestExternalLink[] changedLinks)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestPlan), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testPlan), (object) testPlan);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddArrayParameter<TestExternalLink>(nameof (changedLinks), (IList<TestExternalLink>) changedLinks);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestPlan>(testPlan, nameof (testPlan), this.m_tmRequestContext.RequestContext.ServiceName);
        return testPlan.Update((TestManagementRequestContext) this.m_tmRequestContext, projectName, changedLinks);
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
    public void MarkTestPlanAsDeleted(int testPlanId, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (MarkTestPlanAsDeleted), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter("TestPlanId", (object) testPlanId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        TestPlan.Delete((TestManagementRequestContext) this.m_tmRequestContext, projectName, testPlanId, true);
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
    public List<StateTypeEnumAndStateString> GetTestPlanDefaultStatesMap()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetTestPlanDefaultStatesMap), MethodType.Normal, EstimatedMethodCost.Low));
        return new TestPlanWorkItem().GetDefaultStatesMap();
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
    [return: XmlArrayItem(typeof (TestExternalLink))]
    public List<TestExternalLink> QueryTestPlanLinks(string projectName, int planId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPlanLinks), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        this.EnterMethod(methodInformation);
        return TestExternalLink.QueryTestPlanLinks(this.m_tmRequestContext, projectName, planId);
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
    [return: XmlArrayItem(typeof (SkinnyPlan))]
    public List<SkinnyPlan> QueryTestPlans(ResultsStoreQuery query)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestPlans), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        this.EnterMethod(methodInformation);
        return TestPlan.Query((TestManagementRequestContext) this.m_tmRequestContext, query, false);
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
    public int CreateTestPlanWithRequirements(
      int planId,
      string planName,
      string teamProjectName,
      string areaPath,
      string iteration,
      string description,
      DateTime startDate,
      DateTime endDate,
      Guid owner,
      List<int> requirementIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestPlanWithRequirements), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        methodInformation.AddParameter(nameof (planName), (object) planName);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (areaPath), (object) areaPath);
        methodInformation.AddParameter(nameof (iteration), (object) iteration);
        methodInformation.AddParameter(nameof (description), (object) description);
        methodInformation.AddParameter(nameof (startDate), (object) startDate);
        methodInformation.AddParameter(nameof (endDate), (object) endDate);
        methodInformation.AddParameter(nameof (owner), (object) owner);
        methodInformation.AddArrayParameter<int>(nameof (requirementIds), (IList<int>) requirementIds);
        this.EnterMethod(methodInformation);
        return TestPlan.CreateWithRequirements((TestManagementRequestContext) this.m_tmRequestContext, planId, planName, teamProjectName, areaPath, iteration, description, startDate, endDate, owner, requirementIds, TestPlanSource.Mtm);
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
    public TestPlan CreateTestPlan(
      TestPlan testPlan,
      string teamProjectName,
      TestExternalLink[] links)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTestPlan), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testPlan), (object) testPlan);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddArrayParameter<TestExternalLink>(nameof (links), (IList<TestExternalLink>) links);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<TestPlan>(testPlan, nameof (testPlan), this.m_tmRequestContext.RequestContext.ServiceName);
        return testPlan.Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, links, TestPlanSource.Mtm);
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
    [return: XmlArrayItem(typeof (TestPlan))]
    public List<TestPlan> FetchTestPlans(
      IdAndRev[] idsToFetch,
      out List<int> deletedIds,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestPlans), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<IdAndRev>(nameof (idsToFetch), (IList<IdAndRev>) idsToFetch);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        deletedIds = new List<int>();
        return TestPlan.Fetch((TestManagementRequestContext) this.m_tmRequestContext, idsToFetch, deletedIds, projectName);
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
    public UpdatedProperties AddCasesToSuite(
      IdAndRev parent,
      TestCaseAndOwner[] testCases,
      int toIndex,
      string projectName,
      [XmlArray, XmlArrayItem(typeof (int))] out List<int> configurationIds,
      [XmlArray, XmlArrayItem(typeof (string))] out List<string> configurationNames)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddCasesToSuite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (parent), (object) parent);
        methodInformation.AddArrayParameter<TestCaseAndOwner>(nameof (testCases), (IList<TestCaseAndOwner>) testCases);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<IdAndRev>(parent, nameof (parent), this.m_tmRequestContext.RequestContext.ServiceName);
        return ServerTestSuite.CreateEntries((TestManagementRequestContext) this.m_tmRequestContext, parent, testCases, toIndex, projectName, out configurationIds, out configurationNames, TestSuiteSource.Mtm);
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
    [return: XmlArrayItem(typeof (ServerTestSuite))]
    public List<ServerTestSuite> QueryTestSuites(
      ResultsStoreQuery query,
      int pageSize,
      out SuiteIdAndType[] excessIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestSuites), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (query), (object) query);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        this.EnterMethod(methodInformation);
        List<SuiteIdAndType> excessIds1 = new List<SuiteIdAndType>();
        List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.Query((TestManagementRequestContext) this.m_tmRequestContext, query, pageSize, excessIds1);
        excessIds = excessIds1.ToArray();
        return serverTestSuiteList;
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
    [return: XmlArrayItem(typeof (ServerTestSuite))]
    public List<ServerTestSuite> QueryTestSuitesByTestCaseId(
      string teamProjectName,
      int testCaseId,
      int pageSize,
      out SuiteIdAndType[] excessIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryTestSuitesByTestCaseId), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (testCaseId), (object) testCaseId);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        this.EnterMethod(methodInformation);
        List<SuiteIdAndType> excessIds1 = new List<SuiteIdAndType>();
        List<ServerTestSuite> serverTestSuiteList = ServerTestSuite.QueryByTestCaseId((TestManagementRequestContext) this.m_tmRequestContext, testCaseId, teamProjectName, pageSize, excessIds1);
        excessIds = excessIds1.ToArray();
        return serverTestSuiteList;
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
    public int CopyTestSuiteEntries(
      string teamProjectName,
      IdAndRev fromSuiteId,
      List<TestSuiteEntry> entries,
      IdAndRev toSuiteId,
      int toIndex)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CopyTestSuiteEntries), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (fromSuiteId), (object) fromSuiteId);
        methodInformation.AddArrayParameter<TestSuiteEntry>(nameof (entries), (IList<TestSuiteEntry>) entries);
        methodInformation.AddParameter(nameof (toSuiteId), (object) toSuiteId);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<IdAndRev>(fromSuiteId, nameof (fromSuiteId), this.m_tmRequestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<IdAndRev>(toSuiteId, nameof (toSuiteId), this.m_tmRequestContext.RequestContext.ServiceName);
        return ServerTestSuite.CopyEntries((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, fromSuiteId.Id, entries, toSuiteId, toIndex);
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
    [return: XmlArrayItem(typeof (ServerTestSuite))]
    public List<ServerTestSuite> FetchTestSuites(
      string teamProjectName,
      IdAndRev[] suiteIds,
      out List<int> deletedIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestSuites), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddArrayParameter<IdAndRev>("suitesIds", (IList<IdAndRev>) suiteIds);
        this.EnterMethod(methodInformation);
        deletedIds = new List<int>();
        return ServerTestSuite.FetchWithRepopulate((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, suiteIds, deletedIds);
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
    [return: XmlArrayItem(typeof (ServerTestSuite))]
    public List<ServerTestSuite> FetchTestSuitesForPlan(string teamProjectName, int planId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (FetchTestSuitesForPlan), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (planId), (object) planId);
        this.EnterMethod(methodInformation);
        return ServerTestSuite.FetchTestSuitesForPlan((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, planId, false);
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
    public UpdatedProperties UpdateTestSuite(ServerTestSuite testSuite, string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateTestSuite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (testSuite), (object) testSuite);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<ServerTestSuite>(testSuite, nameof (testSuite), this.m_tmRequestContext.RequestContext.ServiceName);
        return testSuite.Update((TestManagementRequestContext) this.m_tmRequestContext, projectName, TestSuiteSource.Mtm, false);
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
    public UpdatedProperties AddSuiteToSuite(
      IdAndRev parent,
      ServerTestSuite suite,
      string teamProjectName,
      out UpdatedProperties newSuiteProperties,
      int toIndex)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddSuiteToSuite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (parent), (object) parent);
        methodInformation.AddParameter(nameof (suite), (object) suite);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        this.EnterMethod(methodInformation);
        UpdatedProperties updateProperties = this.CreateUpdateProperties(parent, this.m_tmRequestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<ServerTestSuite>(suite, nameof (suite), this.m_tmRequestContext.RequestContext.ServiceName);
        newSuiteProperties = suite.Create((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, ref updateProperties, toIndex, TestSuiteSource.Mtm);
        return updateProperties;
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

    private UpdatedProperties CreateUpdateProperties(IdAndRev idAndRev, string expectedServiceName)
    {
      ArgumentUtility.CheckForNull<IdAndRev>(idAndRev, nameof (idAndRev), expectedServiceName);
      return new UpdatedProperties()
      {
        Revision = idAndRev.Revision,
        Id = idAndRev.Id
      };
    }

    [WebMethod]
    public UpdatedProperties DeleteTestSuiteEntries(
      IdAndRev parentSuiteId,
      List<TestSuiteEntry> entries,
      string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteTestSuiteEntries), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (parentSuiteId), (object) parentSuiteId);
        methodInformation.AddArrayParameter<TestSuiteEntry>(nameof (entries), (IList<TestSuiteEntry>) entries);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<IdAndRev>(parentSuiteId, nameof (parentSuiteId), this.m_tmRequestContext.RequestContext.ServiceName);
        return ServerTestSuite.DeleteEntries(this.m_tmRequestContext, parentSuiteId, entries, projectName);
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
    public UpdatedProperties MoveTestSuiteEntries(
      string teamProjectName,
      IdAndRev fromSuiteId,
      List<TestSuiteEntry> entries,
      IdAndRev toSuiteId,
      out UpdatedProperties newToProps,
      int toIndex)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (MoveTestSuiteEntries), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (fromSuiteId), (object) fromSuiteId);
        methodInformation.AddArrayParameter<TestSuiteEntry>(nameof (entries), (IList<TestSuiteEntry>) entries);
        methodInformation.AddParameter(nameof (toSuiteId), (object) toSuiteId);
        methodInformation.AddParameter(nameof (toIndex), (object) toIndex);
        this.EnterMethod(methodInformation);
        UpdatedProperties updateProperties = this.CreateUpdateProperties(fromSuiteId, this.m_tmRequestContext.RequestContext.ServiceName);
        newToProps = this.CreateUpdateProperties(toSuiteId, this.m_tmRequestContext.RequestContext.ServiceName);
        ServerTestSuite.MoveEntries((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, ref updateProperties, entries, ref newToProps, toIndex);
        return updateProperties;
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
    public void RepopulateTestSuite(string teamProjectName, int suiteId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RepopulateTestSuite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddParameter(nameof (suiteId), (object) suiteId);
        this.EnterMethod(methodInformation);
        ServerTestSuite.Repopulate((TestManagementRequestContext) this.m_tmRequestContext, teamProjectName, suiteId, TestSuiteSource.Mtm);
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
    public UpdatedProperties SetSuiteEntryConfigurations(
      string projectName,
      IdAndRev suite,
      TestCaseAndOwner[] testCases,
      int[] configIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetSuiteEntryConfigurations), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (suite), (object) suite);
        methodInformation.AddArrayParameter<TestCaseAndOwner>(nameof (testCases), (IList<TestCaseAndOwner>) testCases);
        methodInformation.AddArrayParameter<int>(nameof (configIds), (IList<int>) configIds);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<IdAndRev>(suite, nameof (suite), this.m_tmRequestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<TestCaseAndOwner[]>(testCases, nameof (testCases), this.m_tmRequestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<int[]>(configIds, nameof (configIds), this.m_tmRequestContext.RequestContext.ServiceName);
        return ServerTestSuite.SetSuiteEntryConfigurations((TestManagementRequestContext) this.m_tmRequestContext, projectName, suite, testCases, (IEnumerable<int>) configIds);
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
    public List<StateTypeEnumAndStateString> GetTestSuiteDefaultStatesMap()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetTestSuiteDefaultStatesMap), MethodType.Normal, EstimatedMethodCost.Low));
        return new TestSuiteWorkItem().GetDefaultStatesMap();
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
  }
}
