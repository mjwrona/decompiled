// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.IPipelineHostIdMappingRouter
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public interface IPipelineHostIdMappingRouter : IHostIdMappingRouter
  {
    bool TryExtractMappingData(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      out HostIdMappingData mappingData);

    bool TryExtractMappingDataForRepositories(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      List<BuildRepository> resourceRepos,
      out List<HostIdMappingData> mappingDataList);

    bool TryExtractMappingDataForSingleRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildRepository repository,
      out HostIdMappingData mappingData);

    bool TryParseRepoIdFromRoutingKey(string name, out string repositoryId);
  }
}
