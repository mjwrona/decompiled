// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RefreshPackagesResult
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  [DataContract]
  public class RefreshPackagesResult
  {
    private const int MaximumReportedFailures = 25;

    public RefreshPackagesResult(
      Guid feedId,
      IList<RefreshPackageResult> successes = null,
      IList<RefreshPackageFailure> failures = null,
      IList<RefreshPackageFailure> whitelistedFailures = null,
      IList<UpstreamStatistics> upstreamStatistics = null,
      bool completed = true,
      int upstreamVersionListCacheHits = 0,
      int upstreamVersionListCacheMisses = 0,
      int totalNumberOfPackagesToRefresh = -1,
      int plannedUnitsOfWork = -1)
    {
      this.FeedId = feedId;
      this.Successes = successes ?? (IList<RefreshPackageResult>) new List<RefreshPackageResult>();
      this.Failures = failures ?? (IList<RefreshPackageFailure>) new List<RefreshPackageFailure>();
      this.WhitelistedFailures = whitelistedFailures ?? (IList<RefreshPackageFailure>) new List<RefreshPackageFailure>();
      this.SuccessStatistics = upstreamStatistics ?? (IList<UpstreamStatistics>) new List<UpstreamStatistics>();
      this.Completed = completed;
      this.UpstreamVersionListCacheHits = upstreamVersionListCacheHits;
      this.UpstreamVersionListCacheMisses = upstreamVersionListCacheMisses;
      this.TotalNumberOfPackagesToRefresh = totalNumberOfPackagesToRefresh;
      this.PlannedUnitsOfWork = plannedUnitsOfWork;
    }

    [DataMember]
    public Guid FeedId { get; }

    [DataMember]
    public long PackagesRefreshedCount => (long) this.Successes.Count<RefreshPackageResult>((Func<RefreshPackageResult, bool>) (x => x.RefreshNeeded));

    [DataMember]
    public long PackagesUpToDateCount => (long) this.Successes.Count<RefreshPackageResult>((Func<RefreshPackageResult, bool>) (x => !x.RefreshNeeded));

    [DataMember]
    public long TotalUpstreamVersionsDelta => (long) this.Successes.Sum<RefreshPackageResult>((Func<RefreshPackageResult, int>) (x => x.CurUpstreamVersions - x.PrevUpstreamVersions));

    [DataMember]
    public long TotalIgnoredBecausePushedOrIngestedVersions => (long) this.Successes.Sum<RefreshPackageResult>((Func<RefreshPackageResult, int>) (x => x.ShadowedVersions));

    [IgnoreDataMember]
    public IList<RefreshPackageResult> Successes { get; }

    [DataMember]
    public int FailuresCount => this.Failures.Count;

    [IgnoreDataMember]
    public IList<RefreshPackageFailure> Failures { get; }

    [DataMember]
    public IList<RefreshPackageFailure> ReportedFailures => (IList<RefreshPackageFailure>) this.Failures.Take<RefreshPackageFailure>(25).ToList<RefreshPackageFailure>();

    [DataMember]
    public int WhitelistedFailuresCount => this.WhitelistedFailures.Count;

    [IgnoreDataMember]
    public IList<RefreshPackageFailure> WhitelistedFailures { get; }

    [DataMember]
    public IList<RefreshPackageFailure> ReportedWhiteListedFailures => (IList<RefreshPackageFailure>) this.WhitelistedFailures.Take<RefreshPackageFailure>(25 - Math.Min(25, this.Failures.Count)).ToList<RefreshPackageFailure>();

    [DataMember]
    public IList<UpstreamStatistics> SuccessStatistics { get; }

    [DataMember]
    public bool Completed { get; }

    [DataMember]
    public int TotalNumberOfPackagesToRefresh { get; }

    [DataMember]
    public int PlannedUnitsOfWork { get; }

    public int UpstreamVersionListCacheHits { get; }

    public int UpstreamVersionListCacheMisses { get; }
  }
}
