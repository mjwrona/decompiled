// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts.CustomerIntelligenceHelper
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts
{
  public static class CustomerIntelligenceHelper
  {
    public static void PublishResourcesUsageForPipelineRun(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int pipelineId,
      PipelineEnvironment environment,
      PipelineProcess process)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      if (!service.IsTracingEnabled(requestContext) || process == null || environment == null)
        return;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(PipelinePropertyNames.ProjectId, (object) projectId);
      properties.Add(PipelinePropertyNames.DefinitionId, (double) definitionId);
      properties.Add(PipelinePropertyNames.PipelineId, (double) pipelineId);
      IList<Stage> stages = process.Stages;
      IEnumerable<Phase> source = stages != null ? stages.SelectMany<Stage, PhaseNode>((Func<Stage, IEnumerable<PhaseNode>>) (x =>
      {
        IList<PhaseNode> phases = x.Phases;
        return phases == null ? (IEnumerable<PhaseNode>) null : phases.Where<PhaseNode>((Func<PhaseNode, bool>) (y => y is Phase));
      })).Cast<Phase>() : (IEnumerable<Phase>) null;
      foreach (TaskStep taskStep1 in (source != null ? source.SelectMany<Phase, Step>((Func<Phase, IEnumerable<Step>>) (x => x.Steps.Where<Step>((Func<Step, bool>) (y => y is TaskStep)))).Cast<TaskStep>() : (IEnumerable<TaskStep>) null) ?? (IEnumerable<TaskStep>) new List<TaskStep>())
      {
        string taskAlias;
        if (taskStep1 is TaskStep taskStep2 && taskStep2.Inputs.TryGetValue("alias", out taskAlias) && !string.IsNullOrEmpty(taskAlias))
        {
          PipelineResources resources1 = environment.Resources;
          PipelineResource pipelineResource1;
          if (resources1 == null)
          {
            pipelineResource1 = (PipelineResource) null;
          }
          else
          {
            ISet<PipelineResource> pipelines = resources1.Pipelines;
            pipelineResource1 = pipelines != null ? pipelines.FirstOrDefault<PipelineResource>((Func<PipelineResource, bool>) (p => string.Equals(p.Alias, taskAlias, StringComparison.OrdinalIgnoreCase))) : (PipelineResource) null;
          }
          PipelineResource pipelineResource2 = pipelineResource1;
          if (pipelineResource2 != null)
          {
            CustomerIntelligenceHelper.PublishResourceProperties(requestContext, service, properties, (Resource) pipelineResource2, "PipelineResource", "Pipeline");
          }
          else
          {
            PipelineResources resources2 = environment.Resources;
            BuildResource buildResource1;
            if (resources2 == null)
            {
              buildResource1 = (BuildResource) null;
            }
            else
            {
              ISet<BuildResource> builds = resources2.Builds;
              buildResource1 = builds != null ? builds.FirstOrDefault<BuildResource>((Func<BuildResource, bool>) (b => string.Equals(b.Alias, taskAlias, StringComparison.OrdinalIgnoreCase))) : (BuildResource) null;
            }
            BuildResource buildResource2 = buildResource1;
            if (buildResource2 != null)
            {
              CustomerIntelligenceHelper.PublishResourceProperties(requestContext, service, properties, (Resource) buildResource2, "BuildResource", buildResource2.Type);
            }
            else
            {
              PipelineResources resources3 = environment.Resources;
              PackageResource packageResource1;
              if (resources3 == null)
              {
                packageResource1 = (PackageResource) null;
              }
              else
              {
                ISet<PackageResource> packages = resources3.Packages;
                packageResource1 = packages != null ? packages.FirstOrDefault<PackageResource>((Func<PackageResource, bool>) (p => string.Equals(p.Alias, taskAlias, StringComparison.OrdinalIgnoreCase))) : (PackageResource) null;
              }
              PackageResource packageResource2 = packageResource1;
              if (packageResource2 != null)
                CustomerIntelligenceHelper.PublishResourceProperties(requestContext, service, properties, (Resource) packageResource2, "PackageResource", packageResource2.Type);
            }
          }
        }
      }
    }

    private static void PublishResourceProperties(
      IVssRequestContext requestContext,
      CustomerIntelligenceService service,
      CustomerIntelligenceData properties,
      Resource resource,
      string category,
      string type)
    {
      properties.Add("resourceAlias", resource.Alias);
      properties.Add(nameof (category), category);
      properties.Add("artifactType", type);
      service.Publish(requestContext, "PipelineArtifacts", "PipelineResourcesUsageForRunInstance", properties);
    }
  }
}
