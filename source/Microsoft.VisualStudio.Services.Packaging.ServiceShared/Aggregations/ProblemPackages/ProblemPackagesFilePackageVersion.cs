// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages.ProblemPackagesFilePackageVersion
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages
{
  public class ProblemPackagesFilePackageVersion
  {
    public DateTime Timestamp { get; }

    public IImmutableList<TerrapinIngestionValidationReason> Reasons { get; }

    public UpstreamSourceInfo UpstreamSource { get; }

    public ProblemPackagesFilePackageVersion(
      DateTime timestamp,
      UpstreamSourceInfo upstreamSource,
      IEnumerable<TerrapinIngestionValidationReason> reasons)
    {
      this.Timestamp = timestamp;
      this.UpstreamSource = upstreamSource;
      this.Reasons = (IImmutableList<TerrapinIngestionValidationReason>) reasons.ToImmutableList<TerrapinIngestionValidationReason>();
    }

    internal ProblemPackagesFilePackageVersion.Stored Pack() => new ProblemPackagesFilePackageVersion.Stored()
    {
      Timestamp = this.Timestamp,
      UpstreamSource = this.UpstreamSource,
      Reasons = (IList<ProblemPackagesFilePackageVersion.Stored.StoredReason>) this.Reasons.Select<TerrapinIngestionValidationReason, ProblemPackagesFilePackageVersion.Stored.StoredReason>(ProblemPackagesFilePackageVersion.\u003C\u003EO.\u003C0\u003E__Pack ?? (ProblemPackagesFilePackageVersion.\u003C\u003EO.\u003C0\u003E__Pack = new Func<TerrapinIngestionValidationReason, ProblemPackagesFilePackageVersion.Stored.StoredReason>(ProblemPackagesFilePackageVersion.Stored.StoredReason.Pack))).ToList<ProblemPackagesFilePackageVersion.Stored.StoredReason>()
    };

    internal class Stored
    {
      public DateTime Timestamp { get; set; }

      public IList<ProblemPackagesFilePackageVersion.Stored.StoredReason> Reasons { get; set; }

      public UpstreamSourceInfo UpstreamSource { get; set; }

      internal ProblemPackagesFilePackageVersion Unpack() => new ProblemPackagesFilePackageVersion(this.Timestamp, this.UpstreamSource, this.Reasons.Select<ProblemPackagesFilePackageVersion.Stored.StoredReason, TerrapinIngestionValidationReason>((Func<ProblemPackagesFilePackageVersion.Stored.StoredReason, TerrapinIngestionValidationReason>) (x => x.Unpack())));

      internal class StoredReason
      {
        public string Code { get; set; }

        public string Message { get; set; }

        internal static ProblemPackagesFilePackageVersion.Stored.StoredReason Pack(
          TerrapinIngestionValidationReason unpacked)
        {
          return new ProblemPackagesFilePackageVersion.Stored.StoredReason()
          {
            Code = unpacked.Code,
            Message = unpacked.Message
          };
        }

        internal TerrapinIngestionValidationReason Unpack() => new TerrapinIngestionValidationReason(this.Code, this.Message);
      }
    }
  }
}
