// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogStoreConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class TestLogStoreConstants
  {
    public const string TestLogStorageAccountConnectionString = "TestLogStorageAccountConnectionString";
    public const string TestLogStoreStorageAccountPrefix = "TcmLogStoreConnectionString";
    public const string OmegaTestLogStoreStorageAccountPrefix = "OmegaTcmLogStoreConnectionString";
    public const int DefaultTcmLogStoreStorageAccountCapacity = 2;
    public const int TcmLogStoreSASValidityInHours = 1;
    public const int DefaultTcmLogStoreStorageAccountLimitPerProject = 2;
    public const int TcmLogStoreSASRenewTimeInHour = 1;
    public const int ServerTimeoutInSeconds = 60;
    public const int MaximumExecutionTimeInSeconds = 900;
    public const int TcmLogStoreSASRevokeJobDelayInSeconds = 10;
    public const int TcmLogStoreDefaultBlobResultsCount = 1000;
    public const int TcmLogStoreMaxBlobResultsCount = 4000;
    public const int ParallelOperationsCount = 8;
    public const int TcmLogStoreStreamSizeLimitInBytes = 100000000;
    public const bool TcmLogStoreEnableContentMD5Validation = false;
    public const string TcmLogStoreAFDHostSuffixDefault = "vstmrblob.vsassets.io";
    public const string TcmLogStoreAFDTestHostSuffixDefault = "vstmrblob.vsts.io";
    public const string TcmLogStoreAFDBlobStorageHostSuffix = "blob.core.windows.net";
    public const int TcmLogStoreCleanupStorageAccountId = 0;
    public const int TcmLogStoreCleanupMaxContainersToProcess = 5000;
    public const int TcmLogStoreCleanupNoOfDays = 30;
    public const int TcmLogStoreCleanupMaxBlobCalls = 10;
    public const int TcmLogStoreMigrationMaxContainersToProcess = 5000;
    public const int TcmLogStoreMigrationMaxBlobCalls = 10;
    public const int TcmLogStoreMigrationContainerRetryCount = 5;
    public const int TcmLogStoreMigrationMaxDegreeParallelism = 8;
    public const string TcmLogStoreJobAgentPrefix = "TCMLogStoreJobAgent";
    public const string AzureDevOpsDomain = "https://dev.azure.com";
    public const string AzureDevOpsDevfabricDomain = "https://codedev.ms";
    public const string AzureDevOpsAppFabricDomain = "https://codeapp.ms";
    public const int TcmLogStoreMaxAttemptsRetry = 10;
    public const int TcmLogStoreDeltaBackOffInSeconds = 3;
    public const int LogStoreRunLevelContainerDeletionJobMaxExecTimeInSec = 10800;
    public const int LogStoreRunLevelContainerDeletionJobBatchSize = 10000;
    public const int TcmLogStoreAttachmentIdMappingTableInsertBatchSize = 1000;
  }
}
