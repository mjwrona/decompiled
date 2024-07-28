// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ArtifactSpec
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
  public sealed class ArtifactSpec
  {
    internal byte[] m_id = XmlUtility.ZeroLengthArrayOfByte;
    private Guid m_kind = Guid.Empty;
    private string m_moniker;
    private int m_version;

    public ArtifactSpec(Guid kind, string moniker, int version)
    {
      this.m_kind = kind;
      this.m_moniker = moniker;
      this.m_version = version;
    }

    public ArtifactSpec(Guid kind, int artifactId, int version)
      : this(kind, new byte[4]
      {
        (byte) (((long) artifactId & 4278190080L) >> 24),
        (byte) ((artifactId & 16711680) >> 16),
        (byte) ((artifactId & 65280) >> 8),
        (byte) (artifactId & (int) byte.MaxValue)
      }, version)
    {
    }

    public ArtifactSpec(Guid kind, byte[] artifactId, int version)
    {
      this.m_id = artifactId;
      this.m_kind = kind;
      this.m_version = version;
    }

    internal ArtifactSpec()
    {
    }

    public byte[] Id
    {
      get => (byte[]) this.m_id.Clone();
      internal set => this.m_id = value;
    }

    public Guid Kind
    {
      get => this.m_kind;
      internal set => this.m_kind = value;
    }

    public string Moniker
    {
      get => this.m_moniker;
      internal set => this.m_moniker = value;
    }

    public int Version
    {
      get => this.m_version;
      internal set => this.m_version = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ArtifactSpec FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ArtifactSpec artifactSpec = new ArtifactSpec();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "k":
              artifactSpec.m_kind = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "item":
              artifactSpec.m_moniker = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "ver":
              artifactSpec.m_version = XmlUtility.Int32FromXmlAttribute(reader);
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
          if (reader.Name == "Id")
            artifactSpec.m_id = XmlUtility.ArrayOfByteFromXml(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return artifactSpec;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ArtifactSpec instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Id: " + Helper.ArrayToString<byte>(this.m_id));
      stringBuilder.AppendLine("  Kind: " + this.m_kind.ToString());
      stringBuilder.AppendLine("  Moniker: " + this.m_moniker);
      stringBuilder.AppendLine("  Version: " + this.m_version.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_kind != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "k", this.m_kind);
      if (this.m_moniker != null)
        XmlUtility.ToXmlAttribute(writer, "item", this.m_moniker);
      if (this.m_version != 0)
        XmlUtility.ToXmlAttribute(writer, "ver", this.m_version);
      XmlUtility.ToXml(writer, "Id", this.m_id);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ArtifactSpec obj) => obj.ToXml(writer, element);
  }
}
