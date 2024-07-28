// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogClientService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestLogClientService : 
    TeamFoundationTestManagementService,
    ITestLogClientService,
    IVssFrameworkService
  {
    public async Task<TestLogStatus> UploadTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      Stream stream,
      Dictionary<string, string> metaData,
      bool allowDuplicate,
      bool overwrite = false)
    {
      ITestLogClientStore testLogBlobStore;
      TestLogStatus andCheckLogStatus = this._getBlobStoreAndCheckLogStatus(tcmRequestContext, projectId, testLogReference, out testLogBlobStore);
      if (andCheckLogStatus != null)
        return andCheckLogStatus;
      string blobReferenceName = this._getBlobReferenceName(testLogReference);
      ITestLogSingleTransferContext testLogSingleTransferContext = (ITestLogSingleTransferContext) new TestLogSingleTransferContext(tcmRequestContext.RequestContext.ActivityId.ToString(), overwrite);
      ITestLogUploadOptions testLogUploadOptions = (ITestLogUploadOptions) new TestLogUploadOptions();
      return await testLogBlobStore.UploadAsync(tcmRequestContext.RequestContext, stream, blobReferenceName, allowDuplicate, metaData, testLogSingleTransferContext, testLogUploadOptions, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
    }

    public async Task<TestLogStatus> UploadTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      string logFileSourcePath,
      Dictionary<string, string> metaData,
      bool allowDuplicate)
    {
      ITestLogClientStore testLogBlobStore;
      TestLogStatus andCheckLogStatus = this._getBlobStoreAndCheckLogStatus(tcmRequestContext, projectId, testLogReference, out testLogBlobStore);
      if (andCheckLogStatus != null)
        return andCheckLogStatus;
      string blobReferenceName = this._getBlobReferenceName(testLogReference);
      ITestLogSingleTransferContext testLogSingleTransferContext = (ITestLogSingleTransferContext) new TestLogSingleTransferContext(tcmRequestContext.RequestContext.ActivityId.ToString());
      ITestLogUploadOptions testLogUploadOptions = (ITestLogUploadOptions) new TestLogUploadOptions();
      return await testLogBlobStore.UploadAsync(tcmRequestContext.RequestContext, logFileSourcePath, blobReferenceName, allowDuplicate, metaData, testLogSingleTransferContext, testLogUploadOptions, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
    }

    public async Task<TestLogStatus> DownloadTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      Stream stream)
    {
      TestLogStatus testLogStatus = new TestLogStatus();
      testLogStatus.Status = TestLogStatusCode.Success;
      if (!tcmRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService"))
      {
        testLogStatus.Status = TestLogStatusCode.FeatureDisabled;
        return testLogStatus;
      }
      ITestLogClientStore testBlob;
      TestLogStatus testLogBlobStore = this._getTestLogBlobStore(tcmRequestContext, projectId, testLogReference, out testBlob);
      if (testLogBlobStore.Status != TestLogStatusCode.Success)
        return testLogBlobStore;
      string blobReferenceName = this._getBlobReferenceName(testLogReference);
      ITestLogSingleTransferContext testLogSingleTransferContext = (ITestLogSingleTransferContext) new TestLogSingleTransferContext(tcmRequestContext.RequestContext.ActivityId.ToString());
      ITestLogDownloadOptions testLogDownloadOptions = (ITestLogDownloadOptions) new TestLogDownloadOptions(this.isEnabledContentMD5ValidationForDownload(tcmRequestContext));
      return await testBlob.DownloadAsync(tcmRequestContext.RequestContext, blobReferenceName, stream, testLogSingleTransferContext, testLogDownloadOptions, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
    }

    public async Task<PagedList<TestLog>> QueryTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference,
      int top = 1000)
    {
      if (!tcmRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService"))
        return new PagedList<TestLog>((IEnumerable<TestLog>) new List<TestLog>(), (string) null);
      ProjectInfo projectFromGuid = tcmRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectId);
      return this.GetTestLogFromSecureObject(await tcmRequestContext.RequestContext.GetService<ITestLogStoreService>().GetTestLogs(tcmRequestContext, projectFromGuid, logQueryParameters, logReference, new int?(top)).ConfigureAwait(false));
    }

    public async Task<LogStoreContainerCopyResponse> CopyTestLogStoreContainersAsync(
      TestManagementRequestContext tcmRequestContext,
      ITestLogStorageConnection sourceConnectionEndpoint,
      ITestLogStorageConnection targetConnectionEndpoint,
      List<string> containerNames,
      bool isServiceCopy,
      int maxDegreeParallelism = 8,
      int retryCount = 5)
    {
      tcmRequestContext.RequestContext.Trace(1015662, TraceLevel.Info, "TestManagement", "LogStorage", "Executing Action: TestLogClientService.CopyTestLogStoreContainersAsync");
      ITestLogClientMigrationStore clientMigrationStore = this._getTestLogClientMigrationStore(tcmRequestContext, sourceConnectionEndpoint.GetLogStoreConnectionEndpoint(), targetConnectionEndpoint.GetLogStoreConnectionEndpoint());
      LogStoreContainerCopyResponse containerCopyResponse = new LogStoreContainerCopyResponse();
      try
      {
        containerCopyResponse = await clientMigrationStore.CopyContainersInParallelAsync(tcmRequestContext.RequestContext, containerNames, isServiceCopy, maxDegreeParallelism, retryCount, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        containerCopyResponse.Status = LogStoreContainerCopyStatus.Failed;
        containerCopyResponse.StatusReason = ex.Message;
        tcmRequestContext.RequestContext.Trace(1015662, TraceLevel.Warning, "TestManagement", "LogStorage", "CopyTestLogStoreContainers -  failed with exception " + ex.Message);
      }
      LogStoreContainerCopyResponse containerCopyResponse1 = containerCopyResponse;
      containerCopyResponse = (LogStoreContainerCopyResponse) null;
      return containerCopyResponse1;
    }

    private ITestLogClientMigrationStore _getTestLogClientMigrationStore(
      TestManagementRequestContext tcmRequestContext,
      ILogStoreConnectionEndpoint sourceTestLogStorageConnectionEndpoint,
      ILogStoreConnectionEndpoint targetTestLogStorageConnectionEndpoint)
    {
      tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Info, "TestManagement", "LogStorage", "Executing Action: TestLogClientService._getTestLogClientMigrationStore");
      return tcmRequestContext.RequestContext.GetExtension<ITestLogClientProvider>().GetTestLogClientMigrationStore(tcmRequestContext.RequestContext, sourceTestLogStorageConnectionEndpoint, targetTestLogStorageConnectionEndpoint);
    }

    private PagedList<TestLog> GetTestLogFromSecureObject(
      PagedList<TestLogSecureObject> pagedTestLogSecureObjectResultList)
    {
      IList<TestLog> list = (IList<TestLog>) new List<TestLog>();
      foreach (TestLogSecureObject secureObjectResult in (List<TestLogSecureObject>) pagedTestLogSecureObjectResultList)
        list.Add(secureObjectResult.TestLog);
      return new PagedList<TestLog>((IEnumerable<TestLog>) list, pagedTestLogSecureObjectResultList.ContinuationToken);
    }

    private TestLogStatus _getBlobStoreAndCheckLogStatus(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      out ITestLogClientStore testLogBlobStore)
    {
      testLogBlobStore = (ITestLogClientStore) null;
      TestLogStatus andCheckLogStatus = new TestLogStatus();
      andCheckLogStatus.Status = TestLogStatusCode.Success;
      if (!tcmRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService"))
      {
        andCheckLogStatus.Status = TestLogStatusCode.FeatureDisabled;
        return andCheckLogStatus;
      }
      TestLogStatus testLogBlobStore1 = this._getTestLogBlobStore(tcmRequestContext, projectId, testLogReference, out testLogBlobStore);
      return testLogBlobStore1.Status != TestLogStatusCode.Success ? testLogBlobStore1 : (TestLogStatus) null;
    }

    private TestLogStatus _getContainerScopeAndValidate(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      out ContainerScopeDetails containerScopeDetails)
    {
      TestLogStatus scopeAndValidate = new TestLogStatus();
      scopeAndValidate.Status = TestLogStatusCode.Success;
      ProjectInfo projectFromGuid = tcmRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectId);
      containerScopeDetails = (ContainerScopeDetails) null;
      try
      {
        containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, testLogReference);
        LogStoreHelper.ValidateContainerDetails(tcmRequestContext, containerScopeDetails, projectFromGuid, true);
      }
      catch (TestObjectNotFoundException ex)
      {
        scopeAndValidate.Status = testLogReference.Scope == TestLogScope.Build ? TestLogStatusCode.BuildDoesNotExist : TestLogStatusCode.RunDoesNotExist;
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "_getContainerScopeAndValidate -  Build or Run not found, Exception Hit: {0}", (object) ex.Message);
        return scopeAndValidate;
      }
      catch (TestManagementInvalidOperationException ex)
      {
        scopeAndValidate.Status = TestLogStatusCode.FeatureDisabled;
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "_getContainerScopeAndValidate -  Feature flag disable for build definition id, ExceptionHit: {0}", (object) ex.Message);
        return scopeAndValidate;
      }
      return scopeAndValidate;
    }

    private string _getBlobReferenceName(TestLogReference testLogReference)
    {
      LogStorePathFormatter storePathFormatter = new LogStorePathFormatter();
      string destFilePath = storePathFormatter.SanitizeFilePath(testLogReference.FilePath);
      return storePathFormatter.GetBlobReferenceName(testLogReference, testLogReference.Type, destFilePath, false);
    }

    private TestLogStatus _getTestLogBlobStore(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      out ITestLogClientStore testBlob)
    {
      new TestLogStatus().Status = TestLogStatusCode.Success;
      testBlob = (ITestLogClientStore) null;
      ContainerScopeDetails containerScopeDetails;
      TestLogStatus scopeAndValidate = this._getContainerScopeAndValidate(tcmRequestContext, projectId, testLogReference, out containerScopeDetails);
      if (scopeAndValidate.Status != TestLogStatusCode.Success)
        return scopeAndValidate;
      try
      {
        ITestLogStorageConnection storageConnection = tcmRequestContext.RequestContext.GetService<ITeamFoundationTestLogStoreService>().GetTestLogStorageConnection(tcmRequestContext.RequestContext, projectId, containerScopeDetails);
        string containerName = LogStoreHelper.GetContainerName(tcmRequestContext.RequestContext, projectId, containerScopeDetails);
        ITestLogClientProvider extension = tcmRequestContext.RequestContext.GetExtension<ITestLogClientProvider>();
        testBlob = extension.GetTestLogClientStore(tcmRequestContext.RequestContext, storageConnection.GetLogStoreConnectionEndpoint(), containerName);
      }
      catch (TestObjectNotFoundException ex)
      {
        scopeAndValidate.Status = TestLogStatusCode.ContainerNotCreated;
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "_initializeContainerReference - Container doesn't exist. Exception Hit: {0}", (object) ex.Message);
      }
      catch (Exception ex)
      {
        scopeAndValidate.Status = TestLogStatusCode.Failed;
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "_initializeContainerReference - Unable to create the blob client. Exception Hit: {0}", (object) ex.Message);
      }
      return scopeAndValidate;
    }

    private bool isEnabledContentMD5ValidationForDownload(
      TestManagementRequestContext tcmRequestContext)
    {
      return tcmRequestContext.RequestContext.GetService<IVssRegistryService>().GetValue<bool>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreEnableContentMD5Validation", false);
    }
  }
}
