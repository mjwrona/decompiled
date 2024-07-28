// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.GeoReplicationStats
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Globalization;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  public sealed class GeoReplicationStats
  {
    private const string StatusName = "Status";
    private const string LastSyncTimeName = "LastSyncTime";

    private GeoReplicationStats()
    {
    }

    public GeoReplicationStatus Status { get; private set; }

    public DateTimeOffset? LastSyncTime { get; private set; }

    internal static GeoReplicationStatus GetGeoReplicationStatus(string geoReplicationStatus)
    {
      switch (geoReplicationStatus)
      {
        case "unavailable":
          return GeoReplicationStatus.Unavailable;
        case "live":
          return GeoReplicationStatus.Live;
        case "bootstrap":
          return GeoReplicationStatus.Bootstrap;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid geo-replication status in response: '{0}'", (object) geoReplicationStatus), nameof (geoReplicationStatus));
      }
    }

    internal static GeoReplicationStats ReadGeoReplicationStatsFromXml(XElement element)
    {
      string input = element.Element((XName) "LastSyncTime").Value;
      return new GeoReplicationStats()
      {
        Status = GeoReplicationStats.GetGeoReplicationStatus(element.Element((XName) "Status").Value),
        LastSyncTime = string.IsNullOrEmpty(input) ? new DateTimeOffset?() : new DateTimeOffset?(DateTimeOffset.Parse(input, (IFormatProvider) CultureInfo.InvariantCulture))
      };
    }
  }
}
