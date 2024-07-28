// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestDetailComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RequestDetailComparer : IEqualityComparer<RequestDetails>
  {
    public RequestDetailComparer(
      int maxCompressedThresholdTime,
      TeamFoundationLoggingLevel loggingLevel)
    {
      this.MaxCompressedThresholdTime = maxCompressedThresholdTime;
      this.LoggingLevel = loggingLevel;
    }

    public int GetHashCode(RequestDetails requestContext) => requestContext.UniqueIdentifier.GetHashCode();

    public bool Equals(RequestDetails x, RequestDetails y) => x == y || x != null && y != null && x.Status == null && y.Status == null && x.CanAggregate && y.CanAggregate && x.ActivityStatus == ActivityStatus.Success && y.ActivityStatus == ActivityStatus.Success && x.ResponseCode < 400 && y.ResponseCode < 400 && x.ResponseCode == y.ResponseCode && x.DelayTime <= 0L && y.DelayTime <= 0L && (x.Count != 1 || x.ExecutionTime <= (long) this.MaxCompressedThresholdTime) && (y.Count != 1 || y.ExecutionTime <= (long) this.MaxCompressedThresholdTime) && string.Equals(x.ServiceName, y.ServiceName, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Title, y.Title, StringComparison.OrdinalIgnoreCase) && object.Equals((object) x.UniqueIdentifier, (object) y.UniqueIdentifier) && this.LoggingLevel != TeamFoundationLoggingLevel.All && VssStringComparer.UserName.Equals(x.AuthenticatedUserName, y.AuthenticatedUserName) && VssStringComparer.Hostname.Equals(x.RemoteIPAddress, y.RemoteIPAddress) && string.Equals(x.UserAgent, y.UserAgent, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Command, y.Command, StringComparison.OrdinalIgnoreCase) && string.Equals(x.ThrottleReason, y.ThrottleReason, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Referrer, y.Referrer, StringComparison.OrdinalIgnoreCase) && string.Equals(x.UriStem, y.UriStem, StringComparison.OrdinalIgnoreCase) && x.StartTime.Ticks / 600000000L == x.EndTime.Ticks / 600000000L && y.StartTime.Ticks / 600000000L == y.EndTime.Ticks / 600000000L && x.StartTime.Ticks / 600000000L == y.StartTime.Ticks / 600000000L && x.SupportsPublicAccess == y.SupportsPublicAccess && object.Equals((object) x.AuthorizationId, (object) y.AuthorizationId);

    public int GetHashCode(IVssRequestContext obj) => obj == null ? 0 : obj.GetHashCode();

    internal int MaxCompressedThresholdTime { get; set; }

    internal TeamFoundationLoggingLevel LoggingLevel { get; set; }
  }
}
