// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildSqlError
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class BuildSqlError
  {
    public const int SqlServerDefaultUserMessage = 50000;
    public const int GenericDatabaseUpdateFailure = 900004;
    public const int InvalidBuildUri = 900005;
    public const int InvalidPlatformFlavor = 900008;
    public const int BuildNumberAlreadyExists = 900009;
    public const int BuildDefinitionAlreadyExists = 900010;
    public const int BuildDefinitionNameInvalid = 900011;
    public const int BuildGroupAlreadyExists = 900012;
    public const int BuildGroupDoesNotExist = 900013;
    public const int BuildDefinitionDoesNotExist = 900017;
    public const int BuildQualityDoesNotExist = 900018;
    public const int BuildDefinitionInvalidMove = 900020;
    public const int InvalidBuildRequest = 900021;
    public const int InvalidParentNodeSpecified = 900024;
    public const int DuplicateInformationChangeRequest = 900025;
    public const int InvalidEditRequestNodeNotFound = 900026;
    public const int CannotDeleteDefinitionQueuedBuildExists = 900027;
    public const int CannotDeleteDefinitionBuildExists = 900028;
    public const int ResourceBuildNotInProgress = 900033;
    public const int ResourceAlreadyRequested = 900034;
    public const int ResourceAlreadyAcquired = 900035;
    public const int CannotDestroyBuildNotDeleted = 900036;
    public const int CannotAddDuplicateProcessTemplate = 900037;
    public const int MultipleDefaultProcessTemplates = 900038;
    public const int MultipleUpgradeProcessTemplates = 900039;
    public const int ProcessTemplateDoesNotExistForDelete = 900040;
    public const int ProcessTemplateDoesNotExistForUpdate = 900041;
    public const int BuildDefinitionIsDisabled = 900042;
    public const int BatchingNotSupportedForUpgradeTemplate = 900043;
    public const int BuildServiceHostDoesNotExist = 900500;
    public const int BuildServiceHostAlreadyExists = 900501;
    public const int BuildServiceHostInvalidOwner = 900502;
    public const int BuildControllerDoesNotExist = 900510;
    public const int BuildControllerAlreadyExistsForServiceHost = 900511;
    public const int BuildControllerAlreadyExists = 900512;
    public const int BuildControllerCannotBeDeletedBuildsInProgress = 900513;
    public const int BuildControllerCannotBeDeletedAgentsAssociated = 900514;
    public const int BuildAgentDoesNotExist = 900520;
    public const int BuildAgentAlreadyExistsForServiceHost = 900521;
    public const int BuildAgentAlreadyExistsForController = 900522;
    public const int BuildAgentCannotBeRemovedWhileReserved = 900523;
    public const int BuildAgentCannotBeAddedElastic = 900525;
    public const int BuildAgentVersionMismatch = 900526;
    public const int BuildQueueCannotSatisfyPosition = 900530;
    public const int BuildQueueInvalidBuildUri = 900531;
    public const int BuildQueueCannotCancelBuildInvalidStatus = 900532;
    public const int BuildQueueCannotUpdateBuildInvalidStatus = 900533;
    public const int BuildQueueInvalidId = 900534;
    public const int BuildQueueDefinitionNotPaused = 900535;
    public const int BuildQueueControllerNotAvailable = 900536;
    public const int BuildQueueControllerCantStartBuild = 900537;
    public const int BuildQueueCannotRequeueInvalidStatus = 900538;
    public const int BuildQueueCannotStartBuildGatedInProgress = 900539;
    public const int BuildAgentQueueBuildNotInProgress = 900540;
    public const int BuildAgentQueueCannotSatisfyRequest = 900541;
    public const int BuildDefinitionCanOnlyBeSpecifiedOnce = 900542;
    public const int RethrownMessage = 900543;
    public const int BuildAgentQueueVersionMismatch = 900544;
    public const int BuildDeploymentAlreadyExists = 900545;
  }
}
