// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITestLogStoreService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TestLogStoreService))]
  public interface ITestLogStoreService : IVssFrameworkService
  {
    Task<TestLogStoreEndpointDetailsSecureObject> GetTestLogStoreEndpointDetails(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogStoreOperationType testLogStoreOperationType,
      TestLogStoreEndpointType testLogStoreEndpointScope,
      TestLogReference logReference,
      bool isExternalCustomer = false);

    bool RevokeSASPolicy(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      string policyName,
      ContainerScopeDetails containerScopeDetails);

    Task<PagedList<TestLogSecureObject>> GetTestLogs(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference,
      int? top);

    List<TestLog> GetAllTestLogs(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference,
      bool isFile = false);

    List<AttachmentTestLog> GetAllAttachmentTestLogs(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogQueryParameters logQueryParameters,
      AttachmentTestLogReference logReference);

    Task<List<TestLog>> GetAllTestLogsAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference);

    TestLogStatusCode DeleteTestLogStoreContainer(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      ContainerScopeDetails containerScopeDetails);

    Task DownloadToStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      Stream targetStream);

    void DownloadToStream(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      Stream targetStream);

    Task DownloadRangeToStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      int offset,
      int length,
      Stream targetStream);

    int DeleteTestLogStoreContainers(
      TestManagementRequestContext tcmRequestContext,
      List<TestLogContainer> containerToDelete);

    int DeleteTestLogStoreContainers(
      TestManagementRequestContext tcmRequestContext,
      List<TestLogContainer> containerToDelete,
      ITestLogStorageConnection connectionEndPoint);

    List<TestLogContainer> GetStorageContainers(
      TestManagementRequestContext tcmRequestContext,
      ILogStoreConnectionEndpoint logStoreConnectionEndpoint,
      ref string token,
      int maxResults = 5000,
      int maxBlobCalls = 10,
      int OlderByNoOfDays = 30,
      Guid? collectionHostId = null);

    Task<IList<TestTag>> GetTestTagsForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference);

    Task<TestLogStatusCode> CreateTestTagForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      string tag);

    Task<TestLogStatusCode> DeleteTestTagForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      string tag);

    Task<IList<TestTagSecureObject>> GetUniqueTestTagsForBuildOrRelease(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference);

    Task<TestTagSummarySecureObject> GetTestTagDetailForBuildOrRelease(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference);

    Task<bool> CreateEmptyBlobForAttachmentTestLogAsync(
      TestManagementRequestContext tcmRequestContextm,
      Guid projectId,
      AttachmentTestLogReference attachmentTestLogReference,
      Dictionary<string, string> metaData);

    bool CreateEmptyBlobForAttachmentTestLog(
      TestManagementRequestContext tcmRequestContextm,
      Guid projectId,
      AttachmentTestLogReference attachmentTestLogReference,
      Dictionary<string, string> metaData);

    bool CreateBlob(
      TestManagementRequestContext tcmRequestContextm,
      Guid projectId,
      TestLogReference attachmentTestLogReference,
      Stream stream,
      Dictionary<string, string> metaData,
      bool overwrite = false);

    TestLogStatusWithFileName CreateBlobWithFallbackName(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference attachmentTestLogReference,
      Stream stream,
      Dictionary<string, string> metaData);

    bool DeleteBlob(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference);

    Task<bool> CreateBlobAsync(
      TestManagementRequestContext tcmRequestContextm,
      Guid projectId,
      TestLogReference attachmentTestLogReference,
      Stream stream,
      Dictionary<string, string> metaData,
      bool overwrite = false);

    Task<string> GetSasUriForContentScanAsync(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference);
  }
}
