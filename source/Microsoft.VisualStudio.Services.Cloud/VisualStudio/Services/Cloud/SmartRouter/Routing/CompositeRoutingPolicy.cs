// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing.CompositeRoutingPolicy
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing
{
  internal class CompositeRoutingPolicy : RoutingPolicyBase
  {
    private readonly IRoutingPolicy[] m_policies;

    public CompositeRoutingPolicy(params IRoutingPolicy[] policies)
      : base()
    {
      this.m_policies = policies.CheckArgumentIsNotNull<IRoutingPolicy[]>(nameof (policies));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) policies, nameof (policies));
    }

    internal IReadOnlyList<IRoutingPolicy> Policies => (IReadOnlyList<IRoutingPolicy>) ((IEnumerable<IRoutingPolicy>) this.m_policies).ToList<IRoutingPolicy>();

    protected override ServerNodeWithHash? OnSelectTargetServerNode(
      IVssRequestContext requestContext,
      SmartRouterContext smartRouterContext,
      IReadOnlyList<ServerNodeWithHash> servers)
    {
      foreach (IRoutingPolicy policy in this.m_policies)
      {
        ServerNodeWithHash serverNodeWithHash = policy.SelectTargetServerNode(requestContext, smartRouterContext, servers);
        if (serverNodeWithHash != null)
          return serverNodeWithHash;
      }
      return (ServerNodeWithHash) null;
    }
  }
}
