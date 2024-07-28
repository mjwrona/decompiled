// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMetadataInputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Microsoft.OData
{
  internal sealed class ODataMetadataInputContext : ODataInputContext
  {
    private XmlReader baseXmlReader;
    private BufferingXmlReader xmlReader;

    public ODataMetadataInputContext(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
      : base(ODataFormat.Metadata, messageInfo, messageReaderSettings)
    {
      try
      {
        this.baseXmlReader = ODataMetadataReaderUtils.CreateXmlReader(messageInfo.MessageStream, messageInfo.Encoding, messageReaderSettings);
        this.xmlReader = new BufferingXmlReader(this.baseXmlReader, (Uri) null, messageReaderSettings.BaseUri, false, messageReaderSettings.MessageQuotas.MaxNestingDepth);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          messageInfo.MessageStream.Dispose();
        throw;
      }
    }

    internal override IEdmModel ReadMetadataDocument(
      Func<Uri, XmlReader> getReferencedModelReaderFunc)
    {
      return this.ReadMetadataDocumentImplementation(getReferencedModelReaderFunc);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        try
        {
          if (this.baseXmlReader != null)
            this.baseXmlReader.Dispose();
        }
        finally
        {
          this.baseXmlReader = (XmlReader) null;
          this.xmlReader = (BufferingXmlReader) null;
        }
      }
      base.Dispose(disposing);
    }

    private IEdmModel ReadMetadataDocumentImplementation(
      Func<Uri, XmlReader> getReferencedModelReaderFunc)
    {
      IEdmModel model;
      IEnumerable<EdmError> errors;
      if (!CsdlReader.TryParse((XmlReader) this.xmlReader, getReferencedModelReaderFunc, out model, out errors))
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (EdmError edmError in errors)
          stringBuilder.AppendLine(edmError.ToString());
        throw new ODataException(Strings.ODataMetadataInputContext_ErrorReadingMetadata((object) stringBuilder.ToString()));
      }
      return model;
    }
  }
}
