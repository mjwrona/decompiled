// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.FileServiceProperties
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Shared.Protocol;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public sealed class FileServiceProperties
  {
    internal ServiceProperties serviceProperties;

    public FileServiceProperties() => this.serviceProperties = new ServiceProperties();

    public FileServiceProperties(
      MetricsProperties hourMetrics = null,
      MetricsProperties minuteMetrics = null,
      CorsProperties cors = null)
    {
      this.serviceProperties = new ServiceProperties(hourMetrics: hourMetrics, minuteMetrics: minuteMetrics, cors: cors);
    }

    public CorsProperties Cors
    {
      get => this.serviceProperties.Cors;
      set => this.serviceProperties.Cors = value;
    }

    public MetricsProperties HourMetrics
    {
      get => this.serviceProperties.HourMetrics;
      set => this.serviceProperties.HourMetrics = value;
    }

    public MetricsProperties MinuteMetrics
    {
      get => this.serviceProperties.MinuteMetrics;
      set => this.serviceProperties.MinuteMetrics = value;
    }

    internal static FileServiceProperties FromServiceXml(XDocument servicePropertiesDocument)
    {
      XElement xelement = servicePropertiesDocument.Element((XName) "StorageServiceProperties");
      return new FileServiceProperties()
      {
        Cors = ServiceProperties.ReadCorsPropertiesFromXml(xelement.Element((XName) "Cors")),
        HourMetrics = ServiceProperties.ReadMetricsPropertiesFromXml(xelement.Element((XName) "HourMetrics")),
        MinuteMetrics = ServiceProperties.ReadMetricsPropertiesFromXml(xelement.Element((XName) "MinuteMetrics"))
      };
    }

    internal XDocument ToServiceXml() => this.serviceProperties.ToServiceXml();

    internal void WriteServiceProperties(Stream outputStream) => this.serviceProperties.WriteServiceProperties(outputStream);
  }
}
