// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.AgileTracePoints
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class AgileTracePoints
  {
    private const int AgileStart = 200000;
    private const int ControllersBase = 210000;
    private const int ApiControllersBase = 220000;
    private const int BusinessLogicBase = 230000;
    private const int ApplicationDataBase = 250000;
    private const int ExceptionBase = 280000;
    private const int BacklogControllerBase = 210100;
    internal const int BacklogIndexStart = 210101;
    internal const int BacklogIndexAddPanel = 210102;
    internal const int BacklogIndexEnd = 210109;
    internal const int BacklogIterationStart = 210111;
    internal const int BacklogIterationEnd = 210119;
    internal const int BacklogCapacityStart = 210121;
    internal const int BacklogCapacityEnd = 210129;
    internal const int BacklogCapacityPostStart = 210131;
    internal const int BacklogCapacityPostEnd = 210139;
    internal const int BacklogSetupCapacityStart = 210141;
    internal const int BacklogSetupCapacityEnd = 210149;
    internal const int BacklogBoardStart = 210151;
    internal const int BacklogBoardEnd = 210159;
    internal const int BacklogBoardCreationStarted = 210152;
    internal const int BacklogBoardCreationSucceeded = 210153;
    internal const int BacklogBoardCreationSkipped = 210154;
    internal const int BacklogBoardColumnStateMismatchEncountered = 210155;
    internal const int BacklogIterationBoardStart = 210171;
    internal const int PrepareTaskBoardDataStart = 210172;
    internal const int PrepareTaskBoardDataGetData = 210181;
    internal const int PrepareTaskBoardDataEnd = 210174;
    internal const int BacklogIterationBoardEnd = 210179;
    private const int BoardControllerBase = 210200;
    internal const int BoardDataLimitReached = 210221;
    private const int TeamChartApiBase = 220200;
    internal const int BurnDownStart = 220201;
    internal const int BurnDownEnd = 220209;
    internal const int VelocityStart = 220211;
    internal const int VelocityEnd = 220219;
    internal const int CumulativeFlowDiagramStart = 220241;
    internal const int CumulativeFlowDiagramEnd = 220249;
    private const int ProcessConfigurationWebServiceBase = 220400;
    internal const int GetCommonConfigurationStart = 220401;
    internal const int GetCommonConfigurationEnd = 220409;
    internal const int SetCommonConfigurationStart = 220411;
    internal const int SetCommonConfigurationEnd = 220419;
    internal const int ValidateCommonConfigurationStart = 220421;
    internal const int ValidateCommonConfigurationEnd = 220429;
    internal const int GetAgileConfigurationStart = 220431;
    internal const int GetAgileConfigurationEnd = 220439;
    internal const int SetAgileConfigurationStart = 220441;
    internal const int SetAgileConfigurationEnd = 220449;
    internal const int ValidateAgileConfigurationStart = 220451;
    internal const int ValidateAgileConfigurationEnd = 220459;
    internal const int GetProjectProcessConfigurationStart = 220461;
    internal const int GetProjectProcessConfigurationEnd = 220469;
    internal const int SetProjectProcessConfigurationStart = 220471;
    internal const int SetProjectProcessConfigurationEnd = 220479;
    internal const int ValidateProjectProcessConfigurationStart = 220481;
    internal const int ValidateProjectProcessConfigurationEnd = 220489;
    internal const int SprintDashboardBase = 220500;
    internal const int SprintSummaryStart = 220501;
    internal const int SprintSummaryEnd = 220502;
    internal const int SprintBurndownStart = 220503;
    internal const int SprintBurndownEnd = 220509;
    private const int IntegrationWebServiceBase = 220600;
    internal const int DeleteProjectStart = 220601;
    internal const int DeleteProjectEnd = 220609;
    private const int ProvisionKanbanBoardBase = 230300;
    internal const int ProvisionKanbanBoardStart = 230301;
    internal const int ProvisionKanbanBoardEnd = 230302;
    internal const int ProvisionKanbanBoardFailed = 230303;
    internal const int ProvisionKanbanBoardProcessingTeamStart = 230304;
    internal const int ProvisionKanbanBoardProcessingTeamEnd = 230305;
    internal const int ProvisionKanbanBoardProcessingProjectStart = 230306;
    internal const int ProvisionKanbanBoardProcessingProjectEnd = 230307;
    internal const int ProvisionKanbanBoardBackFillStart = 230308;
    internal const int ProvisionKanbanBoardBackFillEnd = 230309;
    internal const int RequestIterationNode = 250001;
    internal const int IterationQuery = 250003;
    internal const int SettingsException = 280001;
    internal const int InvalidIterationPathException = 280002;
    internal const int UnhandledException = 280003;
    internal const int BurndownChartValidationFailed = 280004;
    internal const int VelocityChartValidationFailed = 280005;
    internal const int InvalidTeamSettingsException = 290232;
    internal const int ProjectSettingsException = 290233;
  }
}
