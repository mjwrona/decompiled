// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.AgentSpecificationExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class AgentSpecificationExtensions
  {
    public static Microsoft.TeamFoundation.Build2.Server.AgentSpecification ToServerAgentSpecification(
      this Microsoft.TeamFoundation.Build.WebApi.AgentSpecification webApiAgentSpecification)
    {
      if (webApiAgentSpecification == null)
        return (Microsoft.TeamFoundation.Build2.Server.AgentSpecification) null;
      return new Microsoft.TeamFoundation.Build2.Server.AgentSpecification()
      {
        Identifier = webApiAgentSpecification.Identifier
      };
    }

    public static Microsoft.TeamFoundation.Build.WebApi.AgentSpecification ToWebApiAgentSpecification(
      this Microsoft.TeamFoundation.Build2.Server.AgentSpecification srvAgentSpecification,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvAgentSpecification == null)
        return (Microsoft.TeamFoundation.Build.WebApi.AgentSpecification) null;
      return new Microsoft.TeamFoundation.Build.WebApi.AgentSpecification(securedObject)
      {
        Identifier = srvAgentSpecification.Identifier
      };
    }
  }
}
