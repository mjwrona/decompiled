// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.Schema
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.TeamFoundation.TestManagement.Common
{
  internal class Schema
  {
    internal static Stream GetSchema(string streamName) => Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);

    internal static XmlSchemaSet GetSchemaSet(string streamName)
    {
      XmlSchemaSet schemaSet = new XmlSchemaSet();
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (Stream schema1 = Microsoft.TeamFoundation.TestManagement.Common.Schema.GetSchema(streamName))
      {
        XmlSchema schema2 = XmlSchema.Read(XmlReader.Create(schema1, settings), (ValidationEventHandler) null);
        schemaSet.Add(schema2);
      }
      return schemaSet;
    }

    internal static class SchemaFileNames
    {
      internal const string TestVariable = "testvariable.xsd";
      internal const string TestConfiguration = "testconfiguration.xsd";
      internal const string TestSettings = "testsettings.xsd";
      internal const string TestResolutionState = "testresolutionstate.xsd";
      internal const string BugFieldMapping = "bugfieldmapping.xsd";
      internal const string TestFailureType = "testfailuretype.xsd";
    }
  }
}
