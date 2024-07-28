// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PipelinesMigration.BuildExternalYamlConfigurationService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Server.Migration;
using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Build2.Server.PipelinesMigration
{
  public class BuildExternalYamlConfigurationService : 
    IExternalYamlConfigurationService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public YamlPipelineLoadResult LoadYamlPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      Pipeline pipeline)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Pipeline>(pipeline, nameof (pipeline));
      if (!(pipeline.GetConfiguration(requestContext) is YamlConfiguration))
        throw new InvalidOperationException(BuildServerResources.YamlConfigurationPipelineRequired());
      return requestContext.GetService<IBuildDefinitionService>().GetDefinition(requestContext, projectId, pipeline.Id).LoadYamlPipeline(requestContext, false);
    }
  }
}
