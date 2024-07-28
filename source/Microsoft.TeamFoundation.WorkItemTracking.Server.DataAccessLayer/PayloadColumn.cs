// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PayloadColumn
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class PayloadColumn : IXmlSerializable
  {
    private string m_name;

    private PayloadColumn(int index)
    {
      this.Index = index >= 0 ? index : throw new ArgumentOutOfRangeException(nameof (index), (object) index, (string) null);
      this.DataSetSourceFieldIndex = -1;
    }

    internal PayloadColumn(int index, string name, Type dataType)
      : this(index, name, dataType, -1)
    {
    }

    internal PayloadColumn(int index, string name, Type dataType, int dataSetSourceFieldIndex)
      : this(index)
    {
      this.m_name = name;
      if (dataType == typeof (byte[]))
        dataType = typeof (ulong);
      this.DataType = dataType;
      this.DataSetSourceFieldIndex = dataSetSourceFieldIndex;
    }

    [XmlIgnore]
    public int DataSetSourceFieldIndex { private set; get; }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException(DalResourceStrings.Format("ParameterNotNullOrEmpty", (object) "ColumnName"));
    }

    public Type DataType { get; private set; }

    public bool TranslateDataTypeForOldClients { get; set; }

    public int Index { get; private set; }

    public bool Equals(PayloadColumn other) => StringComparer.OrdinalIgnoreCase.Compare(other.m_name, this.m_name) == 0 && other.DataType.Equals(this.DataType) && other.Index.Equals(this.Index);

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    public void WriteXml(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      writer.WriteStartElement("c");
      writer.WriteElementString("n", this.m_name);
      if (this.DataType.Equals(typeof (short)) || this.DataType.Equals(typeof (bool)) && this.TranslateDataTypeForOldClients)
        writer.WriteElementString("t", typeof (int).FullName);
      else if (this.DataType.Equals(typeof (Guid)) && this.TranslateDataTypeForOldClients)
        writer.WriteElementString("t", typeof (string).FullName);
      else
        writer.WriteElementString("t", this.DataType.FullName);
      writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      reader.ReadStartElement();
      if (reader.IsStartElement("n"))
      {
        reader.ReadStartElement();
        this.m_name = reader.ReadString();
        reader.ReadEndElement();
      }
      if (reader.IsStartElement("t"))
      {
        reader.ReadStartElement();
        this.DataType = Type.GetType(reader.ReadString());
        reader.ReadEndElement();
      }
      reader.ReadEndElement();
    }

    internal static PayloadColumn CreateFrom(XmlReader reader, int index)
    {
      PayloadColumn from = new PayloadColumn(index);
      from.ReadXml(reader);
      return from;
    }
  }
}
