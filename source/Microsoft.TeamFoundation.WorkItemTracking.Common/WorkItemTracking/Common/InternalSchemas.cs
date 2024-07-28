// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.InternalSchemas
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class InternalSchemas
  {
    public static XmlSchema GetSchema(InternalSchemaType type)
    {
      using (IEnumerator<XmlSchema> enumerator = InternalSchemas.GetSchemas(type).GetEnumerator())
      {
        if (enumerator.MoveNext())
          return enumerator.Current;
      }
      return (XmlSchema) null;
    }

    public static IEnumerable<XmlSchema> GetSchemas(InternalSchemaType type)
    {
      switch (type)
      {
        case InternalSchemaType.WorkItemType:
          yield return InternalSchemas.ReadSchema("WorkItemTypeDefinition02.xsd");
          yield return InternalSchemas.ReadSchema("typelib02.xsd");
          yield return InternalSchemas.ReadSchema("WorkItemTypeDefinition01.xsd");
          yield return InternalSchemas.ReadSchema("typelib01.xsd");
          break;
        case InternalSchemaType.GlobalLists:
          yield return InternalSchemas.ReadSchema("globallists01.xsd");
          yield return InternalSchemas.ReadSchema("typelib01.xsd");
          break;
        case InternalSchemaType.WorkItemLinkType:
          yield return InternalSchemas.ReadSchema("WorkItemLinkTypeDefinition01.xsd");
          yield return InternalSchemas.ReadSchema("typelib01.xsd");
          break;
        case InternalSchemaType.Categories:
          yield return InternalSchemas.ReadSchema("categories01.xsd");
          yield return InternalSchemas.ReadSchema("typelib01.xsd");
          break;
        case InternalSchemaType.GlobalWorkflow:
          yield return InternalSchemas.ReadSchema("gworkflow01.xsd");
          yield return InternalSchemas.ReadSchema("typelib02.xsd");
          break;
        default:
          yield return InternalSchemas.ReadSchema("typelib02.xsd");
          yield return InternalSchemas.ReadSchema("typelib01.xsd");
          break;
      }
    }

    private static XmlSchema ReadSchema(string name)
    {
      Assembly assembly = typeof (InternalSchemas).Assembly;
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      string name1 = name;
      using (Stream manifestResourceStream = assembly.GetManifestResourceStream(name1))
        return XmlSchema.Read(XmlReader.Create(manifestResourceStream, settings), (ValidationEventHandler) null);
    }

    public static void InitSchemaSet(InternalSchemaType type, XmlSchemaSet schemas)
    {
      List<XmlSchema> xmlSchemaList = new List<XmlSchema>(InternalSchemas.GetSchemas(type));
      schemas.Add(InternalSchemas.RemoveNamespace(xmlSchemaList[0]));
      foreach (XmlSchema schema in xmlSchemaList)
        schemas.Add(schema);
    }

    private static XmlSchema RemoveNamespace(XmlSchema schema)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (MemoryStream input = new MemoryStream())
      {
        schema.Write((Stream) input);
        input.Flush();
        input.Seek(0L, SeekOrigin.Begin);
        schema = XmlSchema.Read(XmlReader.Create((Stream) input, settings), (ValidationEventHandler) null);
        schema.TargetNamespace = (string) null;
        return schema;
      }
    }
  }
}
