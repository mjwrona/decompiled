// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventTransformController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "notification", ResourceName = "EventTransforms")]
  public class EventTransformController : NotificationControllerBase
  {
    [HttpPost]
    [ClientInternalUseOnly(false)]
    public EventTransformResult TransformEvent(EventTransformRequest transformRequest)
    {
      ArgumentUtility.CheckForNull<EventTransformRequest>(transformRequest, nameof (transformRequest));
      ArgumentUtility.CheckStringForNullOrEmpty(transformRequest.EventType, "transformRequest.eventType");
      ArgumentUtility.CheckStringForNullOrEmpty(transformRequest.EventPayload, "transformRequest.eventPayload");
      return this.TfsRequestContext.GetService<INotificationTransformService>().TransformSampleEvent(this.TfsRequestContext, transformRequest);
    }
  }
}
