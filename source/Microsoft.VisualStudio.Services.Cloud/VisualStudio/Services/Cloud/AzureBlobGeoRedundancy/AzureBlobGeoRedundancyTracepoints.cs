// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyTracepoints
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancyTracepoints
  {
    public const int EnqueueMessage = 15300000;
    public const int EnqueueMessageFailure = 15300001;
    public const int RegistrySettingsChanged = 15300002;
    public const int InvalidRegistryValue = 15300003;
    public const int BeginCopyContainer = 15301000;
    public const int FinishedCopyContainer = 15301001;
    public const int FailedCopyContainer = 15301002;
    public const int ExceptionDuringCopyContainer = 15301003;
    public const int ResumeFromCheckpoint = 15301004;
    public const int FailedBlobCopy = 15301005;
    public const int IncompleteSnapshot = 15301006;
    public const int CheckpointsDisabled = 15301007;
    public const int FailedProgressUpdate = 15301008;
    public const int FailedProcessingCreateBlobMessage = 15302000;
    public const int FailedProcessingDeleteBlobMessage = 15302001;
    public const int FailedProcessingCreateContainerMessage = 15302003;
    public const int FailedProcessingDeleteContainerMessage = 15302004;
    public const int FailedUpdatingVisibilityTimeout = 15302005;
    public const int FailedToProcessMessage = 15302006;
    public const int MessageExpired = 15302007;
    public const int ExceededMaxRuntime = 15302008;
    public const int ProcessingMessage = 15302009;
    public const int MessageSuccessful = 15302010;
    public const int MessageBlockedOrFailed = 15302011;
    public const int MessageNotFound = 15302012;
    public const int StartingBlobProvider = 15303000;
    public const int ExceptionStartingBlobProvider = 15303001;
    public const int StorageAccountForLookupKeyNotFound = 15303002;
    public const int ConfiguredSecondaryBlobProvider = 15303003;
    public const int SecondaryIsSameAsPrimaryError = 15303004;
    public const int MissingInfoForSecondaryBlobProvider = 15303005;
    public const int StrongBoxNotificationRegistered = 15303010;
    public const int NoSecondaryStorageAccount = 15304000;
    public const int NoQueue = 15305000;
    public const int UnableToReadProperties = 15306000;
    public const int DualWriteBlockIdError = 15306001;
    public const int DualWriteBlockIdRecoveryFailure = 15306002;
    public const int DualWriteBlockIdRecoverySuccessful = 15306003;
    public const int ConsistencyProgress = 15307000;
    public const int InconsistencyDetected = 15307001;
    public const int NoMD5Available = 15307002;
    public const int ConsumerCompleted = 15307003;
    public const int EnumerationCompleted = 15307004;
    public const int RepairOperationFailed = 15307005;
    public const int ContainerDoesNotExist = 15307006;
    public const int DeletingBlob = 15307007;
    public const int SuccessfullySetupSwappableBlobProvider = 15308000;
    public const int NoSecondaryFoundForSwappableBlobProvider = 15308001;
    public const int UnableToUpdateProvider = 15308002;
    public const int ExceptionInExtension = 15309000;
  }
}
