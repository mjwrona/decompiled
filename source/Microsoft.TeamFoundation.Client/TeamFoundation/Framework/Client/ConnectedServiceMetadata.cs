// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ConnectedServiceMetadata
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
  public sealed class ConnectedServiceMetadata
  {
    private Guid m_authenticatedBy = Guid.Empty;
    private string m_description;
    private string m_friendlyName;
    private ConnectedServiceKind m_kind;
    private string m_name;
    private Uri m_serviceUri;
    private string m_teamProject;

    public Guid AuthenticatedBy => this.m_authenticatedBy;

    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    public string FriendlyName
    {
      get => this.m_friendlyName;
      set => this.m_friendlyName = value;
    }

    public ConnectedServiceKind Kind
    {
      get => this.m_kind;
      set => this.m_kind = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public Uri ServiceUri
    {
      get => this.m_serviceUri;
      set => this.m_serviceUri = value;
    }

    public string TeamProject
    {
      get => this.m_teamProject;
      set => this.m_teamProject = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ConnectedServiceMetadata FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      ConnectedServiceMetadata connectedServiceMetadata = new ConnectedServiceMetadata();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "AuthenticatedBy":
              connectedServiceMetadata.m_authenticatedBy = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "FriendlyName":
              connectedServiceMetadata.m_friendlyName = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "Kind":
              connectedServiceMetadata.m_kind = XmlUtility.EnumFromXmlAttribute<ConnectedServiceKind>(reader);
              continue;
            case "Name":
              connectedServiceMetadata.m_name = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "ServiceUri":
              connectedServiceMetadata.m_serviceUri = XmlUtility.UriFromXmlAttribute(reader);
              continue;
            case "TeamProject":
              connectedServiceMetadata.m_teamProject = XmlUtility.StringFromXmlAttribute(reader);
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
          if (reader.Name == "Description")
            connectedServiceMetadata.m_description = XmlUtility.StringFromXmlElement(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return connectedServiceMetadata;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ConnectedServiceMetadata instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AuthenticatedBy: " + this.m_authenticatedBy.ToString());
      stringBuilder.AppendLine("  Description: " + this.m_description);
      stringBuilder.AppendLine("  FriendlyName: " + this.m_friendlyName);
      stringBuilder.AppendLine("  Kind: " + this.m_kind.ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  ServiceUri: " + this.m_serviceUri?.ToString());
      stringBuilder.AppendLine("  TeamProject: " + this.m_teamProject);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_friendlyName != null)
        XmlUtility.ToXmlAttribute(writer, "FriendlyName", this.m_friendlyName);
      if (this.m_kind != ConnectedServiceKind.Custom)
        XmlUtility.EnumToXmlAttribute<ConnectedServiceKind>(writer, "Kind", this.m_kind);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "Name", this.m_name);
      if (this.m_serviceUri != (Uri) null)
        XmlUtility.ToXmlAttribute(writer, "ServiceUri", this.m_serviceUri);
      if (this.m_teamProject != null)
        XmlUtility.ToXmlAttribute(writer, "TeamProject", this.m_teamProject);
      if (this.m_description != null)
        XmlUtility.ToXmlElement(writer, "Description", this.m_description);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ConnectedServiceMetadata obj) => obj.ToXml(writer, element);
  }
}
