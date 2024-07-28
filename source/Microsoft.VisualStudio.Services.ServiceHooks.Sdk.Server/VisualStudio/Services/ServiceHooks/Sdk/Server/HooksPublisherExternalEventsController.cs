// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherExternalEventsController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "ExternalEvents")]
  [ClientInternalUseOnly(true)]
  public class HooksPublisherExternalEventsController : ServiceHooksPublisherControllerBase
  {
    private const int MaxPostRequestSize = 26214400;
    private static readonly string s_layer = typeof (HooksPublisherExternalEventsController).Name;
    private static readonly string s_area = typeof (HooksPublisherExternalEventsController).Namespace;

    [HttpPost]
    public IEnumerable<PublisherEvent> PublishExternalEvent(string publisherId, string channelId = null)
    {
      try
      {
        ArgumentUtility.CheckForNull<string>(publisherId, nameof (publisherId));
        ServiceHooksPublisher publisher = this.FindPublisher(publisherId);
        ServiceHooksExternalPublisher externalPublisher = publisher is ServiceHooksExternalPublisher ? (ServiceHooksExternalPublisher) publisher : throw new NotSupportedException();
        string end;
        using (RestrictedStream restrictedStream = new RestrictedStream(this.Request.Content.ReadAsStreamAsync().Result, 0L, 26214401L, true))
          end = new StreamReader((Stream) restrictedStream).ReadToEnd();
        IVssRequestContext requestContext = this.TfsRequestContext.Elevate();
        byte[] bytes = Encoding.UTF8.GetBytes(end);
        externalPublisher.ValidateIncomingEventRequest(requestContext, this.ActionContext.Request, bytes);
        JObject payload = JObject.Parse(end);
        string eventTypeFromRequest = externalPublisher.GetEventTypeFromRequest(this.ActionContext.Request, (object) payload);
        PublisherEvent publisherEvent = publisher.HandleReceivedEvent(this.TfsRequestContext, eventTypeFromRequest, (object) payload, channelId);
        if (publisherEvent == null)
          return (IEnumerable<PublisherEvent>) Array.Empty<PublisherEvent>();
        this.TfsRequestContext.RootContext.Items[RequestContextItemsKeys.ForceHostLastUserAccessUpdate] = (object) true;
        return (IEnumerable<PublisherEvent>) new PublisherEvent[1]
        {
          publisherEvent
        };
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(1063410, HooksPublisherExternalEventsController.s_area, HooksPublisherExternalEventsController.s_layer, ex);
        throw;
      }
    }
  }
}
