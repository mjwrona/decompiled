// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ProcessTemplateSchema
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.TeamFoundation.Common
{
  internal class ProcessTemplateSchema
  {
    internal static string[] SchemaNames = new string[9]
    {
      "ProcessTemplate.xsd",
      "ProcessTemplateItem.xsd",
      "Tasks.xsd",
      "Rosetta.xsd",
      "Css.xsd",
      "Gss.xsd",
      "Wss.xsd",
      "PEPluginData.xsd",
      "CssProperties.xsd"
    };

    internal static Stream GetSchema(ProcessTemplateSchema.Schema schemaType) => Assembly.GetExecutingAssembly().GetManifestResourceStream(ProcessTemplateSchema.SchemaNames[(int) schemaType]);

    internal static XmlSchemaSet GetSchemaSet(ProcessTemplateSchema.Schema schemaType)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      XmlSchemaSet schemaSet = new XmlSchemaSet();
      using (Stream schema1 = ProcessTemplateSchema.GetSchema(schemaType))
      {
        XmlSchema schema2 = XmlSchema.Read(XmlReader.Create(schema1, settings), (ValidationEventHandler) null);
        schemaSet.Add(schema2);
      }
      return schemaSet;
    }

    internal static void Validate(
      XmlNode baseNode,
      ProcessTemplateSchema.Schema type,
      string baseUri)
    {
      using (Stream schema1 = ProcessTemplateSchema.GetSchema(type))
      {
        XmlSchema schema2 = schema1 != null ? XmlSchema.Read(schema1, (ValidationEventHandler) null) : throw new InvalidOperationException(TFCommonResources.XML_SCHEMAVALIDATION_READSCHEMAFAILED());
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.XmlResolver = (XmlResolver) null;
        settings.DtdProcessing = DtdProcessing.Prohibit;
        settings.Schemas.Add(schema2);
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationEventHandler += new ValidationEventHandler(ProcessTemplateSchema.ValidationHandler);
        try
        {
          XmlReader xmlReader = XmlReader.Create((XmlReader) new XmlNodeReader(baseNode), settings);
          do
            ;
          while (xmlReader.Read());
        }
        catch (Exception ex)
        {
          throw new XmlSchemaValidationException(TFCommonResources.XML_SCHEMAVALIDATION_FAILED((object) baseUri, (object) ex.Message));
        }
      }
    }

    private static void ValidationHandler(object sender, ValidationEventArgs args)
    {
      if (args.Severity == XmlSeverityType.Error)
        throw args.Exception;
    }

    internal enum Schema
    {
      Methodology,
      MethodologyItem,
      Tasks,
      Rosetta,
      Css,
      Gss,
      Wss,
      PEPluginData,
      CssProperties,
    }
  }
}
