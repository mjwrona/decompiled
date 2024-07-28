// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.IPipelineSourceProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public interface IPipelineSourceProvider : IHostIdMappingProviderData
  {
    IPipelineConnectionCreator ConnectionCreator { get; }

    IPipelineEventsHandler EventsHandler { get; }

    IPipelinesExternalApp ExternalApp { get; }

    IPipelinePullRequestProvider PullRequestProvider { get; }
  }
}
