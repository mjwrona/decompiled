// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.ServiceStats
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class ServiceStats
  {
    private const string StorageServiceStatsName = "StorageServiceStats";
    private const string GeoReplicationName = "GeoReplication";

    private ServiceStats()
    {
    }

    public GeoReplicationStats GeoReplication { get; private set; }

    internal static Task<ServiceStats> FromServiceXmlAsync(XDocument serviceStatsDocument)
    {
      XElement xelement = serviceStatsDocument.Element((XName) "StorageServiceStats");
      return Task.FromResult<ServiceStats>(new ServiceStats()
      {
        GeoReplication = GeoReplicationStats.ReadGeoReplicationStatsFromXml(xelement.Element((XName) "GeoReplication"))
      });
    }
  }
}
