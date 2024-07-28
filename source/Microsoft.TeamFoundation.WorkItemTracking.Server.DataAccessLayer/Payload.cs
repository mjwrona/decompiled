// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Payload
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.MetadataProcessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [XmlRoot("PayLoad", Namespace = "http://schemas.microsoft.com/Currituck/2005/01/mtservices/payload", IsNullable = true)]
  [ClientType("RowSetCollection")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  [CustomSerializationHooks]
  [ClassNotSealed]
  [Serializable]
  public class Payload : IXmlSerializable
  {
    private PayloadTableCollection m_tables;
    private IDictionary<int, IPayloadTableSchema> m_tableSchemas;
    private string m_preserializedData;
    private bool m_preserializedDataLoaded;
    internal Payload.SqlExceptionCallback SqlExceptionHandler;
    internal Payload.SqlTypeExceptionCallback SqlTypeExeptionHandler;

    internal IPayloadTableSchema GetTableSchema(int index)
    {
      IPayloadTableSchema payloadTableSchema;
      return this.m_tableSchemas != null && this.m_tableSchemas.TryGetValue(index, out payloadTableSchema) ? payloadTableSchema : (IPayloadTableSchema) null;
    }

    internal SqlAccess SqlAccess { get; set; }

    public Payload()
      : this((IDictionary<int, IPayloadTableSchema>) null, (PayloadConverter) null, int.MaxValue)
    {
    }

    internal PayloadConverter Converter { get; private set; }

    internal IList<PayloadProcessor> PayloadProcessors { get; private set; }

    internal Payload(PayloadConverter converter)
      : this((IDictionary<int, IPayloadTableSchema>) null, converter, int.MaxValue)
    {
    }

    internal Payload(IDictionary<int, IPayloadTableSchema> tableSchemas)
      : this(tableSchemas, (PayloadConverter) null, int.MaxValue)
    {
    }

    internal Payload(
      IDictionary<int, IPayloadTableSchema> tableSchemas,
      PayloadConverter converter,
      int inMemoryTableCount)
    {
      this.Converter = converter;
      this.m_tables = new PayloadTableCollection(this);
      this.InMemoryTableCount = inMemoryTableCount;
      this.m_tableSchemas = tableSchemas;
      this.PayloadProcessors = (IList<PayloadProcessor>) new List<PayloadProcessor>();
    }

    internal Payload(string preserializedData)
      : this()
    {
      this.m_preserializedData = preserializedData;
      this.m_preserializedDataLoaded = false;
    }

    internal int InMemoryTableCount { private set; get; }

    [XmlIgnore]
    public int TableCount => this.Tables.Count;

    [XmlIgnore]
    public PayloadTableCollection Tables
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(this.m_preserializedData) && !this.m_preserializedDataLoaded)
        {
          using (StringReader stringReader = new StringReader(this.m_preserializedData))
          {
            StringReader input = stringReader;
            using (XmlReader reader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
            {
              IgnoreProcessingInstructions = true,
              DtdProcessing = DtdProcessing.Prohibit,
              XmlResolver = (XmlResolver) null,
              ConformanceLevel = ConformanceLevel.Fragment
            }))
            {
              this.ReadXml(reader);
              this.m_preserializedDataLoaded = true;
            }
          }
        }
        return this.m_tables;
      }
    }

    [XmlIgnore]
    public bool? XmlDeserialized => string.IsNullOrWhiteSpace(this.m_preserializedData) ? new bool?() : new bool?(this.m_preserializedDataLoaded);

    public bool Fill(IDataReader sourceReader) => this.Fill(sourceReader, int.MaxValue);

    public bool Fill(IDataReader sourceReader, int expectedTableCount)
    {
      if (sourceReader == null)
        throw new ArgumentNullException(nameof (sourceReader));
      this.m_tables.Populate(sourceReader, this.Converter, expectedTableCount, this.InMemoryTableCount);
      return this.InMemoryTableCount == 0 || this.InMemoryTableCount == int.MaxValue;
    }

    public string GetXml() => "To be implemented";

    private string TrimWrappingTag(string serializedPayload) => serializedPayload.Substring(35, serializedPayload.Length - 35 - 15);

    public void WriteCacheXml(XmlWriter writer)
    {
      writer.WriteStartElement("PayloadCache");
      this.WriteXml(writer);
      writer.WriteEndElement();
    }

    internal void SetPreserializedData(string preserializedData)
    {
      this.m_preserializedData = preserializedData;
      this.m_preserializedDataLoaded = true;
    }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public static XmlQualifiedName PayloadSchema(XmlSchemaSet xss) => (XmlQualifiedName) null;

    public void WriteXml(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      writer.WriteAttributeString("xml", "space", (string) null, "preserve");
      if (!string.IsNullOrEmpty(this.m_preserializedData))
      {
        writer.WriteRaw(this.TrimWrappingTag(this.m_preserializedData));
      }
      else
      {
        try
        {
          this.m_tables.WriteXml(writer);
        }
        catch (Exception ex)
        {
          if (this.SqlExceptionHandler != null && ex is SqlException)
            this.SqlExceptionHandler(ex as SqlException);
          else if (this.SqlTypeExeptionHandler != null && ex is SqlTypeException)
            this.SqlTypeExeptionHandler(ex as SqlTypeException);
          else
            throw;
        }
      }
    }

    public void ReadXml(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      this.m_tables.ReadXml(reader);
    }

    public delegate void SqlExceptionCallback(SqlException ex);

    public delegate void SqlTypeExceptionCallback(SqlTypeException typeException);
  }
}
