// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PayloadTableCollection
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class PayloadTableCollection : IXmlSerializable, IEnumerable<PayloadTable>, IEnumerable
  {
    private const int c_baseTablesIndex = 1;
    private List<PayloadTable> m_tableList = new List<PayloadTable>();
    private Dictionary<string, PayloadTable> m_tableDictionary = new Dictionary<string, PayloadTable>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);

    public PayloadTable this[int index] => this.m_tableList[index];

    public PayloadTable this[string name]
    {
      get
      {
        if (string.IsNullOrEmpty(name))
          throw new ArgumentException(DalResourceStrings.Format("ParameterNotNullOrEmpty", (object) nameof (name)));
        return this.m_tableDictionary.ContainsKey(name) ? this.m_tableDictionary[name] : throw new ArgumentOutOfRangeException(nameof (name), (object) name, (string) null);
      }
    }

    public bool TryGetTable(string name, out PayloadTable table) => this.m_tableDictionary.TryGetValue(name, out table);

    public int Count => this.m_tableList.Count;

    public void Add(PayloadTable table)
    {
      if (table == null)
        throw new ArgumentNullException(nameof (table));
      if (this.m_tableDictionary.ContainsValue(table))
        throw new ArgumentException(DalResourceStrings.Get("TableAlreadyInCollection"), nameof (table));
      if (this.m_tableDictionary.ContainsKey(table.TableName))
        throw new ArgumentOutOfRangeException(nameof (table), DalResourceStrings.Format("DuplicateTableName", (object) table.TableName));
      this.m_tableList.Add(table);
      if (!string.IsNullOrEmpty(table.TableName))
        this.m_tableDictionary.Add(table.TableName, table);
      table.Payload = this.Payload;
    }

    public void Remove(PayloadTable table)
    {
      if (table == null)
        throw new ArgumentNullException(nameof (table));
      if (!this.m_tableList.Contains(table))
        throw new ArgumentOutOfRangeException(nameof (table), (object) table, DalResourceStrings.Get("TableNotFound"));
      this.m_tableList.Remove(table);
      if (this.m_tableDictionary.ContainsValue(table))
        this.m_tableDictionary.Remove(table.TableName);
      table.Payload = (Payload) null;
    }

    internal PayloadTableCollection(Payload payloadOwner) => this.Payload = payloadOwner != null ? payloadOwner : throw new ArgumentNullException(nameof (payloadOwner));

    public Payload Payload { private set; get; }

    internal void Populate(SqlDataReader reader, PayloadConverter converter) => this.Populate((IDataReader) reader, converter, int.MaxValue, int.MaxValue);

    internal void Populate(
      IDataReader reader,
      PayloadConverter converter,
      int expectedTableCount,
      int inMemoryTableCount)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      if (inMemoryTableCount != int.MaxValue && expectedTableCount == int.MaxValue)
        throw new InvalidOperationException();
      this.m_tableList.Clear();
      this.m_tableDictionary.Clear();
      int index = 1;
      while (expectedTableCount > 0)
      {
        PayloadTable payloadTable = new PayloadTable(converter == null ? (PayloadTableConverter) null : converter[index]);
        payloadTable.Payload = this.Payload;
        payloadTable.TableName = "Table" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_tableList.Add(payloadTable);
        this.m_tableDictionary.Add(payloadTable.TableName, payloadTable);
        if (inMemoryTableCount > 0)
        {
          payloadTable.InitializationTableSchema = this.Payload.GetTableSchema(index);
          payloadTable.Populate(reader);
          if (inMemoryTableCount != int.MaxValue)
            --inMemoryTableCount;
          if (!reader.NextResult())
          {
            if (this.Payload.SqlAccess != null)
            {
              this.Payload.SqlAccess.Dispose();
              break;
            }
            break;
          }
        }
        if (expectedTableCount != int.MaxValue)
          --expectedTableCount;
        ++index;
      }
      if (this.m_tableList.Count != 1 || this.m_tableList[0].Columns.Count != 0 || this.m_tableList[0].RowCount != 0)
        return;
      this.m_tableDictionary.Clear();
      this.m_tableList.Clear();
    }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    public void WriteXml(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      int index = 1;
      foreach (PayloadTable table in this.m_tableList)
      {
        if (!table.IsLoaded)
          table.InitializationTableSchema = this.Payload.GetTableSchema(index);
        table.WriteXml(writer);
        ++index;
      }
    }

    public void ReadXml(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      this.m_tableList.Clear();
      this.m_tableDictionary.Clear();
      if (reader.IsEmptyElement)
      {
        reader.Read();
      }
      else
      {
        reader.ReadStartElement();
        while (reader.IsStartElement("table"))
        {
          PayloadTable from = PayloadTable.CreateFrom(reader, this.Payload);
          this.m_tableList.Add(from);
          this.m_tableDictionary.Add(from.TableName, from);
        }
        reader.ReadEndElement();
      }
    }

    public IEnumerator<PayloadTable> GetEnumerator() => (IEnumerator<PayloadTable>) this.m_tableList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
