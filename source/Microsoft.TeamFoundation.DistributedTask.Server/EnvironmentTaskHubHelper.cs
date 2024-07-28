// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.EnvironmentTaskHubHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class EnvironmentTaskHubHelper
  {
    public static OrchestrationHubDescription EnsureHubExists(
      IVssRequestContext requestContext,
      string hubName)
    {
      OrchestrationHubDescription orchestrationHubDescription = (OrchestrationHubDescription) null;
      OrchestrationService service = requestContext.GetService<OrchestrationService>();
      try
      {
        orchestrationHubDescription = service.GetHubDescription(requestContext, hubName);
      }
      catch (OrchestrationHubNotFoundException ex)
      {
      }
      if (orchestrationHubDescription == null)
      {
        OrchestrationHubDescription description = new OrchestrationHubDescription()
        {
          CompressionSettings = new CompressionSettings()
          {
            Style = CompressionStyle.Threshold,
            ThresholdInBytes = 32768
          },
          HubName = hubName,
          HubType = EnvironmentConstants.EnvironmentHubType
        };
        try
        {
          orchestrationHubDescription = service.CreateHub(requestContext, description);
        }
        catch (OrchestrationHubExistsException ex)
        {
          orchestrationHubDescription = service.GetHubDescription(requestContext, hubName);
        }
      }
      return orchestrationHubDescription;
    }

    public static async Task PublishOrchestrationEvent(
      IVssRequestContext requestContext,
      string hubName,
      string orchestrationId,
      string eventName,
      object eventData)
    {
      EnvironmentTaskHubHelper.EnsureHubExists(requestContext, hubName);
      requestContext.TraceInfo("OrchestrationEvent", "Raising Event to OrchestrationId: " + orchestrationId);
      OrchestrationService service = requestContext.GetService<OrchestrationService>();
      IVssRequestContext requestContext1 = requestContext;
      string hubName1 = hubName;
      OrchestrationInstance instance = new OrchestrationInstance();
      instance.InstanceId = orchestrationId;
      string eventName1 = eventName;
      object eventData1 = eventData;
      DateTime? fireAt = new DateTime?();
      await service.RaiseEventAsync(requestContext1, hubName1, instance, eventName1, eventData1, false, fireAt);
    }
  }
}
