// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.LocationServiceData
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class LocationServiceData
  {
    internal AccessMapping[] m_accessMappings = Helper.ZeroLengthArrayOfAccessMapping;
    private bool m_accessPointsDoNotIncludeWebAppRelativeDirectory;
    private bool m_clientCacheFresh;
    private int m_clientCacheTimeToLive = 86400;
    private string m_defaultAccessMappingMoniker;
    private int m_lastChangeId;
    internal ServiceDefinition[] m_serviceDefinitions = Helper.ZeroLengthArrayOfServiceDefinition;

    internal LocationServiceData()
    {
    }

    public AccessMapping[] AccessMappings
    {
      get => (AccessMapping[]) this.m_accessMappings.Clone();
      set => this.m_accessMappings = value;
    }

    public bool AccessPointsDoNotIncludeWebAppRelativeDirectory
    {
      get => this.m_accessPointsDoNotIncludeWebAppRelativeDirectory;
      set => this.m_accessPointsDoNotIncludeWebAppRelativeDirectory = value;
    }

    public bool ClientCacheFresh
    {
      get => this.m_clientCacheFresh;
      set => this.m_clientCacheFresh = value;
    }

    public int ClientCacheTimeToLive
    {
      get => this.m_clientCacheTimeToLive;
      set => this.m_clientCacheTimeToLive = value;
    }

    public string DefaultAccessMappingMoniker
    {
      get => this.m_defaultAccessMappingMoniker;
      set => this.m_defaultAccessMappingMoniker = value;
    }

    public int LastChangeId
    {
      get => this.m_lastChangeId;
      set => this.m_lastChangeId = value;
    }

    public ServiceDefinition[] ServiceDefinitions
    {
      get => (ServiceDefinition[]) this.m_serviceDefinitions.Clone();
      set => this.m_serviceDefinitions = value;
    }

    internal static LocationServiceData FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      LocationServiceData locationServiceData = new LocationServiceData();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "AccessPointsDoNotIncludeWebAppRelativeDirectory":
              locationServiceData.m_accessPointsDoNotIncludeWebAppRelativeDirectory = XmlUtility.BooleanFromXmlAttribute(reader);
              continue;
            case "ClientCacheFresh":
              locationServiceData.m_clientCacheFresh = XmlUtility.BooleanFromXmlAttribute(reader);
              continue;
            case "ClientCacheTimeToLive":
              locationServiceData.m_clientCacheTimeToLive = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "DefaultAccessMappingMoniker":
              locationServiceData.m_defaultAccessMappingMoniker = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "LastChangeId":
              locationServiceData.m_lastChangeId = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "AccessMappings":
              locationServiceData.m_accessMappings = Helper.ArrayOfAccessMappingFromXml(serviceProvider, reader, false);
              continue;
            case "ServiceDefinitions":
              locationServiceData.m_serviceDefinitions = Helper.ArrayOfServiceDefinitionFromXml(serviceProvider, reader, false);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return locationServiceData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("LocationServiceData instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AccessMappings: " + Helper.ArrayToString<AccessMapping>(this.m_accessMappings));
      stringBuilder.AppendLine("  AccessPointsDoNotIncludeWebAppRelativeDirectory: " + this.m_accessPointsDoNotIncludeWebAppRelativeDirectory.ToString());
      stringBuilder.AppendLine("  ClientCacheFresh: " + this.m_clientCacheFresh.ToString());
      stringBuilder.AppendLine("  ClientCacheTimeToLive; " + this.m_clientCacheTimeToLive.ToString());
      stringBuilder.AppendLine("  DefaultAccessMappingMoniker: " + this.m_defaultAccessMappingMoniker);
      stringBuilder.AppendLine("  LastChangeId: " + this.m_lastChangeId.ToString());
      stringBuilder.AppendLine("  ServiceDefinitions: " + Helper.ArrayToString<ServiceDefinition>(this.m_serviceDefinitions));
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_accessPointsDoNotIncludeWebAppRelativeDirectory)
        XmlUtility.ToXmlAttribute(writer, "AccessPointsDoNotIncludeWebAppRelativeDirectory", this.m_accessPointsDoNotIncludeWebAppRelativeDirectory);
      if (this.m_clientCacheFresh)
        XmlUtility.ToXmlAttribute(writer, "ClientCacheFresh", this.m_clientCacheFresh);
      if (this.m_clientCacheTimeToLive != 86400)
        XmlUtility.ToXmlAttribute(writer, "ClientCacheTimeToLive", this.m_clientCacheTimeToLive);
      if (this.m_defaultAccessMappingMoniker != null)
        XmlUtility.ToXmlAttribute(writer, "DefaultAccessMappingMoniker", this.m_defaultAccessMappingMoniker);
      if (this.m_lastChangeId != 0)
        XmlUtility.ToXmlAttribute(writer, "LastChangeId", this.m_lastChangeId);
      Helper.ToXml(writer, "AccessMappings", this.m_accessMappings, false, false);
      Helper.ToXml(writer, "ServiceDefinitions", this.m_serviceDefinitions, false, false);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, LocationServiceData obj) => obj.ToXml(writer, element);
  }
}
