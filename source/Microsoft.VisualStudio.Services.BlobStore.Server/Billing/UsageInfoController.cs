// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Billing.UsageInfoController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Billing
{
  [VersionedApiControllerCustomName(Area = "usage", ResourceName = "metrics", ResourceVersion = 1)]
  public sealed class UsageInfoController : TfsApiController
  {
    private const string UsageRangeDateFormat = "yyyy-MM-ddTHH:mm:ss'Z'";

    [HttpGet]
    [ActionName("billing")]
    [ControllerMethodTraceFilter(5707120)]
    public async Task<DataContracts.UsageInfo> GetUsageInfo(string maxTime = null)
    {
      UsageInfoController usageInfoController = this;
      DateTimeOffset maxTime1 = maxTime != null ? DateTimeOffset.ParseExact(maxTime, "yyyy-MM-ddTHH:mm:ss'Z'", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal) : DateTimeOffset.UtcNow;
      return await usageInfoController.TfsRequestContext.GetService<IUsageInfoService>().GetStorageUsageInfo(usageInfoController.TfsRequestContext, maxTime1).ConfigureAwait(true);
    }

    [HttpGet]
    [ActionName("breakdown")]
    [ControllerMethodTraceFilter(5707121)]
    public async Task<DataContracts.RawStorageBreakdownInfo> GetUsageBreakdownInfo(string maxTime = null)
    {
      UsageInfoController usageInfoController = this;
      DateTimeOffset maxTime1 = maxTime != null ? DateTimeOffset.ParseExact(maxTime, "yyyy-MM-ddTHH:mm:ss'Z'", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal) : DateTimeOffset.UtcNow;
      return await usageInfoController.TfsRequestContext.GetService<IUsageInfoService>().GetStorageBreakdownUsageInfo(usageInfoController.TfsRequestContext, maxTime1).ConfigureAwait(true);
    }

    [HttpGet]
    [ActionName("meter")]
    [ControllerMethodTraceFilter(5707122)]
    public async Task<DataContracts.MeterUsageInfo> GetUsageOverMaxQtyUsageInfo(string maxTime = null)
    {
      UsageInfoController usageInfoController = this;
      DateTimeOffset maxTime1 = maxTime != null ? DateTimeOffset.ParseExact(maxTime, "yyyy-MM-ddTHH:mm:ss'Z'", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal) : DateTimeOffset.UtcNow;
      return await usageInfoController.TfsRequestContext.GetService<IUsageInfoService>().GetUsedOverMaxQtyUsageInfo(usageInfoController.TfsRequestContext, maxTime1).ConfigureAwait(true);
    }

    [HttpGet]
    [ActionName("feedbytes")]
    [ControllerMethodTraceFilter(5707123)]
    public async Task<FeedMetric> GetPackageUsageByFeed(string maxTime = null)
    {
      UsageInfoController usageInfoController = this;
      DateTimeOffset maxTime1 = maxTime != null ? DateTimeOffset.ParseExact(maxTime, "yyyy-MM-ddTHH:mm:ss'Z'", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal) : DateTimeOffset.UtcNow;
      return await usageInfoController.TfsRequestContext.GetService<IUsageInfoService>().GetFeedLevelPackageUsageInfo(usageInfoController.TfsRequestContext, maxTime1).ConfigureAwait(true);
    }

    [HttpGet]
    [ActionName("breakdownv2")]
    [ControllerMethodTraceFilter(5707124)]
    public async Task<IDictionary<string, DataContracts.RawStorageBreakdownInfo>> GetUsageBreakdownByDomainInfo(
      string maxTime = null)
    {
      UsageInfoController usageInfoController = this;
      DateTimeOffset maxTime1 = maxTime != null ? DateTimeOffset.ParseExact(maxTime, "yyyy-MM-ddTHH:mm:ss'Z'", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal) : DateTimeOffset.UtcNow;
      return await usageInfoController.TfsRequestContext.GetService<IUsageInfoService>().GetStorageBreakdownUsageInfoByDomain(usageInfoController.TfsRequestContext, maxTime1).ConfigureAwait(true);
    }
  }
}
