// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServicingStepLogEntry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ServicingStepLogEntry : ServicingStepDetail
  {
    private int m_entryKindDataTransfer;
    private string m_message;

    public ServicingStepLogEntryKind EntryKind => (ServicingStepLogEntryKind) this.m_entryKindDataTransfer;

    public override string ToLogEntryLine() => string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "[{0:u}][{1}] {2}", (object) this.DetailTime.ToUniversalTime(), (object) this.EntryKind, (object) this.Message);

    private ServicingStepLogEntry()
    {
    }

    public string Message => this.m_message;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServicingStepLogEntry FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ServicingStepLogEntry servicingStepLogEntry = new ServicingStepLogEntry();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "did":
              servicingStepLogEntry.m_detailId = XmlUtility.Int64FromXmlAttribute(reader);
              continue;
            case "dtime":
              servicingStepLogEntry.m_detailTime = XmlUtility.DateTimeFromXmlAttribute(reader);
              continue;
            case "ek":
              servicingStepLogEntry.m_entryKindDataTransfer = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "sop":
              servicingStepLogEntry.m_servicingOperation = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "gid":
              servicingStepLogEntry.m_servicingStepGroupId = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "sid":
              servicingStepLogEntry.m_servicingStepId = XmlUtility.StringFromXmlAttribute(reader);
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
          if (reader.Name == "Message")
            servicingStepLogEntry.m_message = XmlUtility.StringFromXmlElement(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return servicingStepLogEntry;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServicingStepLogEntry instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  DetailId: " + this.m_detailId.ToString());
      stringBuilder.AppendLine("  DetailTime: " + this.m_detailTime.ToString());
      stringBuilder.AppendLine("  EntryKindDataTransfer: " + this.m_entryKindDataTransfer.ToString());
      stringBuilder.AppendLine("  Message: " + this.m_message);
      stringBuilder.AppendLine("  ServicingOperation: " + this.m_servicingOperation);
      stringBuilder.AppendLine("  ServicingStepGroupId: " + this.m_servicingStepGroupId);
      stringBuilder.AppendLine("  ServicingStepId: " + this.m_servicingStepId);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (element != nameof (ServicingStepLogEntry))
        writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", nameof (ServicingStepLogEntry));
      if (this.m_detailId != 0L)
        XmlUtility.ToXmlAttribute(writer, "did", this.m_detailId);
      if (this.m_detailTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "dtime", this.m_detailTime);
      if (this.m_entryKindDataTransfer != 0)
        XmlUtility.ToXmlAttribute(writer, "ek", this.m_entryKindDataTransfer);
      if (this.m_servicingOperation != null)
        XmlUtility.ToXmlAttribute(writer, "sop", this.m_servicingOperation);
      if (this.m_servicingStepGroupId != null)
        XmlUtility.ToXmlAttribute(writer, "gid", this.m_servicingStepGroupId);
      if (this.m_servicingStepId != null)
        XmlUtility.ToXmlAttribute(writer, "sid", this.m_servicingStepId);
      if (this.m_message != null)
        XmlUtility.ToXmlElement(writer, "Message", this.m_message);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServicingStepLogEntry obj) => obj.ToXml(writer, element);
  }
}
