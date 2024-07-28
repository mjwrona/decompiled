// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.WebApi.NotificationReasonsController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server.WebApi
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "notification", ResourceName = "NotificationReasons")]
  [ClientInternalUseOnly(true)]
  public class NotificationReasonsController : NotificationControllerBase
  {
    [HttpGet]
    public NotificationReason GetNotificationReasons(int notificationId) => (NotificationReason) null;

    [HttpGet]
    public IEnumerable<NotificationReason> ListNotificationReasons([ClientParameterAsIEnumerable(typeof (int), ',')] int notificationIds = -1) => (IEnumerable<NotificationReason>) null;
  }
}
