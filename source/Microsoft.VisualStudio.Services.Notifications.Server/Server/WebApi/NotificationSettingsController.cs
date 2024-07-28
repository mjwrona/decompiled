// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.WebApi.NotificationSettingsController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server.WebApi
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "notification", ResourceName = "Settings")]
  public class NotificationSettingsController : NotificationControllerBase
  {
    [HttpGet]
    public NotificationAdminSettings GetSettings() => this.TfsRequestContext.GetService<INotificationAdminSettingsService>().GetSettings(this.TfsRequestContext);

    [HttpPatch]
    public NotificationAdminSettings UpdateSettings(
      NotificationAdminSettingsUpdateParameters updateParameters)
    {
      this.LoggableDiagnosticParameters[nameof (updateParameters)] = (object) updateParameters;
      return this.TfsRequestContext.GetService<INotificationAdminSettingsService>().UpdateSettings(this.TfsRequestContext, updateParameters);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<UnsupportedDeliveryPreference>(HttpStatusCode.BadRequest);
    }
  }
}
