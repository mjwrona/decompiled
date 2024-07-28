// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelinesUrlResolver
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;
using Microsoft.TeamFoundation.ServiceHooks.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [ExtensionPriority(50)]
  [ExtensionStrategy("Hosted")]
  internal class PipelinesUrlResolver : ExternalServiceEventUrlResolver
  {
    protected override string Layer => nameof (PipelinesUrlResolver);

    public override string Name => "Pipelines";

    protected override string AccessMappingName => PipelineConstants.AccessMappingName;

    protected override bool UseExactMatch => false;

    protected override IHostIdMappingProviderData GetProviderData(IVssRequestContext requestContext) => (IHostIdMappingProviderData) requestContext.GetService<IPipelineSourceProviderService>().GetProvider(HttpContext.Current.Request.QueryString["provider"], false);

    protected override void LogResolveHostTrace(
      IVssRequestContext requestContext,
      Uri requestUri,
      ExternalServiceEventUrlResolver.EventTracingInfo eventTracingInfo)
    {
      if (eventTracingInfo == null)
        return;
      IVssRequestContext requestContext1 = requestContext;
      string pipelineEventId = PipelineEventLogger.GetPipelineEventId(requestContext, eventTracingInfo.Headers);
      Dictionary<string, string> properties = new Dictionary<string, string>();
      string routedTo = PipelineEventProperties.RoutedTo;
      Guid? routeHostId = eventTracingInfo.RouteHostId;
      ref Guid? local = ref routeHostId;
      string str = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      properties[routedTo] = str;
      properties[PipelineEventProperties.Payload] = eventTracingInfo.Serialize<ExternalServiceEventUrlResolver.EventTracingInfo>(true);
      PipelineEventLogger.LogWithoutEvent(requestContext1, PipelineEventType.XPipesRouted, pipelineEventId, (IDictionary<string, string>) properties);
    }
  }
}
