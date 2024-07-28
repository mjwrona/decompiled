// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.SASUriExpiry
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class SASUriExpiry
  {
    internal const int TracePointRequestedExpiryMovedInBounds = 5700101;
    private const string TracePointArea = "BlobStore";
    private const string TracePointLayer = "Service";
    public readonly SASUriExpiryBounds Bounds;
    public readonly IClock Clock;
    public readonly DateTimeOffset? RequestedExpiry;
    private static readonly DateTimeOffset RoundingBaseline = new DateTimeOffset(2017, 1, 1, 0, 0, 0, TimeSpan.Zero);
    public static readonly TimeSpan ClockSkewTime = TimeSpan.FromMinutes(5.0);

    public static SASUriExpiry CreateExpiry(
      SASUriExpiryPolicy policy,
      IVssRequestContext context,
      BlobIdWithHeaders blobId)
    {
      return SASUriExpiry.CreateExpiry(policy, context, blobId.ExpiryTime);
    }

    public static SASUriExpiry CreateExpiry(
      SASUriExpiryPolicy policy,
      IVssRequestContext context,
      DateTimeOffset? requestedExpiry = null)
    {
      ArgumentUtility.CheckForNull<SASUriExpiryPolicy>(policy, nameof (policy));
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      IClock clock = policy.Clock;
      SASUriExpiryBounds expiryBounds = policy.GetExpiryBounds(context);
      if (requestedExpiry.HasValue)
      {
        DateTimeOffset now = clock.Now;
        string description = (string) null;
        DateTimeOffset? moved;
        if (expiryBounds.MovedInBounds(now, requestedExpiry.Value, out moved, out description))
        {
          requestedExpiry = moved;
          context.Trace(5700101, TraceLevel.Info, "BlobStore", "Service", description);
        }
      }
      return new SASUriExpiry(clock, expiryBounds, requestedExpiry);
    }

    internal static SASUriExpiry CreateTestExpiry(
      IClock clock,
      SASUriExpiryBounds expiryBounds,
      DateTimeOffset? requestedExpiry = null)
    {
      return new SASUriExpiry(clock, expiryBounds, requestedExpiry);
    }

    internal static SASUriExpiry CreateTestExpiry(IClock clock, DateTimeOffset? requestedExpiry = null) => SASUriExpiry.CreateTestExpiry(clock, SASUriExpiryPolicy.DefaultBounds, requestedExpiry);

    private SASUriExpiry(
      IClock clock,
      SASUriExpiryBounds expiryBounds,
      DateTimeOffset? requestedExpiry = null)
    {
      this.Clock = clock;
      this.Bounds = expiryBounds;
      this.RequestedExpiry = requestedExpiry;
    }

    public DateTimeOffset GetBlobExpiry(BlobIdentifier identifier) => this.RequestedExpiry.HasValue ? this.RequestedExpiry.Value : this.RoundSASTimeRange(identifier);

    public DateTimeOffset RoundSASTimeRange(BlobIdentifier blobId)
    {
      TimeSpan period = this.Bounds.Period;
      uint num = (uint) ((ulong) period.Ticks / 10000000UL);
      return this.RoundSASTimeRange(this.Clock.Now, blobId.MapToIntegerRange(0U, num - 1U), period);
    }

    private DateTimeOffset RoundSASTimeRange(
      DateTimeOffset now,
      uint secondsIntoRange,
      TimeSpan timeRange)
    {
      TimeSpan minExpiry = this.Bounds.MinExpiry;
      DateTimeOffset dateTimeOffset = (now + minExpiry).RoundUpUtc(SASUriExpiry.RoundingBaseline + TimeSpan.FromSeconds((double) secondsIntoRange), timeRange);
      if (dateTimeOffset < now + minExpiry)
        throw new Exception(string.Format("{0}: {1} {2} < {3} {4} + {5} {6}", (object) nameof (RoundSASTimeRange), (object) "rounded", (object) dateTimeOffset, (object) nameof (now), (object) now, (object) "minimumExpiry", (object) minExpiry));
      if (dateTimeOffset >= now + minExpiry + timeRange)
        throw new Exception(string.Format("{0}: {1} {2} >= {3} {4} + {5} {6} + {7} {8}", (object) nameof (RoundSASTimeRange), (object) "rounded", (object) dateTimeOffset, (object) nameof (now), (object) now, (object) "minimumExpiry", (object) minExpiry, (object) nameof (timeRange), (object) timeRange));
      return dateTimeOffset;
    }
  }
}
