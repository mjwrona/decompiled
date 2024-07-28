// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.NodeJsPipelineFragmentProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class NodeJsPipelineFragmentProvider : PipelineFragmentProviderBase
  {
    protected override IEnumerable<Template> GetTemplatesInternal(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      DetectedBuildFramework detectedBuildFramework)
    {
      return NodeJsPipelineFragmentProvider.GetTemplateIds(detectedBuildFramework).Select<string, Template>((Func<string, Template>) (id => this.GetTemplate(requestContext, id)));
    }

    private static IEnumerable<string> GetTemplateIds(DetectedBuildFramework detectedBuildFramework)
    {
      yield return TemplateIds.NodeJs;
      yield return TemplateIds.Pipelines.NodeJsExpressWebAppToLinuxOnAzure;
      if (detectedBuildFramework.BuildTargets.Any<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (target => string.Equals(target.Type, NodeJsBuildFrameworkDetector.Settings.WellKnownTypes.AzureFunctionApp, StringComparison.OrdinalIgnoreCase))))
        yield return TemplateIds.Pipelines.NodeJsFunctionAppToLinuxOnAzure;
      if (detectedBuildFramework.Settings.ContainsKey(NodeJsBuildFrameworkDetector.Settings.WellKnownTypes.Gulp))
        yield return TemplateIds.NodeJsWithGulp;
      if (detectedBuildFramework.Settings.ContainsKey(NodeJsBuildFrameworkDetector.Settings.WellKnownTypes.Grunt))
        yield return TemplateIds.NodeJsWithGrunt;
      yield return TemplateIds.Pipelines.NodeJsWithVue;
      yield return TemplateIds.NodeJsWithWebpack;
      yield return TemplateIds.Pipelines.NodeJsWithReact;
      yield return TemplateIds.Pipelines.NodeJsReactWebAppToLinuxOnAzure;
      yield return TemplateIds.NodeJsWithAngular;
    }

    protected override bool IsSupportedFramework(DetectedBuildFramework detectedBuildFramework) => string.Equals(detectedBuildFramework.Id, NodeJsBuildFrameworkDetector.Id, StringComparison.OrdinalIgnoreCase);
  }
}
