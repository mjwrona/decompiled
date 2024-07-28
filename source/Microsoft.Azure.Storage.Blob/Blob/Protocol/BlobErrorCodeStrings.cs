// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.BlobErrorCodeStrings
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public static class BlobErrorCodeStrings
  {
    public static readonly string InvalidAppendCondition = "AppendPositionConditionNotMet";
    public static readonly string InvalidMaxBlobSizeCondition = "MaxBlobSizeConditionNotMet";
    public static readonly string InvalidBlobOrBlock = nameof (InvalidBlobOrBlock);
    public static readonly string InvalidBlockId = nameof (InvalidBlockId);
    public static readonly string InvalidBlockList = nameof (InvalidBlockList);
    public static readonly string ContainerNotFound = nameof (ContainerNotFound);
    public static readonly string BlobNotFound = nameof (BlobNotFound);
    public static readonly string ContainerAlreadyExists = nameof (ContainerAlreadyExists);
    public static readonly string ContainerDisabled = nameof (ContainerDisabled);
    public static readonly string ContainerBeingDeleted = nameof (ContainerBeingDeleted);
    public static readonly string BlobAlreadyExists = nameof (BlobAlreadyExists);
    public static readonly string LeaseNotPresentWithBlobOperation = nameof (LeaseNotPresentWithBlobOperation);
    public static readonly string LeaseNotPresentWithContainerOperation = nameof (LeaseNotPresentWithContainerOperation);
    public static readonly string LeaseLost = nameof (LeaseLost);
    public static readonly string LeaseIdMismatchWithBlobOperation = nameof (LeaseIdMismatchWithBlobOperation);
    public static readonly string LeaseIdMismatchWithContainerOperation = nameof (LeaseIdMismatchWithContainerOperation);
    public static readonly string LeaseIdMissing = nameof (LeaseIdMissing);
    public static readonly string LeaseNotPresentWithLeaseOperation = nameof (LeaseNotPresentWithLeaseOperation);
    public static readonly string LeaseIdMismatchWithLeaseOperation = nameof (LeaseIdMismatchWithLeaseOperation);
    public static readonly string LeaseAlreadyPresent = nameof (LeaseAlreadyPresent);
    public static readonly string LeaseAlreadyBroken = nameof (LeaseAlreadyBroken);
    public static readonly string LeaseIsBrokenAndCannotBeRenewed = nameof (LeaseIsBrokenAndCannotBeRenewed);
    public static readonly string LeaseIsBreakingAndCannotBeAcquired = nameof (LeaseIsBreakingAndCannotBeAcquired);
    public static readonly string LeaseIsBreakingAndCannotBeChanged = nameof (LeaseIsBreakingAndCannotBeChanged);
    public static readonly string InfiniteLeaseDurationRequired = nameof (InfiniteLeaseDurationRequired);
    public static readonly string SnapshotsPresent = nameof (SnapshotsPresent);
    public static readonly string InvalidBlobType = nameof (InvalidBlobType);
    public static readonly string InvalidVersionForPageBlobOperation = nameof (InvalidVersionForPageBlobOperation);
    public static readonly string InvalidPageRange = nameof (InvalidPageRange);
    public static readonly string SequenceNumberConditionNotMet = nameof (SequenceNumberConditionNotMet);
    public static readonly string SequenceNumberIncrementTooLarge = nameof (SequenceNumberIncrementTooLarge);
    public static readonly string SourceConditionNotMet = nameof (SourceConditionNotMet);
    public static readonly string TargetConditionNotMet = nameof (TargetConditionNotMet);
    public static readonly string CopyAcrossAccountsNotSupported = nameof (CopyAcrossAccountsNotSupported);
    public static readonly string CannotVerifyCopySource = nameof (CannotVerifyCopySource);
    public static readonly string PendingCopyOperation = nameof (PendingCopyOperation);
    public static readonly string NoPendingCopyOperation = nameof (NoPendingCopyOperation);
    public static readonly string CopyIdMismatch = nameof (CopyIdMismatch);
  }
}
