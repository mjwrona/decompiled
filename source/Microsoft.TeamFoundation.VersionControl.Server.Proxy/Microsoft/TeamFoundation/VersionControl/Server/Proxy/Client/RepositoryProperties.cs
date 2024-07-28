// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Proxy.Client.RepositoryProperties
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Server.Proxy.Client
{
  internal sealed class RepositoryProperties
  {
    internal byte[] m_downloadKey = XmlUtility.ZeroLengthArrayOfByte;
    private Guid m_id = Guid.Empty;
    private int m_latestChangesetId;
    private string m_name;
    private int m_supportedFeatures;
    private string m_version;

    internal RepositoryProperties()
    {
    }

    public byte[] DownloadKey
    {
      get => (byte[]) this.m_downloadKey.Clone();
      set => this.m_downloadKey = value;
    }

    public Guid Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    public int LatestChangesetId
    {
      get => this.m_latestChangesetId;
      set => this.m_latestChangesetId = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public int SupportedFeatures
    {
      get => this.m_supportedFeatures;
      set => this.m_supportedFeatures = value;
    }

    public string Version
    {
      get => this.m_version;
      set => this.m_version = value;
    }

    internal static RepositoryProperties FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      RepositoryProperties repositoryProperties = new RepositoryProperties();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "dkey":
              repositoryProperties.m_downloadKey = XmlUtility.ArrayOfByteFromXmlAttribute(reader);
              continue;
            case "id":
              repositoryProperties.m_id = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "lcset":
              repositoryProperties.m_latestChangesetId = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "name":
              repositoryProperties.m_name = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "features":
              repositoryProperties.m_supportedFeatures = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "ver":
              repositoryProperties.m_version = XmlUtility.StringFromXmlAttribute(reader);
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
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return repositoryProperties;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("RepositoryProperties instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  DownloadKey: " + Helper.ArrayToString<byte>(this.m_downloadKey));
      stringBuilder.AppendLine("  Id: " + this.m_id.ToString());
      stringBuilder.AppendLine("  LatestChangesetId: " + this.m_latestChangesetId.ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  SupportedFeatures: " + this.m_supportedFeatures.ToString());
      stringBuilder.AppendLine("  Version: " + this.m_version);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      XmlUtility.ToXmlAttribute(writer, "dkey", this.m_downloadKey);
      if (this.m_id != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "id", this.m_id);
      if (this.m_latestChangesetId != 0)
        XmlUtility.ToXmlAttribute(writer, "lcset", this.m_latestChangesetId);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "name", this.m_name);
      if (this.m_supportedFeatures != 0)
        XmlUtility.ToXmlAttribute(writer, "features", this.m_supportedFeatures);
      if (this.m_version != null)
        XmlUtility.ToXmlAttribute(writer, "ver", this.m_version);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, RepositoryProperties obj) => obj.ToXml(writer, element);
  }
}
