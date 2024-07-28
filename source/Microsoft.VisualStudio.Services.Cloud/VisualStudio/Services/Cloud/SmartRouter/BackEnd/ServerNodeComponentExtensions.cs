// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.ServerNodeComponentExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd
{
  internal static class ServerNodeComponentExtensions
  {
    public static ServerNodeRecord AddOrUpdateServer(
      this IVssRequestContext requestContext,
      ServerNodeRecord backEndServer)
    {
      using (ServerNodeComponent serverComponent = requestContext.CreateServerComponent())
        return serverComponent.AddOrUpdateServer(backEndServer);
    }

    public static ServerNodeRecord DeleteServer(
      this IVssRequestContext requestContext,
      string roleName,
      string roleInstance)
    {
      using (ServerNodeComponent serverComponent = requestContext.CreateServerComponent())
        return serverComponent.DeleteServer(roleName, roleInstance);
    }

    public static IEnumerable<ServerNodeRecord> GetActiveServers(
      this IVssRequestContext requestContext,
      string roleName)
    {
      using (ServerNodeComponent serverComponent = requestContext.CreateServerComponent())
        return serverComponent.GetActiveServers(roleName);
    }

    public static IEnumerable<ServerNodeRecord> PurgeExpiredServers(
      this IVssRequestContext requestContext,
      string roleName)
    {
      using (ServerNodeComponent serverComponent = requestContext.CreateServerComponent())
        return serverComponent.PurgeExpiredServers(roleName);
    }

    private static ServerNodeComponent CreateServerComponent(this IVssRequestContext requestContext)
    {
      requestContext.CheckRequestContext();
      return requestContext.ToDeploymentHostContext().CreateComponent<ServerNodeComponent>();
    }
  }
}
