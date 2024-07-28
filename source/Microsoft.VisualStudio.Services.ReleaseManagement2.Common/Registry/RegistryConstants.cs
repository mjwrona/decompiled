// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Registry.RegistryConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Registry
{
  public static class RegistryConstants
  {
    public const string RegistrySettingsPath = "/Service/ReleaseManagement/Settings/";
    public const string RetentionPolicyPath = "/Service/ReleaseManagement/Settings/Retention/";
    public const string RetentionCurrentProjectToProcessForReleasesSoftDelete = "/Service/ReleaseManagement/Settings/Retention/CurrentProjectToProcessForReleasesSoftDelete";
    public const string RetentionCurrentProjectToProcessForReleasesHardDelete = "/Service/ReleaseManagement/Settings/Retention/CurrentProjectToProcessForReleasesHardDelete";
    public const string RetentionCurrentProjectToProcessForReleaseDefinitionsHardDelete = "/Service/ReleaseManagement/Settings/Retention/CurrentProjectToProcessForReleaseDefinitionsHardDelete";
    public const string RetentionNextDefinitionToProcess = "/Service/ReleaseManagement/Settings/Retention/NextDefinitionToProcess";
    public const string RetentionMaxReleaseSoftDeletesPerJobRun = "/Service/ReleaseManagement/Settings/Retention/MaxReleaseSoftDeletesPerJobRun";
    public const string RetentionMaxHardDeletesPerJobRun = "/Service/ReleaseManagement/Settings/Retention/MaxHardDeletesPerJobRun";
    public const string RetentionMaxHardDeletesBatchSize = "/Service/ReleaseManagement/Settings/Retention/MaxHardDeletesBatchSize";
    public const string RetentionMaxDefinitionsHardDeletesPerJobRun = "/Service/ReleaseManagement/Settings/Retention/MaxDefinitionsHardDeletesPerJobRun";
    public const string RetentionMaxDefinitionsBatchSize = "/Service/ReleaseManagement/Settings/Retention/MaxDefinitionsBatchSize";
    public const string RetentionDurationInDaysToHardDelete = "/Service/ReleaseManagement/Settings/Retention/MaximumPolicy/DurationInDaysToHardDelete";
    public const string RetentionPolicyDefaultPolicyPath = "/Service/ReleaseManagement/Settings/Retention/DefaultPolicy/";
    public const string DefaultRetentionPolicy = "/Service/ReleaseManagement/Settings/Retention/DefaultPolicy/DefaultRetentionPolicy";
    public const string RetentionPolicyDefaultDaysToKeep = "/Service/ReleaseManagement/Settings/Retention/DefaultPolicy/DaysToKeep";
    public const string RetentionPolicyDefaultReleasesToKeep = "/Service/ReleaseManagement/Settings/Retention/DefaultPolicy/ReleasesToKeep";
    public const string RetentionPolicyDefaultRetainBuild = "/Service/ReleaseManagement/Settings/Retention/DefaultPolicy/RetainBuild";
    public const string RetentionPolicyMaximumPolicyPath = "/Service/ReleaseManagement/Settings/Retention/MaximumPolicy/";
    public const string MaximumRetentionPolicy = "/Service/ReleaseManagement/Settings/Retention/MaximumPolicy/MaximumRetentionPolicy";
    public const string RetentionPolicyMaximumDaysToKeep = "/Service/ReleaseManagement/Settings/Retention/MaximumPolicy/DaysToKeep";
    public const string RetentionPolicyMaximumReleasesToKeep = "/Service/ReleaseManagement/Settings/Retention/MaximumPolicy/ReleasesToKeep";
    public const string RetentionPolicySoftDeleteTestEnabled = "/Service/ReleaseManagement/Settings/Retention/SoftDeleteTestEnabled";
    public const string RetentionPolicyHardDeleteTestEnabled = "/Service/ReleaseManagement/Settings/Retention/HardDeleteTestEnabled";
    public const string UpdateRetainBuildBatchSize = "/Service/ReleaseManagement/Settings/Retention/UpdateRetainBuildBatchSize";
    public const string RecoverAssetPath = "/Service/ReleaseManagement/Settings/Recover/";
    public const string RecoverReleasesPath = "/Service/ReleaseManagement/Settings/Recover/Releases/";
    public const string DaysToCheckPastReleasesKey = "/Service/ReleaseManagement/Settings/Recover/Releases/PastDaysToCheck";
    public const string RevalidateIdentityAuthorizationTimeExpirationInSecondsPath = "/Service/ReleaseManagement/Settings/RevalidateIdentityAuthorizationTimeExpirationInSeconds";
    public const string MaximumBuildDefinitionsToFetchPath = "/Service/ReleaseManagement/Settings/MaximumBuildDefinitionsToFetch";
    public const string ActionRequestContinuationToken = "/Service/ReleaseManagement/Settings/ActionRequestContinuationToken";
    public const string JobPriorityAboveNormal = "/Service/ReleaseManagement/Settings/JobPriorityAboveNormal/";
    public const string StartEnvironmentJobPriority = "/Service/ReleaseManagement/Settings/JobPriorityAboveNormal/StartEnvironmentJob";
    public const string ScheduleReleaseJobPriority = "/Service/ReleaseManagement/Settings/JobPriorityAboveNormal/ScheduleReleaseJob";
    public const string MinimumSamplingIntervalInMinutes = "/Service/ReleaseManagement/Settings/DeploymentGateReevaluationTimeout";
    public const string LandingUIEnvironmentsThresholdToSkipReleasesFetch = "/Service/ReleaseManagement/Settings/LandingUI/EnvironmentsThreshold";
    public const string LandingUIReleasesBatchCount = "/Service/ReleaseManagement/Settings/LandingUI/ReleasesBatchCount";
    public const string RunDeploymentPlanMaxAttemptCountRegistryKey = "/Service/ReleaseManagement/Settings/RunDeploymentPlan/MaxAttemptCount";
    public const string OrgEnforceJobScopeKey = "/Service/ReleaseManagement/Settings/OrgSettings/EnforceJobAuthScope";
    public const string EnforceJobScopeKey = "/Service/ReleaseManagement/Settings/EnforceJobAuthScope/{0}";
  }
}
