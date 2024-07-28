// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.DesignerProcessTargetExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class DesignerProcessTargetExtensions
  {
    public static Microsoft.TeamFoundation.Build2.Server.DesignerProcessTarget ToServerDesignerProcessTarget(
      this Microsoft.TeamFoundation.Build.WebApi.DesignerProcessTarget webApiProcessTarget)
    {
      return DesignerProcessTargetExtensions.ToServerProcessTarget<Microsoft.TeamFoundation.Build.WebApi.DesignerProcessTarget, Microsoft.TeamFoundation.Build2.Server.DesignerProcessTarget>(webApiProcessTarget);
    }

    public static Microsoft.TeamFoundation.Build.WebApi.DesignerProcessTarget ToWebApiDesignerProcessTarget(
      this Microsoft.TeamFoundation.Build2.Server.DesignerProcessTarget srvDesignerProcessTarget,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      return DesignerProcessTargetExtensions.ToWebApiProcessTarget<Microsoft.TeamFoundation.Build.WebApi.DesignerProcessTarget, Microsoft.TeamFoundation.Build2.Server.DesignerProcessTarget>(srvDesignerProcessTarget, requestContext, securedObject, (Func<ISecuredObject, Microsoft.TeamFoundation.Build.WebApi.DesignerProcessTarget>) (s => new Microsoft.TeamFoundation.Build.WebApi.DesignerProcessTarget(s)));
    }

    public static Microsoft.TeamFoundation.Build2.Server.DockerProcessTarget ToServerDockerProcessTarget(
      this Microsoft.TeamFoundation.Build.WebApi.DockerProcessTarget webApiProcessTarget)
    {
      return DesignerProcessTargetExtensions.ToServerProcessTarget<Microsoft.TeamFoundation.Build.WebApi.DockerProcessTarget, Microsoft.TeamFoundation.Build2.Server.DockerProcessTarget>(webApiProcessTarget);
    }

    public static Microsoft.TeamFoundation.Build.WebApi.DockerProcessTarget ToWebApiDockerProcessTarget(
      this Microsoft.TeamFoundation.Build2.Server.DockerProcessTarget srvDesignerProcessTarget,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      return DesignerProcessTargetExtensions.ToWebApiProcessTarget<Microsoft.TeamFoundation.Build.WebApi.DockerProcessTarget, Microsoft.TeamFoundation.Build2.Server.DockerProcessTarget>(srvDesignerProcessTarget, requestContext, securedObject, (Func<ISecuredObject, Microsoft.TeamFoundation.Build.WebApi.DockerProcessTarget>) (s => new Microsoft.TeamFoundation.Build.WebApi.DockerProcessTarget(s)));
    }

    private static ServerT ToServerProcessTarget<WebT, ServerT>(WebT webApiProcessTarget)
      where WebT : Microsoft.TeamFoundation.Build.WebApi.DesignerProcessTarget
      where ServerT : Microsoft.TeamFoundation.Build2.Server.DesignerProcessTarget, new()
    {
      if ((object) webApiProcessTarget == null)
        return default (ServerT);
      ServerT serverProcessTarget = new ServerT();
      if (webApiProcessTarget.AgentSpecification != null)
        serverProcessTarget.AgentSpecification = webApiProcessTarget.AgentSpecification.ToServerAgentSpecification();
      return serverProcessTarget;
    }

    private static WebT ToWebApiProcessTarget<WebT, ServerT>(
      ServerT srvProcessTarget,
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Func<ISecuredObject, WebT> initializer)
      where WebT : Microsoft.TeamFoundation.Build.WebApi.DesignerProcessTarget
      where ServerT : Microsoft.TeamFoundation.Build2.Server.DesignerProcessTarget
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if ((object) srvProcessTarget == null)
        return default (WebT);
      WebT apiProcessTarget = initializer(securedObject);
      if (srvProcessTarget.AgentSpecification != null)
        apiProcessTarget.AgentSpecification = srvProcessTarget.AgentSpecification.ToWebApiAgentSpecification(requestContext, securedObject);
      return apiProcessTarget;
    }
  }
}
