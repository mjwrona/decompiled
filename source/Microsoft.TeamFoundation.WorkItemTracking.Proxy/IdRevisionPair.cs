// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.IdRevisionPair
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public struct IdRevisionPair
  {
    private int m_id;
    private int m_revision;

    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IdRevisionPair FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      IdRevisionPair idRevisionPair = new IdRevisionPair();
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
            case "Id":
              idRevisionPair.m_id = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "Revision":
              idRevisionPair.m_revision = XmlUtility.Int32FromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return idRevisionPair;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("IdRevisionPair instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Id: " + this.m_id.ToString());
      stringBuilder.AppendLine("  Revision: " + this.m_revision.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_id != 0)
        XmlUtility.ToXmlElement(writer, "Id", this.m_id);
      if (this.m_revision != 0)
        XmlUtility.ToXmlElement(writer, "Revision", this.m_revision);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, IdRevisionPair obj) => obj.ToXml(writer, element);

    public IdRevisionPair(int id, int revision)
    {
      this.m_id = id;
      this.m_revision = revision;
    }
  }
}
