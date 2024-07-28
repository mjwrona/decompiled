// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.EventsService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class EventsService : IEventsService, IVssFrameworkService
  {
    private const string c_layer = "EventsService";
    private const string c_pullRequestHandlerJobName = "GitHubPullRequestHandlerJob";
    private const string c_pullRequestHandlerJobClass = "Microsoft.TeamFoundation.Pipelines.Server.Extensions.GitHubPullRequestHandlerJob";
    private Guid m_serviceHostId;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.m_serviceHostId = requestContext.ServiceHost.InstanceId;
      this.ValidateRequestContext(requestContext);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public bool PostEvent(
      IVssRequestContext requestContext,
      string providerId,
      string jsonPayload,
      IDictionary<string, string> headers)
    {
      this.ValidateRequestContext(requestContext);
      using (new Tracer<EventsService>(requestContext, TracePoints.Events.PostEventEnter, TracePoints.Events.PostEventLeave, nameof (PostEvent)))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(jsonPayload, nameof (jsonPayload));
        ArgumentUtility.CheckForNull<IDictionary<string, string>>(headers, nameof (headers));
        IPipelineSourceProvider provider = requestContext.GetService<IPipelineSourceProviderService>().GetProvider(providerId);
        if (!provider.EventsHandler.IsValidEvent(requestContext, jsonPayload, headers))
          throw new ArgumentException(PipelinesResources.ExceptionPayloadSignatureMismatch());
        bool flag = false;
        foreach (IPipelineEventHandler eventHandler in provider.EventsHandler.EventHandlers)
        {
          IPipelineEventHandler handler = eventHandler;
          using (requestContext.TraceSlowCall(nameof (EventsService), 5000, new Lazy<string>((Func<string>) (() => "Handler=" + handler.GetType().Name + "; Provider=" + provider.GetType().Name + " " + providerId + "; ExternalAppId=" + provider.ExternalApp?.AppId)), nameof (PostEvent)))
          {
            requestContext.TraceInfo(TracePoints.Events.HandleEvent, nameof (EventsService), "Calling HandleEvent on {0}", (object) handler.GetType().Name);
            if (handler.HandleEvent(requestContext, provider, jsonPayload, headers))
            {
              flag = true;
              break;
            }
          }
        }
        return flag;
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }
  }
}
