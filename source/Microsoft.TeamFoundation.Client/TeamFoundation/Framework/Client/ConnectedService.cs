// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ConnectedService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ConnectedService
  {
    private StrongBoxItemInfo m_credentialsXmlInfo;
    private StrongBoxItemInfo m_endpointInfo;
    private StrongBoxItemInfo m_oAuthTokenInfo;
    private ConnectedServiceMetadata m_serviceMetadata;

    private TeamFoundationStrongBoxService StrongBox { get; set; }

    public string Endpoint => this.StrongBox.GetString(this.m_endpointInfo.DrawerId, this.m_endpointInfo.LookupKey);

    public string CredentialsXml => this.StrongBox.GetString(this.m_credentialsXmlInfo.DrawerId, this.m_credentialsXmlInfo.LookupKey);

    public string OAuthToken => this.m_oAuthTokenInfo != null ? this.StrongBox.GetString(this.m_oAuthTokenInfo.DrawerId, this.m_oAuthTokenInfo.LookupKey) : (string) null;

    private ConnectedService()
    {
    }

    public ConnectedServiceMetadata ServiceMetadata => this.m_serviceMetadata;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ConnectedService FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ConnectedService connectedService = new ConnectedService();
      if (serviceProvider != null)
        connectedService.StrongBox = (TeamFoundationStrongBoxService) serviceProvider.GetService(typeof (TeamFoundationStrongBoxService));
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "CredentialsXmlInfo":
              connectedService.m_credentialsXmlInfo = StrongBoxItemInfo.FromXml(serviceProvider, reader);
              continue;
            case "EndpointInfo":
              connectedService.m_endpointInfo = StrongBoxItemInfo.FromXml(serviceProvider, reader);
              continue;
            case "OAuthTokenInfo":
              connectedService.m_oAuthTokenInfo = StrongBoxItemInfo.FromXml(serviceProvider, reader);
              continue;
            case "ServiceMetadata":
              connectedService.m_serviceMetadata = ConnectedServiceMetadata.FromXml(serviceProvider, reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return connectedService;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ConnectedService instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  CredentialsXmlInfo: " + this.m_credentialsXmlInfo?.ToString());
      stringBuilder.AppendLine("  EndpointInfo: " + this.m_endpointInfo?.ToString());
      stringBuilder.AppendLine("  OAuthTokenInfo: " + this.m_oAuthTokenInfo?.ToString());
      stringBuilder.AppendLine("  ServiceMetadata: " + this.m_serviceMetadata?.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ConnectedService obj) => obj.ToXml(writer, element);
  }
}
