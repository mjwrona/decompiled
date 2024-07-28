// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PayloadColumnCollection
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class PayloadColumnCollection : IXmlSerializable
  {
    private List<PayloadColumn> m_columnList = new List<PayloadColumn>();
    private Dictionary<string, PayloadColumn> m_columnDictionary = new Dictionary<string, PayloadColumn>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<int, int> m_DataSetToPayloadColumnMapping = new Dictionary<int, int>();
    private Dictionary<string, PayloadTableColumnDescriptor> m_originalDatasetColumns;
    private Dictionary<int, string> m_datasetIndexToNameColumns;

    internal PayloadColumnCollection(PayloadTable table) => this.Table = table != null ? table : throw new ArgumentNullException(nameof (table));

    public PayloadTable Table { private set; get; }

    public int Count => this.m_columnList.Count;

    public PayloadColumn this[int index] => this.m_columnList[index];

    public PayloadColumn this[string name]
    {
      get
      {
        if (string.IsNullOrEmpty(name))
          throw new ArgumentException(DalResourceStrings.Format("ParameterNotNullOrEmpty", (object) nameof (name)));
        PayloadColumn payloadColumn;
        if (!this.m_columnDictionary.TryGetValue(name, out payloadColumn))
          throw new ArgumentOutOfRangeException(nameof (name), (object) name, (string) null);
        return payloadColumn;
      }
    }

    internal bool DatasetColumnExists(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      return this.m_originalDatasetColumns != null ? this.m_originalDatasetColumns.ContainsKey(name) : throw new InvalidOperationException();
    }

    internal PayloadTableColumnDescriptor GetDatasetColumn(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      if (this.m_originalDatasetColumns == null)
        throw new InvalidOperationException();
      PayloadTableColumnDescriptor datasetColumn;
      if (!this.m_originalDatasetColumns.TryGetValue(name, out datasetColumn))
        throw new ArgumentOutOfRangeException(nameof (name));
      return datasetColumn;
    }

    internal int GetPayloadTableIndexOfDataSetColumn(int dsIndex)
    {
      int indexOfDataSetColumn;
      if (!this.m_DataSetToPayloadColumnMapping.TryGetValue(dsIndex, out indexOfDataSetColumn))
        throw new ArgumentException(nameof (dsIndex));
      return indexOfDataSetColumn;
    }

    internal string GetDatasetColumnNameByDatasetColumnIndex(int dsIndex)
    {
      if (this.m_datasetIndexToNameColumns == null)
        throw new InvalidOperationException();
      string datasetColumnIndex;
      if (!this.m_datasetIndexToNameColumns.TryGetValue(dsIndex, out datasetColumnIndex))
        throw new ArgumentOutOfRangeException(nameof (dsIndex));
      return datasetColumnIndex;
    }

    public bool Contains(string name) => !string.IsNullOrEmpty(name) ? this.m_columnDictionary.ContainsKey(name) : throw new ArgumentException(DalResourceStrings.Format("ParameterNotNullOrEmpty", (object) nameof (name)));

    internal void Populate(IPayloadTableSchema schema)
    {
      this.m_columnDictionary.Clear();
      this.m_columnList.Clear();
      PayloadTableColumnDescriptor columnDescriptor1 = (PayloadTableColumnDescriptor) null;
      IEnumerator<KeyValuePair<int, PayloadTableColumnDescriptor>> enumerator = (IEnumerator<KeyValuePair<int, PayloadTableColumnDescriptor>>) null;
      KeyValuePair<int, PayloadTableColumnDescriptor> current;
      if (this.Table.Converter != null)
      {
        enumerator = this.Table.Converter.GetAddedColumnsEnumerator();
        if (enumerator != null)
        {
          PayloadTableColumnDescriptor columnDescriptor2;
          if (!enumerator.MoveNext())
          {
            columnDescriptor2 = (PayloadTableColumnDescriptor) null;
          }
          else
          {
            current = enumerator.Current;
            columnDescriptor2 = current.Value;
          }
          columnDescriptor1 = columnDescriptor2;
        }
      }
      int num = 0;
      this.m_originalDatasetColumns = new Dictionary<string, PayloadTableColumnDescriptor>();
      this.m_datasetIndexToNameColumns = new Dictionary<int, string>();
      for (int index = 0; index < schema.ColumnCount; ++index)
      {
        string columnName = schema.GetColumnName(index);
        if (!string.IsNullOrEmpty(columnName))
        {
          Type type = schema.GetColumnType(index);
          if (type.Equals(typeof (byte[])))
            type = typeof (ulong);
          this.m_originalDatasetColumns.Add(columnName, new PayloadTableColumnDescriptor(columnName, type, index));
          this.m_datasetIndexToNameColumns.Add(index, columnName);
        }
      }
      while (columnDescriptor1 != null || num < schema.ColumnCount)
      {
        int count = this.m_columnList.Count;
        PayloadColumn column;
        if (columnDescriptor1 != null && count == columnDescriptor1.Index || num == schema.ColumnCount)
        {
          column = new PayloadColumn(count, columnDescriptor1.Name, columnDescriptor1.Type);
          PayloadTableColumnDescriptor columnDescriptor3;
          if (!enumerator.MoveNext())
          {
            columnDescriptor3 = (PayloadTableColumnDescriptor) null;
          }
          else
          {
            current = enumerator.Current;
            columnDescriptor3 = current.Value;
          }
          columnDescriptor1 = columnDescriptor3;
        }
        else
        {
          column = new PayloadColumn(count, schema.GetColumnName(num), schema.GetColumnType(num), num);
          this.m_DataSetToPayloadColumnMapping.Add(num, -1);
          ++num;
        }
        this.ProcessColumn(column);
      }
    }

    public void AddColumn(PayloadColumn column)
    {
      if (this.Table.Converter != null && this.Table.Converter.IsAddedColumn(this.m_columnList.Count) && column.DataSetSourceFieldIndex != -1)
        throw new InvalidOperationException();
      int index;
      if (string.IsNullOrEmpty(column.Name))
      {
        PayloadColumn payloadColumn = column;
        index = column.Index;
        string str = "Column" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        payloadColumn.Name = str;
      }
      PayloadColumn payloadColumn1;
      string str1;
      for (; this.m_columnDictionary.ContainsKey(column.Name); payloadColumn1.Name = str1)
      {
        payloadColumn1 = column;
        string name = column.Name;
        index = column.Index;
        string str2 = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        str1 = name + str2;
      }
      this.m_columnDictionary.Add(column.Name, column);
      this.m_columnList.Add(column);
      if (column.DataSetSourceFieldIndex == -1)
        return;
      this.m_DataSetToPayloadColumnMapping[column.DataSetSourceFieldIndex] = this.m_columnList.Count - 1;
    }

    public bool Equals(PayloadColumnCollection other)
    {
      if (other.Count != this.Count)
        return false;
      for (int index = 0; index < this.m_columnList.Count; ++index)
      {
        if (!this.m_columnList[index].Equals(other.m_columnList[index]))
          return false;
      }
      return true;
    }

    private void ProcessColumn(PayloadColumn column)
    {
      if (this.Table.Converter == null || !this.Table.Converter.IsRemoved(column.Name))
        this.Table.Columns.AddColumn(column);
      if (this.Table.Converter != null)
        this.Table.Converter.ExecuteProcessColumnCallbacks(this.Table, column);
      if (this.Table.Payload == null || this.Table.Payload.Converter == null)
        return;
      this.Table.Payload.Converter.ExecuteGlobalProcessColumnCallbacks(this.Table, column);
    }

    public void Remove(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException(DalResourceStrings.Format("ParameterNotNullOrEmpty", (object) nameof (name)));
      PayloadColumn payloadColumn;
      if (this.m_columnDictionary.TryGetValue(name, out payloadColumn))
      {
        if (payloadColumn.DataSetSourceFieldIndex != -1)
          this.m_DataSetToPayloadColumnMapping[payloadColumn.DataSetSourceFieldIndex] = -1;
        this.m_columnDictionary.Remove(name);
        this.m_columnList.Remove(payloadColumn);
      }
      else
      {
        if (this.Table.Converter == null || this.Table.Payload == null || this.Table.Payload.Converter == null)
          return;
        if (!this.m_originalDatasetColumns.ContainsKey(name))
          throw new InvalidOperationException();
        if (this.Table.Converter == null)
          this.Table.Converter = new PayloadTableConverter();
        this.Table.Converter.RemoveExistingColumn(name);
      }
    }

    [Conditional("DEBUG")]
    private void DebugException(string msg) => throw new Exception(msg);

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    public void WriteXml(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      writer.WriteStartElement("columns");
      foreach (IXmlSerializable column in this.m_columnList)
        column.WriteXml(writer);
      writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      this.m_columnDictionary.Clear();
      this.m_columnList.Clear();
      if (!reader.IsStartElement("columns"))
        return;
      reader.ReadStartElement();
      int index = 0;
      while (reader.IsStartElement("c"))
      {
        PayloadColumn from = PayloadColumn.CreateFrom(reader, index);
        this.m_columnDictionary.Add(from.Name, from);
        this.m_columnList.Add(from);
        ++index;
      }
      reader.ReadEndElement();
    }

    internal static PayloadColumnCollection CreateFrom(XmlReader reader, PayloadTable payloadTable)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      PayloadColumnCollection from = new PayloadColumnCollection(payloadTable);
      if (reader.IsStartElement("columns"))
        from.ReadXml(reader);
      return from;
    }
  }
}
