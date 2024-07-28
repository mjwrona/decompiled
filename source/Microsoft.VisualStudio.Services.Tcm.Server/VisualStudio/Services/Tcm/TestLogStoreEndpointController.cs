// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TestLogStoreEndpointController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "testlogstoreendpoint", ResourceVersion = 1)]
  [FeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService")]
  public class TestLogStoreEndpointController : TcmControllerBase
  {
    [HttpPost]
    [ClientLocationId("39B09BE7-F0C9-4A83-A513-9AE31B45C56F")]
    public async Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForBuild(
      int buildId,
      TestLogStoreOperationType testLogStoreOperationType)
    {
      TestLogReference logReference = new TestLogReference()
      {
        Scope = TestLogScope.Build,
        BuildId = buildId
      };
      return await this.GetTestLogStoreEndpointDetails(TestLogStoreEndpointType.Root, testLogStoreOperationType, logReference, true).ConfigureAwait(false);
    }

    [HttpPost]
    [ClientLocationId("67EB3F92-6C97-4FD9-8B63-6CBDC7E526EA")]
    [ClientExample("Post_testlogstore_run_endpoint.json", "Post file endpoint details", null, null)]
    public async Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForRun(
      int runId,
      TestLogStoreOperationType testLogStoreOperationType,
      string filePath = null,
      TestLogType type = TestLogType.GeneralAttachment)
    {
      TestLogReference logReference = new TestLogReference()
      {
        Scope = TestLogScope.Run,
        RunId = runId,
        FilePath = filePath,
        Type = type
      };
      return filePath != null ? await this.GetTestLogStoreEndpointDetails(TestLogStoreEndpointType.File, TestLogStoreOperationType.ReadAndCreate, logReference, true).ConfigureAwait(false) : await this.GetTestLogStoreEndpointDetails(TestLogStoreEndpointType.Root, testLogStoreOperationType, logReference, true).ConfigureAwait(false);
    }

    [HttpPost]
    [ClientLocationId("DA630B37-1236-45B5-945E-1D7BDB673850")]
    [ClientExample("Post_testlogstore_result_endpoint.json", "Post file endpoint details for result", null, null)]
    public async Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForResult(
      int runId,
      int resultId,
      int subResultId,
      string filePath,
      TestLogType type)
    {
      return await this.GetTestLogStoreEndpointDetails(TestLogStoreEndpointType.File, TestLogStoreOperationType.ReadAndCreate, new TestLogReference()
      {
        Scope = TestLogScope.Run,
        RunId = runId,
        ResultId = resultId,
        SubResultId = subResultId,
        FilePath = filePath,
        Type = type
      }, true).ConfigureAwait(false);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("39B09BE7-F0C9-4A83-A513-9AE31B45C56F")]
    public async Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForBuildLog(
      int build,
      TestLogType type,
      string filePath)
    {
      return await this.GetTestLogStoreEndpointDetails(TestLogStoreEndpointType.File, TestLogStoreOperationType.Read, new TestLogReference()
      {
        Scope = TestLogScope.Build,
        BuildId = build,
        Type = type,
        FilePath = filePath
      }, true).ConfigureAwait(false);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("67EB3F92-6C97-4FD9-8B63-6CBDC7E526EA")]
    [ClientExample("Get_testlogstore_runs_endpoint.json", "Get file endpoint details", null, null)]
    public async Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForRunLog(
      int runId,
      TestLogType type,
      string filePath)
    {
      return await this.GetTestLogStoreEndpointDetails(TestLogStoreEndpointType.File, TestLogStoreOperationType.Read, new TestLogReference()
      {
        Scope = TestLogScope.Run,
        RunId = runId,
        Type = type,
        FilePath = filePath
      }, true).ConfigureAwait(false);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("DA630B37-1236-45B5-945E-1D7BDB673850")]
    [ClientExample("Get_testlogstore_results_endpoint.json", "Get file endpoint details", null, null)]
    public async Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForResultLog(
      int runId,
      int resultId,
      TestLogType type,
      string filePath)
    {
      return await this.GetTestLogStoreEndpointDetails(TestLogStoreEndpointType.File, TestLogStoreOperationType.Read, new TestLogReference()
      {
        Scope = TestLogScope.Run,
        RunId = runId,
        ResultId = resultId,
        Type = type,
        FilePath = filePath
      }, true).ConfigureAwait(false);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("DA630B37-1236-45B5-945E-1D7BDB673850")]
    [ClientExample("Get_testlogstore_subresults_endpoint.json", "Get file endpoint details", null, null)]
    public async Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForSubResultLog(
      int runId,
      int resultId,
      int subResultId,
      TestLogType type,
      string filePath)
    {
      return await this.GetTestLogStoreEndpointDetails(TestLogStoreEndpointType.File, TestLogStoreOperationType.Read, new TestLogReference()
      {
        Scope = TestLogScope.Run,
        RunId = runId,
        ResultId = resultId,
        SubResultId = subResultId,
        Type = type,
        FilePath = filePath
      }, true).ConfigureAwait(false);
    }

    private async Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetails(
      TestLogStoreEndpointType testLogStoreEndpointType,
      TestLogStoreOperationType testLogStoreOperationType,
      TestLogReference logReference,
      bool isExternalCustomer)
    {
      TestLogStoreEndpointController endpointController = this;
      TestManagementRequestContext tcmRequestContext = new TestManagementRequestContext(endpointController.TfsRequestContext);
      // ISSUE: explicit non-virtual call
      return (await endpointController.TfsRequestContext.GetService<ITestLogStoreService>().GetTestLogStoreEndpointDetails(tcmRequestContext, __nonvirtual (endpointController.ProjectInfo), testLogStoreOperationType, testLogStoreEndpointType, logReference, isExternalCustomer).ConfigureAwait(false)).TestLogStoreEndpointDetails;
    }
  }
}
