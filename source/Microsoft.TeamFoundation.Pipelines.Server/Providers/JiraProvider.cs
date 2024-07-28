// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class JiraProvider : IPipelineSourceProvider, IHostIdMappingProviderData
  {
    private const string c_layer = "JiraProvider";

    public JiraProvider(
      IPipelineConnectionCreator connectionsCreator = null,
      IPipelineEventsHandler eventsHandler = null,
      IPipelinesExternalApp externalApp = null,
      IPipelinePullRequestProvider pullRequestProvider = null,
      List<IHostIdMappingRouter> routers = null)
    {
      this.ConnectionCreator = connectionsCreator ?? this.ConnectionCreator;
      this.EventsHandler = eventsHandler ?? this.EventsHandler;
      this.ExternalApp = externalApp ?? this.ExternalApp;
      this.PullRequestProvider = pullRequestProvider ?? this.PullRequestProvider;
      this.Routers = (IReadOnlyList<IHostIdMappingRouter>) routers ?? this.Routers;
    }

    public string ProviderId => JiraProviderConstants.ProviderId;

    public IPipelineConnectionCreator ConnectionCreator { get; } = (IPipelineConnectionCreator) new JiraConnectionCreator();

    public IPipelineEventsHandler EventsHandler { get; } = (IPipelineEventsHandler) new JiraEventsHandler();

    public IPipelinesExternalApp ExternalApp { get; } = (IPipelinesExternalApp) new JiraExternalApp();

    public IPipelinePullRequestProvider PullRequestProvider { get; }

    public IReadOnlyList<IHostIdMappingRouter> Routers { get; } = (IReadOnlyList<IHostIdMappingRouter>) new List<IHostIdMappingRouter>()
    {
      (IHostIdMappingRouter) new JiraAccountNameRouter()
    };

    public string DeliveryIdHeaderName => "X-Jira-Delivery";

    public IReadOnlyList<string> SensitiveHeaderNames => (IReadOnlyList<string>) null;

    public IReadOnlyList<string> AllowedHeaders => (IReadOnlyList<string>) null;
  }
}
