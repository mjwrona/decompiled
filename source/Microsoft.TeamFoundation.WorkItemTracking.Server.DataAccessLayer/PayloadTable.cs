// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PayloadTable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.MetadataProcessing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class PayloadTable : IXmlSerializable
  {
    private static readonly string s_hierarchyPropertiesTableName = "HierarchyProperties";
    private string m_name;
    private List<PayloadTable.PayloadRow> m_rows;
    private PayloadColumnCollection m_columns;
    private List<PayloadProcessor> m_payloadTableProcessors;
    private List<PayloadProcessor> m_allPayloadProcessors;
    private bool m_schemaHasDefaultValues;
    private int? m_rowCountHint;
    private object[] m_rowDataLists;
    private System.Func<int, object>[] m_rowDataGetters;
    private Action<int, object>[] m_rowDataSetters;
    private Action[] m_rowDataClearers;
    private Action<object>[] m_rowDataAdders;
    private System.Func<int, bool>[] m_rowDataNullCheckers;
    private Action<int>[] m_rowCapacityAdders;
    private static Func<PayloadTable, int, PayloadTable.PayloadRow> s_payloadRowCreator;

    private PayloadTable() => this.m_rows = new List<PayloadTable.PayloadRow>();

    internal PayloadTable(PayloadTableConverter converter, int? rowCountHint = null)
      : this()
    {
      this.Converter = converter;
      this.m_rowCountHint = rowCountHint;
      this.m_schemaHasDefaultValues = false;
      if (converter == null)
        return;
      IEnumerator<KeyValuePair<int, PayloadTableColumnDescriptor>> columnsEnumerator = converter.GetAddedColumnsEnumerator();
      while (columnsEnumerator.MoveNext())
      {
        if (columnsEnumerator.Current.Value.DefaultValue != null)
          this.m_schemaHasDefaultValues = true;
      }
    }

    internal static PayloadTable CreateEmpty(PayloadTableSchema schema, int? rowCountHint = null)
    {
      PayloadTable table = new PayloadTable();
      table.m_columns = new PayloadColumnCollection(table);
      table.m_columns.Populate((IPayloadTableSchema) schema);
      table.m_schemaHasDefaultValues = false;
      table.m_rowCountHint = rowCountHint;
      table.InitializeFieldIndexMap();
      return table;
    }

    public Payload Payload { internal set; get; }

    public string TableName
    {
      get => this.m_name;
      set => this.m_name = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException(DalResourceStrings.Format("ParameterNotNullOrEmpty", (object) nameof (TableName)));
    }

    public int RowCount
    {
      get
      {
        if (!this.IsLoaded)
          throw new InvalidOperationException();
        return this.m_rows.Count;
      }
    }

    internal PayloadTableConverter Converter { get; set; }

    public bool IsLoaded => this.m_columns != null;

    public ReadOnlyCollection<PayloadTable.PayloadRow> Rows
    {
      get
      {
        if (!this.IsLoaded)
          throw new InvalidOperationException();
        return new ReadOnlyCollection<PayloadTable.PayloadRow>((IList<PayloadTable.PayloadRow>) this.m_rows);
      }
    }

    public PayloadColumnCollection Columns
    {
      get
      {
        if (!this.IsLoaded)
          throw new InvalidOperationException();
        return this.m_columns;
      }
    }

    public bool HasDeletedRow { get; internal set; }

    internal IPayloadTableSchema InitializationTableSchema { get; set; }

    internal void AddCapacity(int numRowsToAdd)
    {
      this.m_rows.Capacity = this.m_rows.Count + numRowsToAdd;
      foreach (Action<int> rowCapacityAdder in this.m_rowCapacityAdders)
        rowCapacityAdder(numRowsToAdd);
    }

    internal void Populate(IDataReader reader)
    {
      this.CreateColumns(reader);
      this.ClearRows();
      if (reader == null)
        return;
      while (reader.Read())
      {
        this.AddOneRow();
        PayloadTable.PayloadRow payloadRow = PayloadTable.s_payloadRowCreator(this, this.m_rows.Count);
        payloadRow.Populate(reader);
        this.m_rows.Add(payloadRow);
      }
    }

    private List<PayloadProcessor> AllPayloadProcessors
    {
      get
      {
        if (this.m_allPayloadProcessors == null)
          this.m_allPayloadProcessors = ((IEnumerable<PayloadProcessor>) this.m_payloadTableProcessors ?? Enumerable.Empty<PayloadProcessor>()).ToList<PayloadProcessor>();
        return this.m_allPayloadProcessors;
      }
    }

    private void CreateColumns(IDataReader reader)
    {
      this.m_columns = new PayloadColumnCollection(this);
      if (this.InitializationTableSchema != null)
        this.m_columns.Populate(this.InitializationTableSchema);
      else
        this.m_columns.Populate((IPayloadTableSchema) new DataTableWrapper(reader == null ? (DataTable) null : reader.GetSchemaTable()));
      this.InitializeFieldIndexMap();
    }

    private void AddOneRow()
    {
      foreach (Action<object> rowDataAdder in this.m_rowDataAdders)
        rowDataAdder((object) null);
    }

    internal void EnsureFieldIndexMapInitialized()
    {
      if (this.m_rowDataLists != null)
        return;
      this.InitializeFieldIndexMap();
    }

    internal void InitializeFieldIndexMap()
    {
      this.m_rowDataLists = new object[this.m_columns.Count];
      this.m_rowDataGetters = new System.Func<int, object>[this.m_columns.Count];
      this.m_rowDataSetters = new Action<int, object>[this.m_columns.Count];
      this.m_rowDataClearers = new Action[this.m_columns.Count];
      this.m_rowDataAdders = new Action<object>[this.m_columns.Count];
      this.m_rowDataNullCheckers = new System.Func<int, bool>[this.m_columns.Count];
      this.m_rowCapacityAdders = new Action<int>[this.m_columns.Count];
      for (int index = 0; index < this.m_columns.Count; ++index)
        this.SetupList(index, this.m_columns[index].DataType);
    }

    internal void ClearRows()
    {
      this.m_rows.Clear();
      if (this.m_rowDataClearers == null)
        return;
      foreach (Action rowDataClearer in this.m_rowDataClearers)
        rowDataClearer();
    }

    internal void RemoveDeletedRows()
    {
      if (!this.IsLoaded)
        throw new InvalidOperationException();
      if (!this.HasDeletedRow)
        return;
      this.m_rows = new List<PayloadTable.PayloadRow>(this.m_rows.Where<PayloadTable.PayloadRow>((System.Func<PayloadTable.PayloadRow, bool>) (row => !row.IsDeleted)));
      this.HasDeletedRow = false;
    }

    internal static void MarkTableForCompatibility(PayloadTable table)
    {
      if (!table.IsLoaded)
        throw new InvalidOperationException();
      for (int index = 0; index < table.Columns.Count; ++index)
      {
        PayloadColumn column = table.Columns[index];
        if (column.DataType == typeof (bool) || column.DataType == typeof (Guid))
          column.TranslateDataTypeForOldClients = true;
      }
    }

    internal void AddPayloadProcessor(PayloadProcessor payloadProcessor)
    {
      ArgumentUtility.CheckForNull<PayloadProcessor>(payloadProcessor, nameof (payloadProcessor));
      if (this.m_payloadTableProcessors == null)
        this.m_payloadTableProcessors = new List<PayloadProcessor>();
      this.m_payloadTableProcessors.Add(payloadProcessor);
    }

    private bool ProcessPayloadRow(PayloadTable.PayloadRow row)
    {
      List<PayloadProcessor> payloadProcessors = this.AllPayloadProcessors;
      return payloadProcessors.Count <= 0 || !payloadProcessors.Any<PayloadProcessor>((System.Func<PayloadProcessor, bool>) (x => !x.ProcessRow(row)));
    }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    public void WriteXml(XmlWriter writer)
    {
      TeamFoundationTracingService.TraceRaw(900767, TraceLevel.Info, "DataAccessLayer", nameof (PayloadTable), "PayloadTable.WriteXml");
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      writer.WriteStartElement("table");
      if (!string.IsNullOrEmpty(this.m_name))
        writer.WriteAttributeString("name", this.m_name);
      SqlDataReader reader = (SqlDataReader) null;
      if (this.m_columns == null)
      {
        reader = this.Payload.SqlAccess.SqlDataReader;
        this.CreateColumns((IDataReader) reader);
      }
      if (!string.IsNullOrEmpty(this.m_name) && this.m_name.Equals(PayloadTable.s_hierarchyPropertiesTableName, StringComparison.OrdinalIgnoreCase) && this.m_columns.Contains("WorkItemTypeID"))
        this.m_columns.Remove("WorkItemTypeID");
      this.m_columns.WriteXml(writer);
      this.EnsureFieldIndexMapInitialized();
      writer.WriteStartElement("rows");
      if (reader != null)
      {
        this.AddOneRow();
        PayloadTable.PayloadRow row = PayloadTable.s_payloadRowCreator(this, this.m_rows.Count);
        this.m_rows.Add(row);
        int num = 0;
        TeamFoundationTracingService.TraceRaw(900768, TraceLevel.Verbose, "DataAccessLayer", nameof (PayloadTable), string.Format("Starting to read from the table : {0}", (object) this.TableName));
        while (reader.Read())
        {
          row.Populate((IDataReader) reader);
          if (this.ProcessPayloadRow(row))
            row.WriteXml(writer);
          ++num;
        }
        this.m_rows.Remove(row);
        if (!reader.NextResult())
          this.Payload.SqlAccess.Dispose();
        TeamFoundationTracingService.TraceRaw(900769, TraceLevel.Verbose, "DataAccessLayer", nameof (PayloadTable), string.Format("Finished reading from the table : {0}. Total number of rows processed : {1}", (object) this.TableName, (object) num));
      }
      else
      {
        foreach (PayloadTable.PayloadRow row in this.m_rows)
        {
          if (!row.IsDeleted && this.ProcessPayloadRow(row))
            row.WriteXml(writer);
        }
      }
      writer.WriteEndElement();
      writer.WriteEndElement();
      TeamFoundationTracingService.TraceRaw(900770, TraceLevel.Info, "DataAccessLayer", nameof (PayloadTable), "PayloadTable.WriteXml");
    }

    public void ReadXml(XmlReader reader)
    {
      this.m_name = reader != null ? reader.GetAttribute("name") : throw new ArgumentNullException(nameof (reader));
      reader.ReadStartElement();
      this.m_columns = PayloadColumnCollection.CreateFrom(reader, this);
      this.InitializeFieldIndexMap();
      this.ClearRows();
      if (reader.IsStartElement("rows"))
      {
        if (reader.IsEmptyElement)
        {
          reader.Read();
        }
        else
        {
          reader.ReadStartElement();
          while (reader.IsStartElement("r"))
            this.m_rows.Add(PayloadTable.PayloadRow.CreateFrom(reader, this));
          reader.ReadEndElement();
        }
      }
      reader.ReadEndElement();
    }

    internal static PayloadTable CreateFrom(
      XmlReader reader,
      Payload payload,
      bool newInternalStorageMechanism = true)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      PayloadTable from = new PayloadTable();
      from.Payload = payload;
      from.ReadXml(reader);
      return from;
    }

    internal PayloadTable.PayloadRow AddNewPayloadRow()
    {
      this.EnsureFieldIndexMapInitialized();
      this.AddOneRow();
      PayloadTable.PayloadRow payloadRow = PayloadTable.s_payloadRowCreator(this, this.m_rows.Count);
      this.m_rows.Add(payloadRow);
      return payloadRow;
    }

    private void SetupList(int columnIndex, Type dt)
    {
      if (dt.Equals(typeof (short)))
        this.SetupListOfType<short?>(columnIndex, this.m_rowCountHint);
      else if (dt.Equals(typeof (int)))
        this.SetupListOfType<int?>(columnIndex, this.m_rowCountHint);
      else if (dt.Equals(typeof (bool)))
        this.SetupListOfType<bool?>(columnIndex, this.m_rowCountHint);
      else
        this.SetupListOfType<object>(columnIndex, this.m_rowCountHint);
    }

    private void SetupListOfType<T>(int columnIndex, int? sizeHint)
    {
      List<T> list = !sizeHint.HasValue ? new List<T>() : new List<T>(sizeHint.Value);
      this.m_rowDataLists[columnIndex] = (object) list;
      if (typeof (T) == typeof (int?))
      {
        this.m_rowDataGetters[columnIndex] = (System.Func<int, object>) (rowIndex => (object) ((int?) (object) list[rowIndex]).Value);
        this.m_rowDataSetters[columnIndex] = (Action<int, object>) ((rowIndex, value) => (list as List<int?>)[rowIndex] = value == null ? new int?() : new int?(Convert.ToInt32(value)));
      }
      else if (typeof (T) == typeof (short?))
      {
        this.m_rowDataGetters[columnIndex] = (System.Func<int, object>) (rowIndex => (object) ((short?) (object) list[rowIndex]).Value);
        this.m_rowDataSetters[columnIndex] = (Action<int, object>) ((rowIndex, value) => (list as List<short?>)[rowIndex] = value == null ? new short?() : new short?((short) value));
      }
      else if (typeof (T) == typeof (bool?))
      {
        this.m_rowDataGetters[columnIndex] = (System.Func<int, object>) (rowIndex => (object) ((bool?) (object) list[rowIndex]).Value);
        this.m_rowDataSetters[columnIndex] = (Action<int, object>) ((rowIndex, value) => (list as List<bool?>)[rowIndex] = value == null ? new bool?() : new bool?((bool) value));
      }
      else
      {
        this.m_rowDataGetters[columnIndex] = (System.Func<int, object>) (rowIndex => (object) list[rowIndex]);
        this.m_rowDataSetters[columnIndex] = (Action<int, object>) ((rowIndex, value) => list[rowIndex] = (T) value);
      }
      this.m_rowDataClearers[columnIndex] = (Action) (() => list.Clear());
      this.m_rowDataAdders[columnIndex] = (Action<object>) (value => list.Add((T) value));
      this.m_rowDataNullCheckers[columnIndex] = (System.Func<int, bool>) (rowIndex => (object) list[rowIndex] == null);
      this.m_rowCapacityAdders[columnIndex] = (Action<int>) (capacityToAdd => list.Capacity += capacityToAdd);
    }

    static PayloadTable() => PayloadTable.PayloadRow.EnsureStaticInitialized();

    public class PayloadRow : IXmlSerializable
    {
      private bool m_isDeleted;
      private int m_rowIndex;

      private static PayloadTable.PayloadRow Create(PayloadTable table, int rowIndex)
      {
        PayloadTable.PayloadRow payloadRow = new PayloadTable.PayloadRow();
        payloadRow.m_rowIndex = rowIndex;
        payloadRow.Table = table;
        payloadRow.InitializeValues();
        return payloadRow;
      }

      public static void EnsureStaticInitialized()
      {
      }

      public PayloadTable Table { get; internal set; }

      public bool IsDeleted
      {
        get => this.m_isDeleted;
        internal set
        {
          if (value)
            this.Table.HasDeletedRow = true;
          this.m_isDeleted = value;
        }
      }

      public object this[int index]
      {
        get => this.Table.m_rowDataNullCheckers[index](this.m_rowIndex) ? (object) null : this.Table.m_rowDataGetters[index](this.m_rowIndex);
        set => this.SetValue(index, value);
      }

      public virtual object this[string field]
      {
        get => this[this.Table.Columns[field].Index];
        set => this[this.Table.Columns[field].Index] = value;
      }

      public void SetValue(string columnName, short value) => this.SetValue(this.Table.Columns[columnName].Index, value);

      public virtual void SetValue(string columnName, int value) => this.SetValue(this.Table.Columns[columnName].Index, value);

      public void SetValue(string columnName, bool value) => this.SetValue(this.Table.Columns[columnName].Index, value);

      public virtual void SetValue(string columnName, object value) => this.SetValue(this.Table.Columns[columnName].Index, value);

      public void SetValue(int index, short value)
      {
        if (index < 0 || index >= this.Table.Columns.Count)
          throw new ArgumentOutOfRangeException(nameof (index));
        this.Table.m_rowDataSetters[index](this.m_rowIndex, (object) value);
      }

      public void SetValue(int index, int value)
      {
        if (index < 0 || index >= this.Table.Columns.Count)
          throw new ArgumentOutOfRangeException(nameof (index));
        this.Table.m_rowDataSetters[index](this.m_rowIndex, (object) value);
      }

      public void SetValue(int index, bool value)
      {
        if (index < 0 || index >= this.Table.Columns.Count)
          throw new ArgumentOutOfRangeException(nameof (index));
        this.Table.m_rowDataSetters[index](this.m_rowIndex, (object) value);
      }

      public void SetValue(int index, object value)
      {
        if (index < 0 || index >= this.Table.Columns.Count)
          throw new ArgumentOutOfRangeException(nameof (index));
        this.Table.m_rowDataSetters[index](this.m_rowIndex, value);
      }

      internal void SetValueNoTypeChecks(int index, object value)
      {
        if (index < 0 || index >= this.Table.Columns.Count)
          throw new ArgumentOutOfRangeException(nameof (index));
        this.SetValue(index, value);
      }

      internal void Populate(IDataReader reader)
      {
        if (reader == null)
          throw new ArgumentNullException(nameof (reader));
        this.InitializeValues();
        PayloadTableConverter converter = this.Table.Converter;
        for (int dsFieldIndex = 0; dsFieldIndex < reader.FieldCount; ++dsFieldIndex)
        {
          bool overrideDefaultReadAction = false;
          converter?.RunReadActions(reader, this.Table, dsFieldIndex, this, out overrideDefaultReadAction);
          if (!overrideDefaultReadAction)
            PayloadTable.PayloadRow.DefaultReadAction(reader, this.Table, dsFieldIndex, this);
        }
        if (converter == null)
          return;
        for (int payloadFieldIndex = 0; payloadFieldIndex < this.Table.Columns.Count; ++payloadFieldIndex)
          converter.RunWriteActions(this.Table, payloadFieldIndex, this);
      }

      private void InitializeValues()
      {
        if (!this.Table.m_schemaHasDefaultValues)
          return;
        for (int index = 0; index < this.Table.Columns.Count; ++index)
        {
          if (this.Table.Converter != null && this.Table.Converter.IsAddedColumn(index))
          {
            PayloadTableColumnDescriptor addedColumnByIndex = this.Table.Converter.GetAddedColumnByIndex(index);
            if (addedColumnByIndex != null && addedColumnByIndex.DefaultValue != null)
              this.SetValue(index, addedColumnByIndex.DefaultValue);
          }
        }
      }

      private static void DefaultReadAction(
        IDataReader reader,
        PayloadTable table,
        int dsFieldIndex,
        PayloadTable.PayloadRow row)
      {
        int indexOfDataSetColumn = table.Columns.GetPayloadTableIndexOfDataSetColumn(dsFieldIndex);
        if (indexOfDataSetColumn == -1)
          return;
        Type dataType = table.Columns[indexOfDataSetColumn].DataType;
        if (reader.IsDBNull(dsFieldIndex))
          return;
        if (dataType.Equals(typeof (int)))
          row.SetValue(indexOfDataSetColumn, reader.GetInt32(dsFieldIndex));
        else if (dataType.Equals(typeof (short)))
          row.SetValue(indexOfDataSetColumn, reader.GetInt16(dsFieldIndex));
        else if (dataType.Equals(typeof (bool)))
          row.SetValue(indexOfDataSetColumn, reader.GetBoolean(dsFieldIndex));
        else if (dataType.Equals(typeof (ulong)))
        {
          object obj = reader.GetValue(dsFieldIndex);
          ulong num1 = 0;
          foreach (byte num2 in (byte[]) obj)
            num1 = (num1 << 8) + (ulong) num2;
          row.SetValueNoTypeChecks(indexOfDataSetColumn, (object) num1);
        }
        else if (dataType.Equals(typeof (DateTime)))
        {
          DateTime dateTime = DateTime.SpecifyKind(reader.GetDateTime(dsFieldIndex), DateTimeKind.Utc);
          row.SetValueNoTypeChecks(indexOfDataSetColumn, (object) dateTime);
        }
        else
          row.SetValueNoTypeChecks(indexOfDataSetColumn, reader.GetValue(dsFieldIndex));
      }

      public XmlSchema GetSchema() => (XmlSchema) null;

      public void WriteXml(XmlWriter writer)
      {
        if (writer == null)
          throw new ArgumentNullException(nameof (writer));
        writer.WriteStartElement("r");
        int num = 0;
        for (int index = 0; index < this.Table.Columns.Count; ++index)
        {
          object rawValue = (object) null;
          Type dataType = this.Table.Columns[index].DataType;
          if (!this.Table.m_rowDataNullCheckers[index](this.m_rowIndex))
            rawValue = this.Table.m_rowDataGetters[index](this.m_rowIndex);
          if (rawValue != null)
          {
            writer.WriteStartElement("f");
            if (index != num)
              writer.WriteAttributeString("k", index.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            bool typeForOldClients = this.Table.Columns[index].TranslateDataTypeForOldClients;
            string xmlString = this.ToXmlString(dataType, rawValue, typeForOldClients);
            writer.WriteString(xmlString);
            writer.WriteEndElement();
            num = index + 1;
          }
        }
        writer.WriteEndElement();
      }

      public void ReadXml(XmlReader reader)
      {
        if (reader == null)
          throw new ArgumentNullException(nameof (reader));
        this.InitializeValues();
        reader.ReadStartElement();
        int index = 0;
        while (reader.IsStartElement("f"))
        {
          if (reader.HasAttributes)
          {
            string attribute = reader.GetAttribute("k");
            if (!string.IsNullOrEmpty(attribute))
              index = int.Parse(attribute, (IFormatProvider) CultureInfo.InvariantCulture);
          }
          string str;
          if (reader.IsEmptyElement)
          {
            str = string.Empty;
            reader.Read();
          }
          else
            str = reader.ReadElementString();
          object obj = str == null ? (object) null : this.Parse(this.Table.Columns[index].DataType, str);
          this.Table.m_rowDataSetters[index](this.m_rowIndex, obj);
          ++index;
        }
        reader.ReadEndElement();
      }

      internal static PayloadTable.PayloadRow CreateFrom(XmlReader reader, PayloadTable table)
      {
        table.AddOneRow();
        PayloadTable.PayloadRow from = PayloadTable.PayloadRow.Create(table, table.m_rows.Count);
        from.ReadXml(reader);
        return from;
      }

      private object Parse(Type type, string value)
      {
        if (type.Equals(typeof (string)))
          return (object) value;
        if (type.Equals(typeof (short)))
          return (object) short.Parse(value);
        if (type.Equals(typeof (int)))
          return (object) int.Parse(value);
        if (type.Equals(typeof (bool)))
        {
          bool flag;
          switch (value)
          {
            case "1":
              flag = true;
              break;
            case "0":
              flag = false;
              break;
            default:
              flag = bool.Parse(value);
              break;
          }
          return (object) flag;
        }
        if (type.Equals(typeof (DateTime)))
          return (object) DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        if (type.Equals(typeof (ulong)))
          return (object) ulong.Parse(value);
        if (type.Equals(typeof (double)))
          return (object) double.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture);
        if (type.Equals(typeof (Guid)))
          return (object) Guid.Parse(value);
        throw new FormatException(DalResourceStrings.Format("UnrecognisedTypeToSerialize", (object) type.FullName));
      }

      private string ToXmlString(Type dt, object rawValue, bool translate)
      {
        if (dt.Equals(typeof (string)))
          return (string) rawValue;
        if (dt.Equals(typeof (short)))
          return ((short) rawValue).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (dt.Equals(typeof (int)))
          return ((int) rawValue).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (dt.Equals(typeof (bool)))
          return !translate ? ((bool) rawValue).ToString((IFormatProvider) CultureInfo.InvariantCulture) : ((bool) rawValue ? "1" : "0");
        if (dt.Equals(typeof (DateTime)))
          return ((DateTime) rawValue).ToString("yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        if (dt.Equals(typeof (ulong)))
          return ((ulong) rawValue).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (dt.Equals(typeof (double)))
          return ((double) rawValue).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (dt.Equals(typeof (Guid)))
        {
          string format = translate ? "D" : "N";
          return ((Guid) rawValue).ToString(format, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        throw new FormatException(DalResourceStrings.Format("UnrecognisedTypeToSerialize", (object) dt.FullName));
      }

      static PayloadRow() => PayloadTable.s_payloadRowCreator = new Func<PayloadTable, int, PayloadTable.PayloadRow>(PayloadTable.PayloadRow.Create);
    }
  }
}
