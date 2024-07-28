// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ConnectedServiceCreationData
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ConnectedServiceCreationData
  {
    private string m_credentialsXml;
    private Uri m_endpoint;
    private ConnectedServiceMetadata m_serviceMetadata;

    public ConnectedServiceCreationData(
      string name,
      string teamProject,
      ConnectedServiceKind kind,
      string friendlyName,
      string description,
      Uri serviceUri,
      Uri endpoint,
      string credentialsXml)
    {
      this.m_serviceMetadata = new ConnectedServiceMetadata();
      this.ServiceMetadata.Name = name;
      this.ServiceMetadata.TeamProject = teamProject;
      this.ServiceMetadata.FriendlyName = friendlyName;
      this.ServiceMetadata.Kind = kind;
      this.ServiceMetadata.Description = description;
      this.ServiceMetadata.ServiceUri = serviceUri;
      this.m_endpoint = endpoint;
      this.m_credentialsXml = credentialsXml;
    }

    public ConnectedServiceCreationData()
    {
    }

    public string CredentialsXml => this.m_credentialsXml;

    public Uri Endpoint => this.m_endpoint;

    public ConnectedServiceMetadata ServiceMetadata => this.m_serviceMetadata;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ConnectedServiceCreationData FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      ConnectedServiceCreationData serviceCreationData = new ConnectedServiceCreationData();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name1 = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name2 = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return serviceCreationData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ConnectedServiceCreationData instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  CredentialsXml: " + this.m_credentialsXml);
      stringBuilder.AppendLine("  Endpoint: " + this.m_endpoint?.ToString());
      stringBuilder.AppendLine("  ServiceMetadata: " + this.m_serviceMetadata?.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_endpoint != (Uri) null)
        XmlUtility.ToXmlAttribute(writer, "Endpoint", this.m_endpoint);
      if (this.m_credentialsXml != null)
        XmlUtility.ToXmlElement(writer, "CredentialsXml", this.m_credentialsXml);
      if (this.m_serviceMetadata != null)
        ConnectedServiceMetadata.ToXml(writer, "ServiceMetadata", this.m_serviceMetadata);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ConnectedServiceCreationData obj) => obj.ToXml(writer, element);
  }
}
