// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks.WebHookConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks
{
  public static class WebHookConstants
  {
    public const string PipelineWebHookExtensionServiceName = "Pipeline";
    public const string ReleaseWebHookExtensionServiceName = "ReleaseManagement";
    public const string NormalizedSelfEndpointUrl = "self";
    public const string WebHookDrawerName = "WebHookDrawer";
    public const string WebhookNameFormat = "Webhook{0}";
    public const string CreateWebHookDataSourceName = "createwebhook";
    public const string RemoveWebHookDataSourceName = "deletewebhook";
    public const string ListWebHookDataSourceName = "listwebhook";
    public const string LinkWebHookDataSourceName = "linkwebhook";
    public const string UnlinkWebHookDataSourceName = "unlinkwebhook";
    public const string WebHookId = "webHookId";
    public const string WebHookName = "webHookName";
    public const string WebHookPayloadUrl = "payloadUrl";
    public const string WebHookSecret = "secret";
    public const string EventPayload = "eventPayload";
    public const string PipelineTriggerType = "pipelineTriggerType";
    public const string HmacSha1AlgorithmPrefix = "sha1=";
  }
}
