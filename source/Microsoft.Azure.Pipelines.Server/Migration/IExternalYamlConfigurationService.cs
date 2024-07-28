// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.Migration.IExternalYamlConfigurationService
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Server.Migration
{
  [DefaultServiceImplementation("Microsoft.TeamFoundation.Build2.Server.PipelinesMigration.BuildExternalYamlConfigurationService, Microsoft.TeamFoundation.Build2.Server")]
  public interface IExternalYamlConfigurationService : IVssFrameworkService
  {
    YamlPipelineLoadResult LoadYamlPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      Pipeline pipeline);
  }
}
