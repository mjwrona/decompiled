// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.PipelinesAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class PipelinesAuditConstants
  {
    private const string PipelinesArea = "Pipelines.";
    public const string ResourceAuthorizedForPipeline = "Pipelines.ResourceAuthorizedForPipeline";
    public const string ResourceNotAuthorizedForPipeline = "Pipelines.ResourceNotAuthorizedForPipeline";
    public const string ResourceUnauthorizedForPipeline = "Pipelines.ResourceUnauthorizedForPipeline";
    public const string ResourceAuthorizedForProject = "Pipelines.ResourceAuthorizedForProject";
    public const string ResourceNotAuthorizedForProject = "Pipelines.ResourceNotAuthorizedForProject";
    public const string ResourceUnauthorizedForProject = "Pipelines.ResourceUnauthorizedForProject";
    public const string PipelineCreated = "Pipelines.PipelineCreated";
    public const string PipelineDeleted = "Pipelines.PipelineDeleted";
    public const string PipelineModified = "Pipelines.PipelineModified";
    public const string DeploymentJobCompleted = "Pipelines.DeploymentJobCompleted";
    public const string PipelineGeneralSettingChanged = "Pipelines.PipelineGeneralSettingChanged";
    public const string PipelineRetentionSettingChanged = "Pipelines.PipelineRetentionSettingChanged";
    public const string HostedParallelismZero = "Pipelines.HostedParallelismZero";
    public const string HostedParallelismPaid = "Pipelines.HostedParallelismPaid";
    public const string HostedParallelismPrivate = "Pipelines.HostedParallelismPrivate";
    public const string HostedParallelismPublic = "Pipelines.HostedParallelismPublic";
    public const string RunRetained = "Pipelines.RunRetained";
    public const string RunUnretained = "Pipelines.RunUnretained";
    public const string ProjectSettings = "Pipelines.ProjectSettings";
    public const string OrganizationSettings = "Pipelines.OrganizationSettings";
    public const string NewValue = "NewValue";
    public const string OldValue = "OldValue";
    public const string PipelineId = "PipelineId";
    public const string PipelineName = "PipelineName";
    public const string PipelineRevision = "PipelineRevision";
    public const string PipelineScope = "PipelineScope";
    public const string ResourceId = "ResourceId";
    public const string ResourceType = "ResourceType";
    public const string RetentionLeaseId = "RetentionLeaseId";
    public const string RetentionOwnerId = "RetentionOwnerId";
    public const string RunId = "RunId";
    public const string RunName = "RunName";
    public const string SettingName = "SettingName";
  }
}
