// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.UpstreamStatusAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking
{
  public class UpstreamStatusAnalyzer : IUpstreamStatusAnalyzer
  {
    private readonly IUpstreamStatusStorage storage;
    private readonly IUpstreamStatusRecordLifecycleProvider lifecycleProvider;

    public UpstreamStatusAnalyzer(
      IUpstreamStatusStorage storage,
      IUpstreamStatusRecordLifecycleProvider lifecycleProvider)
    {
      this.storage = storage;
      this.lifecycleProvider = lifecycleProvider;
    }

    public async Task<IEnumerable<UpstreamHealthStatus>> GetUpstreamStatusesForFeed(
      IProtocolAgnosticFeedRequest request)
    {
      UpstreamStatusAnalyzer upstreamStatusAnalyzer = this;
      // ISSUE: reference to a compiler-generated method
      return (await upstreamStatusAnalyzer.storage.GetUpstreamStatus(request, upstreamStatusAnalyzer.lifecycleProvider.GetEarliestDateTimeToRetrieve(), upstreamStatusAnalyzer.lifecycleProvider.GetLatestDateTimeToRetrieve())).Select<UpstreamStatusRecord, UpstreamHealthStatus>(new Func<UpstreamStatusRecord, UpstreamHealthStatus>(upstreamStatusAnalyzer.\u003CGetUpstreamStatusesForFeed\u003Eb__3_0));
    }

    public async Task ClearOldStatusRecords() => await this.storage.ClearOldStatusRecords(this.lifecycleProvider.GetLatestDateTimeToDelete());

    public UpstreamHealthStatus GetUpstreamStatus(UpstreamStatusRecord upstreamStatusRecord)
    {
      string str = upstreamStatusRecord.Upstream.UpstreamId.ToString();
      if (upstreamStatusRecord.FullRefreshStatus != null && this.IsFullRefreshIsNewerThanPartialStatus(upstreamStatusRecord))
        return new UpstreamHealthStatus()
        {
          UpstreamId = str,
          ConfidentStatus = new StatusDetails()
          {
            StatusCategory = this.GetConfidentStatusCategory((IEnumerable<UpstreamStatusCategory>) upstreamStatusRecord.FullRefreshStatus.Categories),
            TimeStamp = upstreamStatusRecord.FullRefreshStatus.Timestamp
          },
          PartialStatus = (StatusDetails) null
        };
      if (upstreamStatusRecord.PartialRefreshStatuses != null && upstreamStatusRecord.PartialRefreshStatuses.Count > 0)
      {
        PartialRefreshStatusRecord refreshStatusRecord = upstreamStatusRecord.PartialRefreshStatuses.FirstOrDefault<PartialRefreshStatusRecord>((Func<PartialRefreshStatusRecord, bool>) (status => status.Category.Details().CategoryType == CategoryType.Failure));
        if (refreshStatusRecord != null)
          return new UpstreamHealthStatus()
          {
            UpstreamId = str,
            ConfidentStatus = new StatusDetails()
            {
              StatusCategory = refreshStatusRecord.Category,
              TimeStamp = refreshStatusRecord.Timestamp
            },
            PartialStatus = (StatusDetails) null
          };
      }
      PartialRefreshStatusRecord latestPartialStatus = this.GetLatestPartialStatus(upstreamStatusRecord);
      UpstreamHealthStatus upstreamStatus = new UpstreamHealthStatus();
      upstreamStatus.UpstreamId = str;
      StatusDetails statusDetails1;
      if (upstreamStatusRecord.FullRefreshStatus == null)
      {
        statusDetails1 = (StatusDetails) null;
      }
      else
      {
        statusDetails1 = new StatusDetails();
        statusDetails1.StatusCategory = this.GetConfidentStatusCategory((IEnumerable<UpstreamStatusCategory>) upstreamStatusRecord.FullRefreshStatus.Categories);
        statusDetails1.TimeStamp = upstreamStatusRecord.FullRefreshStatus.Timestamp;
      }
      upstreamStatus.ConfidentStatus = statusDetails1;
      StatusDetails statusDetails2;
      if (latestPartialStatus == null)
      {
        statusDetails2 = (StatusDetails) null;
      }
      else
      {
        statusDetails2 = new StatusDetails();
        statusDetails2.StatusCategory = latestPartialStatus.Category;
        statusDetails2.TimeStamp = latestPartialStatus.Timestamp;
      }
      upstreamStatus.PartialStatus = statusDetails2;
      return upstreamStatus;
    }

    public DateTime GetPartialStatusLatestDate(UpstreamStatusRecord upstreamStatusRecord) => upstreamStatusRecord.PartialRefreshStatuses.Max<PartialRefreshStatusRecord, DateTime>((Func<PartialRefreshStatusRecord, DateTime>) (s => s.Timestamp));

    public bool IsFullRefreshIsNewerThanPartialStatus(UpstreamStatusRecord upstreamStatusRecord) => (upstreamStatusRecord.FullRefreshStatus != null ? upstreamStatusRecord.FullRefreshStatus.Timestamp : DateTime.MinValue) > (upstreamStatusRecord.PartialRefreshStatuses == null || upstreamStatusRecord.PartialRefreshStatuses.Count <= 0 ? DateTime.MinValue : this.GetPartialStatusLatestDate(upstreamStatusRecord));

    public PartialRefreshStatusRecord GetLatestPartialStatus(
      UpstreamStatusRecord upstreamStatusRecord)
    {
      IReadOnlyList<PartialRefreshStatusRecord> partialRefreshStatuses = upstreamStatusRecord.PartialRefreshStatuses;
      if (partialRefreshStatuses == null || partialRefreshStatuses.Count <= 0)
        return (PartialRefreshStatusRecord) null;
      if (partialRefreshStatuses.Count<PartialRefreshStatusRecord>() == 1)
        return partialRefreshStatuses.First<PartialRefreshStatusRecord>();
      DateTime timestamp = this.GetPartialStatusLatestDate(upstreamStatusRecord);
      return partialRefreshStatuses.First<PartialRefreshStatusRecord>((Func<PartialRefreshStatusRecord, bool>) (status => status.Timestamp == timestamp));
    }

    public UpstreamStatusCategory GetConfidentStatusCategory(
      IEnumerable<UpstreamStatusCategory> categories)
    {
      if (categories.Count<UpstreamStatusCategory>() == 1)
        return categories.First<UpstreamStatusCategory>();
      UpstreamStatusCategory? categoryWithType1 = this.GetFirstCategoryWithType(categories, CategoryType.Failure);
      if (categoryWithType1.HasValue)
        return categoryWithType1.Value;
      UpstreamStatusCategory? categoryWithType2 = this.GetFirstCategoryWithType(categories, CategoryType.Warning);
      return categoryWithType2.HasValue ? categoryWithType2.Value : categories.First<UpstreamStatusCategory>();
    }

    private UpstreamStatusCategory? GetFirstCategoryWithType(
      IEnumerable<UpstreamStatusCategory> categories,
      CategoryType type)
    {
      IEnumerable<UpstreamStatusCategory> source = categories.Where<UpstreamStatusCategory>((Func<UpstreamStatusCategory, bool>) (c => c.Details().CategoryType == type));
      return source != null && source.Count<UpstreamStatusCategory>() > 0 ? new UpstreamStatusCategory?(source.First<UpstreamStatusCategory>()) : new UpstreamStatusCategory?();
    }
  }
}
