// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.RowSetRow
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public class RowSetRow : IXmlSerializable
  {
    private object[] m_values;
    private int?[] m_ints;
    private readonly RowSet m_rowSet;

    internal RowSetRow(RowSet rowSet) => this.m_rowSet = rowSet != null ? rowSet : throw new ArgumentNullException(nameof (rowSet));

    internal object[] ObjValues => this.m_values;

    internal int?[] IntValues => this.m_ints;

    internal object this[int index]
    {
      get
      {
        if (index < 0 || index >= this.m_rowSet.Columns.Length)
          throw new ArgumentOutOfRangeException(nameof (index));
        RowSetColumn column = this.m_rowSet.Columns[index];
        if (!column.IsValue)
          return this.m_values[column.Offset];
        int? nullable = this.m_ints[column.Offset];
        if (!nullable.HasValue)
          return (object) null;
        switch (column.VarType)
        {
          case VarEnum.VT_I4:
            return (object) nullable.Value;
          case VarEnum.VT_BOOL:
            return (object) (nullable.Value != 0);
          default:
            throw new FormatException(ResourceStrings.Format("UnexpectedColumnType", (object) column.DataType.FullName));
        }
      }
    }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    void IXmlSerializable.WriteXml(XmlWriter w) => throw new NotImplementedException(ResourceStrings.Format("WriteXmlNotImplemented", (object) this.GetType().FullName));

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      this.m_values = (object[]) null;
      this.m_ints = (int?[]) null;
      reader.ReadStartElement();
      if (this.m_rowSet.ObjCount != 0)
      {
        this.m_values = new object[this.m_rowSet.ObjCount];
        for (int index = 0; index < this.m_values.Length; ++index)
          this.m_values[index] = (object) DBNull.Value;
      }
      if (this.m_rowSet.IntCount != 0)
        this.m_ints = new int?[this.m_rowSet.IntCount];
      uint index1 = 0;
      while (reader.IsStartElement("f"))
      {
        if (reader.HasAttributes)
        {
          string attribute = reader.GetAttribute("k");
          if (!string.IsNullOrEmpty(attribute))
            index1 = uint.Parse(attribute, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        string str;
        if (reader.IsEmptyElement)
        {
          str = string.Empty;
          reader.Read();
        }
        else
          str = reader.ReadElementString();
        if (str != null)
        {
          RowSetColumn column = this.m_rowSet.Columns[(int) index1];
          if (column.IsValue)
          {
            int num;
            switch (column.VarType)
            {
              case VarEnum.VT_I4:
                num = int.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
                break;
              case VarEnum.VT_BOOL:
                num = bool.Parse(str) ? 1 : 0;
                break;
              default:
                throw new FormatException(ResourceStrings.Format("UnexpectedColumnType", (object) column.DataType.FullName));
            }
            this.m_ints[column.Offset] = new int?(num);
          }
          else
          {
            object obj;
            switch (column.VarType)
            {
              case VarEnum.VT_R8:
                obj = (object) double.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
                break;
              case VarEnum.VT_DATE:
                obj = (object) DateTime.ParseExact(str, "yyyy-MM-dd\\THH:mm:ss.fff", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
                break;
              case VarEnum.VT_BSTR:
                obj = (object) str;
                break;
              case VarEnum.VT_UI8:
                obj = (object) ulong.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
                break;
              case VarEnum.VT_CLSID:
                obj = (object) new Guid(str);
                break;
              default:
                throw new FormatException(ResourceStrings.Format("UnexpectedColumnType", (object) column.DataType.FullName));
            }
            this.m_values[column.Offset] = obj;
          }
        }
        ++index1;
      }
      reader.ReadEndElement();
    }
  }
}
