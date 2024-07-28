// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Metadata.ODataMetadataWriterUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.OData.Metadata
{
  internal static class ODataMetadataWriterUtils
  {
    internal static XmlWriter CreateXmlWriter(
      Stream stream,
      ODataMessageWriterSettings messageWriterSettings,
      Encoding encoding)
    {
      XmlWriterSettings xmlWriterSettings = ODataMetadataWriterUtils.CreateXmlWriterSettings(messageWriterSettings, encoding);
      return XmlWriter.Create(stream, xmlWriterSettings);
    }

    internal static void WriteError(
      XmlWriter writer,
      ODataError error,
      bool includeDebugInformation,
      int maxInnerErrorDepth)
    {
      ErrorUtils.WriteXmlError(writer, error, includeDebugInformation, maxInnerErrorDepth);
    }

    private static XmlWriterSettings CreateXmlWriterSettings(
      ODataMessageWriterSettings messageWriterSettings,
      Encoding encoding)
    {
      return new XmlWriterSettings()
      {
        CheckCharacters = messageWriterSettings.EnableCharactersCheck,
        ConformanceLevel = ConformanceLevel.Document,
        OmitXmlDeclaration = false,
        Encoding = encoding ?? (Encoding) MediaTypeUtils.EncodingUtf8NoPreamble,
        NewLineHandling = NewLineHandling.Entitize,
        CloseOutput = false
      };
    }
  }
}
