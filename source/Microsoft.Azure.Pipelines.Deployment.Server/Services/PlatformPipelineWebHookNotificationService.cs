// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.PlatformPipelineWebHookNotificationService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Extensions;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts;
using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public class PlatformPipelineWebHookNotificationService : 
    IPipelineWebHookNotificationService,
    IVssFrameworkService
  {
    public void PublishEvent(IVssRequestContext requestContext, VssNotificationEvent notification)
    {
      if (!string.Equals(notification.EventType, "ms.vss-pipelines.stage-completed-event", StringComparison.OrdinalIgnoreCase) && !string.Equals(notification.EventType, "ms.vss-pipelines.pipeline-completed-event", StringComparison.OrdinalIgnoreCase) && (!string.Equals(notification.EventType, "ms.vss-pipelines.pipeline-tags-added-event", StringComparison.OrdinalIgnoreCase) || !requestContext.IsFeatureEnabled("DistributedTask.EnableTagBasedFilteringForPipelineTriggers")))
        return;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      XmlNode jsonXmlNode = notification.SerializeToJsonXmlNode();
      IVssRequestContext requestContext1 = requestContext;
      string jobName = typeof (PipelineWebHookNotificationJob).Namespace;
      string fullName = typeof (PipelineWebHookNotificationJob).FullName;
      XmlNode jobData = jsonXmlNode;
      Guid guid = service.QueueOneTimeJob(requestContext1, jobName, fullName, jobData, true);
      requestContext.Trace(100161010, TraceLevel.Info, "Deployment", "ArtifactTrigger", "Queued Job with Id {0} for webHook notification", (object) guid);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
