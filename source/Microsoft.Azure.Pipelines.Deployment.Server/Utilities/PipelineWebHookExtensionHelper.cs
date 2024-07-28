// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Utilities.PipelineWebHookExtensionHelper
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.Utilities
{
  public static class PipelineWebHookExtensionHelper
  {
    public static bool IsLocalWebHook(this WebHook webHook) => webHook != null && !string.IsNullOrEmpty(webHook.UniqueArtifactIdentifier) && webHook.UniqueArtifactIdentifier.StartsWith("self:", StringComparison.OrdinalIgnoreCase);
  }
}
