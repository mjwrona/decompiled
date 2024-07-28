// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.SigningInfo
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
  public sealed class SigningInfo
  {
    internal byte[] m_downloadKey = XmlUtility.ZeroLengthArrayOfByte;
    private Guid m_id = Guid.Empty;

    internal SigningInfo()
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
      internal set => this.m_id = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static SigningInfo FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      SigningInfo signingInfo = new SigningInfo();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          if (reader.Name == "dkey")
            signingInfo.m_downloadKey = XmlUtility.ArrayOfByteFromXmlAttribute(reader);
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "Id")
            signingInfo.m_id = XmlUtility.GuidFromXmlElement(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return signingInfo;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("SigningInfo instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  DownloadKey: " + Helper.ArrayToString<byte>(this.m_downloadKey));
      stringBuilder.AppendLine("  Id: " + this.m_id.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      XmlUtility.ToXmlAttribute(writer, "dkey", this.m_downloadKey);
      if (this.m_id != Guid.Empty)
        XmlUtility.ToXmlElement(writer, "Id", this.m_id);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, SigningInfo obj) => obj.ToXml(writer, element);
  }
}
