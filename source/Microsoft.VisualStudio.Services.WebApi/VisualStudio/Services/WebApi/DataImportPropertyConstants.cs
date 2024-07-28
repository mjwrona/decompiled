// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.DataImportPropertyConstants
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class DataImportPropertyConstants
  {
    public const string Prefix = "DataImport.";
    public const string InternalPrefix = "DataImport.Internal.";
    public const string ComputedPrefix = "DataImport.Computed.";
    public const string AccountOwner = "DataImport.AccountOwner";
    public const string AADTenantName = "DataImport.AADTenantName";
    public const string AccountName = "DataImport.AccountName";
    public const string AccountRegion = "DataImport.AccountRegion";
    public const string AccountScaleUnit = "DataImport.AccountScaleUnit";
    public const string SourceDacpacLocation = "DataImport.SourceDacpacLocation";
    public const string HostToMovePostImport = "DataImport.HostToMovePostImport";
    public const string TargetDatabaseDowngradeSize = "DataImport.TargetDatabaseDowngradeSize";
    public const string UseDevOpsDomainUrls = "DataImport.Internal.UseCodexDomainUrls";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string IdentityImportMapping = "DataImport.IdentityImportMapping";
    public const string SkipDataImportFileContent = "DataImport.SkipFileContent";
    public const string SkipDataImportValidation = "DataImport.SkipValidation";
    public const string SkipDataImportWIT = "DataImport.SkipDataImportWIT";
    public const string SkipValidationTfvcInFileService = "DataImport.Internal.SkipValidationTfvcInFileService";
    public const string TryMapADGroupsToAADGroupsAutomatically = "DataImport.TryMapADGroupsToAADGroupsAutomatically";
    public const string SourceDatabase = "DataImport.SourceDatabase";
    public const string NeighborHostId = "DataImport.NeighborHostId";
    public const string CollectionCreationJobId = "DataImport.CollectionCreationJobId";
    public const string DatabaseImportJobId = "DataImport.DatabaseImportJobId";
    public const string ActivateImportJobId = "DataImport.ActivateImportJobId";
    public const string ObtainDatabaHoldJobId = "DataImport.ObtainDatabaHoldJobId";
    public const string HostMoveCollectionJobId = "DataImport.HostMoveCollectionJobId";
    public const string OnlinePostHostUpgradeJobId = "DataImport.OnlinePostHostUpgradeJobId";
    public const string StopHostAfterUpgradeJobId = "DataImport.StopHostAfterUpgradeJobId";
    public const string RemoveImportJobId = "DataImport.RemoveImportJobId";
    public const string DehydrateJobId = "DataImport.DehydrateJobId";
    public const string OverrideServiceInstanceIds = "DataImport.OverrideServiceInstanceIds";
    public const string IgnoreImportStatus = "DataImport.IgnoreImportStatus";
    public const string KeepRegistryData = "DataImport.KeepRegistryData";
    public const string ImportSourceType = "DataImport.ImportSourceType";
    public const string RunType = "DataImport.RunType";
    public const string ImportOwner = "DataImport.ImportOwner";
    public const string OrganizationName = "DataImport.OrganizationName";
    public const string CollectionName = "DataImport.CollectionName";
    public const string SourceCollectionId = "DataImport.SourceCollectionId";
    public const string GlobalCollectionId = "DataImport.GlobalCollectionId";
    public const string PreferredRegion = "DataImport.PreferredRegion";
    public const string OwnerId = "DataImport.OwnerId";
    public const string ProcessMode = "DataImport.ProcessMode";
    public const string IdentitySidsToImport = "DataImport.IdentitySidsToImport";
    public const string PermitForNotRules = "DataImport.PermitForNotRules";
    public const string SoftWarningVersionMissmatchInTfsMigrator = "DataImport.Internal.SoftWarningVersionMissmatchInTfsMigrator";
    public const string EnableCommerce = "DataImport.Internal.EnableCommerce";
    public const string DisableLicensingCallsToCommerceV1 = "DataImport.Internal.DisableLicensingCallsToCommerceV1";
    public const string EnableLicensingDataImportStepPerformer = "DataImport.Internal.EnableLicensingDataImportStepPerformer";
    public const string EnableAzCommDataImportStepPerformer = "DataImport.Internal.EnableAzCommDataImportStepPerformer";
    public const string DisableLicensingIdentityStepPerformer = "DataImport.Internal.DisableLicensingIdentityStepPerformer";
    public const string SkipLicenseCount = "DataImport.Internal.SkipLicenseCount";
    public const string SkipCopyTcmDataToTfs = "DataImport.Internal.SkipCopyTcmDataToTfs";
    public const string KnownErrorPrefix = "DataImport.Internal.KnownError.";
    public const string EnableClearProcessIdFromProject = "DataImport.Internal.EnableClearProcessIdFromProject";
    public const string TargetDatabaseServiceObjective = "DataImport.Internal.TargetDatabaseServiceObjective";
    public const string DacPacQueryPropertiesTimeout = "DataImport.Internal.DacPacQueryPropertiesTimeout";
    public const string DacPacDeployTimeout = "DataImport.Internal.DacPacDeployTimeout";
    public const string FirewallWaitTime = "DataImport.Internal.FirewallWaitTime";
    public const string ImportPackage = "DataImport.ImportPackage";
    public const string SqlPackageVersion = "DataImport.SqlPackageVersion";
    public const string DatabaseTotalSize = "DataImport.DatabaseTotalSize";
    public const string DatabaseBlobSize = "DataImport.DatabaseBlobSize";
    public const string DatabaseTableSize = "DataImport.DatabaseTableSize";
    public const string ActiveUserCount = "DataImport.ActiveUserCount";
    public const string TfsVersion = "DataImport.TfsVersion";
    public const string Collation = "DataImport.Collation";
    public const string CommandExecutionTime = "DataImport.CommandExecutionTime";
    public const string CommandExecutionCount = "DataImport.CommandExecutionCount";
    public const string Force = "DataImport.Force";
    public const string TenantId = "DataImport.TenantId";
    public const string ServicesToInclude = "DataImport.ServicesToInclude";
    public const string ValidationChecksum = "DataImport.ValidationChecksum";
    public const string FieldMetaDataValidation = "DataImport.Internal.FieldMetaDataValidation";
    public const string MaxWorkItemTypesPerProcess = "DataImport.Internal.MaxWorkItemTypesPerProcess";
    public const string MaxCustomWorkItemTypesPerProcess = "DataImport.Internal.MaxCustomWorkItemTypesPerProcess";
    public const string MaxFieldsPerCollection = "DataImport.Internal.MaxFieldsPerCollection";
    public const string MaxFieldsPerProcessTemplate = "DataImport.Internal.MaxFieldsPerProcessTemplate";
    public const string MaxCategoriesPerProcess = "DataImport.Internal.MaxCategoriesPerProcess";
    public const string MaxGlobalListCountPerProcess = "DataImport.Internal.MaxGlobalListCountPerProcess";
    public const string MaxGlobalListItemCountPerProcess = "DataImport.Internal.MaxGlobalListItemCountPerProcess";
    public const string MaxCustomLinkTypes = "DataImport.Internal.MaxCustomLinkTypes";
    public const string MaxStatesPerWorkItemType = "DataImport.Internal.MaxStatesPerWorkItemType";
    public const string MaxValuesInSingleRuleValuesList = "DataImport.Internal.MaxValuesInSingleRuleValuesList";
    public const string MaxSyncNameChangesFieldsPerType = "DataImport.Internal.MaxSyncNameChangesFieldsPerType";
    public const string MaxFieldsInWorkItemType = "DataImport.Internal.MaxFieldsInWorkItemType";
    public const string MaxCustomFieldsPerWorkItemType = "DataImport.Internal.MaxCustomFieldsPerWorkItemType";
    public const string MaxRulesPerWorkItemType = "DataImport.Internal.MaxRulesPerWorkItemType";
    public const string MaxPickListItemsPerList = "DataImport.Internal.MaxPickListItemsPerList";
    public const string MaxPickListItemLength = "DataImport.Internal.MaxPickListItemLength";
    public const string MaxCustomStatesPerWorkItemType = "DataImport.Internal.MaxCustomStatesPerWorkItemType";
    public const string MaxPortfolioBacklogLevels = "DataImport.Internal.MaxPortfolioBacklogLevels";
    public const string MaxPickListsPerCollection = "DataImport.Internal.MaxPickListsPerCollection";
    public const string RestrictedMilestones = "DataImport.Internal.RestrictedMilestones";
    public const string BlockDataImportWithRunningImportEnabled = "DataImport.Internal.BlockDataImportWithRunningImportEnabled";
    public const string BlockDataImportWithExistingProductionEnabled = "DataImport.Internal.BlockDataImportWithExistingProductionEnabled";
    public const string BlockDataImportWithRecentlyCompletedEnabled = "DataImport.Internal.BlockDataImportWithRecentlyCompletedEnabled";
    public const string RequirePreviousProductionHardDelete = "DataImport.Internal.RequirePreviousProductionHardDelete";
    public const string TfsMigratorVersion = "DataImport.TfsMigratorVersion";
    public const string TfsMigratorImportVersion = "DataImport.TfsMigratorImportVersion";
    public const string SkipValidationTfsMigratorVersion = "DataImport.Internal.SkipValidationTfsMigratorVersion";
    public const string SkipValidationSourceCollectionId = "DataImport.Internal.SkipValidationSourceCollectionId";
    public const string ServicesToImport = "DataImport.Internal.ServicesToImport";
    public const string PreviewServices = "DataImport.Internal.PreviewServices";
    public const string ServiceMapPrefix = "DataImport.Internal.ServiceMap.";
    public const string DryRunAccountLifeSpan = "DataImport.Internal.DryRunAccountLifeSpan";
    public const string FailedImportLifeSpan = "DataImport.Internal.FailedImportLifeSpan";
    public const string CollectionBlockedWarningConfirmed = "DataImport.CollectionBlockedWarningConfirmed";
    public const string OrchestratingDataImportHostId = "DataImport.Internal.OrchestratingDataImportHostId";
    public const string TfsMigratorBanner = "DataImport.Internal.TfsMigratorBanner";
    public const string NewTfsMigratorVersionMessageAlreadyShown = "DataImport.Internal.NewTfsMigratorVersionMessageAlreadyShown";
    public const string SkipValidationDataImportHistory = "DataImport.Internal.SkipValidationDataImportHistory";
    public const string SkipValidationBlockImportReason = "DataImport.Internal.SkipValidationBlockImportReason";
    public const string CollectionBlockedWarning = "DataImport.Internal.CollectionBlockedWarning";
    public const string BlockImportReason = "DataImport.Internal.BlockImportReason";
    public const string PublicImportsEnabled = "DataImport.Internal.PublicImportsEnabled";
    public const string MinTfsMigratorVersionPrefix = "DataImport.Internal.MinTfsMigratorVersionPrefix";
    public const string MinTfsMigratorImportVersionPrefix = "DataImport.Internal.MinTfsMigratorImportVersionPrefix";
    public const string LatestTfsMigratorVersionPrefix = "DataImport.Internal.LatestTfsMigratorVersionPrefix";
    public const string UnsupportedCollations = "DataImport.Internal.UnsupportedCollations";
    public const string BlockDataImportTenantLimit = "DataImport.Internal.BlockDataImportTenantLimit";
    public const string DisableAutoFix = "DataImport.Internal.DisableAutoFix";
    public const string DisabeWorkItemColorsAutoFix = "DataImport.Internal.DisabeWorkItemColorsAutoFix";
    public const string AllowCustomTeamField = "DataImport.Internal.AllowCustomTeamField";
    public const string SupportedRegions = "DataImport.Internal.SupportedRegions";
    public const string DeleteActiveDryRunAccounts = "DataImport.Internal.DeleteActiveDryRunAccounts";
    public const string MinSqlPackageVersion = "DataImport.Internal.MinSqlPackageVersion";
    public const string AllowParallelImports = "DataImport.Internal.AllowParallelImports";
    public const string BlockLargeDacpacs = "DataImport.Internal.BlockLargeDacpacs";
    public const string OverrideProjectLimit = "DataImport.Internal.OverrideProjectLimit";
    public const string ServicesThatUseParallelCopy = "DataImport.Internal.ServicesThatUseParallelCopy";
    public const string SpsInstanceId = "DataImport.Internal.SpsInstanceId";
    public const string UseStaticSpsInstance = "DataImport.Internal.UseStaticSpsInstance";
    public const string AllowedSpsInstanceRegionStatuses = "DataImport.Internal.AllowedSpsInstanceRegionStatuses";
    public const string SpsRegionCacheLimit = "DataImport.Internal.SpsRegionCacheLimit";
    public const string SkipOrganizationCatalogService = "DataImport.Internal.SkipOrganizationCatalogService";
    public const string AllowedSourceMilestones = "DataImport.Computed.AllowedSourceMilestones";
    [Obsolete("Import Code are no longer supported")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ImportCode = "DataImport.ImportCode";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string CreateImportCodeInput = "DataImport.Internal.CreateImportCodeInput";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string HostMoveAccountJobId = "DataImport.Internal.HostMoveAccountJobId";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string LegacyServicesToImport = "DataImport.Internal.LegacyServicesToImport";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string AdditionalMilestone = "DataImport.Internal.AdditionalMilestone";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string NeighborHostName = "DataImport.NeighborHostName";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string AddSidsToImportFile = "DataImport.Internal.AddSidsToImportFile";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string StopWritingMappingFile = "DataImport.StopWritingMappingFile";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ShouldImportFromSidsList = "DataImport.ShouldImportFromSidsList";
    [Obsolete("This property has been deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string DisableIdentityDualWriteToDeployment = "DataImport.Internal.DisableIdentityDualWriteToDeployment";
  }
}
