// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CustomSqlError
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class CustomSqlError
  {
    public const int GenericDatabaseUserMessage = 50000;
    public const int TransactionRequired = 901002;
    public const int GenericDatabaseUpdateFailure = 901004;
    public const int DefinitionExists = 901005;
    public const int DefinitionNotFound = 901006;
    public const int QueueExists = 901007;
    public const int QueueNotFound = 901008;
    public const int BuildExists = 901009;
    public const int BuildNotFound = 901010;
    public const int DefinitionDisabled = 901011;
    public const int ArtifactExists = 901012;
    public const int DefinitionTemplateExists = 901014;
    public const int BuildStatusInvalidChange = 901015;
    public const int BuildNumberLengthExceeded = 901016;
    public const int FolderExists = 901017;
    public const int FolderNotFound = 901018;
    public const int BuildOrchestrationExists = 901027;
    public const int DefinitionTriggerAlreadyExists = 901019;
    public const int InvalidDefinitionTriggerSource = 901020;
    public const int InvalidDefinitionTriggers_CycleDetected = 901021;
    public const int InvalidDefinitionTriggers_UnsupportedTriggerChainLength = 901022;
    public const int DefinitionsHaveRetainedBuilds = 901023;
    public const int CannotRestoreDeletedDraftWithoutRestoringParent = 901024;
    public const int BuildEventNotFound = 901025;
    public const int InvalidBuildEventStatusUpdate = 901026;
  }
}
