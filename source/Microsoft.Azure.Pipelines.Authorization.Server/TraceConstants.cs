// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.TraceConstants
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  internal static class TraceConstants
  {
    public const string Area = "PipelinePolicy.Authorization";
    public const string ServiceLayer = "Service";
    public const string PipelineResourceAuthorizationService = "PipelineResourceAuthorizationService";
    public const string FeatureIsAuthorizedForAllDefinitions = "PipelineResourceAuthorizationService.IsAuthorizedForAllDefinitions";
    public const string FeatureUpdateAuthorize = "PipelineResourceAuthorizationService.UpdateAuthorize";
    public const int Start = 34002000;
    public const int ResourceAuthorization = 34002000;
    public const int StalePipelinesRemoved = 34002001;
    public const int DefaultAuthorizedResources = 34002002;
    public const int DeletedResourcePermissions = 34002003;
    public const int NotPermittedResources = 34002004;
    public const int InvalidResourceType = 34002005;
    public const int ResourceUnauthorized = 34002006;
    public const int AuthorizationCompletedEventPublishError = 34002007;
    public const int AuthorizationCompletedEventPublishInfo = 34002008;
    public const int UpdatePipelinePermisionsForResourcesFailure = 34002009;
    public const int RemoveStalePipelines = 34002010;
  }
}
