// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.XmlSerializationHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class XmlSerializationHelper
  {
    internal static T Deserialize<T>(string source) => (T) XmlSerializationHelper.Deserialize(typeof (T), source);

    internal static object Deserialize(Type objectType, string source)
    {
      using (StringReader input = new StringReader(source))
      {
        XmlReader xmlReader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        });
        return new XmlSerializer(objectType).Deserialize(xmlReader);
      }
    }

    internal static T Deserialize<T>(XmlReader xmlReader) => (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);

    internal static string ToXmlString(object obj, bool indentOutput)
    {
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        CloseOutput = false,
        OmitXmlDeclaration = true,
        Indent = indentOutput
      };
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        new XmlSerializer(obj.GetType()).Serialize(XmlWriter.Create((TextWriter) output, settings), obj);
        return output.ToString();
      }
    }
  }
}
