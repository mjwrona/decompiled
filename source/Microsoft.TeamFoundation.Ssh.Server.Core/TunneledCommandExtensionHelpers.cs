// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.TunneledCommandExtensionHelpers
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public static class TunneledCommandExtensionHelpers
  {
    public static IVssRequestContext CreateRequestContext(
      this IVssRequestContext deploymentRequestContext,
      Guid hostId)
    {
      return deploymentRequestContext.CreateRequestContext(hostId, (IdentityDescriptor) null);
    }

    public static IVssRequestContext CreateRequestContext(
      this IVssRequestContext deploymentRequestContext,
      Guid hostId,
      IdentityDescriptor identityDescriptor)
    {
      deploymentRequestContext.TraceEnter(13000950, TunneledCommandExtensionHelpers.s_Area, TunneledCommandExtensionHelpers.s_Layer, nameof (CreateRequestContext));
      try
      {
        if (!deploymentRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          deploymentRequestContext.Trace(13000951, TraceLevel.Error, TunneledCommandExtensionHelpers.s_Area, TunneledCommandExtensionHelpers.s_Layer, string.Format("Expected a deployment-level request context but got {0} instead.", (object) deploymentRequestContext.ServiceHost.HostType));
          throw new UnexpectedHostTypeException(deploymentRequestContext.ServiceHost.HostType);
        }
        ITeamFoundationHostManagementService service = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>();
        return !(identityDescriptor != (IdentityDescriptor) null) ? service.BeginRequest(deploymentRequestContext, hostId, RequestContextType.SystemContext) : service.BeginUserRequest(deploymentRequestContext, hostId, identityDescriptor);
      }
      finally
      {
        deploymentRequestContext.TraceLeave(13000955, TunneledCommandExtensionHelpers.s_Area, TunneledCommandExtensionHelpers.s_Layer, nameof (CreateRequestContext));
      }
    }

    public static string s_Layer => "Ssh";

    public static string s_Area => nameof (TunneledCommandExtensionHelpers);
  }
}
