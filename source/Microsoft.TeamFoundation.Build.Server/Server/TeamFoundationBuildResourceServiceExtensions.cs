// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.TeamFoundationBuildResourceServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TeamFoundationBuildResourceServiceExtensions
  {
    public static BuildServiceHost CreateVirtualServiceHost(
      this TeamFoundationBuildResourceService service,
      IVssRequestContext requestContext,
      string poolType,
      string poolName,
      string controllerName = null)
    {
      BuildServiceHostQueryResult serviceHostQueryResult = service.QueryBuildServiceHosts(requestContext, poolName);
      BuildServiceHost serviceHost = new BuildServiceHost()
      {
        BaseUrl = "http://localhost:9191/" + BuildConstants.DefaultServiceHostUrlPath,
        IsVirtual = true,
        Name = poolName,
        RequireClientCertificates = false,
        Status = ServiceHostStatus.Offline
      };
      BuildServiceHost virtualServiceHost;
      if (serviceHostQueryResult.ServiceHosts.Count > 0 && serviceHostQueryResult.ServiceHosts[0].IsVirtual)
      {
        BuildServiceHostUpdateOptions update = new BuildServiceHostUpdateOptions()
        {
          BaseUrl = serviceHost.BaseUrl,
          RequireClientCertificates = serviceHost.RequireClientCertificates,
          Fields = BuildServiceHostUpdate.BaseUrl | BuildServiceHostUpdate.RequireClientCertificates,
          Uri = serviceHostQueryResult.ServiceHosts[0].Uri,
          Properties = new Dictionary<string, object>()
          {
            {
              "PoolType",
              (object) poolType
            },
            {
              "PoolName",
              (object) poolName
            }
          }
        };
        service.UpdateBuildServiceHost(requestContext, update, true);
        virtualServiceHost = service.QueryBuildServiceHostsByUri(requestContext, (IList<string>) new string[1]
        {
          serviceHostQueryResult.ServiceHosts[0].Uri
        }).ServiceHosts[0];
      }
      else
      {
        serviceHost.Properties = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "PoolType",
            (object) poolType
          },
          {
            "PoolName",
            (object) poolName
          }
        };
        virtualServiceHost = service.AddBuildServiceHost(requestContext, serviceHost);
      }
      BuildController buildController = new BuildController()
      {
        Enabled = true,
        MaxConcurrentBuilds = 0,
        Name = string.IsNullOrEmpty(controllerName) ? ResourceStrings.BuildElasticControllerName() : controllerName,
        ServiceHostUri = virtualServiceHost.Uri,
        Status = ControllerStatus.Offline
      };
      string uri;
      if (serviceHostQueryResult.Controllers.Count > 0)
      {
        uri = serviceHostQueryResult.Controllers[0].Uri;
        BuildControllerUpdateOptions controllerUpdateOptions = new BuildControllerUpdateOptions()
        {
          Enabled = buildController.Enabled,
          MaxConcurrentBuilds = buildController.MaxConcurrentBuilds,
          Uri = uri,
          Fields = BuildControllerUpdate.MaxConcurrentBuilds | BuildControllerUpdate.Enabled
        };
        if (!string.IsNullOrEmpty(controllerName))
        {
          controllerUpdateOptions.Name = controllerName;
          controllerUpdateOptions.Fields |= BuildControllerUpdate.Name;
        }
        service.UpdateBuildControllers(requestContext, (IList<BuildControllerUpdateOptions>) new BuildControllerUpdateOptions[1]
        {
          controllerUpdateOptions
        }, true);
      }
      else
      {
        try
        {
          uri = service.AddBuildControllers(requestContext, (IList<BuildController>) new BuildController[1]
          {
            buildController
          })[0].Uri;
        }
        catch (BuildControllerAlreadyExistsException ex)
        {
          buildController.Name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) buildController.Name, (object) poolName);
          uri = service.AddBuildControllers(requestContext, (IList<BuildController>) new BuildController[1]
          {
            buildController
          })[0].Uri;
        }
      }
      BuildAgent buildAgent = new BuildAgent()
      {
        BuildDirectory = BuildCommonUtil.DefaultHostedWorkingDirectory,
        ControllerUri = uri,
        Enabled = true,
        Name = ResourceStrings.BuildElasticAgentName(),
        ServiceHostUri = virtualServiceHost.Uri,
        Status = AgentStatus.Offline
      };
      if (serviceHostQueryResult.Agents.Count > 1)
        service.DeleteBuildAgents(requestContext, (IList<string>) serviceHostQueryResult.Agents.Skip<BuildAgent>(1).Select<BuildAgent, string>((Func<BuildAgent, string>) (a => a.Uri)).ToList<string>(), true);
      if (serviceHostQueryResult.Agents.Count > 0)
      {
        BuildAgentUpdateOptions agentUpdateOptions = new BuildAgentUpdateOptions()
        {
          BuildDirectory = buildAgent.BuildDirectory,
          ControllerUri = uri,
          Enabled = buildAgent.Enabled,
          Name = buildAgent.Name,
          Uri = serviceHostQueryResult.Agents[0].Uri,
          Fields = BuildAgentUpdate.Name | BuildAgentUpdate.ControllerUri | BuildAgentUpdate.BuildDirectory | BuildAgentUpdate.Enabled
        };
        service.UpdateBuildAgents(requestContext, (IList<BuildAgentUpdateOptions>) new BuildAgentUpdateOptions[1]
        {
          agentUpdateOptions
        }, true);
      }
      else
      {
        try
        {
          service.AddBuildAgents(requestContext, (IList<BuildAgent>) new BuildAgent[1]
          {
            buildAgent
          });
        }
        catch (BuildAgentAlreadyExistsException ex)
        {
          buildAgent.Name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) buildAgent.Name, (object) poolName);
          service.AddBuildAgents(requestContext, (IList<BuildAgent>) new BuildAgent[1]
          {
            buildAgent
          });
        }
      }
      return virtualServiceHost;
    }
  }
}
