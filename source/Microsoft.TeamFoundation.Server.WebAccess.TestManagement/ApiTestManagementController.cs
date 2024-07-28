// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ApiTestManagementController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Admin;
using Microsoft.TeamFoundation.Server.WebAccess.Mail;
using Microsoft.TeamFoundation.Server.WebAccess.Requirements.Utilities;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.Utils;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
  public class ApiTestManagementController : TestManagementAreaController
  {
    private TestPlansHelper m_testPlanHelper;
    private TestSuitesHelper m_testSuitesHelper;
    private TestCasesHelper m_testCasesHelper;
    private TeamHelper m_teamHelper;
    private QueryHelper m_queryHelper;
    private SqmHelper m_sqmHelper;
    private const string AssignTesterMailBodyHtml = "<div> <p> {0} <br/><br/> {1} <a href='{2}' rel='nofollow noopener noreferrer' target='_blank'>{3}</a> <br/><br/></ div >";
    private const string ThankYouNoteHtml = "<div> {0} <br/> {1}";
    private const string ClickTwiceNoteHtml = "<div><br/><i>{0} <a href='{1}' rel='nofollow noopener noreferrer' target='_blank'>{2}</a>{3}</i></div>";
    private const string clickTwiceNoteFwdLink = "https://go.microsoft.com/fwlink/?LinkId=524814";
    private const string viewTestsPublicUrl = "{0}{1}/_testManagement? planId = {2}&suiteId={3}&_a=filterByTester";

    internal TestPlansHelper TestPlanHelper
    {
      get
      {
        if (this.m_testPlanHelper == null)
          this.m_testPlanHelper = new TestPlansHelper(this.TestContext);
        return this.m_testPlanHelper;
      }
    }

    internal TestSuitesHelper TestSuitesHelper
    {
      get
      {
        if (this.m_testSuitesHelper == null)
          this.m_testSuitesHelper = new TestSuitesHelper(this.TestContext);
        return this.m_testSuitesHelper;
      }
    }

    internal TestCasesHelper TestCasesHelper
    {
      get
      {
        if (this.m_testCasesHelper == null)
          this.m_testCasesHelper = new TestCasesHelper(this.TestContext);
        return this.m_testCasesHelper;
      }
    }

    internal TeamHelper TeamHelper
    {
      get
      {
        if (this.m_teamHelper == null)
          this.m_teamHelper = new TeamHelper(this.TestContext);
        return this.m_teamHelper;
      }
    }

    internal QueryHelper QueryHelper
    {
      get
      {
        if (this.m_queryHelper == null)
          this.m_queryHelper = new QueryHelper(this.TestContext);
        return this.m_queryHelper;
      }
    }

    internal SqmHelper SqmHelper
    {
      get
      {
        if (this.m_sqmHelper == null)
          this.m_sqmHelper = new SqmHelper(this.TestContext);
        return this.m_sqmHelper;
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525000, 525010)]
    public ActionResult Index() => (ActionResult) this.View();

    [AcceptVerbs(HttpVerbs.Post)]
    [ValidateInput(false)]
    public ActionResult CreateTestPlan(
      [ModelBinder(typeof (JsonModelBinder))] TestPlanCreationRequestModel planCreateRequestModel)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult testPlan = new DataContractJsonResult((object) this.TestPlanHelper.Create(planCreateRequestModel));
      testPlan.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testPlan;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestPlans()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult testPlans = new DataContractJsonResult((object) this.TestPlanHelper.GetAllTestPlans());
      testPlans.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testPlans;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult FetchTestPlansIncludingSpecifiedPlan(int planIdToSelect)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestPlanHelper.FetchTestPlansIncludingSpecifiedPlan(planIdToSelect));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [ValidateInput(false)]
    public ActionResult SaveAndFetchTestPlansByQueryFilter(string planQuery)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestPlanHelper.SaveAndFetchTestPlansByQueryFilter(planQuery));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [ValidateInput(false)]
    public ActionResult GetConvertedFilteredTestPlanQueryFromRegistry(string defaultPlanQuery)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      SecureJsonResult queryFromRegistry = new SecureJsonResult();
      queryFromRegistry.Data = (object) this.TestPlanHelper.GetConvertedFilteredTestPlanQueryFromRegistry(defaultPlanQuery);
      queryFromRegistry.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) queryFromRegistry;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTeamSettings()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult teamSettings = new DataContractJsonResult((object) this.TestPlanHelper.GetTeamFilter());
      teamSettings.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) teamSettings;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTeamFieldForTestPlans(List<int> testPlanIds)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult fieldForTestPlans = new DataContractJsonResult((object) this.TestPlanHelper.GetTeamFieldForPlans(testPlanIds));
      fieldForTestPlans.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) fieldForTestPlans;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestConfigurationsDetail(List<int> configIds, int planId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult configurationsDetail = new DataContractJsonResult((object) this.TestPlanHelper.GetConfigurationDetails(configIds, planId));
      configurationsDetail.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) configurationsDetail;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestSuitesForPlan(int planId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      bool disableShowPointCount = this.TfsRequestContext.IsFeatureEnabled("WebAccess.TestManagement.PointCountFeature");
      DataContractJsonResult testSuitesForPlan = new DataContractJsonResult((object) this.TestSuitesHelper.GetTestSuitesForPlan(planId, disableShowPointCount));
      testSuitesForPlan.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testSuitesForPlan;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetXsltTemplate()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      string xsltFile;
      try
      {
        xsltFile = XsltTransformationHelper.GetXsltFile(this.TestContext);
      }
      catch (Exception ex)
      {
        JsObject data = new JsObject();
        data.Add("success", (object) false);
        data.Add("error", (object) ex.ToJson());
        return (ActionResult) this.Json((object) data);
      }
      JsObject data1 = new JsObject();
      data1.Add("xslFileText", (object) xsltFile);
      return (ActionResult) this.Json((object) data1, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestPlansById(int[] planIds)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult testPlansById = new DataContractJsonResult((object) this.TestPlanHelper.GetTestPlansById((IEnumerable<int>) planIds));
      testPlansById.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testPlansById;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult CreateTestPlanFromWorkItem(int workItemId)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult planFromWorkItem = new DataContractJsonResult((object) this.TestPlanHelper.CreateTestPlanFromWorkItem(workItemId));
      planFromWorkItem.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) planFromWorkItem;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult GetTestPointsForSuite(
      int testPlanId,
      int testSuiteId,
      bool repopulateSuite,
      [ModelBinder(typeof (JsonModelBinder))] TestPointGridDisplayColumn[] columns,
      int top = 2147483647,
      bool recursive = false,
      string outcomeFilter = null,
      string testerFilter = null,
      int? configurationFilter = null)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult testPointsForSuite = new DataContractJsonResult((object) this.TestPlanHelper.GetTestPointsForSuite(testPlanId, testSuiteId, repopulateSuite, columns, top, recursive, outcomeFilter, testerFilter, configurationFilter));
      testPointsForSuite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testPointsForSuite;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UpdateRegistrySettings(string outcome, string tester, int? configuration)
    {
      this.TestSuitesHelper.UpdateTheFilterValues(outcome, tester, configuration);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult GetWitTestsForKanbanBoard([ModelBinder(typeof (JsonModelBinder))] int[] userStoryIds)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult testsForKanbanBoard = userStoryIds != null && userStoryIds.Length >= 1 ? new DataContractJsonResult((object) this.TestPlanHelper.GetWitTestSuitesModelForRequirements(userStoryIds)) : throw new ArgumentException(nameof (userStoryIds));
      testsForKanbanBoard.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testsForKanbanBoard;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult AddWitTestCasesToRequirementSuite(
      int requirementId,
      int planId,
      int suiteId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      ArgumentUtility.CheckGreaterThanZero((float) requirementId, nameof (requirementId));
      DataContractJsonResult requirementSuite = new DataContractJsonResult((object) this.TestPlanHelper.GetWitTestSuiteModelForRequirement(requirementId, planId, suiteId));
      requirementSuite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) requirementSuite;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UpdateSqmPoint(TcmProperty property, int incrementBy)
    {
      this.SqmHelper.UpdateSqmPoint(property, incrementBy);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult DeleteTestSuite(
      int parentTestSuiteId,
      int parentSuiteRevision,
      int suiteIdToDelete)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      this.TestSuitesHelper.DeleteTestSuite(parentTestSuiteId, parentSuiteRevision, suiteIdToDelete);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [ValidateInput(false)]
    public ActionResult RenameTestSuite(int suiteId, int suiteRevision, string title)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestSuitesHelper.RenameTestSuite(suiteId, suiteRevision, title).Revision);
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [ValidateInput(false)]
    public ActionResult CreateStaticSuite(
      [ModelBinder(typeof (JsonModelBinder))] TestSuiteCreationRequestModel suiteCreationRequestModel)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult staticSuite = new DataContractJsonResult((object) this.TestSuitesHelper.CreateStaticSuite(suiteCreationRequestModel).Id);
      staticSuite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) staticSuite;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    public ActionResult MoveTestSuiteEntry(
      int planId,
      int fromSuiteId,
      int fromSuiteRevision,
      int toSuiteId,
      int toSuiteRevision,
      int[] suiteEntriesToMoveIds,
      bool isTestCaseEntry)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestSuitesHelper.MoveTestSuiteEntry(planId, fromSuiteId, fromSuiteRevision, toSuiteId, toSuiteRevision, suiteEntriesToMoveIds, isTestCaseEntry));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    public ActionResult MoveTestSuiteEntryAtPosition(
      int planId,
      int fromSuiteId,
      int fromSuiteRevision,
      int toSuiteId,
      int toSuiteRevision,
      int[] suiteEntriesToMoveIds,
      bool isTestCaseEntry,
      int position)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestSuitesHelper.MoveTestSuiteEntry(planId, fromSuiteId, fromSuiteRevision, toSuiteId, toSuiteRevision, suiteEntriesToMoveIds, isTestCaseEntry, position));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    public ActionResult CreateRequirementSuites(
      int parentSuiteId,
      int parentSuiteRevision,
      int[] requirementIds)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult requirementSuites = new DataContractJsonResult((object) this.TestSuitesHelper.CreateRequirementSuites(new IdAndRev(parentSuiteId, parentSuiteRevision), new List<int>((IEnumerable<int>) requirementIds)));
      requirementSuites.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) requirementSuites;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [ValidateInput(false)]
    public ActionResult UpdateQueryBasedSuite(int suiteId, int suiteRevision, string queryText)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestSuitesHelper.UpdateQueryBasedSuite(suiteId, suiteRevision, queryText).Revision);
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [ValidateInput(false)]
    public ActionResult CreateQueryBasedSuite(
      [ModelBinder(typeof (JsonModelBinder))] QueryBasedSuiteCreationRequestModel suiteCreationRequestModel)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult queryBasedSuite = new DataContractJsonResult((object) this.TestSuitesHelper.CreateQueryBasedSuite(suiteCreationRequestModel));
      queryBasedSuite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) queryBasedSuite;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTeamDaysOffForIteration(string iterationId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return (ActionResult) this.Json((object) this.TeamHelper.GetTeamDaysOff(iterationId), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult FetchTestPoints(
      int testPlanId,
      int[] testPointIds,
      [ModelBinder(typeof (JsonModelBinder))] TestPointGridDisplayColumn[] columns)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestPlanHelper.FetchTestPoints(testPlanId, testPointIds, columns).ToList<TestPointModel>());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525010, 525020)]
    public ActionResult DownloadTcmAttachment(Uri testResultAttachmentUri)
    {
      int attachmentId;
      ArtifactHelper.ParseAttachmentId(testResultAttachmentUri, out attachmentId);
      string attachmentName;
      Stream attachmentStream = new AttachmentsHelper((TestManagementRequestContext) this.TestContext.TestRequestContext).GetAttachmentStream(this.TestContext.ProjectName, attachmentId, out attachmentName);
      if (attachmentStream == null)
      {
        this.Trace(525011, TraceLevel.Warning, "DownloadTcmAttachment: Unable to find attachment for testResultAttachmentUri: {0}", (object) testResultAttachmentUri);
        this.Response.StatusCode = 404;
        return (ActionResult) null;
      }
      this.Trace(525012, TraceLevel.Info, "DownloadTcmAttachment: Downloading attachment for testResultAttachmentUri: {0}", (object) testResultAttachmentUri);
      this.Response.BufferOutput = false;
      return (ActionResult) this.File(attachmentStream, Microsoft.TeamFoundation.Framework.Server.MimeMapping.GetMimeMapping(attachmentName), attachmentName);
    }

    [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(525020, 525030)]
    public ActionResult ResolveArtifacts(string[] uris)
    {
      uris = uris ?? Array.Empty<string>();
      List<string> source = new List<string>((IEnumerable<string>) uris);
      List<JsObject> data = new List<JsObject>();
      AttachmentsHelper attachmentsHelper = new AttachmentsHelper((TestManagementRequestContext) this.TestContext.TestRequestContext);
      TfsTestManagementRequestContext context = new TfsTestManagementRequestContext(this.TfsRequestContext);
      this.Trace(525021, TraceLevel.Info, "Resolving artifact uris: {0}", (object) string.Join(", ", uris));
      foreach (Artifact artifact1 in ArtifactHandler.GetArtifacts((TestManagementRequestContext) context, uris, attachmentsHelper, this.TestContext.CurrentProjectGuid))
      {
        Artifact artifact = artifact1;
        if (artifact != null)
        {
          List<JsObject> jsObjectList = data;
          JsObject jsObject = new JsObject();
          jsObject.Add("uri", (object) artifact.Uri);
          jsObject.Add("title", (object) artifact.ArtifactTitle);
          jsObjectList.Add(jsObject);
          source.Remove(source.First<string>((Func<string, bool>) (uri => string.Equals(uri, artifact.Uri, StringComparison.OrdinalIgnoreCase))));
        }
      }
      if (source.Any<string>())
        data.AddRange(source.Select<string, JsObject>((Func<string, JsObject>) (uri =>
        {
          return new JsObject()
          {
            {
              nameof (uri),
              (object) uri
            },
            {
              "title",
              (object) uri
            }
          };
        })));
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult AddTestCaseToTestSuite(
      int testSuiteId,
      int testSuiteRevision,
      int testCaseId,
      int[] linkedRequirementsIds)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult testSuite = new DataContractJsonResult((object) this.TestSuitesHelper.AddTestCaseToTestSuite(testSuiteId, testSuiteRevision, testCaseId));
      testSuite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testSuite;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult AddTestCasesToTestSuite(
      int testSuiteId,
      int testSuiteRevision,
      int[] testCaseIds)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult testSuite = new DataContractJsonResult((object) this.TestSuitesHelper.AddTestCasesToTestSuite(testSuiteId, testSuiteRevision, testCaseIds));
      testSuite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testSuite;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult GetTestCaseIdsInTestSuite(int testSuiteId)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult caseIdsInTestSuite = new DataContractJsonResult((object) this.TestSuitesHelper.GetTestCaseIdsInSuite(testSuiteId));
      caseIdsInTestSuite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) caseIdsInTestSuite;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult GetTestCaseColumnsFromUserSettings()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult fromUserSettings = new DataContractJsonResult((object) this.TestPlanHelper.GetTestCaseColumnsForUser());
      fromUserSettings.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) fromUserSettings;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult GetTestSuitesData(int testSuitesId, bool includeChildSuite)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.Controller, "ApiTestManagement.GetTestSuitesData");
        if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
          return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
        this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        DataContractJsonResult testSuitesData = new DataContractJsonResult((object) this.TestSuitesHelper.GetSuiteHierarchy(testSuitesId, includeChildSuite));
        testSuitesData.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (ActionResult) testSuitesData;
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.Controller, "ApiTestManagement.GetTestSuitesData");
      }
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult BulkMarkTestPoints(
      int planId,
      int suiteId,
      int[] testPointIds,
      byte outcome,
      bool useTeamSettings = false)
    {
      this.TestPlanHelper.BulkMarkTestPoints(planId, suiteId, testPointIds, (TestOutcome) outcome, useTeamSettings);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult ResetTestPoints(int planId, int[] testPointIds)
    {
      this.TestPlanHelper.ResetTestPoints(planId, testPointIds);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult AssignTester(int planId, int[] testPointIds, string tester)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      this.TestPlanHelper.BulkAssignTester(planId, testPointIds, tester);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult AssignTestersToSuite(int suiteId, string[] testers)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      this.TestSuitesHelper.UpdateTestersAssignedToSuite(suiteId, testers);
      DataContractJsonResult suite = new DataContractJsonResult(new object());
      suite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) suite;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult AssignTestersToSuiteWithAad(
      int suiteId,
      string[] testers,
      string newUsersJson)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      MembershipModel membershipModel = new MembershipModel();
      IEnumerable<string> second = (IEnumerable<string>) new List<string>();
      IEnumerable<string> first = (IEnumerable<string>) new List<string>();
      if (testers != null && testers.Length != 0)
        first = (IEnumerable<string>) ((IEnumerable<string>) testers).ToList<string>();
      if (!string.IsNullOrEmpty(newUsersJson))
        second = IdentityManagementHelpers.ParseNewUsersJson((ITfsController) this, newUsersJson, membershipModel).Select<TeamFoundationIdentity, string>((Func<TeamFoundationIdentity, string>) (id => id.TeamFoundationId.ToString()));
      this.TestSuitesHelper.UpdateTestersAssignedToSuite(suiteId, first.Concat<string>(second).ToArray<string>());
      DataContractJsonResult suiteWithAad = new DataContractJsonResult(new object());
      suiteWithAad.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) suiteWithAad;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult GetTestersAssignedToSuite(int suiteId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      List<TesterModelV2> testersAssignedToSuite1 = this.TestSuitesHelper.GetTestersAssignedToSuite(suiteId);
      foreach (TesterModelV2 testerModelV2 in testersAssignedToSuite1)
      {
        IDirectoryEntity fromTfIdentifier = DirectoryDiscoveryServiceHelper.GetEntityFromTFIdentifier(this.TfsRequestContext, testerModelV2.Id.ToString());
        if (fromTfIdentifier != null)
          testerModelV2.EntityId = fromTfIdentifier.EntityId;
      }
      DataContractJsonResult testersAssignedToSuite2 = new DataContractJsonResult((object) testersAssignedToSuite1);
      testersAssignedToSuite2.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testersAssignedToSuite2;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult IsDataTierUpdated()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TeamHelper.IsDataTierUpdated());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    [ValidateInput(false)]
    public ActionResult FetchSuitesTestPointCountInPlan(
      int planId,
      string outcome,
      string tester,
      int? configuration)
    {
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestSuitesHelper.GeneratePointCountInfoForSuitesinPlan(planId, outcome, tester, configuration));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete)]
    [TfsTraceFilter(525040, 525050)]
    public ActionResult RemoveTestCasesFromSuite(
      int testSuiteId,
      int testSuiteRevision,
      int[] testCaseIds)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestSuitesHelper.RemoveTestCasesFromSuite(testSuiteId, testSuiteRevision, testCaseIds));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525040, 525050)]
    public ActionResult UpdateColumnOptions([ModelBinder(typeof (JsonModelBinder))] ColumnSettingModel[] columnOptions, bool removeExisting)
    {
      this.TestPlanHelper.UpdateColumnOptions(columnOptions, removeExisting);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525040, 525050)]
    public ActionResult UpdateColumnSortOrder([ModelBinder(typeof (JsonModelBinder))] ColumnSortOrderModel columnSortOrder)
    {
      this.TestPlanHelper.UpdateColumnSortOrder(columnSortOrder);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525010, 525020)]
    public ActionResult GetWorkItemTypeNamesForCategories(string[] categoryNames)
    {
      WebAccessWorkItemService service = this.TfsRequestContext.GetService<WebAccessWorkItemService>();
      IEnumerable<Project> projects = service.GetProjects(this.TfsRequestContext);
      List<string> source = new List<string>();
      foreach (Project project in projects)
      {
        IEnumerable<string> namesForCategories = service.GetWorkItemNamesForCategories(this.TfsRequestContext, project.Name, (IEnumerable<string>) ((IEnumerable<string>) categoryNames).ToList<string>());
        source.AddRange(namesForCategories);
      }
      DataContractJsonResult namesForCategories1 = new DataContractJsonResult((object) source.Distinct<string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToList<string>().ToArray());
      namesForCategories1.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) namesForCategories1;
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsTraceFilter(525010, 525020)]
    [ValidateInput(false)]
    public ActionResult ValidateQueryContainsCategory(string queryText, string categoryName)
    {
      this.QueryHelper.CheckTestCaseCategoryCondition(queryText, categoryName);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult AssignConfigurationsToSuite(int suiteId, int[] configurations)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      this.TestSuitesHelper.UpdateAssignedConfigurationsToSuite(suiteId, ((IEnumerable<int>) configurations).ToList<int>());
      DataContractJsonResult suite = new DataContractJsonResult(new object());
      suite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) suite;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult AssignConfigurationsToTestCases(
      [ModelBinder(typeof (JsonModelBinder))] TestCaseAndSuiteModel[] testCases,
      int[] configurations)
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      this.TestSuitesHelper.UpdateAssignedConfigurationsToTestCases(testCases, ((IEnumerable<int>) configurations).ToList<int>());
      DataContractJsonResult testCases1 = new DataContractJsonResult(new object());
      testCases1.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testCases1;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult GetAvailableConfigurationsForSuite(int suiteId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult configurationsForSuite = new DataContractJsonResult((object) this.TestSuitesHelper.GetTestConfigurationsForSuite(suiteId));
      configurationsForSuite.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) configurationsForSuite;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult GetAvailableConfigurationsForTestCases([ModelBinder(typeof (JsonModelBinder))] TestCaseAndSuiteModel[] testCases)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult configurationsForTestCases = new DataContractJsonResult((object) this.TestSuitesHelper.GetTestConfigurationsForTestCases(testCases));
      configurationsForTestCases.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) configurationsForTestCases;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult FetchTcmPlanIds(int[] witPlanIds)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestPlanHelper.GetTcmPlanIds((IEnumerable<int>) witPlanIds));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(525030, 525040)]
    public void SendMailAsync([ModelBinder(typeof (JsonModelBinder))] MailMessage message, int planId, int suiteId)
    {
      string str = string.Empty + this.ConstructHtmlContent(PublicUrlHelper.GetViewTestsLink(this.TestContext, planId, suiteId)) + EmailUtility.ConstructNotesText(message.Body) + this.ConstructThankYouNote();
      message.Body = str;
      MailSender.BeginSendMail(message, this.TfsRequestContext, this.Request.RequestContext.TfsWebContext().IsHosted, this.AsyncManager);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult SendMailCompleted() => (ActionResult) this.Json((object) MailSender.SendMailCompleted(this.AsyncManager));

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(525030, 525040)]
    public void SendExportToHtmlMailAsync(
      [ModelBinder(typeof (JsonModelBinder))] MailMessage message,
      int planId,
      int suiteId,
      string parametersXml = "")
    {
      if (planId <= 0)
        throw new ArgumentException(TestManagementResources.InvalidParameterPlanId);
      if (suiteId <= 0)
        throw new ArgumentException(TestManagementResources.InvalidParameterSuiteId);
      string notes = EmailUtility.ConstructNotesText(message.Body);
      HtmlDocumentGenerator.ScheduleSendMailJob(this.TestContext, planId, suiteId, parametersXml, notes, message);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525030, 525040)]
    public ActionResult SendExportToHtmlMailCompleted() => (ActionResult) this.Json((object) MailSender.SendMailCompleted(this.AsyncManager));

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult QueryTestPlanAssociatedTestArtifacts(int testPlanId)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.Controller, "ApiTestManagement.QueryTestPlanAssociatedTestArtifacts");
        this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestPlanHelper.QueryTestPlanAssociatedTestArtifacts(testPlanId));
        contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (ActionResult) contractJsonResult;
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.Controller, "ApiTestManagement.QueryTestPlanAssociatedTestArtifacts");
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult QueryTestSuiteAssociatedTestArtifacts(int testSuiteId)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.Controller, "ApiTestManagement.QueryTestSuiteAssociatedTestArtifacts");
        this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestSuitesHelper.QueryTestSuiteAssociatedTestArtifacts(testSuiteId));
        contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (ActionResult) contractJsonResult;
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.Controller, "ApiTestManagement.QueryTestSuiteAssociatedTestArtifacts");
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult QueryTestCaseAssociatedTestArtifacts(int testCaseId)
    {
      try
      {
        this.TestContext.TraceEnter(TfsTraceLayers.Controller, "ApiTestManagement.QueryTestCaseAssociatedTestArtifacts");
        this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestCasesHelper.QueryTestCaseAssociatedTestArtifacts(testCaseId));
        contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (ActionResult) contractJsonResult;
      }
      finally
      {
        this.TestContext.TraceLeave(TfsTraceLayers.Controller, "ApiTestManagement.QueryTestCaseAssociatedTestArtifacts");
      }
    }

    private string ConstructThankYouNote() => string.Format("<div> {0} <br/> {1}", (object) TestManagementServerResources.AssignTestersToSuiteEmailTextThanks, (object) this.TfsWebContext.User.Name) + string.Format("<div><br/><i>{0} <a href='{1}' rel='nofollow noopener noreferrer' target='_blank'>{2}</a>{3}</i></div>", (object) TestManagementServerResources.AssignTestersToSuiteHintMessageForRedirectPart1, (object) "https://go.microsoft.com/fwlink/?LinkId=524814", (object) TestManagementServerResources.AssignTestersToSuiteHintMessageForRedirectPart2, (object) TestManagementServerResources.AssignTestersToSuiteHintMessageForRedirectPart3);

    private string ConstructHtmlContent(string viewTestsUrl) => string.Empty + string.Format("<div> <p> {0} <br/><br/> {1} <a href='{2}' rel='nofollow noopener noreferrer' target='_blank'>{3}</a> <br/><br/></ div >", (object) TestManagementServerResources.AssignTestersToSuiteEmailTextHi, (object) TestManagementServerResources.AssignTestersToSuiteEmailTextContent, (object) viewTestsUrl, (object) TestManagementServerResources.AssignTestersToSuiteEmailTextViewTest);
  }
}
