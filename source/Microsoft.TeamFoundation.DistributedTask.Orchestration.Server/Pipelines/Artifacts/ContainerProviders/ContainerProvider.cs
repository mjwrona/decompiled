// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.ContainerProviders.ContainerProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.ContainerProviders
{
  public class ContainerProvider : IContainerProvider
  {
    public void SetContainerResourceVersion(ContainerResource container, string version)
    {
      if (container == null || string.IsNullOrEmpty(version))
        return;
      string str;
      container.Properties.TryGetValue<string>("type", out str);
      if ("AzureContainerRepository".Equals(str, StringComparison.OrdinalIgnoreCase))
        new ACRArtifactProvider().SetContainerResourceVersion(container, version);
      else
        new DockerArtifactProvider().SetContainerResourceVersion(container, version);
    }

    public void Validate(ContainerResource container)
    {
      if (container == null)
        return;
      string str;
      container.Properties.TryGetValue<string>("type", out str);
      if ("AzureContainerRepository".Equals(str, StringComparison.OrdinalIgnoreCase))
        new ACRArtifactProvider().Validate(container);
      else
        new DockerArtifactProvider().Validate(container);
    }

    public IList<IVariable> GetVariables(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerResource container,
      IDictionary<string, string> triggerProperties = null)
    {
      try
      {
        if (container != null)
        {
          string str;
          container.Properties.TryGetValue<string>("type", out str);
          return "AzureContainerRepository".Equals(str, StringComparison.OrdinalIgnoreCase) ? new ACRArtifactProvider().GetVariables(requestContext, projectId, container, triggerProperties) : new DockerArtifactProvider().GetVariables(requestContext, projectId, container, triggerProperties);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10016122, "DistributedTask", "ContainerArtifact", ex);
      }
      return (IList<IVariable>) new List<IVariable>();
    }

    public void Validate(ISet<ContainerResource> containers)
    {
      if (containers == null)
        return;
      foreach (ContainerResource container in (IEnumerable<ContainerResource>) containers)
        this.Validate(container);
    }

    public IList<IVariable> GetVariables(
      IVssRequestContext requestContext,
      Guid projectId,
      ISet<ContainerResource> containers,
      IDictionary<string, string> triggerProperties = null)
    {
      List<IVariable> variables = new List<IVariable>();
      if (containers != null)
      {
        foreach (ContainerResource container in (IEnumerable<ContainerResource>) containers)
          variables.AddRange((IEnumerable<IVariable>) this.GetVariables(requestContext, projectId, container, triggerProperties));
      }
      return (IList<IVariable>) variables;
    }
  }
}
