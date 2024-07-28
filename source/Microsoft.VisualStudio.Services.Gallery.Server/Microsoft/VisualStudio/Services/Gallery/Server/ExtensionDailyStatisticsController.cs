// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionDailyStatisticsController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "stats")]
  public class ExtensionDailyStatisticsController : TfsApiController
  {
    private const string c_traceArea = "WebApi";
    private const string c_activityLogArea = "ExtensionDailyStatisticsController";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotSupportedException>(HttpStatusCode.MethodNotAllowed);
      exceptionMap.AddStatusCode<InvalidAccessException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<InvalidOperationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ExtensionDailyStatsVersionMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionDailyStatsNotSupportedException>(HttpStatusCode.MethodNotAllowed);
      exceptionMap.AddStatusCode<ExtensionDailyStatsAccessDeniedException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<ExtensionDailyStatsAnonymousAccessException>(HttpStatusCode.Unauthorized);
    }

    [HttpGet]
    [ClientLocationId("AE06047E-51C5-4FB4-AB65-7BE488544416")]
    public ExtensionDailyStats GetExtensionDailyStats(
      string publisherName,
      string extensionName,
      [FromUri] int? days = null,
      [FromUri] ExtensionStatsAggregateType? aggregate = null,
      [FromUri] DateTime? afterDate = null)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      this.TfsRequestContext.Trace(12061090, TraceLevel.Info, "gallery", this.LayerName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Controller params, publisherName:{0}, extensionName:{1}, days:{2}, afterDate:{3}, aggregate: {4}", (object) publisherName, (object) extensionName, (object) days, (object) afterDate, (object) aggregate));
      ExtensionDailyStats extensionDailyStats = this.TfsRequestContext.GetService<IExtensionDailyStatsService>().GetExtensionDailyStats(this.TfsRequestContext, publisherName, extensionName, days, aggregate, afterDate);
      if (extensionDailyStats != null)
        extensionDailyStats.ExtensionId = new Guid();
      return extensionDailyStats;
    }

    [HttpGet]
    [ClientLocationId("4FA7ADB6-CA65-4075-A232-5F28323288EA")]
    public ExtensionDailyStats GetExtensionDailyStatsAnonymous(
      string publisherName,
      string extensionName,
      string version)
    {
      throw new InvalidAccessException(GalleryResources.ExtensionDailyStatsActionNotSupported());
    }

    [HttpPost]
    [ClientLocationId("4FA7ADB6-CA65-4075-A232-5F28323288EA")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage IncrementExtensionDailyStat(
      string publisherName,
      string extensionName,
      string version,
      [FromUri] string statType,
      [FromUri] string targetPlatform = null)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      ArgumentUtility.CheckForNull<string>(version, nameof (version));
      if (!this.ShouldLogStat(this.TfsRequestContext))
        return this.Request.CreateResponse(HttpStatusCode.NoContent);
      DailyStatType statTypeOrThrow = ExtensionStatTypeRequestParamHelper.GetStatTypeOrThrow(this.TfsRequestContext, publisherName, extensionName, version, statType, this.LayerName);
      IExtensionDailyStatsService service = this.TfsRequestContext.GetService<IExtensionDailyStatsService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      string version1 = version;
      int statType1 = (int) statTypeOrThrow;
      string str = targetPlatform;
      DateTime? statDate = new DateTime?();
      string targetPlatform1 = str;
      service.IncrementStatCount(tfsRequestContext, publisherName1, extensionName1, version1, (DailyStatType) statType1, statDate, targetPlatform1);
      return this.Request.CreateResponse(HttpStatusCode.Created);
    }

    private bool ShouldLogStat(IVssRequestContext requestContext)
    {
      string userAgent = requestContext.UserAgent;
      return string.IsNullOrEmpty(userAgent) || !userAgent.ToUpperInvariant().Contains("GomezAgent".ToUpperInvariant());
    }

    public override string ActivityLogArea => nameof (ExtensionDailyStatisticsController);

    public override string TraceArea => "WebApi";

    public string LayerName => nameof (ExtensionDailyStatisticsController);
  }
}
