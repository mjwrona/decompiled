// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SettingsTracePoints
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class SettingsTracePoints
  {
    private const int AgileStart = 200000;
    private const int BusinessLogicBase = 240000;
    private const int ExceptionsDataBase = 290000;
    private const int ProjectSettingsServiceBase = 240100;
    internal const int ProjectSettingGetStart = 240101;
    internal const int ProjectSettingGetEnd = 240109;
    internal const int ProjectSettingSetStart = 240111;
    internal const int ProjectSettingSetEnd = 240119;
    internal const int DeleteProjectSettingsStart = 240171;
    internal const int DeleteProjectSettingsException = 240178;
    internal const int DeleteProjectSettingsEnd = 240179;
    internal const int DeleteProjectTeamSettingsException = 240188;
    internal const int TeamCreationSubscriberStart = 240252;
    internal const int TeamCreationSubscriberEnd = 240253;
    internal const int InvalidProjectSettings = 290001;
    internal const int InvalidNodeError = 290002;
    internal const int FailedKanbanProvision = 290003;
    internal const int FailedArtifactCleanup = 290004;
    internal const int UnexpectedException = 290005;
    internal const int TeamCreationSubscriberException = 290006;
    private const int BoardSettingsBase = 240300;
    internal const int BoardSettingsValidationFailed = 240301;
    internal const int BoardSettingsColumnNameNotInWitMap = 240302;
    internal const int BoardSettingsCreationStart = 240303;
    internal const int BoardSettingsCreationEnd = 240304;
    internal const int BoardSettingsBoardExists = 240305;
    internal const int CreateBoardStart = 240306;
    internal const int CreateBoardEnd = 240307;
    internal const int DeleteBoardsStart = 240308;
    internal const int DeleteBoardsEnd = 240309;
    internal const int DeleteTeamBoardsStart = 240310;
    internal const int DeleteTeamBoardsEnd = 240311;
    internal const int DeleteTeamBoardException = 240312;
    internal const int SoftDeleteBoardsStart = 240313;
    internal const int SoftDeleteBoardsEnd = 240314;
    internal const int RestoreBoardStart = 240315;
    internal const int RestoreBoardEnd = 240316;
    internal const int DeleteBoardsByCategoryReferenceNamesStart = 240317;
    internal const int DeleteBoardsByCategoryReferenceNamesEnd = 240318;
    internal const int DefaultSettingsValidatorDataProviderError = 240319;
    private const int KanbanBase = 240400;
    internal const int KanbanExtensionCreationStart = 240401;
    internal const int KanbanExtensionCreationEnd = 240402;
    internal const int KanbanExtensionPredicateStart = 240403;
    internal const int KanbanExtensionPredicateEnd = 240404;
    internal const int KanbanExtensionFieldRulesStart = 240405;
    internal const int KanbanExtensionFieldRulesEnd = 240406;
    internal const int KanbanExtensionUpdateStart = 240407;
    internal const int KanbanExtensionUpdateEnd = 240408;
    internal const int EnsureKanbanBoardIsUpToDateForAllTeamsStart = 240409;
    internal const int EnsureKanbanBoardIsUpToDateForAllTeamsEnd = 240410;
    internal const int EnsureKanbanBoardStart = 240411;
    internal const int EnsureKanbanBoardEnd = 240412;
    internal const int KanbanExtensionUpdateFailed = 240413;
  }
}
