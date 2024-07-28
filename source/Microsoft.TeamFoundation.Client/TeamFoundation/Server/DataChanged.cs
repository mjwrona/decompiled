// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.DataChanged
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public sealed class DataChanged
  {
    public string DataType;
    public DateTime LastModified;

    public DataChanged(string dataType, DateTime dateTime)
    {
      this.DataType = dataType;
      this.LastModified = dateTime;
    }

    public DataChanged()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DataChanged FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      DataChanged dataChanged = new DataChanged();
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
            case "DataType":
              dataChanged.DataType = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "LastModified":
              dataChanged.LastModified = XmlUtility.DateTimeFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return dataChanged;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("DataChanged instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  DataType: " + this.DataType);
      stringBuilder.AppendLine("  LastModified: " + this.LastModified.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.DataType != null)
        XmlUtility.ToXmlElement(writer, "DataType", this.DataType);
      if (this.LastModified != DateTime.MinValue)
        XmlUtility.ToXmlElement(writer, "LastModified", this.LastModified);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, DataChanged obj) => obj.ToXml(writer, element);
  }
}
