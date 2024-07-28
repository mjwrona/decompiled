// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.RowSetCollection
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  [XmlRoot("PayLoad", Namespace = "http://schemas.microsoft.com/Currituck/2005/01/mtservices/payload", IsNullable = true)]
  public class RowSetCollection : IXmlSerializable
  {
    protected List<RowSet> m_rowSetList = new List<RowSet>();
    protected Dictionary<string, RowSet> m_rowSetDictionary = new Dictionary<string, RowSet>();

    internal RowSetCollection()
    {
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("RowSetCollection instance " + this.GetHashCode().ToString());
      return stringBuilder.ToString();
    }

    public RowSet this[int index]
    {
      get
      {
        if (index < 0 || index >= this.m_rowSetList.Count)
          throw new ArgumentOutOfRangeException(nameof (index));
        return this.m_rowSetList[index];
      }
    }

    public RowSet this[string name]
    {
      get
      {
        if (string.IsNullOrEmpty(name))
          throw new ArgumentException(ResourceStrings.Format("ParameterNotNullOrEmpty", (object) nameof (name)));
        return this.m_rowSetDictionary.ContainsKey(name) ? this.m_rowSetDictionary[name] : throw new ArgumentOutOfRangeException(nameof (name));
      }
    }

    public int Count => this.m_rowSetList.Count;

    public bool TryGetRowSet(string name, out RowSet rowset) => this.m_rowSetDictionary.TryGetValue(name, out rowset);

    internal static RowSetCollection FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      RowSetCollection rowSetCollection = new RowSetCollection();
      ((IXmlSerializable) rowSetCollection).ReadXml(reader);
      return rowSetCollection;
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      writer.WriteEndElement();
    }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    void IXmlSerializable.WriteXml(XmlWriter w) => throw new NotImplementedException(ResourceStrings.Format("WriteXmlNotImplemented", (object) this.GetType().FullName));

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      this.m_rowSetList.Clear();
      this.m_rowSetDictionary.Clear();
      if (reader.IsEmptyElement)
      {
        reader.Read();
      }
      else
      {
        reader.ReadStartElement();
        while (reader.IsStartElement("table"))
        {
          RowSet rowSet = new RowSet();
          ((IXmlSerializable) rowSet).ReadXml(reader);
          this.m_rowSetList.Add(rowSet);
          this.m_rowSetDictionary.Add(rowSet.Name, rowSet);
        }
        reader.ReadEndElement();
      }
    }
  }
}
