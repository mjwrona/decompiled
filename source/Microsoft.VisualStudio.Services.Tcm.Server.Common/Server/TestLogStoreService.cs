// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogStoreService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestLogStoreService : 
    TeamFoundationTestManagementService,
    ITestLogStoreService,
    IVssFrameworkService
  {
    private ILogStorePathFormatter _logStorePathFormatter;

    public async Task<PagedList<TestLogSecureObject>> GetTestLogs(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference,
      int? top)
    {
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      this._validateQueryParametersAndCheckAccessPermission(tcmRequestContext, projectInfo, logQueryParameters, logReference, top, containerScopeDetails);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails, false);
      if (azureBlobStore == null)
        return new PagedList<TestLogSecureObject>((IEnumerable<TestLogSecureObject>) this.ConvertToTestLogSecureObject(projectInfo.Id, (IList<TestLog>) new List<TestLog>()), string.Empty);
      ILogStoreBlobResultSegment logStoreBlobResultSegment = await azureBlobStore.ListBlobsAsync(tcmRequestContext.RequestContext, this.GetFilePathPrefix(logQueryParameters, logReference), true, (ILogStoreBlobListingDetails) new LogStoreBlobListingDetails(logQueryParameters.FetchMetaData), top, (ILogStoreBlobContinuationToken) new LogStoreBlobContinuationToken(logQueryParameters.ContinuationToken), this._getLogStoreOperationContext(tcmRequestContext), tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
      return this.GetTestLogSecureObectsFromBlobResultSegment(projectInfo, logStoreBlobResultSegment, logReference);
    }

    public async Task<TestLogStoreEndpointDetailsSecureObject> GetTestLogStoreEndpointDetails(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogStoreOperationType testLogStoreOperationType,
      TestLogStoreEndpointType testLogStoreEndpointScope,
      TestLogReference logReference,
      bool isExternalCustomer = false)
    {
      tcmRequestContext.RequestContext.TraceInfo("RestLayer", "TestLogStoreService.GetTestLogStoreEndpointDetails projectId = {0}, scope = {1}, buildId = {2}, releaseId = {3}, runId = {4}", (object) projectInfo.Id, (object) testLogStoreEndpointScope, (object) logReference.BuildId, (object) logReference.ReleaseId, (object) logReference.RunId);
      TestLogStoreEndpointDetails storeEndpointDetails = new TestLogStoreEndpointDetails();
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      int sasValidityInHours = tcmRequestContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreSASValidityInHours", 1);
      IVssRequestContext vssRequestContext = tcmRequestContext.RequestContext.To(TeamFoundationHostType.Deployment);
      string hostSuffix = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreAFDHostSuffix", "vstmrblob.vsassets.io");
      bool isAFDEnabledForLogStore = tcmRequestContext.IsFeatureEnabled("TestManagement.Server.EnableAFDForTestLogStore");
      SASPermission sasPermission;
      TestLogStoreEndpointDetailsSecureObject endpointSecureObject;
      if (!this._checkPermissionsAndValidateRequest(tcmRequestContext, projectInfo, testLogStoreOperationType, testLogStoreEndpointScope, logReference, containerScopeDetails, out sasPermission, out endpointSecureObject))
        return endpointSecureObject;
      (bool, TestLogStoreEndpointDetailsSecureObject) sasUriAsync = await this._tryCreateSasUriAsync(tcmRequestContext, projectInfo, testLogStoreOperationType, testLogStoreEndpointScope, logReference, containerScopeDetails, sasPermission, sasValidityInHours, isExternalCustomer);
      if (!sasUriAsync.Item1)
        return sasUriAsync.Item2;
      endpointSecureObject = sasUriAsync.Item2;
      if (isAFDEnabledForLogStore)
      {
        TestLogStoreAFDService logStoreAfdService = new TestLogStoreAFDService(hostSuffix);
        endpointSecureObject.TestLogStoreEndpointDetails.EndpointSASUri = logStoreAfdService.GetAFDEndpoint(tcmRequestContext, endpointSecureObject.TestLogStoreEndpointDetails.EndpointSASUri, logReference.RunId, logReference.BuildId, logReference.FilePath);
      }
      return endpointSecureObject;
    }

    public async Task<string> GetSasUriForContentScanAsync(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference)
    {
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails);
      ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
      ITestLogStorageConnection storageConnection = tcmRequestContext.RequestContext.GetService<ITeamFoundationTestLogStoreService>().GetTestLogStorageConnection(tcmRequestContext.RequestContext, projectInfo.Id, containerScopeDetails);
      SASPermission sasPermission = SASPermission.ReadList_Policy;
      int sasValidityInHours = 336;
      return await this._getSASUri(tcmRequestContext, TestLogStoreEndpointType.File, logReference, azureBlobStore, operationContext, sasPermission, sasValidityInHours, storageConnection.SupportsHttps(), false).ConfigureAwait(false);
    }

    public bool RevokeSASPolicy(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      string policyName,
      ContainerScopeDetails containerScopeDetails)
    {
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectId, containerScopeDetails, false);
      if (azureBlobStore != null)
        return azureBlobStore.RevokeSharedAccessPolicyForContainer(tcmRequestContext.RequestContext, policyName, this._getLogStoreOperationContext(tcmRequestContext), (IContainerAccessCondition) new ContainerAccessCondition());
      tcmRequestContext.TraceInfo("LogStorage", "No LogStore connection found.");
      return false;
    }

    public TestLogStatusCode DeleteTestLogStoreContainer(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      ContainerScopeDetails containerScopeDetails)
    {
      return this.DeleteTestLogStoreContainerInternal(tcmRequestContext, projectId, containerScopeDetails);
    }

    private TestLogStatusCode DeleteTestLogStoreContainerInternal(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      ContainerScopeDetails containerScopeDetails,
      ITestLogStorageConnection connectionEndPoint = null)
    {
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectId, containerScopeDetails, false, connectionEndPoint);
      if (azureBlobStore == null)
        return TestLogStatusCode.ContainerNotFound;
      ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
      IContainerAccessCondition containerAccessCondition = (IContainerAccessCondition) new ContainerAccessCondition();
      try
      {
        bool flag = azureBlobStore.DeleteContainer(tcmRequestContext.RequestContext, containerAccessCondition, operationContext);
        if (flag)
          this.TryDeletingShardingMapping(tcmRequestContext, projectId, containerScopeDetails);
        tcmRequestContext.RequestContext.Trace(1015677, TraceLevel.Warning, "TestManagement", "LogStorage", "DeleteTestLogStoreContainer - DeleteContainer status:" + flag.ToString());
        return flag ? TestLogStatusCode.Success : TestLogStatusCode.ContainerNotFound;
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "DeleteTestLogStoreContainer - DeleteContainerIfExists failed, ExceptionHit: {0} ", (object) ex.Message);
        return TestLogStatusCode.Failed;
      }
    }

    public int DeleteTestLogStoreContainers(
      TestManagementRequestContext tcmRequestContext,
      List<TestLogContainer> containerToDelete)
    {
      return this.DeleteTestLogStoreContainers(tcmRequestContext, containerToDelete, (ITestLogStorageConnection) null);
    }

    public int DeleteTestLogStoreContainers(
      TestManagementRequestContext tcmRequestContext,
      List<TestLogContainer> containerToDelete,
      ITestLogStorageConnection connectionEndPoint = null)
    {
      List<Tuple<int, ContainerScopeDetails>> deletedList = new List<Tuple<int, ContainerScopeDetails>>();
      int num1 = 0;
      int num2 = 0;
      if (containerToDelete != null && containerToDelete.Count > 0)
      {
        foreach (TestLogContainer testLogContainer in containerToDelete)
        {
          if (tcmRequestContext.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && testLogContainer != null && testLogContainer.HostId == tcmRequestContext.RequestContext.ServiceHost.InstanceId)
          {
            Guid projectId = this.GetProjectId(tcmRequestContext, testLogContainer.DataspaceId);
            switch (connectionEndPoint != null ? this.DeleteTestLogStoreContainerInternal(tcmRequestContext, projectId, testLogContainer.containerScopeDetails, connectionEndPoint) : this.DeleteTestLogStoreContainerInternal(tcmRequestContext, projectId, testLogContainer.containerScopeDetails))
            {
              case TestLogStatusCode.Success:
                ++num1;
                deletedList.Add(new Tuple<int, ContainerScopeDetails>(testLogContainer.DataspaceId, testLogContainer.containerScopeDetails));
                break;
              case TestLogStatusCode.ContainerNotFound:
                ++num2;
                tcmRequestContext.RequestContext.Trace(1015677, TraceLevel.Info, "TestManagement", "LogStorage", string.Format("Conainer RunId : {0} or Contianer BuildId : {1} or Container ReleaseId : {2}", (object) testLogContainer.containerScopeDetails.RunIdId, (object) testLogContainer.containerScopeDetails.BuildId, (object) testLogContainer.containerScopeDetails.ReleaseId));
                break;
            }
            if (deletedList.Count == 100)
            {
              this._logDeletedContainers(tcmRequestContext, deletedList);
              deletedList.Clear();
            }
          }
          else
            tcmRequestContext.RequestContext.Trace(1015677, TraceLevel.Warning, "TestManagement", "LogStorage", "DeleteTestLogStoreContainers - failed");
        }
        if (deletedList.Count > 0)
        {
          this._logDeletedContainers(tcmRequestContext, deletedList);
          deletedList.Clear();
        }
      }
      return num1;
    }

    public List<TestLogContainer> GetStorageContainers(
      TestManagementRequestContext tcmRequestContext,
      ILogStoreConnectionEndpoint logStoreConnectionEndpoint,
      ref string token,
      int maxResults = 5000,
      int maxBlobCalls = 10,
      int OlderByNoOfDays = 30,
      Guid? collectionHostId = null)
    {
      tcmRequestContext.RequestContext.Trace(1015662, TraceLevel.Info, "TestManagement", "LogStorage", "Entering GetStorageContainers");
      List<TestLogContainer> storageContainers = new List<TestLogContainer>();
      if (!collectionHostId.HasValue && tcmRequestContext.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        collectionHostId = new Guid?(tcmRequestContext.RequestContext.ServiceHost.InstanceId);
      if (collectionHostId.HasValue)
      {
        Guid? nullable = collectionHostId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
        {
          try
          {
            IVssRegistryService service = tcmRequestContext.RequestContext.GetService<IVssRegistryService>();
            int serverTimeoutInSeconds = service.GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreServerTimeoutInSeconds", 60);
            int maximumExecutionTimeInSeconds = service.GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreServerMaxExecutionTimeInSeconds", 900);
            IVssRequestContext requestContext1 = tcmRequestContext.RequestContext;
            RegistryQuery registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreMaxAttemptsRetry";
            ref RegistryQuery local1 = ref registryQuery;
            int maxAttempts = service.GetValue<int>(requestContext1, in local1, 10);
            IVssRequestContext requestContext2 = tcmRequestContext.RequestContext;
            registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreDeltaBackOffInSeconds";
            ref RegistryQuery local2 = ref registryQuery;
            int num1 = service.GetValue<int>(requestContext2, in local2, 3);
            LogStoreExponentialRetryPolicy logStoreExponentialRetryPolicy = new LogStoreExponentialRetryPolicy(tcmRequestContext.RequestContext, TimeSpan.FromSeconds((double) num1), maxAttempts);
            ILogStoreRequestOption storeRequestOption = (ILogStoreRequestOption) new LogStoreRequestOption(serverTimeoutInSeconds, maximumExecutionTimeInSeconds, logStoreExponentialRetryPolicy);
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            BlobContinuationToken continuationToken1 = new LogStoreBlobContinuationToken(token).GetBlobContinuationToken();
            LogStoreBlobContinuationToken continuationToken2;
            do
            {
              ILogStoreContainerSegment result = logStoreConnectionEndpoint.GetBlobContainersAsync(collectionHostId.ToString(), maxResults, continuationToken1, storeRequestOption.GetBlobRequestOptions(), this._getLogStoreOperationContext(tcmRequestContext)).Result;
              continuationToken2 = result.GetLogStoreBlobContinuationToken();
              continuationToken1 = continuationToken2.GetBlobContinuationToken();
              IEnumerable<TestLogContainer> testLogContainers = result.GetTestContainerList().Where<TestLogContainer>((Func<TestLogContainer, bool>) (ct => Math.Ceiling((DateTime.Now - ct.LastModifiedDate).TotalDays) > (double) OlderByNoOfDays)).Where<TestLogContainer>((Func<TestLogContainer, bool>) (tc => tc != null));
              num2 += testLogContainers.Count<TestLogContainer>();
              num4 += result.GetTestContainerList().Count<TestLogContainer>();
              ++num3;
              storageContainers.AddRange(testLogContainers);
            }
            while (continuationToken1 != null && num2 < maxResults && num3 < maxBlobCalls);
            token = continuationToken2.GetContinuationToken();
            tcmRequestContext.RequestContext.Trace(1015662, TraceLevel.Info, "TestManagement", "LogStorage", "TestLogStoreService.GetStorageContainers totalBlobCalls = {0}, cointainersFetchedFiltered = {1}, cointainersFetchedUnFiltered = {2}, maxContainersToProcess = {3}, maxBlobCalls = {4}", (object) num3, (object) num2, (object) num4, (object) maxResults, (object) maxBlobCalls);
            goto label_9;
          }
          catch (Exception ex)
          {
            tcmRequestContext.RequestContext.Trace(1015662, TraceLevel.Warning, "TestManagement", "LogStorage", "TestLogStoreService.GetStorageContainers failed, ExceptionHit: {0} ", (object) ex.Message);
            goto label_9;
          }
        }
      }
      tcmRequestContext.RequestContext.Trace(1015662, TraceLevel.Warning, "TestManagement", "LogStorage", "TestLogStoreService.GetStorageContainers Failed to get Containers Invalid HostId");
label_9:
      return storageContainers;
    }

    public async Task DownloadToStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      Stream targetStream)
    {
      this._validateProjectAndTestLogReferenceParameters(projectInfo, logReference);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails);
      ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
      IContainerAccessCondition containerAccessCondition = (IContainerAccessCondition) new ContainerAccessCondition();
      try
      {
        await azureBlobStore.DownloadToStreamAsync(tcmRequestContext.RequestContext, this.GetFilePath(logReference), operationContext, targetStream, containerAccessCondition, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "DownloadToStreamAsync - DownloadToStreamAsync failed, ExceptionHit: {0} ", (object) ex.Message);
      }
    }

    public async Task DownloadRangeToStreamAsync(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      int offset,
      int length,
      Stream targetStream)
    {
      this._validateProjectAndTestLogReferenceParameters(projectInfo, logReference);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails);
      ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
      IContainerAccessCondition containerAccessCondition = (IContainerAccessCondition) new ContainerAccessCondition();
      try
      {
        await azureBlobStore.DownloadRangeToStreamAsync(tcmRequestContext.RequestContext, this.GetFilePath(logReference), offset, length, operationContext, targetStream, containerAccessCondition, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "DownloadRangeToStreamAsync failed, ExceptionHit: {0} ", (object) ex.Message);
      }
    }

    public void DownloadToStream(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      Stream targetStream)
    {
      this._validateProjectAndTestLogReferenceParameters(projectInfo, logReference);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails);
      ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
      IContainerAccessCondition containerAccessCondition = (IContainerAccessCondition) new ContainerAccessCondition();
      try
      {
        azureBlobStore.DownloadToStream(tcmRequestContext.RequestContext, this.GetFilePath(logReference), operationContext, targetStream, containerAccessCondition);
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "DownloadToStream - DownloadToStream failed, ExceptionHit: {0} ", (object) ex.Message);
      }
    }

    public async Task<IList<TestTag>> GetTestTagsForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference)
    {
      tcmRequestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      List<TestTag> testTagList = new List<TestTag>();
      this._validateProjectAndTestLogReferenceParameters(projectInfo, logReference);
      tcmRequestContext.RequestContext.TraceInfo("RestLayer", "TestLogStoreService.GetTestTagsForRun projectId = {0}, runId = {1}, buildId= {2}", (object) projectInfo.Id, (object) logReference.RunId, (object) logReference.BuildId);
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectInfo.Uri, TestLogStoreOperationType.Read, out SASPermission _);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails, false);
      return azureBlobStore == null ? (IList<TestTag>) testTagList : await this._getTestTagsForRun(tcmRequestContext, logReference, azureBlobStore).ConfigureAwait(false);
    }

    public async Task<TestLogStatusCode> CreateTestTagForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      string tag)
    {
      tcmRequestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      this._validateProjectAndTestLogReferenceParameters(projectInfo, logReference);
      tcmRequestContext.RequestContext.TraceInfo("RestLayer", "TestLogStoreService.CreateTestTagForRun projectId = {0}, runId = {1}, buildId= {2}, releaseId= {3}", (object) projectInfo.Id, (object) logReference.RunId, (object) logReference.BuildId, (object) logReference.ReleaseId);
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectInfo.Uri, TestLogStoreOperationType.ReadAndCreate, out SASPermission _);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails);
      ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
      try
      {
        if (!await azureBlobStore.CreateContainerIfNotExistsAsync(tcmRequestContext.RequestContext, operationContext, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false))
        {
          tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "CreateTestTagForRun - CreateContainerIfNotExistsAsync failed.");
          return TestLogStatusCode.ContainerNotCreated;
        }
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "CreateTestTagForRun - CreateContainerIfNotExistsAsync failed with ex {0}.", (object) ex.Message);
        return TestLogStatusCode.ContainerNotCreated;
      }
      return await this._createTestTagsForRun(tcmRequestContext, logReference, azureBlobStore, tag).ConfigureAwait(false);
    }

    public async Task<TestLogStatusCode> DeleteTestTagForRun(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference,
      string tag)
    {
      tcmRequestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      this._validateProjectAndTestLogReferenceParameters(projectInfo, logReference);
      tcmRequestContext.RequestContext.TraceInfo("RestLayer", "TestLogStoreService.DeleteTestTagForRun projectId = {0}, runId = {1}, buildId= {2}", (object) projectInfo.Id, (object) logReference.RunId, (object) logReference.BuildId);
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectInfo.Uri, TestLogStoreOperationType.ReadAndCreate, out SASPermission _);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails);
      return await this._deleteTestTagForRun(tcmRequestContext, logReference, azureBlobStore, tag).ConfigureAwait(false);
    }

    public async Task<IList<TestTagSecureObject>> GetUniqueTestTagsForBuildOrRelease(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference)
    {
      tcmRequestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      this._validateProjectAndTestLogReferenceParameters(projectInfo, logReference);
      tcmRequestContext.RequestContext.TraceInfo("RestLayer", "TestLogStoreService.GetTestTagsForRun projectId = {0}, runId = {1}, buildId= {2}", (object) projectInfo.Id, (object) logReference.RunId, (object) logReference.BuildId);
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectInfo.Uri, TestLogStoreOperationType.Read, out SASPermission _);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      List<TestTagSecureObject> secureTags = new List<TestTagSecureObject>();
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails, false);
      if (azureBlobStore == null)
        return (IList<TestTagSecureObject>) secureTags;
      foreach (TestTag TestTag in (IEnumerable<TestTag>) await this._getUniqueTagListForBuildOrRelease(tcmRequestContext, logReference, azureBlobStore).ConfigureAwait(false))
        secureTags.Add(new TestTagSecureObject(projectInfo.Id, TestTag));
      return (IList<TestTagSecureObject>) secureTags;
    }

    public bool CreateEmptyBlobForAttachmentTestLog(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      AttachmentTestLogReference attachmentTestLogReference,
      Dictionary<string, string> metaData)
    {
      ContainerScopeDetails containerScopeDetails = new ContainerScopeDetails();
      containerScopeDetails.ContainerScope = ContainerScope.Run;
      containerScopeDetails.RunIdId = attachmentTestLogReference.RunId;
      this._getLogStoreOperationContext(tcmRequestContext);
      try
      {
        ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectId, containerScopeDetails);
        if (azureBlobStore == null)
        {
          tcmRequestContext.TraceInfo("LogStorage", "No LogStore connection found.");
          return false;
        }
        string attachmentTestLog = this.LogStorePathFormatter.GetFilePathForAttachmentTestLog(attachmentTestLogReference);
        return azureBlobStore.CreateBlob(tcmRequestContext.RequestContext, attachmentTestLog, (Stream) new MemoryStream(), (IDictionary<string, string>) metaData);
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "CreateEmptyBlobForAttachmentTestLog - CreateBlobAsync failed, ExceptionHit: {0} ", (object) ex.Message);
        return false;
      }
    }

    public async Task<bool> CreateEmptyBlobForAttachmentTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      AttachmentTestLogReference attachmentTestLogReference,
      Dictionary<string, string> metaData)
    {
      ContainerScopeDetails containerScopeDetails = new ContainerScopeDetails();
      containerScopeDetails.ContainerScope = ContainerScope.Run;
      containerScopeDetails.RunIdId = attachmentTestLogReference.RunId;
      ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
      try
      {
        ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectId, containerScopeDetails);
        if (azureBlobStore == null)
        {
          tcmRequestContext.TraceInfo("LogStorage", "No LogStore connection found.");
          return false;
        }
        string attachmentTestLog = this.LogStorePathFormatter.GetFilePathForAttachmentTestLog(attachmentTestLogReference);
        return await azureBlobStore.CreateBlobAsync(tcmRequestContext.RequestContext, operationContext, attachmentTestLog, (Stream) new MemoryStream(), tcmRequestContext.RequestContext.CancellationToken, (IDictionary<string, string>) metaData).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015680, TraceLevel.Warning, "TestManagement", "LogStorage", "CreateEmptyBlobForAttachmentTestLog - CreateBlobAsync failed, ExceptionHit: {0} ", (object) ex.Message);
        return false;
      }
    }

    public bool DeleteBlob(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference)
    {
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, testLogReference);
      try
      {
        ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectId, containerScopeDetails);
        string filePath1 = this.GetFilePath(testLogReference);
        IVssRequestContext requestContext = tcmRequestContext.RequestContext;
        string filePath2 = filePath1;
        return azureBlobStore.DeleteBlob(requestContext, filePath2);
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "Delete - Delete failed, ExceptionHit: {0} ", (object) ex.Message);
        return false;
      }
    }

    public bool CreateBlob(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      Stream stream,
      Dictionary<string, string> metaData,
      bool overwrite = false)
    {
      ProjectInfo projectFromGuid = tcmRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectId);
      this._validateProjectAndTestLogReferenceParameters(projectFromGuid, testLogReference);
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectFromGuid.Uri, TestLogStoreOperationType.ReadAndCreate, out SASPermission _);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, testLogReference);
      try
      {
        ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectId, containerScopeDetails);
        if (azureBlobStore == null)
        {
          tcmRequestContext.RequestContext.Trace(1015113, TraceLevel.Error, "TestManagement", nameof (TestLogStoreService), "TeamFoundationTestManagementAttachmentsService.CreateTestAttachment No LogStore connection found. for projectID: {0}", (object) projectId);
          return false;
        }
        ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
        if (!azureBlobStore.CreateContainerIfNotExists(tcmRequestContext.RequestContext, operationContext))
        {
          tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlob - CreateContainerIfNotExists failed.");
          return false;
        }
        string filePath = this.GetFilePath(testLogReference);
        return azureBlobStore.CreateBlob(tcmRequestContext.RequestContext, filePath, stream, (IDictionary<string, string>) metaData, overwrite);
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlob - CreateBlob failed, ExceptionHit: {0} ", (object) ex.Message);
        return false;
      }
    }

    public TestLogStatusWithFileName CreateBlobWithFallbackName(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      Stream stream,
      Dictionary<string, string> metaData)
    {
      string str = this.LogStorePathFormatter.SanitizeFilePath(testLogReference.FilePath);
      TestLogStatusWithFileName withFallbackName = new TestLogStatusWithFileName();
      ProjectInfo projectFromGuid = tcmRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectId);
      this._validateProjectAndTestLogReferenceParameters(projectFromGuid, testLogReference);
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectFromGuid.Uri, TestLogStoreOperationType.ReadAndCreate, out SASPermission _);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, testLogReference);
      try
      {
        ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectId, containerScopeDetails);
        if (azureBlobStore == null)
        {
          tcmRequestContext.RequestContext.Trace(1015971, TraceLevel.Error, "TestManagement", nameof (TestLogStoreService), "TeamFoundationTestManagementAttachmentsService.CreateTestAttachment No LogStore connection found. for projectID: {0}", (object) projectId);
          withFallbackName.Status = TestLogStatusCode.ContainerNotFound;
          return withFallbackName;
        }
        ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
        if (!azureBlobStore.CreateContainerIfNotExists(tcmRequestContext.RequestContext, operationContext))
        {
          tcmRequestContext.RequestContext.Trace(1015971, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlobWithFallbackName - CreateContainerIfNotExists failed.");
          withFallbackName.Status = TestLogStatusCode.ContainerNotFound;
          return withFallbackName;
        }
        string filePath1 = this.GetFilePath(testLogReference);
        bool blob;
        if ((azureBlobStore.IsBlobExists(tcmRequestContext.RequestContext, filePath1, operationContext) ? 1 : 0) == 0)
        {
          blob = azureBlobStore.CreateBlob(tcmRequestContext.RequestContext, filePath1, stream, (IDictionary<string, string>) metaData);
        }
        else
        {
          str = this.GetDuplicateFilePath(testLogReference.FilePath);
          testLogReference.FilePath = str;
          string filePath2 = this.GetFilePath(testLogReference);
          blob = azureBlobStore.CreateBlob(tcmRequestContext.RequestContext, filePath2, stream, (IDictionary<string, string>) metaData);
        }
        if (blob)
        {
          withFallbackName.Status = TestLogStatusCode.Success;
          withFallbackName.FileName = str;
        }
        else
          withFallbackName.Status = TestLogStatusCode.Failed;
        return withFallbackName;
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015971, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlobWithFallbackName - CreateBlob failed, ExceptionHit: {0} ", (object) ex.Message);
        withFallbackName.Status = TestLogStatusCode.Failed;
        withFallbackName.Exception = ex.ToString();
        return withFallbackName;
      }
    }

    public async Task<bool> CreateBlobAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      Stream stream,
      Dictionary<string, string> metaData,
      bool overwrite = false)
    {
      ProjectInfo projectFromGuid = tcmRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectId);
      this._validateProjectAndTestLogReferenceParameters(projectFromGuid, testLogReference);
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectFromGuid.Uri, TestLogStoreOperationType.ReadAndCreate, out SASPermission _);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, testLogReference);
      try
      {
        ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectId, containerScopeDetails);
        if (azureBlobStore == null)
        {
          tcmRequestContext.TraceInfo("LogStorage", "No LogStore connection found.");
          return false;
        }
        ILogStoreOperationContext logStoreOperationContext = this._getLogStoreOperationContext(tcmRequestContext);
        if (await azureBlobStore.CreateContainerIfNotExistsAsync(tcmRequestContext.RequestContext, logStoreOperationContext, tcmRequestContext.RequestContext.CancellationToken))
          return await azureBlobStore.CreateBlobAsync(tcmRequestContext.RequestContext, logStoreOperationContext, this.GetFilePath(testLogReference), stream, tcmRequestContext.RequestContext.CancellationToken, (IDictionary<string, string>) metaData, overwrite);
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlobAsync - CreateContainerIfNotExists failed.");
        return false;
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlobAsync - CreateBlob failed, ExceptionHit: {0} ", (object) ex.Message);
        return false;
      }
    }

    public List<TestLog> GetAllTestLogs(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference,
      bool isFile = false)
    {
      List<TestLog> allTestLogs = new List<TestLog>();
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      this._validateQueryParametersAndCheckAccessPermission(tcmRequestContext, projectInfo, logQueryParameters, logReference, new int?(), containerScopeDetails);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails, false);
      do
      {
        PagedList<TestLogSecureObject> testLogs = this.InternalGetTestLogs(tcmRequestContext, projectInfo, logQueryParameters, logReference, new int?(), azureBlobStore, isFile);
        allTestLogs.AddRange(testLogs.Select<TestLogSecureObject, TestLog>((Func<TestLogSecureObject, TestLog>) (l => l.TestLog)));
        logQueryParameters.ContinuationToken = testLogs.ContinuationToken;
      }
      while (!string.IsNullOrEmpty(logQueryParameters.ContinuationToken));
      return allTestLogs;
    }

    public List<AttachmentTestLog> GetAllAttachmentTestLogs(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogQueryParameters logQueryParameters,
      AttachmentTestLogReference logReference)
    {
      List<AttachmentTestLog> attachmentTestLogs = new List<AttachmentTestLog>();
      string attachmentTestLog = this.LogStorePathFormatter.GetFilePathForAttachmentTestLog(logReference);
      ILogStoreBlobListingDetails blobListingDetails = (ILogStoreBlobListingDetails) new LogStoreBlobListingDetails(logQueryParameters.FetchMetaData);
      ILogStoreBlobContinuationToken blobContinuationToken = (ILogStoreBlobContinuationToken) new LogStoreBlobContinuationToken(logQueryParameters.ContinuationToken);
      ILogStoreOperationContext operationContext = this._getLogStoreOperationContext(tcmRequestContext);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, (TestLogReference) logReference);
      LogStoreHelper.ValidateContainerDetails(tcmRequestContext, containerScopeDetails, projectInfo);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails);
      do
      {
        ILogStoreBlobResultSegment blobResultSegment = azureBlobStore.ListBlobs(tcmRequestContext.RequestContext, attachmentTestLog, true, blobListingDetails, new int?(), blobContinuationToken, operationContext);
        if (blobResultSegment == null)
        {
          tcmRequestContext.TraceWarning("RestLayer", "TestLogStoreService.GetAllAttachmentTestLogs: AzureBlobStore.ListBlobs failed for projectId = {0}, runId = {1}", (object) projectInfo?.Id, (object) logReference?.RunId);
          return attachmentTestLogs;
        }
        attachmentTestLogs.AddRange((IEnumerable<AttachmentTestLog>) blobResultSegment.GetAttachmentTestLogs(logReference));
        blobContinuationToken = (ILogStoreBlobContinuationToken) blobResultSegment.GetLogStoreBlobContinuationToken();
      }
      while (!string.IsNullOrEmpty(blobContinuationToken.GetContinuationToken()));
      return attachmentTestLogs;
    }

    public async Task<TestTagSummarySecureObject> GetTestTagDetailForBuildOrRelease(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogReference logReference)
    {
      tcmRequestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      Dictionary<int, HashSet<string>> dictionary = new Dictionary<int, HashSet<string>>();
      this._validateProjectAndTestLogReferenceParameters(projectInfo, logReference);
      tcmRequestContext.RequestContext.TraceInfo("RestLayer", "TestLogStoreService.GetTestTagsForRun projectId = {0}, runId = {1}, buildId= {2}", (object) projectInfo.Id, (object) logReference.RunId, (object) logReference.BuildId);
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectInfo.Uri, TestLogStoreOperationType.Read, out SASPermission _);
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails, false);
      return azureBlobStore == null ? new TestTagSummarySecureObject(projectInfo.Id, (TestTagSummary) null) : new TestTagSummarySecureObject(projectInfo.Id, await this._getTestTagRunIdMapForBuildOrRelease(tcmRequestContext, logReference, azureBlobStore).ConfigureAwait(false));
    }

    public async Task<List<TestLog>> GetAllTestLogsAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference)
    {
      ContainerScopeDetails containerScopeDetails = LogStoreHelper.GetContainerScopeDetails(tcmRequestContext, logReference);
      ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectId, containerScopeDetails, false);
      if (azureBlobStore == null)
        return new List<TestLog>();
      string filePathPrefix = this.GetFilePathPrefix(logQueryParameters, logReference);
      ILogStoreBlobListingDetails logStoreBlobListingDetails = (ILogStoreBlobListingDetails) new LogStoreBlobListingDetails(logQueryParameters.FetchMetaData);
      ILogStoreBlobContinuationToken logStoreBlobContinuationToken = (ILogStoreBlobContinuationToken) new LogStoreBlobContinuationToken(logQueryParameters.ContinuationToken);
      ILogStoreOperationContext logStoreOperationContext = this._getLogStoreOperationContext(tcmRequestContext);
      List<TestLog> allTestLogs = new List<TestLog>();
      do
      {
        tcmRequestContext.RequestContext.CancellationToken.ThrowIfCancellationRequested();
        allTestLogs.AddRange((IEnumerable<TestLog>) (await azureBlobStore.ListBlobsAsync(tcmRequestContext.RequestContext, filePathPrefix, true, logStoreBlobListingDetails, new int?(), logStoreBlobContinuationToken, logStoreOperationContext, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false)).GetTestLogList(logReference.Scope, logReference.Scope == TestLogScope.Build ? logReference.BuildId : logReference.RunId));
      }
      while (!string.IsNullOrEmpty(logStoreBlobContinuationToken.GetContinuationToken()));
      return allTestLogs;
    }

    private async Task<(bool status, TestLogStoreEndpointDetailsSecureObject endpointSecureObject)> _tryCreateSasUriAsync(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogStoreOperationType testLogStoreOperationType,
      TestLogStoreEndpointType testLogStoreEndpointScope,
      TestLogReference logReference,
      ContainerScopeDetails containerScopeDetails,
      SASPermission sasPermission,
      int sasValidityInHours,
      bool isExternalCustomer)
    {
      tcmRequestContext.RequestContext.TraceEnter("TraceLayer.RestLayer", "tryCreateSasUriAsync");
      TestLogStoreEndpointDetails testLogStoreEndpointDetails = new TestLogStoreEndpointDetails();
      TestLogStoreEndpointDetailsSecureObject detailsSecureObject;
      try
      {
        testLogStoreEndpointDetails.EndpointType = testLogStoreEndpointScope;
        testLogStoreEndpointDetails.Status = TestLogStatusCode.Success;
        ITestLogStore azureBlobStore = this._getAzureBlobStore(tcmRequestContext, projectInfo.Id, containerScopeDetails);
        ILogStoreOperationContext logStoreOperationContext = this._getLogStoreOperationContext(tcmRequestContext);
        ITestLogStorageConnection testLogStorageConnection = tcmRequestContext.RequestContext.GetService<ITeamFoundationTestLogStoreService>().GetTestLogStorageConnection(tcmRequestContext.RequestContext, projectInfo.Id, containerScopeDetails);
        if (!await azureBlobStore.CreateContainerIfNotExistsAsync(tcmRequestContext.RequestContext, logStoreOperationContext, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false))
        {
          tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "GetTestLogStoreEndpointDetails - CreateContainerIfNotExistsAsync failed.");
          testLogStoreEndpointDetails.Status = TestLogStatusCode.Failed;
          detailsSecureObject = new TestLogStoreEndpointDetailsSecureObject(projectInfo.Id, testLogStoreEndpointDetails);
          return (false, detailsSecureObject);
        }
        if (testLogStoreEndpointScope == TestLogStoreEndpointType.File && (testLogStoreOperationType == TestLogStoreOperationType.Create || testLogStoreOperationType == TestLogStoreOperationType.ReadAndCreate))
        {
          TestLogStatusCode testLogStatusCode = await this._createEmptyBlobAsync(tcmRequestContext, logStoreOperationContext, logReference, azureBlobStore).ConfigureAwait(false);
          if (testLogStatusCode != TestLogStatusCode.Success)
          {
            testLogStoreEndpointDetails.Status = testLogStatusCode;
            detailsSecureObject = new TestLogStoreEndpointDetailsSecureObject(projectInfo.Id, testLogStoreEndpointDetails);
            return (false, detailsSecureObject);
          }
        }
        string str = await this._getSASUri(tcmRequestContext, testLogStoreEndpointScope, logReference, azureBlobStore, logStoreOperationContext, sasPermission, sasValidityInHours, testLogStorageConnection.SupportsHttps(), isExternalCustomer).ConfigureAwait(false);
        if (string.IsNullOrEmpty(str))
        {
          tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "GetTestLogStoreEndpointDetails - GetSharedAccessPolicyForContainerAsync failed.");
          testLogStoreEndpointDetails.Status = TestLogStatusCode.Failed;
          detailsSecureObject = new TestLogStoreEndpointDetailsSecureObject(projectInfo.Id, testLogStoreEndpointDetails);
          return (false, detailsSecureObject);
        }
        testLogStoreEndpointDetails.EndpointSASUri = str;
        detailsSecureObject = new TestLogStoreEndpointDetailsSecureObject(projectInfo.Id, testLogStoreEndpointDetails);
        azureBlobStore = (ITestLogStore) null;
        logStoreOperationContext = (ILogStoreOperationContext) null;
        testLogStorageConnection = (ITestLogStorageConnection) null;
      }
      finally
      {
        tcmRequestContext.RequestContext.TraceLeave("TraceLayer.RestLayer", "tryCreateSasUriAsync");
      }
      return (true, detailsSecureObject);
    }

    private bool _checkPermissionsAndValidateRequest(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogStoreOperationType testLogStoreOperationType,
      TestLogStoreEndpointType testLogStoreEndpointScope,
      TestLogReference logReference,
      ContainerScopeDetails containerScopeDetails,
      out SASPermission sasPermission,
      out TestLogStoreEndpointDetailsSecureObject endpointSecureObject)
    {
      tcmRequestContext.RequestContext.TraceEnter("TraceLayer.RestLayer", "checkPermissionsAndValidateRequest");
      TestLogStoreEndpointDetails testLogStoreEndpointDetails = new TestLogStoreEndpointDetails();
      this._validateGetTestLogStoreEndpointDetailsParameters(projectInfo, testLogStoreEndpointScope, logReference);
      tcmRequestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectInfo.Uri, testLogStoreOperationType, out sasPermission);
      endpointSecureObject = new TestLogStoreEndpointDetailsSecureObject(projectInfo.Id, testLogStoreEndpointDetails);
      bool checkPublishingPermission = testLogStoreEndpointScope == TestLogStoreEndpointType.Root;
      try
      {
        LogStoreHelper.ValidateContainerDetails(tcmRequestContext, containerScopeDetails, projectInfo, checkPublishingPermission);
        this._checkForPublicProjectBreach(tcmRequestContext.RequestContext, projectInfo, testLogStoreOperationType);
      }
      catch (TestManagementInvalidOperationException ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "GetTestLogStoreEndpointDetails - ValidateContainerDetails Failed, ExceptionHit: {0}", (object) ex.Message);
        testLogStoreEndpointDetails.Status = TestLogStatusCode.FeatureDisabled;
        endpointSecureObject = new TestLogStoreEndpointDetailsSecureObject(projectInfo.Id, testLogStoreEndpointDetails);
        return false;
      }
      catch (TestLogStoreValidationFailureException ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "GetTestLogStoreEndpointDetails - CheckForPublicProjectBreach Failed, ExceptionHit: {0}", (object) ex.Message);
        testLogStoreEndpointDetails.Status = TestLogStatusCode.StorageCapacityExceeded;
        endpointSecureObject = new TestLogStoreEndpointDetailsSecureObject(projectInfo.Id, testLogStoreEndpointDetails);
        return false;
      }
      finally
      {
        tcmRequestContext.RequestContext.TraceLeave("TraceLayer.RestLayer", "checkPermissionsAndValidateRequest");
      }
      return true;
    }

    private void TryDeletingShardingMapping(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      ContainerScopeDetails containerScopeDetails)
    {
      try
      {
        int artifactId = this.GetArtifactId(containerScopeDetails);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(tcmRequestContext))
          managementDatabase.DeleteLogStoreArtifactStorageAccountMap(projectId, containerScopeDetails.ContainerScope, artifactId);
      }
      catch (Exception ex)
      {
        tcmRequestContext.TraceException("LogStorage", ex);
      }
    }

    private int GetArtifactId(ContainerScopeDetails scopeDetails)
    {
      switch (scopeDetails.ContainerScope)
      {
        case ContainerScope.Build:
          return scopeDetails.BuildId;
        case ContainerScope.Run:
          return scopeDetails.RunIdId;
        case ContainerScope.Release:
          return scopeDetails.ReleaseId;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "ContainerScope"));
      }
    }

    private PagedList<TestLogSecureObject> InternalGetTestLogs(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference,
      int? top,
      ITestLogStore azureBlobStore = null,
      bool isFile = false)
    {
      if (azureBlobStore == null)
        return new PagedList<TestLogSecureObject>((IEnumerable<TestLogSecureObject>) this.ConvertToTestLogSecureObject(projectInfo.Id, (IList<TestLog>) new List<TestLog>()), string.Empty);
      string filePathPrefix = isFile ? this.GetFilePath(logReference) : this.GetFilePathPrefix(logQueryParameters, logReference);
      ILogStoreBlobResultSegment logStoreBlobResultSegment = azureBlobStore.ListBlobs(tcmRequestContext.RequestContext, filePathPrefix, true, (ILogStoreBlobListingDetails) new LogStoreBlobListingDetails(logQueryParameters.FetchMetaData), top, (ILogStoreBlobContinuationToken) new LogStoreBlobContinuationToken(logQueryParameters.ContinuationToken), this._getLogStoreOperationContext(tcmRequestContext));
      return this.GetTestLogSecureObectsFromBlobResultSegment(projectInfo, logStoreBlobResultSegment, logReference);
    }

    private void _validateQueryParametersAndCheckAccessPermission(
      TestManagementRequestContext tcmRequestContext,
      ProjectInfo projectInfo,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference,
      int? top,
      ContainerScopeDetails containerScopeDetails)
    {
      this._validateGetTestLogsParameters(tcmRequestContext.RequestContext, projectInfo, logQueryParameters, logReference, top);
      tcmRequestContext.RequestContext.TraceInfo("RestLayer", "TestLogStoreService.GetTestLogStoreEndpointDetails projectId = {0}, runId = {1}, buildId= {2}", (object) projectInfo.Id, (object) logReference.RunId, (object) logReference.BuildId);
      tcmRequestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      this.CheckLogStoreAccessPermission(tcmRequestContext, projectInfo.Uri, TestLogStoreOperationType.Read, out SASPermission _);
      LogStoreHelper.ValidateContainerDetails(tcmRequestContext, containerScopeDetails, projectInfo);
    }

    private PagedList<TestLogSecureObject> GetTestLogSecureObectsFromBlobResultSegment(
      ProjectInfo projectInfo,
      ILogStoreBlobResultSegment logStoreBlobResultSegment,
      TestLogReference logReference)
    {
      string continuationToken = logStoreBlobResultSegment.GetLogStoreBlobContinuationToken().GetContinuationToken();
      IList<TestLog> testLogList = (IList<TestLog>) logStoreBlobResultSegment.GetTestLogList(logReference.Scope, logReference.Scope == TestLogScope.Build ? logReference.BuildId : logReference.RunId);
      return new PagedList<TestLogSecureObject>((IEnumerable<TestLogSecureObject>) this.ConvertToTestLogSecureObject(projectInfo.Id, testLogList), continuationToken);
    }

    private bool CheckIfContainerIsBelowThreshold(
      IVssRequestContext requestContext,
      ContainerScopeDetails containerScopeDetails)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num = containerScopeDetails.ContainerScope == ContainerScope.Build ? service.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreMoveBuildIdThreshold", 0) : service.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreMoveRunIdThreshold", 0);
      return containerScopeDetails.ContainerScope != ContainerScope.Build ? containerScopeDetails.RunIdId <= num : containerScopeDetails.BuildId <= num;
    }

    private bool CheckLogStoreAccessPermission(
      TestManagementRequestContext requestContext,
      string projectUri,
      TestLogStoreOperationType testLogStoreOperationType,
      out SASPermission sasPermission)
    {
      sasPermission = SASPermission.None;
      bool hasReadPermission = requestContext.SecurityManager.HasViewTestResultsPermission(requestContext, projectUri);
      bool hasCreatePermission = requestContext.SecurityManager.HasPublishTestResultsPermission(requestContext, projectUri);
      if (!hasReadPermission && !hasCreatePermission)
        throw new AccessDeniedException(ServerResources.GenericPermission);
      sasPermission = this.GetSASPermission(hasReadPermission, hasCreatePermission, testLogStoreOperationType);
      return true;
    }

    private SASPermission GetSASPermission(
      bool hasReadPermission,
      bool hasCreatePermission,
      TestLogStoreOperationType testLogStoreOperationType)
    {
      SASPermission sasPermission = SASPermission.None;
      switch (testLogStoreOperationType)
      {
        case TestLogStoreOperationType.Read:
          if (hasReadPermission)
          {
            sasPermission = SASPermission.ReadList_Policy;
            break;
          }
          break;
        case TestLogStoreOperationType.Create:
          if (hasCreatePermission)
          {
            sasPermission = SASPermission.AddCreateWrite_Policy;
            break;
          }
          break;
        case TestLogStoreOperationType.ReadAndCreate:
          if (hasReadPermission & hasCreatePermission)
          {
            sasPermission = SASPermission.ReadAddCreateWriteList_Policy;
            break;
          }
          break;
        default:
          sasPermission = SASPermission.None;
          break;
      }
      return sasPermission != SASPermission.None ? sasPermission : throw new AccessDeniedException(ServerResources.GenericPermission);
    }

    private string GetFilePathPrefix(
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference)
    {
      string destinationBlobName = this.LogStorePathFormatter.GetDestinationBlobName(logQueryParameters.FileNamePrefix, logQueryParameters.DirectoryPath);
      return this.LogStorePathFormatter.GetBlobReferenceName(logReference, logQueryParameters.Type, destinationBlobName, false);
    }

    private string GetFilePath(TestLogReference logReference, bool isDuplicate = false)
    {
      string destFilePath = this.LogStorePathFormatter.SanitizeFilePath(logReference.FilePath);
      return this.LogStorePathFormatter.GetBlobReferenceName(logReference, logReference.Type, destFilePath, isDuplicate);
    }

    private string GetDuplicateFilePath(string fileName) => this.LogStorePathFormatter.GetNameWhenDuplicate(this.LogStorePathFormatter.SanitizeFilePath(fileName));

    private ITestLogStore _getAzureBlobStore(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      ContainerScopeDetails containerScopeDetails,
      bool createIfNotExist = true,
      ITestLogStorageConnection testLogStorageConnection = null)
    {
      tcmRequestContext.RequestContext.TraceEnter("TraceLayer.RestLayer", "getAzureBlobStore");
      try
      {
        if (testLogStorageConnection == null)
          testLogStorageConnection = this._getTestLogStoreConnection(tcmRequestContext, projectId, containerScopeDetails, createIfNotExist);
        if (testLogStorageConnection == null)
        {
          tcmRequestContext.TraceInfo("LogStorage", "Unable to get the storage account details or no mapping exist for the artifact.");
          return (ITestLogStore) null;
        }
        ILogStoreContainerAccessPolicy logStoreContainerAccessPolicy = (ILogStoreContainerAccessPolicy) new LogStoreContainerAccessPolicy();
        IVssRegistryService service = tcmRequestContext.RequestContext.GetService<IVssRegistryService>();
        int serverTimeoutInSeconds = service.GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreServerTimeoutInSeconds", 60);
        int maximumExecutionTimeInSeconds = service.GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreServerMaxExecutionTimeInSeconds", 900);
        int maxAttempts = service.GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreMaxAttemptsRetry", 10);
        int num = service.GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreDeltaBackOffInSeconds", 3);
        LogStoreExponentialRetryPolicy logStoreExponentialRetryPolicy = new LogStoreExponentialRetryPolicy(tcmRequestContext.RequestContext, TimeSpan.FromSeconds((double) num), maxAttempts);
        ILogStoreRequestOption logStoreRequestOption = (ILogStoreRequestOption) new LogStoreRequestOption(serverTimeoutInSeconds, maximumExecutionTimeInSeconds, logStoreExponentialRetryPolicy);
        string containerName = LogStoreHelper.GetContainerName(tcmRequestContext.RequestContext, projectId, containerScopeDetails);
        service.GetValue<int>(tcmRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreSASValidityInHours", 1);
        return tcmRequestContext.RequestContext.GetExtension<ITestLogStoreServiceProvider>().GetTestLogServiceStore(tcmRequestContext.RequestContext, testLogStorageConnection.GetLogStoreConnectionEndpoint(), containerName, logStoreContainerAccessPolicy, logStoreRequestOption);
      }
      finally
      {
        tcmRequestContext.RequestContext.TraceLeave("TraceLayer.RestLayer", "getAzureBlobStore");
      }
    }

    private ITestLogStorageConnection _getTestLogStoreConnection(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      ContainerScopeDetails containerScopeDetails,
      bool createIfNotExist)
    {
      ITeamFoundationTestLogStoreService service = tcmRequestContext.RequestContext.GetService<ITeamFoundationTestLogStoreService>();
      ITestLogStorageConnection logStoreConnection;
      if (tcmRequestContext.IsFeatureEnabled("TestManagement.Server.EnableDualReadForTestLogStoreAccounts") & this.CheckIfContainerIsBelowThreshold(tcmRequestContext.RequestContext, containerScopeDetails))
      {
        logStoreConnection = service.GetOmegaTestLogStoreConnection(tcmRequestContext.RequestContext, projectId, containerScopeDetails);
        tcmRequestContext.RequestContext.Trace(1015679, TraceLevel.Info, "TestManagement", "LogStorage", "Using Omega log store connection");
      }
      else
      {
        logStoreConnection = service.GetTestLogStorageConnection(tcmRequestContext.RequestContext, projectId, containerScopeDetails, createIfNotExist);
        tcmRequestContext.RequestContext.Trace(1015679, TraceLevel.Info, "TestManagement", "LogStorage", "Using Primary log store connection");
      }
      return logStoreConnection;
    }

    private async Task<string> _getSASUri(
      TestManagementRequestContext tcmRequestContext,
      TestLogStoreEndpointType testLogStoreEndpointScope,
      TestLogReference logReference,
      ITestLogStore azureBlobStore,
      ILogStoreOperationContext logStoreOperationContext,
      SASPermission sasPermission,
      int sasValidityInHours,
      bool supportsHttps,
      bool isExternalCustomer)
    {
      tcmRequestContext.RequestContext.TraceEnter("TraceLayer.RestLayer", "getSASUri");
      string sasUri;
      try
      {
        string empty = string.Empty;
        ILogStoreContainerSASPolicy logStoreContainerSASPolicy = (ILogStoreContainerSASPolicy) new LogStoreContainerSASPolicy(new DateTimeOffset?(DateTimeOffset.UtcNow.AddHours((double) sasValidityInHours)), sasPermission, supportsHttps, isExternalCustomer);
        IContainerAccessCondition containerAccessCondition = (IContainerAccessCondition) new ContainerAccessCondition();
        string str;
        if (testLogStoreEndpointScope == TestLogStoreEndpointType.File)
          str = await azureBlobStore.GetSharedAccessPolicyForBlobAsync(tcmRequestContext.RequestContext, this.GetFilePath(logReference), logStoreOperationContext, containerAccessCondition, logStoreContainerSASPolicy, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
        else
          str = await azureBlobStore.GetSharedAccessPolicyForContainerAsync(tcmRequestContext.RequestContext, logStoreOperationContext, containerAccessCondition, logStoreContainerSASPolicy, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
        sasUri = str;
      }
      finally
      {
        tcmRequestContext.RequestContext.TraceLeave("TraceLayer.RestLayer", "getSASUri");
      }
      return sasUri;
    }

    private void _checkForPublicProjectBreach(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      TestLogStoreOperationType testLogStoreOperationType)
    {
      if (projectInfo.Visibility != ProjectVisibility.Public || testLogStoreOperationType == TestLogStoreOperationType.Read)
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string query1 = string.Format("/Service/TestManagement/Settings/{0}/TcmLogStoreStorageAccountCapcityReached", (object) projectInfo.Id);
      if (service.GetValue<bool>(requestContext, (RegistryQuery) query1, false))
      {
        string query2 = string.Format("/Service/TestManagement/Settings/{0}/TcmLogStoreStorageAccountCapacity", (object) projectInfo.Id);
        throw new TestLogStoreValidationFailureException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PublicProjectReachedLogStoreCapacityThreshold, (object) service.GetValue<int>(requestContext, (RegistryQuery) query2, 2)));
      }
    }

    private void _validateGetTestLogStoreEndpointDetailsParameters(
      ProjectInfo projectInfo,
      TestLogStoreEndpointType testLogStoreEndpointScope,
      TestLogReference logReference)
    {
      this._validateProject(projectInfo);
      ArgumentUtility.CheckForNull<TestLogReference>(logReference, nameof (logReference), "LogStorage");
      this._validateLogReferenceIds(logReference);
      if (testLogStoreEndpointScope == TestLogStoreEndpointType.File && string.IsNullOrWhiteSpace(logReference.FilePath))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "FilePath"));
    }

    private void _validateGetTestLogsParameters(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference,
      int? top)
    {
      this._validateProjectAndTestLogReferenceParameters(projectInfo, logReference);
      ArgumentUtility.CheckForNull<TestLogQueryParameters>(logQueryParameters, nameof (logQueryParameters), "LogStorage");
      ArgumentUtility.CheckForNull<TestLogReference>(logReference, nameof (logReference), "LogStorage");
      if (top.HasValue)
        ArgumentUtility.CheckForNonPositiveInt(top.Value, nameof (top), "LogStorage");
      int num1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreMaxBlobResultsCount", 4000);
      int? nullable = top;
      int num2 = num1;
      if (nullable.GetValueOrDefault() > num2 & nullable.HasValue)
        throw new ArgumentOutOfRangeException(nameof (top), (object) top, (string) null).Expected("Test Results");
    }

    private void _validateProjectAndTestLogReferenceParameters(
      ProjectInfo projectInfo,
      TestLogReference logReference)
    {
      this._validateProject(projectInfo);
      ArgumentUtility.CheckForNull<TestLogReference>(logReference, nameof (logReference), "LogStorage");
      this._validateLogReferenceIds(logReference);
    }

    private void _validateProject(ProjectInfo projectInfo)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(projectInfo, nameof (projectInfo), "LogStorage");
      ArgumentUtility.CheckForEmptyGuid(projectInfo.Id, "teamProjectId", "LogStorage");
      ArgumentUtility.CheckStringForNullOrEmpty(projectInfo.Name, "teamProjectName", "LogStorage");
    }

    private void _validateLogReferenceIds(TestLogReference logReference)
    {
      if (logReference.Scope == TestLogScope.Build)
        ArgumentUtility.CheckGreaterThanZero((float) logReference.BuildId, "BuildId", "LogStorage");
      else if (logReference.Scope == TestLogScope.Run)
      {
        ArgumentUtility.CheckGreaterThanZero((float) logReference.RunId, "RunId", "LogStorage");
        ArgumentUtility.CheckGreaterThanOrEqualToZero((float) logReference.SubResultId, "SubResultId", "LogStorage");
        if (logReference.SubResultId > 0)
          ArgumentUtility.CheckGreaterThanZero((float) logReference.ResultId, "ResultId", "LogStorage");
        else
          ArgumentUtility.CheckGreaterThanOrEqualToZero((float) logReference.ResultId, "ResultId", "LogStorage");
      }
      else
      {
        if (logReference.Scope != TestLogScope.Release)
          return;
        ArgumentUtility.CheckGreaterThanZero((float) logReference.ReleaseId, "ReleaseId", "LogStorage");
        ArgumentUtility.CheckGreaterThanZero((float) logReference.ReleaseEnvId, "ReleaseEnvId", "LogStorage");
      }
    }

    private ILogStoreOperationContext _getLogStoreOperationContext(
      TestManagementRequestContext tcmRequestContext)
    {
      return (ILogStoreOperationContext) new LogStoreOperationContext(DateTime.UtcNow.Date, tcmRequestContext.RequestContext.ActivityId.ToString());
    }

    private async Task<IList<TestTag>> _getTestTagsForRun(
      TestManagementRequestContext tcmRequestContext,
      TestLogReference logReference,
      ITestLogStore azureBlobStore)
    {
      List<TestTag> runtags = new List<TestTag>();
      IList<TestTag> testTagList = await this._getUniqueTagListForBuildOrRelease(tcmRequestContext, logReference, azureBlobStore).ConfigureAwait(false);
      ILogStoreOperationContext logStoreOperationContext = this._getLogStoreOperationContext(tcmRequestContext);
      foreach (TestTag tag in (IEnumerable<TestTag>) testTagList)
      {
        string referenceNameForTagFile = this.LogStorePathFormatter.GetBlobReferenceNameForTagFile(logReference, tag.Name);
        if (!string.IsNullOrEmpty(referenceNameForTagFile))
        {
          if (await azureBlobStore.IsBlobExists(tcmRequestContext.RequestContext, referenceNameForTagFile, logStoreOperationContext, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false))
            runtags.Add(tag);
        }
      }
      IList<TestTag> testTagsForRun = (IList<TestTag>) runtags;
      runtags = (List<TestTag>) null;
      logStoreOperationContext = (ILogStoreOperationContext) null;
      return testTagsForRun;
    }

    private async Task<TestLogStatusCode> _createTestTagsForRun(
      TestManagementRequestContext tcmRequestContext,
      TestLogReference logReference,
      ITestLogStore azureBlobStore,
      string tag)
    {
      ILogStoreOperationContext logStoreOperationContext = this._getLogStoreOperationContext(tcmRequestContext);
      string tagVirtualPathWithEmptyBlobName = this.LogStorePathFormatter.GetBlobReferenceNameForTagFile(logReference, tag);
      if (string.IsNullOrEmpty(tagVirtualPathWithEmptyBlobName))
        return TestLogStatusCode.InvalidFileName;
      try
      {
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = azureBlobStore.IsBlobExists(tcmRequestContext.RequestContext, tagVirtualPathWithEmptyBlobName, logStoreOperationContext, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
        if (await configuredTaskAwaitable)
          return TestLogStatusCode.FileAlreadyExists;
        configuredTaskAwaitable = azureBlobStore.CreateBlobAsync(tcmRequestContext.RequestContext, logStoreOperationContext, tagVirtualPathWithEmptyBlobName, (Stream) new MemoryStream(), tcmRequestContext.RequestContext.CancellationToken, (IDictionary<string, string>) null).ConfigureAwait(false);
        return await configuredTaskAwaitable ? TestLogStatusCode.Success : TestLogStatusCode.Failed;
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "_createTestTagsForRun - Unable to create the blob {0}. Exception Hit: {1}", (object) tagVirtualPathWithEmptyBlobName, (object) ex.Message);
        return TestLogStatusCode.InvalidFileName;
      }
    }

    private async Task<TestLogStatusCode> _deleteTestTagForRun(
      TestManagementRequestContext tcmRequestContext,
      TestLogReference logReference,
      ITestLogStore azureBlobStore,
      string tag)
    {
      ILogStoreOperationContext logStoreOperationContext = this._getLogStoreOperationContext(tcmRequestContext);
      string tagVirtualPathWithEmptyBlobName = this.LogStorePathFormatter.GetBlobReferenceNameForTagFile(logReference, tag);
      if (string.IsNullOrEmpty(tagVirtualPathWithEmptyBlobName))
        return TestLogStatusCode.InvalidFileName;
      try
      {
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = azureBlobStore.IsBlobExists(tcmRequestContext.RequestContext, tagVirtualPathWithEmptyBlobName, logStoreOperationContext, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
        if (!await configuredTaskAwaitable)
          return TestLogStatusCode.FileAlreadyExists;
        configuredTaskAwaitable = azureBlobStore.DeleteBlobAsync(tcmRequestContext.RequestContext, logStoreOperationContext, tagVirtualPathWithEmptyBlobName, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
        return await configuredTaskAwaitable ? TestLogStatusCode.Success : TestLogStatusCode.Failed;
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "_deleteTestTagForRun - Unable to delete the blob {0}. Exception Hit: {1}", (object) tagVirtualPathWithEmptyBlobName, (object) ex.Message);
        return TestLogStatusCode.InvalidFileName;
      }
    }

    private async Task<IList<TestTag>> _getUniqueTagListForBuildOrRelease(
      TestManagementRequestContext tcmRequestContext,
      TestLogReference logReference,
      ITestLogStore azureBlobStore)
    {
      HashSet<TestTag> uniqueTags = new HashSet<TestTag>();
      this._getLogStoreOperationContext(tcmRequestContext);
      string directoryPrefixForTag = this.LogStorePathFormatter.GetDirectoryPrefixForTag(logReference);
      if (!string.IsNullOrEmpty(directoryPrefixForTag))
      {
        foreach (string directoryPath in (IEnumerable<string>) await this._listAllSubDirectoryAsync(tcmRequestContext, directoryPrefixForTag, azureBlobStore).ConfigureAwait(false))
        {
          string tagNameFromPrefix = this.LogStorePathFormatter.GetTestTagNameFromPrefix(logReference.Scope, directoryPath);
          if (!string.IsNullOrWhiteSpace(tagNameFromPrefix))
            uniqueTags.Add(new TestTag()
            {
              Name = tagNameFromPrefix
            });
        }
      }
      IList<TestTag> forBuildOrRelease = (IList<TestTag>) new List<TestTag>((IEnumerable<TestTag>) uniqueTags);
      uniqueTags = (HashSet<TestTag>) null;
      return forBuildOrRelease;
    }

    private async Task<TestTagSummary> _getTestTagRunIdMapForBuildOrRelease(
      TestManagementRequestContext tcmRequestContext,
      TestLogReference logReference,
      ITestLogStore azureBlobStore)
    {
      Dictionary<int, HashSet<TestTag>> runIdTagsNameMap = new Dictionary<int, HashSet<TestTag>>();
      this._getLogStoreOperationContext(tcmRequestContext);
      string directoryPrefixForTag = this.LogStorePathFormatter.GetDirectoryPrefixForTag(logReference);
      if (!string.IsNullOrEmpty(directoryPrefixForTag))
      {
        foreach (string blobName in (IEnumerable<string>) await this._listAllBlobsInDirectoryAsync(tcmRequestContext, directoryPrefixForTag, azureBlobStore).ConfigureAwait(false))
        {
          TestTagReference referenceFromBlobName = this.LogStorePathFormatter.GetTestTagReferenceFromBlobName(logReference.Scope, logReference.Scope == TestLogScope.Build ? logReference.BuildId : logReference.ReleaseId, blobName, logReference.ReleaseEnvId);
          if (!string.IsNullOrEmpty(referenceFromBlobName.TagName) && referenceFromBlobName.RunId > 0)
          {
            if (runIdTagsNameMap.ContainsKey(referenceFromBlobName.RunId))
            {
              if (runIdTagsNameMap[referenceFromBlobName.RunId] == null)
                runIdTagsNameMap[referenceFromBlobName.RunId] = new HashSet<TestTag>();
              runIdTagsNameMap[referenceFromBlobName.RunId].Add(new TestTag()
              {
                Name = referenceFromBlobName.TagName
              });
            }
            else
              runIdTagsNameMap.Add(referenceFromBlobName.RunId, new HashSet<TestTag>()
              {
                new TestTag() { Name = referenceFromBlobName.TagName }
              });
          }
        }
      }
      Dictionary<int, IList<TestTag>> dictionary = new Dictionary<int, IList<TestTag>>();
      foreach (KeyValuePair<int, HashSet<TestTag>> keyValuePair in runIdTagsNameMap)
        dictionary[keyValuePair.Key] = (IList<TestTag>) new List<TestTag>((IEnumerable<TestTag>) keyValuePair.Value);
      TestTagSummary forBuildOrRelease = new TestTagSummary()
      {
        TagsGroupByTestArtifact = (IDictionary<int, IList<TestTag>>) dictionary
      };
      runIdTagsNameMap = (Dictionary<int, HashSet<TestTag>>) null;
      return forBuildOrRelease;
    }

    private async Task<IList<string>> _listAllSubDirectoryAsync(
      TestManagementRequestContext tcmRequestContext,
      string relativeAddress,
      ITestLogStore azureBlobStore)
    {
      int counter = 0;
      List<string> directoryNameList = new List<string>();
      try
      {
        ILogStoreOperationContext logStoreOperationContext = this._getLogStoreOperationContext(tcmRequestContext);
        ILogStoreBlobListingDetails logStoreBlobListingDetails = (ILogStoreBlobListingDetails) new LogStoreBlobListingDetails(false);
        ILogStoreBlobContinuationToken blobContinuationToken = (ILogStoreBlobContinuationToken) new LogStoreBlobContinuationToken((string) null);
        do
        {
          ILogStoreBlobResultSegment blobResultSegment = await azureBlobStore.ListBlobsInDirectoryAsync(tcmRequestContext.RequestContext, relativeAddress, false, logStoreBlobListingDetails, new int?(), blobContinuationToken, logStoreOperationContext, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
          directoryNameList.AddRange((IEnumerable<string>) blobResultSegment.GetDirectoryPrefixList());
          blobContinuationToken = (ILogStoreBlobContinuationToken) blobResultSegment.GetLogStoreBlobContinuationToken();
          ++counter;
        }
        while (blobContinuationToken.GetBlobContinuationToken() != null);
        logStoreOperationContext = (ILogStoreOperationContext) null;
        logStoreBlobListingDetails = (ILogStoreBlobListingDetails) null;
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "_listAllSubDirectoryAsync - Unable to fetch the Directory list {0}. Exception Hit: {1}", (object) relativeAddress, (object) ex.Message);
      }
      tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Verbose, "TestManagement", "LogStorage", "_listAllSubDirectoryAsync - directory {0}, Number of azure calls {1}", (object) relativeAddress, (object) counter);
      IList<string> stringList = (IList<string>) directoryNameList;
      directoryNameList = (List<string>) null;
      return stringList;
    }

    private async Task<IList<string>> _listAllBlobsInDirectoryAsync(
      TestManagementRequestContext tcmRequestContext,
      string relativeAddress,
      ITestLogStore azureBlobStore)
    {
      int counter = 0;
      List<string> blobNameList = new List<string>();
      try
      {
        ILogStoreOperationContext logStoreOperationContext = this._getLogStoreOperationContext(tcmRequestContext);
        ILogStoreBlobListingDetails logStoreBlobListingDetails = (ILogStoreBlobListingDetails) new LogStoreBlobListingDetails(false);
        ILogStoreBlobContinuationToken blobContinuationToken = (ILogStoreBlobContinuationToken) new LogStoreBlobContinuationToken((string) null);
        do
        {
          ILogStoreBlobResultSegment blobResultSegment = await azureBlobStore.ListBlobsInDirectoryAsync(tcmRequestContext.RequestContext, relativeAddress, true, logStoreBlobListingDetails, new int?(), blobContinuationToken, logStoreOperationContext, tcmRequestContext.RequestContext.CancellationToken).ConfigureAwait(false);
          blobNameList.AddRange((IEnumerable<string>) blobResultSegment.GetBlobNameList());
          blobContinuationToken = (ILogStoreBlobContinuationToken) blobResultSegment.GetLogStoreBlobContinuationToken();
        }
        while (blobContinuationToken.GetBlobContinuationToken() != null);
        logStoreOperationContext = (ILogStoreOperationContext) null;
        logStoreBlobListingDetails = (ILogStoreBlobListingDetails) null;
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "_listAllBlobsInDirectoryAsync - Unable to fetch the bloblist {0}. Exception Hit: {1}", (object) relativeAddress, (object) ex.Message);
      }
      tcmRequestContext.RequestContext.Trace(1015676, TraceLevel.Verbose, "TestManagement", "LogStorage", "_listAllBlobsInDirectoryAsync - directory {0}, Number of azure calls {1}", (object) relativeAddress, (object) counter);
      IList<string> stringList = (IList<string>) blobNameList;
      blobNameList = (List<string>) null;
      return stringList;
    }

    private Guid GetProjectId(TestManagementRequestContext tcmRequestContext, int dataspaceId)
    {
      Guid empty = Guid.Empty;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(tcmRequestContext))
        return managementDatabase.GetDataspaceIdentifier(dataspaceId);
    }

    private async Task<TestLogStatusCode> _createEmptyBlobAsync(
      TestManagementRequestContext tcmRequestContext,
      ILogStoreOperationContext logStoreOperationContext,
      TestLogReference logReference,
      ITestLogStore azureBlobStore)
    {
      try
      {
        return await azureBlobStore.CreateBlobAsync(tcmRequestContext.RequestContext, logStoreOperationContext, this.GetFilePath(logReference), (Stream) new MemoryStream(), tcmRequestContext.RequestContext.CancellationToken, (IDictionary<string, string>) null) ? TestLogStatusCode.Success : TestLogStatusCode.FileAlreadyExists;
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "_createBlobAsync - CreateBlobAsync failed, ExceptionHit: {0} ", (object) ex.Message);
        return TestLogStatusCode.Failed;
      }
    }

    private IList<TestLogSecureObject> ConvertToTestLogSecureObject(
      Guid projectId,
      IList<TestLog> testLogResults)
    {
      IList<TestLogSecureObject> testLogSecureObject1 = (IList<TestLogSecureObject>) new List<TestLogSecureObject>();
      foreach (TestLog testLogResult in (IEnumerable<TestLog>) testLogResults)
      {
        TestLogSecureObject testLogSecureObject2 = new TestLogSecureObject(projectId, testLogResult);
        testLogSecureObject1.Add(testLogSecureObject2);
      }
      return testLogSecureObject1;
    }

    private void CheckForViewTestResultsPermission(
      TestManagementRequestContext tcmRequestContext,
      string projectUri)
    {
      if (!tcmRequestContext.SecurityManager.HasViewTestResultsPermission(tcmRequestContext, projectUri))
        throw new AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
    }

    private void _logDeletedContainers(
      TestManagementRequestContext tcmRequestContext,
      List<Tuple<int, ContainerScopeDetails>> deletedList)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(ServerResources.LogStoreContainerDeletedMessage);
      foreach (Tuple<int, ContainerScopeDetails> deleted in deletedList)
      {
        int num = 0;
        if (deleted.Item2.ContainerScope == ContainerScope.Build)
          num = deleted.Item2.BuildId;
        else if (deleted.Item2.ContainerScope == ContainerScope.Run)
          num = deleted.Item2.RunIdId;
        string str = string.Format(ServerResources.LogStoreContainerDeleted, (object) deleted.Item1, (object) deleted.Item2.ContainerScope.ToString(), (object) num.ToString());
        stringBuilder.Append(str);
      }
      tcmRequestContext.RequestContext.Trace(1015677, TraceLevel.Info, "TestManagement", "LogStorage", stringBuilder.ToString());
    }

    internal ILogStorePathFormatter LogStorePathFormatter
    {
      get
      {
        if (this._logStorePathFormatter == null)
          this._logStorePathFormatter = (ILogStorePathFormatter) new Microsoft.TeamFoundation.TestManagement.Server.LogStorePathFormatter();
        return this._logStorePathFormatter;
      }
      set => this._logStorePathFormatter = value;
    }
  }
}
