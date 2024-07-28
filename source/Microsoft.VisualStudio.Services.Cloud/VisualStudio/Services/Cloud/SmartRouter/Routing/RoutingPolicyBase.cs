// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing.RoutingPolicyBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing
{
  internal abstract class RoutingPolicyBase : SmartRouterBase, IRoutingPolicy
  {
    public RoutingPolicyBase(string? policyEnabledFeatureFlag = null)
      : base(SmartRouterBase.TraceLayer.Routing)
    {
      this.PolicyEnabledFeatureFlag = policyEnabledFeatureFlag;
      if (policyEnabledFeatureFlag != null)
        policyEnabledFeatureFlag.CheckArgumentIsNotNullOrEmpty(nameof (policyEnabledFeatureFlag));
      this.PolicyName = this.GetType().Name;
    }

    public ServerNodeWithHash? SelectTargetServerNode(
      IVssRequestContext requestContext,
      SmartRouterContext smartRouterContext,
      IReadOnlyList<ServerNodeWithHash> servers)
    {
      requestContext = requestContext.CheckArgumentIsNotNull<IVssRequestContext>(nameof (requestContext));
      servers = servers.CheckArgumentIsNotNull<IReadOnlyList<ServerNodeWithHash>>(nameof (servers));
      smartRouterContext = smartRouterContext.CheckArgumentIsNotNull<SmartRouterContext>(nameof (smartRouterContext));
      if (!this.PolicyIsEnabled(requestContext))
      {
        smartRouterContext.SetNotRouted("RoutingPolicyDisabled");
        this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.PolicyNotEnabled, "policy={0}, flagName={1}", (object) this.PolicyName, (object) this.PolicyEnabledFeatureFlag);
        return (ServerNodeWithHash) null;
      }
      if (!servers.Any<ServerNodeWithHash>())
      {
        smartRouterContext.SetNotRouted("NoServersAvailable");
        this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.PolicyTryRouteNoServers, "policy={0}", (object) this.PolicyName);
        return (ServerNodeWithHash) null;
      }
      ServerNodeWithHash serverNodeWithHash = this.OnSelectTargetServerNode(requestContext, smartRouterContext, servers);
      if (serverNodeWithHash != null)
      {
        smartRouterContext.SetRouted(serverNodeWithHash.Server.RoleInstance, this.PolicyName);
        this.Tracer.TraceVerbose(requestContext, SmartRouterBase.TracePoint.PolicyTryRouteSuccess, "policy={0}, server={1}", (object) this.PolicyName, (object) serverNodeWithHash.Server.RoleInstance);
      }
      else if (!smartRouterContext.IsStatus("NotRouted"))
        smartRouterContext.SetNotRouted("NoSelectedTargetServer");
      return serverNodeWithHash;
    }

    protected abstract ServerNodeWithHash? OnSelectTargetServerNode(
      IVssRequestContext requestContext,
      SmartRouterContext smartRouterContext,
      IReadOnlyList<ServerNodeWithHash> servers);

    internal bool PolicyIsEnabled(IVssRequestContext requestContext) => this.PolicyEnabledFeatureFlag == null || requestContext.IsFeatureEnabled(this.PolicyEnabledFeatureFlag);

    private string? PolicyEnabledFeatureFlag { get; }

    private string PolicyName { get; }
  }
}
