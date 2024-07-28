// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MTMCompatResultsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "MTMCompatResults", ResourceVersion = 1)]
  public class MTMCompatResultsController : TfsApiController
  {
    private Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper m_resultsHelper;
    private AttachmentUploadHelper m_attachmentUploadHelper;
    private TestManagementRequestContext m_testManagementRequestContext;
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPropertyException),
        HttpStatusCode.Conflict
      },
      {
        typeof (Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectInUseException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TeamProjectNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ProjectDoesNotExistWithNameException),
        HttpStatusCode.NotFound
      }
    };

    [HttpPost]
    [ClientLocationId("459DC66B-20FC-4B7D-A7EA-88CF57D07616")]
    public void CreateTestResultsLegacy(CreateTestResultsRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().CreateTestResults(this.TestManagementRequestContext, request.ProjectName, request.Results);
    }

    [HttpPost]
    [ClientLocationId("1E0D28B8-ED74-4D97-83AF-2E3CD024A8BF")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun CreateTestRunLegacy(
      CreateTestRunRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      NewProjectStepsPerformer.InitializeNewProject(this.TestManagementRequestContext, request.ProjectName);
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().CreateTestRun(this.TestManagementRequestContext, request.ProjectName, request.TestRun, request.Results, request.TestSettings);
    }

    [HttpPost]
    [ClientLocationId("2D8BEF90-CF8F-44B2-8CFF-148925458C16")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] UpdateTestResultsLegacy(
      BulkResultUpdateRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().Update(this.TestManagementRequestContext, request.Requests, request.ProjectName);
    }

    [HttpPost]
    [ClientLocationId("188287B2-7878-4F28-98BD-D53DD8351D19")]
    public TestResultAcrossProjectResponse GetTestResultsAcrossProjects(
      LegacyTestCaseResultIdentifier identifier)
    {
      if (identifier == null)
        throw new ArgumentNullException(nameof (identifier));
      string projectName;
      LegacyTestCaseResult multipleProjects = this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().GetTestResultInMultipleProjects(this.TestManagementRequestContext, identifier.TestRunId, identifier.TestResultId, out projectName);
      return new TestResultAcrossProjectResponse()
      {
        TestResult = multipleProjects,
        ProjectName = projectName
      };
    }

    [HttpPost]
    [ClientLocationId("C3801E01-633B-42E0-B045-7D6577AA1DA8")]
    public ResultsByQueryResponse GetTestResultsByQueryLegacy(ResultsByQueryRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      List<LegacyTestCaseResultIdentifier> excessIds;
      List<LegacyTestCaseResult> testResultsByQuery = this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().GetTestResultsByQuery(this.TestManagementRequestContext, request.Query, request.PageSize, out excessIds);
      return new ResultsByQueryResponse()
      {
        TestResults = testResultsByQuery,
        ExcessIds = excessIds
      };
    }

    [HttpPost]
    [ClientLocationId("414C81B9-E7CA-4814-869B-216F18CBF510")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun GetTestRunByTmiRunId(
      Guid tmiRunId)
    {
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryTestRunByTmiRunId(this.TestManagementRequestContext, tmiRunId);
    }

    [HttpPost]
    [ClientLocationId("5CC14C9A-6782-4773-98F2-94846AA10C47")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> QueryTestRunsLegacy(
      QueryTestRunsRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().Query(this.TestManagementRequestContext, request.TestRunId, request.Owner, request.BuildUri, request.TeamProjectName, request.PlanId, request.Skip, request.Top);
    }

    [HttpPost]
    [ClientLocationId("466760DB-95DD-413E-9030-BC109CC16C86")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> QueryTestRunsAcrossMultipleProjects(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      if (query == null)
        throw new ArgumentNullException(nameof (query));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryTestRunsInMultipleProjects(this.TestManagementRequestContext, query);
    }

    [HttpPost]
    [ClientLocationId("9D897C0C-48A5-430B-A96A-6B033EE823A4")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> QueryTestRuns2(
      QueryTestRuns2Request request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().Query(this.TestManagementRequestContext, request.Query, request.IncludeStatistics);
    }

    [HttpPost]
    [ClientLocationId("2DAF90F7-FC98-4ACC-881C-EABAF2244AC5")]
    public FetchTestResultsResponse FetchTestResults(FetchTestResultsRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      List<LegacyTestCaseResultIdentifier> webApiDeletedIds;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments;
      List<LegacyTestCaseResult> legacyTestCaseResultList = this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().Fetch(this.TestManagementRequestContext, request.IdAndRevs, request.ProjectName, request.IncludeActionResults, out webApiDeletedIds, out webApiActionResults, out webApiParams, out webApiAttachments);
      return new FetchTestResultsResponse()
      {
        TestParameters = webApiParams,
        Results = legacyTestCaseResultList,
        Attachments = webApiAttachments,
        ActionResults = webApiActionResults,
        DeletedIds = webApiDeletedIds
      };
    }

    [HttpPost]
    [ClientLocationId("BC2B04D6-441B-46D7-B3CD-3DB127FCFC46")]
    public FetchTestResultsResponse QueryByRun(QueryByRunRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      List<LegacyTestCaseResultIdentifier> webApiExcessIds;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments;
      List<LegacyTestCaseResult> legacyTestCaseResultList = this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryByRun(this.TestManagementRequestContext, request.TestRunId, request.PageSize, out webApiExcessIds, request.ProjectName, request.IncludeActionResults, out webApiActionResults, out webApiParams, out webApiAttachments);
      return new FetchTestResultsResponse()
      {
        TestParameters = webApiParams,
        Results = legacyTestCaseResultList,
        Attachments = webApiAttachments,
        ActionResults = webApiActionResults,
        DeletedIds = webApiExcessIds
      };
    }

    [HttpPost]
    [ClientLocationId("980FE82F-BD10-4D3B-AD46-00B679F1B628")]
    public FetchTestResultsResponse QueryByRunAndOwner(QueryByRunRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      List<LegacyTestCaseResultIdentifier> excessIds;
      List<LegacyTestCaseResult> legacyTestCaseResultList = this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryByRunAndOwner(this.TestManagementRequestContext, request.TestRunId, request.Owner, request.PageSize, out excessIds, request.ProjectName);
      return new FetchTestResultsResponse()
      {
        Results = legacyTestCaseResultList,
        DeletedIds = excessIds
      };
    }

    [HttpPost]
    [ClientLocationId("6516DEF2-10DC-49C3-8557-1001AF5BA0D0")]
    public FetchTestResultsResponse QueryByRunAndState(QueryByRunRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      List<LegacyTestCaseResultIdentifier> excessIds;
      List<LegacyTestCaseResult> legacyTestCaseResultList = this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryByRunAndState(this.TestManagementRequestContext, request.TestRunId, request.State, request.PageSize, out excessIds, request.ProjectName);
      return new FetchTestResultsResponse()
      {
        Results = legacyTestCaseResultList,
        DeletedIds = excessIds
      };
    }

    [HttpPost]
    [ClientLocationId("3F690505-1D33-470C-A309-D279C015B7E8")]
    public FetchTestResultsResponse QueryByRunAndOutcome(QueryByRunRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      List<LegacyTestCaseResultIdentifier> excessIds;
      List<LegacyTestCaseResult> legacyTestCaseResultList = this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryByRunAndOutcome(this.TestManagementRequestContext, request.TestRunId, request.Outcome, request.PageSize, out excessIds, request.ProjectName);
      return new FetchTestResultsResponse()
      {
        Results = legacyTestCaseResultList,
        DeletedIds = excessIds
      };
    }

    [HttpPost]
    [ClientLocationId("D8EB6AB5-0457-4EC0-8BF9-A2BE683188B3")]
    public QueryTestActionResultResponse QueryTestActionResults(QueryTestActionResultRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      QueryTestActionResultResponse actionResultResponse = this.TestManagementRequestContext.RequestContext.GetService<ITestActionResultService>().QueryTestActionResults(this.TestManagementRequestContext, request.ProjectName, request.Identifier);
      if (actionResultResponse == null)
        return actionResultResponse;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> testAttachments = actionResultResponse.TestAttachments;
      if (testAttachments == null)
        return actionResultResponse;
      testAttachments.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) (attachment => attachment.PopulateUrlField(request.ProjectName, "tcm")));
      return actionResultResponse;
    }

    [HttpPost]
    [ClientLocationId("2FE7C059-CB0D-4239-A37B-5CF10E57C2B4")]
    public int QueryTestRunsCount(Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      if (query == null)
        throw new ArgumentNullException(nameof (query));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryTestRunsCount(this.TestManagementRequestContext, query);
    }

    [HttpPost]
    [ClientLocationId("6AFB5D3C-3A6F-4598-B6CC-FCF187DD3E6E")]
    public UpdateTestRunResponse UpdateTestRunLegacy(UpdateTestRunRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      ITestResultsService service = this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>();
      int[] attachmentIds;
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties updatedProperties;
      if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.LogStoreAttachmentTableForCreateAttachment") && this.TestManagementRequestContext.RequestContext.ServiceHost.DeploymentServiceHost.IsHosted)
      {
        this.TestManagementRequestContext.RequestContext.TraceVerbose("RestLayer", "MTMCompatResultsController.UpdateTestRunLegacy -> UpdateTestRunForLogStoreAttachments. projectName = {0}, runId = {1}", (object) request.ProjectName, (object) request.TestRun?.TestRunId);
        updatedProperties = service.UpdateTestRunForLogStoreAttachments(this.TestManagementRequestContext, request.ProjectName, request.TestRun, request.AttachmentsToAdd, request.AttachmentsToDelete, out attachmentIds, request.ShouldHyderate);
      }
      else
      {
        this.TestManagementRequestContext.RequestContext.TraceVerbose("RestLayer", "MTMCompatResultsController.UpdateTestRunLegacy -> UpdateTestRun. projectName = {0}, runId = {1}", (object) request.ProjectName, (object) request.TestRun?.TestRunId);
        updatedProperties = service.UpdateTestRun(this.TestManagementRequestContext, request.ProjectName, request.TestRun, request.AttachmentsToAdd, request.AttachmentsToDelete, out attachmentIds, request.ShouldHyderate);
      }
      return new UpdateTestRunResponse()
      {
        UpdatedProperties = updatedProperties,
        AttachmentIds = attachmentIds
      };
    }

    [HttpPost]
    [ClientLocationId("535F2F8B-67A7-4783-B304-71855008B15E")]
    public LegacyTestCaseResult[] ResetTestResults(ResetTestResultsRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().ResetTestResults(this.TestManagementRequestContext, request.Ids, request.ProjectName);
    }

    [HttpPost]
    [ClientLocationId("04A5F40B-A3AA-4C3A-82C6-89174AD0E235")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties AbortTestRun(
      AbortTestRunRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().AbortTestRun(this.TestManagementRequestContext, request.ProjectName, request.TestRunId, request.Revision, request.Options);
    }

    [HttpPost]
    [ClientLocationId("9B343F31-9E27-4D5F-9450-99341AA62954")]
    public List<LegacyTestRunStatistic> QueryTestRunStats(QueryTestRunStatsRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryTestRunStats(this.TestManagementRequestContext, request.TeamProjectName, request.TestRunId);
    }

    [HttpPost]
    [ClientLocationId("EF28B894-F109-453B-BED9-A5A90C58264C")]
    public List<LegacyTestCaseResult> QueryByPoint(QueryByPointRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryByPoint(this.TestManagementRequestContext, request.ProjectName, request.TestPlanId, request.TestPointId);
    }

    [HttpPost]
    [ClientLocationId("8959FE42-AE7F-4F24-AA13-A6FD356D9FCA")]
    public void DeleteTestRunIds(DeleteTestRunRequest deleteTestRunRequest)
    {
      if (deleteTestRunRequest == null)
        throw new ArgumentNullException(nameof (deleteTestRunRequest));
      this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().DeleteTestRun(this.TestManagementRequestContext, deleteTestRunRequest.ProjectName, deleteTestRunRequest.TestRunIds);
    }

    [HttpPost]
    [ClientLocationId("2BCBB8A7-1496-464C-9A64-72CD191C35B1")]
    public List<int> CreateTestMessageLogEntries(CreateTestMessageLogEntryRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().CreateLogEntriesForRun(this.TestManagementRequestContext, request.ProjectName, request.TestRunId, request.TestMessageLogEntry);
    }

    [HttpPost]
    [ClientLocationId("1B5D25B4-E83C-4400-B8DA-C3C89F14CDDE")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry> QueryTestMessageLogEntry(
      QueryTestMessageLogEntryRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().QueryLogEntriesForRun(this.TestManagementRequestContext, request.ProjectName, request.TestRunId, request.TestMessageLogId);
    }

    [HttpPost]
    [ClientLocationId("D1ADD0DD-79C5-40B1-B8BF-CA37C79BA764")]
    public void UploadAttachmentsLegacy(UploadAttachmentsRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.LogStoreAttachmentTableForCreateAttachment") && this.TestManagementRequestContext.RequestContext.ServiceHost.DeploymentServiceHost.IsHosted)
      {
        this.TestManagementRequestContext.RequestContext.TraceVerbose("RestLayer", "MTMCompatResultsController.UploadAttachmentsLegacy -> ProcessUploadInLogStore.");
        this.AttachmentUploadHelper.ProcessUploadInLogStore(this.TestManagementRequestContext, request.RequestParams, request.Attachments);
      }
      else
      {
        this.TestManagementRequestContext.RequestContext.TraceVerbose("RestLayer", "MTMCompatResultsController.UploadAttachmentsLegacy -> ProcessUpload.");
        this.AttachmentUploadHelper.ProcessUpload(this.TestManagementRequestContext, request.RequestParams, request.Attachments);
      }
    }

    [HttpPost]
    [ClientLocationId("F16EE7C3-199B-419F-A8BA-D4A76661E5B3")]
    [ClientResponseType(typeof (Stream), "DownloadAttachmentsLegacyZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "DownloadAttachmentsLegacyContent", "application/octet-stream")]
    public HttpResponseMessage DownloadAttachmentsLegacy(DownloadAttachmentsRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      List<(int attachmentId, Guid projectId)> attachmentProjectMap;
      (Stream contentStream, string contentType, string fileName, long contentLength) tuple = this.TestManagementRequestContext.RequestContext.GetService<ITestResultsService>().DownloadAttachments(this.TestManagementRequestContext, request, out attachmentProjectMap);
      Stream contentStream = tuple.contentStream;
      if (contentStream == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      string fileName = tuple.fileName ?? string.Empty;
      long contentLength = tuple.contentLength;
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult securedObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = attachmentProjectMap[0].projectId.ToString()
        }
      };
      response.Content = (HttpContent) new VssServerStreamContent(contentStream, (object) securedObject);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(tuple.contentType);
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(fileName);
      response.Content.Headers.ContentLength = new long?(contentLength);
      return response;
    }

    private static string GetContentDispositionFileName(HttpResponse response)
    {
      string dispositionFileName = (string) null;
      if (response.Headers["Content-Disposition"] != null)
      {
        string header = response.Headers["Content-Disposition"];
        int num1 = header.IndexOf("filename=", StringComparison.OrdinalIgnoreCase);
        if (num1 > -1 && header.Length > num1 + 9)
        {
          int startIndex = num1 + 9;
          int num2 = header.IndexOf(';', startIndex);
          int length = num2 <= -1 ? header.Length - startIndex : num2 - startIndex;
          dispositionFileName = header.Substring(startIndex, length).Trim('"');
        }
      }
      return dispositionFileName;
    }

    internal Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper ResultsHelper
    {
      get
      {
        if (this.m_resultsHelper == null)
          this.m_resultsHelper = new Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsHelper(this.TestManagementRequestContext);
        return this.m_resultsHelper;
      }
    }

    internal AttachmentUploadHelper AttachmentUploadHelper
    {
      get
      {
        if (this.m_attachmentUploadHelper == null)
          this.m_attachmentUploadHelper = new AttachmentUploadHelper();
        return this.m_attachmentUploadHelper;
      }
    }

    public override string ActivityLogArea => "Test Results";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) MTMCompatResultsController.s_httpExceptions;

    protected TestManagementRequestContext TestManagementRequestContext
    {
      get
      {
        if (this.m_testManagementRequestContext == null)
          this.m_testManagementRequestContext = new TestManagementRequestContext(this.TfsRequestContext);
        return this.m_testManagementRequestContext;
      }
    }
  }
}
