// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.IYamlPipelineLoaderService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (YamlPipelineLoaderService))]
  public interface IYamlPipelineLoaderService : IVssFrameworkService
  {
    YamlPipelineLoadResult Load(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      string filePath,
      PipelineBuilder builder,
      int? defaultQueueId = null,
      CheckoutOptions defaultCheckoutOptions = null,
      WorkspaceOptions defaultWorkspaceOptions = null,
      IYamlNameFormatter nameFormatter = null,
      RetrieveOptions retrieveOptions = RetrieveOptions.All,
      bool validateResources = true,
      bool validateLogicalBoundaries = false,
      string yamlOverride = null,
      IDictionary<string, object> templateParameters = null,
      IDictionary<string, RepositoryResource> repositoryOverrides = null);
  }
}
