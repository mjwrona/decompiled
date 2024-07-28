// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryNotificationsController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "notifications")]
  public class GalleryNotificationsController : TfsApiController
  {
    public string LayerName => nameof (GalleryNotificationsController);

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CustomerContactNotAllowedException>(HttpStatusCode.MethodNotAllowed);
    }

    [HttpPost]
    [ClientLocationId("EAB39817-413C-4602-A49F-07AD00844980")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage SendNotifications([FromBody] NotificationsData notificationData)
    {
      ArgumentUtility.CheckForNull<NotificationsData>(notificationData, nameof (notificationData));
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(notificationData.Identities, "Identities");
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(notificationData.Data, "Data");
      this.TfsRequestContext.GetService<IGalleryNotificationsService>().SendNotifications(this.TfsRequestContext, notificationData);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
