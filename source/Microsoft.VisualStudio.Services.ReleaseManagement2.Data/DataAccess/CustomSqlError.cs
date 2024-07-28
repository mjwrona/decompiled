// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class CustomSqlError
  {
    public const int GenericDatabaseUpdateFailure = 700000;
    public const int TransactionRequired = 700001;
    public const int MoreThanOneRecordAdditionNotAllowed = 700002;
    public const int ObjectDoesNotExist = 700003;
    public const int ReleaseDefinitionNotFound = 700004;
    public const int ReleaseDefinitionAlreadyExists = 700005;
    public const int PlanIdAlreadyExists = 700006;
    public const int ArtifactDefinitionAlreadyExists = 700007;
    public const int ReleaseWithGivenNameAlreadyExists = 700008;
    public const int ArtifactDefinitionDoesNotExist = 700009;
    public const int InvalidApprovalUpdate = 700010;
    public const int ReleaseNotFound = 700011;
    public const int ReleaseEnvironmentNotFound = 700012;
    public const int QueueAlreadyExists = 700013;
    public const int QueueNotFound = 700014;
    public const int DefinitionEnvironmentTemplateAlreadyExists = 700015;
    public const int DefinitionEnvironmentTemplateNotFound = 700016;
    public const int ReleaseDeletionNotAllowedDueToCurrentlyActiveOnEnvironments = 700017;
    public const int InvalidReleaseStatusUpdate = 700018;
    public const int ReleaseStepNotFound = 700019;
    public const int ReleasesNotFound = 700020;
    public const int DraftReleaseCannotBeStarted = 700021;
    public const int ReleaseDefinitionRevisionAlreadyExists = 700022;
    public const int InvalidEnvironmentStatusUpdate = 700023;
    public const int QueueReleaseNotAllowed = 700024;
    public const int DuplicateStepsInsertion = 700025;
    public const int InvalidData = 700026;
    public const int ReleaseNotInProgressState = 700027;
    public const int ReleaseEnvironmentNotInProgressState = 700028;
    public const int ReleaseDeletionNotAllowedForRetainedReleases = 700029;
    public const int ReleaseDefinitionAlreadyUpdated = 700030;
    public const int ReleaseStepByDefinitionEnvironmentIdNotFound = 700031;
    public const int DefinitionEnvironmentNotFound = 700032;
    public const int ReleaseDefinitionSnapshotFileIdUpdateFailed = 700033;
    public const int DeploymentStatusAlreadyUpdated = 700034;
    public const int DeploymentOperationStatusAlreadyUpdated = 700035;
    public const int ManualInterventionNotFound = 700036;
    public const int ManualInterventionStatusIsAlreadyUpdated = 700037;
    public const int FolderExists = 700038;
    public const int FolderNotFound = 700039;
    public const int FolderParentNotFound = 700040;
    public const int ReleaseDeletionNotAllowedDueToPendingOnEnvironments = 700041;
    public const int DeploymentUpdateNotAllowed = 700042;
    public const int ReleaseDefinitionDeletionNotAllowed = 700043;
    public const int DeletedReleaseDefinitionNotFound = 700044;
    public const int DeploymentResourceAlreadyExists = 700045;
    public const int DeploymentResourceNotFound = 700046;
    public const int DeletedDefinitionEnvironmentTemplateNotFound = 700047;
    public const int NewDeploymentAlreadyStarted = 700048;
    public const int InvalidReleaseEnvironmentStepStatus = 700049;
    public const int IgnoreGateUpdateFailed = 700050;
    public const int ReleaseWebHookDoesNotExist = 700051;
    public const int ReleaseWebHookAlreadyExists = 700052;
    public const int ReleaseHistoryEnvironmentIdAttemptMismatch = 700053;
    public const int ReleaseDefinitionSnapshotRevisionNotMatched = 700054;
    public const int ReleaseDefinitionDisabled = 700055;
  }
}
