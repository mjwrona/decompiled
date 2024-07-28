// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.SmartRouterService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter
{
  public sealed class SmartRouterService : 
    SmartRouterFrameworkServiceBase,
    ISmartRouterService,
    IVssFrameworkService
  {
    public SmartRouterService()
      : base(SmartRouterBase.TraceLayer.Common)
    {
    }

    protected override void OnServiceStart(IVssRequestContext requestContext) => SmartRouterService.StartBackEndServices(requestContext);

    public ServerNodeWithHash? GetTargetServerNode(
      IVssRequestContext requestContext,
      SmartRouterContext smartRouterContext)
    {
      requestContext = this.CheckRequestContext(requestContext);
      smartRouterContext = smartRouterContext.CheckArgumentIsNotNull<SmartRouterContext>(nameof (smartRouterContext));
      if (!this.IsEnabled(requestContext))
        return (ServerNodeWithHash) null;
      IReadOnlyList<ServerNodeWithHash> healthyServerNodes = this.GetHealthyServerNodes(requestContext);
      return requestContext.GetService<IRoutingPolicyFactoryService>().GetPolicy(requestContext).SelectTargetServerNode(requestContext, smartRouterContext, healthyServerNodes);
    }

    private IReadOnlyList<ServerNodeWithHash> GetHealthyServerNodes(
      IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentContext = requestContext.ToDeploymentHostContext();
      IServerNodeDiscoveryService discoveryService = deploymentContext.GetServerNodeDiscoveryService();
      IServerNodeHealthService healthService = deploymentContext.GetServerNodeHealthService();
      IVssRequestContext requestContext1 = deploymentContext;
      return (IReadOnlyList<ServerNodeWithHash>) discoveryService.GetDiscoveredServerNodes(requestContext1).Where<ServerNodeWithHash>((Func<ServerNodeWithHash, bool>) (server =>
      {
        bool? nullable = healthService.IsHealthy(deploymentContext, server.Server);
        bool flag = true;
        return nullable.GetValueOrDefault() == flag & nullable.HasValue;
      })).ToList<ServerNodeWithHash>();
    }

    private static void StartBackEndServices(IVssRequestContext requestContext) => requestContext.ToDeploymentHostContext().GetSmartRouterBackEndServices();
  }
}
