// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelineFragmentProviderService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class PipelineFragmentProviderService : 
    IPipelineFragmentProviderService,
    IVssFrameworkService
  {
    private readonly Regex m_vmImageRegex = new Regex("^(\\s+)vmImage:\\s*'[^']+'\\r*$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled, TimeSpan.FromSeconds(1.0));

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IReadOnlyList<Template> GetTemplates(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      DetectedBuildFramework detectedBuildFramework)
    {
      List<Template> list = PipelineFragmentProviderService.GetProviders().SelectMany<IPipelineFragmentProvider, Template>((Func<IPipelineFragmentProvider, IEnumerable<Template>>) (provider => (IEnumerable<Template>) provider.GetTemplates(requestContext, repositoryContext, detectedBuildFramework))).ToList<Template>();
      PipelineFragmentProviderService.AssignWeightsInOrder(detectedBuildFramework, (IEnumerable<Template>) list);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        this.SetTemplatesToNonHostedPool(requestContext, (IEnumerable<Template>) list);
      return (IReadOnlyList<Template>) list;
    }

    private static IReadOnlyList<IPipelineFragmentProvider> GetProviders() => (IReadOnlyList<IPipelineFragmentProvider>) new IPipelineFragmentProvider[8]
    {
      (IPipelineFragmentProvider) new DockerPipelineFragmentProvider(),
      (IPipelineFragmentProvider) new DotNetCorePipelineFragmentProvider(),
      (IPipelineFragmentProvider) new MsBuildPipelineFragmentProvider(),
      (IPipelineFragmentProvider) new NodeJsPipelineFragmentProvider(),
      (IPipelineFragmentProvider) new PhpPipelineFragmentProvider(),
      (IPipelineFragmentProvider) new PythonPipelineFragmentProvider(),
      (IPipelineFragmentProvider) new PowershellPipelineFragmentProvider(),
      (IPipelineFragmentProvider) new FallbackPipelineFragmentProvider()
    };

    private static void AssignWeightsInOrder(
      DetectedBuildFramework detectedBuildFramework,
      IEnumerable<Template> templates)
    {
      foreach ((Template, int) tuple in templates.Reverse<Template>().Select<Template, (Template, int)>((Func<Template, int, (Template, int)>) ((fragment, weight) => (fragment, weight))))
        tuple.Item1.RecommendedWeight = detectedBuildFramework.Weight + tuple.Item2;
    }

    private void SetTemplatesToNonHostedPool(
      IVssRequestContext requestContext,
      IEnumerable<Template> templates)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      string poolName = requestContext.GetService<IDistributedTaskPoolService>().GetAgentPools(requestContext).FirstOrDefault<TaskAgentPool>((Func<TaskAgentPool, bool>) (p => !p.IsHosted))?.Name ?? "Default";
      foreach (Template template in templates)
        template.Content = this.m_vmImageRegex.Replace(template.Content, (MatchEvaluator) (match => match.Groups[1].Value + "name: '" + poolName + "'"));
    }
  }
}
