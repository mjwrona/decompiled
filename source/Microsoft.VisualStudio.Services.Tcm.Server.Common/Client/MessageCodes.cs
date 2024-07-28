// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Client.MessageCodes
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Client
{
  public enum MessageCodes
  {
    Success = 0,
    GenericError = 550001, // 0x00086471
    ObjectNotFound = 550002, // 0x00086472
    ObjectInUse = 550003, // 0x00086473
    ObjectAlreadyUpdated = 550004, // 0x00086474
    IllegalStateTransition = 550005, // 0x00086475
    CannotMoveChildBelow = 550006, // 0x00086476
    DuplicateSuiteEntry = 550007, // 0x00086477
    TestObjectAlreadyExists = 550008, // 0x00086478
    StartDateAfterEndDate = 550009, // 0x00086479
    ParentSuiteNotFound = 550010, // 0x0008647A
    SuiteDepthOverLimit = 550011, // 0x0008647B
    CannotMoveSuiteEntriesAcrossPlans = 550012, // 0x0008647C
    InvalidArgument = 550013, // 0x0008647D
    SourceFileNotFound = 550014, // 0x0008647E
    OneTestId = 550015, // 0x0008647F
    BuildNotFound = 550016, // 0x00086480
    ObjectNotFound_StringIdentifier = 550017, // 0x00086481
    InvalidOperationId = 550018, // 0x00086482
    SourceAndDestinationCannotBeInSamePlan = 550019, // 0x00086483
    SuitesNotInSamePlan = 550020, // 0x00086484
    ConflictingOperation = 550021, // 0x00086485
    DefaultFailureTypeCreationError = 550022, // 0x00086486
    FailureTypeIdExhausted = 550023, // 0x00086487
    TestArtifactExistsInOfflineMode = 550024, // 0x00086488
    EnvironmentAlreadyInUse = 550025, // 0x00086489
    TestRunAlreadyCanceled = 550026, // 0x0008648A
    CannotCancelACompletedTestRun = 550027, // 0x0008648B
    ChartDataError = 550028, // 0x0008648C
    TestConfigurationInactive = 550029, // 0x0008648D
    MaxTestFlakinessBranchLimitExceeded = 550030, // 0x0008648E
    RunSummaryNotComputedException = 550031, // 0x0008648F
    TestCasesNotInsameSuite = 550032, // 0x00086490
  }
}
