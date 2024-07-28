// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.DockerProcessExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class DockerProcessExtensions
  {
    public static Microsoft.TeamFoundation.Build2.Server.DockerProcess ToServerDockerProcess(
      this Microsoft.TeamFoundation.Build.WebApi.DockerProcess source)
    {
      if (source == null)
        return (Microsoft.TeamFoundation.Build2.Server.DockerProcess) null;
      Microsoft.TeamFoundation.Build2.Server.DockerProcess serverDockerProcess = new Microsoft.TeamFoundation.Build2.Server.DockerProcess();
      Microsoft.TeamFoundation.Build.WebApi.DockerProcessTarget target = source.Target;
      serverDockerProcess.Target = target != null ? target.ToServerDockerProcessTarget() : (Microsoft.TeamFoundation.Build2.Server.DockerProcessTarget) null;
      return serverDockerProcess;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.DockerProcess ToWebApiDockerProcess(
      this Microsoft.TeamFoundation.Build2.Server.DockerProcess source,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      if (source == null)
        return (Microsoft.TeamFoundation.Build.WebApi.DockerProcess) null;
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      Microsoft.TeamFoundation.Build.WebApi.DockerProcess apiDockerProcess = new Microsoft.TeamFoundation.Build.WebApi.DockerProcess(securedObject);
      Microsoft.TeamFoundation.Build2.Server.DockerProcessTarget target = source.Target;
      apiDockerProcess.Target = target != null ? target.ToWebApiDockerProcessTarget(requestContext, securedObject) : (Microsoft.TeamFoundation.Build.WebApi.DockerProcessTarget) null;
      return apiDockerProcess;
    }
  }
}
