// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Constants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data
{
  public static class Constants
  {
    public const string ArtifactSourceVersion = "version";
    public const string PreviousArtifactSourceVersion = "previousVersion";
    public const string ArtifactDetailsFileNameSuffix = "artifactDetailsFileNameSuffix";
    public const int DefaultReleaseCount = 10;
    public const string ValidArtifactNameCharacters = "^[a-zA-Z0-9\\*_.-]+$";
    public const string WildCharForArtifactName = "*";
    public const string ReleaseManagementDataspaceCategory = "ReleaseManagement";
    public const string ReleaseObject = "Release";
    public const int ReleaseNameFormatMaxLength = 256;
    public const int ReleaseNameMaxLength = 256;
    public const string BranchesId = "branches";
    public const int ReleaseDescriptionMaxLength = 4000;
    public const int ReleaseDefinitionDescriptionMaxLength = 4000;
    public const int MaxNVarCharSize = -1;
    public const int NVarCharColumnLength = 4000;
    public const int MaxResourceIdLength = 256;
    public const string CommitsId = "commits";
    public const int DeployPhaseNameMaxLength = 256;
    public const int DeployPhaseRefNameMaxLength = 256;
    public const int MaxGateCount = 10;
    public const string ArtifactSourceVersionUrl = "artifactSourceVersionUrl";
    public const string ArtifactSourceDefinitionUrl = "artifactSourceDefinitionUrl";
    public const string ArtifactVersionCreatedOn = "artifactVersionCreatedOn";
    public const int ContainerImageTagFilterRegexTimeout = 1;
    public const string ReleaseSettingsMoniker = "ReleaseManagement.ReleaseSettings";
    public const string RetentionSettingsKey = "RetentionSettings";
    public const string ComplianceSettingsKey = "ComplianceSettings";
    public const string ReleaseSettingsArtifactGuid = "4FCE6B7C-0B39-4D75-BC07-8F194F70F663";
    public const int RetentionDefaultDaysToKeep = 30;
    public const int RetentionDefaultReleasesToKeep = 3;
    public const bool RetentionDefaultRetainBuild = true;
    public const int RetentionMaximumDaysToKeep = 365;
    public const int RetentionMaximumReleasesToKeep = 25;
    public const int DaysToKeepDeletedReleases = 14;
    public const int RetentionMaximumDaysToKeepForVsts = 730;
    public const int RetentionExtendedMaximumDaysToKeepForVsts = 2555;
    public const int RetentionMaximumReleasesToKeepForVsts = 25;
    public const string RetentionExtendedMaximumDaysToKeepRegistrySetting = "/Service/ReleaseManagement/RetentionExtendedMaximumDaysToKeepForVsts";
    public const int DaysToKeepDeletedReleasesForVsts = 14;
    public const int DefaultReleaseDeployPhaseId = 0;
    public const int DefaultAttemptId = 0;
    public const string InstructionsKey = "instructions";
    public const string ManualInterventionEmailRecipientsKey = "emailRecipients";
    public const string ManualInterventionResumeOnTimeoutKey = "onTimeout";
    public const string ManualInterventionOnTimeoutResume = "Resume";
    public const string ManualInterventionOnTimeoutReject = "Reject";
    public const int MaxFolderPathLength = 400;
    public const int MaxFolderNameLength = 248;
    public const string ManualInterventionTaskId = "BCB64569-D51A-4AF0-9C01-EA5D05B3B622";
    public const string DelayTaskId = "28782B92-5E8E-4458-9751-A71CD1492BAE";
    public const int ManualInterventionMaxTimeoutInMinutes = 86400;
    public const bool BlockReleaseDefinitionSaveIfSecretPresentDefaultValue = false;
    public const string CustomTemplateCategory = "Custom";
    public const string DeploymentTemplateCategory = "Deployment";
    public const int ArtifactDetailsVersion = 1;
    public const string PreviousArtifactVersion = "PreviousArtifactVersion";
    public const string ArtifactDetailsReference = "ArtifactDetailsReference";
    public const string TriggeringArtifactAlias = "TriggeringArtifactAlias";
    public const string Alias = "Alias";
    public const string IsTriggeringArtifact = "IsTriggeringArtifact";
    public const string RedeployTrigger = "RedeployTrigger";
    public const string RollbackTrigger = "RollbackTrigger";
    public const int JobScopeSettingCacheMaxElements = 10000;
    public static readonly TimeSpan JobScopeSettingCacheCleanupInterval = TimeSpan.FromHours(24.0);
    public static readonly TimeSpan JobScopeSettingCacheExpirationInterval = TimeSpan.FromHours(24.0);
  }
}
