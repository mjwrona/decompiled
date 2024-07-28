// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.GeoReplicationStats
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Globalization;
using System.Xml.Linq;

namespace Microsoft.Azure.Cosmos.Table
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
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid geo-replication status in response: '{0}'", new object[1]
          {
            (object) geoReplicationStatus
          }), nameof (geoReplicationStatus));
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
