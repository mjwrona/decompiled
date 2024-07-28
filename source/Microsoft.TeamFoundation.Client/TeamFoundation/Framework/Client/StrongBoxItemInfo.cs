// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.StrongBoxItemInfo
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
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StrongBoxItemInfo
  {
    private Guid m_drawerId = Guid.Empty;
    private int m_fileId;
    private StrongBoxItemKind m_itemKind;
    private string m_lookupKey;

    private StrongBoxItemInfo()
    {
    }

    public Guid DrawerId
    {
      [EditorBrowsable(EditorBrowsableState.Never)] get => this.m_drawerId;
    }

    public int FileId
    {
      [EditorBrowsable(EditorBrowsableState.Never)] get => this.m_fileId;
    }

    public StrongBoxItemKind ItemKind
    {
      [EditorBrowsable(EditorBrowsableState.Never)] get => this.m_itemKind;
    }

    public string LookupKey
    {
      [EditorBrowsable(EditorBrowsableState.Never)] get => this.m_lookupKey;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static StrongBoxItemInfo FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      StrongBoxItemInfo strongBoxItemInfo = new StrongBoxItemInfo();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "DrawerId":
              strongBoxItemInfo.m_drawerId = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "FileId":
              strongBoxItemInfo.m_fileId = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "ItemKind":
              strongBoxItemInfo.m_itemKind = XmlUtility.EnumFromXmlAttribute<StrongBoxItemKind>(reader);
              continue;
            case "LookupKey":
              strongBoxItemInfo.m_lookupKey = XmlUtility.StringFromXmlAttribute(reader);
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
      return strongBoxItemInfo;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("StrongBoxItemInfo instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  DrawerId: " + this.m_drawerId.ToString());
      stringBuilder.AppendLine("  FileId: " + this.m_fileId.ToString());
      stringBuilder.AppendLine("  ItemKind: " + this.m_itemKind.ToString());
      stringBuilder.AppendLine("  LookupKey: " + this.m_lookupKey);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, StrongBoxItemInfo obj) => obj.ToXml(writer, element);
  }
}
