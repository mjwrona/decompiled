// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.XmlStorageHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  internal static class XmlStorageHelper
  {
    private static XmlReaderSettings s_xmlReaderSettings;
    private static XmlWriterSettings s_xmlWriterSettings;

    internal static string ToXml(IXmlStorage xmlStorageObject)
    {
      StringBuilder output = new StringBuilder();
      using (XmlWriter writer = XmlWriter.Create(output, XmlStorageHelper.XmlWriterSettings))
        xmlStorageObject.ToXml(writer);
      return output.ToString();
    }

    internal static void FromXml(IXmlStorage xmlStorageObject, string xmlString)
    {
      if (!string.IsNullOrEmpty(xmlString))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) new StringReader(xmlString), XmlStorageHelper.XmlReaderSettings))
        {
          int content = (int) reader.MoveToContent();
          xmlStorageObject.FromXml(reader);
        }
      }
      else
        xmlStorageObject.FromXml((XmlReader) null);
    }

    private static XmlReaderSettings XmlReaderSettings
    {
      get
      {
        if (XmlStorageHelper.s_xmlReaderSettings == null)
          XmlStorageHelper.s_xmlReaderSettings = new XmlReaderSettings()
          {
            IgnoreWhitespace = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            CheckCharacters = false,
            ConformanceLevel = ConformanceLevel.Fragment,
            DtdProcessing = DtdProcessing.Prohibit,
            CloseInput = true
          };
        return XmlStorageHelper.s_xmlReaderSettings;
      }
    }

    private static XmlWriterSettings XmlWriterSettings
    {
      get
      {
        if (XmlStorageHelper.s_xmlWriterSettings == null)
          XmlStorageHelper.s_xmlWriterSettings = new XmlWriterSettings()
          {
            CheckCharacters = false,
            ConformanceLevel = ConformanceLevel.Fragment,
            NewLineHandling = NewLineHandling.None,
            OmitXmlDeclaration = true,
            CloseOutput = true
          };
        return XmlStorageHelper.s_xmlWriterSettings;
      }
    }
  }
}
