// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDefinitionsToYamlController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "yaml", ResourceVersion = 1)]
  public class BuildDefinitionsToYamlController : BuildApiController
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public virtual YamlBuild GetDefinitionYaml(
      int definitionId,
      int? revision = null,
      DateTime? minMetricsTime = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      bool includeLatestBuilds = false)
    {
      IEnumerable<string> propertyFilters1 = ArtifactPropertyKinds.AsPropertyFilters(propertyFilters);
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId, revision, propertyFilters1, minMetricsTime: minMetricsTime, includeLatestBuilds: includeLatestBuilds);
      if (definition == null || !(definition.Process is Microsoft.TeamFoundation.Build2.Server.DesignerProcess process))
        throw new DefinitionNotFoundException(Resources.DefinitionNotDesigner((object) definitionId));
      PipelineBuildContext buildContext = definition.GetPipelineBuilder(this.TfsRequestContext).CreateBuildContext((BuildOptions) null);
      PipelineProcess pipelineProcess = process.ToPipelineProcess(this.TfsRequestContext, definition, (IPipelineContext) buildContext);
      PipelineParserToYaml pipelineParserToYaml = new PipelineParserToYaml();
      return new YamlBuild((ISecuredObject) definition.ToWebApiBuildDefinition(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion))
      {
        Yaml = pipelineParserToYaml.Convert(this.TfsRequestContext, this.ProjectId, pipelineProcess, definition)
      };
    }
  }
}
