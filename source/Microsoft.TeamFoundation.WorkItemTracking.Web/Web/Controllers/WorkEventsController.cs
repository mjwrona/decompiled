// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkEventsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "events")]
  public class WorkEventsController : TfsApiController
  {
    private const int MaxPostRequestSize = 26214400;

    public override string ActivityLogArea => "WorkItem Tracking";

    [HttpPost]
    [ClientIgnore]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public HttpResponseMessage Event([FromUri(Name = "$source")] EventSource eventSource = EventSource.GitHubOAuth)
    {
      bool flag1 = this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
      bool flag2 = this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection);
      if (flag1 | flag2)
      {
        if (flag1 && eventSource != EventSource.GitHubApp || flag2 && eventSource != EventSource.GitHubApp && eventSource != EventSource.GitHubOAuth)
          throw new VssPropertyValidationException("$source", ResourceStrings.InvalidEventSource((object) eventSource));
        this.ProcessEventInternal(eventSource);
      }
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    private void ProcessEventInternal(EventSource eventSource)
    {
      string end;
      using (RestrictedStream restrictedStream = new RestrictedStream(this.Request.Content.ReadAsStreamAsync().Result, 0L, 26214401L, true))
      {
        using (StreamReader streamReader = new StreamReader((Stream) restrictedStream))
          end = streamReader.ReadToEnd();
      }
      Dictionary<string, string> dedupedDictionary = this.Request.Headers.ToDedupedDictionary<KeyValuePair<string, IEnumerable<string>>, string, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (header => header.Key), (Func<KeyValuePair<string, IEnumerable<string>>, string>) (header => header.Value.FirstOrDefault<string>()), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      IExternalEventHandlerService service = this.TfsRequestContext.GetService<IExternalEventHandlerService>();
      try
      {
        service.HandleEvent(this.TfsRequestContext, (IDictionary<string, string>) dedupedDictionary, end, eventSource);
      }
      catch (UnsupportedExternalEventHostType ex)
      {
        this.TfsRequestContext.Trace(918062, TraceLevel.Info, "ExternalEvents", "ExternalEventHandlerService", "Ignoring supported event routed to wrong host type");
      }
      catch (UnsupportedExternalEventSourceException ex)
      {
        this.TfsRequestContext.Trace(918061, TraceLevel.Info, "ExternalEvents", "ExternalEventHandlerService", "Ignoring unsupported event source or type");
      }
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<InvalidRequestContextHostException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidPayloadException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<RepoNotConfiguredForGitHubEventsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<GitHubRepoNotConfiguredForProjectEventsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<RepoNotUniqueForGitHubEventsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UnsupportedGitHubEventTypeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProjectDoesNotExistException>(HttpStatusCode.NotFound);
    }
  }
}
