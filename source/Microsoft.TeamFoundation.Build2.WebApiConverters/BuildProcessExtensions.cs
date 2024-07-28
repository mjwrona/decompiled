// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildProcessExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildProcessExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.YamlProcess ToWebApiYamlProcess(
      this Microsoft.TeamFoundation.Build2.Server.YamlProcess srvYamlProcess,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvYamlProcess == null)
        return (Microsoft.TeamFoundation.Build.WebApi.YamlProcess) null;
      Microsoft.TeamFoundation.Build.WebApi.YamlProcess webApiYamlProcess = new Microsoft.TeamFoundation.Build.WebApi.YamlProcess(securedObject);
      webApiYamlProcess.Type = srvYamlProcess.Type;
      webApiYamlProcess.YamlFilename = srvYamlProcess.YamlFilename;
      return webApiYamlProcess;
    }

    public static Microsoft.TeamFoundation.Build2.Server.YamlProcess ToServerYamlProcess(
      this Microsoft.TeamFoundation.Build.WebApi.YamlProcess webApiYamlProcess)
    {
      if (webApiYamlProcess == null)
        return (Microsoft.TeamFoundation.Build2.Server.YamlProcess) null;
      Microsoft.TeamFoundation.Build2.Server.YamlProcess serverYamlProcess = new Microsoft.TeamFoundation.Build2.Server.YamlProcess();
      serverYamlProcess.Type = webApiYamlProcess.Type;
      serverYamlProcess.YamlFilename = webApiYamlProcess.YamlFilename;
      return serverYamlProcess;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.DesignerProcess ToWebApiDesignerProcess(
      this Microsoft.TeamFoundation.Build2.Server.DesignerProcess srvDesignerProcess,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvDesignerProcess == null)
        return (Microsoft.TeamFoundation.Build.WebApi.DesignerProcess) null;
      using (PerformanceTimer.StartMeasure(requestContext, "BuildProcessExtensions.ToWebApiDesignerProcess"))
      {
        Microsoft.TeamFoundation.Build.WebApi.DesignerProcess apiDesignerProcess = new Microsoft.TeamFoundation.Build.WebApi.DesignerProcess(securedObject);
        if (srvDesignerProcess.Phases != null)
        {
          foreach (Microsoft.TeamFoundation.Build2.Server.Phase phase in srvDesignerProcess.Phases)
            apiDesignerProcess.Phases.Add(phase.ToWebApiPhase(requestContext, securedObject));
        }
        if (srvDesignerProcess.Target != null)
          apiDesignerProcess.Target = srvDesignerProcess.Target.ToWebApiDesignerProcessTarget(requestContext, securedObject);
        return apiDesignerProcess;
      }
    }

    public static Microsoft.TeamFoundation.Build2.Server.DesignerProcess ToServerDesignerProcess(
      this Microsoft.TeamFoundation.Build.WebApi.DesignerProcess webApiDesignerProcess)
    {
      if (webApiDesignerProcess == null)
        return (Microsoft.TeamFoundation.Build2.Server.DesignerProcess) null;
      Microsoft.TeamFoundation.Build2.Server.DesignerProcess designerProcess = new Microsoft.TeamFoundation.Build2.Server.DesignerProcess();
      designerProcess.Type = webApiDesignerProcess.Type;
      Microsoft.TeamFoundation.Build2.Server.DesignerProcess serverDesignerProcess = designerProcess;
      if (webApiDesignerProcess.Target != null)
        serverDesignerProcess.Target = webApiDesignerProcess.Target.ToServerDesignerProcessTarget();
      if (webApiDesignerProcess.Phases != null)
      {
        foreach (Microsoft.TeamFoundation.Build.WebApi.Phase phase in webApiDesignerProcess.Phases)
          serverDesignerProcess.Phases.Add(phase.ToBuildServerPhase());
      }
      return serverDesignerProcess;
    }
  }
}
