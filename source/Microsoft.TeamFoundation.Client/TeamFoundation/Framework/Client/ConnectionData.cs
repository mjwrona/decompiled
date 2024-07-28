// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ConnectionData
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class ConnectionData
  {
    private TeamFoundationIdentity m_authenticatedUser;
    private TeamFoundationIdentity m_authorizedUser;
    private Guid m_catalogResourceId = Guid.Empty;
    private Guid m_instanceId = Guid.Empty;
    private LocationServiceData m_locationServiceData;
    private int m_serverCapabilities;
    private string m_webApplicationRelativeDirectory;
    private string m_serverVersion;

    public ServerCapabilities ServerCapabilities => (ServerCapabilities) (this.m_serverCapabilities & 3);

    internal ConnectionData()
    {
    }

    public TeamFoundationIdentity AuthenticatedUser
    {
      get => this.m_authenticatedUser;
      set => this.m_authenticatedUser = value;
    }

    public TeamFoundationIdentity AuthorizedUser
    {
      get => this.m_authorizedUser;
      set => this.m_authorizedUser = value;
    }

    public Guid CatalogResourceId
    {
      get => this.m_catalogResourceId;
      set => this.m_catalogResourceId = value;
    }

    public Guid InstanceId
    {
      get => this.m_instanceId;
      set => this.m_instanceId = value;
    }

    public LocationServiceData LocationServiceData
    {
      get => this.m_locationServiceData;
      set => this.m_locationServiceData = value;
    }

    public string WebApplicationRelativeDirectory
    {
      get => this.m_webApplicationRelativeDirectory;
      set => this.m_webApplicationRelativeDirectory = value;
    }

    public string ServerVersion
    {
      get => this.m_serverVersion;
      set => this.m_serverVersion = value;
    }

    internal static ConnectionData FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ConnectionData connectionData = new ConnectionData();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "CatalogResourceId":
              connectionData.m_catalogResourceId = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "InstanceId":
              connectionData.m_instanceId = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "ServerCapabilities":
              connectionData.m_serverCapabilities = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "ServerVersion":
              connectionData.m_serverVersion = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "WebApplicationRelativeDirectory":
              connectionData.m_webApplicationRelativeDirectory = XmlUtility.StringFromXmlAttribute(reader);
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
            case "AuthenticatedUser":
              connectionData.m_authenticatedUser = TeamFoundationIdentity.FromXml(serviceProvider, reader);
              continue;
            case "AuthorizedUser":
              connectionData.m_authorizedUser = TeamFoundationIdentity.FromXml(serviceProvider, reader);
              continue;
            case "LocationServiceData":
              connectionData.m_locationServiceData = LocationServiceData.FromXml(serviceProvider, reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return connectionData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ConnectionData instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AuthenticatedUser: " + this.m_authenticatedUser?.ToString());
      stringBuilder.AppendLine("  AuthorizedUser: " + this.m_authorizedUser?.ToString());
      stringBuilder.AppendLine("  CatalogResourceId: " + this.m_catalogResourceId.ToString());
      stringBuilder.AppendLine("  InstanceId: " + this.m_instanceId.ToString());
      stringBuilder.AppendLine("  LocationServiceData: " + this.m_locationServiceData?.ToString());
      stringBuilder.AppendLine("  ServerCapabilities: " + this.m_serverCapabilities.ToString());
      stringBuilder.AppendLine("  WebApplicationRelativeDirectory: " + this.m_webApplicationRelativeDirectory);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_catalogResourceId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "CatalogResourceId", this.m_catalogResourceId);
      if (this.m_instanceId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "InstanceId", this.m_instanceId);
      if (this.m_serverCapabilities != 0)
        XmlUtility.ToXmlAttribute(writer, "ServerCapabilities", this.m_serverCapabilities);
      if (this.m_serverVersion != null)
        XmlUtility.ToXmlAttribute(writer, "ServerVersion", this.m_serverVersion);
      if (this.m_webApplicationRelativeDirectory != null)
        XmlUtility.ToXmlAttribute(writer, "WebApplicationRelativeDirectory", this.m_webApplicationRelativeDirectory);
      if (this.m_authenticatedUser != null)
        TeamFoundationIdentity.ToXml(writer, "AuthenticatedUser", this.m_authenticatedUser);
      if (this.m_authorizedUser != null)
        TeamFoundationIdentity.ToXml(writer, "AuthorizedUser", this.m_authorizedUser);
      if (this.m_locationServiceData != null)
        LocationServiceData.ToXml(writer, "LocationServiceData", this.m_locationServiceData);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, ConnectionData obj) => obj.ToXml(writer, element);
  }
}
