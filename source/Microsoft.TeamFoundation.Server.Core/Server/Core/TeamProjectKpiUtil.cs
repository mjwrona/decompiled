// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamProjectKpiUtil
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.Telemetry;
using Microsoft.VisualStudio.Telemetry;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal static class TeamProjectKpiUtil
  {
    internal const string c_blocked = "Blocked";
    internal const string c_failed = "Failed";
    internal const string c_succeeded = "Succeeded";

    public static void PublishProjectRenameData(
      IVssRequestContext requestContext,
      DateTime startTime,
      TimeSpan durationTime,
      object blocks = null,
      object failures = null)
    {
      string str = "Succeeded";
      if (blocks != null)
        str = "Blocked";
      else if (failures != null)
        str = "Failed";
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        IVssServiceHost serviceHost = requestContext.To(TeamFoundationHostType.Application).ServiceHost;
        CustomerIntelligenceData properties = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            CustomerIntelligenceProperty.StartTime,
            (object) startTime
          },
          {
            CustomerIntelligenceProperty.Duration,
            (object) durationTime
          },
          {
            CustomerIntelligenceProperty.Outcome,
            (object) str
          },
          {
            CustomerIntelligenceProperty.Failures,
            failures
          },
          {
            CustomerIntelligenceProperty.Blocks,
            blocks
          },
          {
            CustomerIntelligenceProperty.AccountName,
            (object) serviceHost.Name
          },
          {
            CustomerIntelligenceProperty.AccountId,
            (object) serviceHost.InstanceId
          }
        });
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Account, CustomerIntelligenceFeature.ProjectRename, properties);
      }
      else
        context.GetService<TeamFoundationTelemetryService>().PostEvent(new TelemetryEvent(TeamProjectTelemetryEvents.ProjectRenameEvent)
        {
          Properties = {
            {
              TeamProjectTelemetryProperties.StartTime,
              (object) startTime
            },
            {
              TeamProjectTelemetryProperties.Duration,
              (object) durationTime
            },
            {
              TeamProjectTelemetryProperties.Outcome,
              (object) str
            },
            {
              TeamProjectTelemetryProperties.Failures,
              failures
            },
            {
              TeamProjectTelemetryProperties.Blocks,
              blocks
            }
          }
        });
    }
  }
}
