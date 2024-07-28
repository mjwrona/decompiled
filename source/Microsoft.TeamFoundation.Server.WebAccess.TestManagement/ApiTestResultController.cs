// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ApiTestResultController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [ValidateInput(false)]
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
  public class ApiTestResultController : TestManagementAreaController
  {
    private TestResultHelper m_testResultHelper;

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525000, 525010)]
    public ActionResult Index() => (ActionResult) this.View();

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525000, 525010)]
    public ActionResult Update([ModelBinder(typeof (JsonModelBinder))] ResultUpdateRequestModel[] updateRequests)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      ArgumentUtility.CheckForNull<ResultUpdateRequestModel[]>(updateRequests, nameof (updateRequests));
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.TestResultHelper.Update(updateRequests));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525000, 525010)]
    public ActionResult GetTestCaseResults(int testRunId, int[] testCaseResultIds)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult testCaseResults = new DataContractJsonResult((object) this.TestResultHelper.GetTestCaseResults(testRunId, testCaseResultIds));
      testCaseResults.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testCaseResults;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestResultAttachments(int testRunId, int testResultId)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult resultAttachments = new DataContractJsonResult((object) TestResultHelper.GetAttachments(this.TestContext, testRunId, testResultId));
      resultAttachments.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) resultAttachments;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestCaseResultForTestCaseId(int testCaseId)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult resultForTestCaseId = new DataContractJsonResult((object) this.TestResultHelper.GetTestCaseResultForTestCaseId(testCaseId));
      resultForTestCaseId.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) resultForTestCaseId;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestResolutionStates()
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      DataContractJsonResult resolutionStates = new DataContractJsonResult((object) this.TestResultHelper.GetTestResolutionStates());
      resolutionStates.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) resolutionStates;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTestFailureTypes()
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      DataContractJsonResult testFailureTypes = new DataContractJsonResult((object) this.TestResultHelper.GetTestFailureTypes());
      testFailureTypes.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) testFailureTypes;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(525020, 525030)]
    public ActionResult AssociateWorkItems(
      [ModelBinder(typeof (JsonModelBinder))] TestCaseResultIdentifierModel[] testResultIdentifiers,
      string[] workItemUris)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.TestResultHelper.AssociateWorkItems(testResultIdentifiers, workItemUris, this.TestContext.ProjectName);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(525000, 525010)]
    public ActionResult UploadAttachment(
      string callback,
      int testRunId,
      int testResultId,
      int iterationId,
      string actionPath = null)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      object obj;
      try
      {
        this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
        CommonUtility.CheckCallbackName(callback);
        obj = (object) new
        {
          success = true,
          attachments = this.TestResultHelper.CreateAndUploadTestResultAttachments(this.Request.Files, testRunId, testResultId, iterationId, actionPath)
        };
      }
      catch (Exception ex)
      {
        obj = (object) new
        {
          success = false,
          error = ex.ToJson()
        };
      }
      if (string.IsNullOrEmpty(callback))
        return (ActionResult) new EmptyResult();
      string str;
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
        str = "var uploadResult = window.parent[" + JsonConvert.SerializeObject((object) callback) + "];if(uploadResult && uploadResult.isCallback === true && typeof(uploadResult.callback) === 'function'){ uploadResult.callback(" + JsonConvert.SerializeObject(obj) + "); }";
      else
        str = "var uploadResult = window.parent[" + new JavaScriptSerializer().Serialize((object) callback) + "];if(uploadResult && uploadResult.isCallback === true && typeof(uploadResult.callback) === 'function'){ uploadResult.callback(" + new JavaScriptSerializer().Serialize(obj) + "); }";
      return (ActionResult) this.Content("<html><body><script type='text/javascript'>" + str + "</script><body></html>", "text/html");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525010, 525020)]
    public ActionResult DownloadAttachment(int attachmentId)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      string attachmentName;
      Stream attachmentStream = new AttachmentsHelper((TestManagementRequestContext) this.TestContext.TestRequestContext).GetAttachmentStream(this.TestContext.ProjectName, attachmentId, out attachmentName);
      if (attachmentStream == null)
      {
        this.Response.StatusCode = 404;
        return (ActionResult) null;
      }
      this.Response.BufferOutput = false;
      return (ActionResult) this.File(attachmentStream, Microsoft.TeamFoundation.Framework.Server.MimeMapping.GetMimeMapping(attachmentName), attachmentName);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525010, 525020)]
    public ActionResult DownloadTestIterationAttachment(
      int attachmentId,
      int runId,
      int resultId,
      int iterationId)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      AttachmentsHelper attachmentsHelper = new AttachmentsHelper((TestManagementRequestContext) this.TestContext.TestRequestContext);
      string str;
      Stream fileStream = !this.TestContext.TfsRequestContext.IsFeatureEnabled("TestManagement.Server.EnableDownloadAttachmentByIterationId") ? attachmentsHelper.GetAttachmentStream(this.TestContext.ProjectName, attachmentId, out str) : attachmentsHelper.GetTestIterationAttachment(this.TestContext.ProjectName, runId, resultId, attachmentId, out str, out CompressionType _, iterationId);
      if (fileStream == null)
      {
        this.Response.StatusCode = 404;
        return (ActionResult) null;
      }
      this.Response.BufferOutput = false;
      return (ActionResult) this.File(fileStream, Microsoft.TeamFoundation.Framework.Server.MimeMapping.GetMimeMapping(str), str);
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete)]
    [TfsTraceFilter(525010, 525020)]
    public ActionResult DeleteAttachment(int attachmentId, int testRunId, int testResultId)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.TestResultHelper.DeleteAttachment(attachmentId, testRunId, testResultId);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult(new object());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    internal TestResultHelper TestResultHelper
    {
      get
      {
        if (this.m_testResultHelper == null)
          this.m_testResultHelper = new TestResultHelper(this.TestContext);
        return this.m_testResultHelper;
      }
    }
  }
}
