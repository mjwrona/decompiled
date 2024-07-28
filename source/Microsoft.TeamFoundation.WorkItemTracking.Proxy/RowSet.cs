// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.RowSet
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public class RowSet : IRowSet, IXmlSerializable, ISerializeRow
  {
    private string m_name;
    private Dictionary<string, int> m_columnNames;
    private RowSetColumn[] m_columns;
    private int m_intCount;
    private int m_objCount;
    private RowSetRow[] m_rows;

    internal string Name => this.m_name;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public RowSetColumn[] Columns => this.m_columns;

    internal int IntCount => this.m_intCount;

    internal int ObjCount => this.m_objCount;

    internal RowSetColumn GetColumn(int column)
    {
      if (column < 0 || this.m_columns == null || column >= this.m_columns.Length)
        throw new ArgumentOutOfRangeException(nameof (column));
      return this.m_columns[column];
    }

    internal RowSetRow GetRow(int row)
    {
      if (row < 0 || this.m_rows == null || row >= this.m_rows.Length)
        throw new ArgumentOutOfRangeException(nameof (row));
      return this.m_rows[row];
    }

    public bool ContainsColumn(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return this.m_columnNames.ContainsKey(name);
    }

    public int RowCount => this.m_rows != null ? this.m_rows.Length : 0;

    public int ColumnCount => this.m_columns != null ? this.m_columns.Length : 0;

    public string this[int column] => this.GetColumn(column).Name;

    public object this[int column, int row] => this.GetRow(row)[column];

    public object this[string column, int row]
    {
      get
      {
        if (string.IsNullOrEmpty(column))
          throw new ArgumentException(ResourceStrings.Format("ParameterNotNullOrEmpty", (object) nameof (column)));
        int index;
        if (!this.m_columnNames.TryGetValue(column, out index))
          throw new ArgumentOutOfRangeException(nameof (column));
        return this.GetRow(row)[index];
      }
    }

    public void SwapRows(int row1, int row2)
    {
      if (this.m_rows == null)
        throw new InvalidOperationException();
      if (row1 < 0 || row1 >= this.m_rows.Length)
        throw new ArgumentOutOfRangeException(nameof (row1));
      if (row2 < 0 || row2 >= this.m_rows.Length)
        throw new ArgumentOutOfRangeException(nameof (row2));
      RowSetRow row = this.m_rows[row1];
      this.m_rows[row1] = this.m_rows[row2];
      this.m_rows[row2] = row;
    }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    void IXmlSerializable.WriteXml(XmlWriter w) => throw new NotImplementedException(ResourceStrings.Format("WriteXmlNotImplemented", (object) this.GetType().FullName));

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      List<RowSetRow> rowSetRowList = new List<RowSetRow>();
      List<RowSetColumn> rowSetColumnList = new List<RowSetColumn>();
      this.m_columnNames = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_name = reader.GetAttribute("name");
      reader.ReadStartElement();
      if (reader.IsStartElement("columns"))
      {
        reader.ReadStartElement();
        int num = 0;
        while (reader.IsStartElement("c"))
        {
          RowSetColumn rowSetColumn = new RowSetColumn();
          ((IXmlSerializable) rowSetColumn).ReadXml(reader);
          rowSetColumnList.Add(rowSetColumn);
          this.m_columnNames.Add(rowSetColumn.Name, num);
          ++num;
        }
        reader.ReadEndElement();
      }
      this.m_columns = rowSetColumnList.ToArray();
      this.InitColumns();
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
          {
            RowSetRow rowSetRow = new RowSetRow(this);
            ((IXmlSerializable) rowSetRow).ReadXml(reader);
            rowSetRowList.Add(rowSetRow);
          }
          reader.ReadEndElement();
        }
      }
      this.m_rows = rowSetRowList.ToArray();
      reader.ReadEndElement();
    }

    void ISerializeRow.CopyRow(int row, IntPtr p, int esz)
    {
      RowSetRow row1 = this.GetRow(row);
      for (int index = 0; index < this.m_columns.Length; ++index)
      {
        RowSetColumn column = this.m_columns[index];
        if (column.IsValue)
        {
          int? intValue = row1.IntValues[column.Offset];
          if (!intValue.HasValue)
          {
            Marshal.GetNativeVariantForObject((object) null, p);
          }
          else
          {
            switch (column.VarType)
            {
              case VarEnum.VT_I4:
                Marshal.GetNativeVariantForObject<int?>(intValue, p);
                break;
              case VarEnum.VT_BOOL:
                int? nullable = intValue;
                int num = 0;
                Marshal.GetNativeVariantForObject<bool>(!(nullable.GetValueOrDefault() == num & nullable.HasValue), p);
                break;
              default:
                throw new FormatException(ResourceStrings.Format("UnexpectedColumnType", (object) column.DataType.FullName));
            }
          }
        }
        else
          Marshal.GetNativeVariantForObject(row1.ObjValues[column.Offset], p);
        p = (IntPtr) (p.ToInt64() + (long) esz);
      }
    }

    private void InitColumns()
    {
      this.m_intCount = 0;
      this.m_objCount = 0;
      for (int index = 0; index < this.m_columns.Length; ++index)
      {
        RowSetColumn column = this.m_columns[index];
        column.Offset = column.IsValue ? this.m_intCount++ : this.m_objCount++;
      }
    }
  }
}
