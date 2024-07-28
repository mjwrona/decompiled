// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionStatisticsController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "statistics")]
  public class ExtensionStatisticsController : GalleryController
  {
    [HttpPatch]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("A0EA3204-11E9-422D-A9CA-45851CC41400")]
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "We need to return the response message")]
    public HttpResponseMessage UpdateExtensionStatistics(
      string publisherName,
      string extensionName,
      ExtensionStatisticUpdate extensionStatisticsUpdate)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
      HttpResponseMessage response;
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && userIdentity != null && IdentityHelper.IsWellKnownGroup(userIdentity.Descriptor, GroupWellKnownIdentityDescriptors.ServiceUsersGroup) || this.IsServicePrincipal(this.TfsRequestContext))
      {
        List<ExtensionStatisticUpdate> extensionStatisticUpdateList = new List<ExtensionStatisticUpdate>();
        IExtensionStatisticService service = this.TfsRequestContext.GetService<IExtensionStatisticService>();
        extensionStatisticUpdateList.Add(extensionStatisticsUpdate);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        List<ExtensionStatisticUpdate> statistics = extensionStatisticUpdateList;
        service.UpdateStatistics(tfsRequestContext, (IEnumerable<ExtensionStatisticUpdate>) statistics);
        response = this.Request.CreateResponse(HttpStatusCode.OK);
        if (extensionStatisticsUpdate != null && extensionStatisticsUpdate.Statistic != null && extensionStatisticsUpdate.Operation == ExtensionStatisticOperation.Increment && extensionStatisticsUpdate.Statistic.Value == 1.0 && (extensionStatisticsUpdate.Statistic.StatisticName == "install" || extensionStatisticsUpdate.Statistic.StatisticName == "updateCount"))
          DailyStatsHelper.IncrementInstallCount(this.TfsRequestContext, publisherName, extensionName);
      }
      else
        response = this.Request.CreateResponse(HttpStatusCode.Unauthorized);
      return response;
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Request context is always available")]
    protected internal virtual bool IsServicePrincipal(IVssRequestContext requestContext) => ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext);
  }
}
