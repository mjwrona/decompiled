// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PersistedNotification.Server.RecipientMetadataController
// Assembly: Microsoft.TeamFoundation.PersistedNotification.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 93EF6375-7D4B-4818-984E-834B7F34DA0F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PersistedNotification.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notification;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.PersistedNotification.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "PersistedNotification", ResourceName = "RecipientMetadata")]
  public class RecipientMetadataController : NotificationApiController
  {
    [HttpGet]
    public HttpResponseMessage GetMetadata() => this.Request.CreateResponse<RecipientMetadata>(HttpStatusCode.OK, this.PersistedNotificationService.GetRecipientMetadata(this.TfsRequestContext));

    [HttpPatch]
    public HttpResponseMessage PatchMetadata(RecipientMetadata metadata) => this.Request.CreateResponse<RecipientMetadata>(HttpStatusCode.OK, this.PersistedNotificationService.UpdateRecipientMetadata(this.TfsRequestContext, metadata));
  }
}
