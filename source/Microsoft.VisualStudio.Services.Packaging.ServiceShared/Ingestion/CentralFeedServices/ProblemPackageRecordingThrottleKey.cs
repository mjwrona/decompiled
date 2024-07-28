// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.ProblemPackageRecordingThrottleKey
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class ProblemPackageRecordingThrottleKey
  {
    public ProblemPackageRecordingThrottleKey(
      Guid feedId,
      IProtocol protocol,
      IPackageName packageName,
      IPackageVersion packageVersion,
      TimeSpan interval,
      DateTime timestamp,
      UpstreamSourceInfo upstreamSource)
    {
      this.FeedId = feedId;
      this.Protocol = protocol;
      this.PackageName = packageName;
      this.PackageVersion = packageVersion;
      this.Interval = interval;
      this.Timestamp = timestamp;
      this.UpstreamSource = upstreamSource;
    }

    public Guid FeedId { get; }

    public IProtocol Protocol { get; }

    public IPackageName PackageName { get; }

    public IPackageVersion PackageVersion { get; }

    public TimeSpan Interval { get; }

    public DateTime Timestamp { get; }

    public UpstreamSourceInfo UpstreamSource { get; }
  }
}
