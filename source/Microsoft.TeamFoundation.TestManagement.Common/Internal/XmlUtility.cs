// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.Internal.XmlUtility
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using System.ComponentModel;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class XmlUtility
  {
    public static XmlDocument LoadXmlDocumentFromFile(string fileName)
    {
      using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        return XmlUtility.LoadXmlDocumentFromStream((Stream) fileStream);
    }

    public static XmlDocument LoadXmlDocumentFromString(string input)
    {
      using (StringReader reader = new StringReader(input))
        return XmlUtility.LoadXmlDocumentFromTextReader((TextReader) reader);
    }

    public static XmlDocument LoadXmlDocumentFromTextReader(TextReader reader)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (XmlReader safeReader = XmlUtility.CreateSafeReader(reader))
        xmlDocument.Load(safeReader);
      return xmlDocument;
    }

    public static XmlDocument LoadXmlDocumentFromStream(Stream stream)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (XmlReader safeReader = XmlUtility.CreateSafeReader(stream))
        xmlDocument.Load(safeReader);
      return xmlDocument;
    }

    public static XmlReader CreateSafeReader(TextReader reader) => XmlReader.Create(reader, new XmlReaderSettings()
    {
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    });

    public static XmlReader CreateSafeReader(Stream stream) => XmlReader.Create(stream, new XmlReaderSettings()
    {
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    });

    public static XmlTextReader CreateSafeXmlTextReader(TextReader reader) => new XmlTextReader(reader)
    {
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    };

    public static XmlTextReader CreateSafeXmlTextReader(Stream stream) => new XmlTextReader(stream)
    {
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    };
  }
}
