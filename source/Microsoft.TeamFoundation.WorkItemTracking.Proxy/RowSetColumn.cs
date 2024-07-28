// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.RowSetColumn
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public class RowSetColumn : IXmlSerializable
  {
    private string m_name;
    private Type m_dataType;
    private VarEnum m_varType;
    private bool m_isValue;
    private int m_offset;

    internal RowSetColumn()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Name => this.m_name;

    internal Type DataType => this.m_dataType;

    internal VarEnum VarType => this.m_varType;

    internal bool IsValue => this.m_isValue;

    internal int Offset
    {
      get => this.m_offset;
      set => this.m_offset = value;
    }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    void IXmlSerializable.WriteXml(XmlWriter w) => throw new NotImplementedException(ResourceStrings.Format("WriteXmlNotImplemented", (object) this.GetType().FullName));

    void IXmlSerializable.ReadXml(XmlReader reader)
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
        this.SetType(Type.GetType(reader.ReadString()));
        reader.ReadEndElement();
      }
      reader.ReadEndElement();
    }

    private void SetType(Type t)
    {
      this.m_dataType = t;
      if (t.Equals(typeof (int)))
        this.m_varType = VarEnum.VT_I4;
      else if (t.Equals(typeof (bool)))
        this.m_varType = VarEnum.VT_BOOL;
      else if (t.Equals(typeof (string)))
        this.m_varType = VarEnum.VT_BSTR;
      else if (t.Equals(typeof (DateTime)))
        this.m_varType = VarEnum.VT_DATE;
      else if (t.Equals(typeof (ulong)))
        this.m_varType = VarEnum.VT_UI8;
      else if (t.Equals(typeof (double)))
        this.m_varType = VarEnum.VT_R8;
      else if (t.Equals(typeof (Guid)))
        this.m_varType = VarEnum.VT_CLSID;
      else
        throw new FormatException(ResourceStrings.Format("UnexpectedColumnType", (object) t.FullName));
      this.m_isValue = this.m_varType == VarEnum.VT_I4 || this.m_varType == VarEnum.VT_BOOL;
    }
  }
}
