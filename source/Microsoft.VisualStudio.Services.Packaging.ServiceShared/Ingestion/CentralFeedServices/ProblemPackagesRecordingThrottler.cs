// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.ProblemPackagesRecordingThrottler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  internal class ProblemPackagesRecordingThrottler : IProblemPackagesRecordingThrottler
  {
    public static readonly Guid RedisNamespace = new Guid("3001F6F4-C97A-419F-822A-37A816FA0716");
    public static readonly DateTime Epoch = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc);
    private readonly IRedisServiceFacade redisService;

    public ProblemPackagesRecordingThrottler(IRedisServiceFacade redisService) => this.redisService = redisService;

    public bool OkToRecord(ProblemPackageRecordingThrottleKey key) => this.redisService.GetVolatileDictionaryContainer<string, long, ProblemPackagesRecordingThrottler.RedisSecurityKey>(ProblemPackagesRecordingThrottler.RedisNamespace, new ContainerSettings()
    {
      KeyExpiry = new TimeSpan?(key.Interval)
    }).IncrementBy(ProblemPackagesRecordingThrottler.BuildKeyString(key), 1L) == 1L;

    private static string BuildKeyString(ProblemPackageRecordingThrottleKey key)
    {
      string correctlyCasedName = key.Protocol.CorrectlyCasedName;
      string normalizedName = key.PackageName.NormalizedName;
      string normalizedVersion = key.PackageVersion.NormalizedVersion;
      long totalSeconds = (long) key.Interval.TotalSeconds;
      TimeSpan timeSpan = key.Timestamp - ProblemPackagesRecordingThrottler.Epoch;
      long ticks1 = timeSpan.Ticks;
      timeSpan = key.Interval;
      long ticks2 = timeSpan.Ticks;
      long num = ticks1 / ticks2;
      Guid id = key.UpstreamSource.Id;
      return FormattableString.Invariant(FormattableStringFactory.Create("{0:D}/{1}/{2:D}/{3}/{4}/{5}/{6}", (object) key.FeedId, (object) correctlyCasedName, (object) id, (object) normalizedName, (object) normalizedVersion, (object) totalSeconds, (object) num));
    }

    private class RedisSecurityKey
    {
    }
  }
}
