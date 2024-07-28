// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.AgileResources
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Agile.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class AgileResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AgileResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (AgileResources.resourceMan == null)
          AgileResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Agile.Server.AgileResources", typeof (AgileResources).Assembly);
        return AgileResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => AgileResources.resourceCulture;
      set => AgileResources.resourceCulture = value;
    }

    public static string BacklogCapacityInvalidCapacityPerDay => AgileResources.ResourceManager.GetString(nameof (BacklogCapacityInvalidCapacityPerDay), AgileResources.resourceCulture);

    public static string BacklogCapacityInvalidUser => AgileResources.ResourceManager.GetString(nameof (BacklogCapacityInvalidUser), AgileResources.resourceCulture);

    public static string BacklogCapacityInvalidWeekend => AgileResources.ResourceManager.GetString(nameof (BacklogCapacityInvalidWeekend), AgileResources.resourceCulture);

    public static string BacklogContextNotSet => AgileResources.ResourceManager.GetString(nameof (BacklogContextNotSet), AgileResources.resourceCulture);

    public static string BacklogDoesNotExistExceptionMessage => AgileResources.ResourceManager.GetString(nameof (BacklogDoesNotExistExceptionMessage), AgileResources.resourceCulture);

    public static string BacklogFieldValueAllowOneDefaultMessage => AgileResources.ResourceManager.GetString(nameof (BacklogFieldValueAllowOneDefaultMessage), AgileResources.resourceCulture);

    public static string BacklogFieldValueCannotBeNullMessage => AgileResources.ResourceManager.GetString(nameof (BacklogFieldValueCannotBeNullMessage), AgileResources.resourceCulture);

    public static string BacklogFieldValueDuplicateId => AgileResources.ResourceManager.GetString(nameof (BacklogFieldValueDuplicateId), AgileResources.resourceCulture);

    public static string BacklogFieldValueRequireDefaultMessage => AgileResources.ResourceManager.GetString(nameof (BacklogFieldValueRequireDefaultMessage), AgileResources.resourceCulture);

    public static string BacklogFilterContextInvalid => AgileResources.ResourceManager.GetString(nameof (BacklogFilterContextInvalid), AgileResources.resourceCulture);

    public static string BacklogHubContextInvalid => AgileResources.ResourceManager.GetString(nameof (BacklogHubContextInvalid), AgileResources.resourceCulture);

    public static string BacklogIdInPostOperationExceptionMessage => AgileResources.ResourceManager.GetString(nameof (BacklogIdInPostOperationExceptionMessage), AgileResources.resourceCulture);

    public static string BacklogLevelInvisible => AgileResources.ResourceManager.GetString(nameof (BacklogLevelInvisible), AgileResources.resourceCulture);

    public static string BacklogWithNameAlreadyExistsExceptionMessage => AgileResources.ResourceManager.GetString(nameof (BacklogWithNameAlreadyExistsExceptionMessage), AgileResources.resourceCulture);

    public static string BoardBadgeClickToSeeTheBoard => AgileResources.ResourceManager.GetString(nameof (BoardBadgeClickToSeeTheBoard), AgileResources.resourceCulture);

    public static string BoardBadgeCustomColumnsSetIncorrectly => AgileResources.ResourceManager.GetString(nameof (BoardBadgeCustomColumnsSetIncorrectly), AgileResources.resourceCulture);

    public static string BoardBadgeLeftText => AgileResources.ResourceManager.GetString(nameof (BoardBadgeLeftText), AgileResources.resourceCulture);

    public static string BoardDoesNotExistExceptionMessage => AgileResources.ResourceManager.GetString(nameof (BoardDoesNotExistExceptionMessage), AgileResources.resourceCulture);

    public static string BoardIdOrNameDoesNotExist => AgileResources.ResourceManager.GetString(nameof (BoardIdOrNameDoesNotExist), AgileResources.resourceCulture);

    public static string BoardUpdateFailureExceptionMessage => AgileResources.ResourceManager.GetString(nameof (BoardUpdateFailureExceptionMessage), AgileResources.resourceCulture);

    public static string BoardValidationFailureExceptionMessage => AgileResources.ResourceManager.GetString(nameof (BoardValidationFailureExceptionMessage), AgileResources.resourceCulture);

    public static string CapacityContainsMemberGroupExceptionMessage => AgileResources.ResourceManager.GetString(nameof (CapacityContainsMemberGroupExceptionMessage), AgileResources.resourceCulture);

    public static string CapacityMultipleCapacitiesNotSupported => AgileResources.ResourceManager.GetString(nameof (CapacityMultipleCapacitiesNotSupported), AgileResources.resourceCulture);

    public static string ChartDoesNotExistExceptionMessage => AgileResources.ResourceManager.GetString(nameof (ChartDoesNotExistExceptionMessage), AgileResources.resourceCulture);

    public static string ChartUpdateFailureExceptionMessage => AgileResources.ResourceManager.GetString(nameof (ChartUpdateFailureExceptionMessage), AgileResources.resourceCulture);

    public static string CumulativeFlowSettingsFutureDateMessage => AgileResources.ResourceManager.GetString(nameof (CumulativeFlowSettingsFutureDateMessage), AgileResources.resourceCulture);

    public static string CumulativeFlowSettingsInvalidValue => AgileResources.ResourceManager.GetString(nameof (CumulativeFlowSettingsInvalidValue), AgileResources.resourceCulture);

    public static string FailedToCreateBacklogExceptionMessage => AgileResources.ResourceManager.GetString(nameof (FailedToCreateBacklogExceptionMessage), AgileResources.resourceCulture);

    public static string FailedToCreateBacklogFieldValues => AgileResources.ResourceManager.GetString(nameof (FailedToCreateBacklogFieldValues), AgileResources.resourceCulture);

    public static string FailedToCreateCapacity => AgileResources.ResourceManager.GetString(nameof (FailedToCreateCapacity), AgileResources.resourceCulture);

    public static string FailedToCreateIterationPaths => AgileResources.ResourceManager.GetString(nameof (FailedToCreateIterationPaths), AgileResources.resourceCulture);

    public static string FailedToCreateMembers => AgileResources.ResourceManager.GetString(nameof (FailedToCreateMembers), AgileResources.resourceCulture);

    public static string FailedToDeleteBacklogExceptionMessage => AgileResources.ResourceManager.GetString(nameof (FailedToDeleteBacklogExceptionMessage), AgileResources.resourceCulture);

    public static string FailedToGetBacklogExceptionMessage => AgileResources.ResourceManager.GetString(nameof (FailedToGetBacklogExceptionMessage), AgileResources.resourceCulture);

    public static string FailedToUpdateBacklogName => AgileResources.ResourceManager.GetString(nameof (FailedToUpdateBacklogName), AgileResources.resourceCulture);

    public static string GeneralInvalidDataMessage => AgileResources.ResourceManager.GetString(nameof (GeneralInvalidDataMessage), AgileResources.resourceCulture);

    public static string GetBoardUserSettings_GetFailed => AgileResources.ResourceManager.GetString(nameof (GetBoardUserSettings_GetFailed), AgileResources.resourceCulture);

    public static string GetParentChildWIMap_GetFailed => AgileResources.ResourceManager.GetString(nameof (GetParentChildWIMap_GetFailed), AgileResources.resourceCulture);

    public static string GetParentChildWIMap_LimiExceeded => AgileResources.ResourceManager.GetString(nameof (GetParentChildWIMap_LimiExceeded), AgileResources.resourceCulture);

    public static string InvalidBacklogFieldValueMessage => AgileResources.ResourceManager.GetString(nameof (InvalidBacklogFieldValueMessage), AgileResources.resourceCulture);

    public static string InvalidBacklogNameExceptionMessage => AgileResources.ResourceManager.GetString(nameof (InvalidBacklogNameExceptionMessage), AgileResources.resourceCulture);

    public static string InvalidDateMessage => AgileResources.ResourceManager.GetString(nameof (InvalidDateMessage), AgileResources.resourceCulture);

    public static string InvalidDateRangeExceptionMessage => AgileResources.ResourceManager.GetString(nameof (InvalidDateRangeExceptionMessage), AgileResources.resourceCulture);

    public static string InvalidDefaultIterationParameters => AgileResources.ResourceManager.GetString(nameof (InvalidDefaultIterationParameters), AgileResources.resourceCulture);

    public static string InvalidIterationMacroMessage => AgileResources.ResourceManager.GetString(nameof (InvalidIterationMacroMessage), AgileResources.resourceCulture);

    public static string InvalidIterationPathMessage => AgileResources.ResourceManager.GetString(nameof (InvalidIterationPathMessage), AgileResources.resourceCulture);

    public static string InvalidMemberExceptionMessage => AgileResources.ResourceManager.GetString(nameof (InvalidMemberExceptionMessage), AgileResources.resourceCulture);

    public static string InvalidProjectPermissionExceptionMessage => AgileResources.ResourceManager.GetString(nameof (InvalidProjectPermissionExceptionMessage), AgileResources.resourceCulture);

    public static string MissingCategory => AgileResources.ResourceManager.GetString(nameof (MissingCategory), AgileResources.resourceCulture);

    public static string MissingOrderByField => AgileResources.ResourceManager.GetString(nameof (MissingOrderByField), AgileResources.resourceCulture);

    public static string NoPermissionReadTeamException => AgileResources.ResourceManager.GetString(nameof (NoPermissionReadTeamException), AgileResources.resourceCulture);

    public static string ProductBacklogReorderHelper_BacklogChanged => AgileResources.ResourceManager.GetString(nameof (ProductBacklogReorderHelper_BacklogChanged), AgileResources.resourceCulture);

    public static string ProductBacklogReorderHelper_InvalidIterationId => AgileResources.ResourceManager.GetString(nameof (ProductBacklogReorderHelper_InvalidIterationId), AgileResources.resourceCulture);

    public static string ProductBacklogReorderHelper_InvalidReorderIds => AgileResources.ResourceManager.GetString(nameof (ProductBacklogReorderHelper_InvalidReorderIds), AgileResources.resourceCulture);

    public static string ProductBacklogReorderHelper_InvalidTypes => AgileResources.ResourceManager.GetString(nameof (ProductBacklogReorderHelper_InvalidTypes), AgileResources.resourceCulture);

    public static string ProductBacklogReorderHelper_ParentIsInCompleteState => AgileResources.ResourceManager.GetString(nameof (ProductBacklogReorderHelper_ParentIsInCompleteState), AgileResources.resourceCulture);

    public static string ProjectDoesNotExistExceptionMessage => AgileResources.ResourceManager.GetString(nameof (ProjectDoesNotExistExceptionMessage), AgileResources.resourceCulture);

    public static string ProjectIdIsNotGuidExceptionMessage => AgileResources.ResourceManager.GetString(nameof (ProjectIdIsNotGuidExceptionMessage), AgileResources.resourceCulture);

    public static string ReorderAboveNestedWorkItemException => AgileResources.ResourceManager.GetString(nameof (ReorderAboveNestedWorkItemException), AgileResources.resourceCulture);

    public static string ReorderNestedWorkItemException => AgileResources.ResourceManager.GetString(nameof (ReorderNestedWorkItemException), AgileResources.resourceCulture);

    public static string ReorderUnderNestedWorkItemException => AgileResources.ResourceManager.GetString(nameof (ReorderUnderNestedWorkItemException), AgileResources.resourceCulture);

    public static string ReorderWorkItemsOperationsNullOrEmpty => AgileResources.ResourceManager.GetString(nameof (ReorderWorkItemsOperationsNullOrEmpty), AgileResources.resourceCulture);

    public static string SetBoardSettings_DeletedColumnHasItems => AgileResources.ResourceManager.GetString(nameof (SetBoardSettings_DeletedColumnHasItems), AgileResources.resourceCulture);

    public static string SetBoardSettings_DeletedRowHasItems => AgileResources.ResourceManager.GetString(nameof (SetBoardSettings_DeletedRowHasItems), AgileResources.resourceCulture);

    public static string Taskboard_ColumnIdNotfound => AgileResources.ResourceManager.GetString("Taskboard-ColumnIdNotfound", AgileResources.resourceCulture);

    public static string Taskboard_ColumnNameInvalid => AgileResources.ResourceManager.GetString("Taskboard-ColumnNameInvalid", AgileResources.resourceCulture);

    public static string Taskboard_ColumnNamesNotUnique => AgileResources.ResourceManager.GetString("Taskboard-ColumnNamesNotUnique", AgileResources.resourceCulture);

    public static string TaskboardColumnDuplicateColumnIdException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnDuplicateColumnIdException), AgileResources.resourceCulture);

    public static string TaskboardColumnFirstColumnToStateInvalid => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnFirstColumnToStateInvalid), AgileResources.resourceCulture);

    public static string TaskboardColumnInvalidException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnInvalidException), AgileResources.resourceCulture);

    public static string TaskboardColumnInvalidForStateException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnInvalidForStateException), AgileResources.resourceCulture);

    public static string TaskboardColumnLastColumnToStateInvalid => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnLastColumnToStateInvalid), AgileResources.resourceCulture);

    public static string TaskboardColumnMappingEmptyException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnMappingEmptyException), AgileResources.resourceCulture);

    public static string TaskboardColumnMappingInvalidException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnMappingInvalidException), AgileResources.resourceCulture);

    public static string TaskboardColumnMaxLimitExceededException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnMaxLimitExceededException), AgileResources.resourceCulture);

    public static string TaskboardColumnMinColumnLimitException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnMinColumnLimitException), AgileResources.resourceCulture);

    public static string TaskboardColumnNotCustomizedException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnNotCustomizedException), AgileResources.resourceCulture);

    public static string TaskboardColumnNotFoundException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnNotFoundException), AgileResources.resourceCulture);

    public static string TaskboardColumnUpdateException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnUpdateException), AgileResources.resourceCulture);

    public static string TaskboardColumnUpdateUserIsNotTeamAdminException => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnUpdateUserIsNotTeamAdminException), AgileResources.resourceCulture);

    public static string TaskboardColumnWorkItemTypeInvalid => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnWorkItemTypeInvalid), AgileResources.resourceCulture);

    public static string TaskboardColumnWorkItemTypeMappingMissing => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnWorkItemTypeMappingMissing), AgileResources.resourceCulture);

    public static string TaskboardColumnWorkItemTypeStateInvalid => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnWorkItemTypeStateInvalid), AgileResources.resourceCulture);

    public static string TaskboardColumnWorkItemTypeStateMappingMissing => AgileResources.ResourceManager.GetString(nameof (TaskboardColumnWorkItemTypeStateMappingMissing), AgileResources.resourceCulture);

    public static string TaskboardInvalidTeamIterationException => AgileResources.ResourceManager.GetString(nameof (TaskboardInvalidTeamIterationException), AgileResources.resourceCulture);

    public static string TaskboardWorkItemInvalidIterationException => AgileResources.ResourceManager.GetString(nameof (TaskboardWorkItemInvalidIterationException), AgileResources.resourceCulture);

    public static string TeamDoesNotExistException => AgileResources.ResourceManager.GetString(nameof (TeamDoesNotExistException), AgileResources.resourceCulture);

    public static string TeamFieldValuesDefaultValueNotFound => AgileResources.ResourceManager.GetString(nameof (TeamFieldValuesDefaultValueNotFound), AgileResources.resourceCulture);

    public static string TeamFieldValuesNoTeamFieldValuesSelected => AgileResources.ResourceManager.GetString(nameof (TeamFieldValuesNoTeamFieldValuesSelected), AgileResources.resourceCulture);

    public static string TeamSettings_CurrentIterationNotFound => AgileResources.ResourceManager.GetString(nameof (TeamSettings_CurrentIterationNotFound), AgileResources.resourceCulture);

    public static string UpdateBoardCardRules_InvalidBoardCardRules => AgileResources.ResourceManager.GetString(nameof (UpdateBoardCardRules_InvalidBoardCardRules), AgileResources.resourceCulture);

    public static string UpdateBoardCardRules_invalidTagName => AgileResources.ResourceManager.GetString(nameof (UpdateBoardCardRules_invalidTagName), AgileResources.resourceCulture);

    public static string UpdateBoardCardRules_isEnabledBoolean => AgileResources.ResourceManager.GetString(nameof (UpdateBoardCardRules_isEnabledBoolean), AgileResources.resourceCulture);

    public static string UpdateBoardCardRules_UpdateBoardFailed => AgileResources.ResourceManager.GetString(nameof (UpdateBoardCardRules_UpdateBoardFailed), AgileResources.resourceCulture);

    public static string UpdateBoardCardRules_UserIsNotTeamAdmin => AgileResources.ResourceManager.GetString(nameof (UpdateBoardCardRules_UserIsNotTeamAdmin), AgileResources.resourceCulture);

    public static string UpdateBoardCardSettings_InvalidBoardCardSettings => AgileResources.ResourceManager.GetString(nameof (UpdateBoardCardSettings_InvalidBoardCardSettings), AgileResources.resourceCulture);

    public static string UpdateBoardCardSettings_UpdateBoardFailed => AgileResources.ResourceManager.GetString(nameof (UpdateBoardCardSettings_UpdateBoardFailed), AgileResources.resourceCulture);

    public static string UpdateBoardCardSettings_UserIsNotTeamAdmin => AgileResources.ResourceManager.GetString(nameof (UpdateBoardCardSettings_UserIsNotTeamAdmin), AgileResources.resourceCulture);

    public static string UpdateBoardColumns_UserIsNotTeamAdmin => AgileResources.ResourceManager.GetString(nameof (UpdateBoardColumns_UserIsNotTeamAdmin), AgileResources.resourceCulture);

    public static string UpdateBoardFilterSettings_InvalidBoardFilter => AgileResources.ResourceManager.GetString(nameof (UpdateBoardFilterSettings_InvalidBoardFilter), AgileResources.resourceCulture);

    public static string UpdateBoardFilterSettings_UpdateBoardFailed => AgileResources.ResourceManager.GetString(nameof (UpdateBoardFilterSettings_UpdateBoardFailed), AgileResources.resourceCulture);

    public static string UpdateBoardOptions_UserIsNotTeamAdmin => AgileResources.ResourceManager.GetString(nameof (UpdateBoardOptions_UserIsNotTeamAdmin), AgileResources.resourceCulture);

    public static string UpdateBoardRows_UserIsNotTeamAdmin => AgileResources.ResourceManager.GetString(nameof (UpdateBoardRows_UserIsNotTeamAdmin), AgileResources.resourceCulture);

    public static string UpdateBoardUserSettings_UpdateFailed => AgileResources.ResourceManager.GetString(nameof (UpdateBoardUserSettings_UpdateFailed), AgileResources.resourceCulture);

    public static string UpdateChartSettings_UserIsNotTeamAdmin => AgileResources.ResourceManager.GetString(nameof (UpdateChartSettings_UserIsNotTeamAdmin), AgileResources.resourceCulture);

    public static string ViewPropertiesFormatException => AgileResources.ResourceManager.GetString(nameof (ViewPropertiesFormatException), AgileResources.resourceCulture);

    public static string ViewTypeDoesNotExistException => AgileResources.ResourceManager.GetString(nameof (ViewTypeDoesNotExistException), AgileResources.resourceCulture);
  }
}
