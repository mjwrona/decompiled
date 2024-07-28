// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TestLogController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "testlog", ResourceVersion = 1)]
  [FeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService")]
  public class TestLogController : TcmControllerBase
  {
    private const string c_continuationTokenHeaderName = "x-ms-continuationtoken";
    private const string c_continuationTokenParamName = "continuationToken";
    private const string c_continuationTokenDescription = "Header to pass the continuationToken";

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ActionName("testlog")]
    [ClientResponseType(typeof (IPagedList<TestLog>), null, null)]
    [ClientLocationId("DFF8CE3A-E539-4817-A405-D968491A88F1")]
    [ClientHeaderParameter("x-ms-continuationtoken", typeof (string), "continuationToken", "Header to pass the continuationToken", true, false)]
    public async Task<HttpResponseMessage> GetTestLogsForBuildAsync(
      int buildId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool fetchMetaData = false,
      int top = 1000)
    {
      TestLogReference logReference = new TestLogReference()
      {
        BuildId = buildId,
        Scope = TestLogScope.Build,
        Type = type
      };
      return await this.GetTestLogsAsync(this.GetTestLogQueryParams(type, directoryPath, fileNamePrefix, fetchMetaData), logReference, top);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ActionName("runs")]
    [ClientResponseType(typeof (IPagedList<TestLog>), null, null)]
    [ClientLocationId("5B47B946-E875-4C9A-ACDC-2A20996CAEBE")]
    [ClientHeaderParameter("x-ms-continuationtoken", typeof (string), "continuationToken", "Header to pass the continuationToken", true, false)]
    [ClientExample("Get_testlog_run.json", "Get test logs for run", null, null)]
    public async Task<HttpResponseMessage> GetTestRunLogsAsync(
      int runId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool fetchMetaData = false,
      int top = 1000)
    {
      TestLogReference logReference = new TestLogReference()
      {
        RunId = runId,
        Scope = TestLogScope.Run,
        Type = type
      };
      return await this.GetTestLogsAsync(this.GetTestLogQueryParams(type, directoryPath, fileNamePrefix, fetchMetaData), logReference, top);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ActionName("results")]
    [ClientResponseType(typeof (IPagedList<TestLog>), null, null)]
    [ClientLocationId("714CAAAC-AE1E-4869-8323-9BC0F5120DBF")]
    [ClientHeaderParameter("x-ms-continuationtoken", typeof (string), "continuationToken", "Header to pass the continuationToken", true, false)]
    [ClientExample("Get_testlog_result.json", "Get test logs for result", null, null)]
    public async Task<HttpResponseMessage> GetTestResultLogsAsync(
      int runId,
      int resultId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool fetchMetaData = false,
      int top = 1000)
    {
      TestLogReference logReference = new TestLogReference()
      {
        RunId = runId,
        ResultId = resultId,
        Scope = TestLogScope.Run,
        Type = type
      };
      return await this.GetTestLogsAsync(this.GetTestLogQueryParams(type, directoryPath, fileNamePrefix, fetchMetaData), logReference, top);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ActionName("results")]
    [ClientResponseType(typeof (IPagedList<TestLog>), null, null)]
    [ClientLocationId("714CAAAC-AE1E-4869-8323-9BC0F5120DBF")]
    [ClientHeaderParameter("x-ms-continuationtoken", typeof (string), "continuationToken", "Header to pass the continuationToken", true, false)]
    [ClientExample("Get_testlog_subresult.json", "Get test logs for subresult", null, null)]
    public async Task<HttpResponseMessage> GetTestSubResultLogsAsync(
      int runId,
      int resultId,
      int subResultId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool fetchMetaData = false,
      int top = 1000)
    {
      TestLogReference logReference = new TestLogReference()
      {
        RunId = runId,
        ResultId = resultId,
        SubResultId = subResultId,
        Scope = TestLogScope.Run,
        Type = type
      };
      return await this.GetTestLogsAsync(this.GetTestLogQueryParams(type, directoryPath, fileNamePrefix, fetchMetaData), logReference, top);
    }

    private async Task<HttpResponseMessage> GetTestLogsAsync(
      TestLogQueryParameters testLogQueryParameters,
      TestLogReference logReference,
      int top)
    {
      TestLogController tfsApiController = this;
      // ISSUE: explicit non-virtual call
      PagedList<TestLogSecureObject> pagedTestLogSecureObjectResultList = await tfsApiController.TfsRequestContext.GetService<ITestLogStoreService>().GetTestLogs(new TestManagementRequestContext(tfsApiController.TfsRequestContext), __nonvirtual (tfsApiController.ProjectInfo), testLogQueryParameters, logReference, new int?(top)).ConfigureAwait(false);
      PagedList<TestLog> fromSecureObject = tfsApiController.GetTestLogFromSecureObject(pagedTestLogSecureObjectResultList);
      // ISSUE: explicit non-virtual call
      HttpResponseMessage response = tfsApiController.GenerateResponse<TestLog>((IEnumerable<TestLog>) fromSecureObject, (ISecuredObject) new TestLogStoreSecuredObject(__nonvirtual (tfsApiController.ProjectInfo).Id));
      if (fromSecureObject != null && !string.IsNullOrEmpty(fromSecureObject.ContinuationToken))
        Utils.SetContinuationToken(response, fromSecureObject.ContinuationToken);
      return response;
    }

    private TestLogQueryParameters GetTestLogQueryParams(
      TestLogType type,
      string directoryPath,
      string fileNamePrefix,
      bool fetchMetaData)
    {
      string headerValue = this.GetHeaderValue(this.Request?.Headers, "x-ms-continuationtoken");
      return new TestLogQueryParameters()
      {
        Type = type,
        DirectoryPath = directoryPath,
        FileNamePrefix = fileNamePrefix,
        FetchMetaData = fetchMetaData,
        ContinuationToken = headerValue
      };
    }

    private PagedList<TestLog> GetTestLogFromSecureObject(
      PagedList<TestLogSecureObject> pagedTestLogSecureObjectResultList)
    {
      IList<TestLog> list = (IList<TestLog>) new List<TestLog>();
      foreach (TestLogSecureObject secureObjectResult in (List<TestLogSecureObject>) pagedTestLogSecureObjectResultList)
        list.Add(secureObjectResult.TestLog);
      return new PagedList<TestLog>((IEnumerable<TestLog>) list, pagedTestLogSecureObjectResultList.ContinuationToken);
    }
  }
}
