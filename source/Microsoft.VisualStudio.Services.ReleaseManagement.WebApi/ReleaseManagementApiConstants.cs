// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseManagementApiConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  public static class ReleaseManagementApiConstants
  {
    public const string ReleaseAreaName = "Release";
    public const string AreaId = "efc2f575-36ef-48e9-b672-0c6fb4a48ac5";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ReleaseManagementAreaName = "ReleaseManagement";
    public const string ConsumersAreaName = "RM";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string InstanceType = "0000000D-0000-8888-8000-000000000000";
    public const string ReleaseTaskHubName = "Release";
    public const string ProjectScopeRoute = "project";
    public const string ResourceRelease = "release";
    public const string TfsConnectionResource = "tfsconnection";
    public const string SubscriptionResource = "azuresubscription";
    public const string ComponentResource = "component";
    public const string StageResource = "stage";
    public const string EnvironmentResource = "environment";
    public const string ReleasePathResource = "releasepath";
    public const string ReleaseTemplateResource = "releasetemplate";
    public const string DefinitionsResource = "definitions";
    public const string DefinitionRevisionsResource = "revisions";
    public const string HistoryResource = "history";
    public const string EnvironmentTemplatesResource = "environmenttemplates";
    public const string ReleasesResource = "releases";
    public const string ReleaseIssuesResource = "issues";
    public const string ReleaseAutoTriggerIssuesResource = "autotriggerissues";
    public const string ApprovalsResource = "approvals";
    public const string ConsumersResource = "consumers";
    public const string ArtifactsDefinitionsResource = "artifactdefs";
    public const string ArtifactsResource = "artifacts";
    public const string AgentArtifactsResource = "agentartifacts";
    public const string TaskLogsResource = "tasklogs";
    public const string ReleaseLogsResource = "logs";
    public const string ReleaseTasksResource = "tasks";
    public const string ReleaseChangesResource = "changes";
    public const string ReleaseWorkItemsResource = "workitems";
    public const string SendMailResource = "sendmail";
    public const string ArtifactsTypesResource = "types";
    public const string ArtifactTypeInputValuesQueryResource = "inputvaluesquery";
    public const string ArtifactsVersionsResource = "versions";
    public const string ArtifactTypeSourcesQueryResource = "sources";
    public const string ArtifactSourceBranchesResource = "sourcebranches";
    public const string ReleaseEnvironmentsResource = "environments";
    public const string ManualInterventionsResource = "manualInterventions";
    public const string DeploymentsResource = "deployments";
    public const string ReleaseSettingsResource = "releasesettings";
    public const string ThrottlingQueueResource = "throttlingQueue";
    public const string ContinuousDeploymentSetupResource = "ContinuousDeployment";
    public const string ProjectsResource = "projects";
    public const string FavoritesResource = "favorites";
    public const string MetricsResource = "metrics";
    public const string FoldersResource = "folders";
    public const string TagsResource = "tags";
    public const string DefinitionEnvironments = "definitionEnvironments";
    public const string ReleaseAttachmentsResource = "attachments";
    public const string DeploymentBadgeResource = "badge";
    public const string ReleaseGatesResource = "gates";
    public const string ReceiveExternalEventResource = "receiveExternalEvent";
    public const string FlightAssignmentsResource = "flightAssignments";
    public const string PipelineReleaseSettingsResource = "pipelineReleaseSettings";
    public const string OrgPipelineReleaseSettingsResource = "orgPipelineReleaseSettings";
    public const string AgentArtifactsLocationGuidString = "D843590D-370D-47EF-97F5-BEA3CEFF021F";
    public const string AgentArtifactsLocationGuidString2 = "F2571C27-BF50-4938-B396-32D109DDEF26";
    public const string ApprovalsLocationGuidString = "9328E074-59FB-465A-89D9-B09C82EE5109";
    public const string ApprovalsGetLocationGuidString = "B47C6458-E73B-47CB-A770-4DF1E8813A91";
    public const string ApprovalHistoryGuidString = "250C7158-852E-4130-A00F-A0CCE9B72D05";
    public const string ReleaseTaskLogsLocationGuidString = "E71BA1ED-C0A4-4A28-A61F-2DD5F68CF3FD";
    public const string GreenlightingGateLogsLocationGuidString = "DEC7CA5A-7F7F-4797-8BF1-8EFC0DC93B28";
    public const string ReleaseTasksGroupTaskLogsLocationGuidString = "17C91AF7-09FD-4256-BFF1-C24EE4F73BC0";
    public const string ReleaseLogsLocationGuidString = "C37FBAB5-214B-48E4-A55B-CB6B4F6E4038";
    public const string ReleaseChangesLocationGuidString = "8DCF9FE9-CA37-4113-8EE1-37928E98407C";
    public const string ReleaseDefinitionsLocationIdGuidString = "D8F96F24-8EA7-4CB6-BAAB-2DF8FC515665";
    public const string ReleaseDefinitionHistoryLocationIdGuidString = "258B82E0-9D41-43F3-86D6-FEF14DDD44BC";
    public const string ReleaseHistoryLocationIdGuidString = "23F461C8-629A-4144-A076-3054FA5F268A";
    public const string ArtifactTypeSourcesQueryLocationGuidString = "A9C09FE4-901E-4B2E-B05D-9888AD883AE9";
    public const string ReleaseWitLocationGuidString = "4F165CC0-875C-4768-B148-F12F78769FAB";
    public const string ReleaseTasksLocationGuidString = "36B276E0-3C70-4320-A63C-1A2E1466A0D1";
    public const string ReleaseTaskGroupTasksLocationGuidString = "4259191D-4B0A-4409-9FB3-09F22AB9BC47";
    public const string SendMailLocationGuidString = "224E92B2-8D13-4C14-B120-13D877C516F8";
    public const string FavoritesLocationGuidString = "938f7222-9acb-48fe-b8a3-4eda04597171";
    public const string ReleasesLocationGuidString = "A166FDE7-27AD-408E-BA75-703C2CC9D500";
    public const string CollectionReleasesLocationGuidString = "6162082c-380f-4648-95d7-a72348c755f0";
    public const string ReleaseDeploymentsLocationIdGuidString = "B005EF73-CDDC-448E-9BA2-5193BF36B19F";
    public const string ContinuousDeploymentSetupLocationIdGuidString = "C5788899-1E84-439B-B5F9-DBC10ECFFE24";
    public const string ProjectsLocationGuidString = "917ACE4A-79D1-45A7-987C-7BE4DB4268FA";
    public const string ProjectMetricsLocationGuidString = "CD1502BB-3C73-4E11-80A6-D11308DCEAE5";
    public const string FoldersLocationGuidString = "F7DDF76D-CE0C-4D68-94FF-BECAEC5D9DEA";
    public const string TagsLocationGuidString = "86CEE25A-68BA-4BA3-9171-8AD6FFC6DF93";
    public const string ReleaseTagsLocationGuidString = "C5B602B6-D1B3-4363-8A51-94384F78068F";
    public const string ReleaseDefinitionTagsLocationGuidString = "3D21B4C8-C32E-45B2-A7CB-770A369012F4";
    public const string ReleaseIssuesGuidString = "CD42261A-F5C6-41C8-9259-F078989B9F25";
    public const string ReleaseAutoTriggerIssuesGuidString = "C1A68497-69DA-40FB-9423-CAB19CFEECA9";
    public const string ApprovalsUpdateLocationGuidString = "c957584a-82aa-4131-8222-6d47f78bfa7a";
    public const string ObsoleteReleaseAttachmentsLocationGuidString = "214111EE-2415-4DF2-8ED2-74417F7D61F9";
    public const string ObsoleteReleaseAttachmentLocationGuidString = "C4071F6D-3697-46CA-858E-8B10FF09E52F";
    public const string ReleaseAttachmentsLocationGuidString = "a4d06688-0dfa-4895-82a5-f43ec9452306";
    public const string ReleaseAttachmentContentLocationGuidString = "60b86efb-7b8c-4853-8f9f-aa142b77b479";
    public const string DeploymentBadgeLocationGuidString = "1a60a35d-b8c9-45fb-bf67-da0829711147";
    public const string ReleaseTaskLogsGuidString = "2577E6C3-6999-4400-BC69-FE1D837755FE";
    public const string ReleaseTasksLocationGuid2String = "4259291D-4B0A-4409-9FB3-04F22AB9BC47";
    public const string GreenlightingGateLocationGuidString = "2666A539-2001-4F80-BCC7-0379956749D4";
    public const string ReleaseReceiveExternalEventLocationGuidString = "930db3e4-243a-4dda-840e-9edf9deeb456";
    public const string FlightAssignmentsLocationGuidString = "409D301F-3046-46F3-BEB9-4357FBCE0A8C";
    public static readonly Guid ProjectMetricsLocationGuid = new Guid("CD1502BB-3C73-4E11-80A6-D11308DCEAE5");
    public static readonly Guid ReleasesLocationGuid = new Guid("A166FDE7-27AD-408E-BA75-703C2CC9D500");
    public static readonly Guid CollectionReleasesLocationGuid = new Guid("6162082c-380f-4648-95d7-a72348c755f0");
    public static readonly Guid ArtifactDefinitionLocationGuid = new Guid("7CAE6362-A21B-4AE4-A614-8F5706B76730");
    public static readonly Guid EnvironmentTemplateLocationGuid = new Guid("6B03B696-824E-4479-8EB2-6644A51ABA89");
    public static readonly Guid ArtifactsLocationGuid = new Guid("DD93ECAF-CE48-4F24-933F-234F87682B24");
    public static readonly Guid ApprovalsLocationGuid = new Guid("9328E074-59FB-465A-89D9-B09C82EE5109");
    public static readonly Guid ApprovalsGetLocationGuid = new Guid("B47C6458-E73B-47CB-A770-4DF1E8813A91");
    public static readonly Guid ApprovalsHistoryLocationsGuid = new Guid("250C7158-852E-4130-A00F-A0CCE9B72D05");
    public static readonly Guid ReleaseTaskLogsLocationGuid = new Guid("E71BA1ED-C0A4-4A28-A61F-2DD5F68CF3FD");
    public static readonly Guid GreenlightingGateLogsLocationGuid = new Guid("DEC7CA5A-7F7F-4797-8BF1-8EFC0DC93B28");
    public static readonly Guid ReleaseTasksGroupTaskLogsLocationGuid = new Guid("17C91AF7-09FD-4256-BFF1-C24EE4F73BC0");
    public static readonly Guid ReleaseLogsLocationGuid = new Guid("C37FBAB5-214B-48E4-A55B-CB6B4F6E4038");
    public static readonly Guid ArtifactTypeLocationGuid = new Guid("8EFC2A3C-1FC8-4F6D-9822-75E98CECB48F");
    public static readonly Guid ArtifactTypeInputValuesQueryLocationGuid = new Guid("71DD499B-317D-45EA-9134-140EA1932B5E");
    public static readonly Guid ArtifactsVersionsLocationGuid = new Guid("30FC787E-A9E0-4A07-9FBC-3E903AA051D2");
    public static readonly Guid ArtifactTypeSourcesQueryLocationGuid = new Guid("A9C09FE4-901E-4B2E-B05D-9888AD883AE9");
    public static readonly Guid ArtifactSourceBranchesLocationGuid = new Guid("0E5DEF23-78B3-461F-8198-1558F25041C8");
    public static readonly Guid AgentArtifactsLocationGuid = new Guid("D843590D-370D-47EF-97F5-BEA3CEFF021F");
    public static readonly Guid ReleaseChangesLocationGuid = new Guid("8DCF9FE9-CA37-4113-8EE1-37928E98407C");
    public static readonly Guid ReleaseWorkItemsLocationGuid = new Guid("4F165CC0-875C-4768-B148-F12F78769FAB");
    public static readonly Guid SendMailLocationGuid = new Guid("224E92B2-8D13-4C14-B120-13D877C516F8");
    public static readonly Guid ReleaseTasksLocationGuid = new Guid("36B276E0-3C70-4320-A63C-1A2E1466A0D1");
    public static readonly Guid ReleaseTaskGroupTasksLocationGuid = new Guid("4259191D-4B0A-4409-9FB3-09F22AB9BC47");
    public static readonly Guid ReleaseEnvironmentsLocationGuid = new Guid("A7E426B1-03DC-48AF-9DFE-C98BAC612DCB");
    public static readonly Guid AgentArtifactsLocationGuid2 = new Guid("F2571C27-BF50-4938-B396-32D109DDEF26");
    public static readonly Guid ManualInterventionsLocationGuid = new Guid("616C46E4-F370-4456-ADAA-FBAF79C7B79E");
    public static readonly Guid ReleaseIssuesGuid = new Guid("CD42261A-F5C6-41C8-9259-F078989B9F25");
    public static readonly Guid ReleaseAutoTriggerIssuesGuid = new Guid("C1A68497-69DA-40FB-9423-CAB19CFEECA9");
    public static readonly Guid ReleaseTasksLocationGuid2 = new Guid("4259291D-4B0A-4409-9FB3-04F22AB9BC47");
    public static readonly Guid GreenlightingGateLocationGuid = new Guid("2666A539-2001-4F80-BCC7-0379956749D4");
    public static readonly Guid ReleaseDefinitionsLocationId = new Guid("D8F96F24-8EA7-4CB6-BAAB-2DF8FC515665");
    public static readonly Guid ProjectsLocationId = new Guid("917ACE4A-79D1-45A7-987C-7BE4DB4268FA");
    public static readonly Guid ReleaseDefinitionHistoryLocationId = new Guid("258B82E0-9D41-43F3-86D6-FEF14DDD44BC");
    public static readonly Guid ReleaseHistoryLocationId = new Guid("23F461C8-629A-4144-A076-3054FA5F268A");
    public static readonly Guid ConsumersQueryLocationId = new Guid("e9abe73d-489c-4d84-b61b-598cd150232c");
    public static readonly Guid ReleaseLocationId = new Guid("3DDEB308-C21B-4CFE-812D-43BDFF8363A3");
    public static readonly Guid TfsConnectionLocationId = new Guid("f5f759c2-d222-4968-a7ad-b1d72deb7f0d");
    public static readonly Guid SubscriptionLocationId = new Guid("c67132ff-671d-451e-b817-f9e1603d4dac");
    public static readonly Guid ComponentLocationId = new Guid("5e66e77f-2581-47e8-a429-5e61c070326a");
    public static readonly Guid StageLocationId = new Guid("ab4410bc-62b4-454c-b5ba-e6cd9b624d70");
    public static readonly Guid EnvironmentLocationId = new Guid("0ac8266a-5443-4bb6-9274-7b0fcf281b26");
    public static readonly Guid ReleasePathLocationId = new Guid("d141a2a2-f181-4480-ad19-4399b3d1a9d7");
    public static readonly Guid ReleaseTemplateLocationId = new Guid("51edd448-01ef-44af-b686-8cb304cd1376");
    public static readonly Guid ReleaseDeploymentsLocationId = new Guid("B005EF73-CDDC-448E-9BA2-5193BF36B19F");
    public static readonly Guid ReleaseSettingsLocationId = new Guid("c63c3718-7cfd-41e0-b89b-81c1ca143437");
    public static readonly Guid ThrottlingQueueLocationId = new Guid("cf6fc7ba-4ad9-403b-86e6-e372cd3b2327");
    public static readonly Guid ContinuousDeploymentSetupLocationId = new Guid("C5788899-1E84-439B-B5F9-DBC10ECFFE24");
    public static readonly Guid FavoritesLocationGuid = new Guid("938f7222-9acb-48fe-b8a3-4eda04597171");
    public static readonly Guid FoldersLocationGuid = new Guid("F7DDF76D-CE0C-4D68-94FF-BECAEC5D9DEA");
    public static readonly Guid TagsLocationGuid = new Guid("86CEE25A-68BA-4BA3-9171-8AD6FFC6DF93");
    public static readonly Guid ReleaseTagsLocationGuid = new Guid("C5B602B6-D1B3-4363-8A51-94384F78068F");
    public static readonly Guid ReleaseDefinitionTagsLocationGuid = new Guid("3D21B4C8-C32E-45B2-A7CB-770A369012F4");
    public static readonly Guid ReleaseDefinitionEnvironmentsLocationGuid = new Guid("12b5d21a-f54c-430e-a8c1-7515d196890e");
    public static readonly Guid ApprovalsUpdateLocationGuid = new Guid("c957584a-82aa-4131-8222-6d47f78bfa7a");
    public static readonly Guid ObsoleteReleaseAttachmentsLocationGuid = new Guid("214111EE-2415-4DF2-8ED2-74417F7D61F9");
    public static readonly Guid ObsoleteReleaseAttachmentLocationGuid = new Guid("C4071F6D-3697-46CA-858E-8B10FF09E52F");
    public static readonly Guid ReleaseAttachmentsLocationGuid = new Guid("a4d06688-0dfa-4895-82a5-f43ec9452306");
    public static readonly Guid ReleaseAttachmentContentLocationGuid = new Guid("60b86efb-7b8c-4853-8f9f-aa142b77b479");
    public static readonly Guid DeploymentBadgeLocationGuid = new Guid("1a60a35d-b8c9-45fb-bf67-da0829711147");
    public static readonly Guid ReleaseTaskLogsGuid = new Guid("2577E6C3-6999-4400-BC69-FE1D837755FE");
    public static readonly Guid ReleaseReceiveExternalEventLocationGuid = new Guid("930db3e4-243a-4dda-840e-9edf9deeb456");
    public static readonly Guid FlightAssignmentsLocationGuid = new Guid("409D301F-3046-46F3-BEB9-4357FBCE0A8C");
    public static readonly Guid PipelineReleaseSettingsLocationGuid = new Guid("e816b9f4-f9fe-46ba-bdcc-a9af6abf3144");
    public static readonly Guid OrgPipelineReleaseSettingsLocationGuid = new Guid("d156c759-ca4e-492b-90d4-db03971796ea");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This type is immutable.")]
    public static readonly ReadOnlyDictionary<string, Guid> ResourceToLocationIds = new ReadOnlyDictionary<string, Guid>((IDictionary<string, Guid>) new Dictionary<string, Guid>()
    {
      {
        "release",
        ReleaseManagementApiConstants.ReleaseLocationId
      },
      {
        "tfsconnection",
        ReleaseManagementApiConstants.TfsConnectionLocationId
      },
      {
        "azuresubscription",
        ReleaseManagementApiConstants.SubscriptionLocationId
      },
      {
        "component",
        ReleaseManagementApiConstants.ComponentLocationId
      },
      {
        "stage",
        ReleaseManagementApiConstants.StageLocationId
      },
      {
        "environment",
        ReleaseManagementApiConstants.EnvironmentLocationId
      },
      {
        "releasepath",
        ReleaseManagementApiConstants.ReleasePathLocationId
      },
      {
        "releasetemplate",
        ReleaseManagementApiConstants.ReleaseTemplateLocationId
      }
    });
  }
}
