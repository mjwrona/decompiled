// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IUsageInfoService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Billing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  [DefaultServiceImplementation(typeof (UsageInfoService))]
  public interface IUsageInfoService : IVssFrameworkService
  {
    Task<DataContracts.UsageInfo> GetStorageUsageInfo(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime);

    Task<DataContracts.RawStorageBreakdownInfo> GetStorageBreakdownUsageInfo(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime);

    Task<IDictionary<string, DataContracts.RawStorageBreakdownInfo>> GetStorageBreakdownUsageInfoByDomain(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime);

    Task<DataContracts.MeterUsageInfo> GetUsedOverMaxQtyUsageInfo(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime);

    Task<FeedMetric> GetFeedLevelPackageUsageInfo(
      IVssRequestContext requestContext,
      DateTimeOffset maxTime);
  }
}
