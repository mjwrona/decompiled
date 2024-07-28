// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.PhaseExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class PhaseExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.Phase ToWebApiPhase(
      this Microsoft.TeamFoundation.Build2.Server.Phase srvPhase,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvPhase == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Phase) null;
      Microsoft.TeamFoundation.Build.WebApi.Phase webApiPhase = new Microsoft.TeamFoundation.Build.WebApi.Phase(securedObject)
      {
        Name = srvPhase.Name,
        RefName = srvPhase.RefName,
        Condition = srvPhase.Condition,
        JobAuthorizationScope = (Microsoft.TeamFoundation.Build.WebApi.BuildAuthorizationScope) srvPhase.JobAuthorizationScope,
        JobTimeoutInMinutes = srvPhase.JobTimeoutInMinutes,
        JobCancelTimeoutInMinutes = srvPhase.JobCancelTimeoutInMinutes
      };
      if (srvPhase.Target != null)
        webApiPhase.Target = srvPhase.Target.ToWebApiPhaseTarget(requestContext, securedObject);
      if (srvPhase.Steps != null)
        webApiPhase.Steps = srvPhase.Steps.Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>) (x => x.ToWebApiBuildDefinitionStep(securedObject))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>();
      if (srvPhase.Variables != null)
        webApiPhase.Variables = (IDictionary<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>) srvPhase.Variables.ToWebApiVariables(securedObject);
      if (srvPhase.Dependencies != null)
        webApiPhase.Dependencies = srvPhase.Dependencies.Select<Microsoft.TeamFoundation.Build2.Server.Dependency, Microsoft.TeamFoundation.Build.WebApi.Dependency>((Func<Microsoft.TeamFoundation.Build2.Server.Dependency, Microsoft.TeamFoundation.Build.WebApi.Dependency>) (x => x.ToWebApiDependency(securedObject))).ToList<Microsoft.TeamFoundation.Build.WebApi.Dependency>();
      return webApiPhase;
    }

    public static Microsoft.TeamFoundation.Build2.Server.Phase ToBuildServerPhase(
      this Microsoft.TeamFoundation.Build.WebApi.Phase webApiPhase)
    {
      if (webApiPhase == null)
        return (Microsoft.TeamFoundation.Build2.Server.Phase) null;
      Microsoft.TeamFoundation.Build2.Server.Phase buildServerPhase = new Microsoft.TeamFoundation.Build2.Server.Phase()
      {
        Name = webApiPhase.Name,
        RefName = webApiPhase.RefName,
        Steps = webApiPhase.Steps.Select<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep>) (x => x.ToBuildServerBuildDefinitionStep())).ToList<Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep>(),
        Condition = webApiPhase.Condition,
        JobAuthorizationScope = (Microsoft.TeamFoundation.Build2.Server.BuildAuthorizationScope) webApiPhase.JobAuthorizationScope,
        JobTimeoutInMinutes = webApiPhase.JobTimeoutInMinutes,
        JobCancelTimeoutInMinutes = webApiPhase.JobCancelTimeoutInMinutes,
        Variables = (IDictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>) webApiPhase.Variables.ToServerVariables(),
        Dependencies = webApiPhase.Dependencies.Select<Microsoft.TeamFoundation.Build.WebApi.Dependency, Microsoft.TeamFoundation.Build2.Server.Dependency>((Func<Microsoft.TeamFoundation.Build.WebApi.Dependency, Microsoft.TeamFoundation.Build2.Server.Dependency>) (x => x.ToBuildServerDependency())).ToList<Microsoft.TeamFoundation.Build2.Server.Dependency>()
      };
      if (webApiPhase.Target != null)
        buildServerPhase.Target = webApiPhase.Target.ToServerPhaseTarget();
      return buildServerPhase;
    }
  }
}
