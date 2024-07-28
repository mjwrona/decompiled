// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.GitHubEventsHandler
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public class GitHubEventsHandler : IPipelineEventsHandler
  {
    private const string c_layer = "GitHubEventsHandler";
    private const string c_headerSignaturePrefix = "sha1=";
    private const string c_signatureSecretNamePrimary = "GitHubLaunchWebhookSecretPrimary";
    private const string c_signatureSecretNameSecondary = "GitHubLaunchWebhookSecretSecondary";

    public bool IsValidEvent(
      IVssRequestContext requestContext,
      string jsonPayload,
      IDictionary<string, string> headers)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ValidateIncomingEvent, nameof (GitHubEventsHandler), "Entering IsValidEvent");
      string signature = headers.GetHeaderValue("X-Hub-Signature").RemovePrefix("sha1=");
      return PipelineHelper.IsValidSignature(requestContext, "GitHubLaunchWebhookSecretPrimary", "GitHubLaunchWebhookSecretSecondary", jsonPayload, signature);
    }

    public bool TryExtractDeliveryId(HttpRequest request, out Guid deliveryId)
    {
      string simpleHeaderValue = request.GetSimpleHeaderValue("X-GitHub-Delivery");
      if (!string.IsNullOrEmpty(simpleHeaderValue) && Guid.TryParse(simpleHeaderValue, out deliveryId))
        return true;
      deliveryId = new Guid();
      return false;
    }

    public IEnumerable<IPipelineEventHandler> EventHandlers { get; } = (IEnumerable<IPipelineEventHandler>) new IPipelineEventHandler[3]
    {
      (IPipelineEventHandler) new GitHubGitEventHandler(),
      (IPipelineEventHandler) new GitHubInstallationEventHandler(),
      (IPipelineEventHandler) new GitHubCommentHandler()
    };

    internal static class Constants
    {
      public const string AppName = "azure-pipelines";
    }
  }
}
