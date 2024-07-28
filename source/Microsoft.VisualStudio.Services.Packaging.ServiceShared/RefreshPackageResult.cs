// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RefreshPackageResult
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RefreshPackageResult
  {
    public static RefreshPackageResult UpstreamsDisabled(FeedCore feed, IPackageName packageName) => new RefreshPackageResult(feed.Id, packageName, false, false, 0, 0, 0, 0, 0, (IReadOnlyList<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics>) new List<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics>(), false, 0, 0, 0, (RefreshPackageIntermediateData) null);

    public static RefreshPackageResult RefreshNotNeeded(FeedCore feed, IPackageName packageName) => new RefreshPackageResult(feed.Id, packageName, true, false, 0, 0, 0, 0, 0, (IReadOnlyList<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics>) new List<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics>(), false, 0, 0, 0, (RefreshPackageIntermediateData) null);

    public static RefreshPackageResult RefreshNotNeeded(
      FeedCore feed,
      IPackageName packageName,
      int curUpstreamVersions,
      int curUpstreamTerrapinVersions,
      int localVersions,
      int shadowedVersions,
      IReadOnlyList<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics> upstreamStatistics,
      int numRetainedEntries,
      int upstreamVersionListCacheHits,
      int upstreamVersionListCacheMisses,
      RefreshPackageIntermediateData? intermediateData)
    {
      return new RefreshPackageResult(feed.Id, packageName, true, false, curUpstreamVersions, curUpstreamVersions, curUpstreamTerrapinVersions, localVersions, shadowedVersions, upstreamStatistics, false, numRetainedEntries, upstreamVersionListCacheHits, upstreamVersionListCacheMisses, intermediateData);
    }

    public static RefreshPackageResult Refreshed(
      FeedCore feed,
      IPackageName packageName,
      int prevUpstreamVersions,
      int curUpstreamVersions,
      int curUpstreamTerrapinVersions,
      int localVersions,
      int shadowedVersions,
      IReadOnlyList<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics> upstreamStatistics,
      bool packageNameMetadataUpdated,
      int numRetainedEntries,
      int upstreamVersionListCacheHits,
      int upstreamVersionListCacheMisses,
      RefreshPackageIntermediateData? intermediateData)
    {
      return new RefreshPackageResult(feed.Id, packageName, true, true, prevUpstreamVersions, curUpstreamVersions, curUpstreamTerrapinVersions, localVersions, shadowedVersions, upstreamStatistics, packageNameMetadataUpdated, numRetainedEntries, upstreamVersionListCacheHits, upstreamVersionListCacheMisses, intermediateData);
    }

    public static RefreshPackageResult Failed(
      FeedCore feed,
      IPackageName packageName,
      UpstreamSource upstreamSource,
      UpstreamFailureException upstreamFailure)
    {
      return new RefreshPackageResult(feed.Id, packageName, upstreamSource, upstreamFailure);
    }

    public RefreshPackageResult(
      Guid feedId,
      IPackageName packageName,
      bool upstreamsEnabled,
      bool refreshNeeded,
      int prevUpstreamVersions,
      int curUpstreamVersions,
      int curUpstreamTerrapinVersions,
      int localVersions,
      int shadowedVersions,
      IReadOnlyList<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics> upstreamStatistics,
      bool packageNameMetadataUpdated,
      int numRetainedEntries,
      int upstreamVersionListCacheHits,
      int upstreamVersionListCacheMisses,
      RefreshPackageIntermediateData? intermediateData)
    {
      this.FeedId = feedId;
      this.PackageName = packageName.DisplayName;
      this.UpstreamsEnabled = upstreamsEnabled;
      this.RefreshNeeded = refreshNeeded;
      this.PrevUpstreamVersions = prevUpstreamVersions;
      this.CurUpstreamVersions = curUpstreamVersions;
      this.CurUpstreamTerrapinVersions = curUpstreamTerrapinVersions;
      this.LocalVersions = localVersions;
      this.ShadowedVersions = shadowedVersions;
      this.UpstreamStatistics = upstreamStatistics;
      this.PackageNameMetadataUpdated = packageNameMetadataUpdated;
      this.NumRetainedEntries = numRetainedEntries;
      this.UpstreamVersionListCacheHits = upstreamVersionListCacheHits;
      this.UpstreamVersionListCacheMisses = upstreamVersionListCacheMisses;
      this.IntermediateData = intermediateData;
    }

    public RefreshPackageResult(
      Guid feedId,
      IPackageName packageName,
      UpstreamSource upstreamSource,
      UpstreamFailureException upstreamFailure)
    {
      this.FeedId = feedId;
      this.PackageName = packageName.DisplayName;
      this.UpstreamSource = upstreamSource;
      this.UpstreamFailureException = upstreamFailure;
      this.UpstreamStatistics = (IReadOnlyList<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics>) Array.Empty<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics>();
    }

    public Guid FeedId { get; }

    public string PackageName { get; }

    public bool UpstreamsEnabled { get; }

    public bool RefreshNeeded { get; }

    public int PrevUpstreamVersions { get; }

    public int CurUpstreamVersions { get; }

    public int CurUpstreamTerrapinVersions { get; }

    public int LocalVersions { get; }

    public int ShadowedVersions { get; }

    public bool PackageNameMetadataUpdated { get; }

    public int NumRetainedEntries { get; }

    public int UpstreamVersionListCacheHits { get; }

    public int UpstreamVersionListCacheMisses { get; }

    public IReadOnlyList<Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamStatistics> UpstreamStatistics { get; }

    public UpstreamSource? UpstreamSource { get; }

    [JsonIgnore]
    public UpstreamFailureException? UpstreamFailureException { get; }

    public string? UpstreamFailureExceptionType => this.UpstreamFailureException?.GetType().FullName;

    public UpstreamStatusCategory? UpstreamFailureExceptionCategory => this.UpstreamFailureException?.ErrorCategory;

    public string? UpstreamFailureExceptionMessage => this.UpstreamFailureException == null ? (string) null : StackTraceCompressor.CompressStackTrace(this.UpstreamFailureException.ToString());

    public bool IsFailed => this.UpstreamFailureException != null;

    [IgnoreDataMember]
    public RefreshPackageIntermediateData? IntermediateData { get; }
  }
}
